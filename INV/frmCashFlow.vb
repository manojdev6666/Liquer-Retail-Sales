Imports System.Data.SQLite

Public Class frmCashFlow

    Private Sub frmCashFlow_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        getcashbankbal()
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(64, 64))
    End Sub

    Private Sub getcashbankbal()

        Dim sql As String = "
SELECT IFNULL(SUM(Cash),0) AS Cash,
       IFNULL(SUM(Bank),0) AS Bank
FROM
(
    SELECT IFNULL(SUM(CashAmtRcv),0) AS Cash,
           IFNULL(SUM(BankAmtRcv),0) AS Bank
    FROM tabLaserSale
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT IFNULL(SUM(-AmtCrCash),0),
           IFNULL(SUM(-AmtCrBank),0)
    FROM tabPartyLaser
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT IFNULL(SUM(-CashToBank),0),
           IFNULL(SUM(CashToBank),0)
    FROM tabCashFlow
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT IFNULL(SUM(BankToCash),0),
           IFNULL(SUM(-BankToCash),0)
    FROM tabCashFlow
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT IFNULL(SUM(CapitalAmountCash),0),
           IFNULL(SUM(CapitalAmountBank),0)
    FROM tabCapital
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND CompanyID=@CompanyID
      AND FY=@FY

    UNION ALL

    SELECT IFNULL(SUM(-Cash),0),
           IFNULL(SUM(-Bank),0)
    FROM tabExpences
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND CompanyID=@CompanyID
      AND FY=@FY
) A"

        Try

            Dim params As New Dictionary(Of String, Object)

            params.Add("@FromDate", Get_FirstDay().ToString("yyyy-MM-dd"))
            params.Add("@ToDate", dtpDate.Value.ToString("yyyy-MM-dd"))
            params.Add("@CompanyID", CurrentCompanyID)
            params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, params)

            If dt.Rows.Count > 0 Then

                lblCash.Text = Format(Val(dt.Rows(0)("Cash")), "0.00")
                lblBank.Text = Format(Val(dt.Rows(0)("Bank")), "0.00")
                lblTotal.Text = Format(Val(lblCash.Text) + Val(lblBank.Text), "#,##,##0.00")

            Else

                lblCash.Text = "0.00"
                lblBank.Text = "0.00"
                lblTotal.Text = "0.00"

            End If

        Catch ex As Exception

            LogError("frmCashFlow", "getcashbankbal", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub txtCtoB_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCtoB.GotFocus
        txtCtoB.SelectAll()
    End Sub
    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub
    
    

    Private Sub txtBtoC_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBtoC.GotFocus
        txtBtoC.SelectAll()
    End Sub

    
    Private Sub txtCtoB_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCtoB.LostFocus
        txtCtoB.Text = Format(Val(txtCtoB.Text), "0.00")
    End Sub

    Private Sub txtBtoC_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBtoC.LostFocus
        txtBtoC.Text = Format(Val(txtBtoC.Text), "0.00")
    End Sub

    Private Sub cmdSubmit_Click(sender As System.Object, e As System.EventArgs) Handles cmdSubmit.Click

        Try

            If Val(txtBtoC.Text) + Val(txtCtoB.Text) > 0 Then

                If Val(txtCtoB.Text) > Val(lblCash.Text) AndAlso Val(lblCash.Text) > 0 Then
                    MsgBox("Amount not Available", MsgBoxStyle.Critical, "ERROR")
                    txtCtoB.Focus()
                    Exit Sub
                End If

                If Val(txtBtoC.Text) > Val(lblBank.Text) AndAlso Val(lblBank.Text) > 0 Then
                    MsgBox("Amount not Available", MsgBoxStyle.Critical, "ERROR")
                    txtBtoC.Focus()
                    Exit Sub
                End If

                InsertTransfer()

                MsgBox("Amount transfer done!", MsgBoxStyle.Information, "Success")

                txtBtoC.Text = "0.00"
                txtCtoB.Text = "0.00"

                txtBtoC.Focus()

                getcashbankbal()

            Else

                MsgBox("Enter Amount to transfer", MsgBoxStyle.Critical, "ERROR")
                txtCtoB.Focus()

            End If

        Catch ex As Exception

            LogError("frmCashFlow", "cmdSubmit_Click", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub


    Private Sub InsertTransfer()

        Dim sql As String =
    "INSERT INTO tabCashFlow
    (Date, CashToBank, BankToCash, Ref, FY, CompanyID, UserID)
    VALUES
    (@Date, @CashToBank, @BankToCash, @Ref, @FY, @CompanyID, @UserID)"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@Date", dtpDate.Value.ToString("yyyy-MM-dd"))
            Params.Add("@CashToBank", Val(txtCtoB.Text))
            Params.Add("@BankToCash", Val(txtBtoC.Text))
            Params.Add("@Ref", "Transfer")
            Params.Add("@FY", CurrentFYID)
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@UserID", CurrentUserID)

            ExecuteNonQuery(sql, Params)

        Catch ex As Exception

            LogError("frmCashFlow", "InsertTransfer", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub dtpDate_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDate.ValueChanged
        getcashbankbal()
    End Sub

    Private Sub txtCtoB_TextChanged(sender As Object, e As EventArgs) Handles txtCtoB.TextChanged
        If Val(txtCtoB.Text) > 0 Then
            txtBtoC.Text = "0.00"
        End If
    End Sub

    Private Sub txtBtoC_TextChanged(sender As Object, e As EventArgs) Handles txtBtoC.TextChanged
        If Val(txtBtoC.Text) > 0 Then
            txtCtoB.Text = "0.00"
        End If
    End Sub
End Class