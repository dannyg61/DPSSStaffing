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

Partial Class _1546
    Inherits System.Web.UI.Page
    Private fileId As Integer
    Private fileData As Byte() = Nothing
    Private fileName As String
    Private description As String = ""
    Private CurrentPositionID As Int32
    Private CurrentStatusID As Int32
    Private CurrentLocationID As Int32
    Private CurrentProgramID As Int32
    Private ContactTypeIDs As ArrayList
    Private RecordCount As Int32
    Private EmailRecipients() As String
    Private EmailCC() As String
    Private EmailAttachments() As String
    Private EmailText As StringBuilder
    Private EmailSubject As String
    Dim SendRequestEmail As Boolean
    Dim SendApprovalEmail As Boolean
    'Private ContactPhoneNumbers As ArrayList

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ' Initiate ViewState holder of filtering values to empty array lists
            ViewState("VacantPositionFilterValue") = New ArrayList()
            ViewState("ProgramFilterValue") = New ArrayList()
            ViewState("RequestedProgramFilterValue") = New ArrayList()
            ViewState("OfficeFilterValue") = New ArrayList()
            ViewState("ShowPositionStatusFilterValue") = New ArrayList()
            ViewState("PositionNumberFilterValue") = New ArrayList()
            ViewState("JobIDFilterValue") = New ArrayList()
        End If
        Dim lblTitle As Label = Me.Master.FindControl("lblTitle")
        lblTitle.Text = "Self Sufficiency 1546 Requests"


        ' Apply the datasources to the radfilter custom field editorss
        Dim filterEditor As CustomEditors.RadCustomFilterDropDownEditor
        ' Position Title filter
        filterEditor = TryCast(rf1546.FieldEditors(2), CustomEditors.RadCustomFilterDropDownEditor)
        filterEditor.DataSource = Utilities.GetLookups("Vacant Position")
        ' Current Office filter
        filterEditor = TryCast(rf1546.FieldEditors(3), CustomEditors.RadCustomFilterDropDownEditor)
        filterEditor.DataSource = Utilities.GetLookups("Office Locations")
        ' Current Program filter
        filterEditor = TryCast(rf1546.FieldEditors(4), CustomEditors.RadCustomFilterDropDownEditor)
        filterEditor.DataSource = Utilities.GetLookups("Program")
        ' Show Filled filter
        filterEditor = TryCast(rf1546.FieldEditors(5), CustomEditors.RadCustomFilterDropDownEditor)
        filterEditor.DataSource = Utilities.GetLookups("Pos Status")

        ' This will add the default CC email address to the email when the rdpToHR date is changed.
        'If Request.Form("__EVENTTARGET") = "rdpToHR" Then
        '    SendApprovalEmail = True
        'End If
        ' Collect the file names of the attachments selected in the Email attachment window up closing of that window
        'If Request.Form("__EVENTTARGET") = "AttachmentsUploaded" Then
        '    Dim strEmailAttachments As String = Request.Form("__EVENTARGUMENT")
        '    ViewState("EmailAttachments") = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
        'End If

    End Sub

    Protected Sub rgd1546_InsertCommand(sender As Object, e As GridCommandEventArgs) Handles rgd1546.InsertCommand
        Try
            'For document upload logic make sure that we are dealing with the detail table based on the datasourceid
            If e.Item.OwnerTableView.DataSourceID = "obj1546Documents" Then
                Dim EmployeeNumber As String
                Dim LoginID As String
                Dim StartingIndex As Int32 = Me.User.Identity.Name.IndexOf("\") + 1
                LoginID = Me.User.Identity.Name.Substring(StartingIndex).ToUpper
                Dim clsEmployee As New Employees
                EmployeeNumber = clsEmployee.GetEmployeeNumberByLoginID(LoginID)
                Dim dal As New Req1546DAL.DAL1546
                dal.Insert1546Document(fileId, fileName, fileData, EmployeeNumber)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Protected Sub rgd1546_ItemCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles rgd1546.ItemCommand
        If e.Item Is Nothing Then
            ' Probably a paging commmand so ignore
            Exit Sub
        End If
        If e.Item.OwnerTableView.DataSourceID = "objDS1546" Then
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
                Case RadGrid.EditCommandName
                    ' Get current lookup column values from the grid and save them to use as input parameters when selecting the look up values
                    ' because if the value being saved belongs to a lookup entry that has been marked as inactive then we still want to include
                    ' it in the lookup table and the only way to do that is to have this id.
                    CurrentPositionID = CType(CType(e.Item, GridEditableItem)("tlkpPosition"), GridTableCell).Text
                    CurrentStatusID = CType(CType(e.Item, GridEditableItem)("tlkpStatus"), GridTableCell).Text
                    CurrentLocationID = CType(CType(e.Item, GridEditableItem)("tlkpLocation"), GridTableCell).Text
                    CurrentProgramID = CType(CType(e.Item, GridEditableItem)("tlkpProgram"), GridTableCell).Text
                    If CType(CType(e.Item, GridEditableItem)("DateToHR"), GridTableCell).Text = "&nbsp;" Then
                        ViewState("CurrentRequestApproved") = False
                    Else
                        ViewState("CurrentRequestApproved") = True
                    End If
                Case RadGrid.PerformInsertCommandName
                    If CType(CType(e.Item, GridEditFormInsertItem).FindControl("chkSendEmail"), CheckBox).Checked Then
                        If CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                            CType(CType(e.Item, GridEditFormInsertItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                            e.Canceled = True
                        Else
                            Dim rdpToExecs As RadDatePicker = CType(CType(e.Item, GridEditFormInsertItem).FindControl("rdpToExecs"), RadDatePicker)
                            If Not rdpToExecs.SelectedDate Is Nothing Then
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
            'For document upload logic make sure that we are dealing with the detail table based on the datasourceid
        ElseIf e.Item.OwnerTableView.DataSourceID = "obj1546Documents" Then
            If e.CommandName = RadGrid.UpdateCommandName OrElse e.CommandName = RadGrid.PerformInsertCommandName Then

                Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
                fileId = CType(e, GridCommandEventArgs).Item.OwnerTableView.ParentItem.GetDataKeyValue("ID")
                fileName = System.IO.Path.GetFileName((TryCast(item.EditManager.GetColumnEditor("FileName"), GridTextBoxColumnEditor)).Text)
                fileData = (TryCast(item.EditManager.GetColumnEditor("Data"), GridAttachmentColumnEditor)).UploadedFileContent

                If fileData.Length = 0 OrElse fileName.Trim() = String.Empty Then
                    e.Canceled = True
                    rgd1546.MasterTableView.DetailTables(0).Controls.Add(New LiteralControl("<b style='color:red;'>No file uploaded. Action canceled.</b>"))
                End If
            End If
            If (e.CommandName = RadGrid.DownloadAttachmentCommandName) Then
                Dim args As GridDownloadAttachmentCommandEventArgs = TryCast(e, GridDownloadAttachmentCommandEventArgs)
                Dim DocumentId As Integer = DirectCast(args.AttachmentKeyValues("ID"), Integer)
                Dim cls1546Requests As New Req1546DAL.DAL1546
                Dim filename As String = ""
                Dim binarydata As Byte()

                Dim dt As DataTable

                dt = cls1546Requests.Get1546DocumentObject(DocumentId)
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

    Protected Sub ConstructInitialRequestEmail(ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        EmailSubject = CType(CType(e.Item, GridEditFormItem).FindControl("txtSubject"), RadTextBox).Text
        EmailRecipients = Split(CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text, ";")
        EmailCC = Split(CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailCC"), RadTextBox).Text, ";")
        Dim strEmailAttachments As String = CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailAttachments"), RadTextBox).Text
        If strEmailAttachments.Length > 0 Then
            strEmailAttachments = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
        End If
        ViewState("EmailAttachments") = strEmailAttachments
        EmailAttachments = Split(ViewState("EmailAttachments").ToString, ";")
        EmailText = New StringBuilder()
        Dim strEmailContent As String
        strEmailContent = CType(CType(e.Item, GridEditFormItem).FindControl("MailEditor"), RadEditor).Content
        strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", CType(CType(e.Item, GridEditFormItem).FindControl("rcbPositionTitle"), RadComboBox).Text)
        EmailText.Append(strEmailContent)
    End Sub

    Protected Sub ConstructApprovalEmail(ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        Dim PositionNumber As String = String.Empty
        Select Case e.CommandName

            Case RadGrid.PerformInsertCommandName
                PositionNumber = CType(CType(e.Item, GridEditFormItem).FindControl("txtPositionNumber"), RadTextBox).Text
        End Select
        EmailSubject = CType(CType(e.Item, GridEditFormItem).FindControl("txtSubject"), RadTextBox).Text
        EmailRecipients = Split(CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailRecipients"), RadTextBox).Text, ";")
        EmailCC = Split(CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailCC"), RadTextBox).Text, ";")
        If Not CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailAttachments"), RadTextBox).Text = String.Empty Then
            Dim strEmailAttachments As String = CType(CType(e.Item, GridEditFormItem).FindControl("txtEmailAttachments"), RadTextBox).Text
            strEmailAttachments = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
            ViewState("EmailAttachments") = strEmailAttachments
            'Dim NumberOfAttachments = Split(ViewState("EmailAttachments").ToString, ";").Length - 1
            EmailAttachments = Split(ViewState("EmailAttachments"), ";")
        End If
        ' Get default email text from database
        EmailText = New StringBuilder()
        Dim strEmailContent As String
        strEmailContent = CType(CType(e.Item, GridEditFormItem).FindControl("MailEditor"), RadEditor).Content
        EmailText.Append(strEmailContent)
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

    Protected Sub objDSPositionStatus_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSPositionStatus.Selecting
        e.InputParameters("CurrentID") = CurrentStatusID
    End Sub

    Protected Sub objDSLocation_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSLocation.Selecting
        e.InputParameters("CurrentID") = CurrentLocationID
    End Sub

    Protected Sub objDSPrograms_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSPrograms.Selecting
        e.InputParameters("CurrentID") = CurrentProgramID
    End Sub

    Protected Sub rgd1546_ItemCreated(sender As Object, e As GridItemEventArgs) Handles rgd1546.ItemCreated
        Try
            If TypeOf e.Item Is GridEditableItem AndAlso e.Item.IsInEditMode Then
                'For document upload logic make sure that we are dealing with the detail table based on the datasourceid

                If e.Item.OwnerTableView.DataSourceID = "obj1546Documents" Then
                    'Assign client event to attachment column when going into edit mode - see JavaScript function uploadFileSelected
                    Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
                    Dim upload As RadUpload = (TryCast(item.EditManager.GetColumnEditor("Data"), GridAttachmentColumnEditor)).RadUploadControl
                    upload.OnClientFileSelected = "uploadFileSelected"
                End If
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Protected Sub rgd1546_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles rgd1546.ItemDataBound
        If TypeOf e.Item Is GridDataItem Then
            If e.Item.OwnerTableView.DataSourceID = "objDS1546" Then
                If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
                    CType(e.Item, GridDataItem)("EditCommandColumn").Controls(0).Visible = False
                End If
                If Not HttpContext.Current.User.IsInRole("SS Staff Delete") Then
                    CType(e.Item, GridDataItem)("Delete1546Button").Controls(0).Visible = False
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
                If e.Item.OwnerTableView.DataSourceID = "objDS1546" Then
                    ' Preselect the supervisor combo box values when in edit mode since we are using load on demand we can't use regular binding
                    ' because the control is empty at first
                    Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                    ' Load the initial tool tip for the rcbPositionTitle with the currently selected item. This will change in client side JavaScript
                    ' when another item is selected
                    CType(e.Item.FindControl("rcbPositionTitle"), RadComboBox).ToolTip = CType(e.Item.FindControl("rcbPositionTitle"), RadComboBox).SelectedItem.Text


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

                    If item("ProtectStatus").Text = True Then
                        Dim comboStatus As RadComboBox = DirectCast(item.FindControl("rcbStatus"), RadComboBox)
                        comboStatus.Enabled = False
                    End If
                End If
            Else
                If e.Item.OwnerTableView.DataSourceID = "objDS1546" Then
                    EmailOnInsert(e)

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
        Else
            CType(e.Item.FindControl("pnlEmail"), Panel).Enabled = True
            CType(e.Item.FindControl("MailEditor"), RadEditor).Enabled = True
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Visible = True
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Checked = False
            'Email crap
            Dim dtDefaultEmailRecipient As DataTable = Utilities.GetDefaultEmailAddresses("1546 Default Email CC")
            Dim clsEmployees As New Employees

            Dim dtEmployee As System.Data.DataTable
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
                If Not dtEmployee Is Nothing Then
                    dtEmployee.Clear()
                End If

                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(item("FirstLineSupervisorEmplID").Text))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            If item("SecondLineSupervisorName").Text <> "&nbsp;" Then
                If Not dtEmployee Is Nothing Then
                    dtEmployee.Clear()
                End If

                dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(item("SecondLineSupervisorEmplID").Text))
                If dtEmployee.Rows.Count > 0 Then
                    txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
            End If
            Dim txtEmail As Telerik.Web.UI.RadEditor = CType(e.Item.FindControl("MailEditor"), RadEditor)
            Dim srEmailContent As StreamReader
            srEmailContent = File.OpenText(Server.MapPath("~\App_Data\EmailTexts\") & "1546ApprovalEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()
            strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", item("PositionTitle").Text)
            strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionNumber", item("PositionNumber").Text)
            txtEmail.Content = strEmailContent
        End If
    End Sub

    Protected Sub rgd1546_ItemEvent(sender As Object, e As GridItemEventArgs) Handles rgd1546.ItemEvent
        If TypeOf e.EventInfo Is GridInitializePagerItem Then
            RecordCount = CType(e.EventInfo, GridInitializePagerItem).PagingManager.DataSourceCount
            lblRecordCount.Text = "Record Count: " & RecordCount
        End If
    End Sub

    Protected Sub rgd1546_ItemInserted(sender As Object, e As GridInsertedEventArgs) Handles rgd1546.ItemInserted
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

    Protected Sub rgd1546_PreRender(sender As Object, e As EventArgs) Handles rgd1546.PreRender

        If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
            For Each cmditm As GridCommandItem In rgd1546.MasterTableView.GetItems(GridItemType.CommandItem)
                Dim btnAdd As Button = CType(cmditm.FindControl("AddNewRecordButton"), Button)
                btnAdd.Visible = False
                Dim lnkAdd As LinkButton = CType(cmditm.FindControl("InitInsertButton"), LinkButton)
                lnkAdd.Visible = False
            Next
            rgd1546.MasterTableView.DetailTables(0).CommandItemSettings.ShowAddNewRecordButton = False
        End If
    End Sub

    Protected Sub objDS1546_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDS1546.Selecting
        e.InputParameters("VacantPosition") = ViewState("VacantPositionFilterValue")
        e.InputParameters("Program") = ViewState("ProgramFilterValue")
        e.InputParameters("Office") = ViewState("OfficeFilterValue")
        e.InputParameters("PositionStatus") = ViewState("ShowPositionStatusFilterValue")
        e.InputParameters("PositionNumber") = ViewState("PositionNumberFilterValue")
        e.InputParameters("JobID") = ViewState("JobIDFilterValue")
    End Sub

    Protected Sub rf1546_ApplyExpressions(sender As Object, e As RadFilterApplyExpressionsEventArgs) Handles rf1546.ApplyExpressions
        ' Applying the custom SQL queries for the RadFilter depending on what the user has selected
        Dim VacationPostionFilterValues As New ArrayList
        Dim ProgramFilterValues As New ArrayList
        Dim RequestedProgramFilterValues As New ArrayList
        Dim OfficeFilterValues As New ArrayList
        Dim ShowPositionStatusFilterValues As New ArrayList
        Dim PostionNumberFilterValues As New ArrayList
        Dim JobIDFilterValues As New ArrayList

        ViewState("VacantPositionFilterValue") = VacationPostionFilterValues
        ViewState("ProgramFilterValue") = ProgramFilterValues
        ViewState("RequestedProgramFilterValue") = RequestedProgramFilterValues
        ViewState("OfficeFilterValue") = OfficeFilterValues
        ViewState("ShowPositionStatusFilterValue") = ShowPositionStatusFilterValues
        ViewState("PositionNumberFilterValue") = PostionNumberFilterValues
        ViewState("JobIDFilterValue") = JobIDFilterValues

        Dim queryProvider As New RadFilterSqlQueryProvider()
        queryProvider.ProcessGroup(e.ExpressionRoot)
        Dim s As String = queryProvider.Result
        Dim FilterFieldNames As New ArrayList
        Dim FilterValues As New ArrayList

        For Each expression As RadFilterExpression In e.ExpressionRoot.Expressions
            Select Case CType(expression, RadFilterEqualToFilterExpression(Of String)).FieldName
                Case "Position Title"
                    VacationPostionFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Program"
                    ProgramFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Requested Program"
                    RequestedProgramFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Office"
                    OfficeFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Position Status"
                    ShowPositionStatusFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Position Number"
                    PostionNumberFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Job ID"
                    JobIDFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
            End Select
            'FilterFieldNames.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).FieldName)
            'FilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
        Next

        'For i As Integer = 0 To FilterFieldNames.Count - 1
        '    Select Case FilterFieldNames(i)
        '        Case "Position Title"
        '            ViewState("VacantPositionFilterValue") = FilterValues(i)
        '        Case "Program"
        '            ViewState("ProgramFilterValue") = FilterValues(i)
        '        Case "Requested Program"
        '            ViewState("RequestedProgramFilterValue") = FilterValues(i)
        '        Case "Office"
        '            ViewState("OfficeFilterValue") = FilterValues(i)
        '        Case "Position Status"
        '            ViewState("ShowPositionStatusFilterValue") = FilterValues(i)
        '    End Select
        'Next
        rgd1546.Rebind()
    End Sub

    Protected Sub rf1546_FieldEditorCreating(sender As Object, e As RadFilterFieldEditorCreatingEventArgs) Handles rf1546.FieldEditorCreating
        If e.EditorType = "RadCustomFilterDropDownEditor" Then
            e.Editor = New CustomEditors.RadCustomFilterDropDownEditor()
            'ElseIf e.EditorType = "RadFilterCheckBoxEditor" Then
            '    e.Editor = New RadFilterCheckBoxEditor
        End If
    End Sub

    Protected Sub rf1546_ItemCommand(sender As Object, e As RadFilterCommandEventArgs) Handles rf1546.ItemCommand
        ' This is a hack so that when the user removes an expression from the filter it will force the expression to actually be removed 
        ' and then reapply the filter
        If e.CommandName = RadFilter.RemoveExpressionCommandName Then
            Dim expr = e.ExpressionItem
            Dim grp = e.ExpressionItem.OwnerGroup
            grp.Expression.Expressions.RemoveAt(Array.IndexOf(grp.ChildItems.ToArray(), expr))
            rf1546.FireApplyCommand()
        End If
    End Sub

    Protected Sub valPositionNumber_ServerValidate(source As Object, args As ServerValidateEventArgs)
        If args.Value = String.Empty Then
            args.IsValid = True
            Exit Sub
        End If
        Dim InsertOrUpdate As String = String.Empty
        If TypeOf source.parent.parent.parent Is GridEditFormInsertItem Then
            InsertOrUpdate = "Insert"
        Else
            InsertOrUpdate = "Update"
        End If
        Dim Request1546ID As Int32
        If InsertOrUpdate = "Insert" Then
            Request1546ID = 0
        Else
            Request1546ID = CType(source.parent.parent.parent, GridEditFormItem).GetDataKeyValue("ID")
        End If
        'Dim PositionNumber As String = CType(CType(source.parent.parent.parent, GridEditFormItem).FindControl("txtPositionNumber"), RadTextBox).Text
        Dim PositionNumber As String = args.Value
        If Req1546DAL.DAL1546.CheckForDuplicate1546(Request1546ID, PositionNumber, InsertOrUpdate) Then
            args.IsValid = False
        Else
            args.IsValid = True
        End If
    End Sub

    Protected Sub objDS1546_Updated(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles objDS1546.Updated
        If Not SendApprovalEmail Then
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
        Try
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
                Dim txtEmail As Telerik.Web.UI.RadEditor = CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor)
                Dim srEmailContent As StreamReader
                srEmailContent = File.OpenText(Server.MapPath("~\App_Data\EmailTexts\") & "1546ApprovalEmail.html")
                Dim strEmailContent As String = srEmailContent.ReadToEnd()
                strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", CType(CType(sender.parent, WebControl).FindControl("rcbPositionTitle"), RadComboBox).Text)
                strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionNumber", CType(CType(sender.parent, WebControl).FindControl("txtPositionNumber"), RadTextBox).Text)
                txtEmail.Content = strEmailContent
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Protected Sub rcbPositionTitle_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        CType(sender, RadComboBox).ToolTip = e.Text
    End Sub

    Protected Sub rdpToExecs_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs)
        If Not CType(sender, RadDatePicker).SelectedDate Is Nothing Then
            CType(CType(sender.parent, WebControl).FindControl("pnlEmail"), Panel).Enabled = True
            CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor).Enabled = True
            CType(CType(sender.parent, WebControl).FindControl("chkSendEmail"), CheckBox).Visible = True
            Dim clsEmployees As New Employees

            Dim dtEmployee As New System.Data.DataTable
            Dim txtEmailCC As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailCC"), RadTextBox)
            txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"
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
            Dim txtEmail As Telerik.Web.UI.RadEditor = CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor)
            Dim srEmailContent As StreamReader
            srEmailContent = File.OpenText(Server.MapPath("~\App_Data\EmailTexts\") & "1546RequestCreatedEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()
            strEmailContent = Replace(strEmailContent, "&amp;&amp;PositionTitle", CType(CType(sender.parent, WebControl).FindControl("rcbPositionTitle"), RadComboBox).Text)
            txtEmail.Content = strEmailContent
        End If
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As ImageClickEventArgs)
        ' I KNOW I AM HARDCODING. THERE WAS NO WAY AROUND IT.
        Dim exportColumnNames As List(Of String) = New List(Of String)(New String() {"RequestDate", "Bilingual", "PositionNumber", "JobID", "PositionTitle", "PositionStatus", "StatusDate", "DateToExecs",
                                                                       "DateToHR", "Location", "Program", "FirstLineSupervisorName", "SecondLineSupervisorName", "ContactNumberFormatted", "Comments"})

        Dim exportColumnHeaders As List(Of String) = New List(Of String)(New String() {"Request Date", "Bilingual", "Position Number", "Job ID", "Position Title", "Position Status", "Last Update", "Date Sent to Execs",
                                                                       "Date Sent to HR", "Office", "Program", "1st Line Sup", "2nd Line Sup", "Contact Number", "Comments"})

        Dim dt As New DataTable
        Dim cls1546 As New Req1546DAL.DAL1546
        dt = cls1546.Get1546s(ViewState("VacantPositionFilterValue"), ViewState("ProgramFilterValue"), ViewState("OfficeFilterValue"), ViewState("ShowPositionStatusFilterValue"), ViewState("PositionNumberFilterValue"), ViewState("JobIDFilterValue"))

        Response.ClearContent()
        Response.Clear()
        Response.ContentType = "text/csv"
        Response.AddHeader("Content-Disposition", "attachment; filename=1546.csv;")

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

        'rgd1546.ExportSettings.Excel.Format = DirectCast([Enum].Parse(GetType(GridExcelExportFormat), "Xlsx"), GridExcelExportFormat)
        'rgd1546.ExportSettings.Excel.Format = GridExcelExportFormat.Xlsx
        'rgd1546.ExportSettings.IgnorePaging = True
        'rgd1546.ExportSettings.ExportOnlyData = True
        'rgd1546.ExportSettings.OpenInNewWindow = True
        'rgd1546.MasterTableView.ExportToExcel()
    End Sub

End Class


