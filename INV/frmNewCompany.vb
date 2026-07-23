Imports System.Data.SQLite
Imports System.Diagnostics.Eventing.Reader

Public Class frmNewCompany

    Dim CompanyID As Integer = 0

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub

    Private Sub frmNewCompany_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' dtpDate.MinDate = Get_FirstDay()
        'dtpDate.MaxDate = Now
        dtpDate.Value = Get_FirstDay()
        Fill_Grid()
        With dg1
            .RowHeadersVisible = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(222, 255, 222)
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Orange
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Regular)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .BackgroundColor = Me.Panel1.BackColor
            .MultiSelect = False
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = .ColumnHeadersDefaultCellStyle.BackColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.ForeColor = .ColumnHeadersDefaultCellStyle.ForeColor
            .ColumnHeadersHeight = 32
            .RowTemplate.Height = 32
        End With
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(64, 64))
    End Sub

    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        If Len(Trim(txtCname.Text)) < 1 Then
            MsgBox("Enter Company Name!", MsgBoxStyle.Critical, "ERROR!!")
            txtCname.Focus()
            Exit Sub
        End If

        If cmdSubmit.Text.Trim = "&SUBMIT" Then
            If isExist() Then
                MsgBox("Company already exists!")
                Exit Sub
            End If
            INSERT_Data()
            MsgBox("Company Created Successfully!", MsgBoxStyle.Information, "SUCCESS")
        Else
            UPDATE_Data()
            MsgBox("Company Modified Successfully!", MsgBoxStyle.Information, "SUCCESS")
        End If
        Fill_Grid()
        txtCname.Clear()
        txtCAddress.Clear()
        txtContNo.Clear()
        txtPAN.Clear()
        txtRemark.Clear()
        dtpDate.Focus()
        txtQuotaBear.Clear()
        txtQuotaEng.Clear()
        CompanyID = 0
        cmdSubmit.Text = "SUBMIT"
    End Sub
    Private Sub Fill_Grid()

        Try

            Dim sql As String =
        "SELECT ID,
        BusinessStartDate,
        CompanyName,
        CompanyAddress,
        ContactNumber,
        RegistrationNo1,
        Remark,
        Quota_Eng,
        Quota_Beear
        FROM tabCompany
        ORDER BY CompanyName"

            dg1.DataSource = GetDataTable(sql)

            dg1.Columns("ID").Visible = False

            dg1.Columns("BusinessStartDate").HeaderText = "Start Date"
            dg1.Columns("CompanyName").HeaderText = "Company Name"
            dg1.Columns("CompanyAddress").HeaderText = "Address"
            dg1.Columns("ContactNumber").HeaderText = "Contact No."
            dg1.Columns("RegistrationNo1").HeaderText = "Registration No."
            dg1.Columns("Quota_Eng").HeaderText = "Eng. Quota"
            dg1.Columns("Quota_Beear").HeaderText = "Beer Quota"

            dg1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dg1.MultiSelect = False
            dg1.ReadOnly = True
            dg1.AllowUserToAddRows = False

        Catch ex As Exception

            LogError(Me.Name, "Fill_Grid", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Function isExist() As Boolean

        Try

            Dim sql As String = "SELECT ID FROM tabCompany WHERE CompanyName=@CompanyName"

            Dim prm As New Dictionary(Of String, Object)
            prm.Add("@CompanyName", txtCname.Text.Trim)

            Dim obj = ExecuteScalar(sql, prm)

            Return obj IsNot Nothing

        Catch ex As Exception

            LogError(Me.Name, "isExist", "", ex)
            Throw

        End Try

    End Function

    Private Sub INSERT_Data()
        Dim sql As String
        Try

            sql =
        "INSERT INTO tabCompany
        (BusinessStartDate,
        CompanyName,
        CompanyAddress,
        ContactNumber,
        RegistrationNo1,
        Remark,
        Quota_Eng,
        Quota_Beear)
        VALUES
        (@BusinessStartDate,
        @CompanyName,
        @CompanyAddress,
        @ContactNumber,
        @RegistrationNo1,
        @Remark,
        @Quota_Eng,
        @Quota_Beear)"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@BusinessStartDate", dtpDate.Value.ToString("yyyy-MM-dd"))
            prm.Add("@CompanyName", txtCname.Text.Trim)
            prm.Add("@CompanyAddress", txtCAddress.Text.Trim)
            prm.Add("@ContactNumber", txtContNo.Text.Trim)
            prm.Add("@RegistrationNo1", txtPAN.Text.Trim)
            prm.Add("@Remark", txtRemark.Text.Trim)
            prm.Add("@Quota_Eng", Val(txtQuotaEng.Text))
            prm.Add("@Quota_Beear", Val(txtQuotaBear.Text))

            ExecuteNonQuery(sql, prm)

        Catch ex As Exception

            LogError(Me.Name, "INSERT_Data", sql, ex)
            Throw

        End Try

    End Sub



    Private Sub txtQuotaEng_GotFocus(sender As Object, e As EventArgs) Handles txtQuotaEng.GotFocus
        txtQuotaEng.SelectAll()
    End Sub

    Private Sub txtQuotaBear_GotFocus(sender As Object, e As EventArgs) Handles txtQuotaBear.GotFocus
        txtQuotaBear.SelectAll()
    End Sub

    Private Sub txtQuotaBear_LostFocus(sender As Object, e As EventArgs) Handles txtQuotaBear.LostFocus
        txtQuotaBear.Text = Format(Val(txtQuotaBear.Text), "0.00")
    End Sub

    Private Sub txtQuotaEng_LostFocus(sender As Object, e As EventArgs) Handles txtQuotaEng.LostFocus
        txtQuotaEng.Text = Format(Val(txtQuotaEng.Text), "0.00")
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub dg1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellClick
        If e.RowIndex < 0 Then Exit Sub
        CompanyID = Val(dg1.Rows(e.RowIndex).Cells("ID").Value)
        Dim v As Object = dg1.Rows(e.RowIndex).Cells("BusinessStartDate").Value
        If IsDBNull(v) OrElse v Is Nothing Then
            dtpDate.Value = Today
        Else
            dtpDate.Value = CDate(v)
        End If
        txtCname.Text = dg1.Rows(e.RowIndex).Cells("CompanyName").Value.ToString
        txtCAddress.Text = dg1.Rows(e.RowIndex).Cells("CompanyAddress").Value.ToString
        txtContNo.Text = dg1.Rows(e.RowIndex).Cells("ContactNumber").Value.ToString
        txtPAN.Text = dg1.Rows(e.RowIndex).Cells("RegistrationNo1").Value.ToString
        txtRemark.Text = dg1.Rows(e.RowIndex).Cells("Remark").Value.ToString
        txtQuotaEng.Text = dg1.Rows(e.RowIndex).Cells("Quota_Eng").Value.ToString
        txtQuotaBear.Text = dg1.Rows(e.RowIndex).Cells("Quota_Beear").Value.ToString
        cmdSubmit.Text = "MODIFY"
    End Sub

    Private Sub UPDATE_Data()
        Dim sql As String
        Try

            sql =
        "UPDATE tabCompany SET
        BusinessStartDate=@BusinessStartDate,
        CompanyName=@CompanyName,
        CompanyAddress=@CompanyAddress,
        ContactNumber=@ContactNumber,
        RegistrationNo1=@RegistrationNo1,
        Remark=@Remark,
        Quota_Eng=@Quota_Eng,
        Quota_Beear=@Quota_Beear
        WHERE ID=@ID"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@BusinessStartDate", dtpDate.Value.ToString("yyyy-MM-dd"))
            prm.Add("@CompanyName", txtCname.Text.Trim)
            prm.Add("@CompanyAddress", txtCAddress.Text.Trim)
            prm.Add("@ContactNumber", txtContNo.Text.Trim)
            prm.Add("@RegistrationNo1", txtPAN.Text.Trim)
            prm.Add("@Remark", txtRemark.Text.Trim)
            prm.Add("@Quota_Eng", Val(txtQuotaEng.Text))
            prm.Add("@Quota_Beear", Val(txtQuotaBear.Text))
            prm.Add("@ID", CompanyID)

            ExecuteNonQuery(sql, prm)

            cmdSubmit.Text = "SUBMIT"

        Catch ex As Exception

            LogError(Me.Name, "UPDATE_Data", Sql, ex)
            Throw

        End Try

    End Sub
End Class