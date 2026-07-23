Imports CrystalDecisions.CrystalReports.Engine
Imports System.Data.SQLite
Public Class frmPrintMonthwiseSale

    Private Sub frmPrintMonthwiseSale_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate.Value = Now
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
"SELECT
    b.ItemName,
    SUM(b.Qty) AS Sold,
    SUM(b.Stock) AS Stock
FROM
(
    SELECT
        i.ItemName,
        SUM(s.Qty) AS Qty,
        0.0 AS Stock
    FROM tabSale s
    INNER JOIN tabItem i
        ON i.ID=s.ItemID
    WHERE CAST(strftime('%m', s.Date) AS INTEGER)=@Month
      AND s.CompanyID=@CompanyID
     
    GROUP BY i.ItemName

    UNION ALL

    SELECT
        a.ItemName,
        0.0 AS Qty,
        SUM(a.Qty) AS Stock
    FROM
    (
        SELECT
            i.ItemName,
            SUM(ps.Qty) AS Qty
        FROM tabPurchageSlave ps
        INNER JOIN tabPurchageMaster pm
            ON pm.ID=ps.MasterID
        INNER JOIN tabItem i
            ON i.ID=ps.ItemID
        WHERE pm.CompanyID=@CompanyID
          
        GROUP BY i.ItemName

        UNION ALL

        SELECT
            i.ItemName,
            SUM(-s.Qty) AS Qty
        FROM tabSale s
        INNER JOIN tabItem i
            ON i.ID=s.ItemID
        WHERE s.CompanyID=@CompanyID
          
        GROUP BY i.ItemName
    ) a
    GROUP BY a.ItemName
) b
GROUP BY b.ItemName
ORDER BY Sold DESC"

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@Month", dtpDate.Value.Month)
            Params.Add("@CompanyID", CurrentCompanyID)


            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            ds.dtDaywiseSalestock.Merge(dt)

            Dim cryRpt As New rptDaywiseSaleandCurrentStock
            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYName)
            cryRpt.SetParameterValue("DateFrom", Get_FirstDay())
            cryRpt.SetParameterValue("DateTo", dtpDate.Value.Date)

            With frmViewRpt.CrystalReportViewer1
                .ReportSource = cryRpt
                .ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
                .ShowRefreshButton = False
                .Refresh()
            End With

            Me.Close()

            frmViewRpt.MdiParent = mdiMain
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception

            LogError(Me.Name, "cmdPrint_Click", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub
End Class