Imports Telerik.Web.UI
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient

Partial Class Admin_OTStaffOfficeAssignment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            'ViewState("tlkpOverTimeLocation") = OvertimeWhatever.Overtime.GetOvertimeFirstOffice("All")
            ViewState("tlkpOverTimeLocation") = GetOverTimeFirstOffice("All")
        End If
    End Sub

    Protected Sub rgOTStaffOfficeAssignments_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles rgOTStaffOfficeAssignments.ItemDataBound
        
        If TypeOf e.Item Is GridEditFormItem And e.Item.IsInEditMode Then
            If Not TypeOf (e.Item) Is GridEditFormInsertItem Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
                ViewState("Emplid") = Server.HtmlDecode(item("Emplid").Text)
                ViewState("tlkpOverTimeLocation") = Server.HtmlDecode(item("tlkpOverTimeLocation").Text)
            End If
        End If
    End Sub

    Protected Sub objDSOTOfficeAssignment_Inserting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDSOTOfficeAssignment.Inserting
        e.InputParameters("Emplid") = ViewState("Emplid")
        e.InputParameters("tlkpOverTimeLocation") = ViewState("tlkpOverTimeLocation")
    End Sub

    Protected Sub objDSOTOfficeAssignment_Updating(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDSOTOfficeAssignment.Updating
        e.InputParameters("Emplid") = ViewState("Emplid")
        e.InputParameters("tlkpOverTimeLocation") = ViewState("tlkpOverTimeLocation")
    End Sub

    Protected Sub rcbEmployee_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        ViewState("Emplid") = e.Value
    End Sub

    Protected Sub rcbOffice_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        ViewState("tlkpOverTimeLocation") = e.Value
    End Sub

    Public Function ShowControlForInsert(ByVal IsInInsertMode As Boolean) As Boolean
        If IsInInsertMode Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ShowControlForEdit(ByVal IsInInsertMode As Boolean) As Boolean
        If IsInInsertMode Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Function GetOvertimeFirstOffice(EmplID As String) As Integer
        Dim OverTimeOffice As Integer
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
        Dim cm As New SqlCommand("uspGetOvertimeFirstOffice", cn)
        cm.CommandType = CommandType.StoredProcedure
        Dim parmEmplID As New SqlParameter("@EmplID", EmplID)
        cm.Parameters.Add(parmEmplID)
        cn.Open()

        Dim dtLookups As New DataTable
        Dim da As New SqlDataAdapter(cm)
        da.Fill(dtLookups)
        cn.Close()
        OverTimeOffice = dtLookups.Rows(0).Item("ID")
        Return OverTimeOffice
        'Return dtLookups.Rows(0).Item("ID")
    End Function
End Class
