Imports Experiment.TcpSocket
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util
Imports Baccarat.Client.Model
Imports System.Text
Imports SharedConstants = Baccarat.Shared.Constants

Namespace Forms
    Partial Public Class FormLobby
        Private WithEvents _tcp As TcpSockets
        Private _handle As Long = -1
        Private _framer As LineFramer
        Private _gameState As ClientGameState

        Private Sub OnFormLoad(sender As Object, e As EventArgs) Handles MyBase.Load
            _tcp = New TcpSockets()
            _tcp.SynchronizingObject = Me
            _framer = New LineFramer()
            _gameState = New ClientGameState()
            AddHandler _framer.LineReceived, AddressOf OnLineReceived

            AddHandler btnConnect.Click, AddressOf OnBtnConnect
        End Sub

        Private Sub OnBtnConnect(sender As Object, e As EventArgs)
            Dim ip = txtIp.Text.Trim()
            Dim nick = txtNickname.Text.Trim()
            If String.IsNullOrWhiteSpace(nick) Then
                AppendLog("Nickname is required.")
                Return
            End If
            Try
                _handle = _tcp.OpenAsClient(ip, SharedConstants.Port)
                AppendLog($"Connecting to {ip}:{SharedConstants.Port} ...")
            Catch ex As Exception
                AppendLog($"Connect error: {ex.Message}")
            End Try
        End Sub

        Private Sub _tcp_Connect(sender As Object, e As ConnectEventArgs) Handles _tcp.Connect
            AppendLog($"[CONNECT] {e.RemoteEndPoint}")
            Dim line As String = $"{CommandNames.HELLO},{txtNickname.Text.Trim()}"
            Dim data As Byte() = Encoding.UTF8.GetBytes(line & vbLf)
            _tcp.Send(e.Handle, data)
        End Sub

        Private Sub _tcp_DataReceive(sender As Object, e As DataReceiveEventArgs) Handles _tcp.DataReceive
            Dim text As String = Encoding.UTF8.GetString(e.Data)
            _framer.Push(text)
        End Sub

        Private Sub _tcp_Disconnect(sender As Object, e As DisconnectEventArgs) Handles _tcp.Disconnect
            AppendLog($"[DISCONNECT] {e.RemoteEndPoint}")
            _handle = -1
        End Sub

        Private Sub OnLineReceived(line As String)
            AppendLog($"[SRV] {line}")
            ParseMessage(line)
        End Sub

        Private Sub ParseMessage(line As String)
            Dim parts = line.Split(","c)
            If parts.Length = 0 Then Return

            Select Case parts(0).Trim()
                Case CommandNames.WELCOME
                    HandleWelcome(parts)
                Case CommandNames.PHASE
                    HandlePhase(parts)
                Case CommandNames.BET_ACK
                    HandleBetAck(parts)
                Case CommandNames.DEAL
                    HandleDeal(parts)
                Case CommandNames.ROUND_RESULT
                    HandleRoundResult(parts)
                Case CommandNames.GAME_OVER
                    HandleGameOver(parts)
                Case CommandNames.ERROR
                    HandleError(parts)
            End Select
        End Sub

        Private Sub HandleWelcome(parts As String())
            ' WELCOME,playerId,seed,maxRounds,initChips
            If parts.Length >= 5 Then
                If Integer.TryParse(parts(1), _gameState.PlayerId) AndAlso
                   Integer.TryParse(parts(3), _gameState.MaxRounds) AndAlso
                   Integer.TryParse(parts(4), _gameState.ChipsP1) Then
                    _gameState.ChipsP2 = _gameState.ChipsP1
                    AppendLog($"[WELCOME] playerId={_gameState.PlayerId}, maxRounds={_gameState.MaxRounds}, initChips={_gameState.ChipsP1}")
                End If
            End If
        End Sub

        Private Sub HandlePhase(parts As String())
            ' PHASE,phase,round
            If parts.Length >= 3 Then
                If Integer.TryParse(parts(2), _gameState.RoundIndex) Then
                    ' phase 文字列をパース（BETTING, DEALING, RESULT, GAMEOVER）
                    Select Case parts(1).Trim()
                        Case "BETTING"
                            _gameState.Phase = GamePhase.BETTING
                        Case "DEALING"
                            _gameState.Phase = GamePhase.DEALING
                        Case "RESULT"
                            _gameState.Phase = GamePhase.RESULT
                        Case "GAMEOVER"
                            _gameState.Phase = GamePhase.GAMEOVER
                    End Select
                    AppendLog($"[PHASE] {_gameState.Phase}, round={_gameState.RoundIndex}")
                End If
            End If
        End Sub

        Private Sub HandleBetAck(parts As String())
            ' BET_ACK,ok[,reason]
            If parts.Length >= 2 Then
                Dim ok = parts(1).Trim().ToLower() = "true"
                Dim reason = If(parts.Length > 2, parts(2).Trim(), "")
                If ok Then
                    AppendLog("[BET_ACK] Accepted")
                Else
                    AppendLog($"[BET_ACK] Rejected: {reason}")
                End If
            End If
        End Sub

        Private Sub HandleDeal(parts As String())
            ' DEAL,playerCodes,bankerCodes
            If parts.Length >= 3 Then
                _gameState.LastPlayerCards = parts(1).Trim()
                _gameState.LastBankerCards = parts(2).Trim()
                AppendLog($"[DEAL] Player: {_gameState.LastPlayerCards}, Banker: {_gameState.LastBankerCards}")
            End If
        End Sub

        Private Sub HandleRoundResult(parts As String())
            ' ROUND_RESULT,winner,payoutP1,payoutP2,chipsP1,chipsP2
            If parts.Length >= 6 Then
                Dim winner = parts(1).Trim()
                If Integer.TryParse(parts(2), _gameState.LastPayoutP1) AndAlso
                   Integer.TryParse(parts(3), _gameState.LastPayoutP2) AndAlso
                   Integer.TryParse(parts(4), _gameState.ChipsP1) AndAlso
                   Integer.TryParse(parts(5), _gameState.ChipsP2) Then
                    AppendLog($"[RESULT] Winner={winner}, Payout P1={_gameState.LastPayoutP1}, P2={_gameState.LastPayoutP2}, Chips P1={_gameState.ChipsP1}, P2={_gameState.ChipsP2}")
                End If
            End If
        End Sub

        Private Sub HandleGameOver(parts As String())
            ' GAME_OVER,winPlayerId,chipsP1,chipsP2
            If parts.Length >= 4 Then
                _gameState.Phase = GamePhase.GAMEOVER
                Dim winId = parts(1).Trim()
                If Integer.TryParse(parts(2), _gameState.ChipsP1) AndAlso
                   Integer.TryParse(parts(3), _gameState.ChipsP2) Then
                    AppendLog($"[GAME_OVER] Winner=Player{winId}, Final Chips P1={_gameState.ChipsP1}, P2={_gameState.ChipsP2}")
                End If
            End If
        End Sub

        Private Sub HandleError(parts As String())
            ' ERROR,reason
            If parts.Length >= 2 Then
                Dim reason = parts(1).Trim()
                AppendLog($"[ERROR] {reason}")
            End If
        End Sub
    End Class
End Namespace