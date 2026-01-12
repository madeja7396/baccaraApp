Imports System.Text
Imports System.Linq
Imports System.Windows.Forms
Imports Experiment.TcpSocket
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util
Imports Baccarat.Client.Util

Namespace Forms
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

    ''' <summary>
    ''' ゲーム画面（ベット・対戦表示）
    ''' </summary>


    Public Class FormGame
        Inherits Form

        Private _logger As Logger
        Private _phase As GamePhase = GamePhase.LOBBY

        Private _tcp As TcpSockets
        Private _handle As Long
        Private _nickname As String

        ' ベット状態（最低限）
        Private _betTarget As String = "PLAYER"   ' PLAYER / BANKER / TIE
        Private _betAmount As Integer = 100       ' とりあえず固定

        ' 受信バッファ（分割受信対策）
        Private _rxBuf As String = ""

        Public Sub New(tcp As TcpSockets, handle As Long, nickname As String)
            InitializeComponent()
            _logger = New Logger(AddressOf AppendLog)

            _tcp = tcp
            _handle = handle
            _nickname = nickname

            AddHandler _tcp.DataReceive, AddressOf Tcp_DataReceive
            AddHandler _tcp.Disconnect, AddressOf Tcp_Disconnect

            ApplyPhase(GamePhase.LOBBY)
            SetTextIfExists("lblPhase", "Phase: WAITING")
            SetTextIfExists("lblRound", "Round: 0")
            SetTextIfExists("lblChips", "Chips: 1000")

            AppendLog("Game 起動: " & _nickname)
        End Sub

        '==============================
        ' Phase制御（UIの有効/無効）
        '==============================
        Public Sub ApplyPhase(p As GamePhase)
            _phase = p

            SetEnabledIfExists("grpBet", (p = GamePhase.BETTING))
            SetEnabledIfExists("btnNext", (p = GamePhase.RESULT))
            SetEnabledIfExists("btnRules", True)

            ' 表示
            SetTextIfExists("lblPhase", "Phase: " & p.ToString())
        End Sub

        '==============================
        ' ボタン（ベット選択）
        '  ※ デザイナにボタンが無くてもコンパイルは通るように
        '==============================
        Private Sub btnBetPlayer_Click(sender As Object, e As EventArgs)
            _betTarget = "PLAYER"
            AppendLog("BetTarget=PLAYER")
        End Sub

        Private Sub btnBetBanker_Click(sender As Object, e As EventArgs)
            _betTarget = "BANKER"
            AppendLog("BetTarget=BANKER")
        End Sub

        Private Sub btnBetTie_Click(sender As Object, e As EventArgs)
            _betTarget = "TIE"
            AppendLog("BetTarget=TIE")
        End Sub

        Private Sub btnBetLock_Click(sender As Object, e As EventArgs)
            If _phase <> GamePhase.BETTING Then
                AppendLog("BETTINGフェーズではないためベットできません")
                Return
            End If

            ' 送信形式（サーバ側仕様に合わせて必要なら変更）
            ' 例: BET,<nickname>,<target>,<amount>
            SendLine($"BET,{_nickname},{_betTarget},{_betAmount}")
            AppendLog($"送信: BET {_betTarget} {_betAmount}")
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs)
            ' 例: NEXT,<nickname>
            SendLine($"NEXT,{_nickname}")
            AppendLog("送信: NEXT")
        End Sub

        Private Sub btnRules_Click(sender As Object, e As EventArgs)
            Dim rules As New FormRules()
            rules.Show()
        End Sub

        '==============================
        ' 送信
        '==============================
        Private Sub SendLine(line As String)
            Dim bytes = Encoding.UTF8.GetBytes(line & vbLf)
            _tcp.Send(_handle, bytes)
        End Sub

        '==============================
        ' 受信
        '==============================
        Private Sub Tcp_DataReceive(sender As Object, e As DataReceiveEventArgs)
            Dim chunk = Encoding.UTF8.GetString(e.Data)
            _rxBuf &= chunk

            ' 改行区切りで処理
            Dim lines = _rxBuf.Split(New String() {vbLf}, StringSplitOptions.None)
            _rxBuf = lines.Last() ' 最後は未完の可能性があるのでバッファへ

            For i As Integer = 0 To lines.Length - 2
                Dim line = lines(i).Trim()
                If line <> "" Then
                    HandleLine(line)
                End If
            Next
        End Sub

        Private Sub Tcp_Disconnect(sender As Object, e As DisconnectEventArgs)
            AppendLog("切断されました")
            ApplyPhase(GamePhase.LOBBY)
        End Sub

        '==============================
        ' プロトコル処理（ここがゲームの中心）
        '==============================
        Private Sub HandleLine(line As String)
            AppendLog("受信: " & line)

            ' 想定メッセージ例（必要に応じてサーバに合わせて変更）
            ' PHASE,BETTING,0
            ' PHASE,DEALING,0
            ' PHASE,RESULT,0
            ' CHIPS,1000
            ' CARDS,P,00_00,01_01,02_02  （Playerのカード）
            ' CARDS,B,03_03,04_00        （Bankerのカード）
            ' SCORE,5,7
            ' RESULT,BANKER

            Dim parts = line.Split(","c)
            If parts.Length = 0 Then Return

            Select Case parts(0)

                Case "PHASE"
                    If parts.Length >= 2 Then
                        Dim pStr = parts(1).Trim()
                        If pStr = "BETTING" Then ApplyPhase(GamePhase.BETTING)
                        If pStr = "DEALING" Then ApplyPhase(GamePhase.DEALING)
                        If pStr = "RESULT" Then ApplyPhase(GamePhase.RESULT)
                        If parts.Length >= 3 Then SetTextIfExists("lblRound", "Round: " & parts(2).Trim())
                    End If

                Case "CHIPS"
                    If parts.Length >= 2 Then
                        SetTextIfExists("lblChips", "Chips: " & parts(1).Trim())
                    End If

                Case "CARDS"
                    If parts.Length >= 3 Then
                        Dim side = parts(1).Trim() ' P or B
                        Dim cards = parts.Skip(2).Where(Function(x) x.Trim() <> "").ToArray()

                        If side = "P" Then
                            ShowCardsPlayer(cards)
                            SetTextIfExists("lblPlayerScore", "Player Score: " & CalcScore(cards))
                        ElseIf side = "B" Then
                            ShowCardsBanker(cards)
                            SetTextIfExists("lblBankerScore", "Banker Score: " & CalcScore(cards))
                        End If
                    End If

                Case "SCORE"
                    If parts.Length >= 3 Then
                        SetTextIfExists("lblPlayerScore", "Player Score: " & parts(1).Trim())
                        SetTextIfExists("lblBankerScore", "Banker Score: " & parts(2).Trim())
                    End If

                Case "RESULT"
                    If parts.Length >= 2 Then
                        AppendLog("勝者: " & parts(1).Trim())
                    End If

            End Select
        End Sub

        '==============================
        ' スコア計算
        '==============================
        Private Function CalcScore(cards As String()) As Integer
            Dim sum As Integer = 0
            For Each c In cards
                sum += BaccaratPoint(c.Trim())
            Next
            Return sum Mod 10
        End Function

        '==============================
        ' カード表示（PictureBox名が無い場合でも落ちない）
        ' 期待する名前: picPlayer1/2/3, picBanker1/2/3
        '==============================
        Private Sub ShowCardsPlayer(cards As String())
            SetCardIfExists("picPlayer1", If(cards.Length >= 1, cards(0), Nothing))
            SetCardIfExists("picPlayer2", If(cards.Length >= 2, cards(1), Nothing))
            SetCardIfExists("picPlayer3", If(cards.Length >= 3, cards(2), Nothing))
        End Sub

        Private Sub ShowCardsBanker(cards As String())
            SetCardIfExists("picBanker1", If(cards.Length >= 1, cards(0), Nothing))
            SetCardIfExists("picBanker2", If(cards.Length >= 2, cards(1), Nothing))
            SetCardIfExists("picBanker3", If(cards.Length >= 3, cards(2), Nothing))
        End Sub

        Private Sub SetCardIfExists(ctrlName As String, cardId As String)
            Dim pb = TryCast(Me.Controls.Find(ctrlName, True).FirstOrDefault(), PictureBox)
            If pb Is Nothing Then Return

            If String.IsNullOrWhiteSpace(cardId) Then
                pb.Image = Nothing
                Return
            End If

            Try
                pb.SizeMode = PictureBoxSizeMode.Zoom
                pb.Image = LoadCardImage(cardId)
            Catch ex As Exception
                AppendLog($"画像読み込み失敗({ctrlName}): {cardId} / {ex.Message}")
            End Try
        End Sub

        '==============================
        ' 汎用UI操作
        '==============================
        Private Sub SetTextIfExists(ctrlName As String, text As String)
            Dim c = Me.Controls.Find(ctrlName, True).FirstOrDefault()
            Dim lbl = TryCast(c, Label)
            If lbl IsNot Nothing Then lbl.Text = text
        End Sub

        Private Sub SetEnabledIfExists(ctrlName As String, enabled As Boolean)
            Dim c = Me.Controls.Find(ctrlName, True).FirstOrDefault()
            If c IsNot Nothing Then c.Enabled = enabled
        End Sub

        '==============================
        ' ログ
        '==============================
        Private Sub AppendLog(line As String)
            Dim tb = TryCast(Me.Controls.Find("txtLog", True).FirstOrDefault(), TextBox)
            If tb Is Nothing Then Return

            If tb.InvokeRequired Then
                tb.Invoke(Sub() AppendLog(line))
            Else
                tb.AppendText($"{DateTime.Now:HH:mm:ss} {line}{Environment.NewLine}")
            End If
        End Sub

        Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        End Sub
    End Class

End Namespace
