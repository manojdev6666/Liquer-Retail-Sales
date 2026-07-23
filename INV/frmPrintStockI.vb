Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine

Public Class frmPrintStockI

    Private Sub frmPrintStockI_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cboItemName.Focus()
        fill_IName()
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub
    Private Sub fill_IName()

        Dim sql As String =
    "SELECT ID, ItemName
     FROM tabItem
          ORDER BY ItemName"

        Try



            Dim dt As DataTable = GetDataTable(sql)

            cboItemName.DataSource = dt
            cboItemName.DisplayMember = "ItemName"
            cboItemName.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_IName", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click

        Dim sql As String = ""

        Try

            sql =
"SELECT * FROM
(
    SELECT
        0 AS ItemID,
        'Opening Stock' AS ItemName,
        @FirstDay AS Date,
        (
            SELECT IFNULL(SUM(PQty-SQty),0)
            FROM
            (
                SELECT ps.Qty AS PQty,
                       0 AS SQty
                FROM tabPurchageSlave ps
                INNER JOIN tabPurchageMaster pm
                    ON pm.ID=ps.MasterID
                WHERE pm.Date<@FirstDay
                  AND ps.ItemID=@ItemID
                  AND pm.CompanyID=@CompanyID

                UNION ALL

                SELECT
                    0,
                    s.Qty
                FROM tabSale s
                WHERE s.Date<@FirstDay
                  AND s.ItemID=@ItemID
                  AND s.CompanyID=@CompanyID
            )
        ) AS PQty,
        0 AS SQty

    UNION ALL

    SELECT
        i.ID,
        i.ItemName,
        pm.Date,
        ps.Qty,
        0
    FROM tabPurchageSlave ps
    INNER JOIN tabPurchageMaster pm
        ON pm.ID=ps.MasterID
    INNER JOIN tabItem i
        ON i.ID=ps.ItemID
    WHERE pm.CompanyID=@CompanyID
      AND pm.FY=@FY
      AND i.ID=@ItemID

    UNION ALL

    SELECT
        i.ID,
        i.ItemName,
        s.Date,
        0,
        s.Qty
    FROM tabSale s
    INNER JOIN tabItem i
        ON i.ID=s.ItemID
    WHERE s.CompanyID=@CompanyID
      AND s.FY=@FY
      AND s.ItemID=@ItemID
)
ORDER BY Date"

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@FirstDay", Get_FirstDay())
            Params.Add("@ItemID", CInt(cboItemName.SelectedValue))
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            'For Each c As DataColumn In dt.Columns
            'MessageBox.Show(c.ColumnName & " = " & c.DataType.ToString)
            'Next
            ds.dtStockI.Merge(dt)

            Dim cryRpt As New rptStockI
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

            frmViewRpt.MdiParent = mdiMain
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

            Me.Close()

        Catch ex As Exception

            LogError(Me.Name, "cmdPrint_Click", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub
End Class