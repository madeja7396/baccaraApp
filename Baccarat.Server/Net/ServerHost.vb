Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Rules
Imports Baccarat.Shared.Util

Namespace Net
    ''' <summary>
    ''' サーバー側のメイン制御クラス
    ''' </summary>
    Public Class ServerHost
        Private ReadOnly _logger As Logger
        Private ReadOnly _rules As IBaccaratRules
        Private ReadOnly _payout As IPayoutCalculator
        Private ReadOnly _state As GameState = New GameState()
        Private ReadOnly _clients As New List(Of Long)()
        Private ReadOnly _sendAction As Action(Of Long, String)
        Private ReadOnly _closeAction As Action(Of Long)
        Private ReadOnly _rng As New Random()
        Private _betStartTime As DateTime = DateTime.MinValue
        Private Const BET_TIMEOUT_SEC As Integer = 30

        Public Sub New(logger As Logger, rules As IBaccaratRules, payout As IPayoutCalculator, Optional sendAction As Action(Of Long, String) = Nothing, Optional closeAction As Action(Of Long) = Nothing)
            _logger = logger
            _rules = rules
            _payout = payout
            _sendAction = sendAction
            _closeAction = closeAction
        End Sub

        Public Sub StartServer(port As Integer)
            _logger.Info($"Server start on {port}")
            BuildAndShuffleShoe()
        End Sub

        Public Sub StopServer()
            _logger.Info("Server stop")
        End Sub

        Public Sub OnAccept(handle As Long)
            _logger.Info($"Accept handle={handle}")
            ' 満席（2名）なら拒否
            If _clients.Count >= 2 Then
                SendTo(handle, $"{CommandNames.ERROR},ROOM_FULL")
                If _closeAction IsNot Nothing Then _closeAction.Invoke(handle)
                Return
            End If
            If Not _clients.Contains(handle) Then
                _clients.Add(handle)
            End If
        End Sub

        Public Sub OnDisconnect(handle As Long)
            _logger.Info($"Disconnect handle={handle}")
            If _clients.Contains(handle) Then
                _clients.Remove(handle)
            End If
            ' detach from state.Clients if assigned
            For i = 0 To _state.Clients.Length - 1
                Dim ci = _state.Clients(i)
                If ci IsNot Nothing AndAlso ci.Handle = handle Then
                    _state.Clients(i) = Nothing
                End If
            Next
        End Sub

        Public Sub OnLineReceived(handle As Long, line As String)
            _logger.Receive(handle.ToString(), line)
            Dim msg As Message = Nothing
            If Not Parser.TryParse(line, msg) Then
                _logger.Error("Bad format")
                SendTo(handle, $"{CommandNames.ERROR},{BetRejectReasons.BAD_ARGS}")
                Return
            End If

            Select Case msg.Command
                Case CommandNames.HELLO : HandleHello(handle, msg)
                Case CommandNames.READY : HandleReady(handle)
                Case CommandNames.BET : HandleBet(handle, msg)
                Case Else
                    _logger.Error($"Unsupported command {msg.Command}")
                    SendTo(handle, $"{CommandNames.ERROR},UNSUPPORTED")
            End Select
        End Sub

        Private Sub HandleHello(handle As Long, msg As Message)
            Dim nickname As String = If(msg.Params IsNot Nothing AndAlso msg.Params.Length > 0, msg.Params(0), "")
            If String.IsNullOrWhiteSpace(nickname) OrElse nickname.Length > Baccarat.Shared.Constants.NicknameMaxLen Then
                SendTo(handle, $"{CommandNames.ERROR},INVALID_NAME")
                Return
            End If

            ' already assigned?
            For i = 0 To _state.Clients.Length - 1
                Dim ci = _state.Clients(i)
                If ci IsNot Nothing AndAlso ci.Handle = handle Then
                    ' re-HELLO: refresh nickname
                    ci.Nickname = nickname
                    SendWelcome(handle, ci.PlayerId)
                    Return
                End If
            Next

            ' assign playerId 1..2
            Dim pid As Integer = 0
            If _state.Clients(0) Is Nothing Then
                pid = 1
                _state.Clients(0) = New ClientInfo With {.Handle = handle, .PlayerId = pid, .Nickname = nickname}
            ElseIf _state.Clients(1) Is Nothing Then
                pid = 2
                _state.Clients(1) = New ClientInfo With {.Handle = handle, .PlayerId = pid, .Nickname = nickname}
            Else
                SendTo(handle, $"{CommandNames.ERROR},ROOM_FULL")
                If _closeAction IsNot Nothing Then _closeAction.Invoke(handle)
                Return
            End If

            SendWelcome(handle, pid)
        End Sub

        Private Sub SendWelcome(handle As Long, playerId As Integer)
            Dim line = $"{CommandNames.WELCOME},{playerId},0,{Baccarat.Shared.Constants.MaxRounds},{Baccarat.Shared.Constants.InitChips}"
            SendTo(handle, line)
        End Sub

        Private Sub HandleReady(handle As Long)
            ' set ready flag
            For i = 0 To _state.Clients.Length - 1
                Dim ci = _state.Clients(i)
                If ci IsNot Nothing AndAlso ci.Handle = handle Then
                    ci.IsReady = True
                End If
            Next

            ' if both players ready, advance to BETTING and notify
            Dim c1 = _state.Clients(0)
            Dim c2 = _state.Clients(1)
            If c1 IsNot Nothing AndAlso c2 IsNot Nothing AndAlso c1.IsReady AndAlso c2.IsReady Then
                _state.Phase = GamePhase.BETTING
                If _state.RoundIndex <= 0 Then _state.RoundIndex = 1
                _betStartTime = DateTime.Now
                Dim line = $"{CommandNames.PHASE},{GamePhase.BETTING},{_state.RoundIndex}"
                Broadcast(line)
            End If
        End Sub

        Private Sub HandleBet(handle As Long, msg As Message)
            ' Check BET timeout
            If _state.Phase = GamePhase.BETTING AndAlso DateTime.Now.Subtract(_betStartTime).TotalSeconds > BET_TIMEOUT_SEC Then
                _logger.Info($"[TIMEOUT] BET phase exceeded {BET_TIMEOUT_SEC}s. Auto-settling...")
                SettleRound()
                Return
            End If

            ' Validate phase
            If _state.Phase <> GamePhase.BETTING Then
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.PHASE_MISMATCH}")
                Return
            End If

            ' Parse BET: BET,playerId,target,amount もしくは BET,target,amount
            Dim pId As Integer = FindPlayerIdByHandle(handle)
            Dim targetStr As String = Nothing
            Dim amountStr As String = Nothing

            If msg.Params Is Nothing OrElse msg.Params.Length < 2 Then
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.BAD_ARGS}")
                Return
            End If

            If msg.Params.Length = 3 Then
                ' playerId,target,amount
                If Not Integer.TryParse(msg.Params(0), pId) Then
                    SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.BAD_PLAYER}")
                    Return
                End If
                targetStr = msg.Params(1)
                amountStr = msg.Params(2)
            ElseIf msg.Params.Length = 2 Then
                targetStr = msg.Params(0)
                amountStr = msg.Params(1)
            Else
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.BAD_ARGS}")
                Return
            End If

            If pId <> 1 AndAlso pId <> 2 Then
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.BAD_PLAYER}")
                Return
            End If

            Dim target As BetTarget
            Select Case targetStr.Trim().ToUpperInvariant()
                Case "PLAYER" : target = BetTarget.Player
                Case "BANKER" : target = BetTarget.Banker
                Case "TIE" : target = BetTarget.Tie
                Case Else
                    SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.BAD_TARGET}")
                    Return
            End Select

            Dim amount As Integer
            If Not Integer.TryParse(amountStr, amount) OrElse amount <= 0 Then
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.BAD_AMOUNT}")
                Return
            End If

            Dim chips As Integer = 0
            If Not _state.Chips.TryGetValue(pId, chips) OrElse amount > chips Then
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.NO_CHIPS}")
                Return
            End If

            ' Prevent re-bet
            Dim existing As BetInfo = Nothing
            If _state.Bets.TryGetValue(pId, existing) AndAlso existing IsNot Nothing AndAlso existing.Locked Then
                SendTo(handle, $"{CommandNames.BET_ACK},false,{BetRejectReasons.ALREADY_LOCKED}")
                Return
            End If

            ' Accept bet
            Dim bet = New BetInfo With {.Target = target, .Amount = amount, .Locked = True}
            _state.Bets(pId) = bet
            SendTo(handle, $"{CommandNames.BET_ACK},true")

            ' If both players have locked bets, settle
            If IsBothBetsLocked() Then
                SettleRound()
            End If
        End Sub

        Private Function IsBothBetsLocked() As Boolean
            Dim b1 As BetInfo = Nothing, b2 As BetInfo = Nothing
            If Not _state.Bets.TryGetValue(1, b1) Then Return False
            If Not _state.Bets.TryGetValue(2, b2) Then Return False
            Return b1 IsNot Nothing AndAlso b1.Locked AndAlso b2 IsNot Nothing AndAlso b2.Locked
        End Function

        Private Sub SettleRound()
            ' DEALING → RESULT
            _state.Phase = GamePhase.DEALING
            Broadcast($"{CommandNames.PHASE},{GamePhase.DEALING},{_state.RoundIndex}")

            ' Ensure shoe and deal
            EnsureShoe()
            _rules.DealInitial(_state)
            _rules.ApplyThirdCardRule(_state)

            ' Broadcast DEAL with card codes（P1,P2[,P3],B1,B2[,B3]）
            Dim playerCodes = String.Join("|", _state.PlayerHand.Cards.Select(Function(c) c.ToCode()))
            Dim bankerCodes = String.Join("|", _state.BankerHand.Cards.Select(Function(c) c.ToCode()))
            Broadcast($"{CommandNames.DEAL},{playerCodes},{bankerCodes}")

            ' Determine winner
            Dim winner = _rules.DetermineWinner(_state)

            ' Compute payouts
            Dim p1delta As Integer = 0
            Dim p2delta As Integer = 0
            Dim b1 As BetInfo = Nothing, b2 As BetInfo = Nothing
            _state.Bets.TryGetValue(1, b1)
            _state.Bets.TryGetValue(2, b2)
            If b1 IsNot Nothing Then p1delta = _payout.CalcPayout(b1.Target, b1.Amount, winner)
            If b2 IsNot Nothing Then p2delta = _payout.CalcPayout(b2.Target, b2.Amount, winner)

            _state.Chips(1) += p1delta
            _state.Chips(2) += p2delta

            ' RESULT
            _state.Phase = GamePhase.RESULT
            Broadcast($"{CommandNames.PHASE},{GamePhase.RESULT},{_state.RoundIndex}")
            Broadcast($"{CommandNames.ROUND_RESULT},{winner},{p1delta},{p2delta},{_state.Chips(1)},{_state.Chips(2)}")

            ' GAME_OVER 判定
            If IsGameOver() Then
                Dim winId As Integer = If(_state.Chips(1) > _state.Chips(2), 1, If(_state.Chips(2) > _state.Chips(1), 2, 0))
                _state.Phase = GamePhase.GAMEOVER
                Broadcast($"{CommandNames.GAME_OVER},{winId},{_state.Chips(1)},{_state.Chips(2)}")
                Return
            End If

            ' Prepare next round: reset ready & bets, increment round, move to BETTING
            _state.Bets.Clear()
            For i = 0 To _state.Clients.Length - 1
                If _state.Clients(i) IsNot Nothing Then _state.Clients(i).IsReady = False
            Next
            _state.RoundIndex += 1
            _state.Phase = GamePhase.BETTING
            Broadcast($"{CommandNames.PHASE},{GamePhase.BETTING},{_state.RoundIndex}")
        End Sub

        Private Sub EnsureShoe()
            ' シンプル運用：毎ラウンド冒頭で新規に山札を用意（DeckCount デック分）
            If _state.Shoe Is Nothing OrElse _state.Shoe.Count < 6 Then
                BuildAndShuffleShoe()
            End If
        End Sub

        Private Sub BuildAndShuffleShoe()
            _state.Shoe.Clear()
            Dim deckCount = Baccarat.Shared.Constants.DeckCount
            Dim suits As Char() = {"S"c, "H"c, "D"c, "C"c}
            For d = 1 To deckCount
                For Each s In suits
                    For r = 1 To 13
                        _state.Shoe.Add(New Card(s, r))
                    Next
                Next
            Next
            ' Shuffle
            For i = _state.Shoe.Count - 1 To 1 Step -1
                Dim j = _rng.Next(0, i + 1)
                Dim tmp = _state.Shoe(i)
                _state.Shoe(i) = _state.Shoe(j)
                _state.Shoe(j) = tmp
            Next
        End Sub

        Private Function IsGameOver() As Boolean
            ' ラウンド上限 or どちらかのチップ枯渇
            If _state.RoundIndex >= Baccarat.Shared.Constants.MaxRounds Then Return True
            If _state.Chips(1) <= 0 OrElse _state.Chips(2) <= 0 Then Return True
            Return False
        End Function

        Private Function FindPlayerIdByHandle(handle As Long) As Integer
            For i = 0 To _state.Clients.Length - 1
                Dim ci = _state.Clients(i)
                If ci IsNot Nothing AndAlso ci.Handle = handle Then
                    Return ci.PlayerId
                End If
            Next
            Return 0
        End Function

        Public Sub SendTo(handle As Long, message As String)
            If _sendAction Is Nothing Then
                _logger.Error($"SendTo called but no sendAction configured for handle={handle}")
                Return
            End If
            Try
                _logger.Send(handle.ToString(), message)
                _sendAction.Invoke(handle, message)
            Catch ex As Exception
                _logger.Error($"SendTo failed: {ex.Message}")
            End Try
        End Sub

        Public Sub Broadcast(message As String)
            For Each h In _clients.ToArray()
                SendTo(h, message)
            Next
        End Sub

    End Class
End Namespace
