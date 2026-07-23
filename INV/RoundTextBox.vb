Imports System.Drawing.Drawing2D
Imports System.Xml
Public Class RoundTextBox


    Private Sub RoundTextBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Dock = DockStyle.Fill
        TextBox1.BorderStyle = BorderStyle.None
        TextBox1.Location = New Point(8, 8)
        PictureBox1.Size = New Size(20, 20)
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        Panel1.Height = 40
        RoundPanel()
        Panel1.BackColor = Color.White
        TextBox1.BackColor = Color.White
    End Sub


    Private Sub RoundTextBox_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim r As Integer = 20
        Dim path As New GraphicsPath
        path.AddArc(0, 0, r, r, 180, 90)
        path.AddArc(Panel1.Width - r, 0, r, r, 270, 90)
        path.AddArc(Panel1.Width - r, 0, r, r, 270, 90)
        path.AddArc(0, Panel1.Height - r, r, r, 90, 90)
        path.CloseFigure()
        Panel1.Region = New Region(path)
    End Sub

    Private Sub TextBox1_Enter(sender As Object, e As EventArgs) Handles TextBox1.Enter
        If TextBox1.Text = _placeholder Then
            TextBox1.Text = ""
            TextBox1.ForeColor = Color.Black
        End If
    End Sub

    Private Sub TextBox1_Leave(sender As Object, e As EventArgs) Handles TextBox1.Leave
        If TextBox1.Text.Trim = "" Then
            TextBox1.Text = _placeholder
            TextBox1.ForeColor = Color.Gray
        End If
    End Sub

    Private _borderColor As Color = Color.Silver
    Public Property BorderColor As Color
        Get
            Return _borderColor
        End Get
        Set(value As Color)
            _borderColor = value
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Using p As New Pen(_borderColor, 1)
            e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            e.Graphics.DrawPath(p, New Drawing2D.GraphicsPath())
        End Using
    End Sub
    Public Overrides Property Text As String
        Get
            Return TextBox1.Text
        End Get
        Set(value As String)
            TextBox1.Text = value
        End Set
    End Property

    Private _placeholder As String = ""
    Public Property Placeholder As String
        Get
            Return _placeholder
        End Get
        Set(value As String)
            _placeholder = value
            TextBox1.Text = value
            TextBox1.ForeColor = Color.Gray
        End Set
    End Property

    Private Sub RoundPanel()
        Dim r As Integer = 25
        Dim path As New GraphicsPath()
        path.AddArc(0, 0, r * 2, r * 2, 180, 90)
        path.AddArc(Panel1.Width - (r * 2), 0, r * 2, r * 2, 270, 90)
        path.AddArc(Panel1.Width - (r * 2), Panel1.Height - (r * 2), r * 2, r * 2, 0, 90)
        path.AddArc(0, Panel1.Height - (r * 2), r * 2, r * 2, 90, 90)
        path.CloseFigure()
        Panel1.Region = New Region(path)
    End Sub

    Private Sub Panel1_Resize(sender As Object, e As EventArgs) Handles Panel1.Resize
        RoundPanel()
    End Sub
End Class
