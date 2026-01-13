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

        Private _gameForm As FormGame
        ' Žè“®ŒŸØ’†‚ÍŽ©“®BET‚ðØ‚é
        Private _disableAutoBet As Boolean = True

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
                UiLog("Nickname is required.")
                Return
            End If
            Try
                _handle = _tcp.OpenAsClient(ip, SharedConstants.Port)
                UiLog($"Connecting to {ip}:{SharedConstants.Port} ...")
            Catch ex As Exception
                UiLog($"Connect error: {ex.Message}")
            End Try
        End Sub

        Private Sub _tcp_Connect(sender As Object, e As ConnectEventArgs) Handles _tcp.Connect
            UiLog($"[CONNECT] {e.RemoteEndPoint}")
            SendLine($"{CommandNames.HELLO},{txtNickname.Text.Trim()}")
        End Sub

        Private Sub _tcp_DataReceive(sender As Object, e As DataReceiveEventArgs) Handles _tcp.DataReceive
            Dim text As String = Encoding.UTF8.GetString(e.Data)
            _framer.Push(text)
        End Sub

        Private Sub _tcp_Disconnect(sender As Object, e As DisconnectEventArgs) Handles _tcp.Disconnect
            UiLog($"[DISCONNECT]")
            _handle = -1
        End Sub

        Private Sub OnLineReceived(line As String)
            UiLog($"[SRV] {line}")
            ParseMessage(line)
        End Sub

        Private Sub ParseMessage(line As String)
            Dim parts = line.Split(","c)
            If parts.Length = 0 Then Return

            Select Case parts(0).Trim()
                Case CommandNames.WELCOME
                    If parts.Length >= 5 Then
                        Integer.TryParse(parts(1), _gameState.PlayerId)
                        Integer.TryParse(parts(3), _gameState.MaxRounds)
                        Integer.TryParse(parts(4), _gameState.ChipsP1)
                        _gameState.ChipsP2 = _gameState.ChipsP1
                        UiLog($"[WELCOME] playerId={_gameState.PlayerId}")
                        ' Auto send READY to simplify flow
                        SendLine(CommandNames.READY)
                        UiLog("[SEND] READY (auto)")
                    End If
                Case CommandNames.PHASE
                    If parts.Length >= 3 Then
                        Integer.TryParse(parts(2), _gameState.RoundIndex)
                        Dim phaseStr = parts(1).Trim()
                        Select Case phaseStr
                            Case "BETTING" : _gameState.Phase = GamePhase.BETTING
                            Case "DEALING" : _gameState.Phase = GamePhase.DEALING
                            Case "RESULT" : _gameState.Phase = GamePhase.RESULT
                            Case "GAMEOVER" : _gameState.Phase = GamePhase.GAMEOVER
                        End Select
                        UiLog($"[PHASE] {_gameState.Phase}, round={_gameState.RoundIndex}")
                        ' Auto BET when entering BETTING (simple, fixed amount)
                        If _gameState.Phase = GamePhase.BETTING AndAlso _gameState.PlayerId > 0 Then
                            If _disableAutoBet Then
                                UiLog("[INFO] Auto-BET disabled for manual UI testing")
                            ElseIf _gameForm Is Nothing OrElse _gameForm.IsDisposed Then
                                Dim betAmount As Integer = Math.Min(100, If(_gameState.PlayerId = 1, _gameState.ChipsP1, _gameState.ChipsP2))
                                Dim target As String = If(_gameState.PlayerId = 1, "PLAYER", "BANKER")
                                SendLine($"{CommandNames.BET},{_gameState.PlayerId},{target},{betAmount}")
                                UiLog($"[SEND] BET auto: {target},{betAmount}")
                            Else
                                UiLog("[INFO] Manual UI present - skipping auto-BET")
                            End If
                        End If
                    End If
                Case CommandNames.BET_ACK
                    If parts.Length >= 2 Then
                        Dim ok = parts(1).Trim().ToLower() = "true"
                        UiLog(If(ok, "[BET_ACK] OK", $"[BET_ACK] Rejected: {If(parts.Length > 2, parts(2).Trim(), "")}"))
                    End If
                Case CommandNames.DEAL
                    If parts.Length >= 3 Then
                        UiLog($"[DEAL] Player: {parts(1)}, Banker: {parts(2)}")
                    End If
                Case CommandNames.ROUND_RESULT
                    If parts.Length >= 6 Then
                        Integer.TryParse(parts(4), _gameState.ChipsP1)
                        Integer.TryParse(parts(5), _gameState.ChipsP2)
                        UiLog($"[RESULT] Winner={parts(1)}, Chips P1={_gameState.ChipsP1}, P2={_gameState.ChipsP2}")
                    End If
                Case CommandNames.GAME_OVER
                    _gameState.Phase = GamePhase.GAMEOVER
                    UiLog($"[GAME_OVER] {parts(1)}")
                Case CommandNames.ERROR
                    UiLog($"[ERROR] {If(parts.Length > 1, parts(1), "")}")
            End Select

            ' Update FormGame UI if present
            Try
                If Me.InvokeRequired Then
                    Me.Invoke(Sub() UpdateGameForm())
                Else
                    UpdateGameForm()
                End If
            Catch ex As Exception
                UiLog($"[ERROR] UpdateGameForm failed: {ex.Message}")
            End Try

            ' Show game UI after welcome
            If Me.InvokeRequired Then Me.Invoke(Sub() ShowGameForm()) Else ShowGameForm()
        End Sub

        Private Sub SendLine(text As String)
            If _handle = -1 Then Return
            Dim data As Byte() = Encoding.UTF8.GetBytes(text & vbLf)
            _tcp.Send(_handle, data)
        End Sub

        Private Sub UiLog(line As String)
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() UiLog(line))
            Else
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}" & Environment.NewLine)
                txtLog.ScrollToCaret()
            End If
        End Sub

        ' Show FormGame on UI thread
        Private Sub ShowGameForm()
            Try
                If _gameForm Is Nothing OrElse _gameForm.IsDisposed Then
                    _gameForm = New FormGame(_gameState, AddressOf SendLine)
                    _gameForm.Show()
                    UiLog("[DEBUG] FormGame shown")
                Else
                    _gameForm.BringToFront()
                    UiLog("[DEBUG] FormGame brought to front")
                End If
                Me.Hide()
            Catch ex As Exception
                UiLog($"[ERROR] ShowGameForm exception: {ex.Message}")
            End Try
        End Sub

        Private Sub UpdateGameForm()
            If _gameForm IsNot Nothing AndAlso Not _gameForm.IsDisposed Then
                _gameForm.ApplyPhase(_gameState.Phase)
                _gameForm.UpdateHeader()
            End If
        End Sub
    End Class
End Namespace