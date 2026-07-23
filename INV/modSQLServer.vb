Imports System.Data.SqlClient

Module modSQLServer

    Public SQLConn As SqlConnection

    Public Function OpenSQLConnection(ByVal ConnString As String) As Boolean

        Try

            If SQLConn Is Nothing Then
                SQLConn = New SqlConnection(ConnString)
            ElseIf SQLConn.ConnectionString <> ConnString Then
                SQLConn.Dispose()
                SQLConn = New SqlConnection(ConnString)
            End If

            If SQLConn.State <> ConnectionState.Open Then
                SQLConn.Open()
            End If

            Return True

        Catch ex As Exception

            LogError("modSQLServer",
                     "OpenSQLConnection",
                     ConnString,
                     ex)

            Return False

        End Try

    End Function

    Public Sub CloseSQLConnection()

        Try

            If SQLConn IsNot Nothing AndAlso SQLConn.State = ConnectionState.Open Then
                SQLConn.Close()
            End If

        Catch
        End Try

    End Sub

    Public Function GetSQLDataTable(ByVal SQL As String) As DataTable

        Dim dt As New DataTable()

        Using cmd As New SqlCommand(SQL, SQLConn)

            Using da As New SqlDataAdapter(cmd)

                da.Fill(dt)

            End Using

        End Using

        Return dt

    End Function

End Module

