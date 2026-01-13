Imports System.Windows.Forms

Namespace Forms
    Partial Class FormLobby
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
            txtIp = New TextBox()
            txtNickname = New TextBox()
            btnConnect = New Button()
            lblIp = New Label()
            lblNick = New Label()
            txtLog = New TextBox()
            lblStatus = New Label()
            Label1 = New Label()
            btnDisconnect = New Button()
            SuspendLayout()
            ' 
            ' txtIp
            ' 
            txtIp.Location = New Point(171, 20)
            txtIp.Margin = New Padding(4, 5, 4, 5)
            txtIp.Name = "txtIp"
            txtIp.Size = New Size(213, 31)
            txtIp.TabIndex = 7
            txtIp.Text = "127.0.0.1"
            ' 
            ' txtNickname
            ' 
            txtNickname.Location = New Point(171, 63)
            txtNickname.Margin = New Padding(4, 5, 4, 5)
            txtNickname.Name = "txtNickname"
            txtNickname.Size = New Size(213, 31)
            txtNickname.TabIndex = 5
            ' 
            ' btnConnect
            ' 
            btnConnect.Location = New Point(50, 126)
            btnConnect.Margin = New Padding(4, 5, 4, 5)
            btnConnect.Name = "btnConnect"
            btnConnect.Size = New Size(134, 50)
            btnConnect.TabIndex = 2
            btnConnect.Text = "ê⁄ë±"
            btnConnect.UseVisualStyleBackColor = True
            ' 
            ' lblIp
            ' 
            lblIp.Location = New Point(29, 20)
            lblIp.Margin = New Padding(4, 0, 4, 0)
            lblIp.Name = "lblIp"
            lblIp.Size = New Size(143, 38)
            lblIp.TabIndex = 6
            lblIp.Text = "Server IP:"
            ' 
            ' lblNick
            ' 
            lblNick.Location = New Point(29, 63)
            lblNick.Margin = New Padding(4, 0, 4, 0)
            lblNick.Name = "lblNick"
            lblNick.Size = New Size(143, 38)
            lblNick.TabIndex = 4
            lblNick.Text = "Nickname:"
            ' 
            ' txtLog
            ' 
            txtLog.Location = New Point(29, 262)
            txtLog.Margin = New Padding(4, 5, 4, 5)
            txtLog.Multiline = True
            txtLog.Name = "txtLog"
            txtLog.ReadOnly = True
            txtLog.ScrollBars = ScrollBars.Vertical
            txtLog.Size = New Size(377, 187)
            txtLog.TabIndex = 3
            ' 
            ' lblStatus
            ' 
            lblStatus.AutoSize = True
            lblStatus.Location = New Point(109, 207)
            lblStatus.Name = "lblStatus"
            lblStatus.Size = New Size(66, 25)
            lblStatus.TabIndex = 11
            lblStatus.Text = "ñ¢ê⁄ë±"
            lblStatus.TextAlign = ContentAlignment.MiddleLeft
            ' 
            ' Label1
            ' 
            Label1.AutoSize = True
            Label1.Location = New Point(29, 207)
            Label1.Name = "Label1"
            Label1.Size = New Size(71, 25)
            Label1.TabIndex = 0
            Label1.Text = "èÛë‘ ÅF"
            ' 
            ' btnDisconnect
            ' 
            btnDisconnect.Location = New Point(239, 126)
            btnDisconnect.Name = "btnDisconnect"
            btnDisconnect.Size = New Size(134, 50)
            btnDisconnect.TabIndex = 12
            btnDisconnect.Text = "êÿíf"
            btnDisconnect.UseVisualStyleBackColor = True
            ' 
            ' FormLobby
            ' 
            AutoScaleDimensions = New SizeF(10.0F, 25.0F)
            AutoScaleMode = AutoScaleMode.Font
            ClientSize = New Size(438, 480)
            Controls.Add(btnDisconnect)
            Controls.Add(lblStatus)
            Controls.Add(Label1)
            Controls.Add(txtLog)
            Controls.Add(btnConnect)
            Controls.Add(lblNick)
            Controls.Add(txtNickname)
            Controls.Add(lblIp)
            Controls.Add(txtIp)
            Margin = New Padding(4, 5, 4, 5)
            Name = "FormLobby"
            Text = "Baccarat - Lobby"
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents txtIp As TextBox
        Friend WithEvents txtNickname As TextBox
        Friend WithEvents btnConnect As Button
        Friend WithEvents txtLog As TextBox
        Friend WithEvents lblIp As Label
        Friend WithEvents lblNick As Label
        Friend WithEvents lblStatus As Label
        Friend WithEvents Label1 As Label
        Friend WithEvents btnDisconnect As Button

    End Class
End Namespace