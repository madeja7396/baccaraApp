Imports System
Imports System.Windows.Forms
Imports Baccarat.Server.Forms

Module Program
    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FormServer())
    End Sub
End Module
