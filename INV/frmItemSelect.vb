Imports System.ComponentModel
Imports System.Data.SQLite

Public Class frmItemSelect
    Public Enum SearchMode
        Sale
        Purchase
    End Enum
    Public Property Mode As SearchMode
    Public Property SearchDate As Date

    Private Sub frmItemSelect_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With dgvItem
            .RowHeadersVisible = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(222, 255, 222)
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Orange
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Regular)
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .ColumnHeadersHeight = 30
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .BackgroundColor = frmSale.Panel1.BackColor
            .MultiSelect = False
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = .ColumnHeadersDefaultCellStyle.BackColor
        End With
    End Sub

    Public Sub LoadItems(SearchText As String)

        Try

            Dim dt As New DataTable

            Using da As New SQLiteDataAdapter(GetQuery(), mySQLiteConn)

                da.SelectCommand.Parameters.AddWithValue("@Date", SearchDate.Date)
                da.SelectCommand.Parameters.AddWithValue("@Search", "%" & SearchText.Trim & "%")
                da.SelectCommand.Parameters.AddWithValue("@CompanyID", CurrentCompanyID)
                'da.SelectCommand.Parameters.AddWithValue("@FY", CurrentFYID)

                da.Fill(dt)

            End Using

            dgvItem.DataSource = dt

            dgvItem.Columns("ID").Visible = False
            dgvItem.Columns("CID").Visible = False

            dgvItem.Columns("ItemName").Width = 300
            dgvItem.Columns("Rate").Width = 70
            dgvItem.Columns("Rate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            dgvItem.Columns("Rate").DefaultCellStyle.Format = "0.00"

            If Mode = SearchMode.Sale Then

                If dgvItem.Columns.Contains("Stock") Then
                    dgvItem.Columns("Stock").Width = 70
                    dgvItem.Columns("Stock").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                End If

            Else

                If dgvItem.Columns.Contains("Stock") Then
                    dgvItem.Columns("Stock").Visible = False
                End If

            End If

            Dim h As Integer = dgvItem.ColumnHeadersHeight +
                           dgvItem.RowCount * dgvItem.RowTemplate.Height + 10

            If h < 80 Then h = 80
            If h > 300 Then h = 300

            Me.Height = h

        Catch ex As Exception

            LogError(Me.Name, "LoadItems", GetQuery(), ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub dgvItem_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvItem.CellContentClick
        ReturnItem()
    End Sub

    Private Sub dgvItem_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvItem.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Hide()
        End If
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            ReturnItem()
        End If
    End Sub

    Private Sub frmItemSelect_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then Me.Hide()
    End Sub

    Private Sub frmItemSelect_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' frmSale.KeyPreview = True
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub dgvItem_SelectionChanged(sender As Object, e As EventArgs) Handles dgvItem.SelectionChanged

        For Each r As DataGridViewRow In dgvItem.Rows
            r.DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Regular)
        Next
        For Each r As DataGridViewRow In dgvItem.SelectedRows
            r.DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        Next

    End Sub

    Private Sub dgvItem_DoubleClick(sender As Object, e As EventArgs) Handles dgvItem.DoubleClick
        ReturnItem()
    End Sub

    Private Sub ReturnItem()
        If Owner Is Nothing Then Exit Sub
        Select Case Mode
            Case SearchMode.Sale
                CType(Owner, frmSale).GetItemFromSearch()
            Case SearchMode.Purchase
                CType(Owner, frmPurchage).GetItemFromSearch()
        End Select
    End Sub

    Private Function GetQuery() As String

        Select Case Mode

            Case SearchMode.Sale

                Return "SELECT b.ID,
                           b.ItemName,
                           b.Rate,
                           b.Stock,
                           b.Category,
                           b.Packing,
                           b.CID
                    FROM
                    (
                        SELECT a.ID,
                               a.ItemName,
                               a.Rate,
                               SUM(a.Stock) AS Stock,
                               a.Category,
                               a.Packing,
                               a.CID
                        FROM
                        (
                            SELECT i.ID,
                                   i.ItemName,
                                   i.SRate AS Rate,
                                   SUM(ps.Qty) AS Stock,
                                   c.Category,
                                   p.Packing,
                                   i.Category AS CID
                            FROM tabPurchageSlave ps
                            INNER JOIN tabPurchageMaster pm
                                   ON pm.ID = ps.MasterID
                            INNER JOIN tabItem i
                                   ON i.ID = ps.ItemID
                            LEFT JOIN tabCategory c
                                   ON c.ID = i.Category
                            LEFT JOIN tabPacking p
                                   ON p.ID = i.Packing
                            WHERE pm.Date <= @Date
                              AND pm.CompanyID = @CompanyID
                              
                            GROUP BY i.ID,
                                     i.ItemName,
                                     i.SRate,
                                     c.Category,
                                     p.Packing,
                                     i.Category

                            UNION ALL

                            SELECT i.ID,
                                   i.ItemName,
                                   i.SRate AS Rate,
                                   SUM(-s.Qty) AS Stock,
                                   c.Category,
                                   p.Packing,
                                   i.Category AS CID
                            FROM tabSale s
                            INNER JOIN tabItem i
                                   ON i.ID = s.ItemID
                            LEFT JOIN tabCategory c
                                   ON c.ID = i.Category
                            LEFT JOIN tabPacking p
                                   ON p.ID = i.Packing
                            WHERE s.Date < @Date
                              AND s.CompanyID = @CompanyID
                              
                            GROUP BY i.ID,
                                     i.ItemName,
                                     i.SRate,
                                     c.Category,
                                     p.Packing,
                                     i.Category

                        ) a

                        GROUP BY a.ID,
                                 a.ItemName,
                                 a.Rate,
                                 a.Category,
                                 a.Packing,
                                 a.CID

                    ) b

                    WHERE b.Stock > 0
                      AND (b.ItemName LIKE @Search
                           OR CAST(b.Rate AS TEXT) LIKE @Search)

                    ORDER BY b.CID,
                             b.ItemName"

            Case Else

                Return "SELECT i.ID,
                           i.ItemName,
                           i.PRate AS Rate,
                           i.Category AS CID,
                           c.Category,
                           p.Packing,
                           i.QtyInCase
                    FROM tabItem i
                    LEFT JOIN tabCategory c
                           ON c.ID = i.Category
                    LEFT JOIN tabPacking p
                           ON p.ID = i.Packing
                    WHERE i.ItemName LIKE @Search
                       OR CAST(i.PRate AS TEXT) LIKE @Search
                    ORDER BY c.Category,
                             i.ItemName"

        End Select

    End Function
End Class