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
        Inherits Form

        Private Const PORT As Integer = 9000

        Private _handle As Long = -1
        Private _gameOpened As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        ' ★Loadイベントはこれ1つだけにする（他のLoad系は全部消してOK）
        Private Sub FormLobby_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ' 念のため：コントロールが生成済みかチェック
            If txtIp Is Nothing OrElse txtLog Is Nothing OrElse lblStatus Is Nothing Then
                MessageBox.Show("フォーム初期化が完了していません（Designerの破損の可能性）")
                Return
            End If

            If String.IsNullOrWhiteSpace(txtIp.Text) Then txtIp.Text = "127.0.0.1"

            btnDisconnect.Enabled = False
            btnConnect.Enabled = True
            lblStatus.Text = "未接続"

            AppendLog("Lobby 起動")
        End Sub

        Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
            If String.IsNullOrWhiteSpace(txtNickname.Text) Then
                MessageBox.Show("ニックネームを入力してください")
                Return
            End If
            If String.IsNullOrWhiteSpace(txtIp.Text) Then
                MessageBox.Show("IPアドレスを入力してください")
                Return
            End If

            ' ★tcp は Designerで生成済みのコンポーネント
            If tcp Is Nothing Then
                MessageBox.Show("tcp コンポーネントが初期化されていません（Designerを確認）")
                Return
            End If

            Try
                ' 既存接続があれば閉じる
                If _handle <> -1 Then
                    Try : tcp.Close(_handle) : Catch : End Try
                    _handle = -1
                End If

                lblStatus.Text = "接続中..."
                btnConnect.Enabled = False

                AppendLog($"接続開始: {txtIp.Text}:{PORT}")

                ' OpenAsClient
                _handle = tcp.OpenAsClient(txtIp.Text.Trim(), PORT)

            Catch ex As Exception
                AppendLog("接続失敗: " & ex.Message)
                lblStatus.Text = "未接続"
                btnConnect.Enabled = True
            End Try
        End Sub

        Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
            Try
                If _handle <> -1 Then
                    tcp.Close(_handle)
                End If
            Catch ex As Exception
                AppendLog("切断エラー: " & ex.Message)
            Finally
                _handle = -1
                lblStatus.Text = "未接続"
                btnDisconnect.Enabled = False
                btnConnect.Enabled = True
            End Try
        End Sub

        ' ===== TcpSockets events（Designerの WithEvents tcp を使うので Handles が使える）=====
        Private Sub tcp_Connect(sender As Object, e As ConnectEventArgs) Handles tcp.Connect
            _handle = e.Handle

            lblStatus.Text = "接続済み"
            btnDisconnect.Enabled = True
            btnConnect.Enabled = False

            AppendLog($"接続成功: {e.RemoteEndPoint}")

            Dim msg = $"HELLO,{txtNickname.Text.Trim()}" & vbLf
            tcp.Send(_handle, Encoding.UTF8.GetBytes(msg))
            AppendLog("送信: " & msg.Trim())

            If Not _gameOpened Then
                _gameOpened = True
                Dim game As New FormGame(tcp, _handle, txtNickname.Text.Trim())
                game.Show()
                Me.Hide()
            End If

        End Sub

        Private Sub tcp_Disconnect(sender As Object, e As DisconnectEventArgs) Handles tcp.Disconnect
            AppendLog("切断されました")
            _handle = -1
            lblStatus.Text = "未接続"
            btnDisconnect.Enabled = False
            btnConnect.Enabled = True
        End Sub

        Private Sub tcp_DataReceive(sender As Object, e As DataReceiveEventArgs) Handles tcp.DataReceive
            Dim line = Encoding.UTF8.GetString(e.Data).Trim()
            AppendLog("受信: " & line)

            ' ゲーム開始合図（サーバがPHASE,BETTINGを送る想定）
            If Not _gameOpened AndAlso line.StartsWith("PHASE,BETTING") Then
                _gameOpened = True
                Dim game As New FormGame(tcp, _handle, txtNickname.Text.Trim())
                game.Show()
                Me.Hide()
            End If
        End Sub

        Private Sub AppendLog(line As String)
            ' ★Null安全
            If txtLog Is Nothing Then Return

            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() AppendLog(line))
            Else
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {line}{Environment.NewLine}")
            End If
        End Sub

    End Class

End Namespace