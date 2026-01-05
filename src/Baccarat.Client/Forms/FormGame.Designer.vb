Imports System.Windows.Forms

Namespace Baccarat.Client.Forms
    Partial Class FormGame
        Inherits Form

        Private Sub InitializeComponent()
            Me.lblPhase = New Label()
            Me.lblRound = New Label()
            Me.grpBet = New GroupBox()
            Me.btnNext = New Button()
            Me.btnRules = New Button()
            Me.SuspendLayout()
            '
            'lblPhase
            '
            Me.lblPhase.AutoSize = True
            Me.lblPhase.Location = New Drawing.Point(20, 20)
            Me.lblPhase.Name = "lblPhase"
            Me.lblPhase.Size = New Drawing.Size(72, 15)
            Me.lblPhase.TabIndex = 0
            Me.lblPhase.Text = "Phase: TBD"
            '
            'lblRound
            '
            Me.lblRound.AutoSize = True
            Me.lblRound.Location = New Drawing.Point(20, 45)
            Me.lblRound.Name = "lblRound"
            Me.lblRound.Size = New Drawing.Size(77, 15)
            Me.lblRound.TabIndex = 1
            Me.lblRound.Text = "Round: TBD"
            '
            'grpBet
            '
            Me.grpBet.Location = New Drawing.Point(20, 80)
            Me.grpBet.Name = "grpBet"
            Me.grpBet.Size = New Drawing.Size(200, 120)
            Me.grpBet.TabIndex = 2
            Me.grpBet.TabStop = False
            Me.grpBet.Text = "Bet (placeholder)"
            '
            'btnNext
            '
            Me.btnNext.Location = New Drawing.Point(20, 220)
            Me.btnNext.Name = "btnNext"
            Me.btnNext.Size = New Drawing.Size(94, 29)
            Me.btnNext.TabIndex = 3
            Me.btnNext.Text = "Next"
            Me.btnNext.UseVisualStyleBackColor = True
            '
            'btnRules
            '
            Me.btnRules.Location = New Drawing.Point(140, 220)
            Me.btnRules.Name = "btnRules"
            Me.btnRules.Size = New Drawing.Size(94, 29)
            Me.btnRules.TabIndex = 4
            Me.btnRules.Text = "Rules"
            Me.btnRules.UseVisualStyleBackColor = True
            '
            'FormGame
            '
            Me.AutoScaleDimensions = New Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.ClientSize = New Drawing.Size(800, 600)
            Me.Controls.Add(Me.btnRules)
            Me.Controls.Add(Me.btnNext)
            Me.Controls.Add(Me.grpBet)
            Me.Controls.Add(Me.lblRound)
            Me.Controls.Add(Me.lblPhase)
            Me.Name = "FormGame"
            Me.Text = "Baccarat - Game"
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents lblPhase As Label
        Friend WithEvents lblRound As Label
        Friend WithEvents grpBet As GroupBox
        Friend WithEvents btnNext As Button
        Friend WithEvents btnRules As Button
    End Class
End Namespace
