Imports System.Data.SqlClient

Public Class frmAddInvestment

    Private Sub frmAddInvestment_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dtpDate.Value = Now.ToLongDateString
        fill_PName()
        fill_Grid()
        dtpDate.MinDate = Get_FirstDay()
        dtpDate.MaxDate = Now
        With dg1
            .RowHeadersVisible = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(222, 255, 222)
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Orange
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 12, FontStyle.Bold)
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

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub
    Private Sub fill_PName()

        Dim sql As String =
        "SELECT ID, BName
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

            LogError("frmAddInvestment", "fill_PName", sql, ex)

        End Try

    End Sub
    Private Sub fill_Grid()

        Dim sql As String =
    "SELECT
        C.ID,
        P.BName,
        IFNULL(C.CapitalAmountCash,0) AS Cash,
        IFNULL(C.CapitalAmountBank,0) AS Bank,
        C.Ref AS Reference,
        C.Date
     FROM tabCapital C
     LEFT JOIN tabParty P ON P.ID=C.PartyID
     WHERE P.BName IS NOT NULL
       AND C.CompanyID=@CompanyID
       AND C.FY=@FY
     ORDER BY C.Date DESC, C.ID DESC"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            dg1.DataSource = dt

            If dg1.Columns.Count > 0 Then

                dg1.Columns(0).Visible = False
                dg1.Columns(1).Width = 175
                dg1.Columns(2).Width = 75
                dg1.Columns(3).Width = 75
                dg1.Columns(4).Width = 300
                dg1.Columns(5).Width = 75

                dg1.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dg1.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                dg1.Columns(2).DefaultCellStyle.Format = "0.00"
                dg1.Columns(3).DefaultCellStyle.Format = "0.00"

            End If

        Catch ex As Exception

            LogError("frmAddInvestment", "fill_Grid", sql, ex)

        End Try

    End Sub

    Private Sub cboParty_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParty.GotFocus
        cboParty.FlatStyle = FlatStyle.Flat
    End Sub

    Private Sub cboParty_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParty.LostFocus
        cboParty.FlatStyle = FlatStyle.Standard
    End Sub

    Private Sub cboParty_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboParty.SelectedIndexChanged
        fill_Grid()
    End Sub

    Private Sub txtcash_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtcash.GotFocus
        txtcash.SelectAll()
    End Sub

    Private Sub txtBank_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBank.GotFocus
        txtBank.SelectAll()
    End Sub

    

    Private Sub txtcash_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtcash.LostFocus
        txtcash.Text = Format(Val(txtcash.Text), "0.00")
    End Sub

    Private Sub txtBank_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBank.LostFocus
        txtBank.Text = Format(Val(txtBank.Text), "0.00")
    End Sub


    Private Sub dg1_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles dg1.KeyUp

        If e.KeyCode <> Keys.Delete Then Exit Sub

        If dg1.SelectedRows.Count = 0 Then Exit Sub

        If MsgBox("This will delete the selected record. Are you sure?",
              MsgBoxStyle.YesNo Or MsgBoxStyle.Question,
              "Confirm Delete") = MsgBoxResult.No Then Exit Sub

        Dim sql As String =
        "DELETE FROM tabCapital
         WHERE ID=@ID
           AND CompanyID=@CompanyID
           AND FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@ID", dg1.SelectedRows(0).Cells(0).Value)
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            ExecuteNonQuery(sql, Params)

            fill_Grid()

            txtBank.Focus()

            MsgBox("Entry Deleted!", MsgBoxStyle.Information)

        Catch ex As Exception

            LogError("frmAddInvestment", "dg1_KeyUp", sql, ex)
            MsgBox("Unable to delete record.", MsgBoxStyle.Critical)

        End Try

    End Sub


    Private Sub InsertCapital()

        Dim sql As String =
    "INSERT INTO tabCapital
    (
        CapitalAmountCash,
        CapitalAmountBank,
        Ref,
        Date,
        PartyID,
        FY,
        CompanyID,
        UserID
    )
    VALUES
    (
        @Cash,
        @Bank,
        @Ref,
        @Date,
        @PartyID,
        @FY,
        @CompanyID,
        @UserID
    )"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@Cash", Val(txtcash.Text))
            Params.Add("@Bank", Val(txtBank.Text))
            Params.Add("@Ref", "Borrow")
            Params.Add("@Date", dtpDate.Value.ToString("yyyy-MM-dd"))
            Params.Add("@PartyID", CInt(cboParty.SelectedValue))
            Params.Add("@FY", CurrentFYID)
            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@UserID", CurrentUserID)

            ExecuteNonQuery(sql, Params)

            txtcash.Text = "0.00"
            txtBank.Text = "0.00"

            MsgBox("Capital Amount Saved", MsgBoxStyle.Information)

        Catch ex As Exception

            LogError("frmAddInvestment", "InsertCapital", sql, ex)
            MsgBox("Unable to save record.", MsgBoxStyle.Critical)

        End Try

    End Sub

    Private Sub cmdSubmit_Click(ByVal sender As System.Object,
                            ByVal e As System.EventArgs) _
                            Handles cmdSubmit.Click

        Try

            If Val(txtcash.Text) + Val(txtBank.Text) <= 0 Then

                MsgBox("Enter Cash or Bank Amount.",
                   MsgBoxStyle.Critical,
                   "ERROR")

                txtcash.Focus()
                Exit Sub

            End If

            If cboParty.SelectedIndex = -1 Then

                MsgBox("Please select a party.",
                   MsgBoxStyle.Critical,
                   "ERROR")

                cboParty.Focus()
                Exit Sub

            End If

            InsertCapital()

            fill_Grid()

            cboParty.Focus()

        Catch ex As Exception

            LogError("frmAddInvestment", "cmdSubmit_Click", "", ex)
            MsgBox("Unexpected error occurred.", MsgBoxStyle.Critical)

        End Try

    End Sub


End Class