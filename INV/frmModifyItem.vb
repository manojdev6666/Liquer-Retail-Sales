Imports System.Data.SQLite

Public Class frmModifyItem

    Private Sub frmModifyItem_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cboItemName.Focus()
        fill_IName()
        fill_Category()
        fill_Packing()
        fill_Data()
        cmdSubmit.Image = New Bitmap(cmdSubmit.Image, New Size(64, 64))
    End Sub
    Private Sub myEventHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
        End If
    End Sub
    Private Sub fill_IName()

        Try
            Dim sql As String = "SELECT ID, ItemName FROM tabItem ORDER BY ItemName"

            Dim dt As DataTable = GetDataTable(sql)

            cboItemName.DataSource = dt
            cboItemName.DisplayMember = "ItemName"
            cboItemName.ValueMember = "ID"

        Catch ex As Exception
            LogError(Me.Name, "fill_IName", "", ex)
        End Try

    End Sub
    Private Sub fill_Data()

        Try

            If cboItemName.SelectedValue Is Nothing Then Exit Sub

            Dim sql As String =
        "SELECT i.ItemName,
        i.PRate,
        i.SRate,
        IFNULL(c.Category,'A') Category,
        IFNULL(p.Packing,'A') Packing,
        IFNULL(i.QtyInCase,0) QtyInCase
        FROM tabItem i
        LEFT JOIN tabCategory c ON c.ID=i.Category
        LEFT JOIN tabPacking p ON p.ID=i.Packing
        WHERE i.ID=@ID"


            Dim prm As New Dictionary(Of String, Object)
            prm.Add("@ID", cboItemName.SelectedValue)


            Dim dt As DataTable = GetDataTable(sql, prm)


            If dt.Rows.Count > 0 Then

                Dim r = dt.Rows(0)

                txtItemName.Text = r("ItemName").ToString
                txtPurRate.Text = r("PRate").ToString
                txtSaleRate.Text = r("SRate").ToString

                cboCategory.Text = r("Category").ToString
                cboPacking.Text = r("Packing").ToString

                txtQtyCase.Text = r("QtyInCase").ToString

            End If


        Catch ex As Exception

            LogError(Me.Name, "fill_Data", "", ex)

        End Try

    End Sub

    Private Sub cboItemName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboItemName.SelectedIndexChanged
        fill_Data()

    End Sub

    Private Sub UPDATE_DATA()

        Dim sql As String = ""

        Try

            sql =
        "UPDATE tabItem SET
        ItemName=@ItemName,
        PRate=@PRate,
        SRate=@SRate,
        Category=@Category,
        Packing=@Packing,
        QtyInCase=@QtyInCase
        WHERE ID=@ID"


            Dim prm As New Dictionary(Of String, Object)

            prm.Add("@ItemName", txtItemName.Text.Trim)
            prm.Add("@PRate", Val(txtPurRate.Text))
            prm.Add("@SRate", Val(txtSaleRate.Text))
            prm.Add("@Category", cboCategory.SelectedValue)
            prm.Add("@Packing", cboPacking.SelectedValue)
            prm.Add("@QtyInCase", Val(txtQtyCase.Text))
            prm.Add("@ID", cboItemName.SelectedValue)


            ExecuteNonQuery(sql, prm)

            cboItemName.Focus()


        Catch ex As Exception

            LogError(Me.Name, "UPDATE_DATA", sql, ex)
            Throw

        End Try

    End Sub

    Private Sub cmdSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
        UPDATE_DATA()
        MsgBox("Item Updated!", MsgBoxStyle.Information, "Success")
    End Sub



    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        Delete_Data()
        fill_Data()
    End Sub

    Private Sub Delete_Data()

        If MsgBox("Do you really Want to Delete???",
              MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then


            Dim sql As String =
        "DELETE FROM tabItem WHERE ID=@ID"


            Dim prm As New Dictionary(Of String, Object)
            prm.Add("@ID", cboItemName.SelectedValue)


            ExecuteNonQuery(sql, prm)


            MsgBox("Record Deleted",
               MsgBoxStyle.Information)

            fill_IName()

        End If

    End Sub

    Private Sub txtPurRate_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPurRate.GotFocus
        txtPurRate.SelectAll()
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

    Private Sub fill_Category()

        Try

            Dim sql As String =
        "SELECT ID, Category 
         FROM tabCategory 
         ORDER BY ID"

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

            Dim sql As String =
            "SELECT ID, Packing 
         FROM tabPacking 
         ORDER BY ID"

            Dim dt As DataTable = GetDataTable(sql)

            cboPacking.DataSource = dt
            cboPacking.DisplayMember = "Packing"
            cboPacking.ValueMember = "ID"

        Catch ex As Exception

            LogError(Me.Name, "fill_Packing", "", ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

End Class