Imports Telerik.Web.UI
Imports CustomEditors
Imports CustomEditors2
Imports System.Data
Imports System.Runtime.Serialization.Json
Imports System.IO
Imports System.Web.Services
Imports System.Runtime.Serialization
Imports System.Collections.Generic
Imports System.Linq
Imports StaffingUtilities
Imports StaffEmployees

Partial Class TAP


    Inherits System.Web.UI.Page
    Private fileId As Integer
    Private fileData As Byte() = Nothing
    Private fileName As String
    Private description As String = ""
    Private CurrentPositionID As Int32
    Private CurrentRequestReasonID As Int32
    Private CurrentLocationID As Int32
    Private CurrentProgramID As Int32
    Private ContactTypeIDs As ArrayList
    Private RecordCount As Int32
    Private EmailRecipients() As String
    Private EmailCC() As String
    Private EmailAttachments() As String
    Private EmailSubject As String
    Private EmailText As StringBuilder
    Dim SendRequestEmail As Boolean
    Dim SendApprovalEmail As Boolean
    Dim SendExtensionCreatedEmail As Boolean
    Dim SendExtensionApprovedEmail As Boolean

    'Private ContactPhoneNumbers As ArrayList

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                ' Initiate ViewState holder of filtering values to empty array lists
                ViewState("TAPPositionFilterValue") = New ArrayList()
                ViewState("ProgramFilterValue") = New ArrayList()
                ViewState("OfficeFilterValue") = New ArrayList()
                'ViewState("ActualEndDateFilterValue") = New ArrayList()
                'ViewState("ActualEndDateFilterOperator") = New ArrayList()
                ViewState("BeginActualEndDate") = ""
                ViewState("EndActualEndDate") = ""
            End If
            Dim lblTitle As Label = Me.Master.FindControl("lblTitle")
            lblTitle.Text = "Self Sufficiency TAP Requests"


            ' Apply the datasources to the radfilter custom field editorss
            Dim filterEditor As CustomEditors.RadCustomFilterDropDownEditor
            ' Position Title filter
            filterEditor = TryCast(rfTAP.FieldEditors(0), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("TAP Position")
            ' Current Office filter
            filterEditor = TryCast(rfTAP.FieldEditors(2), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Office Locations")
            ' Current Program filter
            filterEditor = TryCast(rfTAP.FieldEditors(3), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Program")
            'Dim filterDatePickerEditor As CustomEditors3.RadFilterDatePickerEditor
            'filterDatePickerEditor = TryCast(rfTAP.FieldEditors(3), CustomEditors3.RadFilterDatePickerEditor)

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Protected Sub rfTAP_FieldEditorCreating(sender As Object, e As RadFilterFieldEditorCreatingEventArgs) Handles rfTAP.FieldEditorCreating
        Try
            If e.EditorType = "RadCustomFilterDropDownEditor" Then
                e.Editor = New CustomEditors.RadCustomFilterDropDownEditor()
                'ElseIf e.EditorType = "RadFilterDatePickerEditor" Then
                '    e.Editor = New CustomEditors3.RadFilterDatePickerEditor
                'ElseIf e.EditorType = "RadFilterCheckBoxEditor" Then
                '    e.Editor = New RadFilterCheckBoxEditor
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Protected Sub rfTAP_ApplyExpressions(sender As Object, e As RadFilterApplyExpressionsEventArgs) Handles rfTAP.ApplyExpressions

        ' Applying the custom SQL queries for the RadFilter depending on what the user has selected
        Dim TAPPositionFilterValues As New ArrayList
        Dim ProgramFilterValues As New ArrayList
        Dim OfficeFilterValues As New ArrayList
        'Dim ActualEndDateValues As New ArrayList
        'Dim ActualEndDateOperators As New ArrayList

        ViewState("TAPPositionFilterValue") = TAPPositionFilterValues
        ViewState("ProgramFilterValue") = ProgramFilterValues
        ViewState("OfficeFilterValue") = OfficeFilterValues
        ViewState("BeginActualEndDate") = rdpBeginActualEndDate.SelectedDate
        ViewState("EndActualEndDate") = rdpEndActualEndDate.SelectedDate
        'ViewState("ActualEndDateFilterValue") = ActualEndDateValues
        'ViewState("ActualEndDateFilterOperator") = ActualEndDateOperators

        'Dim queryProvider As New RadFilterSqlQueryProvider()
        'queryProvider.ProcessGroup(e.ExpressionRoot)
        'Dim s As String = queryProvider.Result

        'ViewState("LinqQuery") = s

        Dim FilterFieldNames As New ArrayList
        Dim FilterValues As New ArrayList

        For Each expression As RadFilterExpression In e.ExpressionRoot.Expressions
            If System.Enum.GetName(GetType(RadFilterFunction), expression.FilterFunction) = "EqualTo" Then
                Select Case CType(expression, RadFilterEqualToFilterExpression(Of String)).FieldName
                    Case "Position Title"
                        TAPPositionFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                    Case "Program"
                        ProgramFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                    Case "Office"
                        OfficeFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                        'Case "Actual End Date"
                        '    ActualEndDateValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                        '    ActualEndDateOperators.Add("EqualTo")
                End Select
                ' Actual End Date is the only search filter that has operators other EqualTo Available so... if it isn't EqualTo then we know 
                ' we are dealing with the Actual End Date
                'ElseIf System.Enum.GetName(GetType(RadFilterFunction), expression.FilterFunction) = "LessThan" Then
                '    ActualEndDateValues.Add(CType(expression, Telerik.Web.UI.RadFilterLessThanFilterExpression(Of String)).Value)
                '    ActualEndDateOperators.Add("LessThan")
                'ElseIf System.Enum.GetName(GetType(RadFilterFunction), expression.FilterFunction) = "GreaterThan" Then
                '    ActualEndDateValues.Add(CType(expression, Telerik.Web.UI.RadFilterGreaterThanFilterExpression(Of String)).Value)
                '    ActualEndDateOperators.Add("GreaterThan")
            End If
            'Select Case CType(expression, RadFilterEqualToFilterExpression(Of String)).FieldName

            '    Case "Actual End Date"
            '        Select Case System.Enum.GetName(GetType(RadFilterFunction), expression.FilterFunction)
            '            Case "LessThan"
            '                ActualEndDateValues.Add(CType(expression, Telerik.Web.UI.RadFilterLessThanFilterExpression(Of String)).Value)
            '                ActualEndDateOperators.Add("LessThan")
            '            Case "GreaterThan"
            '                ActualEndDateValues.Add(CType(expression, Telerik.Web.UI.RadFilterGreaterThanFilterExpression(Of String)).Value)
            '                ActualEndDateOperators.Add("GreaterThan")
            '            Case "EqualTo"
            '                ActualEndDateValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
            '                ActualEndDateOperators.Add("EqualTo")
            '        End Select
            'End Select
        Next
        rgdTAP.Rebind()
    End Sub

    Protected Sub objTAPs_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objTAPs.Selecting
        e.InputParameters("Position") = ViewState("TAPPositionFilterValue")
        e.InputParameters("Program") = ViewState("ProgramFilterValue")
        e.InputParameters("Office") = ViewState("OfficeFilterValue")
        'e.InputParameters("ActualEndDate") = ViewState("ActualEndDateFilterValue")
        'e.InputParameters("ActualEndDateOperator") = ViewState("ActualEndDateFilterOperator")
        e.InputParameters("BeginActualEndDate") = ViewState("BeginActualEndDate")
        e.InputParameters("EndActualEndDate") = ViewState("EndActualEndDate")
    End Sub

    'Protected Sub rfTAP_ItemCommand(sender As Object, e As RadFilterCommandEventArgs) Handles rfTAP.ItemCommand
    '    ' This is a hack so that when the user removes an expression from the filter it will force the expression to actually be removed 
    '    ' and then reapply the filter
    '    If e.CommandName = RadFilter.RemoveExpressionCommandName Then
    '        Dim expr = e.ExpressionItem
    '        Dim grp = e.ExpressionItem.OwnerGroup
    '        grp.Expression.Expressions.RemoveAt(Array.IndexOf(grp.ChildItems.ToArray(), expr))
    '        rfTAP.FireApplyCommand()
    '    End If
    'End Sub

    'Protected Sub rgdTAP_EditCommand(sender As Object, e As GridCommandEventArgs) Handles rgdTAP.EditCommand
    '    Dim this As String = String.Empty
    'End Sub

    Protected Sub rgdTAP_InsertCommand(sender As Object, e As GridCommandEventArgs) Handles rgdTAP.InsertCommand
        Try
            'For document upload logic make sure that we are dealing with the detail table based on the datasourceid
            If e.Item.OwnerTableView.DataSourceID = "objTAPDocuments" Then
                Dim EmployeeNumber As String
                Dim LoginID As String
                Dim StartingIndex As Int32 = Me.User.Identity.Name.IndexOf("\") + 1
                LoginID = Me.User.Identity.Name.Substring(StartingIndex).ToUpper
                Dim clsEmployee As New Employees
                ' Even though this is a TAP request we can still use the same method to store the document as we do with a 1546
                EmployeeNumber = clsEmployee.GetEmployeeNumberByLoginID(LoginID)
                Dim dal As New Req1546DAL.DAL1546
                dal.Insert1546Document(fileId, fileName, fileData, EmployeeNumber)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Protected Sub rgdTAP_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles rgdTAP.ItemCommand
        If e.Item Is Nothing Then
            ' Probably a paging commmand so ignore
            Exit Sub
        End If
        If e.Item.OwnerTableView.DataSourceID = "objTAPs" Then

            Select Case e.CommandName

                Case RadGrid.InitInsertCommandName
                    ' this prevents null exception for Canceled checkbox on inserts
                    e.Canceled = True
                    Dim newValues As New System.Collections.Specialized.ListDictionary()
                    newValues("Bilingual") = False
                    e.Item.OwnerTableView.InsertItem(newValues)

                Case RadGrid.UpdateCommandName
                    Dim rdpToHR As RadDatePicker = CType(CType(e.Item, GridEditFormItem).FindControl("rdpToHR"), RadDatePicker)
                    If Not ViewState("CurrentRequestApproved") And Not rdpToHR.SelectedDate Is Nothing Then
                        If CType(CType(e.Item, GridEditFormItem).FindControl("chkSendEmail"), CheckBox).Checked Then
                            If CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                                CType(CType(e.Item, GridEditFormItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                                e.Canceled = True
                            End If
                            ConstructApprovalEmail(e)
                            SendApprovalEmail = True
                        Else
                            SendApprovalEmail = False
                        End If
                    Else
                        If Not rdpToHR.SelectedDate Is Nothing And CType(CType(e.Item, GridEditFormItem).FindControl("chkSendEmail"), CheckBox).Checked Then
                            If CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                                CType(CType(e.Item, GridEditFormItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                                e.Canceled = True
                            End If
                            ConstructApprovalEmail(e)
                            SendApprovalEmail = True
                        Else
                            SendApprovalEmail = False
                        End If
                    End If
                    If CType(CType(e.Item, GridEditFormItem).FindControl("chkSendEmail"), CheckBox).Checked And ViewState("ExtensionCreated") Then
                        If CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                            CType(CType(e.Item, GridEditFormItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                            e.Canceled = True
                        End If
                        ConstructApprovalEmail(e)
                        SendExtensionCreatedEmail = True
                    Else
                        SendExtensionCreatedEmail = False
                    End If
                    If CType(CType(e.Item, GridEditFormItem).FindControl("chkSendEmail"), CheckBox).Checked And ViewState("ExtensionApproved") Then
                        If CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                            CType(CType(e.Item, GridEditFormItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                            e.Canceled = True
                        End If
                        ConstructApprovalEmail(e)
                        SendExtensionApprovedEmail = True
                    Else
                        SendExtensionApprovedEmail = False
                    End If
                Case RadGrid.EditCommandName
                    ' Get current lookup column values from the grid and save them to use as input parameters when selecting the look up values
                    ' because if the value being saved belongs to a lookup entry that has been marked as inactive then we still want to include
                    ' it in the lookup table and the only way to do that is to have this id.
                    CurrentPositionID = CType(CType(e.Item, GridEditableItem)("tlkpPosition"), GridTableCell).Text
                    CurrentRequestReasonID = CType(CType(e.Item, GridEditableItem)("tlkpRequestReason"), GridTableCell).Text
                    CurrentLocationID = CType(CType(e.Item, GridEditableItem)("tlkpLocation"), GridTableCell).Text
                    CurrentProgramID = CType(CType(e.Item, GridEditableItem)("tlkpProgram"), GridTableCell).Text
                    'If CType(CType(e.Item, GridEditableItem)("DateToHR"), GridTableCell).Text = "&nbsp;" Then
                    '    ViewState("CurrentRequestApproved") = False
                    'Else
                    '    ViewState("CurrentRequestApproved") = True
                    'End If

                    ViewState("CurrentSelectedRecordID") = CType(e.Item, GridEditableItem).GetDataKeyValue("ID")
                    ViewState("ExtensionCreated") = False
                Case RadGrid.PerformInsertCommandName
                    If CType(CType(e.Item, GridEditFormInsertItem).FindControl("chkSendEmail"), CheckBox).Checked Then
                        If CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                            CType(CType(e.Item, GridEditFormInsertItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                            e.Canceled = True
                        Else
                            Dim rdpToExecs As RadDatePicker = CType(CType(e.Item, GridEditFormInsertItem).FindControl("rdpToExecs"), RadDatePicker)
                            If Not rdpToExecs Is Nothing Then
                                ConstructInitialRequestEmail(e)
                                SendRequestEmail = True
                            Else
                                SendRequestEmail = False
                            End If
                        End If
                    Else
                        SendRequestEmail = False
                    End If
            End Select
        ElseIf e.Item.OwnerTableView.DataSourceID = "objTAPDocuments" Then
            If e.CommandName = RadGrid.UpdateCommandName OrElse e.CommandName = RadGrid.PerformInsertCommandName Then

                Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
                fileId = CType(e, GridCommandEventArgs).Item.OwnerTableView.ParentItem.GetDataKeyValue("ID")
                fileName = System.IO.Path.GetFileName((TryCast(item.EditManager.GetColumnEditor("FileName"), GridTextBoxColumnEditor)).Text)
                fileData = (TryCast(item.EditManager.GetColumnEditor("Data"), GridAttachmentColumnEditor)).UploadedFileContent

                If fileData.Length = 0 OrElse fileName.Trim() = String.Empty Then
                    e.Canceled = True
                    rgdTAP.MasterTableView.DetailTables(0).Controls.Add(New LiteralControl("<b style='color:red;'>No file uploaded. Action canceled.</b>"))
                End If
            End If
            If (e.CommandName = RadGrid.DownloadAttachmentCommandName) Then
                Dim args As GridDownloadAttachmentCommandEventArgs = TryCast(e, GridDownloadAttachmentCommandEventArgs)
                Dim DocumentId As Integer = DirectCast(args.AttachmentKeyValues("ID"), Integer)
                Dim clsTAPRequests As New Req1546DAL.DAL1546
                Dim filename As String = ""
                Dim binarydata As Byte()

                Dim dt As DataTable
                ' Even though this is a TAP request we can still use the same method to read the document as we do with a 1546
                dt = clsTAPRequests.Get1546DocumentObject(DocumentId)
                If dt.Rows.Count > 0 Then
                    filename = dt.Rows(0).Item("Filename")
                    binarydata = dt.Rows(0).Item("Data")

                    Response.Clear()
                    Response.ContentType = "application/octet-stream"
                    Response.AddHeader("content-disposition", "attachment; filename=" + filename)
                    Response.BinaryWrite(binarydata)
                    Response.[End]()
                End If
            End If
        End If

    End Sub

    Protected Sub rgdTAP_ItemCreated(sender As Object, e As GridItemEventArgs) Handles rgdTAP.ItemCreated
        If TypeOf e.Item Is GridFilteringItem Then
            Dim item As GridFilteringItem = e.Item
            Dim txt As TextBox = CType(item("ID").Controls(0), TextBox)
            txt.Attributes.Add("onkeypress", "return isNumericKey(event);")
        End If
        If TypeOf e.Item Is GridEditableItem AndAlso e.Item.IsInEditMode Then
            If e.Item.OwnerTableView.DataSourceID = "objTAPDocuments" Then
                'Assign client event to attachment column when going into edit mode - see JavaScript function uploadFileSelected
                Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
                Dim upload As RadUpload = (TryCast(item.EditManager.GetColumnEditor("Data"), GridAttachmentColumnEditor)).RadUploadControl
                upload.OnClientFileSelected = "uploadFileSelected"
            End If
        End If
    End Sub

    Protected Sub rgdTAP_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles rgdTAP.ItemDataBound
        If TypeOf e.Item Is GridDataItem Then
            If e.Item.OwnerTableView.DataSourceID = "objTAPs" Then
                If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
                    CType(e.Item, GridDataItem)("EditCommandColumn").Controls(0).Visible = False
                End If
                If Not HttpContext.Current.User.IsInRole("SS Staff Delete") Then
                    CType(e.Item, GridDataItem)("DeleteTAPButton").Controls(0).Visible = False
                End If
            Else
                ' disable delete document button in detail table for non admin
                If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
                    CType(e.Item, GridDataItem)("DeleteColumn").Controls(0).Visible = False
                End If
            End If
        End If
        If TypeOf e.Item Is GridEditFormItem And e.Item.IsInEditMode Then
            If Not TypeOf (e.Item) Is GridEditFormInsertItem Then
                If e.Item.OwnerTableView.DataSourceID = "objTAPs" Then
                    ' Preselect the supervisor combo box values when in edit mode since we are using load on demand we can't use regular binding
                    ' because the control is empty at first
                    Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                    ' Load the initial tool tip for the rcbPositionTitle with the currently selected item. This will change in client side JavaScript
                    ' when another item is selected
                    CType(e.Item.FindControl("rcbPositionTitle"), RadComboBox).ToolTip = CType(e.Item.FindControl("rcbPositionTitle"), RadComboBox).SelectedItem.Text



                    If CType(e.Item.FindControl("rdpToHr"), RadDatePicker).SelectedDate Is Nothing Then
                        CType(e.Item.FindControl("rgdExtensions"), RadGrid).Enabled = False
                    Else
                        CType(e.Item.FindControl("rgdExtensions"), RadGrid).Enabled = True
                    End If

                    If item("FirstLineSupervisorName").Text <> "&nbsp;" Then
                        Dim comboFirstLineSupervisor As RadComboBox = DirectCast(item.FindControl("rcbFirstLineSupervisor"), RadComboBox)
                        Dim preselectedFirstItem As New RadComboBoxItem()
                        preselectedFirstItem.Text = Server.HtmlDecode(item("FirstLineSupervisorName").Text)
                        preselectedFirstItem.Value = Server.HtmlDecode(item("FirstLineSupervisorEmplID").Text)
                        comboFirstLineSupervisor.Items.Insert(0, preselectedFirstItem)
                        comboFirstLineSupervisor.SelectedIndex = 0
                        comboFirstLineSupervisor.DataBind()
                    End If
                    If item("SecondLineSupervisorName").Text <> "&nbsp;" Then
                        Dim comboSecondLineSupervisor As RadComboBox = DirectCast(item.FindControl("rcbSecondLineSupervisor"), RadComboBox)
                        Dim preselectedSecondItem As New RadComboBoxItem()
                        preselectedSecondItem.Text = Server.HtmlDecode(item("SecondLineSupervisorName").Text)
                        preselectedSecondItem.Value = Server.HtmlDecode(item("SecondLineSupervisorEmplID").Text)
                        comboSecondLineSupervisor.Items.Insert(0, preselectedSecondItem)
                        comboSecondLineSupervisor.SelectedIndex = 0
                        comboSecondLineSupervisor.DataBind()
                    End If
                    EmailOnEdit(e)
                End If
            Else
                If e.Item.OwnerTableView.DataSourceID = "objTAPs" Then
                    EmailOnInsert(e)
                    ' Do not show the extensions grid in the edit form if adding a new record 
                    CType(CType(e.Item, GridEditFormItem).FindControl("pnlExtensions"), Panel).Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub EmailOnInsert(ByVal e As Telerik.Web.UI.GridItemEventArgs)
        CType(e.Item.FindControl("pnlEmail"), Panel).Enabled = False
        CType(e.Item.FindControl("MailEditor"), RadEditor).Enabled = False
        CType(e.Item.FindControl("chkSendEmail"), CheckBox).Visible = False
    End Sub

    Protected Sub EmailOnEdit(ByVal e As Telerik.Web.UI.GridItemEventArgs)
        'If Not ViewState("CurrentRequestApproved") And Not rdpToHR.SelectedDate Is Nothing Then
        Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
        If CType(e.Item.FindControl("rdpToHr"), RadDatePicker).SelectedDate Is Nothing Then
            CType(e.Item.FindControl("pnlEmail"), Panel).Enabled = False
            CType(e.Item.FindControl("MailEditor"), RadEditor).Enabled = False
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Visible = False
            CType(e.Item.FindControl("rgdExtensions"), RadGrid).Enabled = False
        Else
            CType(e.Item.FindControl("pnlEmail"), Panel).Enabled = True
            CType(e.Item.FindControl("MailEditor"), RadEditor).Enabled = True
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Visible = True
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Checked = False
            CType(e.Item.FindControl("rgdExtensions"), RadGrid).Enabled = True
            'Email crap
            Dim dtDefaultEmailRecipient As DataTable = Utilities.GetDefaultEmailAddresses("1546 Default Email CC")
            Dim clsEmployees As New Employees

            Dim dtEmployee As New System.Data.DataTable
            Dim txtEmailCC As RadTextBox = CType(e.Item.FindControl("txtEmailCC"), RadTextBox)
            txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"
            If dtDefaultEmailRecipient.Rows.Count > 0 Then
                dtEmployee = clsEmployees.GetEmployeeInformation(dtDefaultEmailRecipient.Rows(0).Item("EmplID"))
                If dtEmployee.Rows.Count > 0 Then
                    For Each dr As DataRow In dtEmployee.Rows
                        txtEmailCC.Text = txtEmailCC.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                    Next
                End If
            End If

            Dim txtEmailRecipients As RadTextBox = CType(e.Item.FindControl("txtEmailRecipients"), RadTextBox)
            If item("FirstLineSupervisorName").Text <> "&nbsp;" Then
                dtEmployee.Clear()
                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(item("FirstLineSupervisorEmplID").Text))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            If item("SecondLineSupervisorName").Text <> "&nbsp;" Then
                dtEmployee.Clear()
                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(item("SecondLineSupervisorEmplID").Text))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            Dim txtSubject As RadTextBox = CType(CType(e.Item, GridEditFormItem).FindControl("txtSubject"), RadTextBox)
            txtSubject.Text = "HR 3318 - TAP Job Order Request Approved"
            Dim txtEmail As Telerik.Web.UI.RadEditor = CType(e.Item.FindControl("MailEditor"), RadEditor)
            Dim srEmailContent As StreamReader
            srEmailContent = File.OpenText(Server.MapPath(".\App_Data\EmailTexts\") & "TAPApprovalEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()
            strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", item("PositionTitle").Text)
            txtEmail.Content = strEmailContent
        End If
    End Sub
    Protected Sub rcbEmployees_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim clsEmployees As New Employees
        Dim dtEmployee As System.Data.DataTable
        dtEmployee = clsEmployees.GetEmployeeInformation(CType(sender, RadComboBox).SelectedValue)
        Select Case CType(sender, RadComboBox).ID
            Case "rcbFirstLineSupervisor"
                Dim txtFirstLineSupervisorEmplID As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtFirstLineSupervisorEmplID")
                If dtEmployee.Rows.Count = 0 Then
                    txtFirstLineSupervisorEmplID.Text = String.Empty
                Else
                    txtFirstLineSupervisorEmplID.Text = dtEmployee.Rows(0).Item("EmplID")
                    If TypeOf sender.parent.parent.parent Is GridEditFormInsertItem Then
                        Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent, GridEditFormInsertItem).FindControl("txtEmailRecipients")
                        txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                    Else
                        If Not CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("rdpToHR"), RadDatePicker).SelectedDate Is Nothing Then
                            Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent, GridEditFormItem).FindControl("txtEmailRecipients")
                            txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                        End If
                    End If
                End If
            Case "rcbSecondLineSupervisor"
                Dim txtSecondLineSupervisorEmplID As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtSecondLineSupervisorEmplID")
                If dtEmployee.Rows.Count = 0 Then
                    txtSecondLineSupervisorEmplID.Text = String.Empty
                Else
                    txtSecondLineSupervisorEmplID.Text = dtEmployee.Rows(0).Item("EmplID")
                    If TypeOf sender.parent.parent.parent Is GridEditFormInsertItem Then
                        Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent, GridEditFormInsertItem).FindControl("txtEmailRecipients")
                        txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                    Else
                        If Not CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("rdpToHR"), RadDatePicker).SelectedDate Is Nothing Then
                            Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent, GridEditFormItem).FindControl("txtEmailRecipients")
                            txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                        End If
                    End If
                End If
        End Select
    End Sub

    Protected Sub objDSVacantPositions_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSVacantPositions.Selecting
        e.InputParameters("CurrentID") = CurrentPositionID
    End Sub

    Protected Sub objRequestReasons_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objRequestReasons.Selecting
        e.InputParameters("CurrentID") = CurrentRequestReasonID
    End Sub

    Protected Sub objDSLocation_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSLocation.Selecting
        e.InputParameters("CurrentID") = CurrentLocationID
    End Sub

    Protected Sub objDSPrograms_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSPrograms.Selecting
        e.InputParameters("CurrentID") = CurrentProgramID
    End Sub

    Protected Sub objDSExtensionApproval_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSExtensionApproval.Selecting
        e.InputParameters("CurrentID") = CurrentProgramID
    End Sub

    Protected Sub rgdTAP_ItemEvent(sender As Object, e As GridItemEventArgs) Handles rgdTAP.ItemEvent
        If TypeOf e.EventInfo Is GridInitializePagerItem Then
            RecordCount = CType(e.EventInfo, GridInitializePagerItem).PagingManager.DataSourceCount
            lblRecordCount.Text = "Record Count: " & RecordCount
        End If
    End Sub

    Protected Sub rgdTAP_ItemInserted(sender As Object, e As GridInsertedEventArgs) Handles rgdTAP.ItemInserted
        If Not SendRequestEmail Then
            If Not CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailAttachments"), RadTextBox).Text = String.Empty Then
                Dim strEmailAttachments As String = CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailAttachments"), RadTextBox).Text
                strEmailAttachments = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
                ViewState("EmailAttachments") = strEmailAttachments
                EmailAttachments = Split(ViewState("EmailAttachments"), ";")
                For i = 0 To EmailAttachments.Count - 1
                    If File.Exists(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString) Then
                        System.IO.File.Delete(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString)
                    End If
                Next
            End If
            Exit Sub
        End If
        If EmailRecipients Is Nothing Then
            Exit Sub
        End If
        If EmailAttachments(0) = "" Then
            ReDim EmailAttachments(0)
            EmailAttachments(0) = "None"
        End If
        Utilities.SendEmail(EmailRecipients, EmailCC, EmailSubject, EmailText.ToString, EmailAttachments)
        ViewState("EmailAttachments") = Nothing
    End Sub

    Protected Sub rgdTAP_PreRender(sender As Object, e As EventArgs) Handles rgdTAP.PreRender


        If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
            For Each cmditm As GridCommandItem In rgdTAP.MasterTableView.GetItems(GridItemType.CommandItem)
                Dim btnAdd As Button = CType(cmditm.FindControl("AddNewRecordButton"), Button)
                btnAdd.Visible = False
                Dim lnkAdd As LinkButton = CType(cmditm.FindControl("InitInsertButton"), LinkButton)
                lnkAdd.Visible = False
            Next
            rgdTAP.MasterTableView.DetailTables(0).CommandItemSettings.ShowAddNewRecordButton = False
        End If
    End Sub


    Protected Sub ConstructInitialRequestEmail(ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        EmailRecipients = Split(CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailRecipients"), RadTextBox).Text, ";")
        EmailCC = Split(CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailCC"), RadTextBox).Text, ";")
        EmailSubject = CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtSubject"), RadTextBox).Text
        Dim strEmailAttachments As String = CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailAttachments"), RadTextBox).Text
        If strEmailAttachments.Length > 0 Then
            strEmailAttachments = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
        End If
        ViewState("EmailAttachments") = strEmailAttachments
        EmailAttachments = Split(ViewState("EmailAttachments").ToString, ";")
        ' Get default email text from database
        EmailText = New StringBuilder()
        Dim strEmailContent As String
        strEmailContent = CType(CType(e.Item, GridEditFormInsertItem).FindControl("MailEditor"), RadEditor).Content
        strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", CType(CType(e.Item, GridEditFormInsertItem).FindControl("rcbPositionTitle"), RadComboBox).Text)
        EmailText.Append(strEmailContent)
    End Sub

    Protected Sub ConstructApprovalEmail(ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        'Dim PositionNumber As String = String.Empty
        'Select Case e.CommandName

        '    Case RadGrid.PerformInsertCommandName
        '        PositionNumber = CType(CType(e.Item, GridEditFormItem).FindControl("txtPositionNumber"), RadTextBox).Text
        'End Select
        EmailRecipients = Split(CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text, ";")
        EmailCC = Split(CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailCC"), RadTextBox).Text, ";")
        EmailSubject = CType(CType(e.Item, GridEditFormItem).FindControl("txtSubject"), RadTextBox).Text
        If Not CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailAttachments"), RadTextBox).Text = String.Empty Then
            Dim strEmailAttachments As String = CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailAttachments"), RadTextBox).Text
            strEmailAttachments = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
            ViewState("EmailAttachments") = strEmailAttachments
            EmailAttachments = Split(ViewState("EmailAttachments"), ";")
        End If
        ' Get default email text from database
        EmailText = New StringBuilder()
        EmailText.Append(CType(CType(e.Item, GridEditFormItem).FindControl("MailEditor"), RadEditor).Content)
    End Sub



#Region "Email form code"

    Protected Sub rcbEmailRecipient_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim clsEmployees As New Employees
        Dim dtEmployee As System.Data.DataTable
        dtEmployee = clsEmployees.GetEmployeeInformation(CType(sender, RadComboBox).SelectedValue)
        If dtEmployee.Rows.Count = 0 Then
            ViewState("EmailRecipient") = Nothing
            Exit Sub
        End If
        ViewState("EmailRecipient") = dtEmployee.Rows(0).Item("LOGIN_ID")
    End Sub

    Protected Sub rcbEmailCC_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim clsEmployees As New Employees
        Dim dtEmployee As System.Data.DataTable
        dtEmployee = clsEmployees.GetEmployeeInformation(CType(sender, RadComboBox).SelectedValue)
        If dtEmployee.Rows.Count = 0 Then
            ViewState("EmailCC") = Nothing
            Exit Sub
        End If
        ViewState("EmailCC") = dtEmployee.Rows(0).Item("LOGIN_ID")
    End Sub

    Protected Sub btnAddEmailRecipient_Click(sender As Object, e As EventArgs)
        If ViewState("EmailRecipient") Is Nothing Then
            Exit Sub
        End If
        Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("txtEmailRecipients")
        txtEmailRecipients.Text = txtEmailRecipients.Text + ViewState("EmailRecipient").ToString.ToLower & "@riversidedpss.org;"
    End Sub

    'Protected Sub btnRemoveEmailRecipient_Click(sender As Object, e As EventArgs)
    '    Dim rlbEmailRecipients As RadListBox = CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("rlbEmailRecipients"), RadListBox)
    '    If rlbEmailRecipients.Items.Count > 0 Then
    '        If rlbEmailRecipients.SelectedIndex < 0 Then
    '            Exit Sub
    '        End If
    '        Dim i As Integer = rlbEmailRecipients.SelectedIndex
    '        rlbEmailRecipients.SelectedItem.Remove()
    '        If rlbEmailRecipients.Items.Count = 0 Then
    '            rlbEmailRecipients.CssClass = "RadListBox_Default"
    '        End If
    '    End If
    'End Sub

    Protected Sub btnAddEmailCC_Click(sender As Object, e As EventArgs)
        If ViewState("EmailCC") Is Nothing Then
            Exit Sub
        End If
        Dim txtEmailCC As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("txtEmailCC")
        txtEmailCC.Text = txtEmailCC.Text + ViewState("EmailCC").ToString.ToLower & "@riversidedpss.org;"
    End Sub

    'Protected Sub btnRemoveEmailCC_Click(sender As Object, e As EventArgs)
    '    Dim rlbEmailCCs As RadListBox = CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("rlbEmailCCs"), RadListBox)
    '    If rlbEmailCCs.Items.Count > 0 Then
    '        If rlbEmailCCs.SelectedIndex < 0 Then
    '            Exit Sub
    '        End If
    '        Dim i As Integer = rlbEmailCCs.SelectedIndex
    '        rlbEmailCCs.SelectedItem.Remove()
    '        If rlbEmailCCs.Items.Count = 0 Then
    '            rlbEmailCCs.CssClass = "RadListBox_Default"
    '        End If
    '    End If
    'End Sub

    'Protected Sub AsyncUpload1_FileUploaded(sender As Object, e As FileUploadedEventArgs)
    '    Dim this As String = String.Empty
    'End Sub
#End Region

    Protected Sub rdpToHR_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs)
        If Not CType(sender, RadDatePicker).SelectedDate Is Nothing Then
            CType(CType(sender.parent, WebControl).FindControl("pnlEmail"), Panel).Enabled = True
            CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor).Enabled = True
            CType(CType(sender.parent, WebControl).FindControl("chkSendEmail"), CheckBox).Visible = True
            'Email crap
            Dim dtDefaultEmailRecipient As DataTable = Utilities.GetDefaultEmailAddresses("1546 Default Email CC")
            Dim clsEmployees As New Employees

            Dim dtEmployee As New System.Data.DataTable
            Dim txtEmailCC As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailCC"), RadTextBox)
            txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"
            If dtDefaultEmailRecipient.Rows.Count > 0 Then
                dtEmployee = clsEmployees.GetEmployeeInformation(dtDefaultEmailRecipient.Rows(0).Item("EmplID"))
                If dtEmployee.Rows.Count > 0 Then
                    For Each dr As DataRow In dtEmployee.Rows
                        txtEmailCC.Text = txtEmailCC.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                    Next
                End If
            End If
            If Not CType(CType(sender.parent, WebControl).FindControl("rcbFirstLineSupervisor"), RadComboBox).Text = String.Empty Then
                Dim FirstLineSupervisor As String = CType(CType(sender.parent, WebControl).FindControl("rcbFirstLineSupervisor"), RadComboBox).SelectedValue
                Dim txtEmailRecipients As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailRecipients"), RadTextBox)
                dtEmployee.Clear()
                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(FirstLineSupervisor))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            If Not CType(CType(sender.parent, WebControl).FindControl("rcbSecondLineSupervisor"), RadComboBox).Text = String.Empty Then
                Dim SecondLineSupervisor As String = CType(CType(sender.parent, WebControl).FindControl("rcbSecondLineSupervisor"), RadComboBox).SelectedValue
                Dim txtEmailRecipients As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailRecipients"), RadTextBox)
                dtEmployee.Clear()
                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(SecondLineSupervisor))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            'Dim dt As DataTable = Utilities.GetDefaultEmailText("Email Text TAP Approval")
            Dim txtSubject As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtSubject"), RadTextBox)
            txtSubject.Text = "HR 3318 - TAP Job Order Request Approved"
            Dim txtEmail As Telerik.Web.UI.RadEditor = CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor)
            Dim srEmailContent As StreamReader
            srEmailContent = File.OpenText(Server.MapPath(".\App_Data\EmailTexts\") & "TAPApprovalEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()

            strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", CType(CType(sender.parent, WebControl).FindControl("rcbPositionTitle"), RadComboBox).Text)
            txtEmail.Content = strEmailContent
        End If

    End Sub

    Protected Sub rfTAP_ItemCommand(sender As Object, e As RadFilterCommandEventArgs) Handles rfTAP.ItemCommand
        ' This is a hack so that when the user removes an expression from the filter it will force the expression to actually be removed 
        ' and then reapply the filter
        If e.CommandName = RadFilter.RemoveExpressionCommandName Then
            Dim expr = e.ExpressionItem
            Dim grp = e.ExpressionItem.OwnerGroup
            grp.Expression.Expressions.RemoveAt(Array.IndexOf(grp.ChildItems.ToArray(), expr))
            rfTAP.FireApplyCommand()
        End If
    End Sub

    Protected Sub objTAPs_Updated(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles objTAPs.Updated
        If Not SendApprovalEmail And Not SendExtensionApprovedEmail And Not SendExtensionCreatedEmail Then
            If Not ViewState("EmailAttachments") Is Nothing Then
                'Dim NumberOfAttachments = Split(ViewState("EmailAttachments").ToString, ";").Length - 1
                EmailAttachments = Split(ViewState("EmailAttachments").ToString, ";")
                For i = 0 To EmailAttachments.Count - 1
                    If File.Exists(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString) Then
                        System.IO.File.Delete(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString)
                    End If
                Next
            End If
            Exit Sub
        End If
        If EmailAttachments Is Nothing Then
            ReDim EmailAttachments(0)
            EmailAttachments(0) = "None"
        End If
        Utilities.SendEmail(EmailRecipients, EmailCC, EmailSubject, EmailText.ToString, EmailAttachments)
        ViewState("EmailAttachments") = Nothing
    End Sub

    Protected Sub objDSExtensions_Inserting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDSExtensions.Inserting
        e.InputParameters("tbl1546ID") = ViewState("CurrentSelectedRecordID")
    End Sub

    Protected Sub objDSExtensions_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSExtensions.Selecting
        e.InputParameters("tbl1546ID") = ViewState("CurrentSelectedRecordID")
    End Sub

    Protected Sub rcbPositionTitle_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        CType(sender, RadComboBox).ToolTip = e.Text
    End Sub

    Protected Sub rgdExtensions_ItemInserted(sender As Object, e As GridInsertedEventArgs)
        Dim formItem As GridEditFormItem = sender.parent.parent.parent.parent
        CType(formItem.FindControl("pnlEmail"), Panel).Enabled = True
        CType(formItem.FindControl("MailEditor"), RadEditor).Enabled = True
        CType(formItem.FindControl("chkSendEmail"), CheckBox).Visible = True
        CType(formItem.FindControl("chkSendEmail"), CheckBox).Checked = True
        'Email crap
        Dim dtDefaultEmailRecipient As DataTable = Utilities.GetDefaultEmailAddresses("1546 Default Email CC")
        Dim clsEmployees As New Employees

        'Dim dtEmployee As New System.Data.DataTable
        Dim txtEmailCC As RadTextBox = CType(formItem.FindControl("txtEmailCC"), RadTextBox)
        txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"
        'If dtDefaultEmailRecipient.Rows.Count > 0 Then
        '    dtEmployee = clsEmployees.GetEmployeeInformation(dtDefaultEmailRecipient.Rows(0).Item("EmplID"))
        '    If dtEmployee.Rows.Count > 0 Then
        '        For Each dr As DataRow In dtEmployee.Rows
        '            txtEmailCC.Text = txtEmailCC.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
        '        Next
        '    End If
        'End If
        'If Not CType(formItem.FindControl("rcbFirstLineSupervisor"), RadComboBox).Text = String.Empty Then
        '    Dim FirstLineSupervisor As String = CType(formItem.FindControl("rcbFirstLineSupervisor"), RadComboBox).SelectedValue
        '    Dim txtEmailRecipients As RadTextBox = CType(formItem.FindControl("txtEmailRecipients"), RadTextBox)
        '    dtEmployee.Clear()
        '    dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(FirstLineSupervisor))
        '    If dtEmployee.Rows.Count > 0 Then
        '        txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
        '    End If
        'End If
        'If Not CType(formItem.FindControl("rcbSecondLineSupervisor"), RadComboBox).Text = String.Empty Then
        '    Dim SecondLineSupervisor As String = CType(formItem.FindControl("rcbSecondLineSupervisor"), RadComboBox).SelectedValue
        '    Dim txtEmailRecipients As RadTextBox = CType(formItem.FindControl("txtEmailRecipients"), RadTextBox)
        '    dtEmployee.Clear()
        '    dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(SecondLineSupervisor))
        '    If dtEmployee.Rows.Count > 0 Then
        '        txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
        '    End If
        'End If
        Dim txtSubject As RadTextBox = CType(formItem.FindControl("txtSubject"), RadTextBox)
        txtSubject.Text = "HR 4429 - TAP Extension Request Received"
        Dim txtEmail As Telerik.Web.UI.RadEditor = CType(formItem.FindControl("MailEditor"), RadEditor)
        Dim srEmailContent As StreamReader
        srEmailContent = File.OpenText(Server.MapPath(".\App_Data\EmailTexts\") & "TAPExtensionCreatedEmail.html")
        Dim strEmailContent As String = srEmailContent.ReadToEnd()

        strEmailContent = Replace(strEmailContent, "&amp;&amp;EmployeeName", CType(formItem.FindControl("txtFirstName"), RadTextBox).Text &
        " " & CType(formItem.FindControl("txtLastName"), RadTextBox).Text)
        txtEmail.Content = strEmailContent
        ViewState("ExtensionCreated") = True
    End Sub

    Protected Sub rdpDateToHR_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs)
        If ViewState("CurrentExtensionRequestApproved") = True Then
            ViewState("ExtensionApproved") = True
        End If
    End Sub

    Protected Sub rgdExtensions_ItemDataBound(sender As Object, e As GridItemEventArgs)
        If TypeOf e.Item Is GridEditFormItem And e.Item.IsInEditMode Then
            If Not TypeOf (e.Item) Is GridEditFormInsertItem Then
                If CType(e.Item.FindControl("rdpDateToHr"), RadDatePicker).SelectedDate Is Nothing Then
                    ViewState("CurrentExtensionRequestApproved") = False
                Else
                    ViewState("CurrentExtensionRequestApproved") = True
                End If
            End If
        End If
    End Sub

    Protected Sub rgdExtensions_ItemUpdated(sender As Object, e As GridUpdatedEventArgs)
        If Not ViewState("ExtensionApproved") Then
            Dim formItem As GridEditFormItem = sender.parent.parent.parent.parent
            CType(formItem.FindControl("pnlEmail"), Panel).Enabled = True
            CType(formItem.FindControl("MailEditor"), RadEditor).Enabled = True
            CType(formItem.FindControl("chkSendEmail"), CheckBox).Visible = True
            CType(formItem.FindControl("chkSendEmail"), CheckBox).Checked = True
            'Email crap
            Dim dtDefaultEmailRecipient As DataTable = Utilities.GetDefaultEmailAddresses("1546 Default Email CC")
            Dim clsEmployees As New Employees

            'Dim dtEmployee As New System.Data.DataTable
            Dim txtEmailCC As RadTextBox = CType(formItem.FindControl("txtEmailCC"), RadTextBox)
            txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"
            'If dtDefaultEmailRecipient.Rows.Count > 0 Then
            '    dtEmployee = clsEmployees.GetEmployeeInformation(dtDefaultEmailRecipient.Rows(0).Item("EmplID"))
            '    If dtEmployee.Rows.Count > 0 Then
            '        For Each dr As DataRow In dtEmployee.Rows
            '            txtEmailCC.Text = txtEmailCC.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
            '        Next
            '    End If
            'End If
            'If Not CType(formItem.FindControl("rcbFirstLineSupervisor"), RadComboBox).Text = String.Empty Then
            '    Dim FirstLineSupervisor As String = CType(formItem.FindControl("rcbFirstLineSupervisor"), RadComboBox).SelectedValue
            '    Dim txtEmailRecipients As RadTextBox = CType(formItem.FindControl("txtEmailRecipients"), RadTextBox)
            '    dtEmployee.Clear()
            '    dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(FirstLineSupervisor))
            '    If dtEmployee.Rows.Count > 0 Then
            '        txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
            '    End If
            'End If
            'If Not CType(formItem.FindControl("rcbSecondLineSupervisor"), RadComboBox).Text = String.Empty Then
            '    Dim SecondLineSupervisor As String = CType(formItem.FindControl("rcbSecondLineSupervisor"), RadComboBox).SelectedValue
            '    Dim txtEmailRecipients As RadTextBox = CType(formItem.FindControl("txtEmailRecipients"), RadTextBox)
            '    dtEmployee.Clear()
            '    dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(SecondLineSupervisor))
            '    If dtEmployee.Rows.Count > 0 Then
            '        txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
            '    End If
            'End If
            Dim txtSubject As RadTextBox = CType(formItem.FindControl("txtSubject"), RadTextBox)
            txtSubject.Text = "HR 4429 - TAP Extension Request Approved"
            Dim txtEmail As Telerik.Web.UI.RadEditor = CType(formItem.FindControl("MailEditor"), RadEditor)
            Dim srEmailContent As StreamReader
            srEmailContent = File.OpenText(Server.MapPath(".\App_Data\EmailTexts\") & "TAPExtensionApprovedEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()

            strEmailContent = Replace(strEmailContent, "&amp;&amp;EmployeeName", CType(formItem.FindControl("txtFirstName"), RadTextBox).Text &
            " " & CType(formItem.FindControl("txtLastName"), RadTextBox).Text)
            txtEmail.Content = strEmailContent
            ViewState("ExtensionCreated") = True
        End If
    End Sub



    Protected Sub rdpToExecs_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs)
        If Not CType(sender, RadDatePicker).SelectedDate Is Nothing Then
            CType(CType(sender.parent, WebControl).FindControl("pnlEmail"), Panel).Enabled = True
            CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor).Enabled = True
            CType(CType(sender.parent, WebControl).FindControl("chkSendEmail"), CheckBox).Visible = True
            CType(CType(sender.parent, WebControl).FindControl("chkSendEmail"), CheckBox).Checked = True
            CType(CType(sender.parent, WebControl).FindControl("rgdExtensions"), RadGrid).Enabled = True
            Dim clsEmployees As New Employees

            Dim txtEmailCC As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailCC"), RadTextBox)
            txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"

            Dim dtEmployee As New System.Data.DataTable

            If CType(CType(sender.parent, WebControl).FindControl("rcbFirstLineSupervisor"), RadComboBox).Text <> "&nbsp;" Then
                Dim FirstLineSupervisor As String = CType(CType(sender.parent, WebControl).FindControl("rcbFirstLineSupervisor"), RadComboBox).SelectedValue
                Dim txtEmailRecipients As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailRecipients"), RadTextBox)
                dtEmployee.Clear()
                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(FirstLineSupervisor))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            If CType(CType(sender.parent, WebControl).FindControl("rcbSecondLineSupervisor"), RadComboBox).Text <> "&nbsp;" Then
                dtEmployee.Clear()
                Dim txtEmailRecipients As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailRecipients"), RadTextBox)
                Dim SecondLineSupervisor As String = CType(CType(sender.parent, WebControl).FindControl("rcbSecondLineSupervisor"), RadComboBox).SelectedValue
                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(SecondLineSupervisor))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            Dim txtSubject As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtSubject"), RadTextBox)
            txtSubject.Text = "HR 3318 - TAP Job Order Request Received"
            Dim txtEmail As Telerik.Web.UI.RadEditor = CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor)
            Dim srEmailContent As StreamReader
            srEmailContent = File.OpenText(Server.MapPath(".\App_Data\EmailTexts\") & "TAPRequestCreatedEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()
            strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", CType(CType(sender.parent, WebControl).FindControl("rcbPositionTitle"), RadComboBox).Text)
            txtEmail.Content = strEmailContent
        End If
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As ImageClickEventArgs)
        ' I KNOW I AM HARDCODING. THERE WAS NO WAY AROUND IT.
        Dim exportColumnNames As List(Of String) = New List(Of String)(New String() {"ID", "RequestDate", "Bilingual", "PositionTitle", "RequestReason", "Location", "Program",
                                                                       "FirstLineSupervisorName", "SecondLineSupervisorName", "ContactNumberFormatted", "DateToExecs", "DateToHR", "StartDate", "AntcpEndDate", "ActualEndDate",
                                                                       "Name", "Comments"})

        Dim exportColumnHeaders As List(Of String) = New List(Of String)(New String() {"Reques #", "Request Date", "Bilingual", "Position", "Reason", "Location", "Program",
                                                                         "1st Line Sup", "2nd Line Sup", "Contact Number", "Dt To Execs", "Dt To HR", "Start Dt", "Exp End Dt", "Actual End Dt",
                                                                       "Name", "Comments"})

        Dim dt As New DataTable
        Dim clsTAP As New TAPDAL.TAP
        If ViewState("BeginActualEndDate") = "" Then
            ViewState("BeginActualEndDate") = "1/1/0001 12:00:00 AM"
        End If
        If ViewState("EndActualEndDate") = "" Then
            ViewState("EndActualEndDate") = "1/1/0001 12:00:00 AM"
        End If
        dt = clsTAP.GetTAPs(ViewState("TAPPositionFilterValue"), ViewState("ProgramFilterValue"), ViewState("OfficeFilterValue"), ViewState("BeginActualEndDate"), ViewState("EndActualEndDate"))

        Response.ClearContent()
        Response.Clear()
        Response.ContentType = "text/csv"
        Response.AddHeader("Content-Disposition", "attachment; filename=TAP.csv;")

        Dim sb As New StringBuilder()
        Dim sep As String = ""
        Dim columnCount As Int32 = dt.Columns.Count - 1

        'Column Headers
        Dim columnNames(exportColumnNames.Count - 1) As String
        Dim i As Int32 = 0
        Dim j As Int32 = 0
        Do While i < columnCount
            If exportColumnNames.Contains(dt.Columns(i).ToString) Then
                columnNames(j) = exportColumnHeaders(j)
                j = j + 1
            End If
            i = i + 1
        Loop
        sb.AppendLine(String.Join(",", columnNames))

        'Data Rows
        Dim k As Int32 = 0
        Dim l As Int32 = 0
        For Each dr As DataRow In dt.Rows
            Dim columnValues(exportColumnNames.Count - 1) As String
            k = 0
            l = 0
            Do While k < columnCount
                If exportColumnNames.Contains(dt.Columns(k).ColumnName) Then
                    If IsDate(dr.Item(k)) Then
                        columnValues(l) = CType(dr.Item(k), Date).ToShortDateString
                    Else
                        columnValues(l) = dr.Item(k).ToString()
                    End If
                    l = l + 1
                End If
                k = k + 1
            Loop
            sb.AppendLine("""" & String.Join(""",""", columnValues) & """") ' escape commas contained in data fields so they aren't mistaken for CSV seperators
        Next
        Response.Write(sb.ToString())
        Response.Flush()
        Response.End()
    End Sub



    'Private Sub lblRecordCount_PreRender(sender As Object, e As EventArgs) Handles lblRecordCount.PreRender
    '    rgdTAP.AllowPaging = False
    '    rgdTAP.Rebind()
    '    CType(sender, Label).Text = rgdTAP.MasterTableView.Items.Count
    '    rgdTAP.AllowPaging = True
    '    rgdTAP.Rebind()
    'End Sub
End Class
