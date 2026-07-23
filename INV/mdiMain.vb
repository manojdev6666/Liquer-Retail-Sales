Imports System.Data.SQLite
Imports System.Windows.Forms
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.ReportAppServer.ReportDefModel
Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Net.NetworkInformation
Imports System.ComponentModel
Public Class mdiMain
    Private Sub network()
        If NetworkInterface.GetIsNetworkAvailable() Then
            lblNet.Text = "Online"
        Else
            lblNet.Text = "Offline"
        End If
    End Sub
    Private Sub Database()
        If mySQLiteConn IsNot Nothing AndAlso mySQLiteConn.State = ConnectionState.Open Then
            lblDatabase.Text = "Connected"
        Else
            lblDatabase.Text = "Disconnected"
        End If
    End Sub

    Private Sub Server()
        Try
            lblServer.Text = Dns.GetHostName
        Catch ex As Exception
            lblServer.Text = "Unknown"
        End Try
    End Sub
    Private Sub GetTemperature()
        Try
            Dim url As String = "https://api.open-meteo.com/v1/forecast?latitude=26.817322&longitude=82.763489&current=temperature_2m"
            Dim wc As New WebClient()
            Dim json As String = wc.DownloadString(url)
            Dim obj As JObject = JObject.Parse(json)
            Dim temp As String = obj("current")("temperature_2m").ToString()
            lblTemp.Text = temp & " °C"
        Catch ex As Exception
            lblTemp.Text = "ERROR"
        End Try
    End Sub
    Private Sub GetLocation()
        Try
            Dim wc As New WebClient()
            Dim json As String = wc.DownloadString("https://ipinfo.io/json")
            Dim obj As JObject = JObject.Parse(json)
            Dim city As String = obj("city").ToString()
            Dim state As String = obj("region").ToString()
            Dim country As String = obj("country").ToString()
            lblLocation.Text = city & ", " & state & ", " & country
        Catch ex As Exception
            lblLocation.Text = ex.Message
            MsgBox(ex.Message)
        End Try
    End Sub


    Private Sub mdiMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Text = "Liquor - " & CurrentCompanyName & " | User : " & CurrentUserName


        ts1.ImageScalingSize = New Size(64, 64)
        ts1.AutoSize = False
        ts1.Height = 70
        MenuStrip.ForeColor = Color.White
        'frmLogin.ShowDialog(Me)
        Dim dash As New frmDashBoard
        dash.MdiParent = Me
        dash.Show()
        ts1.ImageScalingSize = New Size(64, 64)
        ts1.AutoSize = False
        ts1.Height = 70
        GetTemperature()
        GetLocation()
        lblFY.Text = CurrentFYName
        network()
        Database()
        Server()

    End Sub

    Private Sub tmr1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmr1.Tick
        lblDate.Text = Now.ToString("dd-MM-yyyy") & vbCrLf & Now.ToString("dddd")
        lblTime.Text = Now.ToString("hh:mm:ss tt")
        lblUser.Text = CurrentUserName
        lblCompany.Text = CurrentCompanyName
    End Sub

    Private Sub AddNewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNewToolStripMenuItem.Click
        If frmNewParty.Visible = False Then frmNewParty.ShowDialog(Me)
    End Sub

    Private Sub AddNewToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNewToolStripMenuItem1.Click
        If frmItemNew.Visible = False Then frmItemNew.ShowDialog(Me)
    End Sub

    Private Sub ModifyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ModifyToolStripMenuItem.Click
        If frmModifyItem.Visible = False Then frmModifyItem.ShowDialog(Me)
    End Sub

    Private Sub NewPurchageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewPurchageToolStripMenuItem.Click
        If frmPurchage.Visible = False Then frmPurchage.ShowDialog(Me)
    End Sub

    Private Sub AddModifyDeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddModifyDeleteToolStripMenuItem.Click
        If frmAddFY.Visible = False Then frmAddFY.ShowDialog(Me)
    End Sub

    Private Sub AddModifyDelTCSToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddModifyDelTCSToolStripMenuItem.Click
        If frmAddTCS.Visible = False Then frmAddTCS.ShowDialog(Me)
    End Sub

    Private Sub ModifyPurchageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ModifyPurchageToolStripMenuItem.Click
        If frmSelectBillNo.Visible = False Then frmSelectBillNo.ShowDialog(Me)
    End Sub

    Private Sub NewSaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewSaleToolStripMenuItem.Click
        If frmSale.Visible = False Then frmSale.ShowDialog(Me)
    End Sub

    Private Sub ReceiptToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReceiptToolStripMenuItem.Click
        If frmReceipt.Visible = False Then frmReceipt.ShowDialog(Me)
    End Sub

    Private Sub PaymentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PaymentToolStripMenuItem.Click
        If frmPayment.Visible = False Then frmPayment.ShowDialog(Me)
    End Sub

    Private Sub AddModifyDelCapitalAmountToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddModifyDelCapitalAmountToolStripMenuItem.Click
        If frmAddCapital.Visible = False Then frmAddCapital.ShowDialog(Me)
    End Sub

    Private Sub CashFlowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CashFlowToolStripMenuItem.Click
        If frmCashFlow.Visible = False Then frmCashFlow.ShowDialog(Me)
    End Sub

    Private Sub AddModifyDeleteToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddModifyDeleteToolStripMenuItem1.Click
        If frmExpHead.Visible = False Then frmExpHead.ShowDialog(Me)
    End Sub

    Private Sub AddNewExpencesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNewExpencesToolStripMenuItem.Click
        If frmExp.Visible = False Then frmExp.ShowDialog(Me)
    End Sub

    Private Sub SummeryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SummeryToolStripMenuItem.Click

        Dim sql As String = ""

        Try

            sql =
"SELECT
    ItemName,
    PRate,
    SRate,
    IFNULL(SUM(Qty),0) AS STOCK
FROM
(
    SELECT
        I.ItemName,
        I.PRate,
        I.SRate,
        IFNULL(SUM(PS.Qty),0) AS Qty
    FROM tabPurchageSlave PS
    LEFT JOIN tabItem I ON I.ID=PS.ItemID
    WHERE PS.CompanyID=@CompanyID
    GROUP BY I.ItemName,I.PRate,I.SRate

    UNION

    SELECT
        I.ItemName,
        I.PRate,
        I.SRate,
        SUM(-S.Qty) AS Qty
    FROM tabSale S
    LEFT JOIN tabItem I ON I.ID=S.ItemID
    WHERE S.CompanyID=@CompanyID
    GROUP BY I.ItemName,I.PRate,I.SRate
) A
GROUP BY ItemName,PRate,SRate
ORDER BY ItemName"

            Dim Params As New Dictionary(Of String, Object)

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

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            frmViewRpt.MdiParent = Me
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception

            LogError("mdiMain",
                 "SummeryToolStripMenuItem_Click",
                 sql,
                 ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub PartyWiseLaserToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PartyWiseLaserToolStripMenuItem.Click
        If frmPrintPartywiseLaser.Visible = False Then frmPrintPartywiseLaser.ShowDialog(Me)
    End Sub

    Private Sub PartyWiseOutstandingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PartyWiseOutstandingToolStripMenuItem.Click

        Dim sql As String = ""

        Try

            sql =
        "SELECT UPPER(a.BName) AS BName,
                SUM(a.crBank) AS crBank,
                SUM(a.crCash) AS crCash,
                SUM(a.Dr) AS Dr,
                SUM(a.Dr-a.crBank-a.crCash) AS RemAmt
         FROM
         (
             SELECT p.BName,
                    SUM(IFNULL(pl.Amtcrbank,0)) AS crBank,
                    SUM(IFNULL(pl.amtcrcash,0)) AS crCash,
                    SUM(IFNULL(pl.amtdr,0)) AS Dr
             FROM tabPartyLaser pl
             LEFT JOIN tabParty p ON p.ID=pl.PartyID
             WHERE pl.CompanyID=@CompanyID
             GROUP BY p.BName

             UNION ALL

             SELECT p.BName,
                    0 AS crBank,
                    0 AS crCash,
                    SUM(IFNULL(c.CapitalAmountBank,0)+IFNULL(c.CapitalAmountCash,0)) AS Dr
             FROM tabCapital c
             LEFT JOIN tabParty p ON p.ID=c.PartyID
             WHERE c.CompanyID=@CompanyID
             GROUP BY p.BName
         ) a
         GROUP BY a.BName
         ORDER BY a.BName"

            Dim Params As New Dictionary(Of String, Object)
            Params.Add("@CompanyID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports

            ds.dtPartyWiseBalance.Merge(dt)

            ' Strongly Typed Crystal Report
            Dim cryRpt As New rptPartywiseBalance

            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("Date1", Get_BSD())
            cryRpt.SetParameterValue("Date2", Today)
            cryRpt.SetParameterValue("Compname", CurrentCompanyName)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYName)

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            frmViewRpt.MdiParent = Me
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception

            LogError("mdiMain",
                 "PartyWiseOutstandingToolStripMenuItem_Click",
                 sql,
                 ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub PrintExpenceAndIncomeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintExpenceAndIncomeToolStripMenuItem.Click
        If frmPrintExpBetween.Visible = False Then frmPrintExpBetween.ShowDialog(Me)
    End Sub

    Private Sub ItemWiseStockToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemWiseStockToolStripMenuItem.Click
        If frmPrintStockI.Visible = False Then frmPrintStockI.ShowDialog(Me)
    End Sub

    Private Sub PurchageReportAllPartyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PurchageReportAllPartyToolStripMenuItem.Click

        Dim sql As String = ""

        Try

            sql =
        "SELECT p.BName,
                pm.Date,
                pm.BillNo,
                pm.Amount,
                pm.TCS,
                pm.TotalAmount
         FROM tabPurchageMaster pm
         LEFT JOIN tabParty p ON p.ID=pm.PartyID
         WHERE pm.CompanyID=@CompanyID
           AND pm.FY=@FY
         ORDER BY pm.Date,p.BName"

            Dim Params As New Dictionary(Of String, Object)
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            ds.dtPurchage.Merge(dt)

            Dim cryRpt As New rptPurchage

            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("Date1", Get_FirstDay())
            cryRpt.SetParameterValue("Date2", Today)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYName)

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            frmViewRpt.MdiParent = Me
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception

            LogError("mdiMain",
                 "PurchageReportAllPartyToolStripMenuItem_Click",
                 sql,
                 ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub SummeryBetweenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SummeryBetweenToolStripMenuItem.Click
        If frmStockBetween.Visible = False Then frmStockBetween.ShowDialog(Me)
    End Sub



    Private Sub DayWiseReceiptToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DayWiseReceiptToolStripMenuItem.Click
        If frmPrintCashReceipt.Visible = False Then frmPrintCashReceipt.ShowDialog(Me)
    End Sub

    Private Sub PrintExpenceBetweenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintExpenceBetweenToolStripMenuItem.Click
        If frmPrintExpence.Visible = False Then frmPrintExpence.ShowDialog(Me)
    End Sub

    Private Sub PrintCashBookToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintCashBookToolStripMenuItem.Click
        If frmPrintCashBook.Visible = False Then frmPrintCashBook.ShowDialog(Me)
    End Sub

    Private Sub AddDelCapitalAmountReceivedByBorrowingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddDelCapitalAmountReceivedByBorrowingToolStripMenuItem.Click
        If frmAddInvestment.Visible = False Then frmAddInvestment.ShowDialog(Me)
    End Sub

    Private Sub PurchageVSSalesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PurchageVSSalesToolStripMenuItem.Click
        Dim sql As String
        Try

            sql =
"SELECT
    p.ItemName,
    p.PurQty,
    IFNULL(s.SaleQty,0) AS SaleQty,
    p.PurQty - IFNULL(s.SaleQty,0) AS Stock
FROM
(
    SELECT i.ItemName,
           ps.ItemID,
           SUM(ps.Qty) AS PurQty
    FROM tabPurchageSlave ps
    INNER JOIN tabItem i ON i.ID = ps.ItemID
    WHERE ps.CompanyID=@CompanyID
      
    GROUP BY i.ItemName, ps.ItemID
) p
LEFT JOIN
(
    SELECT s.ItemID,
           SUM(s.Qty) AS SaleQty
    FROM tabSale s
    WHERE s.CompanyID=@CompanyID
     
    GROUP BY s.ItemID
) s ON p.ItemID=s.ItemID
ORDER BY Stock, PurQty"

            Dim Params As New Dictionary(Of String, Object)
            Params.Add("@CompanyID", CurrentCompanyID)


            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim ds As New Reports
            'For Each c As DataColumn In dt.Columns
            'MessageBox.Show(c.ColumnName & " = " & c.DataType.ToString)
            'Next
            ds.dtPurVsSale.Merge(dt)

            Dim cryRpt As New rptPurVsSale
            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("Compname", CurrentCompanyName)
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
            End With

            frmViewRpt.MdiParent = Me
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception

            LogError("mdiMain",
                 "PurchageVSSalesToolStripMenuItem_Click",
                 Sql,
                 ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub ItemwiseSaleReportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemwiseSaleReportToolStripMenuItem.Click
        Dim sql As String
        Try
            Sql = "SELECT i.ItemName, " &
                            "SUM(s.Qty * s.Rate) AS SaleVal, " &
                            "(SUM(s.Qty * s.Rate) * 100.0 / " &
                            "(SELECT SUM(Qty * Rate) FROM tabSale " &
                            " WHERE CompanyID=@CompanyID AND FY=@FY)) AS SalePercentage " &
                            "FROM tabSale s " &
                            "LEFT JOIN tabItem i ON i.ID = s.ItemID " &
                            "WHERE s.CompanyID=@CompanyID AND s.FY=@FY " &
                            "GROUP BY i.ItemName " &
                            "ORDER BY SaleVal DESC"

            Dim params As New Dictionary(Of String, Object)
            params.Add("@CompanyID", CurrentCompanyID)
            params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, params)

            Dim ds As New Reports
            ds.dtTotalSale.Merge(dt)

            Dim cryRpt As New rptTotalSale
            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("Date1", Get_FirstDay())
            cryRpt.SetParameterValue("Date2", Today)
            cryRpt.SetParameterValue("FY", Get_CurrentFY())

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            frmViewRpt.CrystalReportViewer1.Refresh()

            frmViewRpt.MdiParent = Me
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception
            LogError("mdiMain", "ItemwiseSaleReportToolStripMenuItem_Click", Sql, ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Sub Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pnlBtm.Paint
        Using br As New LinearGradientBrush(ts1.ClientRectangle, Color.RoyalBlue, Color.Blue, LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(br, ts1.ClientRectangle)
        End Using
    End Sub
    Private Sub AddNewToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNewToolStripMenuItem2.Click
        If frmNewUser.Visible = False Then frmNewUser.ShowDialog(Me)
    End Sub
    Private Sub AddNewCompanyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNewCompanyToolStripMenuItem.Click
        If frmNewCompany.Visible = False Then frmNewCompany.ShowDialog(Me)
    End Sub

    Private Sub ChangePasswordToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangePasswordToolStripMenuItem.Click
        If frmChangePassword.Visible = False Then frmChangePassword.ShowDialog(Me)
    End Sub

    Private Sub AddModifyDelCategoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddModifyDelCategoryToolStripMenuItem.Click
        If frmAddCategory.Visible = False Then frmAddCategory.ShowDialog(Me)
    End Sub

    Private Sub AddModifyDelPackingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddModifyDelPackingToolStripMenuItem.Click
        If frmAddPackingh.Visible = False Then frmAddPackingh.ShowDialog(Me)
    End Sub



    Private Sub CurrentMonthSaleWithStockToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurrentMonthSaleWithStockToolStripMenuItem.Click
        If frmPrintMonthwiseSale.Visible = False Then frmPrintMonthwiseSale.ShowDialog(Me)
    End Sub

    Private Sub cmdSale1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSale1.Click
        If frmSale.Visible = False Then frmSale.ShowDialog(Me)
    End Sub

    Private Sub cmdPurchage_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPurchage.Click
        If frmPurchage.Visible = False Then frmPurchage.ShowDialog(Me)
    End Sub

    Private Sub cmdReceipt_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReceipt.Click
        If frmReceipt.Visible = False Then frmReceipt.ShowDialog(Me)
    End Sub


    Private Sub cmdPayment_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPayment.Click
        If frmPayment.Visible = False Then frmPayment.ShowDialog(Me)
    End Sub


    Private Sub cmdExp_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExp.Click
        If frmExp.Visible = False Then frmExp.ShowDialog(Me)
    End Sub

    Private Sub cmdBackup_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBackup.Click
        If BackupDatabase() Then
            MessageBox.Show("Backup Completed Successfully.")
        Else
            MessageBox.Show("Backup Failed")
        End If

    End Sub

    Private Sub ts1_Paint(sender As Object, e As PaintEventArgs) Handles ts1.Paint
        Using br As New LinearGradientBrush(ts1.ClientRectangle, Color.Orange, Color.White, LinearGradientMode.Horizontal)
            e.Graphics.FillRectangle(br, ts1.ClientRectangle)
        End Using
    End Sub

    Private Sub cmdLogOut_Click(sender As Object, e As EventArgs) Handles cmdLogOut.Click
        For Each frm As Form In Me.MdiChildren
            frm.Close()
        Next
        frmLogin.txtUserID.Text = ""
        frmLogin.txtPassword.Text = ""
        frmLogin.ShowDialog(Me)
    End Sub

    Private Sub MenuStrip_Paint(sender As Object, e As PaintEventArgs) Handles MenuStrip.Paint
        Using br As New LinearGradientBrush(MenuStrip.ClientRectangle, Color.DarkBlue, Color.Blue, LinearGradientMode.Horizontal)
            e.Graphics.FillRectangle(br, MenuStrip.ClientRectangle)
        End Using
    End Sub

    Private Sub mdiMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Application.Exit()
    End Sub

    Private Sub AboutUsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutUsToolStripMenuItem.Click
        splash.ShowDialog(Me)
    End Sub

    Private Sub DataMigrateToolStripMenuItem_Click(sender As Object, e As EventArgs)
        frmDataMigration.ShowDialog(Me)
    End Sub
End Class
