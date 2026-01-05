Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util

Namespace Baccarat.Client.Forms
    ''' <summary>
    ''' ゲーム画面（ベット・対戦表示）
    ''' </summary>
    ''' <remarks>
    ''' 【共同開発者向けUI実装ガイド】
    ''' 
    ''' 1. Phaseによる制御 (重要)
    '''    - `ApplyPhase(GamePhase)` メソッド内で、現在のフェーズに応じてコントロールの有効/無効を切り替えます。
    '''      (例: BETTING中のみベットボタンを有効化)
    '''    - 個別のイベントハンドラで勝手に `.Enabled = True` にしないよう注意してください。
    ''' 
    ''' 2. カード画像の表示
    '''    - `PictureBox` (例: picPlayer1, picBanker1...) を使用します。
    '''    - 画像リソースは `My.Resources` または `ImageLoader` クラス(未実装なら作成)経由で取得してください。
    ''' 
    ''' 3. ルール画面
    '''    - `btnRules` は常に有効(Enabled=True)とし、いつでも `FormRules` を参照できるようにします。
    '''    - モーダル(`ShowDialog`)ではなく、モードレス(`Show`)で表示してください。
    ''' 
    ''' 4. ログとデバッグ
    '''    - サーバーからの受信メッセージや内部状態を `txtLog` に出力するとデバッグが容易になります。
    ''' </remarks>
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

        Private Sub btnBetLock_Click(sender As Object, e As EventArgs)
            ' TODO: send BET command
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs)
            ' TODO: request next round or notify server
        End Sub

        Private Sub btnRules_Click(sender As Object, e As EventArgs)
            ' TODO: show FormRules (single instance)
        End Sub

        Private Sub AppendLog(line As String)
            ' TODO: append to txtLog
        End Sub
    End Class
End Namespace
