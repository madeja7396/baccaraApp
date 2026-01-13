Imports System.Windows.Forms
Imports Baccarat.Shared.Protocol
Imports Baccarat.Client.Model

Namespace Forms
    Partial Public Class FormGame
        Inherits Form

        Private _state As ClientGameState
        Private _send As Action(Of String)

        ' Dynamic controls
        Private radPlayer As RadioButton
        Private radBanker As RadioButton
        Private radTie As RadioButton
        Private numAmount As NumericUpDown
        Private btnBet As Button

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(state As ClientGameState, sendAction As Action(Of String))
            Me.New()
            _state = state
            _send = sendAction
        End Sub

        Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If _state Is Nothing Then _state = New ClientGameState()
            If _send Is Nothing Then _send = Sub(s As String)
                                             End Sub
            BuildBetControls()
            ApplyPhase(_state.Phase)
            UpdateHeader()
            AddHandler btnNext.Click, AddressOf OnNextClick
            AddHandler btnRules.Click, AddressOf OnRulesClick
        End Sub

        Private Sub BuildBetControls()
            Try
                grpBet.Controls.Clear()
                radPlayer = New RadioButton() With {.Text = "Player", .Location = New Drawing.Point(10, 25), .Checked = True}
                radBanker = New RadioButton() With {.Text = "Banker", .Location = New Drawing.Point(110, 25)}
                radTie = New RadioButton() With {.Text = "Tie", .Location = New Drawing.Point(210, 25)}
                numAmount = New NumericUpDown() With {.Minimum = 1, .Maximum = 9999, .Value = 100, .Location = New Drawing.Point(310, 22), .Width = 100}
                btnBet = New Button() With {.Text = "Bet", .Location = New Drawing.Point(430, 20), .Width = 100}
                AddHandler btnBet.Click, AddressOf OnBetClick

                grpBet.Controls.Add(radPlayer)
                grpBet.Controls.Add(radBanker)
                grpBet.Controls.Add(radTie)
                grpBet.Controls.Add(numAmount)
                grpBet.Controls.Add(btnBet)

                ' set maximum according to current chips
                Try
                    Dim chips = If(_state.PlayerId = 1, _state.ChipsP1, _state.ChipsP2)
                    If numAmount IsNot Nothing Then numAmount.Maximum = Math.Max(1, chips)
                Catch
                End Try
            Catch ex As Exception
                ' ignore layout errors
            End Try
        End Sub

        Private Sub OnBetClick(sender As Object, e As EventArgs)
            If _state Is Nothing OrElse _state.PlayerId <= 0 Then
                AppendLog("[WARN] PlayerId not set")
                Return
            End If
            Dim target = If(radPlayer.Checked, "PLAYER", If(radBanker.Checked, "BANKER", "TIE"))
            Dim amt As Integer = CInt(numAmount.Value)
            Dim line = $"{CommandNames.BET},{_state.PlayerId},{target},{amt}"
            AppendLog($"[SEND] {line}")
            Try
                btnBet.Enabled = False
                grpBet.Enabled = False
                _send.Invoke(line)
            Catch
                btnBet.Enabled = True
                grpBet.Enabled = True
            End Try
        End Sub

        Private Sub OnNextClick(sender As Object, e As EventArgs)
            Try
                AppendLog("[SEND] READY")
                _send.Invoke(CommandNames.READY)
                btnNext.Enabled = False
            Catch
            End Try
        End Sub

        Private Sub OnRulesClick(sender As Object, e As EventArgs)
            Try
                Dim frm As New FormRules()
                frm.Show(Me)
            Catch
            End Try
        End Sub

        Public Sub ApplyPhase(p As GamePhase)
            lblPhase.Text = $"Phase: {p}"
            grpBet.Enabled = (p = GamePhase.BETTING)
            btnNext.Enabled = (p = GamePhase.RESULT)
            btnRules.Enabled = True
        End Sub

        Public Sub UpdateHeader()
            lblRound.Text = $"Round: {_state.RoundIndex} / {_state.MaxRounds}"
            Dim chips = If(_state.PlayerId = 1, _state.ChipsP1, _state.ChipsP2)
            lblChips.Text = $"Chips: {chips}"
            Try
                If numAmount IsNot Nothing Then numAmount.Maximum = Math.Max(1, chips)
            Catch
            End Try
        End Sub

        Public Sub AppendLog(line As String)
            If txtLog Is Nothing Then Return
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() AppendLog(line))
            Else
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}" & Environment.NewLine)
                txtLog.ScrollToCaret()
            End If
        End Sub

        ' DEAL を表示（テキスト表示、パネルをクリアしてラベルを置く）
        Public Sub ShowDeal(playerCodes As String, bankerCodes As String)
            If pnlPlayer.InvokeRequired Then
                pnlPlayer.Invoke(Sub() ShowDeal(playerCodes, bankerCodes))
                Return
            End If
            pnlPlayer.Controls.Clear()
            pnlBanker.Controls.Clear()
            Dim lblP As New Label With {
                .Text = playerCodes,
                .Dock = DockStyle.Fill,
                .TextAlign = ContentAlignment.MiddleCenter,
                .AutoSize = False
            }
            Dim lblB As New Label With {
                .Text = bankerCodes,
                .Dock = DockStyle.Fill,
                .TextAlign = ContentAlignment.MiddleCenter,
                .AutoSize = False
            }
            pnlPlayer.Controls.Add(lblP)
            pnlBanker.Controls.Add(lblB)
            AppendLog($"[DEAL] Player: {playerCodes} | Banker: {bankerCodes}")
        End Sub

        ' ROUND_RESULT を画面に反映（ログ＋ヘッダのチップ更新）
        Public Sub ApplyRoundResult(winner As String, payoutP1 As Integer, payoutP2 As Integer, chipsP1 As Integer, chipsP2 As Integer)
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() ApplyRoundResult(winner, payoutP1, payoutP2, chipsP1, chipsP2))
                Return
            End If
            AppendLog($"[RESULT] Winner={winner}, payoutP1={payoutP1}, payoutP2={payoutP2}, chipsP1={chipsP1}, chipsP2={chipsP2}")
            ' 更新はクライアントの state と UI に反映
            _state.ChipsP1 = chipsP1
            _state.ChipsP2 = chipsP2
            UpdateHeader()
            ' re-enable bet UI for next betting phase (will be gated by ApplyPhase)
            grpBet.Enabled = False
        End Sub
    End Class
End Namespace