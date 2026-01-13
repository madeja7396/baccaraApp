Imports System.Windows.Forms
Imports System.Drawing

Namespace Forms
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FormGame
        Inherits Form

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.lblPhase = New System.Windows.Forms.Label()
            Me.lblRound = New System.Windows.Forms.Label()
            Me.lblChips = New System.Windows.Forms.Label()
            Me.pnlPlayer = New System.Windows.Forms.Panel()
            Me.pnlBanker = New System.Windows.Forms.Panel()
            Me.lblPlayerScore = New System.Windows.Forms.Label()
            Me.lblBankerScore = New System.Windows.Forms.Label()
            Me.grpBet = New System.Windows.Forms.GroupBox()
            Me.rbPlayer = New System.Windows.Forms.RadioButton()
            Me.rbBanker = New System.Windows.Forms.RadioButton()
            Me.rbTie = New System.Windows.Forms.RadioButton()
            Me.lblBetAmount = New System.Windows.Forms.Label()
            Me.nudBet = New System.Windows.Forms.NumericUpDown()
            Me.btnBet = New System.Windows.Forms.Button()
            Me.lblBetInfo = New System.Windows.Forms.Label()
            Me.lblResult = New System.Windows.Forms.Label()
            Me.btnRules = New System.Windows.Forms.Button()
            Me.btnNext = New System.Windows.Forms.Button()
            Me.txtLog = New System.Windows.Forms.TextBox()
            Me.grpBet.SuspendLayout()
            CType(Me.nudBet, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            ' lblPhase
            '
            Me.lblPhase.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
            Me.lblPhase.Location = New System.Drawing.Point(17, 15)
            Me.lblPhase.Name = "lblPhase"
            Me.lblPhase.Size = New System.Drawing.Size(286, 50)
            Me.lblPhase.TabIndex = 0
            Me.lblPhase.Text = "Phase: WAITING"
            '
            ' lblRound
            '
            Me.lblRound.Location = New System.Drawing.Point(311, 25)
            Me.lblRound.Name = "lblRound"
            Me.lblRound.Size = New System.Drawing.Size(143, 38)
            Me.lblRound.TabIndex = 1
            Me.lblRound.Text = "Round: 0"
            '
            ' lblChips
            '
            Me.lblChips.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.lblChips.ForeColor = System.Drawing.Color.Goldenrod
            Me.lblChips.Location = New System.Drawing.Point(463, 25)
            Me.lblChips.Name = "lblChips"
            Me.lblChips.Size = New System.Drawing.Size(214, 38)
            Me.lblChips.TabIndex = 2
            Me.lblChips.Text = "Chips: 1000"
            '
            ' pnlPlayer
            '
            Me.pnlPlayer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlPlayer.Location = New System.Drawing.Point(71, 100)
            Me.pnlPlayer.Name = "pnlPlayer"
            Me.pnlPlayer.Size = New System.Drawing.Size(428, 249)
            Me.pnlPlayer.TabIndex = 3
            '
            ' pnlBanker
            '
            Me.pnlBanker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlBanker.Location = New System.Drawing.Point(571, 100)
            Me.pnlBanker.Name = "pnlBanker"
            Me.pnlBanker.Size = New System.Drawing.Size(428, 249)
            Me.pnlBanker.TabIndex = 4
            '
            ' lblPlayerScore
            '
            Me.lblPlayerScore.Location = New System.Drawing.Point(71, 358)
            Me.lblPlayerScore.Name = "lblPlayerScore"
            Me.lblPlayerScore.Size = New System.Drawing.Size(429, 38)
            Me.lblPlayerScore.TabIndex = 5
            Me.lblPlayerScore.Text = "Player Score: 0"
            Me.lblPlayerScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            ' lblBankerScore
            '
            Me.lblBankerScore.Location = New System.Drawing.Point(571, 358)
            Me.lblBankerScore.Name = "lblBankerScore"
            Me.lblBankerScore.Size = New System.Drawing.Size(429, 38)
            Me.lblBankerScore.TabIndex = 6
            Me.lblBankerScore.Text = "Banker Score: 0"
            Me.lblBankerScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            ' grpBet
            '
            Me.grpBet.Controls.Add(Me.lblResult)
            Me.grpBet.Controls.Add(Me.lblBetInfo)
            Me.grpBet.Controls.Add(Me.btnBet)
            Me.grpBet.Controls.Add(Me.nudBet)
            Me.grpBet.Controls.Add(Me.lblBetAmount)
            Me.grpBet.Controls.Add(Me.rbTie)
            Me.grpBet.Controls.Add(Me.rbBanker)
            Me.grpBet.Controls.Add(Me.rbPlayer)
            Me.grpBet.Location = New System.Drawing.Point(71, 413)
            Me.grpBet.Name = "grpBet"
            Me.grpBet.Size = New System.Drawing.Size(929, 210)
            Me.grpBet.TabIndex = 7
            Me.grpBet.TabStop = False
            Me.grpBet.Text = "Betting Area"
            '
            ' rbPlayer
            '
            Me.rbPlayer.AutoSize = True
            Me.rbPlayer.Checked = True
            Me.rbPlayer.Location = New System.Drawing.Point(68, 29)
            Me.rbPlayer.Name = "rbPlayer"
            Me.rbPlayer.Size = New System.Drawing.Size(84, 29)
            Me.rbPlayer.TabIndex = 0
            Me.rbPlayer.TabStop = True
            Me.rbPlayer.Text = "Player"
            Me.rbPlayer.UseVisualStyleBackColor = True
            '
            ' rbBanker
            '
            Me.rbBanker.AutoSize = True
            Me.rbBanker.Location = New System.Drawing.Point(354, 32)
            Me.rbBanker.Name = "rbBanker"
            Me.rbBanker.Size = New System.Drawing.Size(90, 29)
            Me.rbBanker.TabIndex = 1
            Me.rbBanker.Text = "Banker"
            Me.rbBanker.UseVisualStyleBackColor = True
            '
            ' rbTie
            '
            Me.rbTie.AutoSize = True
            Me.rbTie.Location = New System.Drawing.Point(622, 29)
            Me.rbTie.Name = "rbTie"
            Me.rbTie.Size = New System.Drawing.Size(59, 29)
            Me.rbTie.TabIndex = 2
            Me.rbTie.Text = "Tie"
            Me.rbTie.UseVisualStyleBackColor = True
            '
            ' lblBetAmount
            '
            Me.lblBetAmount.AutoSize = True
            Me.lblBetAmount.Location = New System.Drawing.Point(55, 81)
            Me.lblBetAmount.Name = "lblBetAmount"
            Me.lblBetAmount.Size = New System.Drawing.Size(37, 25)
            Me.lblBetAmount.TabIndex = 3
            Me.lblBetAmount.Text = "Bet"
            '
            ' nudBet
            '
            Me.nudBet.Increment = New Decimal(New Integer() {10, 0, 0, 0})
            Me.nudBet.Location = New System.Drawing.Point(136, 79)
            Me.nudBet.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.nudBet.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.nudBet.Name = "nudBet"
            Me.nudBet.Size = New System.Drawing.Size(180, 31)
            Me.nudBet.TabIndex = 4
            Me.nudBet.Value = New Decimal(New Integer() {100, 0, 0, 0})
            '
            ' btnBet
            '
            Me.btnBet.Location = New System.Drawing.Point(354, 81)
            Me.btnBet.Name = "btnBet"
            Me.btnBet.Size = New System.Drawing.Size(112, 36)
            Me.btnBet.TabIndex = 5
            Me.btnBet.Text = "Bet"
            Me.btnBet.UseVisualStyleBackColor = True
            '
            ' lblBetInfo
            '
            Me.lblBetInfo.AutoSize = True
            Me.lblBetInfo.Location = New System.Drawing.Point(55, 125)
            Me.lblBetInfo.Name = "lblBetInfo"
            Me.lblBetInfo.Size = New System.Drawing.Size(64, 25)
            Me.lblBetInfo.TabIndex = 6
            Me.lblBetInfo.Text = "‘I‘ð: -"
            '
            ' lblResult
            '
            Me.lblResult.AutoSize = True
            Me.lblResult.Location = New System.Drawing.Point(55, 167)
            Me.lblResult.Name = "lblResult"
            Me.lblResult.Size = New System.Drawing.Size(64, 25)
            Me.lblResult.TabIndex = 7
            Me.lblResult.Text = "Œ‹‰Ê: -"
            '
            ' btnRules
            '
            Me.btnRules.Location = New System.Drawing.Point(71, 633)
            Me.btnRules.Name = "btnRules"
            Me.btnRules.Size = New System.Drawing.Size(143, 67)
            Me.btnRules.TabIndex = 8
            Me.btnRules.Text = "Show Rules"
            Me.btnRules.UseVisualStyleBackColor = True
            '
            ' btnNext
            '
            Me.btnNext.Enabled = False
            Me.btnNext.Location = New System.Drawing.Point(794, 633)
            Me.btnNext.Name = "btnNext"
            Me.btnNext.Size = New System.Drawing.Size(206, 67)
            Me.btnNext.TabIndex = 9
            Me.btnNext.Text = "Next Round"
            Me.btnNext.UseVisualStyleBackColor = True
            '
            ' txtLog
            '
            Me.txtLog.Location = New System.Drawing.Point(71, 733)
            Me.txtLog.Multiline = True
            Me.txtLog.Name = "txtLog"
            Me.txtLog.ReadOnly = True
            Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.txtLog.Size = New System.Drawing.Size(927, 164)
            Me.txtLog.TabIndex = 10
            '
            ' FormGame
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1071, 933)
            Me.Controls.Add(Me.txtLog)
            Me.Controls.Add(Me.btnNext)
            Me.Controls.Add(Me.btnRules)
            Me.Controls.Add(Me.grpBet)
            Me.Controls.Add(Me.lblBankerScore)
            Me.Controls.Add(Me.lblPlayerScore)
            Me.Controls.Add(Me.pnlBanker)
            Me.Controls.Add(Me.pnlPlayer)
            Me.Controls.Add(Me.lblChips)
            Me.Controls.Add(Me.lblRound)
            Me.Controls.Add(Me.lblPhase)
            Me.Name = "FormGame"
            Me.Text = "Baccarat Game Table"
            Me.grpBet.ResumeLayout(False)
            Me.grpBet.PerformLayout()
            CType(Me.nudBet, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents lblPhase As Label
        Friend WithEvents lblRound As Label
        Friend WithEvents lblChips As Label
        Friend WithEvents pnlPlayer As Panel
        Friend WithEvents pnlBanker As Panel
        Friend WithEvents lblPlayerScore As Label
        Friend WithEvents lblBankerScore As Label
        Friend WithEvents grpBet As GroupBox
        Friend WithEvents rbPlayer As RadioButton
        Friend WithEvents rbBanker As RadioButton
        Friend WithEvents rbTie As RadioButton
        Friend WithEvents lblBetAmount As Label
        Friend WithEvents nudBet As NumericUpDown
        Friend WithEvents btnBet As Button
        Friend WithEvents lblBetInfo As Label
        Friend WithEvents lblResult As Label
        Friend WithEvents btnRules As Button
        Friend WithEvents btnNext As Button
        Friend WithEvents txtLog As TextBox
    End Class
End Namespace
