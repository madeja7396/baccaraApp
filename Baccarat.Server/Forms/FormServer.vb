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
            _host = New Net.ServerHost(logger, New BaccaratRulesPlaceholder(), New PayoutCalculatorPlaceholder(), AddressOf SendMessage)
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
            ' T1-00: Data property is available on DataReceiveEventArgs per experiment spec.
            Try
                Dim text As String = Encoding.UTF8.GetString(e.Data)

                If _framers.ContainsKey(e.Handle) Then
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
                Dim data As Byte() = Encoding.UTF8.GetBytes(message & vbLf)
                TcpSockets1.Send(handle, data)
                AppendLog($"[SND] [{handle}] {message}")
            Catch ex As Exception
                AppendLog($"[ERR] SendMessage: {ex.Message}")
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

