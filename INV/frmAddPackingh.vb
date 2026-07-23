Imports System.Data.SQLite
Public Class frmAddPackingh
    Dim da As SQLiteDataAdapter
    Dim dt As New DataTable

    Private Sub frmAddCategory_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        LoadData()
    End Sub

    Private Sub frmAddCategory_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub LoadData()

        Try

            Dim sql As String = "SELECT * FROM tabPacking"

            da = New SQLiteDataAdapter(sql, mySQLiteConn)

            Dim cb As New SQLiteCommandBuilder(da)

            dt.Clear()
            da.Fill(dt)

            DG1.DataSource = dt

        Catch ex As Exception
            LogError(Me.Name, "LoadData", "SELECT * FROM tabPacking", ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click

        Try

            da.Update(dt)

            MessageBox.Show("Record Saved Successfully.")

            LoadData()

            Me.Close()

        Catch ex As Exception
            LogError(Me.Name, "cmdSave_Click", "", ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub DG1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DG1.DataBindingComplete
        If DG1.Columns.Contains("ID") Then
            DG1.Columns("ID").ReadOnly = True
        End If
    End Sub
End Class