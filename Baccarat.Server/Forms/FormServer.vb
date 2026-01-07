Imports Experiment.TcpSocket
Imports Baccarat.Shared.Rules
Imports Baccarat.Shared.Util
Imports Baccarat.Shared.Protocol
Imports System.Text
Imports System.Collections.Generic

Namespace Forms
    ''' <summary>
    ''' サーバー用メインフォーム
    ''' </summary>
    ''' <remarks>
    ''' 【共同開発者向け UI 実装ヒント（追記）】
    ''' - 目的: サーバーの起動/停止、接続イベントのログ可視化、`ServerHost` との橋渡し。
    ''' - コントロール命名: 起動/停止ボタンは `btnStart`/`btnStop`、ログは `txtLog` を使用。
    ''' - TcpSockets コンポーネント: デザイナー上に貼り付け、`SynchronizingObject=Me` に設定（UIスレッドでイベントを受ける）。
    ''' - 受信処理: `DataReceive` → UTF-8 文字列化 → `LineFramer` に `Push` → 1行ごとに `ServerHost.OnLineReceived` へ。
    ''' - 送信処理: `ServerHost` から受け取った文字列に LF を付与して `TcpSockets1.Send`。必ず 1 行 = 1 メッセージ。
    ''' - ログ: UI スレッド保証あり。`AppendLog` 経由で時刻付きで追記（デバッグで重要）。
    ''' - 最低限の結合テスト手順:
    '''   1) サーバを起動（btnStart）→ ログにポートが出る
    '''   2) クライアントから接続 → `Accept`, `HELLO` が行として流れる
    '''   3) `READY` を両者から送ると `PHASE,BETTING` が Broadcast される
    ''' </remarks>
    Public Class FormServer
        Inherits System.Windows.Forms.Form

        Private _host As Net.ServerHost
        Private _framers As New Dictionary(Of Long, LineFramer)
        Private _serverHandle As Long = -1

        Public Sub New()
            ' Windows Forms デザイナ用
            InitializeComponent()
            Dim logger = New Logger(AddressOf AppendLog)
            ' ServerHost needs to be able to send data back. Inject sendAction.
            _host = New Net.ServerHost(logger, New BaccaratRulesPlaceholder(), New PayoutCalculatorPlaceholder(), AddressOf SendMessage, AddressOf CloseConnection)
        End Sub

        Private Sub FormServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            AddHandler btnStart.Click, AddressOf btnStart_Click
            AddHandler btnStop.Click, AddressOf btnStop_Click

            ' Wire up TcpSockets events
            AddHandler TcpSockets1.Accept, AddressOf OnAccept
            AddHandler TcpSockets1.Disconnect, AddressOf OnDisconnect
            AddHandler TcpSockets1.DataReceive, AddressOf OnDataReceive
        End Sub

        Private Sub btnStart_Click(sender As Object, e As EventArgs)
            Try
                ' Use the port from Constants
                Dim port = Baccarat.Shared.Constants.Port
                _serverHandle = TcpSockets1.OpenAsServer(port)
                AppendLog($"Server started on port {port}")
                lblStatus.Text = "Status: Running"
                btnStart.Enabled = False
                btnStop.Enabled = True
                _host.StartServer(port)
            Catch ex As Exception
                AppendLog($"Error starting server: {ex.Message}")
            End Try
        End Sub

        Private Sub btnStop_Click(sender As Object, e As EventArgs)
            Try
                If _serverHandle <> -1 Then
                    TcpSockets1.Close(_serverHandle)
                    _serverHandle = -1
                End If

                AppendLog("Stopping server...")
                lblStatus.Text = "Status: Stopped"
                btnStart.Enabled = True
                btnStop.Enabled = False
                _host.StopServer()
            Catch ex As Exception
                AppendLog($"Error stopping server: {ex.Message}")
            End Try
        End Sub

        Private Sub OnAccept(sender As Object, e As AcceptEventArgs)
            AppendLog($"[ACCEPT] ClientHandle={e.ClientHandle}, Remote={e.RemoteEndPoint}")

            Dim framer As New LineFramer()
            AddHandler framer.LineReceived, Sub(line)
                                                _host.OnLineReceived(e.ClientHandle, line)
                                            End Sub
            _framers(e.ClientHandle) = framer

            _host.OnAccept(e.ClientHandle)
        End Sub

        Private Sub OnDisconnect(sender As Object, e As DisconnectEventArgs)
            AppendLog($"[DISCONNECT] Handle={e.Handle}")
            If _framers.ContainsKey(e.Handle) Then
                _framers.Remove(e.Handle)
            End If
            _host.OnDisconnect(e.Handle)
        End Sub

        Private Sub OnDataReceive(sender As Object, e As DataReceiveEventArgs)
            ' データ受信イベントが発生したときの処理
            Try
                ' 受信データを UTF-8 文字列に変換
                Dim text As String = Encoding.UTF8.GetString(e.Data)

                If _framers.ContainsKey(e.Handle) Then
                    ' 対応するフレーマーがあればデータをプッシュ
                    _framers(e.Handle).Push(text)
                Else
                    AppendLog($"[WARN] Received data from unknown handle {e.Handle}")
                End If
            Catch ex As Exception
                AppendLog($"[ERR] DataReceive: {ex.Message}")
            End Try
        End Sub

        Private Sub SendMessage(handle As Long, message As String)
            Try
                ' メッセージに LF を付加してバイト配列に変換
                Dim data As Byte() = Encoding.UTF8.GetBytes(message & vbLf)
                TcpSockets1.Send(handle, data)
                AppendLog($"[SND] [{handle}] {message}")
            Catch ex As Exception
                AppendLog($"[ERR] SendMessage: {ex.Message}")
            End Try
        End Sub

        Private Sub CloseConnection(handle As Long)
            Try
                TcpSockets1.Close(handle)
                AppendLog($"[CLOSE] {handle}")
            Catch ex As Exception
                AppendLog($"[ERR] Close: {ex.Message}")
            End Try
        End Sub

        Private Sub AppendLog(line As String)
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() AppendLog(line))
            Else
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}" & Environment.NewLine)
            End If
        End Sub
    End Class
End Namespace

