Imports System.Windows.Forms

Module modStartup

    <STAThread()>
    Public Sub Main()

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim sp As New splash
        sp.Show()
        Application.DoEvents()

        OpenDB()

        sp.Close()

        Application.Run(New frmLogin)

    End Sub

End Module