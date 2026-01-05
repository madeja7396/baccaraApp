Imports Baccarat.Server.Net
Imports Baccarat.Shared.Rules
Imports Baccarat.Shared.Util

Namespace Baccarat.Server.Forms
    ''' <summary>
    ''' サーバー用メインフォーム
    ''' </summary>
    ''' <remarks>
    ''' 【共同開発者向けUI実装ガイド】
    ''' 
    ''' 1. UIの編集 (Visual Studio推奨)
    '''    - 原則として Visual Studio のフォームデザイナーを使用してください。
    '''    - ツールボックスから TextBox, Button 等を配置し、プロパティウィンドウで名前(Name)を設定してください。
    '''      (例: ログ用 -> txtLog, 起動ボタン -> btnStart)
    '''    - InitializeComponent() 内のコードは自動生成されるため、手動で書き換えないでください。
    ''' 
    ''' 2. イベントハンドラの追加
    '''    - デザイナー上のボタンをダブルクリックすると、クリックイベントが自動生成されます。
    '''    - コードで書く場合は `Handles btnStart.Click` のように記述します。
    ''' 
    ''' 3. TcpSocketコンポーネント
    '''    - ツールボックスの "Experiment.TcpSocket" から `TcpSockets` をフォームに配置します。
    '''    - プロパティ `SynchronizingObject` に `FormServer` (自分自身) を設定してください。
    '''      これにより、DataReceiveイベント等がUIスレッドで安全に実行されます。
    ''' 
    ''' 4. ログ出力
    '''    - ログは必ず `AppendLog` メソッドを経由してください。
    '''    - `TextBox` への追記は UIスレッドで行う必要があります (SynchronizingObject設定済みなら気にする必要はありません)。
    ''' </remarks>
    Public Class FormServer
        Private _host As Baccarat.Server.Net.ServerHost

        Public Sub New()
            ' Windows Forms デザイナ用
            InitializeComponent()
            Dim logger = New Logger(AddressOf AppendLog)
            _host = New Baccarat.Server.Net.ServerHost(logger, New BaccaratRulesPlaceholder(), New PayoutCalculatorPlaceholder())
        End Sub

        Private Sub FormServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ' TODO: wire UI events to StartServer/StopServer
        End Sub

        Private Sub AppendLog(line As String)
            ' TODO: bind to txtLog whenフォームを実装
        End Sub
    End Class
End Namespace

