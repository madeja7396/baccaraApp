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
            Button1 = New Button()
            Button2 = New Button()
            Button3 = New Button()
            Label1 = New Label()
            Label2 = New Label()
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
            btnConnect.Location = New Point(442, 30)
            btnConnect.Margin = New Padding(4, 5, 4, 5)
            btnConnect.Name = "btnConnect"
            btnConnect.Size = New Size(167, 50)
            btnConnect.TabIndex = 2
            btnConnect.Text = "Connect to Server"
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
            txtLog.Location = New Point(442, 123)
            txtLog.Margin = New Padding(4, 5, 4, 5)
            txtLog.Multiline = True
            txtLog.Name = "txtLog"
            txtLog.ReadOnly = True
            txtLog.ScrollBars = ScrollBars.Vertical
            txtLog.Size = New Size(210, 269)
            txtLog.TabIndex = 3
            ' 
            ' Button1
            ' 
            Button1.Location = New Point(12, 345)
            Button1.Name = "Button1"
            Button1.Size = New Size(112, 34)
            Button1.TabIndex = 8
            Button1.Text = "Button1"
            Button1.UseVisualStyleBackColor = True
            ' 
            ' Button2
            ' 
            Button2.Location = New Point(156, 345)
            Button2.Name = "Button2"
            Button2.Size = New Size(112, 34)
            Button2.TabIndex = 9
            Button2.Text = "Button2"
            Button2.UseVisualStyleBackColor = True
            ' 
            ' Button3
            ' 
            Button3.Location = New Point(304, 345)
            Button3.Name = "Button3"
            Button3.Size = New Size(112, 34)
            Button3.TabIndex = 10
            Button3.Text = "Button3"
            Button3.UseVisualStyleBackColor = True
            ' 
            ' Label1
            ' 
            Label1.AutoSize = True
            Label1.Location = New Point(42, 197)
            Label1.Name = "Label1"
            Label1.Size = New Size(63, 25)
            Label1.TabIndex = 0
            Label1.Text = "Label1"
            ' 
            ' Label2
            ' 
            Label2.AutoSize = True
            Label2.Location = New Point(221, 161)
            Label2.Name = "Label2"
            Label2.Size = New Size(63, 25)
            Label2.TabIndex = 11
            Label2.Text = "Label2"
            ' 
            ' FormLobby
            ' 
            AutoScaleDimensions = New SizeF(10F, 25F)
            AutoScaleMode = AutoScaleMode.Font
            ClientSize = New Size(665, 434)
            Controls.Add(Label2)
            Controls.Add(Label1)
            Controls.Add(Button3)
            Controls.Add(Button2)
            Controls.Add(Button1)
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
        Friend WithEvents Button1 As Button
        Friend WithEvents Button2 As Button
        Friend WithEvents Button3 As Button
        Friend WithEvents Label1 As Label
        Friend WithEvents Label2 As Label
    End Class
End Namespace