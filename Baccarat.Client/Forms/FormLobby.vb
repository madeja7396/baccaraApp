Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util

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
    Public Class FormLobby
        Inherits System.Windows.Forms.Form

        Private _logger As Logger

        Public Sub New()
            InitializeComponent()
            _logger = New Logger(AddressOf AppendLog)
        End Sub

        Private Sub FormLobby_Load(sender As Object, e As EventArgs)
            ' TODO: hook TcpSockets events and ApplyConnState
        End Sub

        Private Sub btnConnect_Click(sender As Object, e As EventArgs)
            ' TODO: validate IP/Nickname and OpenAsClient
        End Sub

        Private Sub btnDisconnect_Click(sender As Object, e As EventArgs)
            ' TODO: close connection
        End Sub

        Private Sub AppendLog(line As String)
            ' TODO: append to txtLog
        End Sub

        Private Sub FormLobby_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load

        End Sub

        Private Sub FlowLayoutPanel1_Paint(sender As Object, e As PaintEventArgs)

        End Sub

        Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)

        End Sub

        Private Sub txtLog_TextChanged(sender As Object, e As EventArgs) Handles txtLog.TextChanged

        End Sub
    End Class
End Namespace
