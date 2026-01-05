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
            Me.lblPhase = New Label()
            Me.lblRound = New Label()
            Me.grpBet = New GroupBox()
            Me.btnNext = New Button()
            Me.btnRules = New Button()
            Me.pnlPlayer = New Panel()
            Me.pnlBanker = New Panel()
            Me.lblPlayerScore = New Label()
            Me.lblBankerScore = New Label()
            Me.lblChips = New Label()
            Me.txtLog = New TextBox()
            Me.SuspendLayout()
            '
            'lblPhase (現在のフェーズ表示)
            '
            Me.lblPhase.Font = New Drawing.Font("Segoe UI", 12.0!, Drawing.FontStyle.Bold)
            Me.lblPhase.Location = New Drawing.Point(12, 9)
            Me.lblPhase.Name = "lblPhase"
            Me.lblPhase.Size = New Drawing.Size(200, 30)
            Me.lblPhase.Text = "Phase: WAITING"
            '
            'lblRound (ラウンド数)
            '
            Me.lblRound.Location = New Drawing.Point(218, 15)
            Me.lblRound.Name = "lblRound"
            Me.lblRound.Size = New Drawing.Size(100, 23)
            Me.lblRound.Text = "Round: 0"
            '
            'lblChips (所持チップ表示)
            '
            Me.lblChips.Font = New Drawing.Font("Segoe UI", 10.0!, Drawing.FontStyle.Bold)
            Me.lblChips.ForeColor = Drawing.Color.Goldenrod
            Me.lblChips.Location = New Drawing.Point(324, 15)
            Me.lblChips.Name = "lblChips"
            Me.lblChips.Size = New Drawing.Size(150, 23)
            Me.lblChips.Text = "Chips: 1000"
            '
            'pnlPlayer (プレイヤー側のカード配置エリア)
            '
            Me.pnlPlayer.BorderStyle = BorderStyle.FixedSingle
            Me.pnlPlayer.Location = New Drawing.Point(50, 60)
            Me.pnlPlayer.Name = "pnlPlayer"
            Me.pnlPlayer.Size = New Drawing.Size(300, 150)
            Me.pnlPlayer.TabIndex = 5
            '
            'lblPlayerScore (プレイヤーの点数)
            '
            Me.lblPlayerScore.Location = New Drawing.Point(50, 215)
            Me.lblPlayerScore.Name = "lblPlayerScore"
            Me.lblPlayerScore.Size = New Drawing.Size(300, 23)
            Me.lblPlayerScore.Text = "Player Score: 0"
            Me.lblPlayerScore.TextAlign = Drawing.ContentAlignment.MiddleCenter
            '
            'pnlBanker (バンカー側のカード配置エリア)
            '
            Me.pnlBanker.BorderStyle = BorderStyle.FixedSingle
            Me.pnlBanker.Location = New Drawing.Point(400, 60)
            Me.pnlBanker.Name = "pnlBanker"
            Me.pnlBanker.Size = New Drawing.Size(300, 150)
            Me.pnlBanker.TabIndex = 6
            '
            'lblBankerScore (バンカーの点数)
            '
            Me.lblBankerScore.Location = New Drawing.Point(400, 215)
            Me.lblBankerScore.Name = "lblBankerScore"
            Me.lblBankerScore.Size = New Drawing.Size(300, 23)
            Me.lblBankerScore.Text = "Banker Score: 0"
            Me.lblBankerScore.TextAlign = Drawing.ContentAlignment.MiddleCenter
            '
            'grpBet (ベット操作エリア)
            '
            Me.grpBet.Location = New Drawing.Point(50, 260)
            Me.grpBet.Name = "grpBet"
            Me.grpBet.Size = New Drawing.Size(650, 100)
            Me.grpBet.TabIndex = 2
            Me.grpBet.TabStop = False
            Me.grpBet.Text = "Betting Area"
            '
            'btnNext (次へボタン)
            '
            Me.btnNext.Enabled = False
            Me.btnNext.Location = New Drawing.Point(556, 380)
            Me.btnNext.Name = "btnNext"
            Me.btnNext.Size = New Drawing.Size(144, 40)
            Me.btnNext.TabIndex = 3
            Me.btnNext.Text = "Next Round"
            Me.btnNext.UseVisualStyleBackColor = True
            '
            'btnRules (ルール確認ボタン)
            '
            Me.btnRules.Location = New Drawing.Point(50, 380)
            Me.btnRules.Name = "btnRules"
            Me.btnRules.Size = New Drawing.Size(100, 40)
            Me.btnRules.TabIndex = 4
            Me.btnRules.Text = "Show Rules"
            Me.btnRules.UseVisualStyleBackColor = True
            '
            'txtLog (ゲームログ)
            '
            Me.txtLog.Location = New Drawing.Point(50, 440)
            Me.txtLog.Multiline = True
            Me.txtLog.Name = "txtLog"
            Me.txtLog.ReadOnly = True
            Me.txtLog.ScrollBars = ScrollBars.Vertical
            Me.txtLog.Size = New Drawing.Size(650, 100)
            Me.txtLog.TabIndex = 7
            '
            'FormGame
            '
            Me.AutoScaleDimensions = New Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.ClientSize = New Drawing.Size(750, 560)
            Me.Controls.Add(Me.txtLog)
            Me.Controls.Add(Me.lblChips)
            Me.Controls.Add(Me.lblBankerScore)
            Me.Controls.Add(Me.pnlBanker)
            Me.Controls.Add(Me.lblPlayerScore)
            Me.Controls.Add(Me.pnlPlayer)
            Me.Controls.Add(Me.btnRules)
            Me.Controls.Add(Me.btnNext)
            Me.Controls.Add(Me.grpBet)
            Me.Controls.Add(Me.lblRound)
            Me.Controls.Add(Me.lblPhase)
            Me.Name = "FormGame"
            Me.Text = "Baccarat Game Table"
            Me.ResumeLayout(False)
            Me.PerformLayout()
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