Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Linq

Namespace OvertimeNamespace
    <System.ComponentModel.DataObject()> _
    Public Class Overtime
        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Shared Function GetOvertime(ByVal tlkpOtFY As Int32, ByVal FiscalYear As String, ByVal tlkpOtOffice As Int32) As DataView
            Try

                Dim BeginningCalendarYear As String = FiscalYear.Substring(0, 4)
                Dim EndingCalendarYear As String = FiscalYear.Substring(5, 4)

                Dim HistOrCurr As String = String.Empty
                If EndingCalendarYear < 2017 Then
                    HistOrCurr = "Hist"
                Else
                    HistOrCurr = "Curr"
                End If

                Dim cnOvertime As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

                ' Get the Overtime
                Dim cmOvertime As New SqlCommand("uspGetOvertimePrograms", cnOvertime)
                cmOvertime.CommandType = CommandType.StoredProcedure
                Dim parmHistOrCurr As New SqlParameter("@HistOrCurr", SqlDbType.VarChar)
                parmHistOrCurr.Value = HistOrCurr
                cmOvertime.Parameters.Add(parmHistOrCurr)
                cnOvertime.Open()
                Dim dsPrograms As New dsOvertime
                Dim dtPrograms As New DataTable
                Dim daOvertimePrograms As New SqlDataAdapter(cmOvertime)
                daOvertimePrograms.Fill(dtPrograms)
                cnOvertime.Close()

                'Build a table with one row per program per pay period
                Dim dsOvertime As New dsOvertime

                For Each dr As DataRow In dtPrograms.Rows
                    For i As Int32 = 15 To 26
                        Dim newPayPeriodRow As DataRow = dsOvertime.Tables("dtPayPeriodPrograms").NewRow()
                        newPayPeriodRow("YearAndPayPeriod") = BeginningCalendarYear & " " & i.ToString
                        newPayPeriodRow("PayPeriod") = i
                        newPayPeriodRow("ID") = dr("ID")
                        newPayPeriodRow("Program") = dr("DataValue")
                        newPayPeriodRow("DisplayOrder") = dr("DisplayOrder")
                        dsOvertime.Tables("dtPayPeriodPrograms").Rows.Add(newPayPeriodRow)
                    Next
                    For i As Int32 = 1 To 14
                        Dim newPayPeriodRow As DataRow = dsOvertime.Tables("dtPayPeriodPrograms").NewRow()
                        If i < 10 Then
                            newPayPeriodRow("YearAndPayPeriod") = EndingCalendarYear & " 0" & i.ToString
                        Else
                            newPayPeriodRow("YearAndPayPeriod") = EndingCalendarYear & " " & i.ToString
                        End If
                        newPayPeriodRow("PayPeriod") = i
                        newPayPeriodRow("ID") = dr("ID")
                        newPayPeriodRow("Program") = dr("DataValue")
                        newPayPeriodRow("DisplayOrder") = dr("DisplayOrder")
                        dsOvertime.Tables("dtPayPeriodPrograms").Rows.Add(newPayPeriodRow)
                    Next
                Next
                Dim dvPayPeriodPrograms As DataView
                dvPayPeriodPrograms = dsOvertime.Tables("dtPayPeriodPrograms").DefaultView
                dvPayPeriodPrograms.Sort = "DisplayOrder"

                'Now plug in the overtime hours 
                Dim cmOvertime2 As New SqlCommand("uspGetOvertime", cnOvertime)
                cmOvertime2.CommandType = CommandType.StoredProcedure
                Dim paramFiscalyear As New SqlParameter("@tlkpOtFY", SqlDbType.Int)
                paramFiscalyear.Value = tlkpOtFY
                Dim paramOffice As New SqlParameter("@tlkpOtOffice", SqlDbType.Int)
                paramOffice.Value = tlkpOtOffice
                cmOvertime2.Parameters.Add(paramFiscalyear)
                cmOvertime2.Parameters.Add(paramOffice)
                cnOvertime.Open()
                Dim daOverTime As New SqlDataAdapter(cmOvertime2)
                daOverTime.Fill(dsOvertime.Tables("dtOvertime"))

                For Each drPayPeriodProgram As dsOvertime.dtPayPeriodProgramsRow In dsOvertime.dtPayPeriodPrograms.Rows
                    'Maybe one or no rows with overtime for each pay period/program row
                    For Each drOvertime As dsOvertime.dtOvertimeRow In drPayPeriodProgram.GetChildRows("dtPayPeriodPrograms_dtOvertime")
                        drPayPeriodProgram.OTHours = drOvertime.OTHours
                    Next
                Next

                cnOvertime.Close()
                Return dvPayPeriodPrograms
            Catch ex As Exception
                Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)>
        Shared Function GetOvertimeLookups(ByVal HistOrCurr As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeEntryPrograms", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmHistOrCurr As New SqlParameter("@HistOrCurr", HistOrCurr)
            cm.Parameters.Add(parmHistOrCurr)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Dim dr As DataRow
            dr = dtLookups.NewRow()
            dr.Item("ID") = 0
            dr.Item("Category") = "Overtime"
            dr.Item("SubCategory") = DBNull.Value
            dr.Item("DataValue") = String.Empty
            dr.Item("Description") = String.Empty
            dr.Item("Active") = 1
            dr.Item("DisplayOrder") = DBNull.Value
            dr.Item("ModifiedBy") = String.Empty
            dr.Item("ModifiedDate") = DBNull.Value
            dtLookups.Rows.InsertAt(dr, 0)
            Return dtLookups
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetOvertimeLookupsNoBlankRow(ByVal SubCategory As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeLookups", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmCategory As New SqlParameter("@SubCategory", SubCategory)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetOvertimeCalendarYearValue(ByVal Year As String) As Int32
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeCalendarYearValue", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmYear As New SqlParameter("@Year", Year)
            cm.Parameters.Add(parmYear)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups.Rows(0).Item(0)
        End Function

        '<System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Public Shared Function InsertUpdateOvertime(ByVal tlkpOtFY As Int32, ByVal tlkpOtOffice As Integer, ByVal tlkpOtYear As Integer,
                                       ByVal PayPeriod As Integer, ByVal tlkpOtProgram As Integer, ByVal OTHours As Decimal,
                                       ByVal ModifiedBy As String) As Int32

            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertUpdateOvertime", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmOtFY As New SqlParameter("@tlkpOtFY", SqlDbType.Int)
            Dim parmOtOffice As New SqlParameter("@tlkpOtOffice", SqlDbType.Int)
            Dim parmOtYear As New SqlParameter("@tlkpOtYear", SqlDbType.Int)
            Dim parmPayPeriod As New SqlParameter("@PayPeriod", SqlDbType.Int)
            Dim parmOtProgram As New SqlParameter("@tlkpOtProgram", SqlDbType.Int)
            Dim parmOtHours As New SqlParameter("@OTHours", SqlDbType.Decimal)
            Dim parmModifiedBy As New SqlParameter("@ModifiedBy", SqlDbType.VarChar, 6)
            parmOtFY.Value = tlkpOtFY
            parmOtOffice.Value = tlkpOtOffice
            parmOtYear.Value = tlkpOtYear
            parmOtHours.Value = OTHours
            parmPayPeriod.Value = PayPeriod
            parmOtProgram.Value = tlkpOtProgram
            parmModifiedBy.Value = ModifiedBy
            cm.Parameters.Add(parmOtFY)
            cm.Parameters.Add(parmOtOffice)
            cm.Parameters.Add(parmOtYear)
            cm.Parameters.Add(parmPayPeriod)
            cm.Parameters.Add(parmOtProgram)
            cm.Parameters.Add(parmOtHours)
            cm.Parameters.Add(parmModifiedBy)
            Try
                cn.Open()

                cm.ExecuteNonQuery()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                cn.Close()
            End Try

            Return 0
        End Function

        Public Shared Function GetOvertimeFiscalYear(ByVal Fiscalyear As String) As Int32
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeFiscalYear", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmCategory As New SqlParameter("@FiscalYear", Fiscalyear)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups.Rows(0).Item("ID")
        End Function

        'Public Shared Function GetOvertimeFirstOffice(EmployeeID As String) As Integer
        '    Dim OverTimeOffice As Integer
        '    Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
        '    Dim cm As New SqlCommand("uspGetOvertimeFirstOffice", cn)
        '    cm.CommandType = CommandType.StoredProcedure
        '    Dim parmEmplID As New SqlParameter("@EmplID", EmployeeID)
        '    cm.Parameters.Add(parmEmplID)
        '    cn.Open()

        '    Dim dtLookups As New DataTable
        '    Dim da As New SqlDataAdapter(cm)
        '    da.Fill(dtLookups)
        '    cn.Close()
        '    OverTimeOffice = dtLookups.Rows(0).Item("ID")
        '    Return OverTimeOffice
        '    'Return dtLookups.Rows(0).Item("ID")
        'End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Shared Function GetOvertimeEmployeeOffices(ByVal EmplID As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeEmployeeOffice", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmEmplID As New SqlParameter("@EmplID", EmplID)
            cm.Parameters.Add(parmEmplID)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups
        End Function
    End Class
End Namespace
