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

        Private Const PORT As Integer = 50000

        Private _tcp As TcpSockets = Nothing
        Private _handle As Long = -1
        Private _gameOpened As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FormLobby_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If String.IsNullOrWhiteSpace(txtIp.Text) Then
                txtIp.Text = "127.0.0.1"
            End If

            lblStatus.Text = "未接続"
            btnDisconnect.Enabled = False
            AppendLog("Lobby 起動")
        End Sub

        ' ===== 接続 =====
        Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
            If String.IsNullOrWhiteSpace(txtNickname.Text) Then
                MessageBox.Show("ニックネームを入力してください")
                Return
            End If

            Try
                _tcp = New TcpSockets()

                AddHandler _tcp.Connect, AddressOf Tcp_Connect
                AddHandler _tcp.Disconnect, AddressOf Tcp_Disconnect
                AddHandler _tcp.DataReceive, AddressOf Tcp_DataReceive

                lblStatus.Text = "接続中..."
                AppendLog($"接続開始: {txtIp.Text}:{PORT}")

                _handle = _tcp.OpenAsClient(txtIp.Text.Trim(), PORT)

            Catch ex As Exception
                AppendLog("接続失敗: " & ex.Message)
            End Try
        End Sub

        ' ===== 切断 =====
        Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
            Try
                If _tcp IsNot Nothing AndAlso _handle <> -1 Then
                    _tcp.Close(_handle)
                End If
            Catch ex As Exception
                AppendLog("切断エラー: " & ex.Message)
            Finally
                _handle = -1
                lblStatus.Text = "未接続"
            End Try
        End Sub

        ' ===== TcpSockets =====
        Private Sub Tcp_Connect(sender As Object, e As ConnectEventArgs)
            _handle = e.Handle
            lblStatus.Text = "接続済み"
            btnDisconnect.Enabled = True

            AppendLog($"接続成功: {e.RemoteEndPoint}")

            Dim msg = $"HELLO,{txtNickname.Text.Trim()}" & vbLf
            _tcp.Send(_handle, Encoding.UTF8.GetBytes(msg))
            AppendLog("送信: " & msg.Trim())
        End Sub

        Private Sub Tcp_Disconnect(sender As Object, e As DisconnectEventArgs)
            AppendLog("切断されました")
            lblStatus.Text = "未接続"
            _handle = -1
        End Sub

        Private Sub Tcp_DataReceive(sender As Object, e As DataReceiveEventArgs)
            Dim line = Encoding.UTF8.GetString(e.Data).Trim()
            AppendLog("受信: " & line)

            ' ===== ゲーム開始合図 =====
            If Not _gameOpened AndAlso line.StartsWith("PHASE,BETTING") Then
                _gameOpened = True

                ' Lobby側のイベント解除（任意）
                RemoveHandler _tcp.DataReceive, AddressOf Tcp_DataReceive
                RemoveHandler _tcp.Disconnect, AddressOf Tcp_Disconnect

                Dim game As New FormGame(_tcp, _handle, txtNickname.Text.Trim())
                game.Show()
                Me.Hide()
            End If
        End Sub

        ' ===== ログ =====
        Private Sub AppendLog(line As String)
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() AppendLog(line))
            Else
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {line}{Environment.NewLine}")
            End If
        End Sub

    End Class
End Namespace