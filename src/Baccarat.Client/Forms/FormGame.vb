Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util

Namespace Baccarat.Client.Forms
    Public Class FormGame
        Private _logger As Logger
        Private _phase As GamePhase = GamePhase.LOBBY

        Public Sub New()
            InitializeComponent()
            _logger = New Logger(AddressOf AppendLog)
        End Sub

        Public Sub ApplyPhase(p As GamePhase)
            _phase = p
            grpBet.Enabled = (p = GamePhase.BETTING)
            btnNext.Enabled = (p = GamePhase.RESULT)
            btnRules.Enabled = True
        End Sub

        Private Sub btnBetLock_Click(sender As Object, e As EventArgs) Handles btnBetLock.Click
            ' TODO: send BET command
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
            ' TODO: request next round or notify server
        End Sub

        Private Sub btnRules_Click(sender As Object, e As EventArgs) Handles btnRules.Click
            ' TODO: show FormRules (single instance)
        End Sub

        Private Sub AppendLog(line As String)
            ' TODO: append to txtLog
        End Sub
    End Class
End Namespace
