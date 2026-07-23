Imports System.Data.SQLite
Imports System.Drawing.Drawing2D


Public Class frmReceipt
    Dim cash, bank As Decimal

    Private Sub txtcashR_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtcashR.SelectAll()
    End Sub



    Private Sub txtBankR_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtBankR.Text = Format(Val(txtBankR.Text), "0.00")
    End Sub


    Private Sub txtBankR_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtTotal.Text = Format(Val(txtBankR.Text) + Val(txtcashR.Text), "0.00")
    End Sub


    Private Sub frmReceipt_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cash = 0
        bank = 0
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate.Focus()
        dtpDate.Value = Now.Date
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(32, 32))
        FillRemaining()

    End Sub

    Private Sub txtCashS_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtTotalS.Text = Format(Val(txtBankS.Text) + Val(txtCashS.Text), "0.00")
        ' txtcashR.Text = Format(Val(txtCashS.Text), "0.00")
    End Sub

    Private Sub txtBankS_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtTotalS.Text = Format(Val(txtBankS.Text) + Val(txtCashS.Text), "0.00")
        ' txtBankR.Text = Format(Val(txtBankS.Text), "0.00")
    End Sub

    Private Sub txtcashR_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtcashR.Text = Format(Val(txtcashR.Text), "0.00")
    End Sub

    Private Sub txtcashR_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtTotal.Text = Format(Val(txtBankR.Text) + Val(txtcashR.Text), "0.00")
    End Sub

    Private Sub GetData()

        Dim sql As String = ""
        Dim rdr As SQLiteDataReader = Nothing

        Try

            cash = 0
            bank = 0

            sql = "SELECT IFNULL(SoldAmtCash,0) AS SoldAmtCash," &
              " IFNULL(SoldAmtBank,0) AS SoldAmtBank " &
              "FROM tabLaserSale " &
              "WHERE DATE(Date)=DATE(@Date) " &
              "AND CompanyID=@CompanyID " &
              "AND FY=@FY"

            Dim params As New Dictionary(Of String, Object) From {
            {"@Date", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            rdr = ExecuteReader(sql, params)

            If rdr.Read() Then

                cash = Val(rdr("SoldAmtCash"))
                bank = Val(rdr("SoldAmtBank"))

                txtCashS.Text = "₹ " & Format(cash, "#,##,##0")
                txtBankS.Text = "₹ " & Format(bank, "#,##,##0")
                txtTotalS.Text = "₹ " & Format(cash + bank, "#,##,##0")

            Else

                cash = 0
                bank = 0

                txtCashS.Text = "₹ 0"
                txtBankS.Text = "₹ 0"
                txtTotalS.Text = "₹ 0"

                txtcashR.Text = "0.00"
                txtBankR.Text = "0.00"

            End If

        Catch ex As Exception

            LogError(Me.Name, "GetData", sql, ex)
            MessageBox.Show(ex.Message)

        Finally

            If rdr IsNot Nothing Then rdr.Close()

        End Try

    End Sub

    Private Sub dtpDate_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDate.ValueChanged
        cash = 0
        bank = 0
        GetData()
        GetRecepit()

    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub GetRecepit()

        Dim sql As String = ""
        Dim rdr As SQLiteDataReader = Nothing

        Try

            txtNarration.Clear()

            sql = "SELECT IFNULL(CashAmtRcv,0) AS CashAmtRcv," &
              " IFNULL(BankAmtRcv,0) AS BankAmtRcv," &
              " IFNULL(Narration,'') AS Narration " &
              "FROM tabLaserSale " &
              "WHERE DATE(Date)=DATE(@Date) " &
              "AND CompanyID=@CompanyID " &
              "AND FY=@FY"

            Dim params As New Dictionary(Of String, Object) From {
            {"@Date", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            rdr = ExecuteReader(sql, params)

            If rdr.Read() Then

                txtcashR.Text = Format(rdr("CashAmtRcv"), "0.00")
                txtBankR.Text = Format(rdr("BankAmtRcv"), "0.00")
                txtNarration.Text = rdr("Narration").ToString()

            Else

                txtcashR.Text = "0.00"
                txtBankR.Text = "0.00"
                txtTotal.Text = "₹ 0"
                txtNarration.Clear()

            End If

        Catch ex As Exception

            LogError(Me.Name, "GetRecepit", sql, ex)
            MessageBox.Show(ex.Message)

        Finally

            If rdr IsNot Nothing Then rdr.Close()

        End Try

    End Sub

    Private Sub Update_SaleLaser()

        Dim sql As String = ""

        Try

            sql = "UPDATE tabLaserSale SET " &
              "CashAmtRcv=@CashAmtRcv, " &
              "BankAmtRcv=@BankAmtRcv, " &
              "Narration=@Narration " &
              "WHERE DATE(Date)=DATE(@Date) " &
              "AND CompanyID=@CompanyID " &
              "AND FY=@FY"

            Dim params As New Dictionary(Of String, Object) From {
            {"@CashAmtRcv", Val(txtcashR.Text)},
            {"@BankAmtRcv", Val(txtBankR.Text)},
            {"@Narration", txtNarration.Text.Trim()},
            {"@Date", dtpDate.Value.ToString("yyyy-MM-dd")},
            {"@CompanyID", CurrentCompanyID},
            {"@FY", CurrentFYID}
        }

            ExecuteNonQuery(sql, params)

        Catch ex As Exception

            LogError(Me.Name, "Update_SaleLaser", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        '
        Update_SaleLaser()
        MsgBox("Data Saved", MsgBoxStyle.Information, "Data Saved")
        GetData()
        GetRecepit()
        dtpDate.Focus()
    End Sub

    Private Sub txtcashR_TextChanged_1(sender As Object, e As EventArgs) Handles txtcashR.TextChanged
        Calculatettl()
        FillRemaining()
    End Sub
    Private Sub Calculatettl()
        txtTotal.Text = "₹ " & Format(Val(txtcashR.Text) + Val(txtBankR.Text), "#,##,##0")
    End Sub

    Private Sub txtBankR_TextChanged_1(sender As Object, e As EventArgs) Handles txtBankR.TextChanged
        Calculatettl()
        FillRemaining()
    End Sub

    Private Sub FillRemaining()
        Dim remAmt As Double
        remAmt = (cash + bank) - Val(txtcashR.Text) - Val(txtBankR.Text)
        lblRemaining.Text = "₹ " & Format(remAmt, "#,##,##0")
    End Sub
End Class