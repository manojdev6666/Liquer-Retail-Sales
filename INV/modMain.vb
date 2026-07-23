Imports System.Data.SqlClient
Imports System.Data
Module modMain
    Public mySqlConn As SqlConnection
    Public UserID As Integer
    Public sCompanyName As String
    Public iCompanyID As Integer
    Public scomcont As String
    Public scomAdd As String
    Public scomReg As String
    Public sPath As String
    Public sDBID As String
    Public sDBPassword As String
    Public sDataSource As String
    Public sInitialCatalog As String
    Public sFY As String

    Public Sub Connect_DB()
        mySqlConn = New SqlConnection("Data Source=CDU; Initial Catalog=INV; User ID=sa; Password=123456;")
        Try
            mySqlConn.Open()
            GetSystemData()
        Catch errex As Exception
            MessageBox.Show("Database Connection Problem")
        End Try
    End Sub

    Public Function Get_CurrentFY() As String
        Return ""
    End Function



    Public Sub Get_CompanyDetail()
        Dim sql As String = "Select * from tabCompany Where ID='" & iCompanyID & "'"
        Dim cmd As New SqlCommand(sql, mySqlConn)
        Dim reader As SqlDataReader = cmd.ExecuteReader
        If reader.HasRows Then
            reader.Read()
            scomAdd = reader.Item("CompanyAddress")
            scomcont = reader.Item("ContactNumber")
            scomReg = reader.Item("RegistrationNo1")
            reader.Close()
        Else
            reader.Close()
            MsgBox("No Company Found")
        End If
    End Sub


    Private Function SystemDataExists() As Boolean
        Dim x As Boolean = False
        Dim sql As String = "SELECT * FROM tabSystem"
        Dim cmd As SqlCommand
        Dim rdr As SqlDataReader
        cmd = New SqlCommand(sql, mySqlConn)
        rdr = cmd.ExecuteReader
        If rdr.HasRows Then
            x = True
        Else
            x = False
        End If
        rdr.Close()
        Return x
    End Function

    Private Sub GetSystemData()

        Dim sql As String = "SELECT * FROM tabSystem"
        Dim cmd As SqlCommand
        Dim rdr As SqlDataReader
        cmd = New SqlCommand(sql, mySqlConn)
        rdr = cmd.ExecuteReader
        If rdr.HasRows Then
            rdr.Read()
            sPath = rdr("Path")
            sDBID = rdr("DBID")
            sDBPassword = rdr("DBPassword")
            sDataSource = rdr("DataSource")
            sInitialCatalog = rdr("InitialCatalog")

        Else
            sPath = ""
            sDBID = ""
            sDBPassword = ""
            sDataSource = ""
            sInitialCatalog = ""
        End If
        rdr.Close()

    End Sub

End Module
