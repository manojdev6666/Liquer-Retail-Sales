Imports System.Data.SQLite

Public Class frmNewParty
    Private editID As Integer = 0
    Private Sub frmNewParty_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtBname.Focus()
        Fill_Grid()
        With dg1
            .RowHeadersVisible = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(222, 255, 222)
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Orange
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 12, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Regular)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .BackgroundColor = Me.Panel1.BackColor
            .MultiSelect = False
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = .ColumnHeadersDefaultCellStyle.BackColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.ForeColor = .ColumnHeadersDefaultCellStyle.ForeColor
            .ColumnHeadersHeight = 32
            .RowTemplate.Height = 32
        End With
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(64, 64))
    End Sub
    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub Insert_Data()

        Dim sql As String = ""

        Try

            sql = "INSERT INTO tabParty
              (BName,CPName,GSTNo,ContNo,Address,CompanyID,UserID)
               VALUES
              (@BName,@CPName,@GSTNo,@ContNo,@Address,@CompanyID,@UserID)"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@BName", txtBname.Text.Trim)
            prm.Add("@CPName", txtPName.Text.Trim)
            prm.Add("@GSTNo", txtGST.Text.Trim)
            prm.Add("@ContNo", txtCont.Text.Trim)
            prm.Add("@Address", txtAddress.Text.Trim)
            prm.Add("@CompanyID", CurrentCompanyID)
            prm.Add("@UserID", CurrentUserID)

            ExecuteNonQuery(sql, prm)

        Catch ex As Exception

            LogError(Me.Name, "Insert_Data", sql, ex)
            Throw

        End Try

    End Sub


    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        If Trim(Len(txtBname.Text)) > 0 Then
            If cmdSubmit.Text = "&SUBMIT" Then
                Insert_Data()
                Fill_Grid()
                ClearField()
                txtBname.Focus()
            Else
                Update_Data()
                Fill_Grid()
                ClearField()
                txtBname.Focus()
            End If

        Else
            MsgBox("Enter Business Name", MsgBoxStyle.Critical, "Blank Record Can't Insert")
            txtBname.Focus()
        End If
    End Sub
    Private Sub ClearField()
        txtAddress.Clear()
        txtBname.Clear()
        txtCont.Clear()
        txtGST.Clear()
        txtPName.Clear()
    End Sub

    Private Sub Fill_Grid()

        Dim sql As String = ""

        Try

            sql = "SELECT ID,BName,CPName,GSTNo,ContNo,Address
               FROM tabParty
               WHERE CompanyID=@CompanyID
               ORDER BY BName"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@CompanyID", CurrentCompanyID)

            dg1.DataSource = GetDataTable(sql, prm)

            dg1.Columns("ID").Visible = False
            dg1.Columns("BName").Width = 200

        Catch ex As Exception

            LogError(Me.Name, "Fill_Grid", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub
    Private Sub Update_Data()

        Dim sql As String = ""

        Try

            sql = "UPDATE tabParty SET
               BName=@BName,
               CPName=@CPName,
               GSTNo=@GSTNo,
               ContNo=@ContNo,
               Address=@Address
               WHERE ID=@ID"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@BName", txtBname.Text.Trim)
            prm.Add("@CPName", txtPName.Text.Trim)
            prm.Add("@GSTNo", txtGST.Text.Trim)
            prm.Add("@ContNo", txtCont.Text.Trim)
            prm.Add("@Address", txtAddress.Text.Trim)
            prm.Add("@ID", editID)

            ExecuteNonQuery(sql, prm)

            cmdSubmit.Text = "&SUBMIT"

        Catch ex As Exception

            LogError(Me.Name, "Update_Data", sql, ex)
            Throw

        End Try

    End Sub

    Private Sub dg1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellContentClick

    End Sub

    Private Sub dg1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellClick
        If e.RowIndex < 0 Then Exit Sub
        editID = Val(dg1.Rows(e.RowIndex).Cells("ID").Value)
        txtBname.Text = dg1.Rows(e.RowIndex).Cells("BName").Value.ToString
        txtPName.Text = dg1.Rows(e.RowIndex).Cells("CPName").Value.ToString
        txtGST.Text = dg1.Rows(e.RowIndex).Cells("GSTNo").Value.ToString
        txtCont.Text = dg1.Rows(e.RowIndex).Cells("ContNo").Value.ToString
        txtAddress.Text = dg1.Rows(e.RowIndex).Cells("Address").Value.ToString
        cmdSubmit.Text = "Modify"
    End Sub
End Class