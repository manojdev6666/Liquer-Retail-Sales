Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Data.SQLite
Imports System.Drawing.Text
Imports System.Web.Compilation
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Tab
Imports CrystalDecisions.CrystalReports.Engine
Public Class frmSale
    Dim cid As Integer
    Private frmSearch As New frmItemSelect
    Dim Beear, Wine As Decimal
    Private IsChanged As Boolean
    Private Sub frmSale_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            .Columns.Clear()
            .Columns.Add("ItemID", "ItemID")
            .Columns.Add("ItemName", "ItemName")
            .Columns.Add("Rate", "Rate")
            .Columns.Add("Qty", "Qty")
            .Columns.Add("Amount", "Amount")
            .Columns.Add("CID", "CID")
            .Columns("ItemID").Visible = False
            .Columns("CID").Visible = False
            .Columns("ItemName").Width = 330
            .Columns("Rate").Width = 90
            .Columns("Qty").Width = 90
            .Columns("Rate").DefaultCellStyle.Format = "N2"
            .Columns("Amount").DefaultCellStyle.Format = "N2"
            .Columns("Rate").DefaultCellStyle.Format = "N2"
            .Columns("Amount").DefaultCellStyle.Format = "N2"
            .Columns("Rate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("Qty").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersHeight = 32
            .RowTemplate.Height = 32
            dg1.ScrollBars = ScrollBars.Vertical
        End With
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(64, 64))
        cmdPrint.Image = New Bitmap(cmdPrint.Image, New Size(64, 64))
        cmdDel.Image = New Bitmap(cmdDel.Image, New Size(64, 64))
        AddMouseEvent(Me)
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now.AddDays(-1).Date
        dtpDate.MaxDate = Now.Date
        dtpDate.Value = Now.AddDays(-1).Date.ToLongDateString
        dtpDate.Focus()
        ReadSale()
    End Sub
    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
        If e.KeyCode = Keys.Escape Then
            frmSearch.Hide()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtAmtBnk_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBankSale.GotFocus
        txtBankSale.SelectAll()
    End Sub

    Private Sub txtAmtBnk_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBankSale.LostFocus
        txtBankSale.Text = Format(Val(txtBankSale.Text), "0.00")
    End Sub

    Private Sub dtpDate_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpDate.LostFocus

    End Sub

    Private Sub txtSearchItem_TextChanged_1(sender As Object, e As EventArgs) Handles txtSearchItem.TextChanged

        If Not frmSearch.Visible Then
            frmSearch.StartPosition = FormStartPosition.Manual
            Dim p As Point = Panel1.PointToScreen(New Point(txtSearchItem.Left, txtSearchItem.Bottom))
            frmSearch.Location = p
            Me.KeyPreview = False
            frmSearch.Owner = Me
            frmSearch.Mode = frmItemSelect.SearchMode.Sale
            frmSearch.SearchDate = dtpDate.Value.Date
            frmSearch.Width = txtSearchItem.Width
            frmSearch.Show(Me)
        End If
        frmSearch.SearchDate = dtpDate.Value.Date
        frmSearch.LoadItems(txtSearchItem.Text)
        txtSearchItem.Focus()
        txtSearchItem.SelectionStart = txtSearchItem.TextLength
        txtSearchItem.SelectionLength = 0
        txtRate.Text = "0.00"
        txtQty.Text = "0"
        lblAmount.Text = "0.00"
    End Sub

    Private Sub txtSearchItem_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchItem.KeyDown
        If frmSearch.dgvItem.Rows.Count = 0 Then Exit Sub
        If frmSearch.dgvItem.CurrentCell Is Nothing Then
            frmSearch.dgvItem.CurrentCell = frmSearch.dgvItem.Rows(0).Cells(1)
        End If
        If e.KeyCode = Keys.Escape Then
            frmSearch.Hide()
            e.SuppressKeyPress = True
        End If
        If e.KeyCode = Keys.Down Then
            Dim r As Integer = frmSearch.dgvItem.CurrentCell.RowIndex
            If r < frmSearch.dgvItem.Rows.Count - 1 Then
                frmSearch.dgvItem.CurrentCell = frmSearch.dgvItem.Rows(r + 1).Cells(1)
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up Then
            Dim r As Integer = frmSearch.dgvItem.CurrentCell.RowIndex
            If r > 0 Then
                frmSearch.dgvItem.CurrentCell = frmSearch.dgvItem.Rows(r - 1).Cells(1)
            End If
            e.SuppressKeyPress = True
        End If
        If e.KeyCode = Keys.Enter Then
            GetItemFromSearch()
        End If

    End Sub

    Public Sub GetItemFromSearch()
        Dim r As DataGridViewRow = frmSearch.dgvItem.CurrentRow
        If r Is Nothing Then Exit Sub
        lblItemID.Text = r.Cells(0).Value.ToString()
        lblItemName.Text = r.Cells(1).Value.ToString()
        txtRate.Text = Format(r.Cells(2).Value, "0.00")
        lblCID.Text = r.Cells(6).Value.ToString()
        lblCategory.Text = r.Cells(4).Value.ToString() & ", " & r.Cells(5).Value.ToString()
        lblStock.Text = Format(r.Cells(3).Value, "0")
        frmSearch.Hide()
        txtQty.Focus()
        txtQty.SelectAll()
    End Sub


    Private Sub txtQty_TextChanged(sender As Object, e As EventArgs) Handles txtQty.TextChanged
        lblAmount.Text = Format((Val(txtRate.Text) * Val(txtQty.Text)), "0.00")
    End Sub

    Private Sub txtRate_TextChanged(sender As Object, e As EventArgs) Handles txtRate.TextChanged
        lblAmount.Text = Format((Val(txtRate.Text) * Val(txtQty.Text)), "0.00")
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        If lblItemName.Text = "-" Or lblItemName.Text.Trim = "" Then Exit Sub
        If Val(txtQty.Text) > Val(lblStock.Text) Then
            MessageBox.Show("Can't Sale More than Avl. Stock", "Information")
            txtQty.Focus()
            txtQty.SelectAll()
            Exit Sub
        End If
        If Val(lblAmount.Text) < 1 Then
            MsgBox("Enter Quantity or Rate")
            Exit Sub
        End If
        Dim exists As Boolean = False
        For Each r As DataGridViewRow In dg1.Rows
            If Not r.IsNewRow Then
                If CInt(r.Cells("ItemID").Value) = lblItemID.Text Then
                    exists = True
                    Exit For
                End If
            End If
        Next
        If exists Then
            MessageBox.Show("Item Already added. Delete it from right grid then enter new", "Information")
            Exit Sub
        Else
            dg1.Rows.Add(lblItemID.Text, lblItemName.Text, txtRate.Text, txtQty.Text, lblAmount.Text, lblCID.Text)
            dg1.ClearSelection()
            If Val(lblCID.Text) = 1 Then
                Wine += Val(lblAmount.Text)
            Else
                Beear += Val(lblAmount.Text)
            End If
            txtSearchItem.Clear()
            txtSearchItem.Focus()
            lblItemID.Text = ""
            lblItemName.Text = ""
            lblCategory.Text = ""
            lblStock.Text = ""
            frmSearch.Hide()
        End If
        lblWine.Text = Format(Wine, "0.00")
        lblBeear.Text = Format(Beear, "0.00")
        lblTotal.Text = Format((Wine + Beear), "0.00")
        'lblCashSale.Text = Format((Wine + Beear), "0.00")
        UpdateSummary()
        IsChanged = True
    End Sub

    Private Sub txtBankSale_TextChanged(sender As Object, e As EventArgs) Handles txtBankSale.TextChanged
        lblCashSale.Text = Format((Wine + Beear) - Val(txtBankSale.Text), "0.00")
        IsChanged = True
    End Sub


    Private Sub txtQty_KeyDown(sender As Object, e As KeyEventArgs) Handles txtQty.KeyDown
        If e.KeyCode = Keys.Enter Then
            cmdAdd.Focus()
        End If
    End Sub

    Private Sub lblTotal_TextChanged(sender As Object, e As EventArgs) Handles lblTotal.TextChanged
        lblCashSale.Text = Format((Wine + Beear) - Val(txtBankSale.Text), "0.00")
    End Sub

    Private Sub AnyControl_MouseDown(sender As Object, e As MouseEventArgs)
        If frmSearch.Visible Then
            frmSearch.Hide()
        End If
    End Sub

    Private Sub AddMouseEvent(ctrl As Control)
        AddHandler ctrl.MouseDown, AddressOf AnyControl_MouseDown
        For Each c As Control In ctrl.Controls
            AddMouseEvent(c)
        Next
    End Sub

    Private Sub cmdSubmit_Click(sender As Object, e As EventArgs) Handles cmdSubmit.Click
        If cmdSubmit.Text = "&Submit" Then
            InsertSale()
            cmdSubmit.Text = "&Update"
        Else
            UpdateSale()
        End If
    End Sub
    Private Sub UpdateSale()

        Dim trans As SQLiteTransaction = Nothing

        Try

            trans = mySQLiteConn.BeginTransaction()

            '---------------- Update Header ----------------
            Dim sqlHeader As String =
        "UPDATE tabLaserSale
         SET SoldAmtCash=@Cash,
             SoldAmtBank=@Bank,
             UserID=@UserID
         WHERE Date=@Date
           AND FY=@FY
           AND CompanyID=@CompanyID"

            Using cmd As New SQLiteCommand(sqlHeader, mySQLiteConn, trans)

                cmd.Parameters.AddWithValue("@Cash", Val(lblCashSale.Text))
                cmd.Parameters.AddWithValue("@Bank", Val(txtBankSale.Text))
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)
                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                cmd.ExecuteNonQuery()

            End Using

            '---------------- Delete Old Detail ----------------
            Dim sqlDelete As String =
        "DELETE FROM tabSale
         WHERE Date=@Date
           AND FY=@FY
           AND CompanyID=@CompanyID"

            Using cmd As New SQLiteCommand(sqlDelete, mySQLiteConn, trans)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                cmd.ExecuteNonQuery()

            End Using

            '---------------- Insert New Detail ----------------
            Dim sqlInsert As String =
        "INSERT INTO tabSale
        (Date,ItemID,Rate,Qty,FY,CompanyID,UserID)
        VALUES
        (@Date,@ItemID,@Rate,@Qty,@FY,@CompanyID,@UserID)"

            For Each r As DataGridViewRow In dg1.Rows

                If r.IsNewRow Then Continue For

                Using cmd As New SQLiteCommand(sqlInsert, mySQLiteConn, trans)

                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                    cmd.Parameters.AddWithValue("@ItemID", r.Cells("ItemID").Value)
                    cmd.Parameters.AddWithValue("@Rate", r.Cells("Rate").Value)
                    cmd.Parameters.AddWithValue("@Qty", r.Cells("Qty").Value)
                    cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                    cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                    cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                    cmd.ExecuteNonQuery()

                End Using

            Next

            trans.Commit()

            MessageBox.Show("Sale Updated Successfully.")

            IsChanged = False

        Catch ex As Exception

            If trans IsNot Nothing Then
                trans.Rollback()
            End If

            LogError(Me.Name, "UpdateSale", "", ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub
    Private Sub InsertSale()

        If dg1.Rows.Count = 0 OrElse (dg1.Rows.Count = 1 AndAlso dg1.Rows(0).IsNewRow) Then
            MessageBox.Show("Please add at least one item.")
            Exit Sub
        End If

        Dim trans As SQLiteTransaction = Nothing

        Try

            trans = mySQLiteConn.BeginTransaction()

            '================ Header =================
            Dim sqlHeader As String =
        "INSERT INTO tabLaserSale
        (Date,SoldAmtCash,SoldAmtBank,FY,CompanyID,UserID)
        VALUES
        (@Date,@Cash,@Bank,@FY,@CompanyID,@UserID)"

            Using cmd As New SQLiteCommand(sqlHeader, mySQLiteConn, trans)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@Cash", Val(lblCashSale.Text))
                cmd.Parameters.AddWithValue("@Bank", Val(txtBankSale.Text))
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                cmd.ExecuteNonQuery()

            End Using

            '================ Detail =================
            Dim sqlDetail As String =
        "INSERT INTO tabSale
        (Date,ItemID,Rate,Qty,FY,CompanyID,UserID)
        VALUES
        (@Date,@ItemID,@Rate,@Qty,@FY,@CompanyID,@UserID)"

            For Each r As DataGridViewRow In dg1.Rows

                If r.IsNewRow Then Continue For

                Using cmd As New SQLiteCommand(sqlDetail, mySQLiteConn, trans)

                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                    cmd.Parameters.AddWithValue("@ItemID", CInt(r.Cells("ItemID").Value))
                    cmd.Parameters.AddWithValue("@Rate", CDbl(r.Cells("Rate").Value))
                    cmd.Parameters.AddWithValue("@Qty", CDec(r.Cells("Qty").Value))
                    cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                    cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                    cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                    cmd.ExecuteNonQuery()

                End Using

            Next

            trans.Commit()

            MessageBox.Show("Sale Saved Successfully.")

            IsChanged = False

        Catch ex As Exception

            If trans IsNot Nothing Then trans.Rollback()

            LogError(Me.Name, "InsertSale", "", ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub dg1_KeyDown(sender As Object, e As KeyEventArgs) Handles dg1.KeyDown
        If e.KeyCode = Keys.Delete Then
            If MessageBox.Show("Do you want to delete?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If dg1.CurrentRow IsNot Nothing Then
                    If dg1.SelectedRows(0).Cells(5).Value = 1 Then
                        Wine = Wine - dg1.SelectedRows(0).Cells(4).Value
                    Else
                        Beear = Beear - dg1.SelectedRows(0).Cells(4).Value
                    End If
                    dg1.Rows.Remove(dg1.CurrentRow)
                    lblWine.Text = Format(Wine, "0.00")
                    lblBeear.Text = Format(Beear, "0.00")
                    lblTotal.Text = Format((Wine + Beear), "0.00")
                End If
                IsChanged = True
            End If
        End If
        UpdateSummary()
    End Sub

    Private Sub dtpDate_ValueChanged(sender As Object, e As EventArgs) Handles dtpDate.ValueChanged
        ReadSale()
    End Sub

    Private Sub ReadSale()

        Try

            dg1.Rows.Clear()

            Wine = 0
            Beear = 0

            txtBankSale.Text = "0.00"

            '================ Header =================
            Dim sqlH As String = "SELECT SoldAmtBank
                              FROM tabLaserSale
                              WHERE Date=@Date
                              AND FY=@FY
                              AND CompanyID=@CompanyID"

            Using cmdH As New SQLiteCommand(sqlH, mySQLiteConn)

                cmdH.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmdH.Parameters.AddWithValue("@FY", CurrentFYID)
                cmdH.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                Using drH As SQLiteDataReader = cmdH.ExecuteReader()

                    If drH.Read() Then
                        txtBankSale.Text = Format(drH("SoldAmtBank"), "0.00")
                    End If

                End Using

            End Using

            '================ Detail =================
            Dim sql As String =
        "SELECT s.ItemID,
                i.ItemName,
                s.Rate,
                s.Qty,
                (s.Rate*s.Qty) AS Amount,
                i.Category AS CID
         FROM tabSale s
         INNER JOIN tabItem i ON i.ID=s.ItemID
         WHERE s.Date=@Date
         AND s.FY=@FY
         AND s.CompanyID=@CompanyID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                Using dr As SQLiteDataReader = cmd.ExecuteReader()

                    While dr.Read()

                        dg1.Rows.Add(
                        dr("ItemID"),
                        dr("ItemName"),
                        Format(dr("Rate"), "0.00"),
                        Format(dr("Qty"), "0"),
                        Format(dr("Amount"), "0.00"),
                        dr("CID"))

                        If CInt(dr("CID")) = 1 Then
                            Wine += CDec(dr("Amount"))
                        Else
                            Beear += CDec(dr("Amount"))
                        End If

                    End While

                End Using

            End Using

            lblWine.Text = Format(Wine, "0.00")
            lblBeear.Text = Format(Beear, "0.00")
            lblTotal.Text = Format(Wine + Beear, "0.00")
            lblCashSale.Text = Format((Wine + Beear) - Val(txtBankSale.Text), "0.00")

            If SaleExists() Then
                cmdSubmit.Text = "&Update"
            Else
                cmdSubmit.Text = "&Submit"
            End If

            UpdateSummary()

        Catch ex As Exception

            LogError(Me.Name, "ReadSale", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Function SaleExists() As Boolean
        Dim sql As String = ""
        Try

            sql =
            "SELECT COUNT(*) 
             FROM tabLaserSale
             WHERE Date=@Date
             AND FY=@FY
             AND CompanyID=@CompanyID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using

        Catch ex As Exception

            LogError(Me.Name, "SaleExists", Sql, ex)
            MessageBox.Show(ex.Message)
            Return False

        End Try

    End Function

    Private Sub dg1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellContentClick

    End Sub

    Private Sub UpdateSummary()

        Dim ItemCount As Integer = 0
        Dim TotalQty As Integer = 0
        Dim WineQty As Integer = 0
        Dim BeerQty As Integer = 0

        For Each r As DataGridViewRow In dg1.Rows

            If r.IsNewRow Then Continue For

            ItemCount += 1

            Dim Qty As Integer = CInt(r.Cells("Qty").Value)

            TotalQty += Qty

            If CInt(r.Cells("CID").Value) = 1 Then
                WineQty += Qty
            Else
                BeerQty += Qty
            End If

        Next

        lblSummary.Text = "Items : " & ItemCount &
                          "   Qty : " & TotalQty &
                          "   Wine : " & WineQty &
                          "   Beer : " & BeerQty

    End Sub

    Private Sub cmdPrint_Click(sender As Object, e As EventArgs) Handles cmdPrint.Click

        Dim sql As String = ""

        Try

            sql = "
SELECT
    s.Date,
    i.ItemName,
    s.Rate,
    s.Qty,
    s.Rate * s.Qty AS Amount,
    CAST(i.Category AS INTEGER) AS Category ,
    l.SoldAmtCash,
    l.SoldAmtBank
FROM tabSale s
INNER JOIN tabItem i
    ON i.ID = s.ItemID
INNER JOIN tabLaserSale l
    ON l.Date = s.Date
   AND l.FY = s.FY
   AND l.CompanyID = s.CompanyID
WHERE s.Date = @Date
  AND s.FY = @FY
  AND s.CompanyID = @CompanyID
ORDER BY i.Category,
         i.ItemName"

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@Date", dtpDate.Value.Date)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@CompanyID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            'For Each c As DataColumn In dt.Columns
            'MessageBox.Show(c.ColumnName & " = " & c.DataType.ToString)
            'Next
            ds.dtDailySale.Merge(dt)

            Dim cryRpt As New rptDailySale

            cryRpt.SetDataSource(ds)


            cryRpt.SetParameterValue("Date", dtpDate.Value.Date)
            cryRpt.SetParameterValue("CompanyName", CurrentCompanyName)
            cryRpt.SetParameterValue("CompAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYName)

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None

            Me.Close()

            frmViewRpt.MdiParent = mdiMain
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception

            LogError("frmDailySale",
                 "cmdPrint_Click",
                 sql,
                 ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub frmSale_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If IsChanged Then
            Dim Result As DialogResult = MessageBox.Show("Sale is not Saved" & vbCrLf & "Do you Want to close Sale Entry Form?", "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If Result = DialogResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub cmdDel_Click(sender As Object, e As EventArgs) Handles cmdDel.Click

        If Not SaleExists() Then
            MessageBox.Show("Nothing to Delete", "Information")
            Exit Sub
        End If

        If MessageBox.Show("The Sale will be deleted Permanentaly." & vbCrLf &
                       vbCrLf &
                       "Do You Want to Delete?",
                       "Confirm Delete",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Warning,
                       MessageBoxDefaultButton.Button2) = DialogResult.Yes Then

            DeleteSale()

        End If

    End Sub

    Private Sub DeleteSale()

        Dim trans As SQLiteTransaction = Nothing

        Try

            If Not SaleExists() Then
                MessageBox.Show("No Sale found for selected date.")
                Exit Sub
            End If

            If MessageBox.Show("Delete Sale of " & dtpDate.Value.ToShortDateString() & " ?",
                               "Confirm Delete",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            trans = mySQLiteConn.BeginTransaction()

            'Delete Detail
            Dim sql As String =
                "DELETE FROM tabSale
             WHERE Date=@Date
             AND FY=@FY
             AND CompanyID=@CompanyID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn, trans)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                cmd.ExecuteNonQuery()

            End Using

            'Delete Header
            sql =
                "DELETE FROM tabLaserSale
             WHERE Date=@Date
             AND FY=@FY
             AND CompanyID=@CompanyID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn, trans)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                cmd.ExecuteNonQuery()

            End Using

            trans.Commit()

            MessageBox.Show("Sale Deleted Successfully.")

            ReadSale()

            cmdSubmit.Text = "&Submit"
            IsChanged = False

        Catch ex As Exception

            If trans IsNot Nothing Then trans.Rollback()

            LogError(Me.Name, "DeleteSale", "", ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub
End Class