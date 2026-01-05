Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Rules
Imports Baccarat.Shared.Util

Namespace Net
    ' Server-side coordinator (Experiment.TcpSocket を後で接続)
    Public Class ServerHost
        Private ReadOnly _logger As Logger
        Private ReadOnly _rules As IBaccaratRules
        Private ReadOnly _payout As IPayoutCalculator
        Private ReadOnly _state As GameState = New GameState()

        Public Sub New(logger As Logger, rules As IBaccaratRules, payout As IPayoutCalculator)
            _logger = logger
            _rules = rules
            _payout = payout
        End Sub

        Public Sub StartServer(port As Integer)
            _logger.Info($"Server start on {port}")
            ' TODO: TcpSockets.OpenAsServer(port)
        End Sub

        Public Sub StopServer()
            _logger.Info("Server stop")
            ' TODO: close sockets and cleanup
        End Sub

        Public Sub OnAccept(handle As Integer)
            _logger.Info($"Accept handle={handle}")
            ' TODO: enqueue until HELLO arrives
        End Sub

        Public Sub OnDisconnect(handle As Integer)
            _logger.Info($"Disconnect handle={handle}")
            ' TODO: notify opponent & transition to GAMEOVER
        End Sub

        Public Sub OnLineReceived(handle As Integer, line As String)
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

        Private Sub HandleHello(handle As Integer, msg As Message)
            ' TODO: validate nickname and assign player id
        End Sub

        Private Sub HandleReady(handle As Integer)
            ' TODO: set ready flag and maybe advance phase
        End Sub

        Private Sub HandleBet(handle As Integer, msg As Message)
            ' TODO: validate phase and bet amount
        End Sub
    End Class
End Namespace
