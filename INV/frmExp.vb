Imports System.Data.SQLite

Public Class frmExp

    Private Sub frmExp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate.Value = Now.Date
        getcashbankbal()
        fill_Exp()
        fill_Grid()
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

    Private Sub txtCash_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtCash.SelectAll()
    End Sub

    Private Sub txtCash_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtCash.Text = Format(Val(txtCash.Text), "0.00")
    End Sub

    Private Sub txtBank_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtBank.SelectAll()
    End Sub

    Private Sub txtBank_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtBank.Text = Format(Val(txtBank.Text), "0.00")
    End Sub



    Private Sub getcashbankbal()

        Dim sql As String =
<sql>
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
        IFNULL(SUM(-AmtCRCash),0) AS Cash,
        IFNULL(SUM(-AmtCRBank),0) AS Bank
    FROM tabPartyLaser
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(-CashToBank),0) AS Cash,
        IFNULL(SUM(CashToBank),0) AS Bank
    FROM tabCashFlow
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(BankToCash),0) AS Cash,
        IFNULL(SUM(-BankToCash),0) AS Bank
    FROM tabCashFlow
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(CapitalAmountCash),0) AS Cash,
        IFNULL(SUM(CapitalAmountBank),0) AS Bank
    FROM tabCapital
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT
        IFNULL(SUM(-Cash),0) AS Cash,
        IFNULL(SUM(-Bank),0) AS Bank
    FROM tabExpences
    WHERE Date BETWEEN @FDate AND @TDate
      AND CompanyID=@CompanyID
      AND FY=@FY
) A
</sql>.Value

        Try

            Dim params As New Dictionary(Of String, Object) From {
            {"@FDate", Get_FirstDay().ToString("yyyy-MM-dd")},
            {"@TDate", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            Dim dt As DataTable = GetDataTable(sql, params)

            If dt.Rows.Count > 0 Then

                Dim Cash As Decimal = If(IsDBNull(dt.Rows(0)("Cash")), 0D, CDec(dt.Rows(0)("Cash")))
                Dim Bank As Decimal = If(IsDBNull(dt.Rows(0)("Bank")), 0D, CDec(dt.Rows(0)("Bank")))

                lblCash.Text = Cash.ToString("#,##,##0.00")
                lblBank.Text = Bank.ToString("#,##,##0.00")
                lblTotal.Text = (Cash + Bank).ToString("#,##,##0.00")

            Else

                lblCash.Text = "0.00"
                lblBank.Text = "0.00"
                lblTotal.Text = "0.00"

            End If

        Catch ex As Exception

            LogError(Me.Name, "getcashbankbal", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub fill_Exp()

        Dim sql As String =
            "SELECT ID, ExpHead
         FROM tabExpHead
         WHERE CompanyID=@CompanyID
         ORDER BY ExpHead"

        Try

            Dim params As New Dictionary(Of String, Object) From {
                {"@CompanyID", CurrentCompanyID}
            }

            Dim dt As DataTable = GetDataTable(sql, params)

            cboExp.DataSource = dt
            cboExp.DisplayMember = "ExpHead"
            cboExp.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_Exp", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub



    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        If Val(txtCash.Text) > Val(lblCash.Text) And Val(txtCash.Text) > 0 Then
            MsgBox("Amount Not available!", MsgBoxStyle.Critical, "ERROR")
            txtCash.Focus()
            Exit Sub
        End If
        If Val(txtBank.Text) > Val(lblBank.Text) And Val(txtBank.Text) > 0 Then
            MsgBox("Amount Not available!", MsgBoxStyle.Critical, "ERROR")
            txtBank.Focus()
            Exit Sub
        End If

        If Val(txtBank.Text) < 0 Or Val(txtCash.Text) < 0 Then
            MsgBox("Amount Not Valid!", MsgBoxStyle.Critical, "ERROR")
            txtCash.Focus()
            Exit Sub
        End If
        InsertExp()
        getcashbankbal()
    End Sub


    Private Sub InsertExp()

        Dim sql As String =
        "INSERT INTO tabExpences
        (Date,ExpHeadID,Cash,Bank,Ref,FY,CompanyID,UserID)
        VALUES
        (@Date,@ExpHeadID,@Cash,@Bank,@Ref,@FY,@CompanyID,@UserID)"

        Try

            Dim params As New Dictionary(Of String, Object) From {
            {"@Date", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@ExpHeadID", cboExp.SelectedValue},
            {"@Cash", Val(txtCash.Text)},
            {"@Bank", Val(txtBank.Text)},
            {"@Ref", txtRef.Text.Trim()},
            {"@FY", CurrentFYID},
            {"@CompanyID", CurrentCompanyID},
            {"@UserID", CurrentUserID}
        }

            ExecuteNonQuery(sql, params)

            getcashbankbal()
            fill_Grid()

            txtCash.Text = "0.00"
            txtBank.Text = "0.00"
            txtRef.Clear()
            cboExp.Focus()

        Catch ex As Exception

            LogError(Me.Name, "InsertExp", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub


    Private Sub fill_Grid()

        Dim sql As String =
"SELECT
    e.ID,
    e.Date,
    h.ExpHead,
    e.Cash,
    e.Bank,
    e.Ref
FROM tabExpences e
LEFT JOIN tabExpHead h
ON h.ID=e.ExpHeadID
WHERE e.Date=@Date
AND e.CompanyID=@CompanyID
AND e.FY=@FY"

        Try

            Dim params As New Dictionary(Of String, Object) From {
            {"@Date", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            dg1.DataSource = GetDataTable(sql, params)

            dg1.Columns("ID").Visible = False
            dg1.Columns("Date").Width = 75
            dg1.Columns("ExpHead").Width = 120
            dg1.Columns("Cash").Width = 75
            dg1.Columns("Bank").Width = 75
            dg1.Columns("Ref").Width = 200

            dg1.Columns("Cash").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            dg1.Columns("Bank").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            dg1.Columns("Cash").DefaultCellStyle.Format = "N2"
            dg1.Columns("Bank").DefaultCellStyle.Format = "N2"

            dg1.ClearSelection()

        Catch ex As Exception

            LogError(Me.Name, "fill_Grid", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub dtpDate_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDate.ValueChanged
        fill_Grid()
        getcashbankbal()
    End Sub

    Private Sub dg1_KeyUp(sender As Object, e As KeyEventArgs) Handles dg1.KeyUp

        If e.KeyCode <> Keys.Delete Then Exit Sub

        If dg1.SelectedRows.Count = 0 Then Exit Sub

        If MessageBox.Show("Delete selected expense?", "Confirm",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) = DialogResult.No Then Exit Sub

        Dim sql As String = "DELETE FROM tabExpences WHERE ID=@ID"

        Try

            Dim params As New Dictionary(Of String, Object) From {
            {"@ID", CInt(dg1.SelectedRows(0).Cells("ID").Value)}
        }

            ExecuteNonQuery(sql, params)

            fill_Grid()
            getcashbankbal()

        Catch ex As Exception

            LogError(Me.Name, "dg1_KeyUp", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub txtCash_TextChanged(sender As Object, e As EventArgs) Handles txtCash.TextChanged
        If Val(txtCash.Text) > 0 Then
            txtBank.Text = "0.00"
        End If
    End Sub

    Private Sub txtBank_TextChanged(sender As Object, e As EventArgs) Handles txtBank.TextChanged
        If Val(txtBank.Text) > 0 Then
            txtCash.Text = "0.00"
        End If
    End Sub
End Class