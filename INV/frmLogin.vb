Imports System.Data.SQLite
Public Class frmLogin
    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Application.Exit()
    End Sub

    Private Sub frmLogin_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            If CInt(ExecuteScalar("SELECT COUNT(*) FROM tabCompany")) = 0 Then
                MessageBox.Show("Please create your company first.")
                frmNewCompany.ShowDialog()
            End If

            If CInt(ExecuteScalar("SELECT COUNT(*) FROM tabUsers")) = 0 Then
                MessageBox.Show("Please create the first user account.")
                frmNewUser.ShowDialog()
            End If


            txtUserID.Focus()

            fill_Company()

            cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(48, 48))

        Catch ex As Exception

            LogError(Me.Name, "frmLogin_Load", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Function Match_Login() As Boolean

        Dim sql As String = ""


        Try

            sql = "SELECT ID, UserID, CompanyID
                FROM tabUsers
                    WHERE UserID=@UserID
                       
                            AND CompanyID=@CompanyID"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@UserID", txtUserID.Text.Trim)
            ' prm.Add("@Password", HashPassword(txtPassword.Text.Trim))
            prm.Add("@CompanyID", CInt(cboBusinessName.SelectedValue))

            Using rdr As SQLiteDataReader = ExecuteReader(sql, prm)

                If rdr.Read() Then
                    CurrentUserID = CInt(rdr("ID"))
                    CurrentUserName = rdr("UserID").ToString()
                    CurrentCompanyID = CInt(rdr("CompanyID"))
                    CurrentCompanyName = cboBusinessName.Text
                    ' GetCurrentFY()
                    Return True
                End If
            End Using
            Return False
        Catch ex As Exception
            LogError(Me.Name, "Match_Login", Sql, ex)
            MessageBox.Show("Login failed. Please try again.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        End Try

    End Function

    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click

        If Match_Login() Then
            GetCurrentFY()
            LoadCompanySession()
            mdiMain.tmr1.Enabled = True
            mdiMain.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid User ID or Password.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPassword.Clear()
            txtUserID.Focus()
            txtUserID.SelectAll()
        End If
    End Sub

    Private Sub txtUserID_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtUserID.GotFocus
        txtUserID.SelectAll()
    End Sub

    Private Sub txtUserID_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtUserID.TextChanged

    End Sub

    Private Sub txtPassword_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPassword.GotFocus
        txtPassword.SelectAll()
    End Sub

    Private Sub fill_Company()

        If mySQLiteConn Is Nothing OrElse mySQLiteConn.State <> ConnectionState.Open Then
            OpenDB()
        End If

        Try

            Dim dt As DataTable = GetDataTable("SELECT ID, CompanyName FROM tabCompany ORDER BY CompanyName")

            cboBusinessName.DataSource = dt
            cboBusinessName.DisplayMember = "CompanyName"
            cboBusinessName.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_Company", "", ex)
            MessageBox.Show("Unable to load company list.", "Error")
            MessageBox.Show(ex.Message)
        End Try

    End Sub
End Class
