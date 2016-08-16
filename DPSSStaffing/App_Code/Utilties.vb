Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Mail
Imports System.Linq

Namespace StaffingUtilities
    <System.ComponentModel.DataObject()> _
    Public Class Utilities

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetLookups(ByVal Category As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetLookups", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmCategory As New SqlParameter("@Category", Category)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Dim dr As DataRow
            dr = dtLookups.NewRow()
            dr.Item("ID") = 0
            dr.Item("Category") = Category
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
        Shared Function GetLookupsForUpdateForm(ByVal Category As String, ByVal CurrentID As Int32) As DataView
            Dim dvLookups As DataView
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetLookups", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmCategory As New SqlParameter("@Category", Category)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Dim dr As DataRow
            dr = dtLookups.NewRow()
            dr.Item("ID") = 0
            dr.Item("Category") = Category
            dr.Item("SubCategory") = DBNull.Value
            dr.Item("DataValue") = String.Empty
            dr.Item("Description") = String.Empty
            dr.Item("Active") = 1
            dr.Item("DisplayOrder") = DBNull.Value
            dr.Item("ModifiedBy") = String.Empty
            dr.Item("ModifiedDate") = DBNull.Value
            dtLookups.Rows.InsertAt(dr, 0)
            ' Only allow active items in comboboxes but pick up an inactive value if it is assigned to the record currently being updated
            dvLookups = dtLookups.DefaultView
            dvLookups.Sort = "DataValue"
            If CurrentID = 0 Then
                dvLookups.RowFilter = "Active = True"
            Else
                dvLookups.RowFilter = "( Active = True ) or ( ID = " & CurrentID.ToString.Trim & ")"
            End If
            Return dvLookups
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetLookupsForLookupMaintenance(ByVal Category As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetLookups", cn)
            cm.CommandType = CommandType.StoredProcedure
            If Category Is Nothing Then
                Category = String.Empty
            End If
            Dim parmCategory As New SqlParameter("@Category", Category)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetDefaultEmailAddresses(ByVal Category As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetDefaultEmailAddresses", cn)
            cm.CommandType = CommandType.StoredProcedure
            If Category Is Nothing Then
                Category = String.Empty
            End If
            Dim parmCategory As New SqlParameter("@Category", Category)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetLookupCategories() As DataTable
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetLookupCategories", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim dt As New DataTable
            Dim da As New SqlDataAdapter(cm)
            cn.Open()
            da.Fill(dt)
            cn.Close()
            Dim dr As DataRow
            dr = dt.NewRow()
            dr.Item("Category") = ""
            dt.Rows.InsertAt(dr, 0)
            Return dt
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Shared Function InsertLookup(ByVal Category As String, ByVal DataValue As String, ByVal Description As String, _
                                 ByVal Active As Boolean, ByVal EmplID As String) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertLookup", cn)
            cm.CommandType = CommandType.StoredProcedure

            Dim parmCategory As New SqlParameter("@Category", SqlDbType.VarChar, 30)
            Dim parmDataValue As New SqlParameter("@DataValue", SqlDbType.VarChar, 45)
            Dim parmDescription As New SqlParameter("@Description", SqlDbType.VarChar, 45)
            Dim parmActive As New SqlParameter("@Active", SqlDbType.Bit)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmCategory.Value = Category
            parmDataValue.Value = DataValue
            parmDescription.Value = Description
            parmActive.Value = Active
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmCategory)
            cm.Parameters.Add(parmDataValue)
            cm.Parameters.Add(parmDescription)
            cm.Parameters.Add(parmActive)
            cm.Parameters.Add(parmEmplID)
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
        Shared Function UpdateLookup(ByVal Original_ID As Int32, ByVal Category As String, ByVal DataValue As String, ByVal Description As String, _
                                 ByVal Active As Boolean, ByVal EmplID As String) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdateLookup", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            Dim parmCategory As New SqlParameter("@Category", SqlDbType.VarChar, 30)
            Dim parmDataValue As New SqlParameter("@DataValue", SqlDbType.VarChar, 45)
            Dim parmDescription As New SqlParameter("@Description", SqlDbType.VarChar, 45)
            Dim parmActive As New SqlParameter("@Active", SqlDbType.Bit)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmID.Value = Original_ID
            parmCategory.Value = Category
            parmDataValue.Value = DataValue
            parmDescription.Value = Description
            parmActive.Value = Active
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmID)
            cm.Parameters.Add(parmCategory)
            cm.Parameters.Add(parmDataValue)
            cm.Parameters.Add(parmDescription)
            cm.Parameters.Add(parmActive)
            cm.Parameters.Add(parmEmplID)
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
        Shared Function DeleteLookup(ByVal Original_ID As Int32) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspDeleteLookup", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            parmID.Value = Original_ID
            cm.Parameters.Add(parmID)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetOvertimeOfficeAssigments() As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeOfficeAssigments", cn)
            cm.CommandType = CommandType.StoredProcedure
            cn.Open()

            Dim dsOverTime As New dsOvertime
            'Dim dtOvertimeOfficeAssignments As New dsOvertime.dtOvertimeOfficeAssignmentsDataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dsOverTime.dtOvertimeOfficeAssignments)

            Dim EmployeeIds As String = Utilities.FormatEmployeeIDsXML(dsOverTime.dtOvertimeOfficeAssignments)

            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspDPSSStaffingCurrentInfo", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure

            'Get all of the employee data from the Employee Database for each transfer request
            'Dim dtEmployees As New dsOvertime.dtEmployeesDataTable
            Dim parmEmployeeIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = EmployeeIds
            cmEmployee.Parameters.Add(parmEmployeeIDs)
            cnEmployee.Open()
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            daEmployees.Fill(dsOverTime.dtEmployees)
            cnEmployee.Close()


            For Each drOvertimeOfficeAssignmentRow As dsOvertime.dtOvertimeOfficeAssignmentsRow In dsOverTime.dtOvertimeOfficeAssignments.Rows
                For Each drEmployee As dsOvertime.dtEmployeesRow In drOvertimeOfficeAssignmentRow.GetChildRows("dtOvertimeOfficeAssignments_dtEmployees")
                    drOvertimeOfficeAssignmentRow.Employee_Name = drEmployee.EMPLOYEE_NAME
                Next
            Next

            cn.Close()
            Return dsOverTime.dtOvertimeOfficeAssignments
        End Function


        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Shared Function InsertOvertimeOfficeAssigment(ByVal EmplID As String, ByVal tlkpOverTimeLocation As Int32) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertOvertimeOfficeAssigment", cn)
            cm.CommandType = CommandType.StoredProcedure

            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmEmplID)

            Dim parmOvertimeLocation As New SqlParameter("@tlkpOverTimeLocation", SqlDbType.Int)
            parmOvertimeLocation.Value = tlkpOverTimeLocation
            cm.Parameters.Add(parmOvertimeLocation)
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
        Shared Function UpdateOvertimeOfficeAssigment(ByVal Original_ID As Int32, ByVal EmplID As String, ByVal tlkpOverTimeLocation As Int32) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdateOvertimeOfficeAssigment", cn)
            cm.CommandType = CommandType.StoredProcedure

            Dim parmID As New SqlParameter("@ID", SqlDbType.VarChar, 6)
            parmID.Value = Original_ID
            cm.Parameters.Add(parmID)

            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmEmplID)

            Dim parmOvertimeLocation As New SqlParameter("@tlkpOverTimeLocation", SqlDbType.Int)
            parmOvertimeLocation.Value = tlkpOverTimeLocation
            cm.Parameters.Add(parmOvertimeLocation)
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
        Shared Function DeleteOvertimeOfficeAssigment(ByVal Original_ID As Int32) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspDeleteOvertimeOfficeAssigment", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            parmID.Value = Original_ID
            cm.Parameters.Add(parmID)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetOvertimeLookupSubCategories(ByVal SubCategory As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeLookupSubCategories", cn)
            cm.CommandType = CommandType.StoredProcedure
            cn.Open()

            Dim dtOvertimeLookupSubCategories As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtOvertimeLookupSubCategories)
            cn.Close()
            Return dtOvertimeLookupSubCategories
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetOvertimeLookupsForLookupMaintenance(ByVal SubCategory As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetOvertimeLookupsForMaintenance", cn)
            cm.CommandType = CommandType.StoredProcedure
            If SubCategory Is Nothing Then
                SubCategory = String.Empty
            End If
            Dim parmSubCategory As New SqlParameter("@SubCategory", SubCategory)
            cm.Parameters.Add(parmSubCategory)
            cn.Open()

            Dim dtOvertimeLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtOvertimeLookups)
            cn.Close()
            Return dtOvertimeLookups
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Shared Function InsertOvertimeLookup(ByVal SubCategory As String, ByVal DataValue As String, ByVal Description As String, _
                                 ByVal Active As Boolean, ByVal EmplID As String) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertOvertimeLookup", cn)
            cm.CommandType = CommandType.StoredProcedure

            Dim parmSubCategory As New SqlParameter("@SubCategory", SqlDbType.VarChar, 30)
            Dim parmDataValue As New SqlParameter("@DataValue", SqlDbType.VarChar, 30)
            Dim parmDescription As New SqlParameter("@Description", SqlDbType.VarChar, 45)
            Dim parmActive As New SqlParameter("@Active", SqlDbType.Bit)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmSubCategory.Value = SubCategory
            parmDataValue.Value = DataValue
            parmDescription.Value = Description
            parmActive.Value = Active
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmSubCategory)
            cm.Parameters.Add(parmDataValue)
            cm.Parameters.Add(parmDescription)
            cm.Parameters.Add(parmActive)
            cm.Parameters.Add(parmEmplID)
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
        Shared Function UpdateOvertimeLookup(ByVal Original_ID As Int32, ByVal SubCategory As String, ByVal DataValue As String, ByVal Description As String, _
                                 ByVal Active As Boolean, ByVal EmplID As String) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdateOvertimeLookup", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            Dim parmDataValue As New SqlParameter("@DataValue", SqlDbType.VarChar, 30)
            Dim parmDescription As New SqlParameter("@Description", SqlDbType.VarChar, 45)
            Dim parmActive As New SqlParameter("@Active", SqlDbType.Bit)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmID.Value = Original_ID
            parmDataValue.Value = DataValue
            parmDescription.Value = Description
            parmActive.Value = Active
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmID)
            cm.Parameters.Add(parmDataValue)
            cm.Parameters.Add(parmDescription)
            cm.Parameters.Add(parmActive)
            cm.Parameters.Add(parmEmplID)
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
        Shared Function DeleteOvertimeLookup(ByVal Original_ID As Int32) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspDeleteLookup", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            parmID.Value = Original_ID
            cm.Parameters.Add(parmID)
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

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetDefaultEmailAddressCategories() As DataTable

            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetDefaultEmailAddressCategories", cn)
            cm.CommandType = CommandType.StoredProcedure
            cn.Open()
            Dim dtAddressCategories As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtAddressCategories)
            cn.Close()
            Return dtAddressCategories
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Select)> _
        Shared Function GetDefaultEmailAddressesLookupMaintenance(ByVal Category As String) As DataTable

            Dim dsEmailAddresses As New dsDefaultEmailAddresses

            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetDefaultEmailAddressesForMaintenance", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmCategory As New SqlParameter("@Category", SqlDbType.VarChar, 30)
            parmCategory.Value = Category
            cm.Parameters.Add(parmCategory)
            cn.Open()


            Dim da As New SqlDataAdapter(cm)
            da.Fill(dsEmailAddresses.dtDefaultEmailAddresses)
            cn.Close()

            Dim EmployeeIds As String = Utilities.FormatEmployeeIDsXML(dsEmailAddresses.dtDefaultEmailAddresses)

            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspDPSSStaffingCurrentInfo", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure

            'Get all of the employee data from the Employee Database for each transfer request
            'Dim dtEmployees As New dsOvertime.dtEmployeesDataTable
            Dim parmEmployeeIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = EmployeeIds
            cmEmployee.Parameters.Add(parmEmployeeIDs)
            cnEmployee.Open()
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            daEmployees.Fill(dsEmailAddresses.dtEmployees)
            cnEmployee.Close()

            For Each drEmailAddress As dsDefaultEmailAddresses.dtDefaultEmailAddressesRow In dsEmailAddresses.dtDefaultEmailAddresses.Rows
                For Each drEmployee As dsDefaultEmailAddresses.dtEmployeesRow In drEmailAddress.GetChildRows("dtDefaultEmailAddresses_dtEmployees")
                    drEmailAddress.EMPLOYEE_NAME = drEmployee.EMPLOYEE_NAME
                Next
            Next
            Return dsEmailAddresses.dtDefaultEmailAddresses
        End Function

        <System.ComponentModel.DataObjectMethod(ComponentModel.DataObjectMethodType.Insert)> _
        Shared Function InsertDefaultEmailAddress(ByVal Category As String, ByVal EmailEmplID As String, _
                                 ByVal Active As Boolean, ByVal EmplID As String) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspInsertDefaultEmailAddress", cn)
            cm.CommandType = CommandType.StoredProcedure

            Dim parmCategory As New SqlParameter("@Category", SqlDbType.VarChar, 30)
            Dim parmDataValue As New SqlParameter("@DataValue", SqlDbType.VarChar, 30)
            Dim parmActive As New SqlParameter("@Active", SqlDbType.Bit)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmCategory.Value = Category
            parmDataValue.Value = EmailEmplID
            parmActive.Value = Active
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmCategory)
            cm.Parameters.Add(parmDataValue)
            cm.Parameters.Add(parmActive)
            cm.Parameters.Add(parmEmplID)
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
        Shared Function UpdateDefaultEmailAddress(ByVal Original_ID As Int32, ByVal Category As String, ByVal EmailEmplID As String, _
                                 ByVal Active As Boolean, ByVal EmplID As String) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspUpdateDefaultEmailAddress", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            Dim parmDataValue As New SqlParameter("@DataValue", SqlDbType.VarChar, 30)
            Dim parmActive As New SqlParameter("@Active", SqlDbType.Bit)
            Dim parmEmplID As New SqlParameter("@EmplID", SqlDbType.VarChar, 6)
            parmID.Value = Original_ID
            parmDataValue.Value = EmailEmplID
            parmActive.Value = Active
            parmEmplID.Value = EmplID
            cm.Parameters.Add(parmID)
            cm.Parameters.Add(parmDataValue)
            cm.Parameters.Add(parmActive)
            cm.Parameters.Add(parmEmplID)
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
        Shared Function DeleteDefaultEmailAddress(ByVal Original_ID As Int32) As Int32
            Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspDeleteDefaultEmailAddress", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmID As New SqlParameter("@ID", SqlDbType.Int)
            parmID.Value = Original_ID
            cm.Parameters.Add(parmID)
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

        Shared Function FormatEmployeeIDsXML(ByVal InputTable As DataTable) As String
            Dim xmlTransfers As New StringBuilder

            xmlTransfers.Append("<Employees>")


            For Each dr As DataRow In InputTable.Rows
                xmlTransfers.Append("<row>")
                xmlTransfers.Append("<EmplID>" & dr("EmplID") & "</EmplID>")
                xmlTransfers.Append("</row>")
            Next
            xmlTransfers.AppendFormat("</Employees>")

            Return xmlTransfers.ToString

        End Function

        Shared Function FormatFirstLineSupervisorEmployeeIDsXML(ByVal TransferTable As DataTable) As String
            Dim xmlTransfers As New StringBuilder

            xmlTransfers.Append("<Employees>")


            For Each dr As DataRow In TransferTable.Rows
                xmlTransfers.Append("<row>")
                xmlTransfers.Append("<EmplID>" & dr("FirstLineSupervisorEmplID") & "</EmplID>")
                xmlTransfers.Append("</row>")
            Next
            xmlTransfers.AppendFormat("</Employees>")

            Return xmlTransfers.ToString

        End Function

        Shared Function FormatSecondLineSupervisorEmployeeIDsXML(ByVal TransferTable As DataTable) As String
            Dim xmlTransfers As New StringBuilder

            xmlTransfers.Append("<Employees>")


            For Each dr As DataRow In TransferTable.Rows
                xmlTransfers.Append("<row>")
                xmlTransfers.Append("<EmplID>" & dr("SecondLineSupervisorEmplID") & "</EmplID>")
                xmlTransfers.Append("</row>")
            Next
            xmlTransfers.AppendFormat("</Employees>")

            Return xmlTransfers.ToString

        End Function

        Shared Function FormatContactPhoneNumbersXML(ByVal ContactTypeIDs As ArrayList, ByVal ContactPhoneNumbers As ArrayList) As String
            Dim xmlContactPhoneNumbers As New StringBuilder

            xmlContactPhoneNumbers.Append("<ContactPhoneNumbers>")


            For i As Integer = 0 To ContactPhoneNumbers.Count - 1
                xmlContactPhoneNumbers.Append("<row>")
                xmlContactPhoneNumbers.Append("<ContactTypeID>" & ContactTypeIDs(i) & "</ContactTypeID>")
                xmlContactPhoneNumbers.Append("<ContactPhoneNumber>" & ContactPhoneNumbers(i) & "</ContactPhoneNumber>")
                xmlContactPhoneNumbers.Append("</row>")
            Next
            xmlContactPhoneNumbers.AppendFormat("</ContactPhoneNumbers>")

            Return xmlContactPhoneNumbers.ToString

        End Function

        Shared Function GetDefaultEmailText(ByVal EmailType As String) As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspGetLookups", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmCategory As New SqlParameter("@Category", EmailType)
            cm.Parameters.Add(parmCategory)
            cn.Open()

            Dim dtLookups As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtLookups)
            cn.Close()
            Return dtLookups
        End Function

        Shared Function SendEmail(ByVal Recipients() As String, ByVal EmailCC() As String, ByVal Subject As String, ByVal HTMLBody As String, ByVal EmailAttachments() As String) As String
            Try
                If Recipients.Count = 0 Then
                    Return ""
                End If
                Dim Mail1546 As New MailMessage()

                For i = 0 To Recipients.Count - 1
                    If Recipients(i) <> String.Empty Then
                        Mail1546.To.Add(Recipients(i))
                    End If
                Next

                For i = 0 To EmailCC.Count - 1
                    If EmailCC(i) <> String.Empty Then
                        Mail1546.CC.Add(EmailCC(i))
                    End If
                Next

                If Not EmailAttachments(0) = "None" Then
                    For i = 0 To EmailAttachments.Count - 1
                        Dim attachment As New Attachment(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString)
                        Mail1546.Attachments.Add(attachment)
                    Next
                End If

                Mail1546.From = New MailAddress(System.Configuration.ConfigurationManager.AppSettings("SiteEmailAccount"))
                Mail1546.Subject = Subject
                Mail1546.Body = HTMLBody
                Mail1546.IsBodyHtml = True
                Dim client As SmtpClient = New SmtpClient(System.Configuration.ConfigurationManager.AppSettings("EmailServer"))

                client.Send(Mail1546)
                Mail1546.Dispose()
                For i = 0 To EmailAttachments.Count - 1
                    System.IO.File.Delete(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString)
                Next
                Return ""
            Catch e As Exception
                Throw New Exception(e.Message)
            End Try

        End Function


    End Class
End Namespace
