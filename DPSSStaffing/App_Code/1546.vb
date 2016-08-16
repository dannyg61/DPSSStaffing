Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Linq
Imports StaffingUtilities

Namespace Req1546DAL
    <System.ComponentModel.DataObject()> _
    Public Class DAL1546

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)>
        Public Function Get1546s(ByVal VacantPosition As ArrayList, ByVal Program As ArrayList, ByVal Office As ArrayList, ByVal PositionStatus As ArrayList,
                                 ByVal PositionNumber As ArrayList, ByVal JobID As ArrayList) As DataTable

            Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

            ' Get the transfers
            Dim cm1546 As New SqlCommand("uspGet1546s", cn1546)
            cm1546.CommandType = CommandType.StoredProcedure

            cn1546.Open()
            Dim ds1546 As New ds1546
            Dim dtAll1546s As New DataTable
            Dim da1546 As New SqlDataAdapter(cm1546)
            da1546.Fill(dtAll1546s)
            cn1546.Close()

            ds1546.dt1546.Merge(Filter1546Data(dtAll1546s, VacantPosition, Program, Office, PositionStatus, PositionNumber, JobID))

            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspDPSSStaffingCurrentInfo", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure

            Dim FirstLineSupervisorEmployeeIds As String = Utilities.FormatFirstLineSupervisorEmployeeIDsXML(ds1546.dt1546)
            Dim SecondLineSupervisorEmployeeIds As String = Utilities.FormatSecondLineSupervisorEmployeeIDsXML(ds1546.dt1546)

            'Get the first line supervisor based on the first line supervisor employee id on the DPSS Staffing database)
            'Dim parmFirstLineSupervisorEmplIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            Dim parmEmployeeIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = FirstLineSupervisorEmployeeIds
            cmEmployee.Parameters.Add(parmEmployeeIDs)
            cnEmployee.Open()
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            daEmployees.Fill(ds1546.dtFirstLineSupervisor)
            cnEmployee.Close()

            'Get the second line supervisor based on the second line supervisor employee id on the DPSS Staffing database
            'Dim parmSecondLineSupervisorEmplIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = SecondLineSupervisorEmployeeIds
            cnEmployee.Open()
            daEmployees.Fill(ds1546.dtSecondLineSupervisor)
            cnEmployee.Close()

            For Each dr1546 As ds1546.dt1546Row In ds1546.dt1546.Rows
                'Should be only one row for supervisors
                For Each drFirstLineSupervisor As ds1546.dtFirstLineSupervisorRow In dr1546.GetChildRows("dt1546_dtFirstLineSupervisor")
                    dr1546.FirstLineSupervisorName = drFirstLineSupervisor.EMPLOYEE_NAME
                Next
                For Each drSecondLineSupervisor As ds1546.dtSecondLineSupervisorRow In dr1546.GetChildRows("dt1546_dtSecondLineSupervisor")
                    dr1546.SecondLineSupervisorName = drSecondLineSupervisor.EMPLOYEE_NAME
                Next
            Next
            Return ds1546.dt1546
        End Function

        Private Function Filter1546Data(ByVal DataTable1546 As DataTable, ByVal VacantPosition As ArrayList, ByVal Program As ArrayList,
                                        ByVal Office As ArrayList, ByVal PositionStatus As ArrayList, ByVal PositionNumber As ArrayList, ByVal JobID As ArrayList) As DataTable

            Dim Filtered1546Table As DataTable = DataTable1546
            'Default to only showing Active requests else combine status criteria with other search criteria
            If VacantPosition.Count = 0 And Program.Count = 0 And Office.Count = 0 And PositionStatus.Count = 0 And PositionNumber.Count = 0 Then
                Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                If query.Count = 0 Then
                    Filtered1546Table.Clear()
                    Return Filtered1546Table
                End If
                Filtered1546Table = query.CopyToDataTable
            End If

            If VacantPosition.Count <> 0 Then
                If PositionStatus.Count = 0 Then
                    'Dim query = _
                    '    From req1546 In Filtered1546Table.AsEnumerable() _
                    '        Where req1546.Field(Of Int32)("tlkpPosition") = VacantPosition _
                    '        And req1546.Field(Of String)("PositionStatus") = "Active" _
                    '        Select req1546
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where VacantPosition.Contains(req1546.Field(Of Int32)("tlkpPosition").ToString) _
                    And req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                Else
                    'Dim query = _
                    '    From req1546 In Filtered1546Table.AsEnumerable() _
                    '        Where req1546.Field(Of Int32)("tlkpPosition") = VacantPosition _
                    '            And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus _
                    '        Select req1546
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where VacantPosition.Contains(req1546.Field(Of Int32)("tlkpPosition").ToString) _
                    And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus(0)
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                End If
            End If
            If Program.Count <> 0 Then
                If PositionStatus.Count = 0 Then
                    'Dim query = _
                    '    From req1546 In Filtered1546Table.AsEnumerable() _
                    '        Where req1546.Field(Of Int32)("tlkpProgram") = Program _
                    '        And req1546.Field(Of String)("PositionStatus") = "Active" _
                    '        Select req1546
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where Program.Contains(req1546.Field(Of Int32)("tlkpProgram").ToString) _
                    And req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                Else
                    'Dim query = _
                    '   From req1546 In Filtered1546Table.AsEnumerable() _
                    '       Where req1546.Field(Of Int32)("tlkpProgram") = Program _
                    '            And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus _
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where Program.Contains(req1546.Field(Of Int32)("tlkpProgram").ToString) _
                    And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus(0)
                    Select req1546
                    '       Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                End If
            End If
            If Office.Count <> 0 Then
                If PositionStatus.Count = 0 Then
                    'Dim query = _
                    '       From req1546 In Filtered1546Table.AsEnumerable() _
                    '           Where req1546.Field(Of Int32)("tlkpLocation") = Office _
                    '            And req1546.Field(Of String)("PositionStatus") = "Active" _
                    '           Select req1546
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where Office.Contains(req1546.Field(Of Int32)("tlkpLocation").ToString) _
                    And req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                Else
                    'Dim query = _
                    '       From req1546 In Filtered1546Table.AsEnumerable() _
                    '           Where req1546.Field(Of Int32)("tlkpLocation") = Office _
                    '            And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus _
                    '           Select req1546
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where Office.Contains(req1546.Field(Of Int32)("tlkpLocation").ToString) _
                    And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus(0)
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                End If
            End If

            If PositionNumber.Count <> 0 Then

                If PositionStatus.Count = 0 Then
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where PositionNumber.Contains(req1546.Field(Of String)("PositionNumber").ToString) _
                    And req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                Else
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where PositionNumber.Contains(req1546.Field(Of String)("PositionNumber").ToString) _
                    And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus(0)
                    Select req1546
                    '       Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                End If
            End If

            If JobID.Count <> 0 Then

                If PositionStatus.Count = 0 Then
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where JobID.Contains(req1546.Field(Of String)("JobID").ToString) _
                    And req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                Else
                    Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where JobID.Contains(req1546.Field(Of String)("JobID").ToString) _
                    And req1546.Field(Of Int32)("tlkpStatus") = PositionStatus(0)
                    Select req1546
                    '       Select req1546
                    If query.Count = 0 Then
                        Filtered1546Table.Clear()
                        Return Filtered1546Table
                    End If
                    Filtered1546Table = query.CopyToDataTable
                End If
            End If


            If PositionStatus.Count <> 0 Then
                'Dim query = _
                '       From req1546 In Filtered1546Table.AsEnumerable() _
                '           Where req1546.Field(Of Int32)("tlkpStatus") = PositionStatus _
                '           Select req1546
                Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where req1546.Field(Of Int32)("tlkpStatus") = PositionStatus(0)
                    Select req1546
                If query.Count = 0 Then
                    Filtered1546Table.Clear()
                    Return Filtered1546Table
                End If
                Filtered1546Table = query.CopyToDataTable
            Else
                Dim query =
                    From req1546 In Filtered1546Table.AsEnumerable()
                    Where req1546.Field(Of String)("PositionStatus") = "Active"
                    Select req1546
                If query.Count = 0 Then
                    Filtered1546Table.Clear()
                    Return Filtered1546Table
                End If
                Filtered1546Table = query.CopyToDataTable
            End If



            ' Filter for showing filled requests or not based on user selection
            'If ShowFilled = False Then
            '    Dim query = _
            '           From transfer In Filtered1546Table.AsEnumerable() _
            '               Where transfer.Field(Of Boolean)("PositionStatus") = "Filled" _
            '               Select transfer
            '    If query.Count = 0 Then
            '        Filtered1546Table.Clear()
            '        Return Filtered1546Table
            '    End If
            '    Filtered1546Table = query.CopyToDataTable
            'End If


            Return Filtered1546Table
        End Function

        Public Shared Function CheckForDuplicate1546(ByVal Request1546ID As Int32, ByVal PositionNumber As String, ByVal InsertOrUpdate As String) As Boolean
            Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm1546 As New SqlCommand("uspGet1546CountByPositionNumber", cn1546)
            cm1546.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@Request1546ID", SqlDbType.Int)
            Dim parmPositionNumber As New SqlParameter("@PositionNumber", SqlDbType.VarChar, 6)
            Dim parmInsertOrUpdate As New SqlParameter("@InsertOrUpdate", SqlDbType.VarChar, 6)
            parmID.Value = Request1546ID
            parmPositionNumber.Value = PositionNumber
            parmInsertOrUpdate.Value = InsertOrUpdate
            cm1546.Parameters.Add(parmID)
            cm1546.Parameters.Add(parmPositionNumber)
            cm1546.Parameters.Add(parmInsertOrUpdate)
            cn1546.Open()
            Dim dupeCount As Int32
            Try
                dupeCount = cm1546.ExecuteScalar()
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                cn1546.Close()
            End Try
            If dupeCount > 0 Then
                Return True
            End If
            Return False
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)>
        Public Function Insert1546(ByVal RequestDate As Date, ByVal Bilingual As Boolean, ByVal PositionNumber As String,
                                   ByVal tlkpPosition As Integer, ByVal tlkpStatus As Integer,
                                   ByVal DateToExecs As Date, ByVal DateToHR As Date,
                                   ByVal tlkpLocation As Integer, ByVal tlkpProgram As Integer,
                                   ByVal FirstLineSupervisorEmplID As String, ByVal SecondLineSupervisorEmplID As String,
                                   ByVal Comments As String, ByVal ContactTypeIDs As ArrayList,
                                   ByVal ContactNumber As String, ByVal JobID As String) As Int32

            'Dim xmlContactPhoneNumbers As String = String.Empty
            '' Convert Contact Numbers to xml to pass to stored procedure for multiple inserts
            'If Not ContactPhoneNumbers Is Nothing Then
            '    xmlContactPhoneNumbers = Utilities.FormatContactPhoneNumbersXML(ContactTypeIDs, ContactPhoneNumbers)
            'End If
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsert1546", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmRequestDate As New SqlParameter("@RequestDate", SqlDbType.Date)
            Dim parmBilingual As New SqlParameter("@Bilingual", SqlDbType.Bit)
            Dim parmPositionNumber As New SqlParameter("@PositionNumber", SqlDbType.VarChar)
            Dim parmtlkpPosition As New SqlParameter("@tlkpPosition", SqlDbType.Int)
            Dim parmtlkpStatus As New SqlParameter("@tlkpStatus", SqlDbType.Int)
            Dim parmDateToExecs As New SqlParameter("@DateToExecs", SqlDbType.Date)
            Dim parmDateToHR As New SqlParameter("@DateToHR", SqlDbType.Date)
            Dim parmtlkpLocation As New SqlParameter("@tlkpLocation", SqlDbType.Int)
            Dim parmtlkpProgram As New SqlParameter("@tlkpProgram", SqlDbType.Int)
            Dim parmFirstLineSupervisorEmplID As New SqlParameter("@FirstLineSupervisorEmplID", SqlDbType.VarChar)
            Dim parmSecondLineSupervisorEmplID As New SqlParameter("@SecondLineSupervisorEmplID", SqlDbType.VarChar)
            Dim parmComments As New SqlParameter("@Comments", SqlDbType.VarChar, 500)
            Dim parmContactTypeIDs As New SqlParameter("@ContactTypeIDs", SqlDbType.Xml)
            Dim parmContactNumber As New SqlParameter("@ContactNumber", SqlDbType.VarChar, 10)
            Dim parmJobID As New SqlParameter("@JobID", SqlDbType.VarChar, 20)
            If RequestDate = "#12:00:00 AM#" Then
                parmRequestDate.Value = DBNull.Value
            Else
                parmRequestDate.Value = RequestDate
            End If
            parmBilingual.Value = Bilingual
            If PositionNumber Is Nothing Then
                parmPositionNumber.Value = DBNull.Value
            Else
                parmPositionNumber.Value = PositionNumber
            End If
            If tlkpPosition = 0 Then
                parmtlkpPosition.Value = DBNull.Value
            Else
                parmtlkpPosition.Value = tlkpPosition
            End If
            If tlkpStatus = 0 Then
                parmtlkpStatus.Value = DBNull.Value
            Else
                parmtlkpStatus.Value = tlkpStatus
            End If
            If DateToExecs = "#12:00:00 AM#" Then
                parmDateToExecs.Value = DBNull.Value
            Else
                parmDateToExecs.Value = DateToExecs
            End If
            If DateToHR = "#12:00:00 AM#" Then
                parmDateToHR.Value = DBNull.Value
            Else
                parmDateToHR.Value = DateToHR
            End If
            If tlkpLocation = 0 Then
                parmtlkpLocation.Value = DBNull.Value
            Else
                parmtlkpLocation.Value = tlkpLocation
            End If
            If tlkpProgram = 0 Then
                parmtlkpProgram.Value = DBNull.Value
            Else
                parmtlkpProgram.Value = tlkpProgram
            End If
            If FirstLineSupervisorEmplID Is Nothing Then
                parmFirstLineSupervisorEmplID.Value = DBNull.Value
            Else
                parmFirstLineSupervisorEmplID.Value = FirstLineSupervisorEmplID
            End If
            If SecondLineSupervisorEmplID Is Nothing Then
                parmSecondLineSupervisorEmplID.Value = DBNull.Value
            Else
                parmSecondLineSupervisorEmplID.Value = SecondLineSupervisorEmplID
            End If
            If Comments Is Nothing Then
                parmComments.Value = String.Empty
            Else
                parmComments.Value = Comments
            End If
            If ContactNumber = String.Empty Then
                parmContactNumber.Value = DBNull.Value
            Else
                parmContactNumber.Value = ContactNumber
            End If
            If JobID Is Nothing Then
                parmJobID.Value = DBNull.Value
            ElseIf JobID = String.Empty Then
                parmJobID.Value = String.Empty
            Else
                parmJobID.Value = JobID
            End If
            cm.Parameters.Add(parmRequestDate)
            cm.Parameters.Add(parmBilingual)
            cm.Parameters.Add(parmPositionNumber)
            cm.Parameters.Add(parmtlkpPosition)
            cm.Parameters.Add(parmtlkpStatus)
            cm.Parameters.Add(parmDateToExecs)
            cm.Parameters.Add(parmDateToHR)
            cm.Parameters.Add(parmtlkpLocation)
            cm.Parameters.Add(parmtlkpProgram)
            cm.Parameters.Add(parmFirstLineSupervisorEmplID)
            cm.Parameters.Add(parmSecondLineSupervisorEmplID)
            cm.Parameters.Add(parmComments)
            cm.Parameters.Add(parmContactNumber)
            cm.Parameters.Add(parmJobID)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)>
        Public Function Update1546(ByVal RequestDate As Date, ByVal Bilingual As Boolean, ByVal PositionNumber As String,
                                   ByVal tlkpPosition As Integer, ByVal tlkpStatus As Integer,
                                   ByVal DateToExecs As Date, ByVal DateToHR As Date,
                                   ByVal tlkpLocation As Integer, ByVal tlkpProgram As Integer,
                                   ByVal FirstLineSupervisorEmplID As String, ByVal SecondLineSupervisorEmplID As String,
                                   ByVal Comments As String,
                                   ByVal Original_ID As Integer, ByVal ContactTypeIDs As ArrayList,
                                   ByVal ContactNumber As String, ByVal JobID As String) As Int32


            'Dim xmlContactPhoneNumbers As String = String.Empty
            '' Convert Contact Numbers to xml to pass to stored procedure for multiple inserts
            'If Not ContactPhoneNumbers Is Nothing Then
            '    xmlContactPhoneNumbers = Utilities.FormatContactPhoneNumbersXML(ContactTypeIDs, ContactPhoneNumbers)
            'End If

            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdate1546", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            Dim parmRequestDate As New SqlParameter("@RequestDate", SqlDbType.Date)
            Dim parmBilingual As New SqlParameter("@Bilingual", SqlDbType.Bit)
            Dim parmPositionNumber As New SqlParameter("@PositionNumber", SqlDbType.VarChar)
            Dim parmtlkpPosition As New SqlParameter("@tlkpPosition", SqlDbType.Int)
            Dim parmtlkpStatus As New SqlParameter("@tlkpStatus", SqlDbType.Int)
            Dim parmDateToExecs As New SqlParameter("@DateToExecs", SqlDbType.Date)
            Dim parmDateToHR As New SqlParameter("@DateToHR", SqlDbType.Date)
            Dim parmtlkpLocation As New SqlParameter("@tlkpLocation", SqlDbType.Int)
            Dim parmtlkpProgram As New SqlParameter("@tlkpProgram", SqlDbType.Int)
            Dim parmFirstLineSupervisorEmplID As New SqlParameter("@FirstLineSupervisorEmplID", SqlDbType.VarChar, 6)
            Dim parmSecondLineSupervisorEmplID As New SqlParameter("@SecondLineSupervisorEmplID", SqlDbType.VarChar, 6)
            Dim parmComments As New SqlParameter("@Comments", SqlDbType.VarChar, 500)
            Dim parmContactTypeIDs As New SqlParameter("@ContactTypeIDs", SqlDbType.Xml)
            Dim parmContactNumber As New SqlParameter("@ContactNumber", SqlDbType.VarChar, 10)
            Dim parmJobID As New SqlParameter("@JobID", SqlDbType.VarChar, 20)
            parmID.Value = Original_ID
            If RequestDate = "#12:00:00 AM#" Then
                parmRequestDate.Value = DBNull.Value
            Else
                parmRequestDate.Value = RequestDate
            End If
            parmBilingual.Value = Bilingual
            If PositionNumber Is Nothing Then
                parmPositionNumber.Value = DBNull.Value
            Else
                parmPositionNumber.Value = PositionNumber
            End If
            If tlkpPosition = 0 Then
                parmtlkpPosition.Value = DBNull.Value
            Else
                parmtlkpPosition.Value = tlkpPosition
            End If
            If tlkpStatus = 0 Then
                parmtlkpStatus.Value = DBNull.Value
            Else
                parmtlkpStatus.Value = tlkpStatus
            End If
            If DateToExecs = "#12:00:00 AM#" Then
                parmDateToExecs.Value = DBNull.Value
            Else
                parmDateToExecs.Value = DateToExecs
            End If
            If DateToHR = "#12:00:00 AM#" Then
                parmDateToHR.Value = DBNull.Value
            Else
                parmDateToHR.Value = DateToHR
            End If
            If tlkpLocation = 0 Then
                parmtlkpLocation.Value = DBNull.Value
            Else
                parmtlkpLocation.Value = tlkpLocation
            End If
            If tlkpProgram = 0 Then
                parmtlkpProgram.Value = DBNull.Value
            Else
                parmtlkpProgram.Value = tlkpProgram
            End If
            If FirstLineSupervisorEmplID Is Nothing Then
                parmFirstLineSupervisorEmplID.Value = DBNull.Value
            Else
                parmFirstLineSupervisorEmplID.Value = FirstLineSupervisorEmplID
            End If
            If SecondLineSupervisorEmplID Is Nothing Then
                parmSecondLineSupervisorEmplID.Value = DBNull.Value
            Else
                parmSecondLineSupervisorEmplID.Value = SecondLineSupervisorEmplID
            End If
            If Comments Is Nothing Then
                parmComments.Value = String.Empty
            Else
                parmComments.Value = Comments
            End If
            If ContactNumber = String.Empty Then
                parmContactNumber.Value = DBNull.Value
            Else
                parmContactNumber.Value = ContactNumber
            End If
            If JobID Is Nothing Then
                parmJobID.Value = DBNull.Value
            ElseIf JobID = String.Empty Then
                parmJobID.Value = String.Empty
            Else
                parmJobID.Value = JobID
            End If
            cm.Parameters.Add(parmID)
            cm.Parameters.Add(parmRequestDate)
            cm.Parameters.Add(parmBilingual)
            cm.Parameters.Add(parmPositionNumber)
            cm.Parameters.Add(parmtlkpPosition)
            cm.Parameters.Add(parmtlkpStatus)
            cm.Parameters.Add(parmDateToExecs)
            cm.Parameters.Add(parmDateToHR)
            cm.Parameters.Add(parmtlkpLocation)
            cm.Parameters.Add(parmtlkpProgram)
            cm.Parameters.Add(parmFirstLineSupervisorEmplID)
            cm.Parameters.Add(parmSecondLineSupervisorEmplID)
            cm.Parameters.Add(parmComments)
            cm.Parameters.Add(parmContactNumber)
            cm.Parameters.Add(parmJobID)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Delete)> _
        Public Function Delete1546(ByVal Original_ID As Int32) As Int32
            Try
                Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
                Dim cm1546 As New SqlCommand("uspDelete1546", cn1546)
                cm1546.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
                parmID.Value = Original_ID
                cm1546.Parameters.Add(parmID)
                cn1546.Open()
                cm1546.ExecuteNonQuery()
                cn1546.Close()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            End Try
            Return 0
        End Function

        'Shared Function Get1546ContactNumbers(ByVal Req1546ID As Int32) As DataTable
        '    Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

        '    ' Get all of the contact numbers for the 1546
        '    Dim cm1546 As New SqlCommand("uspGet1546ContactNumbers", cn1546)
        '    cm1546.CommandType = CommandType.StoredProcedure
        '    Dim parmID As New SqlParameter("@1546ID", SqlDbType.Int)
        '    parmID.Value = Req1546ID
        '    cm1546.Parameters.Add(parmID)
        '    cn1546.Open()
        '    Dim dtContactNumbers As New ds1546.dtContactNumbersDataTable
        '    Dim daContactNumbers As New SqlDataAdapter(cm1546)
        '    daContactNumbers.Fill(dtContactNumbers)
        '    cn1546.Close()
        '    Return dtContactNumbers
        'End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function Get1546Documents(ByVal Request1546ID As Int32) As DataTable
            Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)


            ' Get all of the 1546s
            Dim cm1546 As New SqlCommand("uspGet1546Documents", cn1546)
            cm1546.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@Request1546ID", SqlDbType.Int)
            parmID.Value = Request1546ID
            cm1546.Parameters.Add(parmID)
            cn1546.Open()
            Dim ds1546 As New ds1546
            Dim da1546Documents As New SqlDataAdapter(cm1546)
            da1546Documents.Fill(ds1546, ds1546.dt1546Documents.TableName)
            cn1546.Close()
            Return ds1546.dt1546Documents
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function Get1546DocumentObject(ByVal ID As Int32) As Data.DataTable
            Dim dt As New DataTable
            Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Try

                Dim cm As New SqlClient.SqlCommand("uspGetDocumentObject", cn1546)
                cm.Parameters.Add(New SqlParameter("@ID", Data.SqlDbType.Int))
                cm.Parameters("@ID").Value = ID
                cm.CommandType = CommandType.StoredProcedure

                Dim da As New SqlClient.SqlDataAdapter(cm)
                cn1546.Open()
                da.Fill(dt)
                cn1546.Close()
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If cn1546.State = ConnectionState.Open Then
                    cn1546.Close()
                End If
            End Try
            Return dt

        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Public Function Insert1546Document(ByVal Request1546ID As Integer, ByVal FileName As String, ByVal Data As Byte(), ByVal LoadedBy As String) As Int32
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsert1546Document", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parm1546RequestID As New SqlParameter("@Request1546ID", SqlDbType.Int)
            Dim parmFileName As New SqlParameter("@FileName", SqlDbType.VarChar, 50)
            Dim parmData As New SqlParameter("@Data", SqlDbType.VarBinary)
            Dim parmLoadedBy As New SqlParameter("@LoadedBy", SqlDbType.VarChar, 6)
            parm1546RequestID.Value = Request1546ID
            parmFileName.Value = FileName
            parmData.Value = Data
            parmLoadedBy.Value = LoadedBy
            cm.Parameters.Add(parm1546RequestID)
            cm.Parameters.Add(parmFileName)
            cm.Parameters.Add(parmData)
            cm.Parameters.Add(parmLoadedBy)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Delete)> _
        Public Function Delete1546Document(ByVal Original_ID As Int32) As Int32
            Try
                Dim cn1546 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
                Dim cm1546 As New SqlCommand("uspDeleteDocument", cn1546)
                cm1546.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
                parmID.Value = Original_ID
                cm1546.Parameters.Add(parmID)
                cn1546.Open()
                cm1546.ExecuteNonQuery()
                cn1546.Close()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            End Try
            Return 0
        End Function
    End Class
End Namespace