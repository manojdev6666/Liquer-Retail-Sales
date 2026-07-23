Imports System.Data.SQLite

Public Class frmNewUser


    Private Sub frmNewUser_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        fill_Company()
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(24, 24))
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub fill_Company()

        Try

            Dim dt As DataTable = GetDataTable("SELECT ID, CompanyName FROM tabCompany ORDER BY CompanyName")

            cboBusinessName.DataSource = dt
            cboBusinessName.DisplayMember = "CompanyName"
            cboBusinessName.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_Company", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        If Len(Trim(txtUserID.Text)) < 1 Then
            MsgBox("Enter UserID!", MsgBoxStyle.Critical, "ERROR!!")
            txtUserID.Focus()
            Exit Sub
        End If
        If txtPassword.Text.Trim <> txtPassword1.Text.Trim Then
            MsgBox("Password Not Matched!")
            txtPassword.Focus()
            Exit Sub
        End If
        If Len(Trim(txtPassword.Text)) < 1 Then
            MsgBox("Enter Password!", MsgBoxStyle.Critical, "ERROR!!")
            txtPassword.Focus()
            Exit Sub
        End If
        If isExist() Then
            MsgBox("UserID Already Exist!", MsgBoxStyle.Critical, "ERROR!!")
            txtUserID.Focus()
            Exit Sub
        End If
        If IsValidPassword(txtPassword.Text.Trim) Then
            INSERT_Data()
            MsgBox("User Account Created Successfully!", MsgBoxStyle.Information, "SUCCESS")
            txtPassword.Clear()
            txtUserID.Clear()
            txtPassword1.Clear()
        Else
            MsgBox("Invalid Password")
            txtPassword.Focus()
            Exit Sub
        End If

    End Sub

    Private Function isExist() As Boolean
        Dim sql As String = ""
        Try

            sql = "SELECT COUNT(*) FROM tabUsers
                             WHERE UserID=@UserID
                             AND CompanyID=@CompanyID"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@UserID", txtUserID.Text.Trim.ToUpper)
            prm.Add("@CompanyID", CInt(cboBusinessName.SelectedValue))

            Return CInt(ExecuteScalar(sql, prm)) > 0

        Catch ex As Exception

            LogError(Me.Name, "isExist", Sql, ex)
            Throw

        End Try

    End Function


    Private Sub INSERT_Data()
        Dim sql As String = ""
        Try

            sql = "INSERT INTO tabUsers
                            (UserID,Password,CompanyID)
                            VALUES
                            (@UserID,@Password,@CompanyID)"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@UserID", txtUserID.Text.Trim.ToUpper)
            prm.Add("@Password", HashPassword(txtPassword.Text.Trim))
            prm.Add("@CompanyID", CInt(cboBusinessName.SelectedValue))

            ExecuteNonQuery(sql, prm)
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception

            LogError(Me.Name, "INSERT_Data", Sql, ex)
            Throw

        End Try




    End Sub
    Private Function IsValidPassword(ByVal pwd As String) As Boolean

        If pwd.Length < 8 Then Return False

        Dim hasUpper As Boolean = False
        Dim hasDigit As Boolean = False
        Dim hasSymbol As Boolean = False

        For Each ch As Char In pwd
            If Char.IsUpper(ch) Then
                hasUpper = True
            ElseIf Char.IsDigit(ch) Then
                hasDigit = True
            ElseIf Not Char.IsLetterOrDigit(ch) Then
                hasSymbol = True
            End If
        Next

        Return hasUpper And hasDigit And hasSymbol

    End Function
End Class