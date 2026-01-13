Imports System.Windows.Forms

Namespace Forms
    Partial Class FormRules
        Inherits Form

        Private components As System.ComponentModel.IContainer

        Public Sub New()
            InitializeComponent()
            ' Pre-populate texts
            If txtBasic IsNot Nothing Then
                txtBasic.Text = "【バカラ 基本ルール（簡易）】" & Environment.NewLine &
                                "- プレイヤーとバンカーに2枚ずつ配られる。" & Environment.NewLine &
                                "- 点数は各カードの値を合計し、10の位を切り捨て（mod 10）。" & Environment.NewLine &
                                "  A=1、2..9=数字、10/J/Q/K=0。" & Environment.NewLine &
                                "- ナチュラル：最初の2枚で合計 8 または 9 の場合は即決着。" & Environment.NewLine &
                                "- それ以外は第三カード規則に従って1枚引く可能性がある（段階的対応）。" & Environment.NewLine &
                                "- 点数比較で高い方が勝利。同点は『タイ』。"
            End If
            If txtPayout IsNot Nothing Then
                txtPayout.Text = "【配当（プレースホルダ）】" & Environment.NewLine &
                                 "- Player 的中: 賭け額 × 1.0" & Environment.NewLine &
                                 "- Banker 的中: 賭け額 × 0.95（コミッション控除）" & Environment.NewLine &
                                 "- Tie 的中: 賭け額 × 8.0" & Environment.NewLine &
                                 "※ 実際の配当はハウスルールにより異なります。"
            End If
            If txtHowto IsNot Nothing Then
                txtHowto.Text = "【操作方法】" & Environment.NewLine &
                                 "- ベット：Player/Banker/Tie と金額を選び『Bet』で送信。" & Environment.NewLine &
                                 "- 結果：配牌と勝敗はサーバ通知を表示。" & Environment.NewLine &
                                 "- 次へ：『Next Round』で次ラウンド開始（MVPは START）。" & Environment.NewLine &
                                 "- 『Rules』で本画面をいつでも参照可能。"
            End If
        End Sub

        Private Sub InitializeComponent()
            Me.tabRules = New TabControl()
            Me.tabBasic = New TabPage()
            Me.tabPayout = New TabPage()
            Me.tabHowto = New TabPage()
            Me.txtBasic = New TextBox()
            Me.txtPayout = New TextBox()
            Me.txtHowto = New TextBox()
            Me.SuspendLayout()
            ' 
            ' tabRules
            ' 
            Me.tabRules.Dock = DockStyle.Fill
            Me.tabRules.Name = "tabRules"
            Me.tabRules.TabIndex = 0
            ' 
            ' tabBasic
            ' 
            Me.tabBasic.Name = "tabBasic"
            Me.tabBasic.Text = "基本"
            Me.tabBasic.Padding = New Padding(6)
            Me.tabBasic.UseVisualStyleBackColor = True
            ' 
            ' tabPayout
            ' 
            Me.tabPayout.Name = "tabPayout"
            Me.tabPayout.Text = "配当（予定）"
            Me.tabPayout.Padding = New Padding(6)
            Me.tabPayout.UseVisualStyleBackColor = True
            ' 
            ' tabHowto
            ' 
            Me.tabHowto.Name = "tabHowto"
            Me.tabHowto.Text = "操作"
            Me.tabHowto.Padding = New Padding(6)
            Me.tabHowto.UseVisualStyleBackColor = True
            ' 
            ' txtBasic
            ' 
            Me.txtBasic.Multiline = True
            Me.txtBasic.ReadOnly = True
            Me.txtBasic.ScrollBars = ScrollBars.Vertical
            Me.txtBasic.Dock = DockStyle.Fill
            Me.txtBasic.Name = "txtBasic"
            ' 
            ' txtPayout
            ' 
            Me.txtPayout.Multiline = True
            Me.txtPayout.ReadOnly = True
            Me.txtPayout.ScrollBars = ScrollBars.Vertical
            Me.txtPayout.Dock = DockStyle.Fill
            Me.txtPayout.Name = "txtPayout"
            ' 
            ' txtHowto
            ' 
            Me.txtHowto.Multiline = True
            Me.txtHowto.ReadOnly = True
            Me.txtHowto.ScrollBars = ScrollBars.Vertical
            Me.txtHowto.Dock = DockStyle.Fill
            Me.txtHowto.Name = "txtHowto"

            ' Add text boxes to tabs
            Me.tabBasic.Controls.Add(Me.txtBasic)
            Me.tabPayout.Controls.Add(Me.txtPayout)
            Me.tabHowto.Controls.Add(Me.txtHowto)

            ' Add tabs to tab control
            Me.tabRules.Controls.Add(Me.tabBasic)
            Me.tabRules.Controls.Add(Me.tabPayout)
            Me.tabRules.Controls.Add(Me.tabHowto)

            ' FormRules
            Me.ClientSize = New Drawing.Size(640, 480)
            Me.Controls.Add(Me.tabRules)
            Me.Name = "FormRules"
            Me.Text = "Baccarat - Rules"
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents tabRules As TabControl
        Friend WithEvents tabBasic As TabPage
        Friend WithEvents tabPayout As TabPage
        Friend WithEvents tabHowto As TabPage
        Friend WithEvents txtBasic As TextBox
        Friend WithEvents txtPayout As TextBox
        Friend WithEvents txtHowto As TextBox
    End Class
End Namespace
