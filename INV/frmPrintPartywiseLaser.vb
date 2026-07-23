Imports System.Data.SQLite
Imports CrystalDecisions.CrystalReports.Engine

Public Class frmPrintPartywiseLaser

    Private Sub frmPrintPartywiseLaser_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        fill_BName()
    End Sub
    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub fill_BName()

        Dim sql As String =
    "SELECT ID,BName
     FROM tabParty
     WHERE CompanyID=@CompanyID
     ORDER BY BName"

        Try

            Dim Params As New Dictionary(Of String, Object)
            Params.Add("@CompanyID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            cboParty.DataSource = dt
            cboParty.DisplayMember = "BName"
            cboParty.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_BName", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub


    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click

        Dim sql As String = ""

        Try

            sql =
    "SELECT
    'OB' AS BName,
    '' AS Address,
    @FirstDay AS Date,
    0.0 AS CrBank,
    0.0 AS CrCash,
    0.0 AS Dr,
    SUM(OB) AS OpeningBal,
    'Opening Balance' AS Ref
FROM
(
    SELECT
        IFNULL(SUM(AmtDr),0)
        -IFNULL(SUM(AmtCrBank),0)
        -IFNULL(SUM(AmtCrCash),0) AS OB
    FROM tabPartyLaser
    WHERE PartyID=@PartyID
      AND CompanyID=@CompanyID
      AND date(Date)<date(@FirstDay)

    UNION ALL

    SELECT
        IFNULL(SUM(CapitalAmountBank),0)
        +IFNULL(SUM(CapitalAmountCash),0)
    FROM tabCapital
    WHERE PartyID=@PartyID
      AND CompanyID=@CompanyID
      AND date(Date)<date(@FirstDay)
)

UNION ALL

SELECT
    p.BName,
    p.Address,
    date(pl.Date) AS Date,
    IFNULL(pl.AmtCrBank,0) AS CrBank,
    IFNULL(pl.AmtCrCash,0) AS CrCash,
    IFNULL(pl.AmtDr,0) AS Dr,
    0 AS OpeningBal,
    pl.Ref
FROM tabPartyLaser pl
LEFT JOIN tabParty p
ON p.ID=pl.PartyID
WHERE pl.PartyID=@PartyID
  AND pl.CompanyID=@CompanyID
  AND pl.FY=@FY

UNION ALL

SELECT
    p.BName,
    p.Address,
    date(c.Date) AS Date,
    0 AS CrBank,
    0 AS CrCash,
    IFNULL(c.CapitalAmountBank,0)
    +IFNULL(c.CapitalAmountCash,0) AS Dr,
    0 AS OpeningBal,
    c.Ref
FROM tabCapital c
LEFT JOIN tabParty p
ON p.ID=c.PartyID
WHERE c.PartyID=@PartyID
  AND c.CompanyID=@CompanyID
  AND c.FY=@FY

ORDER BY date(Date)"
            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@PartyID", CInt(cboParty.SelectedValue))
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@FirstDay", Get_FirstDay().ToString("yyyy-MM-dd"))

            Dim dt As DataTable = GetDataTable(sql, Params)



            Dim ds As New Reports

            ds.dtPartyLedger.Clear()
            ds.dtPartyLedger.Merge(dt)

            Dim cryRpt As New rptPartyLaser

            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("Date1", Get_FirstDay())
            cryRpt.SetParameterValue("Date2", Get_LastDay())
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYName)
            cryRpt.SetParameterValue("PartyName", cboParty.Text)

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView =
                CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            frmViewRpt.CrystalReportViewer1.ShowRefreshButton = False
            frmViewRpt.CrystalReportViewer1.Refresh()

            Me.Close()

            frmViewRpt.MdiParent = mdiMain
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()
        Catch ex As Exception

            LogError(Me.Name,
                     "cmdSubmit_Click",
                     sql,
                     ex)

            MessageBox.Show(ex.Message)

        End Try

    End Sub
End Class