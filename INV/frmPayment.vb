Imports System.Data.SQLite


Public Class frmPayment
    Dim LadgerAmt As Decimal
    Private Sub frmPayment_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        dtpDate.MinDate = Get_FirstDay()

        dtpDate.MaxDate = Now
        dtpDate.Focus()
        dtpDate.Value = Now.Date
        fill_PName()

        getcashbankbal()
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(32, 32))
        With dg1
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Orange
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            .EnableHeadersVisualStyles = False
            .AlternatingRowsDefaultCellStyle.BackColor = Panel3.BackColor
            .BackgroundColor = Panel3.BackColor
            .CellBorderStyle = DataGridViewCellBorderStyle.None
        End With
        txtTotalOutS.Text = txtLaserAmt.Text
        txtBank.Text="0.00"
        txtCash.Text = "0.00"
        getLaserAmt()
    End Sub

    Private Sub getcashbankbal()



        Dim sql As String =
"
SELECT
    IFNULL(SUM(Cash),0) AS Cash,
    IFNULL(SUM(Bank),0) AS Bank
FROM
(
    SELECT
        IFNULL(SUM(CashAmtRcv),0) AS Cash,
        IFNULL(SUM(BankAmtRcv),0) AS Bank
    FROM tabLaserSale
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(-AmtCRCash),0),
        IFNULL(SUM(-AmtCRBank),0)
    FROM tabPartyLaser
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(-CashToBank),0),
        IFNULL(SUM(CashToBank),0)
    FROM tabCashFlow
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(BankToCash),0),
        IFNULL(SUM(-BankToCash),0)
    FROM tabCashFlow
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(CapitalAmountCash),0),
        IFNULL(SUM(CapitalAmountBank),0)
    FROM tabCapital
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(-Cash),0),
        IFNULL(SUM(-Bank),0)
    FROM tabExpences
    WHERE DATE(Date) BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY
)"
        Dim rdr As SQLiteDataReader = Nothing

        Try

            Dim params As New Dictionary(Of String, Object) From {
            {"@FDate", Get_BSD().ToString("yyyy-MM-dd")},
            {"@TDate", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            rdr = ExecuteReader(sql, params)

            If rdr.Read() Then

                Dim Cash As Decimal = If(IsDBNull(rdr("Cash")), 0D, CDec(rdr("Cash")))
                Dim Bank As Decimal = If(IsDBNull(rdr("Bank")), 0D, CDec(rdr("Bank")))

                txtACash.Text = "₹ " & Format(Cash, "#,##,##0.00")
                txtABank.Text = "₹ " & Format(Bank, "#,##,##0.00")
                txtATotal.Text = "₹ " & Format(Cash + Bank, "#,##,##0.00")

            Else

                txtACash.Text = "₹ 0.00"
                txtABank.Text = "₹ 0.00"
                txtATotal.Text = "₹ 0.00"

            End If

        Catch ex As Exception

            LogError(Me.Name, "getcashbankbal", sql, ex)
            MessageBox.Show(ex.ToString)

        Finally

            If rdr IsNot Nothing Then rdr.Close()

        End Try

    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub
    Private Sub fill_PName()

        Dim sql As String = ""
        Dim rdr As SQLiteDataReader = Nothing

        Try

            sql = "SELECT ID,BName FROM tabParty " &
              "WHERE CompanyID=@CompanyID " &
              "ORDER BY BName"

            Dim params As New Dictionary(Of String, Object) From {
            {"@CompanyID", CurrentCompanyID}
        }

            rdr = ExecuteReader(sql, params)

            Dim dt As New DataTable
            dt.Load(rdr)

            cboParty.DataSource = dt
            cboParty.DisplayMember = "BName"
            cboParty.ValueMember = "ID"

            cboParty.FlatStyle = FlatStyle.Flat
            cboParty.BackColor = Color.White
            cboParty.ForeColor = Color.FromArgb(68, 105, 80)

        Catch ex As Exception

            LogError(Me.Name, "fill_PName", sql, ex)
            MessageBox.Show(ex.Message)

        Finally

            If rdr IsNot Nothing Then rdr.Close()

        End Try

    End Sub

    Private Sub cboParty_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParty.GotFocus
        cboParty.FlatStyle = FlatStyle.Flat
    End Sub

    Private Sub cboParty_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParty.LostFocus
        ' cboParty.FlatStyle = FlatStyle.Standard
    End Sub

    Private Sub cboParty_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboParty.SelectedIndexChanged
        If cboParty.SelectedValue Is Nothing Then Exit Sub
        If cboParty.SelectedValue.ToString = "System.Data.DataRowView" Then Exit Sub
        getLaserAmt()
        fill_Grid()
        'MsgBox(cboParty.SelectedItem(0))
        cboParty.FlatStyle = FlatStyle.Flat
        cboParty.BackColor = Color.White
        cboParty.ForeColor = Color.FromArgb(68, 105, 80)
        getcashbankbal()
        getLaserAmt()

        txtTotalOutS.Text = txtLaserAmt.Text
    End Sub

    Private Sub getLaserAmt()

        Dim sql As String = ""
        Dim rdr As SQLiteDataReader = Nothing

        Try

            sql = "SELECT IFNULL(SUM(AmountDr),0) AS AmountDr, " &
              "IFNULL(SUM(AmountCR),0) AS AmountCR " &
              "FROM ( " &
              "SELECT IFNULL(SUM(AmtDr),0) AS AmountDr, " &
              "IFNULL(SUM(AmtCRBank + AmtCRCash),0) AS AmountCR " &
              "FROM tabPartyLaser " &
              "WHERE PartyID=@PartyID " &
              "AND CompanyID=@CompanyID " &
              "AND FY=@FY " &
              "UNION ALL " &
              "SELECT IFNULL(SUM(CapitalAmountCash + CapitalAmountBank),0) AS AmountDr, " &
              "0 AS AmountCR " &
              "FROM tabCapital " &
              "WHERE PartyID=@PartyID " &
              "AND CompanyID=@CompanyID " &
              "AND FY=@FY " &
              ") A"

            Dim params As New Dictionary(Of String, Object) From {
            {"@PartyID", cboParty.SelectedValue},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            rdr = ExecuteReader(sql, params)

            If rdr.Read() Then

                LadgerAmt = Val(rdr("AmountDr")) - Val(rdr("AmountCR"))

                txtLaserAmt.Text = "₹ " & Format(LadgerAmt, "#,##,##0.00")

            Else

                LadgerAmt = 0
                txtLaserAmt.Text = "₹ 0.00"

            End If

        Catch ex As Exception

            LogError(Me.Name, "getLaserAmt", sql, ex)
            MessageBox.Show(ex.Message)

        Finally

            If rdr IsNot Nothing Then rdr.Close()

        End Try

    End Sub

    Private Sub txtCash_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCash.GotFocus
        txtCash.SelectAll()
    End Sub

    Private Sub txtCash_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCash.LostFocus
        txtCash.Text = Format(Val(txtCash.Text), "0.00")
    End Sub

    Private Sub txtBank_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBank.GotFocus
        txtBank.SelectAll()
    End Sub

    Private Sub txtBank_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBank.LostFocus
        txtBank.Text = Format(Val(txtBank.Text), "0.00")
    End Sub

    Private Sub txtRef_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRef.GotFocus
        txtRef.SelectAll()
    End Sub


    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        Dim cash, bank As Decimal
        cash = Val(txtACash.Text.Replace("₹ ", "").Replace(",", ""))
        bank = Val(txtABank.Text.Replace("₹ ", "").Replace(",", ""))
        If Val(txtCash.Text) > cash Or Val(txtBank.Text) > bank Then
            MsgBox("Amount Not Availabe", MsgBoxStyle.Critical, "ERROR")
            If Val(txtCash.Text) > 0 Then
                txtCash.Focus()
            Else
                txtBank.Focus()
            End If
            Exit Sub
        End If

        If Val(txtCash.Text) + Val(txtBank.Text) < 1 Then
            MsgBox("Invalid Amount!", MsgBoxStyle.Critical, "ERROR")
            txtBank.Focus()
            Exit Sub
        End If

        UpdatePartyLaser()

        MsgBox("Payment done!", MsgBoxStyle.Information, "Success")

        fill_Grid()
        getcashbankbal()
        dtpDate.Focus()
        getLaserAmt()
        txtCash.Text = "0.00"
        txtBank.Text = "0.00"
    End Sub

    Private Sub UpdatePartyLaser()

        Dim sql As String = ""

        Try

            sql = "INSERT INTO tabPartyLaser " &
              "(PartyID,Date,AmtCRBank,AmtCRCash,Ref,FY,CompanyID,UserID) " &
              "VALUES " &
              "(@PartyID,@Date,@AmtCRBank,@AmtCRCash,@Ref,@FY,@CompanyID,@UserID)"

            Dim params As New Dictionary(Of String, Object) From {
            {"@PartyID", cboParty.SelectedValue},
            {"@Date", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@AmtCRBank", Val(txtBank.Text)},
            {"@AmtCRCash", Val(txtCash.Text)},
            {"@Ref", txtRef.Text.Trim()},
            {"@FY", CurrentFYID},
            {"@CompanyID", CurrentCompanyID},
            {"@UserID", CurrentUserID}
        }

            ExecuteNonQuery(sql, params)

        Catch ex As Exception

            LogError(Me.Name, "UpdatePartyLaser", sql, ex)
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub txtBank_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBank.TextChanged
        txtTotal.Text = "₹ " & Format(Val(txtBank.Text) + Val(txtCash.Text), "#,##,##0.00")
        txtTotalOutS.Text = "₹ " & Format((LadgerAmt - Val(txtBank.Text) - Val(txtCash.Text)), "#,##,##0.00")
        If (LadgerAmt - Val(txtBank.Text) - Val(txtCash.Text)) < 0 Then
            txtTotalOutS.ForeColor = Color.Red
        Else
            txtTotalOutS.ForeColor = Color.Orange
        End If
    End Sub

    Private Sub txtCash_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCash.TextChanged
        txtTotal.Text = "₹ " & Format(Val(txtBank.Text) + Val(txtCash.Text), "#,##,##0.00")
        txtTotalOutS.Text = "₹ " & Format((LadgerAmt - Val(txtBank.Text) - Val(txtCash.Text)), "#,##,##0.00")
        If (LadgerAmt - Val(txtBank.Text) - Val(txtCash.Text)) < 0 Then
            txtTotalOutS.ForeColor = Color.Red
        Else
            txtTotalOutS.ForeColor = Color.Orange
        End If
    End Sub

    Private Sub fill_Grid()

        Dim sql As String = ""

        Try

            If cboParty.SelectedValue Is Nothing Then Exit Sub
            If cboParty.SelectedValue.ToString = "System.Data.DataRowView" Then Exit Sub

            sql = "SELECT ID, " &
              "Date, " &
              "IFNULL(AmtCRBank,0) AS Bank, " &
              "IFNULL(AmtCRCash,0) AS Cash, " &
              "Ref AS Narration " &
              "FROM tabPartyLaser " &
              "WHERE Ref<>'Purchage' " &
              "AND PartyID=@PartyID " &
              "AND CompanyID=@CompanyID " &
              "AND FY=@FY " &
              "ORDER BY Date DESC, ID DESC"

            Dim params As New Dictionary(Of String, Object) From {
            {"@PartyID", cboParty.SelectedValue},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            Dim dt As DataTable = GetDataTable(sql, params)

            With dg1

                .DataSource = dt

                .Columns("ID").Visible = False
                .Columns("Date").Width = 100
                .Columns("Bank").Width = 100
                .Columns("Cash").Width = 100
                .Columns("Narration").Width = 300

                .Columns("Bank").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns("Cash").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                .Columns("Bank").DefaultCellStyle.Format = "0.00"
                .Columns("Cash").DefaultCellStyle.Format = "0.00"

                .RowHeadersVisible = False
                .ClearSelection()

            End With

        Catch ex As Exception

            LogError(Me.Name, "fill_Grid", sql, ex)
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub


    Private Sub dg1_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dg1.KeyUp

        If e.KeyCode <> Keys.Delete Then Exit Sub
        If dg1.SelectedRows.Count = 0 Then Exit Sub

        If MessageBox.Show("Are you sure to delete?",
                       "DELETE",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) = DialogResult.No Then Exit Sub

        Dim sql As String = ""

        Try

            sql = "DELETE FROM tabPartyLaser WHERE ID=@ID"

            Dim params As New Dictionary(Of String, Object) From {
            {"@ID", dg1.SelectedRows(0).Cells("ID").Value}
        }

            ExecuteNonQuery(sql, params)

            fill_Grid()
            getLaserAmt()
            getcashbankbal()

            cboParty.Focus()
            txtTotalOutS.Text = txtLaserAmt.Text

        Catch ex As Exception

            LogError(Me.Name, "dg1_KeyUp", sql, ex)
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub dtpDate_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDate.ValueChanged
        getcashbankbal()
        getLaserAmt()
        fill_Grid()
    End Sub

    Private Sub cmdSubmit_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub txtLaserAmt_Click(sender As Object, e As EventArgs) Handles txtLaserAmt.Click

    End Sub
End Class