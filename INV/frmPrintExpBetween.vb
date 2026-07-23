Imports CrystalDecisions.CrystalReports.Engine

Public Class frmPrintExpBetween

    Private Sub frmPrintExpBetween_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate2.MinDate = Get_FirstDay()
        dtpDate2.MaxDate = Now

        dtpDate.Value = Get_FirstDay()
        dtpDate2.Value = Today()
        dtpDate.Focus()
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Function CalculateDays() As Integer
        Return DateDiff(DateInterval.Day, dtpDate.Value.Date, dtpDate2.Value.Date) + 1
    End Function

    Private Sub dtpDate_ValueChanged(sender As Object, e As EventArgs) Handles dtpDate.ValueChanged
        CalculateDays()
    End Sub

    Private Sub dtpDate2_ValueChanged(sender As Object, e As EventArgs) Handles dtpDate2.ValueChanged
        CalculateDays()
    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
        Dim sql As String
        Try

            Dim cryRpt As New rptExp

            Sql =
    "SELECT
0 AS SortOrder,
'Sales Profit' AS ExpHead,
IFNULL(SUM((s.Qty*s.Rate)-(s.Qty*i.PRate)),0) AS Income,
0 AS Expense
FROM tabSale s
LEFT JOIN tabItem i ON i.ID=s.ItemID
WHERE s.Date BETWEEN @FromDate AND @ToDate
AND s.CompanyID=@CompanyID
AND s.FY=@FY
HAVING IFNULL(SUM((s.Qty*s.Rate)-(s.Qty*i.PRate)),0) > 0

UNION ALL

SELECT
1 AS SortOrder,
eh.ExpHead,
0 AS Income,
IFNULL(SUM(IFNULL(ex.Cash,0)+IFNULL(ex.Bank,0)),0) AS Expense
FROM tabExpHead eh
LEFT JOIN tabExpences ex
ON ex.ExpHeadID=eh.ID
AND ex.Date BETWEEN @FromDate AND @ToDate
AND ex.CompanyID=@CompanyID
AND ex.FY=@FY
GROUP BY eh.ExpHead
HAVING IFNULL(SUM(IFNULL(ex.Cash,0)+IFNULL(ex.Bank,0)),0) > 0

ORDER BY SortOrder, ExpHead;"

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@FromDate", dtpDate.Value.Date)
            Params.Add("@ToDate", dtpDate2.Value.Date)
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            ds.dtExp.Clear()
            ds.dtExp.Merge(dt)

            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue(0, dtpDate.Value.ToShortDateString)
            cryRpt.SetParameterValue(1, dtpDate2.Value.ToShortDateString)
            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", Get_CurrentFY())

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.Refresh()
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None

            Me.Close()

            frmViewRpt.MdiParent = mdiMain
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception
            LogError(Me.Name, "cmdPrint_Click", Sql, ex)
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

End Class