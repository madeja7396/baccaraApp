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

        Public Sub New(logger As Logger, rules As IBaccaratRules, payout As IPayoutCalculator, Optional sendAction As Action(Of Long, String) = Nothing)
            _logger = logger
            _rules = rules
            _payout = payout
            _sendAction = sendAction
        End Sub

        Public Sub StartServer(port As Integer)
            _logger.Info($"Server start on {port}")
        End Sub

        Public Sub StopServer()
            _logger.Info("Server stop")
        End Sub

        Public Sub OnAccept(handle As Long)
            _logger.Info($"Accept handle={handle}")
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
                SendTo(handle, $"{CommandNames.ERROR},BAD_FORMAT")
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
                ' room full (予定)
                SendTo(handle, $"{CommandNames.ERROR},ROOM_FULL")
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
                Dim line = $"{CommandNames.PHASE},{GamePhase.BETTING},{_state.RoundIndex}"
                Broadcast(line)
            End If
        End Sub

        Private Sub HandleBet(handle As Long, msg As Message)
            ' TODO: validate phase and bet amount
        End Sub

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
