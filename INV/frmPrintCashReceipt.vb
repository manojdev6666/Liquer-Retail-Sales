Imports CrystalDecisions.CrystalReports.Engine
Imports System.Data.SqlClient
Public Class frmPrintCashReceipt

    Private Sub frmPrintCashReceipt_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate2.MinDate = Get_FirstDay()
        dtpDate2.MaxDate = Now
        dtpDate.Value = Get_FirstDay()
        dtpDate2.Value = Today
        'dtpDate.Enabled = False
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click

        Dim sql As String = ""

        Try

            sql =
        "SELECT Date,
                IFNULL(SoldAmtCash,0) AS CashSale,
                IFNULL(SoldAmtBank,0) AS BankSale,
                IFNULL(SoldAmtCash,0)+IFNULL(SoldAmtBank,0) AS TotalSale,
                IFNULL(CashAmtRcv,0) AS CashReceipt,
                IFNULL(BankAmtRcv,0) AS BankReceipt,
                IFNULL(CashAmtRcv,0)+IFNULL(BankAmtRcv,0) AS TotalReceipt
         FROM tabLaserSale
         WHERE CompanyID=@CompanyID
           AND FY=@FY
           AND Date BETWEEN @FromDate AND @ToDate
         ORDER BY Date"

            Dim Params As New Dictionary(Of String, Object)
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@FromDate", dtpDate.Value.Date)
            Params.Add("@ToDate", dtpDate2.Value.Date)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            ds.dtCashBankReceipt.Merge(dt)

            Dim cryRpt As New rptCashBankReceipt

            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("Date1", dtpDate.Value.ToLongDateString)
            cryRpt.SetParameterValue("Date2", dtpDate2.Value.ToLongDateString)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
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

            LogError("frmPrintCashReceipt",
                 "cmdPrint_Click",
                 sql,
                 ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub
End Class