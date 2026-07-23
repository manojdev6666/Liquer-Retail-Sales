<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDataMigration
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmdTest = New System.Windows.Forms.Button()
        Me.cmdMaster = New System.Windows.Forms.Button()
        Me.cmdTransaction = New System.Windows.Forms.Button()
        Me.cmdAll = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.qrpSQL = New System.Windows.Forms.GroupBox()
        Me.txtServer = New System.Windows.Forms.TextBox()
        Me.txtDatabase = New System.Windows.Forms.TextBox()
        Me.txtUser = New System.Windows.Forms.TextBox()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.chkWindows = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.qrpSQL.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdTest
        '
        Me.cmdTest.Location = New System.Drawing.Point(379, 27)
        Me.cmdTest.Name = "cmdTest"
        Me.cmdTest.Size = New System.Drawing.Size(190, 41)
        Me.cmdTest.TabIndex = 0
        Me.cmdTest.Text = "Test Connection"
        Me.cmdTest.UseVisualStyleBackColor = True
        '
        'cmdMaster
        '
        Me.cmdMaster.Location = New System.Drawing.Point(379, 74)
        Me.cmdMaster.Name = "cmdMaster"
        Me.cmdMaster.Size = New System.Drawing.Size(190, 41)
        Me.cmdMaster.TabIndex = 1
        Me.cmdMaster.Text = "Migrate Master Tables"
        Me.cmdMaster.UseVisualStyleBackColor = True
        '
        'cmdTransaction
        '
        Me.cmdTransaction.Location = New System.Drawing.Point(379, 121)
        Me.cmdTransaction.Name = "cmdTransaction"
        Me.cmdTransaction.Size = New System.Drawing.Size(190, 41)
        Me.cmdTransaction.TabIndex = 2
        Me.cmdTransaction.Text = "Migrate Transaction Table"
        Me.cmdTransaction.UseVisualStyleBackColor = True
        '
        'cmdAll
        '
        Me.cmdAll.Location = New System.Drawing.Point(379, 168)
        Me.cmdAll.Name = "cmdAll"
        Me.cmdAll.Size = New System.Drawing.Size(190, 41)
        Me.cmdAll.TabIndex = 3
        Me.cmdAll.Text = "Migrate All"
        Me.cmdAll.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(10, 345)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(559, 35)
        Me.ProgressBar1.TabIndex = 4
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(7, 383)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(10, 13)
        Me.lblStatus.TabIndex = 5
        Me.lblStatus.Text = "-"
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(18, 179)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.Size = New System.Drawing.Size(324, 126)
        Me.txtLog.TabIndex = 6
        '
        'qrpSQL
        '
        Me.qrpSQL.Controls.Add(Me.Label4)
        Me.qrpSQL.Controls.Add(Me.Label3)
        Me.qrpSQL.Controls.Add(Me.Label2)
        Me.qrpSQL.Controls.Add(Me.Label1)
        Me.qrpSQL.Controls.Add(Me.chkWindows)
        Me.qrpSQL.Controls.Add(Me.txtPassword)
        Me.qrpSQL.Controls.Add(Me.txtUser)
        Me.qrpSQL.Controls.Add(Me.txtDatabase)
        Me.qrpSQL.Controls.Add(Me.txtServer)
        Me.qrpSQL.Controls.Add(Me.txtLog)
        Me.qrpSQL.Controls.Add(Me.lblStatus)
        Me.qrpSQL.Controls.Add(Me.ProgressBar1)
        Me.qrpSQL.Controls.Add(Me.cmdAll)
        Me.qrpSQL.Controls.Add(Me.cmdTransaction)
        Me.qrpSQL.Controls.Add(Me.cmdMaster)
        Me.qrpSQL.Controls.Add(Me.cmdTest)
        Me.qrpSQL.Location = New System.Drawing.Point(12, 12)
        Me.qrpSQL.Name = "qrpSQL"
        Me.qrpSQL.Size = New System.Drawing.Size(600, 410)
        Me.qrpSQL.TabIndex = 7
        Me.qrpSQL.TabStop = False
        '
        'txtServer
        '
        Me.txtServer.Location = New System.Drawing.Point(81, 25)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(225, 20)
        Me.txtServer.TabIndex = 7
        '
        'txtDatabase
        '
        Me.txtDatabase.Location = New System.Drawing.Point(81, 51)
        Me.txtDatabase.Name = "txtDatabase"
        Me.txtDatabase.Size = New System.Drawing.Size(225, 20)
        Me.txtDatabase.TabIndex = 8
        '
        'txtUser
        '
        Me.txtUser.Location = New System.Drawing.Point(81, 77)
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(225, 20)
        Me.txtUser.TabIndex = 9
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(81, 103)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(225, 20)
        Me.txtPassword.TabIndex = 10
        '
        'chkWindows
        '
        Me.chkWindows.AutoSize = True
        Me.chkWindows.Location = New System.Drawing.Point(18, 145)
        Me.chkWindows.Name = "chkWindows"
        Me.chkWindows.Size = New System.Drawing.Size(70, 17)
        Me.chkWindows.TabIndex = 11
        Me.chkWindows.Text = "Windows"
        Me.chkWindows.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(15, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Server"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(16, 58)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 13)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Database"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(16, 84)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "User"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(18, 110)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 13)
        Me.Label4.TabIndex = 15
        Me.Label4.Text = "Password"
        '
        'frmDataMigration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(615, 519)
        Me.Controls.Add(Me.qrpSQL)
        Me.Name = "frmDataMigration"
        Me.Text = "frmDataMigration"
        Me.qrpSQL.ResumeLayout(False)
        Me.qrpSQL.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cmdTest As Button
    Friend WithEvents cmdMaster As Button
    Friend WithEvents cmdTransaction As Button
    Friend WithEvents cmdAll As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lblStatus As Label
    Friend WithEvents txtLog As TextBox
    Friend WithEvents qrpSQL As GroupBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents chkWindows As CheckBox
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents txtUser As TextBox
    Friend WithEvents txtDatabase As TextBox
    Friend WithEvents txtServer As TextBox
End Class
