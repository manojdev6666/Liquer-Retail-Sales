Imports System.Data.SQLite

Public Class frmItemNew

    Private Sub frmItemNew_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtItemName.Focus()
        fill_Category()
        fill_Packing()
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(48, 48))
    End Sub

    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If


    End Sub

    Private Sub txtPurRate_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPurRate.GotFocus
        txtPurRate.SelectAll()
    End Sub

    Private Sub fill_Category()

        Try
            Dim sql As String = "SELECT ID, Category FROM tabCategory"

            Dim dt As DataTable = GetDataTable(sql)

            cboCategory.DataSource = dt
            cboCategory.DisplayMember = "Category"
            cboCategory.ValueMember = "ID"

        Catch ex As Exception
            LogError(Me.Name, "fill_Category", "", ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub fill_Packing()

        Try
            Dim sql As String = "SELECT ID, Packing FROM tabPacking"

            Dim dt As DataTable = GetDataTable(sql)

            cboPacking.DataSource = dt
            cboPacking.DisplayMember = "Packing"
            cboPacking.ValueMember = "ID"

        Catch ex As Exception
            LogError(Me.Name, "fill_Packing", "", ex)
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub txtSaleRate_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSaleRate.GotFocus
        txtSaleRate.SelectAll()
    End Sub


    Private Sub txtPurRate_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPurRate.LostFocus
        txtPurRate.Text = Format(Val(txtPurRate.Text), "#0.00")
    End Sub

    Private Sub txtSaleRate_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSaleRate.LostFocus
        txtSaleRate.Text = Format(Val(txtSaleRate.Text), "#0.00")
    End Sub


    Private Sub Insert_Data()

        Dim sql As String = ""

        Try

            sql =
        "INSERT INTO tabItem
        (ItemName,
        PRate,
        SRate,
        Category,
        Packing,
        QtyInCase)
        VALUES
        (@ItemName,
        @PRate,
        @SRate,
        @Category,
        @Packing,
        @QtyInCase)"

            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@ItemName", txtItemName.Text.Trim)
            prm.Add("@PRate", Val(txtPurRate.Text))
            prm.Add("@SRate", Val(txtSaleRate.Text))
            prm.Add("@Category", cboCategory.SelectedValue)
            prm.Add("@Packing", cboPacking.SelectedValue)
            prm.Add("@QtyInCase", Val(txtIQtyInCase.Text))

            ExecuteNonQuery(sql, prm)

        Catch ex As Exception

            LogError(Me.Name, "Insert_Data", sql, ex)
            Throw

        End Try

    End Sub


    Private Sub cmdSubmit_Click(sender As Object, e As EventArgs) Handles cmdSubmit.Click

        If txtItemName.Text.Trim = "" Then
            MsgBox("Enter Item Name")
            txtItemName.Focus()
            Exit Sub
        End If


        If Chq_duplicate() Then
            MsgBox("Item already Exists", MsgBoxStyle.Critical)
            txtItemName.Focus()
            Exit Sub
        End If


        Insert_Data()

        MsgBox("Item Created Successfully", MsgBoxStyle.Information)

        ClearField()

        txtItemName.Focus()

    End Sub
    Private Sub ClearField()
        txtItemName.Clear()
        txtPurRate.Text = "0.00"
        txtSaleRate.Text = "0.00"
    End Sub

    Private Function Chq_duplicate() As Boolean

        Try

            Dim sql As String =
        "SELECT ID FROM tabItem WHERE ItemName=@ItemName"

            Dim prm As New Dictionary(Of String, Object)
            prm.Add("@ItemName", txtItemName.Text.Trim)

            Dim obj = ExecuteScalar(sql, prm)

            Return obj IsNot Nothing

        Catch ex As Exception

            LogError(Me.Name, "Chq_duplicate", "", ex)
            Throw

        End Try

    End Function
End Class