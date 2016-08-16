Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Linq
Imports StaffingUtilities

Namespace TAPDAL
    <System.ComponentModel.DataObject()> _
    Public Class TAP
        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)>
        Public Function GetTAPs(ByVal Position As ArrayList, ByVal Program As ArrayList, ByVal Office As ArrayList,
                                ByVal BeginActualEndDate As DateTime, ByVal EndActualEndDate As DateTime) As DataTable
            Dim cnTAP As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

            ' Get the transfers
            Dim cmTAP As New SqlCommand("uspGetTAPs", cnTAP)
            cmTAP.CommandType = CommandType.StoredProcedure


            cnTAP.Open()
            Dim dsTAPs As New dsTAPs
            Dim dtAllTAPs As New DataTable
            Dim daTAP As New SqlDataAdapter(cmTAP)
            daTAP.Fill(dtAllTAPs)
            cnTAP.Close()
            dsTAPs.dtTAPs.Merge(FilterTAPData(dtAllTAPs, Position, Program, Office, BeginActualEndDate, EndActualEndDate))

            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspDPSSStaffingCurrentInfo", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure

            Dim FirstLineSupervisorEmployeeIds As String = Utilities.FormatFirstLineSupervisorEmployeeIDsXML(dsTAPs.dtTAPs)
            Dim SecondLineSupervisorEmployeeIds As String = Utilities.FormatSecondLineSupervisorEmployeeIDsXML(dsTAPs.dtTAPs)

            'Get the first line supervisor based on the first line supervisor employee id on the DPSS Staffing database)
            'Dim parmFirstLineSupervisorEmplIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            Dim parmEmployeeIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = FirstLineSupervisorEmployeeIds
            cmEmployee.Parameters.Add(parmEmployeeIDs)
            cnEmployee.Open()
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            daEmployees.Fill(dsTAPs.dtFirstLineSupervisor)
            cnEmployee.Close()

            'Get the second line supervisor based on the second line supervisor employee id on the DPSS Staffing database
            'Dim parmSecondLineSupervisorEmplIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = SecondLineSupervisorEmployeeIds
            cnEmployee.Open()
            daEmployees.Fill(dsTAPs.dtSecondLineSupervisor)
            cnEmployee.Close()

            For Each drTAP As dsTAPs.dtTAPsRow In dsTAPs.dtTAPs.Rows
                'Should be only one row for supervisors
                For Each drFirstLineSupervisor As dsTAPs.dtFirstLineSupervisorRow In drTAP.GetChildRows("dtTAPs_dtFirstLineSupervisor")
                    drTAP.FirstLineSupervisorName = drFirstLineSupervisor.EMPLOYEE_NAME
                Next
                For Each drSecondLineSupervisor As dsTAPs.dtSecondLineSupervisorRow In drTAP.GetChildRows("dtTAPs_dtSecondLineSupervisor")
                    drTAP.SecondLineSupervisorName = drSecondLineSupervisor.EMPLOYEE_NAME
                Next
            Next
            Return dsTAPs.dtTAPs
        End Function

        Private Function FilterTAPData(ByVal DataTableTAP As DataTable, ByVal Position As ArrayList, ByVal Program As ArrayList, Office As ArrayList,
                                       ByVal BeginActualEndDate As DateTime, ByVal EndActualEndDate As DateTime) As DataTable
            Dim FilteredTAPTable As DataTable = DataTableTAP

            'No filter criteria - default to showing all rows where ActualEndDate is null
            If Position.Count = 0 And Program.Count = 0 And Office.Count = 0 And BeginActualEndDate.ToString = "1/1/0001 12:00:00 AM" And EndActualEndDate.ToString = "1/1/0001 12:00:00 AM" Then
                Dim tempTable As DataTable
                tempTable = FilteredTAPTable.Clone
                For Each dr As DataRow In FilteredTAPTable.Rows
                    If dr.Item("ActualEndDate") Is DBNull.Value Then
                        tempTable.ImportRow(dr)
                    End If
                Next
                FilteredTAPTable = tempTable.Copy()
                Return FilteredTAPTable
            End If

            If Position.Count > 0 Then
                'Linq query against ArrayList of values
                Dim query =
                    From TAP In FilteredTAPTable.AsEnumerable()
                    Where Position.Contains(TAP.Field(Of Int32)("tlkpPosition").ToString)
                    Select TAP

                'LINQ query for single value where criteria
                'Dim query = _
                '    From TAP In FilteredTAPTable.AsEnumerable() _
                '        Where TAP.Field(Of Int32)("tlkpPosition") = Position
                '        Select TAP
                If query.Count = 0 Then
                    FilteredTAPTable.Clear()
                    Return FilteredTAPTable
                End If
                FilteredTAPTable = query.CopyToDataTable
            End If

            If Program.Count > 0 Then
                'LINQ query against ArrayList of values
                Dim query =
                    From TAP In FilteredTAPTable.AsEnumerable()
                    Where Program.Contains(TAP.Field(Of Int32)("tlkpProgram").ToString)
                    Select TAP

                'LINQ query for single value where criteria
                'Dim query = _
                '    From TAP In FilteredTAPTable.AsEnumerable() _
                '        Where TAP.Field(Of Int32)("tlkpProgram") = Program _
                '        Select TAP

                If query.Count = 0 Then
                    FilteredTAPTable.Clear()
                    Return FilteredTAPTable
                End If
                FilteredTAPTable = query.CopyToDataTable
            End If

            If Office.Count > 0 Then
                'LINQ query against ArrayList of values
                Dim query =
                    From TAP In FilteredTAPTable.AsEnumerable()
                    Where Office.Contains(TAP.Field(Of Int32)("tlkpLocation").ToString)
                    Select TAP

                'LINQ query for single value where criteria
                'Dim query = _
                '       From TAP In FilteredTAPTable.AsEnumerable() _
                '           Where TAP.Field(Of Int32)("tlkpLocation") = Office
                '           Select TAP
                If query.Count = 0 Then
                    FilteredTAPTable.Clear()
                    Return FilteredTAPTable
                End If
                FilteredTAPTable = query.CopyToDataTable
            End If

            If BeginActualEndDate = "1/1/0001 12:00:00 AM" And EndActualEndDate = "1/1/0001 12:00:00 AM" Then
            Else
                Dim tempTable As DataTable
                tempTable = FilteredTAPTable.Clone
                If BeginActualEndDate <> "1/1/0001 12:00:00 AM" And EndActualEndDate = "1/1/0001 12:00:00 AM" Then
                    For Each dr As DataRow In FilteredTAPTable.Rows
                        If Not dr.Item("ActualEndDate") Is DBNull.Value Then
                            If dr.Item("ActualEndDate") >= BeginActualEndDate Then
                                tempTable.ImportRow(dr)
                            End If
                        End If
                    Next
                ElseIf BeginActualEndDate = "1/1/0001 12:00:00 AM" And EndActualEndDate <> "1/1/0001 12:00:00 AM" Then
                    For Each dr As DataRow In FilteredTAPTable.Rows
                        If Not dr.Item("ActualEndDate") Is DBNull.Value Then
                            If dr.Item("ActualEndDate") <= EndActualEndDate Then
                                tempTable.ImportRow(dr)
                            End If
                        End If
                    Next
                ElseIf BeginActualEndDate <> "1/1/0001 12:00:00 AM" And EndActualEndDate <> "1/1/0001 12:00:00 AM" _
                    And BeginActualEndDate <> EndActualEndDate Then
                    For Each dr As DataRow In FilteredTAPTable.Rows
                        If Not dr.Item("ActualEndDate") Is DBNull.Value Then
                            If dr.Item("ActualEndDate") >= BeginActualEndDate And dr.Item("ActualEndDate") <= EndActualEndDate Then
                                tempTable.ImportRow(dr)
                            End If
                        End If
                    Next
                ElseIf BeginActualEndDate = EndActualEndDate Then
                    For Each dr As DataRow In FilteredTAPTable.Rows
                        If Not dr.Item("ActualEndDate") Is DBNull.Value Then
                            If dr.Item("ActualEndDate") = BeginActualEndDate Then
                                tempTable.ImportRow(dr)
                            End If
                        End If
                    Next
                End If
                FilteredTAPTable = tempTable.Copy()
            End If

            'If ActualEndDate.Count > 0 Then
            ''Remove duplicate date filters that the user may have entered
            'Dim distinctActualEndDates As New ArrayList
            'Dim distinctActualEndDateOperators As New ArrayList
            'Dim found As Boolean = False
            'For i As Int32 = 0 To ActualEndDate.Count - 1
            '    found = False
            '    For j As Int32 = 0 To distinctActualEndDates.Count
            '        If (ActualEndDate(i) = distinctActualEndDates(j) And ActualEndDateOperator(i)) Then
            '            found = True
            '        End If
            '    Next
            '    If found = False Then
            '        distinctActualEndDates.Add(ActualEndDate(i))
            '        distinctActualEndDateOperators.Add(ActualEndDateOperator(i))
            '    End If
            'Next

            'Dim tempTable As DataTable
            '    tempTable = FilteredTAPTable.Clone
            '    For i As Int32 = 0 To distinctActualEndDates.Count - 1
            '        Select Case distinctActualEndDateOperators(i)
            '            Case "LessThan"
            '                For Each dr As DataRow In FilteredTAPTable.Rows
            '                    If Not dr.Item("ActualEndDate") Is DBNull.Value Then
            '                        If dr.Item("ActualEndDate") < distinctActualEndDates(i) Then
            '                            tempTable.ImportRow(dr)
            '                        End If
            '                    End If
            '                Next
            '            Case "GreaterThan"
            '                For Each dr As DataRow In FilteredTAPTable.Rows
            '                    If Not dr.Item("ActualEndDate") Is DBNull.Value Then
            '                        If dr.Item("ActualEndDate") > distinctActualEndDates(i) Then
            '                            tempTable.ImportRow(dr)
            '                        End If
            '                    End If
            '                Next
            '            Case "EqualTo"
            '                For Each dr As DataRow In FilteredTAPTable.Rows
            '                    If Not dr.Item("ActualEndDate") Is DBNull.Value Then
            '                        If dr.Item("ActualEndDate") = distinctActualEndDates(i) Then
            '                            tempTable.ImportRow(dr)
            '                        End If
            '                    End If
            '                Next
            '        End Select
            '    Next
            '    FilteredTAPTable = tempTable.Copy()
            'End If


            Return FilteredTAPTable
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Public Function InsertTAP(ByVal RequestDate As Date, ByVal Bilingual As Boolean, _
                                   ByVal tlkpPosition As Integer, ByVal tlkpRequestReason As Integer,
                                   ByVal DateToExecs As Date, ByVal DateToHR As Date,
                                   ByVal FirstName As String, ByVal LastName As String,
                                   ByVal tlkpLocation As Integer, ByVal tlkpProgram As Integer,
                                   ByVal FirstLineSupervisorEmplID As String, ByVal SecondLineSupervisorEmplID As String,
                                   ByVal StartDate As Date, ByVal ActualEndDate As Date,
                                   ByVal Comments As String, ByVal ContactNumber As String, ByVal AntcpEndDate As Date) As Int32

            Dim xmlContactPhoneNumbers As String = String.Empty
            ' Convert Contact Numbers to xml to pass to stored procedure for multiple inserts
            'If Not ContactPhoneNumbers Is Nothing Then
            '    xmlContactPhoneNumbers = Utilities.FormatContactPhoneNumbersXML(ContactTypeIDs, ContactPhoneNumbers)
            'End If
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertTAP", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmRequestDate As New SqlParameter("@RequestDate", SqlDbType.Date)
            Dim parmBilingual As New SqlParameter("@Bilingual", SqlDbType.Bit)
            Dim parmtlkpPosition As New SqlParameter("@tlkpPosition", SqlDbType.Int)
            Dim parmtlkpRequestReason As New SqlParameter("@tlkpRequestReason", SqlDbType.Int)
            Dim parmDateToExecs As New SqlParameter("@DateToExecs", SqlDbType.Date)
            Dim parmDateToHR As New SqlParameter("@DateToHR", SqlDbType.Date)
            Dim parmFirstName As New SqlParameter("@FirstName", SqlDbType.VarChar, 55)
            Dim parmLastName As New SqlParameter("@LastName", SqlDbType.VarChar, 55)
            Dim parmtlkpLocation As New SqlParameter("@tlkpLocation", SqlDbType.Int)
            Dim parmtlkpProgram As New SqlParameter("@tlkpProgram", SqlDbType.Int)
            Dim parmFirstLineSupervisorEmplID As New SqlParameter("@FirstLineSupervisorEmplID", SqlDbType.VarChar)
            Dim parmSecondLineSupervisorEmplID As New SqlParameter("@SecondLineSupervisorEmplID", SqlDbType.VarChar)
            Dim parmComments As New SqlParameter("@Comments", SqlDbType.VarChar, 500)
            Dim parmContactNumber As New SqlParameter("@ContactNumber", SqlDbType.VarChar, 10)
            Dim parmStartDate As New SqlParameter("@StartDate", SqlDbType.Date)
            Dim parmActualEndDate As New SqlParameter("@ActualEndDate", SqlDbType.Date)
            Dim parmAntcpEndDate As New SqlParameter("@AntcpEndDate", SqlDbType.Date)
            If RequestDate = "#12:00:00 AM#" Then
                parmRequestDate.Value = DBNull.Value
            Else
                parmRequestDate.Value = RequestDate
            End If
            parmBilingual.Value = Bilingual
            If tlkpPosition = 0 Then
                parmtlkpPosition.Value = DBNull.Value
            Else
                parmtlkpPosition.Value = tlkpPosition
            End If
            If tlkpRequestReason = 0 Then
                parmtlkpRequestReason.Value = DBNull.Value
            Else
                parmtlkpRequestReason.Value = tlkpRequestReason
            End If
            If FirstName Is Nothing Then
                parmFirstName.Value = DBNull.Value
            Else
                parmFirstName.Value = FirstName
            End If
            If LastName Is Nothing Then
                parmLastName.Value = DBNull.Value
            Else
                parmLastName.Value = LastName
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
            If StartDate = "#12:00:00 AM#" Then
                parmStartDate.Value = DBNull.Value
            Else
                parmStartDate.Value = StartDate
            End If
            If ActualEndDate = "#12:00:00 AM#" Then
                parmActualEndDate.Value = DBNull.Value
            Else
                parmActualEndDate.Value = ActualEndDate
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
            If AntcpEndDate = "#12:00:00 AM#" Then
                parmAntcpEndDate.Value = DBNull.Value
            Else
                parmAntcpEndDate.Value = AntcpEndDate
            End If
            cm.Parameters.Add(parmRequestDate)
            cm.Parameters.Add(parmBilingual)
            cm.Parameters.Add(parmtlkpPosition)
            cm.Parameters.Add(parmtlkpRequestReason)
            cm.Parameters.Add(parmDateToExecs)
            cm.Parameters.Add(parmDateToHR)
            cm.Parameters.Add(parmFirstName)
            cm.Parameters.Add(parmLastName)
            cm.Parameters.Add(parmtlkpLocation)
            cm.Parameters.Add(parmtlkpProgram)
            cm.Parameters.Add(parmFirstLineSupervisorEmplID)
            cm.Parameters.Add(parmSecondLineSupervisorEmplID)
            cm.Parameters.Add(parmComments)
            cm.Parameters.Add(parmContactNumber)
            cm.Parameters.Add(parmStartDate)
            cm.Parameters.Add(parmActualEndDate)
            cm.Parameters.Add(parmAntcpEndDate)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Update)> _
        Public Function UpdateTAP(ByVal Original_ID As Integer, ByVal RequestDate As Date, ByVal Bilingual As Boolean, _
                                   ByVal tlkpPosition As Integer, ByVal tlkpRequestReason As Integer,
                                   ByVal DateToExecs As Date, ByVal DateToHR As Date,
                                   ByVal FirstName As String, ByVal LastName As String,
                                   ByVal tlkpLocation As Integer, ByVal tlkpProgram As Integer,
                                   ByVal FirstLineSupervisorEmplID As String, ByVal SecondLineSupervisorEmplID As String,
                                   ByVal StartDate As Date, ByVal ActualEndDate As Date,
                                   ByVal Comments As String, ByVal ContactNumber As String, ByVal AntcpEndDate As Date) As Int32


            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdateTAP", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            Dim parmRequestDate As New SqlParameter("@RequestDate", SqlDbType.Date)
            Dim parmBilingual As New SqlParameter("@Bilingual", SqlDbType.Bit)
            Dim parmtlkpPosition As New SqlParameter("@tlkpPosition", SqlDbType.Int)
            Dim parmtlkpRequestReason As New SqlParameter("@tlkpRequestReason", SqlDbType.Int)
            Dim parmDateToExecs As New SqlParameter("@DateToExecs", SqlDbType.Date)
            Dim parmDateToHR As New SqlParameter("@DateToHR", SqlDbType.Date)
            Dim parmFirstName As New SqlParameter("@FirstName", SqlDbType.VarChar, 55)
            Dim parmLastName As New SqlParameter("@LastName", SqlDbType.VarChar, 55)
            Dim parmtlkpLocation As New SqlParameter("@tlkpLocation", SqlDbType.Int)
            Dim parmtlkpProgram As New SqlParameter("@tlkpProgram", SqlDbType.Int)
            Dim parmFirstLineSupervisorEmplID As New SqlParameter("@FirstLineSupervisorEmplID", SqlDbType.VarChar)
            Dim parmSecondLineSupervisorEmplID As New SqlParameter("@SecondLineSupervisorEmplID", SqlDbType.VarChar)
            Dim parmComments As New SqlParameter("@Comments", SqlDbType.VarChar, 500)
            Dim parmContactTypeIDs As New SqlParameter("@ContactTypeIDs", SqlDbType.Xml)
            Dim parmContactNumber As New SqlParameter("@ContactNumber", SqlDbType.VarChar, 10)
            Dim parmStartDate As New SqlParameter("@StartDate", SqlDbType.Date)
            Dim parmActualEndDate As New SqlParameter("@ActualEndDate", SqlDbType.Date)
            Dim parmAntcpEndDate As New SqlParameter("@AntcpEndDate", SqlDbType.Date)
            parmID.Value = Original_ID
            If RequestDate = "#12:00:00 AM#" Then
                parmRequestDate.Value = DBNull.Value
            Else
                parmRequestDate.Value = RequestDate
            End If
            parmBilingual.Value = Bilingual
            If tlkpPosition = 0 Then
                parmtlkpPosition.Value = DBNull.Value
            Else
                parmtlkpPosition.Value = tlkpPosition
            End If
            If tlkpRequestReason = 0 Then
                parmtlkpRequestReason.Value = DBNull.Value
            Else
                parmtlkpRequestReason.Value = tlkpRequestReason
            End If
            If FirstName Is Nothing Then
                parmFirstName.Value = DBNull.Value
            Else
                parmFirstName.Value = FirstName
            End If
            If LastName Is Nothing Then
                parmLastName.Value = DBNull.Value
            Else
                parmLastName.Value = LastName
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
            If StartDate = "#12:00:00 AM#" Then
                parmStartDate.Value = DBNull.Value
            Else
                parmStartDate.Value = StartDate
            End If
            If ActualEndDate = "#12:00:00 AM#" Then
                parmActualEndDate.Value = DBNull.Value
            Else
                parmActualEndDate.Value = ActualEndDate
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
            If AntcpEndDate = "#12:00:00 AM#" Then
                parmAntcpEndDate.Value = DBNull.Value
            Else
                parmAntcpEndDate.Value = AntcpEndDate
            End If
            cm.Parameters.Add(parmID)
            cm.Parameters.Add(parmRequestDate)
            cm.Parameters.Add(parmBilingual)
            cm.Parameters.Add(parmtlkpPosition)
            cm.Parameters.Add(parmtlkpRequestReason)
            cm.Parameters.Add(parmDateToExecs)
            cm.Parameters.Add(parmDateToHR)
            cm.Parameters.Add(parmFirstName)
            cm.Parameters.Add(parmLastName)
            cm.Parameters.Add(parmtlkpLocation)
            cm.Parameters.Add(parmtlkpProgram)
            cm.Parameters.Add(parmFirstLineSupervisorEmplID)
            cm.Parameters.Add(parmSecondLineSupervisorEmplID)
            cm.Parameters.Add(parmComments)
            cm.Parameters.Add(parmContactNumber)
            cm.Parameters.Add(parmStartDate)
            cm.Parameters.Add(parmActualEndDate)
            cm.Parameters.Add(parmAntcpEndDate)
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
        Public Function DeleteTAP(ByVal Original_ID As Int32) As Int32
            Try
                Dim cnTAP As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
                Dim cmTAP As New SqlCommand("uspDeleteTAP", cnTAP)
                cmTAP.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
                parmID.Value = Original_ID
                cmTAP.Parameters.Add(parmID)
                cnTAP.Open()
                cmTAP.ExecuteNonQuery()
                cnTAP.Close()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            End Try
            Return 0
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTAPExtensions(ByVal tbl1546ID As Int32) As DataTable
            Dim cnDPSSStaffing As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

            Dim cmTAP As New SqlCommand("uspGetTAPExtensions", cnDPSSStaffing)
            cmTAP.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@tbl1546ID", SqlDbType.Int)
            parmID.Value = tbl1546ID
            cmTAP.Parameters.Add(parmID)
            cnDPSSStaffing.Open()
            Dim dsTAPs As New dsTAPs
            Dim daTAPs As New SqlDataAdapter(cmTAP)
            daTAPs.Fill(dsTAPs, dsTAPs.dtExtensions.TableName)
            cnDPSSStaffing.Close()
            Return dsTAPs.dtExtensions
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Public Function InsertTAPExtension(ByVal tbl1546ID As Int32, ByVal ExtStartDate As Date,
                                           ByVal AntcpEndDate As Date, ByVal tlkpExtensionApproval As Int32, ByVal DateToExecs As Date,
                                           ByVal DateToHR As Date) As Int32
            Try
                Dim cnTAP As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

                Dim cmTAP As New SqlCommand("uspInsertTAPExtension", cnTAP)
                cmTAP.CommandType = CommandType.StoredProcedure
                Dim parm1546ID As New SqlParameter("@tbl1546ID", SqlDbType.Int)
                Dim parmExtStartDate As New SqlParameter("@ExtStartDate", SqlDbType.DateTime)
                Dim parmAntcpEndDate As New SqlParameter("@AntcpEndDate", SqlDbType.DateTime)
                Dim parmDateToExecs As New SqlParameter("@DateToExecs", SqlDbType.DateTime)
                Dim parmDateToHR As New SqlParameter("@DateToHR", SqlDbType.DateTime)
                Dim parmExtensionApproval As New SqlParameter("@tlkpExtensionApproval", SqlDbType.Int)
                parm1546ID.Value = tbl1546ID
                If ExtStartDate = "#12:00:00 AM#" Then
                    parmExtStartDate.Value = DBNull.Value
                Else
                    parmExtStartDate.Value = ExtStartDate
                End If
                If AntcpEndDate = "#12:00:00 AM#" Then
                    parmAntcpEndDate.Value = DBNull.Value
                Else
                    parmAntcpEndDate.Value = AntcpEndDate
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
                If tlkpExtensionApproval = 0 Then
                    parmExtensionApproval.Value = DBNull.Value
                Else
                    parmExtensionApproval.Value = tlkpExtensionApproval
                End If
                cmTAP.Parameters.Add(parm1546ID)
                cmTAP.Parameters.Add(parmExtStartDate)
                cmTAP.Parameters.Add(parmAntcpEndDate)
                cmTAP.Parameters.Add(parmExtensionApproval)
                cmTAP.Parameters.Add(parmDateToExecs)
                cmTAP.Parameters.Add(parmDateToHR)
                cnTAP.Open()

                cmTAP.ExecuteNonQuery()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            End Try
            Return 0
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Update)> _
        Public Function UpdateTAPExtension(ByVal Original_ID As Integer, ByVal original_tbl1546ID As Int32, ByVal ExtStartDate As Date,
                                           ByVal AntcpEndDate As Date, ByVal tlkpExtensionApproval As Int32, ByVal DateToExecs As Date,
                                           ByVal DateToHR As Date) As Int32
            Try
                Dim cnTAP As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

                Dim cmTAP As New SqlCommand("uspUpdateTAPExtension", cnTAP)
                cmTAP.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
                Dim parmExtStartDate As New SqlParameter("@ExtStartDate", SqlDbType.DateTime)
                Dim parmAntcpEndDate As New SqlParameter("@AntcpEndDate", SqlDbType.DateTime)
                Dim parmExtensionApproval As New SqlParameter("@tlkpExtensionApproval", SqlDbType.Int)
                Dim parmDateToExecs As New SqlParameter("@DateToExecs", SqlDbType.DateTime)
                Dim parmDateToHR As New SqlParameter("@DateToHR", SqlDbType.DateTime)
                parmID.Value = Original_ID
                If ExtStartDate = "#12:00:00 AM#" Then
                    parmExtStartDate.Value = DBNull.Value
                Else
                    parmExtStartDate.Value = ExtStartDate
                End If
                If AntcpEndDate = "#12:00:00 AM#" Then
                    parmAntcpEndDate.Value = DBNull.Value
                Else
                    parmAntcpEndDate.Value = AntcpEndDate
                End If
                If tlkpExtensionApproval = 0 Then
                    parmExtensionApproval.Value = DBNull.Value
                Else
                    parmExtensionApproval.Value = tlkpExtensionApproval
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
                cmTAP.Parameters.Add(parmID)
                cmTAP.Parameters.Add(parmExtStartDate)
                cmTAP.Parameters.Add(parmAntcpEndDate)
                cmTAP.Parameters.Add(parmExtensionApproval)
                cmTAP.Parameters.Add(parmDateToExecs)
                cmTAP.Parameters.Add(parmDateToHR)
                cnTAP.Open()

                cmTAP.ExecuteNonQuery()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            End Try
            Return 0
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Delete)> _
        Public Function DeleteTAPExtension(ByVal Original_ID As Int32, ByVal original_tbl1546ID As Int32) As Int32
            Try
                Dim cnTAP As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
                Dim cmTAP As New SqlCommand("uspDeleteTAPExtension", cnTAP)
                cmTAP.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
                parmID.Value = Original_ID
                cmTAP.Parameters.Add(parmID)
                cnTAP.Open()
                cmTAP.ExecuteNonQuery()
                cnTAP.Close()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            End Try
            Return 0
        End Function
    End Class
End Namespace
