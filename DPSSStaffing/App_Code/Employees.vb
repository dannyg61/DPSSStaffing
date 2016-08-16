Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient

Namespace StaffEmployees

    <System.ComponentModel.DataObject()> _
    Public Class Employees
        Shared Function GetEmployees() As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspDPSSStaffingSearchName", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmLastName As New SqlParameter("@LAST_NAME", DBNull.Value)
            Dim parmFirstName As New SqlParameter("@FIRST_NAME", DBNull.Value)
            cm.Parameters.Add(parmLastName)
            cm.Parameters.Add(parmFirstName)
            cn.Open()

            Dim dtEmployees As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtEmployees)
            cn.Close()
            Return dtEmployees
        End Function

        Public Function GetSupervisors() As DataTable
            Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cm As New SqlCommand("uspDPSSStaffingSearchName", cn)
            cm.CommandType = CommandType.StoredProcedure
            Dim parmLastName As New SqlParameter("@LAST_NAME", DBNull.Value)
            Dim parmFirstName As New SqlParameter("@FIRST_NAME", DBNull.Value)
            cm.Parameters.Add(parmLastName)
            cm.Parameters.Add(parmFirstName)
            cn.Open()

            Dim dtSupervisors As New DataTable
            Dim da As New SqlDataAdapter(cm)
            da.Fill(dtSupervisors)
            cn.Close()
            Dim dr As DataRow
            dr = dtSupervisors.NewRow()
            dr.Item("NAME") = ""
            dr.Item("EMPLID") = ""
            dtSupervisors.Rows.InsertAt(dr, 0)
            Return dtSupervisors
        End Function

        Public Function GetEmployeeInformation(ByVal EmployeeNumber As String) As DataTable
            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspDPSSStaffingCurrentInfo", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure


            Dim parmEmployeeIDs As New SqlParameter("@EMPLIDs", SqlDbType.Xml)
            parmEmployeeIDs.Value = FormatEmployeeIDXML(EmployeeNumber)
            cmEmployee.Parameters.Add(parmEmployeeIDs)
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            Dim dtEmployee As New DataTable
            cnEmployee.Open()
            daEmployees.Fill(dtEmployee)
            cnEmployee.Close()
            Return dtEmployee
        End Function

        Public Function GetEmployeeNumberByLoginID(ByVal LoginID As String) As String
            Dim cnEmployee As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EmployeeInfo_SqlConnectionString").ConnectionString)
            Dim cmEmployee As New SqlCommand("uspARS_GetEmployeeID", cnEmployee)
            cmEmployee.CommandType = CommandType.StoredProcedure

            Dim parmLoginID As New SqlParameter("@LOGIN_ID", SqlDbType.VarChar, 10)
            parmLoginID.Value = LoginID
            cmEmployee.Parameters.Add(parmLoginID)
            Dim daEmployees As New SqlDataAdapter(cmEmployee)
            Dim dtEmployee As New DataTable
            cnEmployee.Open()
            daEmployees.Fill(dtEmployee)
            cnEmployee.Close()
            If dtEmployee.Rows.Count > 0 Then
                Return dtEmployee.Rows(0).Item("EMPLID")
            Else
                Return 0
            End If
        End Function

        Private Function FormatEmployeeIDXML(ByVal EmployeeNumber As String) As String
            Dim xmlTransfers As New StringBuilder

            xmlTransfers.Append("<Employees>")
            xmlTransfers.Append("<row>")
            xmlTransfers.Append("<EmplID>" & EmployeeNumber & "</EmplID>")
            xmlTransfers.Append("</row>")
            xmlTransfers.AppendFormat("</Employees>")

            Return xmlTransfers.ToString

        End Function
    End Class

End Namespace
