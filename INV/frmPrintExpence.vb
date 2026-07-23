Imports CrystalDecisions.CrystalReports.Engine

Public Class frmPrintExpence

    Private Sub frmPrintExpence_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        dtpDate2.MinDate = Get_FirstDay()
        dtpDate2.MaxDate = Now
        dtpDate.Value = Get_FirstDay()
        dtpDate2.Value = Today
        fill_ExpID()
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
        Dim sql As String
        Try

            Dim params As New Dictionary(Of String, Object)

            If chkAll.Checked Then

                sql = "SELECT e.Date,eh.ExpHead,e.Ref," &
                      "CAST (IFNULL(e.Cash,0) as REAL) AS Cash," &
                      "cast (IFNULL(e.Bank,0) as Real) AS Bank," &
                      "cast(IFNULL(e.Cash,0)+IFNULL(e.Bank,0) as real) AS Total " &
                      "FROM tabExpences e " &
                      "LEFT JOIN tabExpHead eh ON eh.ID=e.ExpHeadID " &
                      "WHERE e.CompanyID=@CompanyID " &
                      "AND e.FY=@FY " &
                      "AND e.Date BETWEEN @Date1 AND @Date2 " &
                      "ORDER BY e.Date,e.ID"

                params.Add("@CompanyID", CurrentCompanyID)
                params.Add("@FY", CurrentFYID)
                params.Add("@Date1", dtpDate.Value.Date)
                params.Add("@Date2", dtpDate2.Value.Date)

            Else

                sql = "SELECT e.Date,eh.ExpHead,e.Ref," &
                      "CAST (IFNULL(e.Cash,0) as REAL) AS Cash," &
                      "cast (IFNULL(e.Bank,0) as Real) AS Bank," &
                      "cast(IFNULL(e.Cash,0)+IFNULL(e.Bank,0) as real) AS Total " &
                      "FROM tabExpences e " &
                      "LEFT JOIN tabExpHead eh ON eh.ID=e.ExpHeadID " &
                      "WHERE e.CompanyID=@CompanyID " &
                      "AND e.FY=@FY " &
                      "AND e.ExpHeadID=@ExpHeadID " &
                      "AND e.Date BETWEEN @Date1 AND @Date2 " &
                      "ORDER BY e.Date,e.ID"

                params.Add("@CompanyID", CurrentCompanyID)
                params.Add("@FY", CurrentFYID)
                params.Add("@ExpHeadID", cboHead.SelectedValue)
                params.Add("@Date1", dtpDate.Value.Date)
                params.Add("@Date2", dtpDate2.Value.Date)

            End If

            Dim dt As DataTable = GetDataTable(sql, params)

            Dim ds As New Reports
            'For Each c As DataColumn In dt.Columns
            'MessageBox.Show(c.ColumnName & " = " & c.DataType.ToString)
            'Next
            ds.dtExpence.Merge(dt)

            Dim cryRpt As New rptExpence
            cryRpt.SetDataSource(ds)

            cryRpt.SetParameterValue(0, dtpDate.Value.ToShortDateString)
            cryRpt.SetParameterValue(1, dtpDate2.Value.ToShortDateString)
            cryRpt.SetParameterValue("CompName", CurrentCompanyName)
            cryRpt.SetParameterValue("CompanyAddress", CurrentCompanyAddress)
            cryRpt.SetParameterValue("ContactNo", CurrentCompanyContact)
            cryRpt.SetParameterValue("RegistrationNo", CurrentCompanyRegistrationNo)
            cryRpt.SetParameterValue("FY", CurrentFYID)

            frmViewRpt.CrystalReportViewer1.ReportSource = cryRpt
            frmViewRpt.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            frmViewRpt.CrystalReportViewer1.Refresh()

            Me.Close()

            frmViewRpt.MdiParent = mdiMain
            frmViewRpt.WindowState = FormWindowState.Maximized
            frmViewRpt.Show()

        Catch ex As Exception
            LogError("frmPrintExpence", "cmdPrint_Click", Sql, ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub chkAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAll.CheckedChanged
        cboHead.Enabled = Not chkAll.Checked
    End Sub

    Private Sub fill_ExpID()

        Try

            Dim sql As String = "SELECT ID,ExpHead FROM tabExpHead WHERE CompanyID=@CompanyID ORDER BY ExpHead"

            Dim params As New Dictionary(Of String, Object)
            params.Add("@CompanyID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, params)

            cboHead.DataSource = dt
            cboHead.DisplayMember = "ExpHead"
            cboHead.ValueMember = "ID"

        Catch ex As Exception
            LogError("frmPrintExpence", "fill_ExpID", "", ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub

End Class