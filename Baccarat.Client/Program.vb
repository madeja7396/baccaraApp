Imports System
Imports System.Windows.Forms
Imports Baccarat.Client.Forms

Module Program
    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FormLobby())
    End Sub
End Module
