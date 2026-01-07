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
            ' TODO: TcpSockets.OpenAsServer(port)
        End Sub

        Public Sub StopServer()
            _logger.Info("Server stop")
            ' TODO: close sockets and cleanup
        End Sub

        Public Sub OnAccept(handle As Long)
            _logger.Info($"Accept handle={handle}")
            If Not _clients.Contains(handle) Then
                _clients.Add(handle)
            End If
            ' TODO: enqueue until HELLO arrives
        End Sub

        Public Sub OnDisconnect(handle As Long)
            _logger.Info($"Disconnect handle={handle}")
            If _clients.Contains(handle) Then
                _clients.Remove(handle)
            End If
            ' TODO: notify opponent & transition to GAMEOVER
        End Sub

        Public Sub OnLineReceived(handle As Long, line As String)
            _logger.Receive(handle.ToString(), line)
            Dim msg As Message = Nothing
            If Not Parser.TryParse(line, msg) Then
                _logger.Error("Bad format")
                Return
            End If

            Select Case msg.Command
                Case CommandNames.HELLO : HandleHello(handle, msg)
                Case CommandNames.READY : HandleReady(handle)
                Case CommandNames.BET : HandleBet(handle, msg)
                Case Else
                    _logger.Error($"Unsupported command {msg.Command}")
            End Select
        End Sub

        Private Sub HandleHello(handle As Long, msg As Message)
            ' TODO: validate nickname and assign player id
        End Sub

        Private Sub HandleReady(handle As Long)
            ' TODO: set ready flag and maybe advance phase
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
