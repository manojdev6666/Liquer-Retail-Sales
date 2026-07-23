Imports System.Drawing.Drawing2D
Imports System.Data.SQLite
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Text.RegularExpressions

Public Class frmDashBoard
    Private down As Boolean
    Private Sub frmDashBoard_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        LoadChart()
        lv1.Clear()
        Dim amount As Integer = GetDailySale()
        Dim amountMonth As Integer = GetDailySaleMonth()
        Dim amountPrevMonth As Integer = GetDailySaleLastMonth()
        lblDailysale.Text = "₹" & amount.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblMonthAvg.Text = "₹" & amountMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblAvgLastMonth.Text = "₹" & amountPrevMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblDaysPast.Text = GetDayPast()
        GetTotalSalePurValue()
        Dim totalMonthSale As Integer = GetMonthSale()
        Dim YesterdaySale As Integer = GetYesterdaySale()
        Dim purchageFYAmt As Integer = GetPurchageFY()
        Dim purchageMonth As Integer = GetPurchageMonth()
        Dim lastmonthPurchage As Integer = GetPurchageLastMonth()
        Dim profit As Integer = GetProfitFY()
        Dim profitMonth As Integer = GetProfitMonth()
        Dim profitLastMonth As Integer = GetProfitLastMonth()
        Dim ProfitPer As Decimal = GetProfitPer()
        Dim TotalExp As Integer = GetTotalExp()
        Dim TotalExpMonth As Integer = GetTotalExpMonth()
        Dim totalExpLastMonth As Integer = GetTotalExpLastMonth()
        Dim cashPending As Integer = GetReceiptPending()
        Dim cashPendingMonth As Integer = GetReceiptPendingMonth()
        Dim cashPendingLastMonth As Integer = GetReceiptPendingLastMonth()
        Dim pVal As Integer = GetStockPValue()
        Dim sVal As Integer = GetStockSValue()

        lblTotalSaleMonth.Text = "₹" & totalMonthSale.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblYesterday.Text = "₹" & YesterdaySale.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblPurchageFY.Text = "₹" & purchageFYAmt.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblPurchageMonth.Text = "₹" & purchageMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblLastMPurchage.Text = "₹" & lastmonthPurchage.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblProfit.Text = "₹" & profit.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblProfitMonth.Text = "₹" & profitMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblprofitLastMonth.Text = "₹" & profitLastMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblPer.Text = ProfitPer.ToString("0.00") & "%"
        lblTotalExp.Text = "₹" & TotalExp.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblTotalExpMonth.Text = "₹" & TotalExpMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lbltotalExpLastMonth.Text = "₹" & totalExpLastMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblCashPending.Text = "₹" & cashPending.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblCashPendingMonth.Text = "₹" & cashPendingMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblCashPendingLastMonth.Text = "₹" & cashPendingLastMonth.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblPVal.Text = "₹" & pVal.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        lblSVal.Text = "₹" & sVal.ToString("N", Globalization.CultureInfo.CreateSpecificCulture("en-IN"))
        If amount < YesterdaySale Then
            Dim bmp As New Bitmap(My.Resources.up)
            bmp.MakeTransparent(Color.White)
            picUPDown.Image = bmp
        Else
            Dim bmp As New Bitmap(My.Resources.down)
            bmp.MakeTransparent(Color.White)
            picUPDown.Image = bmp
        End If
        picUPDown.BackColor = GroupBox2.BackColor
        LoadProductsList()
        ' load daily Sale
        LoadDailySalesGraph(20)
        With lv1
            .View = View.Details
            .OwnerDraw = True
            .FullRowSelect = True
            .GridLines = False
            .HeaderStyle = ColumnHeaderStyle.Nonclickable
            .BorderStyle = BorderStyle.None
            .Scrollable = False
            .HideSelection = False
            .BackColor = Color.FromArgb(0, 82, 200)
            .ForeColor = Color.White
            .Columns.Clear()
            .Columns.Add("Category", 120)
            .Columns.Add("Packing", 180)
            .Columns.Add("Qty", 90)
            Dim img As New ImageList
            img.ImageSize = New Size(1, 30)
            .SmallImageList = img
        End With

    End Sub
    Private Sub LoadProductsList()

        Dim sql As String =
"SELECT
    A.Category,
    A.Packing,
    SUM(A.Qty) AS Qty
FROM
(
    SELECT
        PM.Date,
        I.ItemName,
        C.Category,
        P.Packing,
        SUM(PS.Qty) AS Qty
    FROM tabPurchageSlave PS
    LEFT JOIN tabItem I ON I.ID=PS.ItemID
    LEFT JOIN tabCategory C ON C.ID=I.Category
    LEFT JOIN tabPacking P ON P.ID=I.Packing
    LEFT JOIN tabPurchageMaster PM ON PM.ID=PS.MasterID
    WHERE PS.CompanyID=@CompanyID
      AND PM.Date BETWEEN @FromDate AND @ToDate
    GROUP BY PM.Date,I.ItemName,C.Category,P.Packing

    UNION ALL

    SELECT
        S.Date,
        I.ItemName,
        C.Category,
        P.Packing,
        SUM(-S.Qty) AS Qty
    FROM tabSale S
    LEFT JOIN tabItem I ON I.ID=S.ItemID
    LEFT JOIN tabCategory C ON C.ID=I.Category
    LEFT JOIN tabPacking P ON P.ID=I.Packing
    WHERE S.CompanyID=@CompanyID
      AND S.Date BETWEEN @FromDate AND @ToDate
    GROUP BY S.Date,I.ItemName,C.Category,P.Packing
) A
GROUP BY A.Category,A.Packing
ORDER BY A.Category"

        Try

            lv1.Items.Clear()

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FromDate", Get_BSD())
            Params.Add("@ToDate", Today)

            Dim dt As DataTable = GetDataTable(sql, Params)

            For Each dr As DataRow In dt.Rows

                Dim itm As New ListViewItem(dr("Category").ToString)

                itm.SubItems.Add(dr("Packing").ToString)
                itm.SubItems.Add(Format(Val(dr("Qty")), "0"))

                lv1.Items.Add(itm)

            Next

        Catch ex As Exception

            LogError("frmDashBoard", "LoadProductsList", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Public Sub frmDashBoard_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        Dim g As Graphics = e.Graphics
        Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)
        Dim brush As New LinearGradientBrush(rect, Color.Blue, Color.DarkBlue, LinearGradientMode.ForwardDiagonal)
        g.FillRectangle(brush, rect)
    End Sub

    Private Function GetDayPast() As Integer
        Dim iDay As Integer = 0
        iDay = DateDiff(DateInterval.Day, Get_FirstDay, Now)
        Return iDay
    End Function

    Private Function GetDailySale() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(Rate * Qty),0) AS Sold
     FROM tabSale
     WHERE CompanyID=@CompanyID
       AND FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then

                Return Val(dt.Rows(0)("Sold")) / GetDayPast()

            Else

                Return 0

            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetDailySale", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetDailySaleMonth() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(Rate * Qty),0) AS Sold
     FROM tabSale
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim Sold As Decimal = 0

            If dt.Rows.Count > 0 Then
                Sold = Val(dt.Rows(0)("Sold"))
            End If

            If Date.Today.Day > 1 Then
                Return Sold / (Date.Today.Day - 1)
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetDailySaleMonth", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetDailySaleLastMonth() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(Rate * Qty),0) AS Sold
     FROM tabSale
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.AddMonths(-1).Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim Sold As Decimal = 0

            If dt.Rows.Count > 0 Then
                Sold = Val(dt.Rows(0)("Sold"))
            End If

            Dim PrevMonth As Date = Today.AddMonths(-1)
            Dim DaysInPrevMonth As Integer = Date.DaysInMonth(PrevMonth.Year, PrevMonth.Month)

            If DaysInPrevMonth > 0 Then
                Return Sold / DaysInPrevMonth
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetDailySaleLastMonth", sql, ex)
            Return 0

        End Try

    End Function

    Private Sub GetTotalSalePurValue()

        Dim sql As String =
    "SELECT
        IFNULL(SUM(S.Qty * S.Rate),0) - IFNULL(SUM(S.Qty * I.PRate),0) AS ProfitAmt,
        IFNULL(SUM(S.Qty * S.Rate),0) AS SoldAmt,
        IFNULL(SUM(S.Qty * I.PRate),0) AS PurchageAmt,
        CASE
            WHEN IFNULL(SUM(S.Qty * I.PRate),0)=0 THEN 0
            ELSE (IFNULL(SUM(S.Qty * S.Rate),0) * 100.0 /
                  IFNULL(SUM(S.Qty * I.PRate),0)) / 10
        END AS Percentage
     FROM tabSale S
     LEFT JOIN tabItem I ON I.ID=S.ItemID
     WHERE S.CompanyID=@CompanyID
       AND S.FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then

                Dim TotalSale As Decimal = Val(dt.Rows(0)("SoldAmt"))

                lblTotalSale.Text = "₹" &
                TotalSale.ToString("N",
                Globalization.CultureInfo.CreateSpecificCulture("en-IN"))

            Else

                lblTotalSale.Text = "₹0.00"

            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetTotalSalePurValue", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub
    Private Function GetMonthSale() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(SoldAmtCash,0) + IFNULL(SoldAmtBank,0)),0) AS SoldAmtMonth
     FROM tabLaserSale
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("SoldAmtMonth"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetMonthSale", sql, ex)
            Return 0

        End Try

    End Function
    Private Function GetYesterdaySale() As Decimal

        Dim sql As String =
"SELECT IFNULL(SUM(IFNULL(SoldAmtCash,0) + IFNULL(SoldAmtBank,0)),0) AS SoldAmtYesterday
 FROM tabLaserSale
 WHERE CompanyID=@CompanyID
   AND FY=@FY
   AND date(Date)=date(@Date)"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Date", Today.AddDays(-1))

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Convert.ToDecimal(dt.Rows(0)("SoldAmtYesterday"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetYesterdaySale", sql, ex)
            Return 0D

        End Try

    End Function

    Private Function GetPurchageFY() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(TotalAmount,0)),0) AS PurchageAmt
     FROM tabPurchageMaster
     WHERE CompanyID=@CompanyID
       AND FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("PurchageAmt"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetPurchageFY", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetPurchageMonth() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(TotalAmount,0)),0) AS PurchageAmt
     FROM tabPurchageMaster
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("PurchageAmt"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetPurchageMonth", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetPurchageLastMonth() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(TotalAmount,0)),0) AS PurchageAmt
     FROM tabPurchageMaster
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.AddMonths(-1).Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("PurchageAmt"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetPurchageLastMonth", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetProfitFY() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SUM(S.Qty * S.Rate),0) -
        IFNULL(SUM(S.Qty * I.PRate),0) AS ProfitAmt
     FROM tabSale S
     LEFT JOIN tabItem I ON I.ID = S.ItemID
     WHERE S.CompanyID=@CompanyID
       AND S.FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("ProfitAmt"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetProfitFY", sql, ex)
            Return 0

        End Try

    End Function


    Private Function GetProfitMonth() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SUM(S.Qty * S.Rate),0) -
        IFNULL(SUM(S.Qty * I.PRate),0) AS ProfitAmt
     FROM tabSale S
     LEFT JOIN tabItem I ON I.ID = S.ItemID
     WHERE S.CompanyID=@CompanyID
       AND S.FY=@FY
       AND CAST(strftime('%m', S.Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("ProfitAmt"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetProfitMonth", sql, ex)
            Return 0

        End Try

    End Function
    Private Function GetProfitLastMonth() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SUM(S.Qty * S.Rate),0) -
        IFNULL(SUM(S.Qty * I.PRate),0) AS ProfitAmt
     FROM tabSale S
     LEFT JOIN tabItem I ON I.ID = S.ItemID
     WHERE S.CompanyID=@CompanyID
       AND S.FY=@FY
       AND CAST(strftime('%m', S.Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.AddMonths(-1).Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("ProfitAmt"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetProfitLastMonth", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetProfitPer() As Decimal

        Dim sql As String =
    "SELECT
        CASE
            WHEN IFNULL(SUM(S.Qty * I.PRate),0)=0 THEN 0
            ELSE (IFNULL(SUM(S.Qty * S.Rate),0) * 100.0 /
                  IFNULL(SUM(S.Qty * I.PRate),0)) / 10
        END AS Percentage
     FROM tabSale S
     LEFT JOIN tabItem I ON I.ID = S.ItemID
     WHERE S.CompanyID=@CompanyID
       AND S.FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("Percentage"))
            Else
                Return 0
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetProfitPer", sql, ex)
            Return 0

        End Try

    End Function

    Private Function GetTotalExp() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(Cash,0) + IFNULL(Bank,0)),0) AS TotalExp
     FROM tabExpences
     WHERE CompanyID=@CompanyID
       AND FY=@FY"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("TotalExp"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetTotalExp", sql, ex)
            Return 0D

        End Try

    End Function

    Private Function GetTotalExpMonth() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(Cash,0) + IFNULL(Bank,0)),0) AS TotalExp
     FROM tabExpences
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("TotalExp"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetTotalExpMonth", sql, ex)
            Return 0D

        End Try

    End Function

    Private Function GetTotalExpLastMonth() As Decimal

        Dim sql As String =
    "SELECT IFNULL(SUM(IFNULL(Cash,0) + IFNULL(Bank,0)),0) AS TotalExp
     FROM tabExpences
     WHERE CompanyID=@CompanyID
       AND FY=@FY
       AND CAST(strftime('%m', Date) AS INTEGER)=@Month"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.AddMonths(-1).Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("TotalExp"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetTotalExpLastMonth", sql, ex)
            Return 0D

        End Try

    End Function


    Private Function GetReceiptPending() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SoldAmt - RcvAmt,0) AS Pending
     FROM
     (
         SELECT
             IFNULL(SUM(IFNULL(SoldAmtCash,0) + IFNULL(SoldAmtBank,0)),0) AS SoldAmt,
             IFNULL(SUM(IFNULL(CashAmtRcv,0) + IFNULL(BankAmtRcv,0)),0) AS RcvAmt
         FROM tabLaserSale
         WHERE CompanyID=@CompanyID
           AND FY=@FY
     ) A"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("Pending"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetReceiptPending", sql, ex)
            Return 0D

        End Try

    End Function
    Private Function GetReceiptPendingMonth() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SoldAmt - RcvAmt,0) AS Pending
     FROM
     (
         SELECT
             IFNULL(SUM(IFNULL(SoldAmtCash,0) + IFNULL(SoldAmtBank,0)),0) AS SoldAmt,
             IFNULL(SUM(IFNULL(CashAmtRcv,0) + IFNULL(BankAmtRcv,0)),0) AS RcvAmt
         FROM tabLaserSale
         WHERE CompanyID=@CompanyID
           AND FY=@FY
           AND CAST(strftime('%m', Date) AS INTEGER)=@Month
     ) A"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("Pending"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetReceiptPendingMonth", sql, ex)
            Return 0D

        End Try

    End Function

    Private Function GetReceiptPendingLastMonth() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SoldAmt - RcvAmt,0) AS Pending
     FROM
     (
         SELECT
             IFNULL(SUM(IFNULL(SoldAmtCash,0) + IFNULL(SoldAmtBank,0)),0) AS SoldAmt,
             IFNULL(SUM(IFNULL(CashAmtRcv,0) + IFNULL(BankAmtRcv,0)),0) AS RcvAmt
         FROM tabLaserSale
         WHERE CompanyID=@CompanyID
           AND FY=@FY
           AND CAST(strftime('%m', Date) AS INTEGER)=@Month
     ) A"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)
            Params.Add("@Month", Now.AddMonths(-1).Month)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("Pending"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetReceiptPendingLastMonth", sql, ex)
            Return 0D

        End Try

    End Function

    Private Sub LoadChart()

        Try

            Chart1.Series.Clear()
            Chart1.ChartAreas.Clear()
            Chart1.ChartAreas.Add(New ChartArea("Main"))

            Chart1.Legends(0).Enabled = False
            Chart1.ChartAreas(0).AxisX.MajorGrid.Enabled = False
            Chart1.ChartAreas(0).BackColor = Color.Transparent
            Chart1.ChartAreas(0).BorderColor = Color.Transparent
            Chart1.ChartAreas(0).AxisX.LabelStyle.ForeColor = Color.Orange
            Chart1.ChartAreas(0).AxisY.MajorGrid.LineColor = Color.Transparent
            Chart1.ChartAreas(0).AxisY.LabelStyle.Enabled = False
            Chart1.ChartAreas(0).AxisX.LineColor = Color.Transparent
            Chart1.ChartAreas(0).AxisY.LineColor = Color.Transparent
            Chart1.ChartAreas(0).AxisY.MajorTickMark.Enabled = False
            Chart1.ChartAreas(0).AxisX.Interval = 1

            Dim s As New Series("Sales")
            s.ChartType = SeriesChartType.Bar
            s.IsValueShownAsLabel = True
            s("PointWidth") = "0.8"
            s.Color = Color.Orange

            Dim sql As String =
            "SELECT
            CAST(strftime('%m', Date) AS INTEGER) AS SaleMonth,
            IFNULL(SUM(Rate*Qty),0) AS Sold
         FROM tabSale
         WHERE CompanyID=@CompanyID
           AND FY=@FY
         GROUP BY strftime('%m', Date)
         ORDER BY SaleMonth"

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            Dim TotalSale As Decimal = 0

            For Each r As DataRow In dt.Rows
                TotalSale += Val(r("Sold"))
            Next

            For Each r As DataRow In dt.Rows

                Dim MonthNo As Integer = Val(r("SaleMonth"))

                Dim MonthName As String =
                    Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthNo)

                Dim Sold As Decimal = Val(r("Sold"))

                Dim Per As Decimal = 0

                If TotalSale > 0 Then
                    Per = Math.Round((Sold * 100D) / TotalSale, 2)
                End If

                Dim p As Integer = s.Points.AddXY(MonthName, Sold)

                s.Points(p).Label =
                    Format(Sold, "#,##0") & " | " & Per.ToString("0.00") & "%"

            Next

            s.Font = New Font("Arial", 11, FontStyle.Bold)
            s.LabelForeColor = Color.White
            s.SmartLabelStyle.Enabled = False
            s("BarLabelStyle") = "Inside"
            s.ChartArea = "Main"

            Chart1.Series.Add(s)

        Catch ex As Exception

            LogError("frmDashBoard", "LoadChart", "", ex)

        End Try

    End Sub

    Private Function GetStockPValue() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SUM(PRate * Stock),0) AS PVal,
        IFNULL(SUM(SRate * Stock),0) AS SVal
     FROM
     (
        SELECT
            ItemName,
            PRate,
            SRate,
            IFNULL(SUM(Qty),0) AS Stock
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

            UNION ALL

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
     ) B"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("PVal"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetStockPValue", sql, ex)
            Return 0D

        End Try

    End Function

    Private Function GetStockSValue() As Decimal

        Dim sql As String =
    "SELECT
        IFNULL(SUM(PRate * Stock),0) AS PVal,
        IFNULL(SUM(SRate * Stock),0) AS SVal
     FROM
     (
         SELECT
             ItemName,
             PRate,
             SRate,
             IFNULL(SUM(Qty),0) AS Stock
         FROM
         (
             SELECT
                 I.ItemName,
                 I.PRate,
                 I.SRate,
                 IFNULL(SUM(PS.Qty),0) AS Qty
             FROM tabPurchageSlave PS
             LEFT JOIN tabItem I ON I.ID = PS.ItemID
             WHERE PS.CompanyID=@CompanyID
             GROUP BY I.ItemName, I.PRate, I.SRate

             UNION ALL

             SELECT
                 I.ItemName,
                 I.PRate,
                 I.SRate,
                 SUM(-S.Qty) AS Qty
             FROM tabSale S
             LEFT JOIN tabItem I ON I.ID = S.ItemID
             WHERE S.CompanyID=@CompanyID
             GROUP BY I.ItemName, I.PRate, I.SRate
         ) A
         GROUP BY ItemName, PRate, SRate
     ) B"

        Try

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, Params)

            If dt.Rows.Count > 0 Then
                Return Val(dt.Rows(0)("SVal"))
            Else
                Return 0D
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "GetStockSValue", sql, ex)
            Return 0D

        End Try

    End Function

    Private Sub LoadDailySalesGraph(ByVal days As Integer)

        Dim sql As String = ""

        Try

            Chart2.Series.Clear()
            Chart2.ChartAreas.Clear()
            Chart2.ChartAreas.Add(New ChartArea("Main"))

            Chart2.BackColor = Color.Transparent
            Chart2.ChartAreas(0).BackColor = Color.Transparent
            Chart2.BorderlineWidth = 0
            Chart2.BorderlineColor = Color.Transparent
            Chart2.Legends.Clear()
            Chart2.Titles.Clear()

            Chart2.ChartAreas(0).AxisX.LabelStyle.ForeColor = Color.White
            Chart2.ChartAreas(0).AxisX.LabelStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            Chart2.ChartAreas(0).AxisY.LabelStyle.ForeColor = Color.White
            Chart2.ChartAreas(0).AxisY.LabelStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            Chart2.ChartAreas(0).AxisX.LineColor = Color.Gray
            Chart2.ChartAreas(0).AxisY.LineColor = Color.Gray
            Chart2.ChartAreas(0).AxisY.MajorGrid.LineColor = Color.DimGray
            Chart2.ChartAreas(0).AxisY.MajorTickMark.LineColor = Color.Transparent

            Dim s As New Series("Daily Sales")

            Chart2.Titles.Add("Daily Sales")
            Chart2.Titles(0).Font = New Font("Segoe UI", 12, FontStyle.Bold)
            Chart2.Titles(0).ForeColor = Color.White

            s.ChartType = SeriesChartType.Spline
            s.ChartArea = "Main"
            s.BorderWidth = 2
            s.Color = Color.Orange
            s.MarkerStyle = MarkerStyle.Circle
            s.MarkerSize = 3
            s.MarkerColor = Color.Red

            If days = 0 Then

                sql =
"SELECT Date AS SaleDate,
        IFNULL(SoldAmtCash,0)+IFNULL(SoldAmtBank,0) AS Amount
FROM tabLaserSale
WHERE CompanyID=@CompanyID
  AND FY=@FY
ORDER BY Date"

            Else

                sql =
"SELECT Date AS SaleDate,
        IFNULL(SoldAmtCash,0)+IFNULL(SoldAmtBank,0) AS Amount
FROM tabLaserSale
WHERE CompanyID=@CompanyID
  AND FY=@FY
  AND date(Date)>=date(@FromDate)
ORDER BY Date"

            End If

            Dim Params As New Dictionary(Of String, Object)

            Params.Add("@CompanyID", CurrentCompanyID)
            Params.Add("@FY", CurrentFYID)

            If days > 0 Then
                Params.Add("@FromDate", Today.AddDays(-days))
            End If

            Dim dt As DataTable = GetDataTable(sql, Params)

            For Each row As DataRow In dt.Rows

                s.Points.AddXY(
                CDate(row("SaleDate")).ToString("dd-MM-yy"),
                Convert.ToDouble(row("Amount")))

            Next

            s.ToolTip = "Date : #VALX" & vbCrLf &
                    "Sales : #VALY{N0}"

            Chart2.Series.Add(s)

            With Chart2.ChartAreas(0)

                .AxisX.Interval = 1
                .AxisX.MajorGrid.Enabled = False
                .AxisY.MajorGrid.LineColor = Color.LightGray
                .AxisX.LabelStyle.Angle = -45

            End With

            Dim cnt As Integer = Chart2.Series(0).Points.Count

            If cnt <= 10 Then
                Chart2.ChartAreas(0).AxisX.Interval = 1
            Else
                Chart2.ChartAreas(0).AxisX.Interval = Math.Ceiling(cnt / 10.0)
            End If

        Catch ex As Exception

            LogError("frmDashBoard", "LoadDailySalesGraph", sql, ex)
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub Filter_Click(sender As Object,
                         e As EventArgs) _
Handles btn5D.Click,
        btn10D.Click,
        btn20D.Click,
        btn1M.Click,
        btn3M.Click,
        btnAll.Click

        Dim days As Integer = 0

        Select Case DirectCast(sender, Button).Name

            Case "btn5D"
                days = 5

            Case "btn10D"
                days = 10

            Case "btn20D"
                days = 20

            Case "btn1M"
                days = 30

            Case "btn3M"
                days = 90

            Case "btnAll"
                days = 0

        End Select

        LoadDailySalesGraph(days)

    End Sub

    Private Sub lv1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lv1.SelectedIndexChanged

    End Sub

    Private Sub lv1_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) Handles lv1.DrawColumnHeader
        Using br As New SolidBrush(Color.Orange)
            e.Graphics.FillRectangle(br, e.Bounds)
        End Using
        TextRenderer.DrawText(e.Graphics, e.Header.Text, New Font("Segoe UI", 10, FontStyle.Bold), e.Bounds, Color.White, TextFormatFlags.VerticalCenter Or TextFormatFlags.Left)
    End Sub

    Private Sub lv1_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles lv1.DrawItem
        e.DrawDefault = False
    End Sub

    Private Sub lv1_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles lv1.DrawSubItem
        Dim bg As Color = Color.FromArgb(0, 82, 200)
        If e.Item.Selected Then
            bg = Color.FromArgb(0, 110, 220)
        End If
        Using br As New SolidBrush(bg)
            e.Graphics.FillRectangle(br, e.Bounds)
        End Using
        Dim flags As TextFormatFlags = TextFormatFlags.VerticalCenter
        If e.ColumnIndex = 2 Then
            flags = flags Or TextFormatFlags.Right
        Else
            flags = flags Or TextFormatFlags.Left
        End If
        TextRenderer.DrawText(e.Graphics, e.SubItem.Text, New Font("Segoe UI", 10), e.Bounds, Color.White, TextFormatFlags.VerticalCenter Or TextFormatFlags.Left)
    End Sub

    Private Sub PictureBox10_Click(sender As Object, e As EventArgs) Handles PictureBox10.Click
        ' refresh dashboard
        Call frmDashBoard_Load(Nothing, Nothing)
        MsgBox("Data Refreshed")

    End Sub
End Class