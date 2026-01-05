Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util

Namespace Baccarat.Client.Forms
    Public Class FormLobby
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

        Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
            ' TODO: close connection
        End Sub

        Private Sub AppendLog(line As String)
            ' TODO: append to txtLog
        End Sub
    End Class
End Namespace
