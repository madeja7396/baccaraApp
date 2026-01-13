Imports System.Text
Imports System.Linq
Imports System.Windows.Forms
Imports Experiment.TcpSocket
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util
Imports Baccarat.Client.Util
Imports System.Collections.Generic
Imports System.IO
Imports System.Drawing


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
        Inherits System.Windows.Forms.Form

        Private ReadOnly _tcp As TcpSockets
        Private ReadOnly _handle As Long
        Private ReadOnly _nick As String

        Private ReadOnly _rx As New StringBuilder()
        Private ReadOnly _imgCache As New Dictionary(Of String, Image)(StringComparer.OrdinalIgnoreCase)

        ' 表示用状態（サーバから来た内容を反映）
        Private _round As Integer = 0
        Private _chips As Integer = 1000
        Private _phase As String = "WAITING"

        ' ---- コンストラクタ：Lobbyから tcp/handle を受け取る ----
        Public Sub New(tcp As TcpSockets, handle As Long, nickname As String)
            InitializeComponent()
            _tcp = tcp
            _handle = handle
            _nick = nickname
        End Sub

        Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ' サーバ受信イベント（重要）
            AddHandler _tcp.DataReceive, AddressOf Tcp_DataReceive
            AddHandler _tcp.Disconnect, AddressOf Tcp_Disconnect

            ' UI初期化
            rbPlayer.Checked = True
            nudBet.Minimum = 10
            nudBet.Maximum = 10000
            nudBet.Increment = 10
            If nudBet.Value < 100 Then nudBet.Value = 100

            lblBetInfo.Text = "選択: -"
            lblResult.Text = "結果: -"

            pnlPlayer.Controls.Clear()
            pnlBanker.Controls.Clear()

            ApplyHeader()

            AppendLog("Game start: " & _nick)

            ' サーバへ：HELLOはLobbyで送っているはずだが、二重送信しても害が少ない
            SendLine($"HELLO,{_nick}")

            ' サーバへ：準備完了（ServerHost側が READY を期待している想定）
            SendLine("READY")
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            MyBase.OnFormClosed(e)

            ' イベント解除（多重購読防止）
            Try
                RemoveHandler _tcp.DataReceive, AddressOf Tcp_DataReceive
                RemoveHandler _tcp.Disconnect, AddressOf Tcp_Disconnect
            Catch
            End Try
        End Sub

        ' -----------------------------
        ' UI
        ' -----------------------------
        Private Sub ApplyHeader()
            lblPhase.Text = $"Phase: {_phase}"
            lblRound.Text = $"Round: {_round}"
            lblChips.Text = $"Chips: {_chips}"
        End Sub

        Private Sub AppendLog(line As String)
            If InvokeRequired Then
                Invoke(New Action(Of String)(AddressOf AppendLog), line)
                Return
            End If
            txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {line}{Environment.NewLine}")
        End Sub

        Private Function SelectedTargetChar() As String
            If rbBanker.Checked Then Return "B"
            If rbTie.Checked Then Return "T"
            Return "P"
        End Function

        Private Sub UpdateBetInfo()
            Dim t = SelectedTargetChar()
            lblBetInfo.Text = $"選択: {t} / Bet: {CInt(nudBet.Value)}"
        End Sub

        Private Sub rbPlayer_CheckedChanged(sender As Object, e As EventArgs) Handles rbPlayer.CheckedChanged
            UpdateBetInfo()
        End Sub
        Private Sub rbBanker_CheckedChanged(sender As Object, e As EventArgs) Handles rbBanker.CheckedChanged
            UpdateBetInfo()
        End Sub
        Private Sub rbTie_CheckedChanged(sender As Object, e As EventArgs) Handles rbTie.CheckedChanged
            UpdateBetInfo()
        End Sub
        Private Sub nudBet_ValueChanged(sender As Object, e As EventArgs) Handles nudBet.ValueChanged
            UpdateBetInfo()
        End Sub

        ' -----------------------------
        ' Buttons
        ' -----------------------------
        Private Sub btnBet_Click(sender As Object, e As EventArgs) Handles btnBet.Click
            Dim t = SelectedTargetChar()
            Dim amt = CInt(nudBet.Value)

            ' ServerHostが理解するBETを送る（既にあなたがHELLO形式で進めているのでこの形式を維持）
            SendLine($"BET,{t},{amt}")
            AppendLog($"Bet sent: {t} {amt}")
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
            ' 次ラウンド/配る要求
            SendLine("DEAL")
            AppendLog("Deal requested")
        End Sub

        Private Sub btnRules_Click(sender As Object, e As EventArgs) Handles btnRules.Click
            Try
                Dim f As New FormRules()
                f.Show()
            Catch
                MessageBox.Show("FormRules が見つかりません。", "Rules")
            End Try
        End Sub

        ' -----------------------------
        ' Network Send/Receive
        ' -----------------------------
        Private Sub SendLine(line As String)
            Dim msg = line & vbLf
            Dim bytes = Encoding.UTF8.GetBytes(msg)
            _tcp.Send(_handle, bytes)
        End Sub

        Private Sub Tcp_Disconnect(sender As Object, e As DisconnectEventArgs)
            AppendLog("Disconnected from server.")
        End Sub

        Private Sub Tcp_DataReceive(sender As Object, e As DataReceiveEventArgs)
            Dim text = Encoding.UTF8.GetString(e.Data)

            If InvokeRequired Then
                Invoke(New Action(Of String)(AddressOf RxAppend), text)
            Else
                RxAppend(text)
            End If
        End Sub

        Private Sub RxAppend(text As String)
            _rx.Append(text)

            Dim bufStr = _rx.ToString()
            Dim lines = bufStr.Split({vbLf}, StringSplitOptions.None)

            _rx.Clear()
            If Not bufStr.EndsWith(vbLf) Then
                _rx.Append(lines.Last())
                lines = lines.Take(lines.Length - 1).ToArray()
            Else
                lines = lines.Take(lines.Length - 1).ToArray()
            End If

            For Each raw In lines
                Dim line = raw.Trim()
                If line.Length = 0 Then Continue For
                AppendLog("[RCV] " & line)
                HandleServerLine(line)
            Next
        End Sub

        ' -----------------------------
        ' Server → UI 反映
        ' ここは「サーバが送ってくる行」に合わせて増やせる
        ' -----------------------------
        Private Sub HandleServerLine(line As String)
            Dim parts = line.Split(","c)
            Dim cmd = parts(0).Trim().ToUpperInvariant()

            Select Case cmd

                Case "STATE"
                    ' STATE,<round>,<chips>,<phase>
                    If parts.Length >= 4 Then
                        Integer.TryParse(parts(1), _round)
                        Integer.TryParse(parts(2), _chips)
                        _phase = parts(3).Trim()
                        ApplyHeader()

                        ' フェーズに応じて操作可否
                        grpBet.Enabled = (_phase = "BETTING")
                        btnNext.Enabled = (_phase = "BETTING" OrElse _phase = "RESULT")
                    End If

                Case "CARDS"
                    ' CARDS,P:00_00|01_00|..,B:00_01|..
                    ParseAndRenderCards(line)

                Case "SCORE"
                    ' SCORE,<pScore>,<bScore>
                    If parts.Length >= 3 Then
                        lblPlayerScore.Text = $"Player Score: {parts(1).Trim()}"
                        lblBankerScore.Text = $"Banker Score: {parts(2).Trim()}"
                    End If

                Case "RESULT"
                    ' RESULT,<winner>,<delta>,<chips>
                    If parts.Length >= 4 Then
                        Dim w = parts(1).Trim()
                        Dim delta = parts(2).Trim()
                        Dim chipsStr = parts(3).Trim()
                        lblResult.Text = $"結果: {w} / 収支: {delta}"
                        Integer.TryParse(chipsStr, _chips)
                        ApplyHeader()
                    End If

                Case Else
                    ' 未対応メッセージはログだけ（ServerHostの実装に合わせてここを増やす）
            End Select
        End Sub

        Private Sub ParseAndRenderCards(fullLine As String)
            Dim pPart As String = ""
            Dim bPart As String = ""

            Dim rest = fullLine.Substring(5).Trim() ' "CARDS"分
            If rest.StartsWith(","c) Then rest = rest.Substring(1).Trim()

            Dim segs = rest.Split(","c)
            For Each seg In segs
                Dim t = seg.Trim()
                If t.StartsWith("P:", StringComparison.OrdinalIgnoreCase) Then pPart = t.Substring(2)
                If t.StartsWith("B:", StringComparison.OrdinalIgnoreCase) Then bPart = t.Substring(2)
            Next

            RenderPanelCards(pnlPlayer, pPart)
            RenderPanelCards(pnlBanker, bPart)
        End Sub

        Private Sub RenderPanelCards(panel As Panel, payload As String)
            panel.Controls.Clear()

            If String.IsNullOrWhiteSpace(payload) Then Return
            Dim items = payload.Split("|"c).Where(Function(s) Not String.IsNullOrWhiteSpace(s)).ToArray()

            Dim x As Integer = 10
            Dim y As Integer = 10
            Dim w As Integer = 120
            Dim h As Integer = 180
            Dim gap As Integer = 10

            For i As Integer = 0 To items.Length - 1
                Dim code = items(i).Trim() ' "00_00"
                Dim pb As New PictureBox() With {
                    .Left = x + i * (w + gap),
                    .Top = y,
                    .Width = w,
                    .Height = h,
                    .SizeMode = PictureBoxSizeMode.Zoom,
                    .BorderStyle = BorderStyle.FixedSingle
                }

                Dim img = LoadCardImage(code)
                If img IsNot Nothing Then pb.Image = img

                panel.Controls.Add(pb)
            Next
        End Sub

        ' assets/cards/00_00.bmp を読む
        Private Function LoadCardImage(code As String) As Image
            Dim fileName = code & ".bmp"
            If _imgCache.ContainsKey(fileName) Then Return _imgCache(fileName)

            Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
            Dim path1 = Path.Combine(baseDir, "assets", "cards", fileName)
            Dim path2 = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "assets", "cards", fileName))

            Dim found As String = Nothing
            If File.Exists(path1) Then found = path1
            If found Is Nothing AndAlso File.Exists(path2) Then found = path2

            If found Is Nothing Then
                AppendLog($"[WARN] image not found: {fileName}")
                Return Nothing
            End If

            Using fs As New FileStream(found, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim img = Image.FromStream(fs)
                Dim clone As New Bitmap(img)
                _imgCache(fileName) = clone
                Return clone
            End Using
        End Function

    End Class

End Namespace
