Imports System.Windows.Forms
Imports Baccarat.Shared.Protocol
Imports Baccarat.Client.Model
Imports System.Text

Namespace Forms
    Partial Public Class FormGame
        Private _state As ClientGameState
        Private _send As Action(Of String)
        ' ìÆìIÇ…çÏÇÈç≈è¨BET UI
        Private radPlayer As RadioButton
        Private radBanker As RadioButton
        Private radTie As RadioButton
        Private numAmount As NumericUpDown
        Private btnBetLock As Button

        Public Sub New(state As ClientGameState, sendAction As Action(Of String))
            Me.New()
            _state = state
            _send = sendAction
        End Sub

        Private Sub FormGame_Load_Minimal(sender As Object, e As EventArgs) Handles MyBase.Load
            If _state Is Nothing Then _state = New ClientGameState()
            If _send Is Nothing Then _send = Sub(s As String)
                                             End Sub

            BuildBetControls()
            ' ä˘ë∂ÇÃ ApplyPhase Çóòóp
            ApplyPhase(GamePhase.BETTING)
            UpdateHeader()
        End Sub

        Private Sub BuildBetControls()
            grpBet.Controls.Clear()

            radPlayer = New RadioButton() With {.Text = "Player", .Location = New Drawing.Point(10, 25), .Checked = True}
            radBanker = New RadioButton() With {.Text = "Banker", .Location = New Drawing.Point(90, 25)}
            radTie = New RadioButton() With {.Text = "Tie", .Location = New Drawing.Point(180, 25)}
            numAmount = New NumericUpDown() With {.Minimum = 1, .Maximum = 999, .Value = 100, .Location = New Drawing.Point(260, 22), .Width = 80}
            btnBetLock = New Button() With {.Text = "Bet", .Location = New Drawing.Point(360, 20), .Width = 80}
            AddHandler btnBetLock.Click, AddressOf OnBetClick

            grpBet.Controls.Add(radPlayer)
            grpBet.Controls.Add(radBanker)
            grpBet.Controls.Add(radTie)
            grpBet.Controls.Add(numAmount)
            grpBet.Controls.Add(btnBetLock)
        End Sub

        Private Sub OnBetClick(sender As Object, e As EventArgs)
            If _state Is Nothing OrElse _state.PlayerId <= 0 Then
                Log("[WARN] PlayerId not set")
                Return
            End If
            Dim t As String = If(radPlayer.Checked, "PLAYER", If(radBanker.Checked, "BANKER", "TIE"))
            Dim amt As Integer = CInt(numAmount.Value)
            Dim line = $"{CommandNames.BET},{_state.PlayerId},{t},{amt}"
            Log($"[SEND] {line}")
            _send.Invoke(line)
            grpBet.Enabled = False
        End Sub

        Public Sub UpdateHeader()
            lblRound.Text = $"Round: {_state.RoundIndex} / {_state.MaxRounds}"
            Dim chips = If(_state.PlayerId = 1, _state.ChipsP1, _state.ChipsP2)
            lblChips.Text = $"Chips: {chips}"
        End Sub

        Private Sub Log(msg As String)
            If txtLog Is Nothing Then Return
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() Log(msg))
            Else
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}" & Environment.NewLine)
                txtLog.ScrollToCaret()
            End If
        End Sub
    End Class
End Namespace