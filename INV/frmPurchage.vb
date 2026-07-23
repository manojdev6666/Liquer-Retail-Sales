Imports System.ComponentModel.Design
Imports System.Data.SQLite

Public Class frmPurchage
    Private frmSearch As New frmItemSelect
    Dim amount As Decimal
    Dim TCSRate As Decimal
    Dim purqty As Integer
    Public mFormMode As String
    Public LoadBillNo As String
    Public loadPartyID As Integer
    Public LoadDate As Date
    Public loadAmount As Decimal
    Public loadTCS As Decimal
    Public loadGTotal As Decimal
    Public LoadMasterID As Integer

    Private Sub frmPurchage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        fill_PName()
        cboParty.BackColor = Color.White
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
            .Columns.Add("ItemID", "ItemID")
            .Columns.Add("ItemName", "ItemName")
            .Columns.Add("Rate", "Rate")
            .Columns.Add("Qty", "Qty")
            .Columns.Add("Amount", "Amount")
            .Columns("ItemID").Visible = False
            .Columns("ItemName").Width = 300
            .Columns("Rate").Width = 60
            .Columns("Qty").Width = 60
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
        cmdDel.Image = New Bitmap(cmdDel.Image, New Size(64, 64))
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate.Focus()
        dtpDate.Value = Now.ToLongDateString
        AddMouseEvent(Me)
        TCSRate = GetTCS()
        lblTCSlable.Text = "TCS:@" & TCSRate & "%"
        purqty = 0
        If mFormMode = "EDIT" And loadPartyID <> 0 Then
            cmdSubmit.Text = "&UPDATE"
            txtBillNo.Enabled = False
            txtBillNo.Text = LoadBillNo
            lblTotalAmt.Text = loadAmount.ToString("0.00")
            lblTCS.Text = loadTCS.ToString("0.00")
            lblGTotal.Text = loadGTotal.ToString("0.00")
            Dim isSelected As Boolean = False
            cboParty.SelectedValue = loadPartyID
            dtpDate.Value = LoadDate
            LoadPurchage()

        End If
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If

        If e.KeyCode = Keys.F2 Then
            frmItemNew.Show(Me)
        End If
    End Sub

    Private Sub fill_PName()

        Try
            ' MsgBox("Party filling")
            Dim dt As New DataTable

            Dim sql As String =
            "SELECT ID,BName
             FROM tabParty
             WHERE CompanyID=@CompanyID
             ORDER BY BName"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)

                Using da As New SQLiteDataAdapter(cmd)

                    da.Fill(dt)

                End Using

            End Using

            cboParty.DataSource = dt
            cboParty.DisplayMember = "BName"
            cboParty.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_PName", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub txtRate_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRate.GotFocus
        txtRate.SelectAll()
    End Sub

    Private Sub txtQty_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQty.GotFocus
        txtQty.SelectAll()
    End Sub



    Private Sub txtRate_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRate.LostFocus
        txtRate.Text = Format(Val(txtRate.Text), "#0.00")
    End Sub

    Private Sub txtRate_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRate.TextChanged
        lblAmount.Text = Format(Val(txtRate.Text) * Val(txtQty.Text) * Val(lblQtyinCase.Text), "#0.00")
    End Sub


    Private Sub txtQty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtQty.TextChanged
        lblAmount.Text = Format(Val(txtRate.Text) * Val(txtQty.Text) * Val(lblQtyinCase.Text), "#0.00")
    End Sub

    Private Sub txtSearchItem_TextChanged(sender As Object, e As EventArgs) Handles txtSearchItem.TextChanged
        If Not frmSearch.Visible Then
            frmSearch.StartPosition = FormStartPosition.Manual
            frmSearch.Location = Panel2.PointToScreen(New Point(txtSearchItem.Left, txtSearchItem.Bottom))
            Me.KeyPreview = False
            frmSearch.Owner = Me
            frmSearch.Mode = frmItemSelect.SearchMode.Purchase
            frmSearch.SearchDate = dtpDate.Value.Date
            frmSearch.Show(Me)
        End If
        frmSearch.SearchDate = dtpDate.Value.Date
        frmSearch.LoadItems(txtSearchItem.Text)
        txtSearchItem.Focus()
        txtSearchItem.SelectionStart = txtSearchItem.TextLength
        txtSearchItem.SelectionLength = 0
        txtRate.Text = "0.00"
        txtQty.Text = "0"
    End Sub

    Private Sub txtSearchItem_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchItem.KeyDown
        If frmSearch.dgvItem.Rows.Count = 0 Then Exit Sub
        If frmSearch.dgvItem.CurrentCell Is Nothing Then
            frmSearch.dgvItem.CurrentCell = frmSearch.dgvItem.Rows(0).Cells(1)
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
        ElseIf e.KeyCode = Keys.Enter Then
            GetItemFromSearch()
        ElseIf e.KeyCode = Keys.Escape Then
            frmSearch.Hide()
        End If

        If e.KeyCode = Keys.F2 Then
            frmItemNew.Show(Me)
        End If
    End Sub

    Public Sub GetItemFromSearch()
        Dim r As DataGridViewRow = frmSearch.dgvItem.CurrentRow
        If r Is Nothing Then Exit Sub
        lblItemID.Text = r.Cells("ID").Value.ToString
        lblItemName.Text = r.Cells("ItemName").Value.ToString
        lblCategory.Text = r.Cells(4).Value.ToString() & ", " & r.Cells(5).Value.ToString()
        txtRate.Text = Format(r.Cells("Rate").Value, "0.00")
        lblQtyinCase.Text = Format(r.Cells("QtyInCase").Value, "0")
        frmSearch.Hide()
        txtQty.Focus()
        txtQty.SelectAll()
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs)

    End Sub

    Private Function GetTCS() As Decimal

        Dim sql As String = "SELECT TCSRate FROM tabTCS LIMIT 1"

        Try

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                Dim obj As Object = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    Return Convert.ToDecimal(obj)
                End If

            End Using

        Catch ex As Exception

            LogError(Me.Name, "GetTCS", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

        Return 0D

    End Function

    Private Sub SavePurchase()

        Dim tran As SQLiteTransaction = Nothing

        Try

            tran = mySQLiteConn.BeginTransaction()

            '================ PURCHASE MASTER =================
            MessageBox.Show("PartyID=" & cboParty.SelectedValue &
vbCrLf & "CompanyID=" & CurrentCompanyID &
vbCrLf & "UserID=" & CurrentUserID &
vbCrLf & "FY=" & CurrentFYID)
            Dim sqlMaster As String =
        "INSERT INTO tabPurchageMaster
        (Date,BillNo,PartyID,Amount,TCS,TotalAmount,FY,CompanyID,UserID)
        VALUES
        (@Date,@BillNo,@PartyID,@Amount,@TCS,@Total,@FY,@CompanyID,@UserID)"

            Using cmd As New SQLiteCommand(sqlMaster, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@BillNo", txtBillNo.Text.Trim)
                cmd.Parameters.AddWithValue("@PartyID", CInt(cboParty.SelectedValue))
                cmd.Parameters.AddWithValue("@Amount", CDec(lblTotalAmt.Text))
                cmd.Parameters.AddWithValue("@TCS", CDec(lblTCS.Text))
                cmd.Parameters.AddWithValue("@Total", CDec(lblGTotal.Text))
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                cmd.ExecuteNonQuery()

            End Using
            MessageBox.Show("master saved")
            '=========== LAST INSERT ID ===========

            Dim MasterID As Integer

            Using cmd As New SQLiteCommand("SELECT last_insert_rowid()", mySQLiteConn, tran)

                MasterID = Convert.ToInt32(cmd.ExecuteScalar())

            End Using

            '================ PURCHASE DETAIL =================

            Dim sqlDetail As String =
        "INSERT INTO tabPurchageSlave
        (MasterID,ItemID,Rate,Qty,CompanyID,UserID,FY)
        VALUES
        (@MasterID,@ItemID,@Rate,@Qty,@CompanyID,@UserID,@FY)"

            For Each r As DataGridViewRow In dg1.Rows

                If r.IsNewRow Then Continue For

                Using cmd As New SQLiteCommand(sqlDetail, mySQLiteConn, tran)

                    cmd.Parameters.AddWithValue("@MasterID", MasterID)
                    cmd.Parameters.AddWithValue("@ItemID", CInt(r.Cells("ItemID").Value))
                    cmd.Parameters.AddWithValue("@Rate", CDbl(r.Cells("Rate").Value))
                    cmd.Parameters.AddWithValue("@Qty", CDec(r.Cells("Qty").Value))
                    cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                    cmd.Parameters.AddWithValue("@UserID", CurrentUserID)
                    cmd.Parameters.AddWithValue("@FY", CurrentFYID)

                    cmd.ExecuteNonQuery()

                End Using

            Next
            MessageBox.Show("slave saved")
            '================ PARTY LEDGER =================

            Dim sqlLedger As String =
        "INSERT INTO tabPartyLaser
        (MasterID,PartyID,Date,BillNo,AmtDr,Ref,FY,CompanyID,UserID)
        VALUES
        (@MasterID,@PartyID,@Date,@BillNo,@AmtDr,@Ref,@FY,@CompanyID,@UserID)"

            Using cmd As New SQLiteCommand(sqlLedger, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@MasterID", MasterID)
                cmd.Parameters.AddWithValue("@PartyID", CInt(cboParty.SelectedValue))
                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@BillNo", txtBillNo.Text.Trim)
                cmd.Parameters.AddWithValue("@AmtDr", CDec(lblGTotal.Text))
                cmd.Parameters.AddWithValue("@Ref", "Purchage")
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                cmd.ExecuteNonQuery()

            End Using
            MessageBox.Show("party ledger saved")
            tran.Commit()

            MessageBox.Show("Purchase Saved Successfully")
            ClearForm()
        Catch ex As Exception

            If tran IsNot Nothing Then tran.Rollback()

            LogError(Me.Name, "SavePurchase", "", ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Function IsDuplicateBillNo() As Boolean

        Dim sql As String =
        "SELECT COUNT(*)
         FROM tabPurchageMaster
         WHERE BillNo=@BillNo
         AND CompanyID=@CompanyID
         AND FY=@FY"

        Try

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                cmd.Parameters.AddWithValue("@BillNo", txtBillNo.Text.Trim)
                cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                cmd.Parameters.AddWithValue("@FY", CurrentFYID)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using

        Catch ex As Exception

            LogError(Me.Name, "IsDuplicateBillNo", sql, ex)
            MessageBox.Show(ex.Message)

            Return False

        End Try

    End Function



    Private Sub cmdSubmit_Click_1(sender As Object, e As EventArgs) Handles cmdSubmit.Click
        If mFormMode = "EDIT" Then
            UpdatePurchase()
        Else
            If IsDuplicateBillNo() Or txtBillNo.Text.Trim = "" Then
                MessageBox.Show("Bill No already exists.")
                txtBillNo.Focus()
                Exit Sub
            End If
            SavePurchase()
        End If


    End Sub

    Private Sub txtQty_KeyDown(sender As Object, e As KeyEventArgs) Handles txtQty.KeyDown
        If e.KeyCode = Keys.Enter Then
            If lblItemName.Text = "-" Or lblItemName.Text.Trim = "" Then Exit Sub
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
                Dim r As Integer = dg1.Rows.Add(lblItemID.Text, lblItemName.Text, txtRate.Text, Val(txtQty.Text) * Val(lblQtyinCase.Text), lblAmount.Text)
                dg1.Rows(r).DefaultCellStyle.ForeColor = Color.Red
                amount = amount + Val(lblAmount.Text)
                lblTotalAmt.Text = Format(amount, "0.00")
                lblTCS.Text = Format(amount * TCSRate / 100, "0.00")
                lblGTotal.Text = Format(amount + (amount * TCSRate / 100), "0.00")
                lblItemID.Text = ""
                lblItemName.Text = ""
                lblCategory.Text = ""
                dg1.ClearSelection()
                txtSearchItem.Clear()
                txtSearchItem.Focus()
                frmSearch.Hide()
            End If
        End If
    End Sub

    Private Sub dg1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellContentClick

    End Sub

    Private Sub dg1_KeyDown(sender As Object, e As KeyEventArgs) Handles dg1.KeyDown
        If e.KeyCode = Keys.Delete Then
            If MessageBox.Show("Do you want to delete?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If dg1.CurrentRow IsNot Nothing Then
                    amount -= Val(dg1.CurrentRow.Cells("Amount").Value)
                    dg1.Rows.Remove(dg1.CurrentRow)
                    lblTotalAmt.Text = amount.ToString("0.00")
                    Dim tcsamount As Decimal = amount * TCSRate / 100
                    lblTCS.Text = tcsamount.ToString("0.00")
                    lblGTotal.Text = (amount + tcsamount).ToString("0.00")


                End If
            End If
        End If
    End Sub

    Private Sub LoadPurchage()
        Dim sql As String = ""
        Try

            dg1.Rows.Clear()

            amount = 0

            Sql =
        "SELECT
            PS.ItemID,
            I.ItemName,
            PS.Rate,
            PS.Qty
        FROM tabPurchageSlave PS
        INNER JOIN tabItem I
            ON I.ID = PS.ItemID
        WHERE PS.MasterID=@MasterID
        ORDER BY PS.ID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                cmd.Parameters.AddWithValue("@MasterID", LoadMasterID)

                Using dr As SQLiteDataReader = cmd.ExecuteReader()

                    While dr.Read()

                        Dim Amt As Decimal =
                        Convert.ToDecimal(dr("Qty")) *
                        Convert.ToDecimal(dr("Rate"))

                        dg1.Rows.Add(
                        dr("ItemID"),
                        dr("ItemName").ToString(),
                        Format(Convert.ToDecimal(dr("Rate")), "0.00"),
                        Format(Convert.ToDecimal(dr("Qty")), "0"),
                        Format(Amt, "0.00"))

                        amount += Amt

                    End While

                End Using

            End Using

            lblTotalAmt.Text = amount.ToString("0.00")

            Dim TCSAmt As Decimal = amount * TCSRate / 100D

            lblTCS.Text = TCSAmt.ToString("0.00")
            lblGTotal.Text = (amount + TCSAmt).ToString("0.00")

        Catch ex As Exception

            LogError(Me.Name, "LoadPurchage", Sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub UpdatePurchase()

        Dim tran As SQLiteTransaction = Nothing

        Try

            tran = mySQLiteConn.BeginTransaction()

            '================ PURCHASE MASTER =================

            Dim sqlMaster As String =
        "UPDATE tabPurchageMaster SET
            Date=@Date,
            PartyID=@PartyID,
            Amount=@Amount,
            TCS=@TCS,
            TotalAmount=@Total,
            UserID=@UserID
        WHERE ID=@ID"

            Using cmd As New SQLiteCommand(sqlMaster, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@ID", LoadMasterID)
                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@PartyID", CInt(cboParty.SelectedValue))
                cmd.Parameters.AddWithValue("@Amount", CDec(lblTotalAmt.Text))
                cmd.Parameters.AddWithValue("@TCS", CDec(lblTCS.Text))
                cmd.Parameters.AddWithValue("@Total", CDec(lblGTotal.Text))
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                cmd.ExecuteNonQuery()

            End Using

            '================ DELETE OLD ITEMS =================

            Dim sqlDelete As String =
            "DELETE FROM tabPurchageSlave
             WHERE MasterID=@MasterID"

            Using cmd As New SQLiteCommand(sqlDelete, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@MasterID", LoadMasterID)
                cmd.ExecuteNonQuery()

            End Using

            '================ INSERT NEW ITEMS =================

            Dim sqlDetail As String =
        "INSERT INTO tabPurchageSlave
        (MasterID,ItemID,Rate,Qty,CompanyID,UserID,FY)
        VALUES
        (@MasterID,@ItemID,@Rate,@Qty,@CompanyID,@UserID,@FY)"

            For Each r As DataGridViewRow In dg1.Rows

                If r.IsNewRow Then Continue For

                Using cmd As New SQLiteCommand(sqlDetail, mySQLiteConn, tran)

                    cmd.Parameters.AddWithValue("@MasterID", LoadMasterID)
                    cmd.Parameters.AddWithValue("@ItemID", CInt(r.Cells("ItemID").Value))
                    cmd.Parameters.AddWithValue("@Rate", CDbl(r.Cells("Rate").Value))
                    cmd.Parameters.AddWithValue("@Qty", CDec(r.Cells("Qty").Value))
                    cmd.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                    cmd.Parameters.AddWithValue("@UserID", CurrentUserID)
                    cmd.Parameters.AddWithValue("@FY", CurrentFYID)

                    cmd.ExecuteNonQuery()

                End Using

            Next

            '================ UPDATE PARTY LEDGER =================

            Dim sqlLedger As String =
        "UPDATE tabPartyLaser SET
            PartyID=@PartyID,
            Date=@Date,
            BillNo=@BillNo,
            AmtDr=@AmtDr,
            UserID=@UserID
        WHERE MasterID=@MasterID"

            Using cmd As New SQLiteCommand(sqlLedger, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@MasterID", LoadMasterID)
                cmd.Parameters.AddWithValue("@PartyID", CInt(cboParty.SelectedValue))
                cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date)
                cmd.Parameters.AddWithValue("@BillNo", txtBillNo.Text.Trim)
                cmd.Parameters.AddWithValue("@AmtDr", CDec(lblGTotal.Text))
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                cmd.ExecuteNonQuery()

            End Using

            tran.Commit()

            MessageBox.Show("Purchase Updated Successfully")

        Catch ex As Exception

            If tran IsNot Nothing Then tran.Rollback()

            LogError(Me.Name, "UpdatePurchase", "", ex)

            MessageBox.Show(ex.Message)

        End Try

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

    Private Sub ClearForm()

        txtBillNo.Clear()
        cboParty.SelectedIndex = -1

        dtpDate.Value = Now.Date

        txtSearchItem.Clear()
        txtRate.Text = "0.00"
        txtQty.Text = "0"

        lblItemID.Text = ""
        lblItemName.Text = "-"
        lblCategory.Text = "-"
        lblQtyinCase.Text = "1"

        dg1.Rows.Clear()

        amount = 0D
        lblTotalAmt.Text = "0.00"
        lblTCS.Text = "0.00"
        lblGTotal.Text = "0.00"

        txtBillNo.Focus()

    End Sub

    Private Sub DeletePurchase()

        Dim tran As SQLiteTransaction = Nothing

        Try

            If LoadMasterID = 0 Then
                MessageBox.Show("No Purchase selected.")
                Exit Sub
            End If

            If MessageBox.Show("Do you want to delete this Purchase?", "Confirm Delete",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            tran = mySQLiteConn.BeginTransaction()

            '================ DELETE PARTY LEDGER =================

            Dim sql As String =
                "DELETE FROM tabPartyLaser
             WHERE MasterID=@MasterID
             AND Ref='Purchage'"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@MasterID", LoadMasterID)
                cmd.ExecuteNonQuery()

            End Using

            '================ DELETE PURCHASE DETAILS =================

            sql =
                "DELETE FROM tabPurchageSlave
             WHERE MasterID=@MasterID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@MasterID", LoadMasterID)
                cmd.ExecuteNonQuery()

            End Using

            '================ DELETE PURCHASE MASTER =================

            sql =
                "DELETE FROM tabPurchageMaster
             WHERE ID=@ID"

            Using cmd As New SQLiteCommand(sql, mySQLiteConn, tran)

                cmd.Parameters.AddWithValue("@ID", LoadMasterID)
                cmd.ExecuteNonQuery()

            End Using

            tran.Commit()

            MessageBox.Show("Purchase deleted successfully.")

            ClearForm()

            Me.Close()

        Catch ex As Exception

            If tran IsNot Nothing Then
                tran.Rollback()
            End If

            LogError(Me.Name, "DeletePurchase", "", ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub cmdDel_Click(sender As Object, e As EventArgs) Handles cmdDel.Click

        If MessageBox.Show("Do you want to delete this Purchase?",
                       "Confirm Delete",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2) = DialogResult.Yes Then

            DeletePurchase()

        End If

    End Sub
End Class