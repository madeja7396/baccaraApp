Imports System.Text
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util
Imports Experiment.TcpSocket

Namespace Forms
    ''' <summary>
    ''' ロビー画面（接続・待機）
    ''' </summary>
    ''' <remarks>
    ''' 【共同開発者向けUI実装ガイド】
    ''' 
    ''' 1. UIの編集
    '''    - Visual Studio のフォームデザイナーを使用してください。
    '''    - 入力欄: txtIp (IPアドレス), txtNickname (名前)
    '''    - ボタン: btnConnect (接続), btnDisconnect (切断)
    '''    - ログ: txtLog (Multiline=True, ReadOnly=True)
    ''' 
    ''' 2. TcpSocketコンポーネント
    '''    - `TcpSockets` をフォームに配置し、`SynchronizingObject` に `FormLobby` を設定してください。
    '''    - これにより、受信イベント等がUIスレッドで実行されます。
    ''' 
    ''' 3. 画面遷移 (FormGameへ)
    '''    - ロビーで接続完了し、ゲーム開始(READY完了)となったら `FormGame` を表示します。
    '''    - 遷移例: `Dim game As New FormGame(...) : game.Show() : Me.Hide()`
    ''' 
    ''' 4. ログ出力
    '''    - `AppendLog` メソッドを経由して `txtLog` に追記してください。
    ''' </remarks>
    ''' 

    Public Class FormLobby
        Inherits System.Windows.Forms.Form

        Private Const PORT As Integer = 9000

        Private _tcp As TcpSockets = Nothing
        Private _handle As Long = -1

        ' 受信バッファ（TCPは分割されることがあるので改行まで貯める）
        Private _recvBuffer As New StringBuilder()

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FormLobby_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If String.IsNullOrWhiteSpace(txtIp.Text) Then txtIp.Text = "127.0.0.1"
            SetConnState(False)
            AppendLog("Lobby 起動")
        End Sub

        '==============================
        ' 接続
        '==============================
        Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
            If String.IsNullOrWhiteSpace(txtNickname.Text) Then
                MessageBox.Show("ニックネームを入力してください。")
                Return
            End If
            If String.IsNullOrWhiteSpace(txtIp.Text) Then
                MessageBox.Show("IPアドレスを入力してください。")
                Return
            End If

            Try
                If _tcp IsNot Nothing AndAlso _handle <> -1 Then
                    Try : _tcp.Close(_handle) : Catch : End Try
                End If

                _tcp = New TcpSockets()
                Try : _tcp.SynchronizingObject = Me : Catch : End Try

                RemoveHandler _tcp.Connect, AddressOf Tcp_Connect
                RemoveHandler _tcp.Disconnect, AddressOf Tcp_Disconnect
                RemoveHandler _tcp.DataReceive, AddressOf Tcp_DataReceive

                AddHandler _tcp.Connect, AddressOf Tcp_Connect
                AddHandler _tcp.Disconnect, AddressOf Tcp_Disconnect
                AddHandler _tcp.DataReceive, AddressOf Tcp_DataReceive

                _recvBuffer.Clear()

                AppendLog($"接続開始: {txtIp.Text}:{PORT}")
                lblStatus.Text = "🟡 接続中..."
                _handle = _tcp.OpenAsClient(txtIp.Text.Trim(), PORT)

            Catch ex As Exception
                AppendLog("接続失敗: " & ex.Message)
                _handle = -1
                SetConnState(False)
            End Try
        End Sub

        '==============================
        ' 切断
        '==============================
        Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
            Try
                If _tcp IsNot Nothing AndAlso _handle <> -1 Then
                    _tcp.Close(_handle)
                End If
            Catch ex As Exception
                AppendLog("切断エラー: " & ex.Message)
            Finally
                _handle = -1
                SetConnState(False)
                AppendLog("切断されました")
            End Try
        End Sub

        '==============================
        ' TcpSockets events
        '==============================
        Private Sub Tcp_Connect(sender As Object, e As ConnectEventArgs)
            Try
                _handle = e.Handle
                AppendLog($"接続成功: {e.RemoteEndPoint}")
                SetConnState(True)

                ' HELLO送信
                Dim msg = $"HELLO,{txtNickname.Text.Trim()}" & vbLf
                Dim bytes = Encoding.UTF8.GetBytes(msg)
                _tcp.Send(_handle, bytes)
                AppendLog("送信: " & msg.Trim())

                ' ★ここで確実にゲーム画面へ（UIスレッドで実行）
                If Me.IsHandleCreated Then
                    Me.BeginInvoke(New Action(Sub() OpenGameForm()))
                Else
                    OpenGameForm()
                End If

            Catch ex As Exception
                AppendLog("Tcp_Connect で例外: " & ex.ToString())
            End Try
        End Sub


        Private Sub Tcp_Disconnect(sender As Object, e As DisconnectEventArgs)
            AppendLog($"切断: {e.RemoteEndPoint}")
            _handle = -1
            SetConnState(False)
        End Sub

        Private Sub Tcp_DataReceive(sender As Object, e As DataReceiveEventArgs)
            Dim chunk = Encoding.UTF8.GetString(e.Data)
            _recvBuffer.Append(chunk)

            ' 改行ごとに処理（CRLF/LFどっちも対応）
            Dim all = _recvBuffer.ToString()
            Dim lines = all.Replace(vbCrLf, vbLf).Split({vbLf}, StringSplitOptions.None)

            ' 最後が改行で終わってない場合は未完了なのでバッファに戻す
            _recvBuffer.Clear()
            If Not all.EndsWith(vbLf) Then
                _recvBuffer.Append(lines(lines.Length - 1))
                ReDim Preserve lines(lines.Length - 2)
            End If

            For Each line In lines
                If String.IsNullOrWhiteSpace(line) Then Continue For
                AppendLog("受信: " & line)

                ' ===== ここが重要：サーバからの合図で画面遷移 =====
                ' 例: START / READY / GAMESTART など、どれか来たらゲーム画面へ
                Dim upper = line.Trim().ToUpperInvariant()

                If upper.StartsWith("START") OrElse upper.StartsWith("READY") OrElse upper.StartsWith("GAMESTART") Then
                    OpenGameForm()
                End If
            Next
        End Sub

        '==============================
        ' FormGameを開く
        '==============================
        Private Sub OpenGameForm()
            Try
                If Me.IsDisposed Then Return

                ' UIスレッド保証
                If Me.InvokeRequired Then
                    Me.Invoke(New Action(AddressOf OpenGameForm))
                    Return
                End If

                AppendLog("ゲーム画面へ遷移します...")

                Dim game As New FormGame(_tcp, _handle, txtNickname.Text.Trim())

                ' 落ちた時にログに出す（超重要）
                AddHandler game.Load, Sub()
                                          AppendLog("FormGame Load")
                                      End Sub
                AddHandler game.Shown, Sub()
                                           AppendLog("FormGame Shown")
                                           game.Activate()
                                           game.BringToFront()
                                       End Sub
                AddHandler game.FormClosed, Sub()
                                                AppendLog("FormGame Closed")
                                                ' ゲームを閉じたらロビーを戻す
                                                Me.Show()
                                                Me.Activate()
                                                SetConnState(False)
                                            End Sub

                game.Show()
                game.Activate()
                game.BringToFront()

                Me.Hide()

            Catch ex As Exception
                AppendLog("OpenGameForm で例外: " & ex.ToString())
            End Try
        End Sub



        '==============================
        ' 送信（1行）
        '==============================
        Private Sub SendLine(line As String)
            If _tcp Is Nothing OrElse _handle = -1 Then Return
            Dim msg = line & vbLf
            Dim bytes = Encoding.UTF8.GetBytes(msg)
            _tcp.Send(_handle, bytes)
            AppendLog("送信: " & line)
        End Sub

        '==============================
        ' UI状態切替
        '==============================
        Private Sub SetConnState(connected As Boolean)
            If InvokeRequired Then
                Invoke(New Action(Of Boolean)(AddressOf SetConnState), connected)
                Return
            End If

            btnConnect.Enabled = Not connected
            btnDisconnect.Enabled = connected
            txtIp.Enabled = Not connected
            txtNickname.Enabled = Not connected
            lblStatus.Text = If(connected, "🟢 接続済み", "⚫ 未接続")
        End Sub

        '==============================
        ' ログ
        '==============================
        Private Sub AppendLog(line As String)
            If InvokeRequired Then
                Invoke(New Action(Of String)(AddressOf AppendLog), line)
                Return
            End If
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}")
        End Sub
    End Class
End Namespace