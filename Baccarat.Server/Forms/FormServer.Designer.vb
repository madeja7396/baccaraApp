Imports System.Windows.Forms

Namespace Forms
    Partial Class FormServer
        Inherits Form

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.TcpSockets1 = New Experiment.TcpSocket.TcpSockets(Me.components)
            Me.txtLog = New TextBox()
            Me.btnStart = New Button()
            Me.btnStop = New Button()
            Me.lblStatus = New Label()
            Me.SuspendLayout()
            '
            'TcpSockets1
            '
            Me.TcpSockets1.ReceiveBufferSize = 8192
            Me.TcpSockets1.SendBufferSize = 8192
            Me.TcpSockets1.SynchronizingObject = Me
            '
            'txtLog (サーバーログ表示用)
            '
            Me.txtLog.Location = New Drawing.Point(12, 50)
            Me.txtLog.Multiline = True
            Me.txtLog.Name = "txtLog"
            Me.txtLog.ReadOnly = True
            Me.txtLog.ScrollBars = ScrollBars.Vertical
            Me.txtLog.Size = New Drawing.Size(460, 300)
            Me.txtLog.TabIndex = 0
            '
            'btnStart (サーバー開始ボタン)
            '
            Me.btnStart.Location = New Drawing.Point(12, 12)
            Me.btnStart.Name = "btnStart"
            Me.btnStart.Size = New Drawing.Size(100, 30)
            Me.btnStart.TabIndex = 1
            Me.btnStart.Text = "Start Server"
            Me.btnStart.UseVisualStyleBackColor = True
            '
            'btnStop (サーバー停止ボタン)
            '
            Me.btnStop.Location = New Drawing.Point(118, 12)
            Me.btnStop.Name = "btnStop"
            Me.btnStop.Size = New Drawing.Size(100, 30)
            Me.btnStop.TabIndex = 2
            Me.btnStop.Text = "Stop Server"
            Me.btnStop.UseVisualStyleBackColor = True
            '
            'lblStatus (現在の状態表示)
            '
            Me.lblStatus.AutoSize = True
            Me.lblStatus.Location = New Drawing.Point(230, 20)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New Drawing.Size(110, 15)
            Me.lblStatus.TabIndex = 3
            Me.lblStatus.Text = "Status: Stopped"
            '
            'FormServer
            '
            Me.AutoScaleDimensions = New Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.ClientSize = New Drawing.Size(484, 361)
            Me.Controls.Add(Me.lblStatus)
            Me.Controls.Add(Me.btnStop)
            Me.Controls.Add(Me.btnStart)
            Me.Controls.Add(Me.txtLog)
            Me.Name = "FormServer"
            Me.Text = "Baccarat Server"
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        ' 共同開発者へのメモ: これらのコントロールはデザイナーから編集可能です
        Friend WithEvents txtLog As TextBox
        Friend WithEvents btnStart As Button
        Friend WithEvents btnStop As Button
        Friend WithEvents lblStatus As Label
        Friend WithEvents TcpSockets1 As Experiment.TcpSocket.TcpSockets
    End Class
End Namespace