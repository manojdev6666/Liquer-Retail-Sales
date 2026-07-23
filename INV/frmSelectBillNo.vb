Imports System.Data.SQLite


Public Class frmSelectBillNo


    Private Sub frmSelectBillNo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With dg1
            .RowHeadersVisible = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192)
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Orange
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 12, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Regular)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .BackgroundColor = Me.Panel1.BackColor
            .RowsDefaultCellStyle.ForeColor = Color.Green
            .MultiSelect = False
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = .ColumnHeadersDefaultCellStyle.BackColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.ForeColor = .ColumnHeadersDefaultCellStyle.ForeColor
            .Columns.Clear()
            .Columns.Add("ID", "ID")
            .Columns.Add("Date", "Date")
            .Columns.Add("BillNo", "BillNo")
            .Columns.Add("PartyID", "PartyID")
            .Columns.Add("BName", "BName")
            .Columns.Add("Amount", "Amount")
            .Columns.Add("TCS", "TCS")
            .Columns.Add("TotalAmount", "G.Total")
            .Columns("ID").Visible = False
            .Columns("PartyID").Visible = False
            .Columns("Date").Width = 78
            .Columns("BillNo").Width = 60
            .Columns("BName").Width = 300
            .Columns("Amount").Width = 95
            .Columns("TCS").Width = 95
            .Columns("TotalAmount").Width = 100
            .Columns("Amount").DefaultCellStyle.Format = "N2"
            .Columns("TCS").DefaultCellStyle.Format = "N2"
            .Columns("TotalAmount").DefaultCellStyle.Format = "N2"
            .Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TCS").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TotalAmount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("Date").DefaultCellStyle.Format = "dd-MM-yyyy"
            .ColumnHeadersHeight = 32
            .RowTemplate.Height = 32
            dg1.ScrollBars = ScrollBars.Vertical
        End With
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(64, 64))
        fillBills()

    End Sub

    Private Sub txtSearchItem_TextChanged(sender As Object, e As EventArgs) Handles txtSearchItem.TextChanged
        fillBills(txtSearchItem.Text.Trim)
    End Sub

    Private Sub fillBills(Optional SearchText As String = "")
        Dim sql As String = ""
        Try

            dg1.Rows.Clear()

            sql =
        "SELECT PM.ID,
                PM.Date,
                PM.BillNo,
                P.ID AS PartyID,
                P.BName,
                PM.Amount,
                PM.TCS,
                PM.TotalAmount
         FROM tabPurchageMaster PM
         LEFT JOIN tabParty P
                ON P.ID = PM.PartyID
         WHERE PM.FY = @FY
           AND PM.CompanyID = @CompanyID
           AND PM.BillNo LIKE @BillNo
         ORDER BY CAST(PM.BillNo AS INTEGER) DESC"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                cmd.Parameters.AddWithValue("@BillNo", "%" & SearchText & "%")

                Using dr As SQLiteDataReader = cmd.ExecuteReader()

                    While dr.Read()

                        dg1.Rows.Add(
                        Convert.ToInt32(dr("ID")),
                        Convert.ToDateTime(dr("Date")),
                        dr("BillNo").ToString(),
                        Convert.ToInt32(dr("PartyID")),
                        dr("BName").ToString(),
                        Convert.ToDecimal(dr("Amount")),
                        Convert.ToDecimal(dr("TCS")),
                        Convert.ToDecimal(dr("TotalAmount")))

                    End While

                End Using

            End Using

        Catch ex As Exception

            LogError(Me.Name, "fillBills", Sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub dg1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellContentClick

    End Sub

    Private Sub dg1_KeyDown(sender As Object, e As KeyEventArgs) Handles dg1.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            cmdSubmit.Focus()
        End If
        If e.KeyCode = Keys.Up And dg1.SelectedRows.Count > 0 AndAlso dg1.SelectedRows(0).Index = 0 Then
            txtSearchItem.Focus()
        End If
    End Sub

    Private Sub txtSearchItem_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchItem.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Down Then
            dg1.Focus()
        End If
    End Sub

    Private Sub cmdSubmit_Click(sender As Object, e As EventArgs) Handles cmdSubmit.Click
        If dg1.SelectedRows.Count = 0 Then
            MessageBox.Show("Select Record")
            Exit Sub
        End If
        Dim frm As New frmPurchage
        frm.LoadBillNo = dg1.SelectedRows(0).Cells("BillNo").Value.ToString
        frm.loadPartyID = CInt(dg1.SelectedRows(0).Cells("PartyID").Value)
        frm.LoadDate = CDate(dg1.SelectedRows(0).Cells("Date").Value)
        frm.loadAmount = CDec(dg1.SelectedRows(0).Cells("Amount").Value)
        frm.loadTCS = CDec(dg1.SelectedRows(0).Cells("TCS").Value)
        frm.loadGTotal = CDec(dg1.SelectedRows(0).Cells("TotalAmount").Value)
        frm.LoadMasterID = CInt(dg1.SelectedRows(0).Cells("ID").Value)
        frm.mFormMode = "EDIT"

        frm.ShowDialog(Me)

    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub
End Class