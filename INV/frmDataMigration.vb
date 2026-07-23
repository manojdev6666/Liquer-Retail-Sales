Public Class frmDataMigration
    Private Sub cmdTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdTest.Click

        Try

            Dim ConnStr As String = ""

            If chkWindows.Checked = True Then

                ConnStr = "Data Source=" & txtServer.Text.Trim & ";" &
                      "Initial Catalog=" & txtDatabase.Text.Trim & ";" &
                      "Integrated Security=True"

            Else

                ConnStr = "Data Source=" & txtServer.Text.Trim & ";" &
                      "Initial Catalog=" & txtDatabase.Text.Trim & ";" &
                      "User ID=" & txtUser.Text.Trim & ";" &
                      "Password=" & txtPassword.Text.Trim

            End If

            lblStatus.Text = "Connecting..."

            If OpenSQLConnection(ConnStr) Then

                lblStatus.Text = "Connected Successfully"

                txtLog.AppendText("[" & Now.ToString("dd/MM/yyyy HH:mm:ss") & "] SQL Server Connected Successfully." & vbCrLf)

                MsgBox("SQL Server Connected Successfully.", MsgBoxStyle.Information)

                CloseSQLConnection()

            Else

                lblStatus.Text = "Connection Failed"

                txtLog.AppendText("[" & Now.ToString("dd/MM/yyyy HH:mm:ss") & "] Connection Failed." & vbCrLf)

                MsgBox("Connection Failed.", MsgBoxStyle.Critical)

            End If

        Catch ex As Exception

            lblStatus.Text = "Error"

            txtLog.AppendText("[" & Now.ToString("dd/MM/yyyy HH:mm:ss") & "] " & ex.Message & vbCrLf)

            LogError("frmDataMigration",
                 "cmdTest_Click",
                 "",
                 ex)

            MsgBox(ex.Message, MsgBoxStyle.Critical)

        End Try

    End Sub

    Private Sub Migrate_tabCompany()

        Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabCompany")

        ExecuteNonQuery("DELETE FROM tabCompany")

        For Each dr As DataRow In dt.Rows

            Dim sql As String =
            "INSERT INTO tabCompany
        (ID,BusinessStartDate,CompanyName,CompanyAddress,
        ContactNumber,RegistrationNo1,RegistrationNo2,
        RegistrationNo3,Remark,Quota_Eng,Quota_Beear)
        VALUES
        (@ID,@BusinessStartDate,@CompanyName,@CompanyAddress,
        @ContactNumber,@RegistrationNo1,@RegistrationNo2,
        @RegistrationNo3,@Remark,@Quota_Eng,@Quota_Beear)"

            Dim p As New Dictionary(Of String, Object)

            p.Add("@ID", dr("ID"))
            p.Add("@BusinessStartDate", dr("BusinessStartDate"))
            p.Add("@CompanyName", dr("CompanyName"))
            p.Add("@CompanyAddress", dr("CompanyAddress"))
            p.Add("@ContactNumber", dr("ContactNumber"))
            p.Add("@RegistrationNo1", dr("RegistrationNo1"))
            p.Add("@RegistrationNo2", dr("RegistrationNo2"))
            p.Add("@RegistrationNo3", dr("RegistrationNo3"))
            p.Add("@Remark", dr("Remark"))
            p.Add("@Quota_Eng", dr("Quota_Eng"))
            p.Add("@Quota_Beear", dr("Quota_Beear"))

            ExecuteNonQuery(sql, p)

        Next

    End Sub

    Private Sub Migrate_tabFY()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabFY")

            ExecuteNonQuery("DELETE FROM tabFY")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabFY
            (ID,FY,Current,FirstDate,LastDate)
            VALUES
            (@ID,@FY,@Current,@FirstDate,@LastDate)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@FY", dr("FY"))
                p.Add("@Current", dr("Current"))
                p.Add("@FirstDate", dr("FirstDate"))
                p.Add("@LastDate", dr("LastDate"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("modMigration",
                     "Migrate_tabFY",
                     "",
                     ex)
            Throw

        End Try

    End Sub

    Private Sub Migrate_tabCategory()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabCategory")

            ExecuteNonQuery("DELETE FROM tabCategory")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabCategory
            (ID,Category)
            VALUES
            (@ID,@Category)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Category", dr("Category"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("modMigration",
                     "Migrate_tabCategory",
                     "",
                     ex)
            Throw

        End Try

    End Sub

    Private Sub Migrate_tabPacking()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabPacking")

            ExecuteNonQuery("DELETE FROM tabPacking")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabPacking
            (ID,Packing)
            VALUES
            (@ID,@Packing)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Packing", dr("Packing"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("modMigration",
                     "Migrate_tabPacking",
                     "",
                     ex)
            Throw

        End Try

    End Sub

    Private Sub Migrate_tabItem()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabItem")

            ExecuteNonQuery("DELETE FROM tabItem")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabItem
            (ID,ItemName,PRate,SRate,Category,Packing,QtyInCase)
            VALUES
            (@ID,@ItemName,@PRate,@SRate,@Category,@Packing,@QtyInCase)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@ItemName", dr("ItemName"))

                If IsDBNull(dr("PRate")) Then
                    p.Add("@PRate", 0)
                Else
                    p.Add("@PRate", dr("PRate"))
                End If

                If IsDBNull(dr("SRate")) Then
                    p.Add("@SRate", 0)
                Else
                    p.Add("@SRate", dr("SRate"))
                End If

                If IsDBNull(dr("Category")) Then
                    p.Add("@Category", DBNull.Value)
                Else
                    p.Add("@Category", dr("Category"))
                End If

                If IsDBNull(dr("Packing")) Then
                    p.Add("@Packing", DBNull.Value)
                Else
                    p.Add("@Packing", dr("Packing"))
                End If

                If IsDBNull(dr("QtyInCase")) Then
                    p.Add("@QtyInCase", 0)
                Else
                    p.Add("@QtyInCase", dr("QtyInCase"))
                End If

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("modMigration",
                     "Migrate_tabItem",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabTCS()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabTCS")

            ExecuteNonQuery("DELETE FROM tabTCS")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabTCS
            (ID,TCSRate)
            VALUES
            (@ID,@TCSRate)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@TCSRate", dr("TCSRate"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("modMigration",
                     "Migrate_tabTCS",
                     "",
                     ex)

            Throw

        End Try

    End Sub



    Private Sub Migrate_tabUsers()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabUsers")

            ExecuteNonQuery("DELETE FROM tabUsers")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabUsers
            (ID,UserID,Password,CompanyID)
            VALUES
            (@ID,@UserID,@Password,@CompanyID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@UserID", dr("UserID"))
                p.Add("@Password", dr("Password"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("modMigration",
                     "Migrate_tabUsers",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabParty()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabParty")

            ExecuteNonQuery("DELETE FROM tabParty")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabParty
            (ID,BName,CPName,GSTNo,ContNo,Address,CompanyID,UserID)
            VALUES
            (@ID,@BName,@CPName,@GSTNo,@ContNo,@Address,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@BName", dr("BName"))
                p.Add("@CPName", If(IsDBNull(dr("CPName")), DBNull.Value, dr("CPName")))
                p.Add("@GSTNo", If(IsDBNull(dr("GSTNo")), DBNull.Value, dr("GSTNo")))
                p.Add("@ContNo", If(IsDBNull(dr("ContNo")), DBNull.Value, dr("ContNo")))
                p.Add("@Address", If(IsDBNull(dr("Address")), DBNull.Value, dr("Address")))
                p.Add("@CompanyID", If(IsDBNull(dr("CompanyID")), DBNull.Value, dr("CompanyID")))
                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabParty",
                     "",
                     ex)
            Throw

        End Try

    End Sub


    Private Sub Migrate_tabCapital()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabCapital")

            ExecuteNonQuery("DELETE FROM tabCapital")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabCapital
            (ID,CapitalAmountCash,CapitalAmountBank,Ref,Date,
             PartyID,FY,CompanyID,UserID)
            VALUES
            (@ID,@CapitalAmountCash,@CapitalAmountBank,@Ref,@Date,
             @PartyID,@FY,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@CapitalAmountCash", If(IsDBNull(dr("CapitalAmountCash")), 0, dr("CapitalAmountCash")))
                p.Add("@CapitalAmountBank", If(IsDBNull(dr("CapitalAmountBank")), 0, dr("CapitalAmountBank")))
                p.Add("@Ref", If(IsDBNull(dr("Ref")), DBNull.Value, dr("Ref")))
                p.Add("@Date", If(IsDBNull(dr("Date")), DBNull.Value, dr("Date")))
                p.Add("@PartyID", If(IsDBNull(dr("PartyID")), DBNull.Value, dr("PartyID")))
                p.Add("@FY", dr("FY"))
                p.Add("@CompanyID", If(IsDBNull(dr("CompanyID")), DBNull.Value, dr("CompanyID")))
                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabCapital",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabCashFlow()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabCashFlow")

            ExecuteNonQuery("DELETE FROM tabCashFlow")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabCashFlow
            (ID,Date,CashToBank,BankToCash,Ref,FY,CompanyID,UserID)
            VALUES
            (@ID,@Date,@CashToBank,@BankToCash,@Ref,@FY,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Date", dr("Date"))

                If IsDBNull(dr("CashToBank")) Then
                    p.Add("@CashToBank", 0)
                Else
                    p.Add("@CashToBank", dr("CashToBank"))
                End If

                If IsDBNull(dr("BankToCash")) Then
                    p.Add("@BankToCash", 0)
                Else
                    p.Add("@BankToCash", dr("BankToCash"))
                End If

                If IsDBNull(dr("Ref")) Then
                    p.Add("@Ref", DBNull.Value)
                Else
                    p.Add("@Ref", dr("Ref"))
                End If

                p.Add("@FY", dr("FY"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabCashFlow",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabExpHead()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabExpHead")

            ExecuteNonQuery("DELETE FROM tabExpHead")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabExpHead
            (ID,ExpHead,Recurring,RecurringAmt,ValidDay,CompanyID,UserID)
            VALUES
            (@ID,@ExpHead,@Recurring,@RecurringAmt,@ValidDay,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@ExpHead", dr("ExpHead"))

                If IsDBNull(dr("Recurring")) Then
                    p.Add("@Recurring", 0)
                Else
                    p.Add("@Recurring", dr("Recurring"))
                End If

                If IsDBNull(dr("RecurringAmt")) Then
                    p.Add("@RecurringAmt", 0)
                Else
                    p.Add("@RecurringAmt", dr("RecurringAmt"))
                End If

                If IsDBNull(dr("ValidDay")) Then
                    p.Add("@ValidDay", 0)
                Else
                    p.Add("@ValidDay", dr("ValidDay"))
                End If

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabExpHead",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabExpences()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabExpences")

            ExecuteNonQuery("DELETE FROM tabExpences")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabExpences
            (ID,Date,ExpHeadID,Cash,Bank,Ref,FY,CompanyID,UserID)
            VALUES
            (@ID,@Date,@ExpHeadID,@Cash,@Bank,@Ref,@FY,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Date", dr("Date"))

                If IsDBNull(dr("ExpHeadID")) Then
                    p.Add("@ExpHeadID", DBNull.Value)
                Else
                    p.Add("@ExpHeadID", dr("ExpHeadID"))
                End If

                If IsDBNull(dr("Cash")) Then
                    p.Add("@Cash", 0)
                Else
                    p.Add("@Cash", dr("Cash"))
                End If

                If IsDBNull(dr("Bank")) Then
                    p.Add("@Bank", 0)
                Else
                    p.Add("@Bank", dr("Bank"))
                End If

                If IsDBNull(dr("Ref")) Then
                    p.Add("@Ref", DBNull.Value)
                Else
                    p.Add("@Ref", dr("Ref"))
                End If

                p.Add("@FY", dr("FY"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabExpences",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabLaserSale()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabLaserSale")

            ExecuteNonQuery("DELETE FROM tabLaserSale")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabLaserSale
            (ID,Date,SoldAmtCash,SoldAmtBank,CashAmtRcv,BankAmtRcv,
             FY,Narration,CompanyID,UserID)
            VALUES
            (@ID,@Date,@SoldAmtCash,@SoldAmtBank,@CashAmtRcv,@BankAmtRcv,
             @FY,@Narration,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Date", dr("Date"))

                p.Add("@SoldAmtCash", If(IsDBNull(dr("SoldAmtCash")), 0, dr("SoldAmtCash")))
                p.Add("@SoldAmtBank", If(IsDBNull(dr("SoldAmtBank")), 0, dr("SoldAmtBank")))
                p.Add("@CashAmtRcv", If(IsDBNull(dr("CashAmtRcv")), 0, dr("CashAmtRcv")))
                p.Add("@BankAmtRcv", If(IsDBNull(dr("BankAmtRcv")), 0, dr("BankAmtRcv")))

                p.Add("@FY", dr("FY"))
                p.Add("@Narration", If(IsDBNull(dr("Narration")), DBNull.Value, dr("Narration")))
                p.Add("@CompanyID", If(IsDBNull(dr("CompanyID")), DBNull.Value, dr("CompanyID")))
                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabLaserSale",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabPurchageMaster()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabPurchageMaster")

            ExecuteNonQuery("DELETE FROM tabPurchageMaster")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabPurchageMaster
            (ID,Date,BillNo,PartyID,Amount,TCS,TotalAmount,
             FY,CompanyID,UserID)
            VALUES
            (@ID,@Date,@BillNo,@PartyID,@Amount,@TCS,@TotalAmount,
             @FY,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Date", dr("Date"))
                p.Add("@BillNo", dr("BillNo"))
                p.Add("@PartyID", dr("PartyID"))
                p.Add("@Amount", dr("Amount"))
                p.Add("@TCS", dr("TCS"))
                p.Add("@TotalAmount", dr("TotalAmount"))
                p.Add("@FY", dr("FY"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabPurchageMaster",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabPurchageSlave()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabPurchageSlave")

            ExecuteNonQuery("DELETE FROM tabPurchageSlave")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabPurchageSlave
            (ID,MasterID,ItemID,Rate,Qty,
             CompanyID,UserID,FY)
            VALUES
            (@ID,@MasterID,@ItemID,@Rate,@Qty,
             @CompanyID,@UserID,@FY)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@MasterID", dr("MasterID"))
                p.Add("@ItemID", dr("ItemID"))
                p.Add("@Rate", dr("Rate"))
                p.Add("@Qty", dr("Qty"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))
                p.Add("@FY", dr("FY"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabPurchageSlave",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabSale()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabSale")

            ExecuteNonQuery("DELETE FROM tabSale")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabSale
            (ID,Date,ItemID,Rate,Qty,FY,CompanyID,UserID)
            VALUES
            (@ID,@Date,@ItemID,@Rate,@Qty,@FY,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Date", dr("Date"))
                p.Add("@ItemID", dr("ItemID"))
                p.Add("@Rate", dr("Rate"))
                p.Add("@Qty", dr("Qty"))
                p.Add("@FY", dr("FY"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabSale",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub Migrate_tabPartyLaser()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabPartyLaser")

            ExecuteNonQuery("DELETE FROM tabPartyLaser")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabPartyLaser
            (ID,MasterID,PartyID,Date,BillNo,
             AmtCRBank,AmtCRCash,AmtDr,Ref,
             FY,CompanyID,UserID)
            VALUES
            (@ID,@MasterID,@PartyID,@Date,@BillNo,
             @AmtCRBank,@AmtCRCash,@AmtDr,@Ref,
             @FY,@CompanyID,@UserID)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))

                If IsDBNull(dr("MasterID")) Then
                    p.Add("@MasterID", DBNull.Value)
                Else
                    p.Add("@MasterID", dr("MasterID"))
                End If

                p.Add("@PartyID", dr("PartyID"))
                p.Add("@Date", dr("Date"))

                If IsDBNull(dr("BillNo")) Then
                    p.Add("@BillNo", DBNull.Value)
                Else
                    p.Add("@BillNo", dr("BillNo"))
                End If

                p.Add("@AmtCRBank", If(IsDBNull(dr("AmtCRBank")), 0, dr("AmtCRBank")))
                p.Add("@AmtCRCash", If(IsDBNull(dr("AmtCRCash")), 0, dr("AmtCRCash")))
                p.Add("@AmtDr", If(IsDBNull(dr("AmtDr")), 0, dr("AmtDr")))

                If IsDBNull(dr("Ref")) Then
                    p.Add("@Ref", DBNull.Value)
                Else
                    p.Add("@Ref", dr("Ref"))
                End If

                p.Add("@FY", dr("FY"))

                If IsDBNull(dr("CompanyID")) Then
                    p.Add("@CompanyID", DBNull.Value)
                Else
                    p.Add("@CompanyID", dr("CompanyID"))
                End If

                p.Add("@UserID", dr("UserID"))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabPartyLaser",
                     "",
                     ex)

            Throw

        End Try

    End Sub

    Private Sub cmdAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAll.Click

        Try

            If MsgBox("SQL Server Data SQLite में Transfer करना चाहते हैं ?",
                      MsgBoxStyle.YesNo + MsgBoxStyle.Question) = MsgBoxResult.No Then Exit Sub

            ProgressBar1.Minimum = 0
            ProgressBar1.Maximum = 18
            ProgressBar1.Value = 0

            txtLog.Clear()
            lblStatus.Text = "Opening SQL Server..."

            Dim ConnStr As String

            If chkWindows.Checked Then

                ConnStr = "Data Source=" & txtServer.Text.Trim & ";" &
                          "Initial Catalog=" & txtDatabase.Text.Trim & ";" &
                          "Integrated Security=True"

            Else

                ConnStr = "Data Source=" & txtServer.Text.Trim & ";" &
                          "Initial Catalog=" & txtDatabase.Text.Trim & ";" &
                          "User ID=" & txtUser.Text.Trim & ";" &
                          "Password=" & txtPassword.Text.Trim

            End If

            If OpenSQLConnection(ConnStr) = False Then

                MsgBox("SQL Server Connection Failed.", MsgBoxStyle.Critical)
                Exit Sub

            End If

            OpenDB()
            ExecuteNonQuery("PRAGMA foreign_keys=OFF;")
            SQLiteTran = mySQLiteConn.BeginTransaction()



            '=====================
            ' MASTER TABLES
            '=====================

            lblStatus.Text = "Migrating Master Tables..."

            Migrate_tabSystem()
            ProgressBar1.Value += 1

            Migrate_tabCompany()
            ProgressBar1.Value += 1

            Migrate_tabFY()
            ProgressBar1.Value += 1

            Migrate_tabCategory()
            ProgressBar1.Value += 1

            Migrate_tabPacking()
            ProgressBar1.Value += 1

            Migrate_tabItem()
            ProgressBar1.Value += 1

            Migrate_tabTCS()
            ProgressBar1.Value += 1

            Migrate_tabUsers()
            ProgressBar1.Value += 1

            '=====================
            ' TRANSACTION TABLES
            '=====================

            lblStatus.Text = "Migrating Transaction Tables..."

            Migrate_tabParty()
            ProgressBar1.Value += 1

            Migrate_tabCapital()
            ProgressBar1.Value += 1

            Migrate_tabCashFlow()
            ProgressBar1.Value += 1

            Migrate_tabExpHead()
            ProgressBar1.Value += 1

            Migrate_tabExpences()
            ProgressBar1.Value += 1

            Migrate_tabLaserSale()
            ProgressBar1.Value += 1

            Migrate_tabPurchageMaster()
            ProgressBar1.Value += 1

            Migrate_tabPurchageSlave()
            ProgressBar1.Value += 1

            Migrate_tabSale()
            ProgressBar1.Value += 1

            Migrate_tabPartyLaser()
            ProgressBar1.Value += 1

            ExecuteNonQuery("PRAGMA foreign_keys=ON;")

            SQLiteTran.Commit()
            SQLiteTran = Nothing

            CloseSQLConnection()

            lblStatus.Text = "Migration Completed"

            txtLog.AppendText(vbCrLf)
            txtLog.AppendText("===================================" & vbCrLf)
            txtLog.AppendText("Migration Completed Successfully." & vbCrLf)
            txtLog.AppendText("Date : " & Now.ToString("dd/MM/yyyy HH:mm:ss") & vbCrLf)

            MsgBox("SQL Server To SQLite Migration Completed Successfully.", MsgBoxStyle.Information)

        Catch ex As Exception

            If SQLiteTran IsNot Nothing Then

                SQLiteTran.Rollback()
                SQLiteTran = Nothing

            End If

            CloseSQLConnection()

            LogError("frmDataMigration",
                     "cmdAll_Click",
                     "",
                     ex)

            MsgBox("Error in : " & lblStatus.Text & vbCrLf & ex.Message, MsgBoxStyle.Critical)

        End Try

    End Sub



    Private Sub Migrate_tabSystem()

        Try

            Dim dt As DataTable = GetSQLDataTable("SELECT * FROM tabSystem")

            ExecuteNonQuery("DELETE FROM tabSystem")

            For Each dr As DataRow In dt.Rows

                Dim sql As String =
                "INSERT INTO tabSystem
            (ID,Path,DBID,DBPassword,DataSource,InitialCatalog)
            VALUES
            (@ID,@Path,@DBID,@DBPassword,@DataSource,@InitialCatalog)"

                Dim p As New Dictionary(Of String, Object)

                p.Add("@ID", dr("ID"))
                p.Add("@Path", If(IsDBNull(dr("Path")), DBNull.Value, dr("Path")))
                p.Add("@DBID", If(IsDBNull(dr("DBID")), DBNull.Value, dr("DBID")))
                p.Add("@DBPassword", If(IsDBNull(dr("DBPassword")), DBNull.Value, dr("DBPassword")))
                p.Add("@DataSource", If(IsDBNull(dr("DataSource")), DBNull.Value, dr("DataSource")))
                p.Add("@InitialCatalog", If(IsDBNull(dr("InitialCatalog")), DBNull.Value, dr("InitialCatalog")))

                ExecuteNonQuery(sql, p)

            Next

        Catch ex As Exception

            LogError("frmDataMigration",
                     "Migrate_tabSystem",
                     "",
                     ex)

            Throw

        End Try

    End Sub

End Class