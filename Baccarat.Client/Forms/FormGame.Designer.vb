Imports System.Windows.Forms

Namespace Forms
    Partial Class FormGame
        Inherits Form

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

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            lblPhase = New Label()
            lblRound = New Label()
            grpBet = New GroupBox()
            btnNext = New Button()
            btnRules = New Button()
            pnlPlayer = New Panel()
            pnlBanker = New Panel()
            lblPlayerScore = New Label()
            lblBankerScore = New Label()
            lblChips = New Label()
            txtLog = New TextBox()
            SuspendLayout()
            ' 
            ' lblPhase
            ' 
            lblPhase.Font = New Font("Segoe UI", 12F, FontStyle.Bold)
            lblPhase.Location = New Point(17, 15)
            lblPhase.Margin = New Padding(4, 0, 4, 0)
            lblPhase.Name = "lblPhase"
            lblPhase.Size = New Size(286, 50)
            lblPhase.TabIndex = 12
            lblPhase.Text = "Phase: WAITING"
            ' 
            ' lblRound
            ' 
            lblRound.Location = New Point(311, 25)
            lblRound.Margin = New Padding(4, 0, 4, 0)
            lblRound.Name = "lblRound"
            lblRound.Size = New Size(143, 38)
            lblRound.TabIndex = 11
            lblRound.Text = "Round: 0"
            ' 
            ' grpBet
            ' 
            grpBet.Location = New Point(71, 433)
            grpBet.Margin = New Padding(4, 5, 4, 5)
            grpBet.Name = "grpBet"
            grpBet.Padding = New Padding(4, 5, 4, 5)
            grpBet.Size = New Size(929, 167)
            grpBet.TabIndex = 2
            grpBet.TabStop = False
            grpBet.Text = "Betting Area"
            ' 
            ' btnNext
            ' 
            btnNext.Enabled = False
            btnNext.Location = New Point(794, 633)
            btnNext.Margin = New Padding(4, 5, 4, 5)
            btnNext.Name = "btnNext"
            btnNext.Size = New Size(206, 67)
            btnNext.TabIndex = 3
            btnNext.Text = "Next Round"
            btnNext.UseVisualStyleBackColor = True
            ' 
            ' btnRules
            ' 
            btnRules.Location = New Point(71, 633)
            btnRules.Margin = New Padding(4, 5, 4, 5)
            btnRules.Name = "btnRules"
            btnRules.Size = New Size(143, 67)
            btnRules.TabIndex = 4
            btnRules.Text = "Show Rules"
            btnRules.UseVisualStyleBackColor = True
            ' 
            ' pnlPlayer
            ' 
            pnlPlayer.BorderStyle = BorderStyle.FixedSingle
            pnlPlayer.Location = New Point(71, 100)
            pnlPlayer.Margin = New Padding(4, 5, 4, 5)
            pnlPlayer.Name = "pnlPlayer"
            pnlPlayer.Size = New Size(428, 249)
            pnlPlayer.TabIndex = 5
            ' 
            ' pnlBanker
            ' 
            pnlBanker.BorderStyle = BorderStyle.FixedSingle
            pnlBanker.Location = New Point(571, 100)
            pnlBanker.Margin = New Padding(4, 5, 4, 5)
            pnlBanker.Name = "pnlBanker"
            pnlBanker.Size = New Size(428, 249)
            pnlBanker.TabIndex = 6
            ' 
            ' lblPlayerScore
            ' 
            lblPlayerScore.Location = New Point(71, 358)
            lblPlayerScore.Margin = New Padding(4, 0, 4, 0)
            lblPlayerScore.Name = "lblPlayerScore"
            lblPlayerScore.Size = New Size(429, 38)
            lblPlayerScore.TabIndex = 10
            lblPlayerScore.Text = "Player Score: 0"
            lblPlayerScore.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblBankerScore
            ' 
            lblBankerScore.Location = New Point(571, 358)
            lblBankerScore.Margin = New Padding(4, 0, 4, 0)
            lblBankerScore.Name = "lblBankerScore"
            lblBankerScore.Size = New Size(429, 38)
            lblBankerScore.TabIndex = 9
            lblBankerScore.Text = "Banker Score: 0"
            lblBankerScore.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblChips
            ' 
            lblChips.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
            lblChips.ForeColor = Color.Goldenrod
            lblChips.Location = New Point(463, 25)
            lblChips.Margin = New Padding(4, 0, 4, 0)
            lblChips.Name = "lblChips"
            lblChips.Size = New Size(214, 38)
            lblChips.TabIndex = 8
            lblChips.Text = "Chips: 1000"
            ' 
            ' txtLog
            ' 
            txtLog.Location = New Point(71, 733)
            txtLog.Margin = New Padding(4, 5, 4, 5)
            txtLog.Multiline = True
            txtLog.Name = "txtLog"
            txtLog.ReadOnly = True
            txtLog.ScrollBars = ScrollBars.Vertical
            txtLog.Size = New Size(927, 164)
            txtLog.TabIndex = 7
            ' 
            ' FormGame
            ' 
            AutoScaleDimensions = New SizeF(10F, 25F)
            AutoScaleMode = AutoScaleMode.Font
            ClientSize = New Size(1071, 933)
            Controls.Add(txtLog)
            Controls.Add(lblChips)
            Controls.Add(lblBankerScore)
            Controls.Add(pnlBanker)
            Controls.Add(lblPlayerScore)
            Controls.Add(pnlPlayer)
            Controls.Add(btnRules)
            Controls.Add(btnNext)
            Controls.Add(grpBet)
            Controls.Add(lblRound)
            Controls.Add(lblPhase)
            Margin = New Padding(4, 5, 4, 5)
            Name = "FormGame"
            Text = "Baccarat Game Table"
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents lblPhase As Label
        Friend WithEvents lblRound As Label
        Friend WithEvents grpBet As GroupBox
        Friend WithEvents btnNext As Button
        Friend WithEvents btnRules As Button
        Friend WithEvents pnlPlayer As Panel
        Friend WithEvents pnlBanker As Panel
        Friend WithEvents lblPlayerScore As Label
        Friend WithEvents lblBankerScore As Label
        Friend WithEvents lblChips As Label
        Friend WithEvents txtLog As TextBox
    End Class
End Namespace