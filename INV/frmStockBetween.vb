Imports CrystalDecisions.CrystalReports.Engine
Imports System.Data.SqlClient

Public Class frmStockBetween

    Private Sub dtpDate2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDate2.ValueChanged

    End Sub

    Private Sub frmStockBetween_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_BSD()
        dtpDate.MaxDate = Now
        dtpDate.Value = Get_BSD()
        dtpDate2.MinDate = Get_FirstDay()
        dtpDate2.MaxDate = Now
        dtpDate2.Focus()
        dtpDate2.Value = Now
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
    ItemName,
    Prate,
    SRate,
    IFNULL(SUM(Qty),0) AS STOCK
FROM
(
    SELECT
        i.ItemName,
        i.PRate,
        i.SRate,
        SUM(ps.Qty) AS Qty
    FROM tabPurchageSlave ps
    INNER JOIN tabItem i
        ON i.ID=ps.ItemID
    INNER JOIN tabPurchageMaster pm
        ON pm.ID=ps.MasterID
    WHERE pm.Date BETWEEN @Date1 AND @Date2
      AND pm.CompanyID=@CompanyID
      
    GROUP BY i.ItemName,i.PRate,i.SRate

    UNION ALL

    SELECT
        i.ItemName,
        i.PRate,
        i.SRate,
        SUM(-s.Qty) AS Qty
    FROM tabSale s
    INNER JOIN tabItem i
        ON i.ID=s.ItemID
    WHERE s.Date BETWEEN @Date1 AND @Date2
      AND s.CompanyID=@CompanyID
      
    GROUP BY i.ItemName,i.PRate,i.SRate
)
GROUP BY ItemName,Prate,SRate
ORDER BY ItemName"

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@Date1", Get_BSD())
            Params.Add("@Date2", dtpDate2.Value.Date)
            Params.Add("@CompanyID", CurrentCompanyID)


            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            ds.dtCurrentStock.Merge(dt)

            Dim cryRpt As New rptCurrentStock
            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYName)
            cryRpt.SetParameterValue("Date1", Get_FirstDay())
            cryRpt.SetParameterValue("Date2", Today)

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