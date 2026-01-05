Imports Baccarat.Server.Net
Imports Baccarat.Shared.Rules
Imports Baccarat.Shared.Util

Namespace Baccarat.Server.Forms
    Public Class FormServer
        Private _host As ServerHost

        Public Sub New()
            ' Windows Forms デザイナ用
            InitializeComponent()
            Dim logger = New Logger(AddressOf AppendLog)
            _host = New ServerHost(logger, New BaccaratRulesPlaceholder(), New PayoutCalculatorPlaceholder())
        End Sub

        Private Sub FormServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ' TODO: wire UI events to StartServer/StopServer
        End Sub

        Private Sub AppendLog(line As String)
            ' TODO: bind to txtLog whenフォームを実装
        End Sub
    End Class
End Namespace
