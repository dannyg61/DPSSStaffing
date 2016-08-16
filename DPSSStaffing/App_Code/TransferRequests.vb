Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Linq
Imports StaffingUtilities

Namespace StaffTransferRequests
    <System.ComponentModel.DataObject()> _
    Public Class TransferRequests

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTransfers(ByVal CurrentPosition As ArrayList, ByVal CurrentProgram As ArrayList, ByVal RequestedProgram As ArrayList, _
                                 ByVal CurrentOffice As ArrayList, ByVal RequestedOffice As ArrayList, ByVal ShowCanceled As Boolean) As DataTable
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)

            ' Get the transfers
            Dim cmTransfer As New SqlCommand("uspGetTransfers", cnTransfer)
            cmTransfer.CommandType = CommandType.StoredProcedure

            cnTransfer.Open()
            Dim dsTransfers As New dsTransfers
            Dim dtAllTransfers As New DataTable
            Dim daTransfers As New SqlDataAdapter(cmTransfer)
            daTransfers.Fill(dtAllTransfers)
            cnTransfer.Close()
            dsTransfers.dtTransfers.Merge(FilterTransferData(dtAllTransfers, CurrentPosition, CurrentProgram, RequestedProgram, CurrentOffice, RequestedOffice, ShowCanceled))

            'Get all of the employee information for that have transfers in the above table

            Dim EmployeeIds As String = Utilities.FormatEmployeeIDsXML(dsTransfers.dtTransfers)
            Dim FirstLineSupervisorEmployeeIds As String = Utilities.FormatFirstLineSupervisorEmployeeIDsXML(dsTransfers.dtTransfers)
            Dim SecondLineSupervisorEmployeeIds As String = Utilities.FormatSecondLineSupervisorEmployeeIDsXML(dsTransfers.dtTransfers)

            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspDPSSStaffingCurrentInfo", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure

            'Get all of the employee data from the Employee Database for each transfer request
            Dim parmEmployeeIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = EmployeeIds
            cmEmployee.Parameters.Add(parmEmployeeIDs)
            cnEmployee.Open()
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            daEmployees.Fill(dsTransfers, dsTransfers.dtEmployees.TableName)
            cnEmployee.Close()

            'Get the first line supervisor based on the first line supervisor employee id on the DPSS Staffing database)
            'Dim parmFirstLineSupervisorEmplIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = FirstLineSupervisorEmployeeIds
            cnEmployee.Open()
            daEmployees.Fill(dsTransfers, dsTransfers.dtFirstLineSupervisor.TableName)
            cnEmployee.Close()

            'Get the second line supervisor based on the second line supervisor employee id on the DPSS Staffing database
            'Dim parmSecondLineSupervisorEmplIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = SecondLineSupervisorEmployeeIds
            cnEmployee.Open()
            daEmployees.Fill(dsTransfers, dsTransfers.dtSecondLineSupervisor.TableName)
            cnEmployee.Close()

            'Combine the transfers and employee information into one table to bind to the grid
            For Each drTransfer As DataRow In dsTransfers.dtTransfers.Rows
                For Each drEmployee As DataRow In drTransfer.GetChildRows("dtTransfers_dtEmployees")
                    Dim drTransferDisplayData As DataRow
                    drTransferDisplayData = dsTransfers.dtTransferDisplayData.NewRow()
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.IDColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.IDColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.EmplIDColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.EmplIDColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpRequestReasonColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpRequestReasonColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.RequestDTColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.RequestDTColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.ExpirationDTColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.ExpirationDTColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.ToHRDTColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.ToHRDTColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.FromHRDTColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.FromHRDTColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpOfficeColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpOfficeColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpProgramColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpProgramColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.CancelledColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.CancelledColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpCancelReasonColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpCancelReasonColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.TransferDTColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.TransferDTColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpTransLocationColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpTransLocationColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.CommentsColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.CommentsColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.RequestReasonColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.RequestReasonColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.CurrentProgramColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.CurrentProgramColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.CurrentOfficeColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.CurrentOfficeColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.TransferLocationColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.TransferLocationColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.CancelReasonColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.CancelReasonColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.FIRST_NAMEColumn.ColumnName) = drEmployee(dsTransfers.dtEmployees.FIRST_NAMEColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.LAST_NAMEColumn.ColumnName) = drEmployee(dsTransfers.dtEmployees.LAST_NAMEColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.OasisPositionColumn.ColumnName) = drEmployee(dsTransfers.dtEmployees.DESCRColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.EMPLOYEE_NAMEColumn.ColumnName) = drEmployee(dsTransfers.dtEmployees.EMPLOYEE_NAMEColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.OasisFirstLineSupvrColumn.ColumnName) = drEmployee(dsTransfers.dtEmployees.OasisFirstLineSupvrColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.OasisSecondLineSupvrColumn.ColumnName) = drEmployee(dsTransfers.dtEmployees.OasisSecondLineSupvrColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.RequestProgramColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.RequestProgramColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpReqProgramColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpReqProgramColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.tlkpPositionColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.tlkpPositionColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.CurrentPositionColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.CurrentPositionColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.FirstLineSupervisorEmplIDColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.FirstLineSupervisorEmplIDColumn.ColumnName)
                    drTransferDisplayData(dsTransfers.dtTransferDisplayData.SecondLineSupervisorEmplIDColumn.ColumnName) = drTransfer(dsTransfers.dtTransfers.SecondLineSupervisorEmplIDColumn.ColumnName)
                    'Should be only one row for supervisors
                    For Each drFirstLineSupervisor As DataRow In drTransfer.GetChildRows("dtTransfers_dtFirstLineSupervisor")
                        drTransferDisplayData(dsTransfers.dtTransferDisplayData.FirstLineSupervisorNameColumn.ColumnName) = drFirstLineSupervisor(dsTransfers.dtFirstLineSupervisor.EMPLOYEE_NAMEColumn.ColumnName)
                    Next
                    For Each drSecondLineSupervisor As DataRow In drTransfer.GetChildRows("dtTransfers_dtSecondLineSupervisor")
                        drTransferDisplayData(dsTransfers.dtTransferDisplayData.SecondLineSupervisorNameColumn.ColumnName) = drSecondLineSupervisor(dsTransfers.dtSecondLineSupervisor.EMPLOYEE_NAMEColumn.ColumnName)
                    Next
                    dsTransfers.dtTransferDisplayData.Rows.Add(drTransferDisplayData)
                Next

            Next



            Return dsTransfers.dtTransferDisplayData
            'Return dsTransfers.dtTransfers

        End Function


        Private Function FilterTransferData(ByVal TransferDataTable As DataTable, ByVal CurrentPosition As ArrayList, ByVal CurrentProgram As ArrayList, ByVal RequestedProgram As ArrayList, _
                                            ByVal CurrentOffice As ArrayList, ByVal RequestedOffice As ArrayList, ByVal ShowCanceled As Boolean) As DataTable
            If CurrentPosition.Count = 0 And CurrentProgram.Count = 0 And RequestedProgram.Count = 0 And CurrentOffice.Count = 0 And RequestedOffice.Count = 0 And ShowCanceled = True Then
                Return TransferDataTable
            End If

            Dim FilteredTransferTable As DataTable = TransferDataTable
            If CurrentPosition.Count > 0 Then
                'Dim query = _
                '    From transfer In FilteredTransferTable.AsEnumerable() _
                '        Where transfer.Field(Of Int32)("tlkpPosition") = CurrentPosition _
                '        Select transfer
                Dim query = _
                    From transfer In FilteredTransferTable.AsEnumerable() _
                        Where CurrentPosition.Contains(transfer.Field(Of Int32)("tlkpPosition").ToString) _
                        Select transfer
                If query.Count = 0 Then
                    FilteredTransferTable.Clear()
                    Return FilteredTransferTable
                End If
                FilteredTransferTable = query.CopyToDataTable
            End If
            If CurrentProgram.Count > 0 Then
                'Dim query = _
                '    From transfer In FilteredTransferTable.AsEnumerable() _
                '        Where transfer.Field(Of Int32)("tlkpProgram") = CurrentProgram _
                '        Select transfer
                Dim query = _
                    From transfer In FilteredTransferTable _
                        Where CurrentProgram.Contains(transfer.Field(Of Int32)("tlkpProgram").ToString) _
                        Select transfer
                If query.Count = 0 Then
                    FilteredTransferTable.Clear()
                    Return FilteredTransferTable
                End If
                FilteredTransferTable = query.CopyToDataTable
            End If
            If RequestedProgram.Count > 0 Then
                'Dim query = _
                '        From transfer In FilteredTransferTable.AsEnumerable() _
                '            Where transfer.Field(Of Int32)("tlkpReqProgram") = RequestedProgram _
                '            Select transfer
                Dim query = _
                    From transfer In FilteredTransferTable.AsEnumerable() _
                        Where RequestedProgram.Contains(transfer.Field(Of Int32)("tlkpReqProgram").ToString) _
                        Select transfer
                If query.Count = 0 Then
                    FilteredTransferTable.Clear()
                    Return FilteredTransferTable
                End If
                FilteredTransferTable = query.CopyToDataTable
            End If
            If CurrentOffice.Count > 0 Then
                'Dim query = _
                '       From transfer In FilteredTransferTable.AsEnumerable() _
                '           Where transfer.Field(Of Int32)("tlkpOffice") = CurrentOffice _
                '           Select transfer
                Dim query = _
                    From transfer In FilteredTransferTable.AsEnumerable() _
                        Where CurrentOffice.Contains(transfer.Field(Of Int32)("tlkpOffice").ToString) _
                        Select transfer
                If query.Count = 0 Then
                    FilteredTransferTable.Clear()
                    Return FilteredTransferTable
                End If
                FilteredTransferTable = query.CopyToDataTable
            End If
            ' For each transfer brought back from the database, go out and get the requested office locations
            ' If one of the office locations for that transfer request is being filtered by the user then remove it from the transfer datatable
            If RequestedOffice.Count > 0 Then
                Dim TR As New TransferRequests
                Dim dtRequestLocations As New DataTable
                Dim arrRecordLocationsToRemove As New ArrayList
                Dim i As Int32 = 0
                Dim j As Int32 = 0
                Dim RemoveRecord As Boolean
                For Each dr As DataRow In FilteredTransferTable.Rows
                    RemoveRecord = True ' Assume remove the record until proven that the office being filtered on is related to that transfer record
                    dtRequestLocations = TR.GetTransferRequestLocations(dr.Item("ID"))
                    For Each dr1 As DataRow In dtRequestLocations.Rows
                        For k = 0 To RequestedOffice.Count - 1
                            If dr1.Item("tlkpOfficeLocations") = RequestedOffice(k) Then
                                RemoveRecord = False
                            End If
                        Next
                    Next
                    If RemoveRecord = True Then
                        arrRecordLocationsToRemove.Add(i)
                    End If
                    i = i + 1
                Next
                For i = (arrRecordLocationsToRemove.Count - 1) To 0 Step -1
                    FilteredTransferTable.Rows.RemoveAt(arrRecordLocationsToRemove(i))
                Next
                If FilteredTransferTable.Rows.Count = 0 Then
                    FilteredTransferTable.Clear()
                    Return FilteredTransferTable
                End If
            End If
            ' Filter for showing canceled transfers or not based on user selection
            If ShowCanceled = False Then
                Dim query = _
                       From transfer In FilteredTransferTable.AsEnumerable() _
                           Where transfer.Field(Of Boolean)("Cancelled") = 0 _
                           Select transfer
                If query.Count = 0 Then
                    FilteredTransferTable.Clear()
                    Return FilteredTransferTable
                End If
                FilteredTransferTable = query.CopyToDataTable
            End If


            Return FilteredTransferTable
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTransferRequestLocations(ByVal TransferID As Int32) As DataTable
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)


            ' Get all of the transfers
            Dim cmTransfer As New SqlCommand("uspGetTransferRequestLocations", cnTransfer)
            cmTransfer.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@TransReqID", SqlDbType.Int)
            parmID.Value = TransferID
            cmTransfer.Parameters.Add(parmID)
            cnTransfer.Open()
            Dim dsTransfers As New dsTransfers
            'Dim dtTransfers As New DataTable
            Dim daTransfers As New SqlDataAdapter(cmTransfer)
            daTransfers.Fill(dsTransfers, dsTransfers.dtTransferRequestLocations.TableName)
            cnTransfer.Close()
            Return dsTransfers.dtTransferRequestLocations
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)>
        Public Function InsertTransfer(ByVal EmplID As Integer, ByVal RequestDT As Date, ByVal ToHRDT As Date, ByVal ExpirationDT As Date, ByVal tlkpRequestReason As Integer,
                                   ByVal tlkpPosition As Integer, ByVal FirstLineSupervisorEmplID As String, ByVal SecondLineSupervisorEmplID As String,
                                   ByVal FromHRDT As Date, ByVal tlkpOffice As Integer, ByVal tlkpProgram As Integer, ByVal tlkpReqProgram As Integer,
                                   ByVal Cancelled As Boolean, ByVal tlkpCancelReason As Integer, ByVal TransferDT As Date,
                                   ByVal tlkpTransLocation As Integer, ByVal Comments As String, ByVal TransferRequestLocations As ArrayList) As Int32

            Dim xmlTransferRequestLocations As String
            ' Convert Transfer Request Locations to xml to pass to stored procedure for multiple inserts
            If Not TransferRequestLocations Is Nothing Then
                xmlTransferRequestLocations = FormatTransferLocationRequestsXML(TransferRequestLocations)
            Else
                xmlTransferRequestLocations = String.Empty
            End If

            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertTransfer", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.Int)
            Dim parmRequestDT As New SqlParameter("@RequestDT", SqlDbType.Date)
            Dim parmToHRDT As New SqlParameter("@ToHRDT", SqlDbType.Date)
            Dim parmExpirationDT As New SqlParameter("@ExpirationDT", SqlDbType.Date)
            Dim parmtlkpRequestReason As New SqlParameter("@tlkpRequestReason", SqlDbType.Int)
            Dim parmtlkpPosition As New SqlParameter("@tlkpPosition", SqlDbType.Int)
            Dim parmFirstLineSupervisorEmplID As New SqlParameter("@FirstLineSupervisorEmplID", SqlDbType.VarChar, 50)
            Dim parmSecondLineSupervisorEmplID As New SqlParameter("@SecondLineSupervisorEmplID", SqlDbType.VarChar, 50)
            Dim parmFromHRDT As New SqlParameter("@FromHRDT", SqlDbType.Date)
            Dim parmtlkpOffice As New SqlParameter("@tlkpOffice", SqlDbType.Int)
            Dim parmtlkpProgram As New SqlParameter("@tlkpProgram", SqlDbType.Int)
            Dim parmtlkpReqProgram As New SqlParameter("@tlkpReqProgram", SqlDbType.Int)
            Dim parmCancelled As New SqlParameter("@Cancelled", SqlDbType.Bit)
            Dim parmtlkpCancelReason As New SqlParameter("@tlkpCancelReason", SqlDbType.Int)
            Dim parmTransferDT As New SqlParameter("@TransferDT", SqlDbType.Date)
            Dim parmtlkpTransLocation As New SqlParameter("@tlkpTransLocation", SqlDbType.Int)
            Dim parmComments As New SqlParameter("@Comments", SqlDbType.VarChar, 500)
            Dim parmTransferRequestLocations As New SqlParameter("@TransferRequestLocations", SqlDbType.Xml)
            parmEmplID.Value = EmplID
            If RequestDT = "#12:00:00 AM#" Then
                parmRequestDT.Value = DBNull.Value
            Else
                parmRequestDT.Value = RequestDT
            End If
            If ToHRDT = "#12:00:00 AM#" Then
                parmToHRDT.Value = DBNull.Value
            Else
                parmToHRDT.Value = ToHRDT
            End If
            If ExpirationDT = "#12:00:00 AM#" Then
                parmExpirationDT.Value = DBNull.Value
            Else
                parmExpirationDT.Value = ExpirationDT
            End If
            If tlkpRequestReason = 0 Then
                parmtlkpRequestReason.Value = DBNull.Value
            Else
                parmtlkpRequestReason.Value = tlkpRequestReason
            End If
            If tlkpPosition = 0 Then
                parmtlkpPosition.Value = DBNull.Value
            Else
                parmtlkpPosition.Value = tlkpPosition
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
            If FromHRDT = "#12:00:00 AM#" Then
                parmFromHRDT.Value = DBNull.Value
            Else
                parmFromHRDT.Value = FromHRDT
            End If
            If tlkpOffice = 0 Then
                parmtlkpOffice.Value = DBNull.Value
            Else
                parmtlkpOffice.Value = tlkpOffice
            End If
            If tlkpProgram = 0 Then
                parmtlkpProgram.Value = DBNull.Value
            Else
                parmtlkpProgram.Value = tlkpProgram
            End If
            If tlkpReqProgram = 0 Then
                parmtlkpReqProgram.Value = DBNull.Value
            Else
                parmtlkpReqProgram.Value = tlkpReqProgram
            End If
            parmCancelled.Value = Cancelled
            If tlkpCancelReason = 0 Then
                parmtlkpCancelReason.Value = DBNull.Value
            Else
                parmtlkpCancelReason.Value = tlkpCancelReason
            End If
            If TransferDT = "#12:00:00 AM#" Then
                parmTransferDT.Value = DBNull.Value
            Else
                parmTransferDT.Value = TransferDT
            End If
            If tlkpTransLocation = 0 Then
                parmtlkpTransLocation.Value = DBNull.Value
            Else
                parmtlkpTransLocation.Value = tlkpTransLocation
            End If
            If Comments Is Nothing Then
                parmComments.Value = String.Empty
            Else
                parmComments.Value = Comments
            End If
            If xmlTransferRequestLocations = String.Empty Then
                parmTransferRequestLocations.Value = DBNull.Value
            Else
                parmTransferRequestLocations.Value = xmlTransferRequestLocations
            End If
            cm.Parameters.Add(parmEmplID)
            cm.Parameters.Add(parmRequestDT)
            cm.Parameters.Add(parmToHRDT)
            cm.Parameters.Add(parmExpirationDT)
            cm.Parameters.Add(parmtlkpRequestReason)
            cm.Parameters.Add(parmtlkpPosition)
            cm.Parameters.Add(parmFirstLineSupervisorEmplID)
            cm.Parameters.Add(parmSecondLineSupervisorEmplID)
            cm.Parameters.Add(parmFromHRDT)
            cm.Parameters.Add(parmtlkpOffice)
            cm.Parameters.Add(parmtlkpProgram)
            cm.Parameters.Add(parmtlkpReqProgram)
            cm.Parameters.Add(parmCancelled)
            cm.Parameters.Add(parmtlkpCancelReason)
            cm.Parameters.Add(parmTransferDT)
            cm.Parameters.Add(parmtlkpTransLocation)
            cm.Parameters.Add(parmComments)
            cm.Parameters.Add(parmTransferRequestLocations)
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
        Public Function UpdateTransfer(ByVal EmplID As Integer, ByVal RequestDT As Date, ByVal ToHRDT As Date, ByVal ExpirationDT As Date, ByVal tlkpRequestReason As Integer,
                                   ByVal tlkpPosition As Integer, ByVal FirstLineSupervisorEmplID As String, ByVal SecondLineSupervisorEmplID As String,
                                   ByVal FromHRDT As Date, ByVal tlkpOffice As Integer, ByVal tlkpProgram As Integer, ByVal tlkpReqProgram As Integer,
                                   ByVal Cancelled As Boolean, ByVal tlkpCancelReason As Integer, ByVal TransferDT As Date,
                                   ByVal tlkpTransLocation As Integer, ByVal Comments As String, ByVal TransferRequestLocations As ArrayList,
                                   ByVal Original_ID As Integer) As Int32

            Dim xmlTransferRequestLocations As String
            ' Convert Transfer Request Locations to xml to pass to stored procedure for multiple inserts
            If Not TransferRequestLocations Is Nothing Then
                xmlTransferRequestLocations = FormatTransferLocationRequestsXML(TransferRequestLocations)
            Else
                xmlTransferRequestLocations = String.Empty
            End If

            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdateTransfer", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.Int)
            Dim parmRequestDT As New SqlParameter("@RequestDT", SqlDbType.Date)
            Dim parmToHRDT As New SqlParameter("@ToHRDT", SqlDbType.Date)
            Dim parmExpirationDT As New SqlParameter("@ExpirationDT", SqlDbType.Date)
            Dim parmtlkpRequestReason As New SqlParameter("@tlkpRequestReason", SqlDbType.Int)
            Dim parmtlkpPosition As New SqlParameter("@tlkpPosition", SqlDbType.Int)
            Dim parmFirstLineSupervisorEmplID As New SqlParameter("@FirstLineSupervisorEmplID", SqlDbType.VarChar, 50)
            Dim parmSecondLineSupervisorEmplID As New SqlParameter("@SecondLineSupervisorEmplID", SqlDbType.VarChar, 50)
            Dim parmFromHRDT As New SqlParameter("@FromHRDT", SqlDbType.Date)
            Dim parmtlkpOffice As New SqlParameter("@tlkpOffice", SqlDbType.Int)
            Dim parmtlkpProgram As New SqlParameter("@tlkpProgram", SqlDbType.Int)
            Dim parmtlkpReqProgram As New SqlParameter("@tlkpReqProgram", SqlDbType.Int)
            Dim parmCancelled As New SqlParameter("@Cancelled", SqlDbType.Bit)
            Dim parmtlkpCancelReason As New SqlParameter("@tlkpCancelReason", SqlDbType.Int)
            Dim parmTransferDT As New SqlParameter("@TransferDT", SqlDbType.Date)
            Dim parmtlkpTransLocation As New SqlParameter("@tlkpTransLocation", SqlDbType.Int)
            Dim parmComments As New SqlParameter("@Comments", SqlDbType.VarChar, 500)
            Dim parmTransferRequestLocations As New SqlParameter("@TransferRequestLocations", SqlDbType.Xml)
            parmID.Value = Original_ID
            parmEmplID.Value = EmplID
            If RequestDT = "#12:00:00 AM#" Then
                parmRequestDT.Value = DBNull.Value
            Else
                parmRequestDT.Value = RequestDT
            End If
            If ToHRDT = "#12:00:00 AM#" Then
                parmToHRDT.Value = DBNull.Value
            Else
                parmToHRDT.Value = ToHRDT
            End If
            If ExpirationDT = "#12:00:00 AM#" Then
                parmExpirationDT.Value = DBNull.Value
            Else
                parmExpirationDT.Value = ExpirationDT
            End If
            If tlkpRequestReason = 0 Then
                parmtlkpRequestReason.Value = DBNull.Value
            Else
                parmtlkpRequestReason.Value = tlkpRequestReason
            End If
            If tlkpPosition = 0 Then
                parmtlkpPosition.Value = DBNull.Value
            Else
                parmtlkpPosition.Value = tlkpPosition
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
            If FromHRDT = "#12:00:00 AM#" Then
                parmFromHRDT.Value = DBNull.Value
            Else
                parmFromHRDT.Value = FromHRDT
            End If
            If tlkpOffice = 0 Then
                parmtlkpOffice.Value = DBNull.Value
            Else
                parmtlkpOffice.Value = tlkpOffice
            End If
            If tlkpProgram = 0 Then
                parmtlkpProgram.Value = DBNull.Value
            Else
                parmtlkpProgram.Value = tlkpProgram
            End If
            If tlkpReqProgram = 0 Then
                parmtlkpReqProgram.Value = DBNull.Value
            Else
                parmtlkpReqProgram.Value = tlkpReqProgram
            End If
            parmCancelled.Value = Cancelled
            If tlkpCancelReason = 0 Then
                parmtlkpCancelReason.Value = DBNull.Value
            Else
                parmtlkpCancelReason.Value = tlkpCancelReason
            End If
            If TransferDT = "#12:00:00 AM#" Then
                parmTransferDT.Value = DBNull.Value
            Else
                parmTransferDT.Value = TransferDT
            End If
            If tlkpTransLocation = 0 Then
                parmtlkpTransLocation.Value = DBNull.Value
            Else
                parmtlkpTransLocation.Value = tlkpTransLocation
            End If
            If Comments Is Nothing Then
                parmComments.Value = String.Empty
            Else
                parmComments.Value = Comments
            End If
            If xmlTransferRequestLocations = String.Empty Then
                parmTransferRequestLocations.Value = DBNull.Value
            Else
                parmTransferRequestLocations.Value = xmlTransferRequestLocations
            End If
            parmTransferRequestLocations.Value = xmlTransferRequestLocations
            cm.Parameters.Add(parmID)
            cm.Parameters.Add(parmEmplID)
            cm.Parameters.Add(parmRequestDT)
            cm.Parameters.Add(parmToHRDT)
            cm.Parameters.Add(parmExpirationDT)
            cm.Parameters.Add(parmtlkpRequestReason)
            cm.Parameters.Add(parmtlkpPosition)
            cm.Parameters.Add(parmFirstLineSupervisorEmplID)
            cm.Parameters.Add(parmSecondLineSupervisorEmplID)
            cm.Parameters.Add(parmFromHRDT)
            cm.Parameters.Add(parmtlkpOffice)
            cm.Parameters.Add(parmtlkpProgram)
            cm.Parameters.Add(parmtlkpReqProgram)
            cm.Parameters.Add(parmCancelled)
            cm.Parameters.Add(parmtlkpCancelReason)
            cm.Parameters.Add(parmTransferDT)
            cm.Parameters.Add(parmtlkpTransLocation)
            cm.Parameters.Add(parmComments)
            cm.Parameters.Add(parmTransferRequestLocations)
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
        Public Function DeleteTransfer(ByVal Original_ID As Int32) As Int32
            Try
                Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
                Dim cmTransfer As New SqlCommand("uspDeleteTransfer", cnTransfer)
                cmTransfer.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@TransferRequestID", SqlDbType.Int)
                parmID.Value = Original_ID
                cmTransfer.Parameters.Add(parmID)
                cnTransfer.Open()
                Dim dsTransfers As New dsTransfers
                Dim daTransferDocuments As New SqlDataAdapter(cmTransfer)
                daTransferDocuments.Fill(dsTransfers, dsTransfers.dtTransferDocuments.TableName)
                cnTransfer.Close()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            End Try
            Return 0
        End Function

        Private Function FormatTransferLocationRequestsXML(ByVal TransferRequestLocations As ArrayList) As String
            ' Given an arraylist of office locations that the employee has requested to be transferred to, format into XML to pass to the database for insert or update
            Dim xmlTransfers As New StringBuilder

            xmlTransfers.Append("<Transfers>")


            For i As Int32 = 0 To TransferRequestLocations.Count - 1
                xmlTransfers.Append("<row>")
                xmlTransfers.Append("<Location>" & TransferRequestLocations(i) & "</Location>")
                xmlTransfers.Append("<ReqNbr>" & i + 1 & "</ReqNbr>")
                xmlTransfers.Append("</row>")
            Next
            xmlTransfers.AppendFormat("</Transfers>")

            Return xmlTransfers.ToString

        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTransferDocuments(ByVal TransferRequestID As Int32) As DataTable
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)


            ' Get all of the transfers
            Dim cmTransfer As New SqlCommand("uspGetTransferDocuments", cnTransfer)
            cmTransfer.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@TransferReqID", SqlDbType.Int)
            parmID.Value = TransferRequestID
            cmTransfer.Parameters.Add(parmID)
            cnTransfer.Open()
            Dim dsTransfers As New dsTransfers
            Dim daTransferDocuments As New SqlDataAdapter(cmTransfer)
            daTransferDocuments.Fill(dsTransfers, dsTransfers.dtTransferDocuments.TableName)
            cnTransfer.Close()
            Return dsTransfers.dtTransferDocuments
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTransferDocumentObject(ByVal ID As Int32) As Data.DataTable
            Dim dt As New DataTable
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Try

                Dim cm As New SqlClient.SqlCommand("uspGetDocumentObject", cnTransfer)
                cm.Parameters.Add(New SqlParameter("@ID", Data.SqlDbType.Int))
                cm.Parameters("@ID").Value = ID
                cm.CommandType = CommandType.StoredProcedure

                Dim da As New SqlClient.SqlDataAdapter(cm)
                cnTransfer.Open()
                da.Fill(dt)
                cnTransfer.Close()
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If cnTransfer.State = ConnectionState.Open Then
                    cnTransfer.Close()
                End If
            End Try
            Return dt

        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Public Function InsertTransferDocument(ByVal TransferReqID As Integer, ByVal FileName As String, ByVal Data As Byte(), ByVal LoadedBy As String) As Int32
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertTransferDocument", cn)
            cm.CommandType = CommandType.StoredProcedure

            cm.CommandType = CommandType.StoredProcedure
            Dim parmTransferRequestID As New SqlParameter("@TransferRequestID", SqlDbType.Int)
            Dim parmFileName As New SqlParameter("@FileName", SqlDbType.VarChar, 50)
            Dim parmData As New SqlParameter("@Data", SqlDbType.VarBinary)
            Dim parmLoadedBy As New SqlParameter("@LoadedBy", SqlDbType.VarChar, 6)
            parmTransferRequestID.Value = TransferReqID
            parmFileName.Value = FileName
            parmData.Value = Data
            parmLoadedBy.Value = LoadedBy
            cm.Parameters.Add(parmTransferRequestID)
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
        Public Function DeleteTransferDocument(ByVal Original_ID As Int32) As Int32
            Try
                Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
                Dim cmTransfer As New SqlCommand("uspDeleteDocument", cnTransfer)
                cmTransfer.CommandType = CommandType.StoredProcedure
                Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
                parmID.Value = Original_ID
                cmTransfer.Parameters.Add(parmID)
                cnTransfer.Open()
                cmTransfer.ExecuteNonQuery()
                cnTransfer.Close()
            Catch sqlEx As SqlException
                Throw New Exception(sqlEx.Message)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            End Try
            Return 0
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTransferAssignedLocations(ByVal TransferID As Int32) As DataTable
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cmTransfer As New SqlCommand("uspGetTransferRequestLocations", cnTransfer)
            cmTransfer.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@TransReqID", SqlDbType.Int)
            parmID.Value = TransferID
            cmTransfer.Parameters.Add(parmID)
            cnTransfer.Open()
            Dim dtAssignedLocations As New DataTable
            Dim daTransfers As New SqlDataAdapter(cmTransfer)
            daTransfers.Fill(dtAssignedLocations)
            cnTransfer.Close()
            Return dtAssignedLocations
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Public Function GetTransferAvailableLocations(ByVal TransferID As Int32) As DataTable
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cmTransfer As New SqlCommand("uspGetTransferAvailableLocations", cnTransfer)
            cmTransfer.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@TransReqID", SqlDbType.Int)
            parmID.Value = TransferID
            cmTransfer.Parameters.Add(parmID)
            cnTransfer.Open()
            Dim dtAvalailableLocations As New DataTable
            Dim daTransfers As New SqlDataAdapter(cmTransfer)
            daTransfers.Fill(dtAvalailableLocations)
            cnTransfer.Close()
            Return dtAvalailableLocations
        End Function

        Public Shared Function CheckForDuplicateTransfer(ByVal TransferID As Int32, ByVal EmplID As String, ByVal RequestDate As Date, ByVal InsertOrUpdate As String) As Boolean
            Dim cnTransfer As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cmTransfer As New SqlCommand("uspGetTransferCountByEmployeeAndRequestDate", cnTransfer)
            cmTransfer.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@TransferID", SqlDbType.Int)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            Dim parmRequestDate As New SqlParameter("@RequestDate", SqlDbType.Date)
            Dim parmInsertOrUpdate As New SqlParameter("@InsertOrUpdate", SqlDbType.VarChar, 6)
            parmID.Value = TransferID
            parmEmplID.Value = EmplID
            parmRequestDate.Value = RequestDate
            parmInsertOrUpdate.Value = InsertOrUpdate
            cmTransfer.Parameters.Add(parmID)
            cmTransfer.Parameters.Add(parmEmplID)
            cmTransfer.Parameters.Add(parmRequestDate)
            cmTransfer.Parameters.Add(parmInsertOrUpdate)
            cnTransfer.Open()
            Dim dupeCount As Int32
            Try
                dupeCount = cmTransfer.ExecuteScalar()
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                cnTransfer.Close()
            End Try
            If dupeCount > 0 Then
                Return True
            End If
            'If InsertOrUpdate = "Insert" And dupeCount > 0 Then
            '    Return True
            '    'ElseIf InsertOrUpdate = "Update" And dupeCount > 1 Then
            '    '    Return True
            'End If
            Return False
        End Function




    End Class
End Namespace
