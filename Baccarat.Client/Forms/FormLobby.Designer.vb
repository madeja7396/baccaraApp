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
            Me.txtIp = New TextBox()
            Me.txtNickname = New TextBox()
            Me.btnConnect = New Button()
            Me.lblIp = New Label()
            Me.lblNick = New Label()
            Me.txtLog = New TextBox()
            Me.SuspendLayout()
            '
            'lblIp (IPアドレスラベル)
            '
            Me.lblIp.Location = New Drawing.Point(20, 20)
            Me.lblIp.Name = "lblIp"
            Me.lblIp.Size = New Drawing.Size(100, 23)
            Me.lblIp.Text = "Server IP:"
            '
            'txtIp (IP入力欄)
            '
            Me.txtIp.Location = New Drawing.Point(120, 20)
            Me.txtIp.Name = "txtIp"
            Me.txtIp.Size = New Drawing.Size(150, 23)
            Me.txtIp.Text = "127.0.0.1"
            '
            'lblNick (ニックネームラベル)
            '
            Me.lblNick.Location = New Drawing.Point(20, 50)
            Me.lblNick.Name = "lblNick"
            Me.lblNick.Size = New Drawing.Size(100, 23)
            Me.lblNick.Text = "Nickname:"
            '
            'txtNickname (ニックネーム入力欄)
            '
            Me.txtNickname.Location = New Drawing.Point(120, 50)
            Me.txtNickname.Name = "txtNickname"
            Me.txtNickname.Size = New Drawing.Size(150, 23)
            '
            'btnConnect (接続ボタン)
            '
            Me.btnConnect.Location = New Drawing.Point(20, 90)
            Me.btnConnect.Name = "btnConnect"
            Me.btnConnect.Size = New Drawing.Size(250, 30)
            Me.btnConnect.TabIndex = 2
            Me.btnConnect.Text = "Connect to Server"
            Me.btnConnect.UseVisualStyleBackColor = True
            '
            'txtLog (ロビーログ)
            '
            Me.txtLog.Location = New Drawing.Point(20, 130)
            Me.txtLog.Multiline = True
            Me.txtLog.Name = "txtLog"
            Me.txtLog.ReadOnly = True
            Me.txtLog.ScrollBars = ScrollBars.Vertical
            Me.txtLog.Size = New Drawing.Size(250, 100)
            Me.txtLog.TabIndex = 3
            '
            'FormLobby
            '
            Me.AutoScaleDimensions = New Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.ClientSize = New Drawing.Size(300, 250)
            Me.Controls.Add(Me.txtLog)
            Me.Controls.Add(Me.btnConnect)
            Me.Controls.Add(Me.lblNick)
            Me.Controls.Add(Me.txtNickname)
            Me.Controls.Add(Me.lblIp)
            Me.Controls.Add(Me.txtIp)
            Me.Name = "FormLobby"
            Me.Text = "Baccarat - Lobby"
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents txtIp As TextBox
        Friend WithEvents txtNickname As TextBox
        Friend WithEvents btnConnect As Button
        Friend WithEvents txtLog As TextBox
        Friend WithEvents lblIp As Label
        Friend WithEvents lblNick As Label
    End Class
End Namespace