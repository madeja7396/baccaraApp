Imports Experiment.TcpSocket
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Util
Imports System.Text
Imports SharedConstants = Baccarat.Shared.Constants

Namespace Forms
    Partial Public Class FormLobby
        Private WithEvents _tcp As TcpSockets
        Private _handle As Long = -1
        Private _framer As LineFramer

        Private Sub OnFormLoad(sender As Object, e As EventArgs) Handles MyBase.Load
            _tcp = New TcpSockets()
            _tcp.SynchronizingObject = Me
            _framer = New LineFramer()
            AddHandler _framer.LineReceived, AddressOf OnLineReceived

            AddHandler btnConnect.Click, AddressOf OnBtnConnect
        End Sub

        Private Sub OnBtnConnect(sender As Object, e As EventArgs)
            Dim ip = txtIp.Text.Trim()
            Dim nick = txtNickname.Text.Trim()
            If String.IsNullOrWhiteSpace(nick) Then
                AppendLog("Nickname is required.")
                Return
            End If
            Try
                _handle = _tcp.OpenAsClient(ip, SharedConstants.Port)
                AppendLog($"Connecting to {ip}:{SharedConstants.Port} ...")
            Catch ex As Exception
                AppendLog($"Connect error: {ex.Message}")
            End Try
        End Sub

        Private Sub _tcp_Connect(sender As Object, e As ConnectEventArgs) Handles _tcp.Connect
            AppendLog($"[CONNECT] {e.RemoteEndPoint}")
            Dim line As String = $"{CommandNames.HELLO},{txtNickname.Text.Trim()}"
            Dim data As Byte() = Encoding.UTF8.GetBytes(line & vbLf)
            _tcp.Send(e.Handle, data)
        End Sub

        Private Sub _tcp_DataReceive(sender As Object, e As DataReceiveEventArgs) Handles _tcp.DataReceive
            Dim text As String = Encoding.UTF8.GetString(e.Data)
            _framer.Push(text)
        End Sub

        Private Sub _tcp_Disconnect(sender As Object, e As DisconnectEventArgs) Handles _tcp.Disconnect
            AppendLog($"[DISCONNECT] {e.RemoteEndPoint}")
            _handle = -1
        End Sub

        Private Sub OnLineReceived(line As String)
            AppendLog($"[SRV] {line}")
            ' TODO: parse WELCOME and transition
        End Sub
    End Class
End Namespace