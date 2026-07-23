Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Web
Imports System.Data.SQLite
Public Class frmPrintCashBook

    Private Sub frmPrintCashBook_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate2.MinDate = Get_FirstDay()
        dtpDate2.MaxDate = Now
        dtpDate.Value = Get_FirstDay()
        dtpDate2.Value = Now.ToLongDateString
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
        Dim sql As String
        Try



            Dim params As New Dictionary(Of String, Object)

            params.Add("@CompanyID", CurrentCompanyID)
            params.Add("@FY", CurrentFYID)
            params.Add("@Date1", dtpDate.Value.Date)
            params.Add("@Date2", dtpDate2.Value.Date)

            sql = "
SELECT *
FROM
(

SELECT
@Date1 AS Date,
'Opening Balance' AS Particulars,
SUM(CashIn) AS CashIn,
SUM(BankIn) AS BankIn,
0.0 AS CashOut,
0.0 AS BankOut
FROM
(

SELECT
IFNULL(SUM(CashAmtRcv),0) CashIn,
IFNULL(SUM(BankAmtRcv),0) BankIn
FROM tabLaserSale
WHERE Date<@Date1
AND CompanyID=@CompanyID

UNION ALL

SELECT
IFNULL(SUM(-AmtCRCash),0),
IFNULL(SUM(-AmtCRBank),0)
FROM tabPartyLaser
WHERE Date<@Date1
AND CompanyID=@CompanyID

UNION ALL

SELECT
IFNULL(SUM(-CashToBank),0),
IFNULL(SUM(CashToBank),0)
FROM tabCashFlow
WHERE Date<@Date1
AND CompanyID=@CompanyID

UNION ALL

SELECT
IFNULL(SUM(BankToCash),0),
IFNULL(SUM(-BankToCash),0)
FROM tabCashFlow
WHERE Date<@Date1
AND CompanyID=@CompanyID

UNION ALL

SELECT
IFNULL(SUM(CapitalAmountCash),0),
IFNULL(SUM(CapitalAmountBank),0)
FROM tabCapital
WHERE Date<@Date1
AND CompanyID=@CompanyID

UNION ALL

SELECT
IFNULL(SUM(-Cash),0),
IFNULL(SUM(-Bank),0)
FROM tabExpences
WHERE Date<@Date1
AND CompanyID=@CompanyID

)

UNION ALL

SELECT

Date,
'Capital Receipt',
CapitalAmountCash,
CapitalAmountBank,
0.0,
0.0

FROM tabCapital

WHERE CompanyID=@CompanyID
AND FY=@FY
AND Date BETWEEN @Date1 AND @Date2

UNION ALL

SELECT

Date,
'Sale Receipt',
CashAmtRcv,
BankAmtRcv,
0.0,
0.0

FROM tabLaserSale

WHERE CompanyID=@CompanyID
AND FY=@FY
AND Date BETWEEN @Date1 AND @Date2
AND (CashAmtRcv>0 OR BankAmtRcv>0)

            UNION ALL

SELECT
Date,
'Purchage Payment',
0.0,
0.0,
AmtCRCash,
AmtCRBank

FROM tabPartyLaser

WHERE CompanyID=@CompanyID
AND FY=@FY
AND Date BETWEEN @Date1 AND @Date2
AND (AmtCRCash>0 OR AmtCRBank>0)

UNION ALL

SELECT

e.Date,
eh.ExpHead || ' {' || IFNULL(e.Ref,'') || '}',
0.0,
0.0,
e.Cash,
e.Bank

FROM tabExpences e
LEFT JOIN tabExpHead eh
ON eh.ID=e.ExpHeadID

WHERE e.CompanyID=@CompanyID
AND e.FY=@FY
AND e.Date BETWEEN @Date1 AND @Date2
AND (e.Cash>0 OR e.Bank>0)

UNION ALL

SELECT

Date,
'Cash Bank Transfer',
BankToCash,
CashToBank,
CashToBank,
BankToCash

FROM tabCashFlow

WHERE CompanyID=@CompanyID
AND FY=@FY
AND Date BETWEEN @Date1 AND @Date2

)

ORDER BY Date"

        Dim dt As DataTable = GetDataTable(sql, params)

        For Each r As DataRow In dt.Rows
            r("CashIn") = CDbl(r("CashIn"))
            r("BankIn") = CDbl(r("BankIn"))
            r("CashOut") = CDbl(r("CashOut"))
            r("BankOut") = CDbl(r("BankOut"))
        Next

        Dim ds As New Reports
        ds.dtCashBook.Merge(dt)

        Dim cryRpt As New rptCashBook
        cryRpt.SetDataSource(ds)

        cryRpt.SetParameterValue("CompName", CurrentCompanyName)
        cryRpt.SetParameterValue("Date1", dtpDate.Value.ToShortDateString)
        cryRpt.SetParameterValue("Date2", dtpDate2.Value.ToShortDateString)
        cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
        cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
        cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
        cryRpt.SetParameterValue("FY", Get_CurrentFY())

        frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
        frmViewRpt.CrystalReportViewer1.ToolPanelView =
            CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        frmViewRpt.CrystalReportViewer1.Refresh()

        Me.Close()

        frmViewRpt.MdiParent = mdiMain
        frmViewRpt.WindowState = FormWindowState.Maximized
        frmViewRpt.Show()

    Catch ex As Exception

        LogError("frmPrintCashBook",
                 "cmdPrint_Click",
                 sql,
                 ex)

        MessageBox.Show(ex.Message)

    End Try

End Sub

End Class