Imports System.Data.SQLite
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Module modSQLite

    Public DBPath As String = Application.StartupPath & "\INV.db"
    Public mySQLiteConn As SQLiteConnection
    Public SQLiteTran As SQLiteTransaction = Nothing

    Public Sub OpenDB()

        Try

            'Database Create If Not Exists
            If Not File.Exists(DBPath) Then
                SQLiteConnection.CreateFile(DBPath)
            End If

            'Connection Already Open
            If mySQLiteConn IsNot Nothing Then
                If mySQLiteConn.State = ConnectionState.Open Then Exit Sub
            End If

            'Connection String
            Dim ConnStr As String =
            "Data Source=" & DBPath & ";" &
            "Version=3;" &
            "Pooling=True;"

            mySQLiteConn = New SQLiteConnection(ConnStr)
            mySQLiteConn.Open()

            'SQLite Settings
            Using cmd As New SQLiteCommand(mySQLiteConn)

                cmd.CommandText = "PRAGMA foreign_keys = ON;"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "PRAGMA journal_mode = WAL;"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "PRAGMA synchronous = NORMAL;"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "PRAGMA temp_store = MEMORY;"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "PRAGMA cache_size = 10000;"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "PRAGMA busy_timeout = 5000;"
                cmd.ExecuteNonQuery()

            End Using

            'Create Tables
            CreateDatabase()

        Catch ex As Exception

            LogError("modSQLite", "OpenDB", "", ex)
            Throw

        End Try

    End Sub

    Public Sub CreateDatabase()

        '==========================
        ' MASTER TABLES
        '==========================

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabSystem(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Path TEXT,
DBID TEXT,
DBPassword TEXT,
DataSource TEXT,
InitialCatalog TEXT
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabCompany(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
BusinessStartDate TEXT NOT NULL,
CompanyName TEXT NOT NULL,
CompanyAddress TEXT NOT NULL,
ContactNumber TEXT,
RegistrationNo1 TEXT,
RegistrationNo2 TEXT,
RegistrationNo3 TEXT,
Remark TEXT,
Quota_Eng REAL DEFAULT 0,
Quota_Beear REAL DEFAULT 0
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabFY(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
FY TEXT NOT NULL,
Current INTEGER DEFAULT 0,
FirstDate TEXT,
LastDate TEXT
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabUsers(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
UserID TEXT NOT NULL,
Password TEXT NOT NULL,
CompanyID INTEGER,
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabCategory(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Category TEXT NOT NULL
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabPacking(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Packing TEXT NOT NULL
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabItem(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
ItemName TEXT NOT NULL,
PRate REAL DEFAULT 0,
SRate REAL DEFAULT 0,
Category INTEGER,
Packing INTEGER,
QtyInCase INTEGER DEFAULT 0,
FOREIGN KEY(Category) REFERENCES tabCategory(ID),
FOREIGN KEY(Packing) REFERENCES tabPacking(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabTCS(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
TCSRate REAL DEFAULT 0
);")

        '==========================
        ' TRANSACTION TABLES - PART 2
        '==========================

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabParty(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
BName TEXT NOT NULL,
CPName TEXT,
GSTNo TEXT,
ContNo TEXT,
Address TEXT,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabCapital(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
CapitalAmountCash REAL DEFAULT 0,
CapitalAmountBank REAL DEFAULT 0,
Ref TEXT,
Date TEXT,
PartyID INTEGER,
FY INTEGER,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(PartyID) REFERENCES tabParty(ID),
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabCashFlow(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Date TEXT NOT NULL,
CashToBank REAL DEFAULT 0,
BankToCash REAL DEFAULT 0,
Ref TEXT,
FY INTEGER,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabExpHead(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
ExpHead TEXT NOT NULL,
Recurring INTEGER DEFAULT 0,
RecurringAmt REAL DEFAULT 0,
ValidDay INTEGER DEFAULT 0,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabExpences(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Date TEXT NOT NULL,
ExpHeadID INTEGER,
Cash REAL DEFAULT 0,
Bank REAL DEFAULT 0,
Ref TEXT,
FY INTEGER,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(ExpHeadID) REFERENCES tabExpHead(ID),
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabLaserSale(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Date TEXT NOT NULL,
SoldAmtCash REAL DEFAULT 0,
SoldAmtBank REAL DEFAULT 0,
CashAmtRcv REAL DEFAULT 0,
BankAmtRcv REAL DEFAULT 0,
FY INTEGER,
Narration TEXT,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        '==========================
        ' TRANSACTION TABLES - PART 3
        '==========================

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabPurchageMaster(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Date TEXT NOT NULL,
BillNo TEXT NOT NULL,
PartyID INTEGER NOT NULL,
Amount REAL NOT NULL,
TCS REAL NOT NULL,
TotalAmount REAL NOT NULL,
FY INTEGER NOT NULL,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(PartyID) REFERENCES tabParty(ID),
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabPurchageSlave(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
MasterID INTEGER NOT NULL,
ItemID INTEGER NOT NULL,
Rate REAL NOT NULL,
Qty REAL NOT NULL,
CompanyID INTEGER,
UserID INTEGER,
FY INTEGER,
FOREIGN KEY(MasterID) REFERENCES tabPurchageMaster(ID),
FOREIGN KEY(ItemID) REFERENCES tabItem(ID),
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabSale(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
Date TEXT NOT NULL,
ItemID INTEGER NOT NULL,
Rate REAL NOT NULL,
Qty REAL NOT NULL,
FY INTEGER NOT NULL,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(ItemID) REFERENCES tabItem(ID),
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        ExecuteNonQuery("
CREATE TABLE IF NOT EXISTS tabPartyLaser(
ID INTEGER PRIMARY KEY AUTOINCREMENT,
MasterID INTEGER,
PartyID INTEGER NOT NULL,
Date TEXT NOT NULL,
BillNo TEXT,
AmtCRBank REAL DEFAULT 0,
AmtCRCash REAL DEFAULT 0,
AmtDr REAL DEFAULT 0,
Ref TEXT,
FY INTEGER,
CompanyID INTEGER,
UserID INTEGER,
FOREIGN KEY(PartyID) REFERENCES tabParty(ID),
FOREIGN KEY(FY) REFERENCES tabFY(ID),
FOREIGN KEY(CompanyID) REFERENCES tabCompany(ID),
FOREIGN KEY(UserID) REFERENCES tabUsers(ID)
);")

        '==========================
        ' INDEXES
        '==========================

        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS IDX_tabUsers_UserID ON tabUsers(UserID);")
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS IDX_tabParty_BName ON tabParty(BName);")
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS IDX_tabItem_ItemName ON tabItem(ItemName);")
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS IDX_tabPurchageMaster_BillNo ON tabPurchageMaster(BillNo);")
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS IDX_tabSale_Date ON tabSale(Date);")

        '==========================
        ' DEFAULT DATA
        '==========================

        ExecuteNonQuery("INSERT OR IGNORE INTO tabTCS(ID,TCSRate) VALUES(1,1);")

        ExecuteNonQuery("INSERT OR IGNORE INTO tabCategory(ID,Category) VALUES(1,'GENERAL');")

        ExecuteNonQuery("INSERT OR IGNORE INTO tabPacking(ID,Packing) VALUES(1,'NOS');")

        ExecuteNonQuery("INSERT OR IGNORE INTO tabSystem
(ID,Path,DBID,DBPassword,DataSource,InitialCatalog)
VALUES(1,'','','','','');")


        '==========================
        ' DEFAULT FINANCIAL YEAR
        '==========================

        Dim fyCount As Integer = Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM tabFY"))

        If fyCount = 0 Then

            ExecuteNonQuery("
    INSERT INTO tabFY
    (FY,Current,FirstDate,LastDate)
    VALUES
    ('2026-2027',1,'2026-04-01','2027-03-31');
    ")

        End If





    End Sub

    Public Sub LogError(FormName As String,
                        FunctionName As String,
                        SQL As String,
                        ex As Exception)

        Try
            Dim LogFolder As String = Path.Combine(Application.StartupPath, "Logs")

            If Not Directory.Exists(LogFolder) Then
                Directory.CreateDirectory(LogFolder)
            End If

            Dim FileName As String = Path.Combine(LogFolder, Date.Now.ToString("yyyy-MM-dd") & ".log")

            Using sw As New StreamWriter(FileName, True)
                sw.WriteLine("====================================================")
                sw.WriteLine("Date/Time : " & Date.Now.ToString("dd-MM-yyyy HH:mm:ss"))
                sw.WriteLine("Form      : " & FormName)
                sw.WriteLine("Function  : " & FunctionName)
                sw.WriteLine("SQL       : " & SQL)
                sw.WriteLine("Error     : " & ex.Message)
                sw.WriteLine("Stack     : " & ex.StackTrace)
                sw.WriteLine()
            End Using

        Catch
        End Try

    End Sub

    Public Function ExecuteNonQuery(ByVal sql As String,
                                Optional ByVal params As Dictionary(Of String, Object) = Nothing) As Integer
        Try

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                'Transaction Support
                If SQLiteTran IsNot Nothing Then
                    cmd.Transaction = SQLiteTran
                End If

                'Parameters
                If params IsNot Nothing Then
                    For Each p As KeyValuePair(Of String, Object) In params
                        cmd.Parameters.AddWithValue(p.Key, If(p.Value Is Nothing, DBNull.Value, p.Value))
                    Next
                End If

                Return cmd.ExecuteNonQuery()

            End Using

        Catch ex As Exception

            LogError("modSQLite", "ExecuteNonQuery", sql, ex)
            Throw

        End Try
    End Function

    Public Function ExecuteScalar(ByVal sql As String,
                              Optional ByVal params As Dictionary(Of String, Object) = Nothing) As Object
        Try

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                'Transaction Support
                If SQLiteTran IsNot Nothing Then
                    cmd.Transaction = SQLiteTran
                End If

                'Parameters
                If params IsNot Nothing Then
                    For Each p As KeyValuePair(Of String, Object) In params
                        cmd.Parameters.AddWithValue(p.Key, If(p.Value Is Nothing, DBNull.Value, p.Value))
                    Next
                End If

                Return cmd.ExecuteScalar()

            End Using

        Catch ex As Exception

            LogError("modSQLite", "ExecuteScalar", sql, ex)
            Throw

        End Try
    End Function

    Public Function GetDataTable(ByVal sql As String,
                             Optional ByVal params As Dictionary(Of String, Object) = Nothing) As DataTable
        Try

            Dim dt As New DataTable()

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                If SQLiteTran IsNot Nothing Then
                    cmd.Transaction = SQLiteTran
                End If

                If params IsNot Nothing Then
                    For Each p As KeyValuePair(Of String, Object) In params
                        cmd.Parameters.AddWithValue(p.Key, If(p.Value Is Nothing, DBNull.Value, p.Value))
                    Next
                End If

                Using da As New SQLiteDataAdapter()
                    da.SelectCommand = cmd
                    da.Fill(dt)
                End Using

            End Using

            Return dt

        Catch ex As Exception

            LogError("modSQLite", "GetDataTable", sql, ex)
            Throw

        End Try

    End Function
    Public Sub BeginTransaction()

        If SQLiteTran Is Nothing Then
            SQLiteTran = mySQLiteConn.BeginTransaction()
        End If

    End Sub

    Public Sub CommitTransaction()

        If SQLiteTran IsNot Nothing Then
            SQLiteTran.Commit()
            SQLiteTran.Dispose()
            SQLiteTran = Nothing
        End If

    End Sub

    Public Sub RollbackTransaction()

        If SQLiteTran IsNot Nothing Then
            SQLiteTran.Rollback()
            SQLiteTran.Dispose()
            SQLiteTran = Nothing
        End If

    End Sub

    Public Function ExecuteReader(ByVal sql As String,
    Optional ByVal params As Dictionary(Of String, Object) = Nothing) As SQLiteDataReader

        Try

            If mySQLiteConn Is Nothing Then
                OpenDB()
            ElseIf mySQLiteConn.State <> ConnectionState.Open Then
                mySQLiteConn.Open()
            End If

            Dim cmd As New SQLiteCommand(sql, mySQLiteConn)

            If SQLiteTran IsNot Nothing Then
                cmd.Transaction = SQLiteTran
            End If

            If params IsNot Nothing Then
                For Each p As KeyValuePair(Of String, Object) In params
                    cmd.Parameters.AddWithValue(p.Key, If(p.Value Is Nothing, DBNull.Value, p.Value))
                Next
            End If
            'MsgBox(If(mySQLiteConn Is Nothing, "Nothing", "OK"))
            Return cmd.ExecuteReader()

        Catch ex As Exception

            LogError("modSQLite", "ExecuteReader", sql, ex)
            Throw
        End Try

    End Function

    Public Sub CloseDB()

        Try
            If SQLiteTran IsNot Nothing Then
                SQLiteTran.Rollback()
                SQLiteTran.Dispose()
                SQLiteTran = Nothing
            End If

            'Connection Close
            If mySQLiteConn IsNot Nothing Then

                If mySQLiteConn.State <> ConnectionState.Closed Then
                    mySQLiteConn.Close()
                End If

                mySQLiteConn.Dispose()
                mySQLiteConn = Nothing

            End If

        Catch ex As Exception

            LogError("modSQLite", "CloseDB", "", ex)

        End Try

    End Sub
    Public Function HashPassword(ByVal Password As String) As String

        Using sha As SHA256 = SHA256.Create()

            Dim bytes() As Byte = Encoding.UTF8.GetBytes(Password)
            Dim hash() As Byte = sha.ComputeHash(bytes)

            Dim sb As New StringBuilder()

            For Each b As Byte In hash
                sb.Append(b.ToString("x2"))
            Next

            Return sb.ToString()

        End Using

    End Function

    Public Function BackupDatabase() As Boolean

        Try

            Dim BackupFolder As String = Path.Combine(Application.StartupPath, "Backup")

            If Not Directory.Exists(BackupFolder) Then
                Directory.CreateDirectory(BackupFolder)
            End If

            Dim BackupFile As String = Path.Combine(BackupFolder,
                "INV_" & Date.Now.ToString("yyyyMMdd_HHmmss") & ".db")

            'Connection Close
            CloseDB()

            File.Copy(DBPath, BackupFile, True)

            'Reopen Database
            OpenDB()

            Return True

        Catch ex As Exception

            LogError("modSQLite", "BackupDatabase", "", ex)
            Return False

        End Try

    End Function

    Public Function RestoreDatabase(ByVal BackupFile As String) As Boolean

        Try

            If Not File.Exists(BackupFile) Then
                Throw New Exception("Backup File Not Found.")

            End If

            'Close Connection
            CloseDB()

            'Restore Database
            File.Copy(BackupFile, DBPath, True)

            'Open Again
            OpenDB()

            Return True

        Catch ex As Exception

            LogError("modSQLite", "RestoreDatabase", BackupFile, ex)
            Return False

        End Try

    End Function

    Public Function Get_FirstDay() As Date

        Try

            Dim sql As String = "SELECT FirstDate FROM tabFY WHERE Current=1"

            Dim obj As Object = ExecuteScalar(sql)

            If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                Return CDate(obj)
            Else
                Return Today
            End If

        Catch ex As Exception

            LogError("modMain", "Get_FirstDay", "", ex)
            Return Today

        End Try

    End Function

    Public Function Get_LastDay() As Date

        Try

            Dim sql As String = "SELECT LastDate FROM tabFY WHERE CURRENT=1"

            Dim obj As Object = ExecuteScalar(sql)

            If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                Return CDate(obj)
            Else
                Return Today
            End If

        Catch ex As Exception

            LogError("modMain", "Get_LastDay", "", ex)
            Return Today

        End Try

    End Function


    Public Sub GetCurrentFY()

        Dim sql As String =
            "SELECT ID, FY
         FROM tabFY
         WHERE current = 1
         LIMIT 1"

        Try

            Using cmd As New SQLiteCommand(sql, mySQLiteConn)

                Using dr As SQLiteDataReader = cmd.ExecuteReader()

                    If dr.Read() Then
                        CurrentFYID = Convert.ToInt32(dr("ID"))
                        CurrentFYName = dr("FY").ToString()
                    Else
                        CurrentFYID = 0
                        CurrentFYName = ""
                        Throw New Exception("No Active Financial Year Found.")
                    End If

                End Using

            End Using

        Catch ex As Exception

            CurrentFYID = 0
            CurrentFYName = ""
            LogError("modGlobal", "GetCurrentFY", sql, ex)
            Throw

        End Try

    End Sub

    Public Function Get_BSD() As Date

        Dim sql As String = ""
        Dim rdr As SQLiteDataReader = Nothing

        Try

            sql = "SELECT BusinessStartDate
               FROM tabCompany
               WHERE ID=@CompanyID"

            Dim params As New Dictionary(Of String, Object) From {
                {"@CompanyID", CurrentCompanyID}
            }

            rdr = ExecuteReader(sql, params)

            If rdr.Read() Then
                Return CDate(rdr("BusinessStartDate"))
            Else
                Return Today
            End If

        Catch ex As Exception

            LogError("modSQLite", "Get_BSD", sql, ex)
            Return Today

        Finally

            If rdr IsNot Nothing Then rdr.Close()

        End Try

    End Function

    Public Sub LoadCompanySession()

        Try

            Dim sql As String =
                "SELECT CompanyName, CompanyAddress, ContactNumber, RegistrationNo1
             FROM tabCompany
             WHERE ID=@ID"

            Dim params As New Dictionary(Of String, Object)
            params.Add("@ID", CurrentCompanyID)

            Dim dt As DataTable = GetDataTable(sql, params)

            If dt.Rows.Count > 0 Then
                CurrentCompanyName = dt.Rows(0)("CompanyName").ToString()
                CurrentCompanyAddress = dt.Rows(0)("CompanyAddress").ToString()
                CurrentCompanyContact = dt.Rows(0)("ContactNumber").ToString()
                CurrentCompanyRegistrationNo = dt.Rows(0)("RegistrationNo1").ToString()
            End If

        Catch ex As Exception
            LogError("modSession", "LoadCompanySession", "", ex)
        End Try

    End Sub
End Module