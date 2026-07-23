Imports System.Data.SQLite

Public Class frmChangePassword

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then

            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub


    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click

        Try

            If txtNewPassword.Text <> txtNewPassword2.Text Then
                MsgBox("New Password and Retype New Password must be same", MsgBoxStyle.Critical)
                txtNewPassword.Focus()
                Exit Sub
            End If

            If HashPassword(txtOldPassword.Text.Trim) <> getOldPassword() Then
                MsgBox("Old Password Not Match", MsgBoxStyle.Critical)
                txtOldPassword.Focus()
                Exit Sub
            End If

            UPDATE_DATA()

            MsgBox("Password Changed Successfully")

            txtOldPassword.Clear()
            txtNewPassword.Clear()
            txtNewPassword2.Clear()

        Catch ex As Exception

            LogError(Me.Name, "cmdSubmit_Click", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Function getOldPassword() As String

        Dim sql As String = ""

        Try

            sql = "SELECT Password FROM tabUsers
               WHERE ID=@ID
               AND CompanyID=@CompanyID"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@ID", CurrentUserID)
            prm.Add("@CompanyID", CurrentCompanyID)

            Dim obj As Object = ExecuteScalar(sql, prm)

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return ""
            Else
                Return obj.ToString()
            End If

        Catch ex As Exception

            LogError(Me.Name, "getOldPassword", sql, ex)
            Throw

        End Try

    End Function

    Private Sub UPDATE_DATA()

        Dim sql As String = ""

        Try

            sql = "UPDATE tabUsers
               SET Password=@Password
               WHERE ID=@ID
               AND CompanyID=@CompanyID"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@Password", HashPassword(txtNewPassword.Text.Trim))
            prm.Add("@ID", CurrentUserID)
            prm.Add("@CompanyID", CurrentCompanyID)

            ExecuteNonQuery(sql, prm)

        Catch ex As Exception

            LogError(Me.Name, "UPDATE_DATA", sql, ex)
            Throw

        End Try

    End Sub


End Class