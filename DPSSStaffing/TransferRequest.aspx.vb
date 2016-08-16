Imports Telerik.Web.UI
Imports System.Data
Imports CustomEditors
Imports CustomEditors2
Imports System.Linq
Imports StaffingUtilities
Imports StaffEmployees
Imports StaffTransferRequests
'Imports Aspose.Pdf
'Imports Aspose.Pdf.Forms

Partial Class TransferRequest
    Inherits System.Web.UI.Page

    Private SelectedOffices As ArrayList
    Private fileId As Integer
    Private fileData As Byte() = Nothing
    Private fileName As String
    Private description As String = ""
    Private CurrentRequestReasonID As Int32
    Private CurrentPositionID As Int32
    Private CurrentProgramID As Int32
    Private RequestedProgramID As Int32
    Private CurrentOfficeID As Int32
    Private CurrentTransferLocationID As Int32
    Private CurrentCancelReasonID As Int32
    Private RecordCount As Int32
    Private EmailRecipients() As String
    Private EmailCC() As String
    Private EmailAttachments() As String
    Private EmailText As StringBuilder
    Private EmailSubject As String
    Dim SendApprovalEmail As Boolean

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                ' Initiate ViewState holders of filtering values to empty array lists
                ViewState("RequestedOfficeFilterValue") = New ArrayList()
                ViewState("CurrentPositionFilterValue") = New ArrayList()
                ViewState("CurrentProgramFilterValue") = New ArrayList()
                ViewState("RequestedProgramFilterValue") = New ArrayList()
                ViewState("CurrentOfficeFilterValue") = New ArrayList()
                ViewState("RequestedOfficeFilterValue") = New ArrayList()
                ViewState("ShowCanceledFilterValue") = False
            End If

            'Dim license As Aspose.Pdf.License = New Aspose.Pdf.License
            'license.SetLicense("Aspose.Total.lic")


            Dim lblTitle As Label = Me.Master.FindControl("lblTitle")
            lblTitle.Text = "Self Sufficiency Transfer Requests"


            ' Apply the datasources to the radfilter custom field editorss
            Dim filterEditor As CustomEditors.RadCustomFilterDropDownEditor
            ' Position Title filter
            filterEditor = TryCast(rfTransfers.FieldEditors(0), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Current Position")
            ' Current Program filter
            filterEditor = TryCast(rfTransfers.FieldEditors(1), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Program")
            ' Requested Program filter
            filterEditor = TryCast(rfTransfers.FieldEditors(2), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Program")
            ' Current Office filter
            filterEditor = TryCast(rfTransfers.FieldEditors(3), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Office Locations")
            ' Requested Office filter
            filterEditor = TryCast(rfTransfers.FieldEditors(4), CustomEditors.RadCustomFilterDropDownEditor)
            filterEditor.DataSource = Utilities.GetLookups("Office Locations")
            ' Show Canceled filter
            Dim filterCheckBoxEditor As MyRadFilterCheckBoxEditor
            filterCheckBoxEditor = TryCast(rfTransfers.FieldEditors(5), MyRadFilterCheckBoxEditor)

        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Protected Sub rgdTransfers_ItemEvent(sender As Object, e As GridItemEventArgs) Handles rgdTransfers.ItemEvent
        If TypeOf e.EventInfo Is GridInitializePagerItem Then
            RecordCount = CType(e.EventInfo, GridInitializePagerItem).PagingManager.DataSourceCount
            lblRecordCount.Text = "Record Count: " & RecordCount
        End If
    End Sub

    Protected Sub rgdTransfers_PreRender(sender As Object, e As EventArgs) Handles rgdTransfers.PreRender
        If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
            For Each cmditm As GridCommandItem In rgdTransfers.MasterTableView.GetItems(GridItemType.CommandItem)
                Dim btnAdd As Button = CType(cmditm.FindControl("AddNewRecordButton"), Button)
                btnAdd.Visible = False
                Dim lnkAdd As LinkButton = CType(cmditm.FindControl("InitInsertButton"), LinkButton)
                lnkAdd.Visible = False
            Next
            rgdTransfers.MasterTableView.DetailTables(0).CommandItemSettings.ShowAddNewRecordButton = False
        End If
        ' Remove Canceled column from grid if the show canceled filter is not being applied
        Dim currentFilterExpressionCount As Int32 = CType(CType(rfTransfers.Controls(0), RadFilterExpressionContainer).Controls(0), RadFilterGroupExpressionItem).Expression.Expressions.Count
        If currentFilterExpressionCount = 0 Then
            For Each column As GridColumn In rgdTransfers.Columns
                If column.UniqueName = "Cancelled" Then
                    CType(column, GridTemplateColumn).Visible = False
                End If
            Next
        Else
            ViewState("ShowCanceled") = False
            'Dim ShowCanceled As Boolean = False
            For i As Int32 = 0 To currentFilterExpressionCount - 1
                If CType(CType(CType(rfTransfers.Controls(0), RadFilterExpressionContainer).Controls(0), RadFilterGroupExpressionItem).Expression.Expressions(i), RadFilterEqualToFilterExpression(Of String)).FieldName = "Show Canceled" Then
                    If Not CType(CType(CType(rfTransfers.Controls(0), RadFilterExpressionContainer).Controls(0), RadFilterGroupExpressionItem).Expression.Expressions(i), RadFilterEqualToFilterExpression(Of String)).Value Is Nothing Then
                        If CType(CType(CType(rfTransfers.Controls(0), RadFilterExpressionContainer).Controls(0), RadFilterGroupExpressionItem).Expression.Expressions(i), RadFilterEqualToFilterExpression(Of String)).Value.ToString = "True" Then
                            ViewState("ShowCanceled") = True
                        End If
                    End If
                End If
            Next
            If ViewState("ShowCanceled") = False Then
                For Each column As GridColumn In rgdTransfers.Columns
                    If column.UniqueName = "Cancelled" Then
                        CType(column, GridTemplateColumn).Visible = False
                    End If
                Next
            Else
                For Each column As GridColumn In rgdTransfers.Columns
                    If column.UniqueName = "Cancelled" Then
                        CType(column, GridTemplateColumn).Visible = True
                    End If
                Next
            End If
        End If
    End Sub

    Protected Sub rgdTransfers_InsertCommand(sender As Object, e As GridCommandEventArgs) Handles rgdTransfers.InsertCommand
        Try
            'For document upload logic make sure that we are dealing with the detail table based on the datasourceid
            If e.Item.OwnerTableView.DataSourceID = "objTransferDocuments" Then
                Dim EmployeeNumber As String
                Dim LoginID As String
                Dim StartingIndex As Int32 = Me.User.Identity.Name.IndexOf("\") + 1
                LoginID = Me.User.Identity.Name.Substring(StartingIndex).ToUpper
                Dim clsEmployee As New Employees
                EmployeeNumber = clsEmployee.GetEmployeeNumberByLoginID(LoginID)
                Dim dal As New TransferRequests
                dal.InsertTransferDocument(fileId, fileName, fileData, EmployeeNumber)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Protected Sub rgdTransfers_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles rgdTransfers.ItemCommand
        If e.Item Is Nothing Then
            ' Probably a paging commmand so ignore
            Exit Sub
        End If
        If e.Item.OwnerTableView.DataSourceID = "objDSTransfers" Then
            Select Case e.CommandName
                Case RadGrid.InitInsertCommandName
                    ' this prevents null exception for Canceled checkbox on inserts
                    e.Canceled = True
                    Dim newValues As New System.Collections.Specialized.ListDictionary()
                    newValues("Cancelled") = False
                    e.Item.OwnerTableView.InsertItem(newValues)
                Case RadGrid.PerformInsertCommandName
                    ' Grab values from unbound listbox control  for inserting
                    SelectedOffices = New ArrayList
                    Dim rlbOffices As RadListBox = CType(CType(e.Item, GridEditFormInsertItem).FindControl("rlbSelectedOffices"), RadListBox)
                    ' Validation of the unbound listbox controls in the edit form must be done here
                    If rlbOffices.Items.Count = 0 Then
                        CType(CType(e.Item, GridEditFormInsertItem).FindControl("valSelectedOffices"), CustomValidator).IsValid = False
                        e.Canceled = True
                        Exit Sub
                    End If
                    For Each rlbItem As RadListBoxItem In rlbOffices.Items
                        SelectedOffices.Add(rlbItem.Value)
                    Next
                    If CType(CType(e.Item, GridEditFormInsertItem).FindControl("chkSendEmail"), CheckBox).Checked Then
                        If CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailRecipients"), RadTextBox).Text = String.Empty Then
                            CType(CType(e.Item, GridEditFormInsertItem).FindControl("lblError"), Label).Text = "At least one email recipient must be entered."
                            e.Canceled = True
                        Else
                            Dim rdpRecdFromHR As RadDatePicker = CType(CType(e.Item, GridEditFormInsertItem).FindControl("rdpRecdFromHR"), RadDatePicker)
                            If Not rdpRecdFromHR.SelectedDate Is Nothing Then
                                ConstructApprovalEmail(e)
                                SendApprovalEmail = True
                            Else
                                SendApprovalEmail = False
                            End If
                        End If
                    Else
                        SendApprovalEmail = False
                    End If
                Case RadGrid.UpdateCommandName
                    ' Grab values from unbound listbox control for updating
                    SelectedOffices = New ArrayList
                    Dim rlbOffices As RadListBox = CType(CType(e.Item, GridEditFormItem).FindControl("rlbSelectedOffices"), RadListBox)
                    If rlbOffices.Items.Count = 0 Then
                        CType(CType(e.Item, GridEditFormItem).FindControl("valSelectedOffices"), CustomValidator).IsValid = False
                        e.Canceled = True
                        Exit Sub
                    End If
                    For Each rlbItem As RadListBoxItem In rlbOffices.Items
                        SelectedOffices.Add(rlbItem.Value)
                    Next
                    Dim rdpRecdFromHR As RadDatePicker = CType(CType(e.Item, GridEditFormItem).FindControl("rdpRecdFromHR"), RadDatePicker)
                    If Not ViewState("CurrentRequestApproved") And Not rdpRecdFromHR.SelectedDate Is Nothing Then
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
                        If Not rdpRecdFromHR.SelectedDate Is Nothing And CType(CType(e.Item, GridEditFormItem).FindControl("chkSendEmail"), CheckBox).Checked Then
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
                    CurrentRequestReasonID = CType(CType(e.Item, GridEditableItem)("tlkpRequestReason"), GridTableCell).Text
                    CurrentPositionID = CType(CType(e.Item, GridEditableItem)("tlkpPosition"), GridTableCell).Text
                    CurrentProgramID = CType(CType(e.Item, GridEditableItem)("tlkpProgram"), GridTableCell).Text
                    RequestedProgramID = CType(CType(e.Item, GridEditableItem)("tlkpReqProgram"), GridTableCell).Text
                    CurrentOfficeID = CType(CType(e.Item, GridEditableItem)("tlkpOffice"), GridTableCell).Text
                    CurrentTransferLocationID = CType(CType(e.Item, GridEditableItem)("tlkpTransLocation"), GridTableCell).Text
                    CurrentCancelReasonID = CType(CType(e.Item, GridEditableItem)("tlkpCancelReason"), GridTableCell).Text
                    If CType(CType(e.Item, GridEditableItem)("FromHRDT"), GridTableCell).Text = "&nbsp;" Then
                        ViewState("CurrentRequestApproved") = False
                    Else
                        ViewState("CurrentRequestApproved") = True
                    End If
            End Select
            'For document upload logic make sure that we are dealing with the detail table based on the datasourceid
        ElseIf e.Item.OwnerTableView.DataSourceID = "objTransferDocuments" Then
            If e.CommandName = RadGrid.UpdateCommandName OrElse e.CommandName = RadGrid.PerformInsertCommandName Then

                Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
                fileId = CType(e, GridCommandEventArgs).Item.OwnerTableView.ParentItem.GetDataKeyValue("ID")
                fileName = System.IO.Path.GetFileName((TryCast(item.EditManager.GetColumnEditor("FileName"), GridTextBoxColumnEditor)).Text)
                fileData = (TryCast(item.EditManager.GetColumnEditor("Data"), GridAttachmentColumnEditor)).UploadedFileContent

                If fileData.Length = 0 OrElse fileName.Trim() = String.Empty Then
                    e.Canceled = True
                    rgdTransfers.MasterTableView.DetailTables(0).Controls.Add(New LiteralControl("<b style='color:red;'>No file uploaded. Action canceled.</b>"))
                End If
            End If

            If (e.CommandName = RadGrid.DownloadAttachmentCommandName) Then
                Dim args As GridDownloadAttachmentCommandEventArgs = TryCast(e, GridDownloadAttachmentCommandEventArgs)
                Dim DocumentId As Integer = DirectCast(args.AttachmentKeyValues("ID"), Integer)
                Dim clsTransferRequests As New TransferRequests
                Dim filename As String = ""
                Dim binarydata As Byte()

                Dim dt As DataTable

                dt = clsTransferRequests.GetTransferDocumentObject(DocumentId)
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

    Protected Sub rgdTransfers_ItemCreated(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles rgdTransfers.ItemCreated
        Try
            If TypeOf e.Item Is GridFilteringItem Then
                Dim item As GridFilteringItem = e.Item
                Dim txt As TextBox = CType(item("EmplID").Controls(0), TextBox)
                txt.Attributes.Add("onkeypress", "return isNumericKey(event);")
            End If
            If TypeOf e.Item Is GridEditableItem AndAlso e.Item.IsInEditMode Then
                'For document upload logic make sure that we are dealing with the detail table based on the datasourceid
                If e.Item.OwnerTableView.DataSourceID = "objTransferDocuments" Then
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

    Protected Sub rgdTransfers_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles rgdTransfers.ItemDataBound

        If TypeOf e.Item Is GridDataItem Then
            If e.Item.OwnerTableView.DataSourceID = "objDSTransfers" Then
                If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
                    CType(e.Item, GridDataItem)("EditCommandColumn").Controls(0).Visible = False
                End If
                If Not HttpContext.Current.User.IsInRole("SS Staff Delete") Then
                    CType(e.Item, GridDataItem)("DeleteTransferButton").Controls(0).Visible = False
                End If
                Dim TransferRequestID As Int32 = CType(e.Item, GridDataItem).GetDataKeyValue("ID")
                Dim lbPrograms As RadListBox = e.Item.FindControl("lbRequestedOfficesDisplay")
                Dim TR As New TransferRequests
                lbPrograms.DataSource = TR.GetTransferRequestLocations(TransferRequestID)
                lbPrograms.DataTextField = "RequestedOffice"
                lbPrograms.DataValueField = "id"
                lbPrograms.DataBind()
            Else
                ' disable delete document button in detail table for non admin
                If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
                    CType(e.Item, GridDataItem)("DeleteColumn").Controls(0).Visible = False
                End If
            End If
        End If
        If TypeOf e.Item Is GridEditFormItem And e.Item.IsInEditMode Then
            'Populate the List Boxes that contain the offices available for a request and the offices already assigned to this request
            If e.Item.OwnerTableView.DataSourceID = "objDSTransfers" Then
                Dim clsTransfers As New TransferRequests
                Dim lbAvailableOffices As RadListBox = e.Item.FindControl("rlbAvailableOffices")
                Dim lbAssignedLocations As RadListBox = e.Item.FindControl("rlbSelectedOffices")
                If TypeOf (e.Item) Is GridEditFormInsertItem Then
                    'Insert makes all offices available and none assigned
                    EmailOnInsert(e)
                    lbAvailableOffices.DataSource = clsTransfers.GetTransferAvailableLocations(0)
                    lbAvailableOffices.DataTextField = "DataValue"
                    lbAvailableOffices.DataValueField = "ID"
                    lbAvailableOffices.DataBind()
                Else
                    'Update - Available offices list box will not have offices that are already in the assigned list box
                    EmailOnEdit(e)
                    Dim TransferRequestID As Int32 = CType(e.Item, GridEditFormItem).GetDataKeyValue("ID")
                    lbAssignedLocations.DataSource = clsTransfers.GetTransferRequestLocations(TransferRequestID)
                    lbAssignedLocations.DataTextField = "RequestedOffice"
                    lbAssignedLocations.DataValueField = "tlkpOfficeLocations"
                    lbAssignedLocations.DataBind()
                    lbAvailableOffices.DataSource = clsTransfers.GetTransferAvailableLocations(TransferRequestID)
                    lbAvailableOffices.DataTextField = "DataValue"
                    lbAvailableOffices.DataValueField = "ID"
                    lbAvailableOffices.DataBind()
                    ' Preselect the supervisor combo box values when in edit mode since we are using load on demand we can't use regular binding
                    ' because the control is empty at first
                    Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
                    If item("FirstLineSupervisorName").Text <> "&nbsp;" Then
                        Dim comboFirstLineSupervisor As RadComboBox = DirectCast(item.FindControl("rcbFirstLineSupervisor"), RadComboBox)
                        Dim preselectedFirstItem As New RadComboBoxItem()
                        preselectedFirstItem.Text = Server.HtmlDecode(item("FirstLineSupervisorName").Text)
                        preselectedFirstItem.Value = Server.HtmlDecode(item("FirstLineSupervisorEmplID").Text)
                        comboFirstLineSupervisor.Items.Insert(0, preselectedFirstItem)
                        comboFirstLineSupervisor.SelectedIndex = 0
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
                    ' Make the canceled data items protected if the cancel reason has been set to 'Expired' or if the Transfer Date has a value
                    Dim rdpTransferDate As RadDatePicker = DirectCast(item.FindControl("rdpTransferDate"), RadDatePicker)
                    Dim chkCanceled As CheckBox = DirectCast(item.FindControl("chkCanceled"), CheckBox)
                    Dim rcbCancelReason As RadComboBox = DirectCast(item.FindControl("rcbCancelReason"), RadComboBox)
                    Dim cancelItem As RadComboBoxItem = rcbCancelReason.FindItemByValue(rcbCancelReason.SelectedValue)
                    If (Not rdpTransferDate.SelectedDate Is Nothing) Or cancelItem.Text = "Expired" Then
                        chkCanceled.Enabled = False
                        rcbCancelReason.Enabled = False
                    End If
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
        Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

        If CType(e.Item.FindControl("rdpRecdFromHR"), RadDatePicker).SelectedDate Is Nothing Then
            CType(e.Item.FindControl("pnlEmail"), Panel).Enabled = False
            CType(e.Item.FindControl("MailEditor"), RadEditor).Enabled = False
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Visible = False
            Dim clsEmployees As New Employees
            Dim dtEmployee As System.Data.DataTable

            Dim txtEmailRecipients As RadTextBox = CType(e.Item.FindControl("txtEmailRecipients"), RadTextBox)
            dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(item("EmplID").Text))
            If dtEmployee.Rows.Count > 0 Then
                txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
            End If
        Else
            CType(e.Item.FindControl("pnlEmail"), Panel).Enabled = True
            CType(e.Item.FindControl("MailEditor"), RadEditor).Enabled = True
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Visible = True
            CType(e.Item.FindControl("chkSendEmail"), CheckBox).Checked = False

            Dim clsEmployees As New Employees
            Dim dtEmployee As System.Data.DataTable
            Dim txtEmailRecipients As RadTextBox = CType(e.Item.FindControl("txtEmailRecipients"), RadTextBox)

            Dim txtEmailCC As RadTextBox = CType(e.Item.FindControl("txtEmailCC"), RadTextBox)
            txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"

            dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(item("EmplID").Text))
            If dtEmployee.Rows.Count > 0 Then
                txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
            End If
            If item("FirstLineSupervisorName").Text <> "&nbsp;" Then
                If Not dtEmployee Is Nothing Then
                    dtEmployee.Clear()
                End If

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
            Dim srEmailContent As System.IO.StreamReader
            srEmailContent = System.IO.File.OpenText(Server.MapPath("~\App_Data\EmailTexts\") & "TransferApprovalEmail.html")
            Dim strEmailContent As String = srEmailContent.ReadToEnd()
            Dim ExpirationDate As Date = CType(e.Item.FindControl("rdpExpirationDate"), RadDatePicker).SelectedDate.Value
            strEmailContent = Replace(strEmailContent, "&amp;&amp;ExpirationDate", ExpirationDate.ToString("MM/dd/yyyy"))
            txtEmail.Content = strEmailContent
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
    End Sub

    Protected Sub rlbAvailableOffices_Transferred(sender As Object, e As RadListBoxTransferredEventArgs)
        ' When an item is transferred back into the original listbox then resort
        Dim rlbAvailOffices As RadListBox = CType(sender, RadListBox)
        If CType(e, RadListBoxTransferredEventArgs).DestinationListBox.ID = "rlbAvailableOffices" Then
            rlbAvailOffices.Sort = Telerik.Web.UI.RadListBoxSort.Ascending
            rlbAvailOffices.SortItems()
        End If
    End Sub

    Protected Sub rcbEmployees_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim clsEmployees As New Employees
        Dim dtEmployee As System.Data.DataTable
        dtEmployee = clsEmployees.GetEmployeeInformation(CType(sender, RadComboBox).SelectedValue)
        If dtEmployee.Rows.Count = 0 Then
            ' clear out employee data if not found
            Dim txtEmployeeNumber As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtEmployeeNumber")
            txtEmployeeNumber.Text = String.Empty
            'This next one is the one actually bound to the update data item
            Dim txtEmployeeID As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtEmplID")
            txtEmployeeID.Text = String.Empty
            Dim txtPosition As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtOasisPosition")
            txtPosition.Text = String.Empty
            Dim txtSuper As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtOasisSupervisor")
            txtSuper.Text = String.Empty
            Exit Sub
        End If
        Select Case CType(sender, RadComboBox).ID
            Case "rcbEmployees"
                'This next one is for display only when inserting.
                Dim txtEmployeeNumber As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtEmployeeNumber")
                txtEmployeeNumber.Text = dtEmployee.Rows(0).Item("EmplID")
                'This next one is the one actually bound to the update data item
                Dim txtEmployeeID As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtEmplID")
                txtEmployeeID.Text = dtEmployee.Rows(0).Item("EmplID")
                Dim txtPosition As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtOasisPosition")
                txtPosition.Text = dtEmployee.Rows(0).Item("DESCR")
                Dim txtSuper As RadTextBox = CType(sender, RadComboBox).Parent.FindControl("txtOasisSupervisor")
                If dtEmployee.Rows(0).Item("OasisFirstLineSupvr") Is DBNull.Value Then
                    txtSuper.Text = String.Empty
                Else
                    txtSuper.Text = dtEmployee.Rows(0).Item("OasisFirstLineSupvr")
                End If
                If TypeOf sender.parent.parent.parent.parent Is GridEditFormInsertItem Then
                    Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormInsertItem).FindControl("txtEmailRecipients")
                    txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                Else
                    Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("txtEmailRecipients")
                    txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                End If
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
                        If Not CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("rdpDateSentToHR"), RadDatePicker).SelectedDate Is Nothing Then
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
                        If Not CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("rdpDateSentToHR"), RadDatePicker).SelectedDate Is Nothing Then
                            Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent, GridEditFormItem).FindControl("txtEmailRecipients")
                            txtEmailRecipients.Text = txtEmailRecipients.Text + dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                        End If
                    End If
                End If
        End Select

    End Sub

    Protected Sub objDSTransfers_Inserting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDSTransfers.Inserting
        e.InputParameters("TransferRequestLocations") = SelectedOffices
    End Sub

    Protected Sub objDSTransfers_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSTransfers.Selecting
        e.InputParameters("CurrentPosition") = ViewState("CurrentPositionFilterValue")
        e.InputParameters("CurrentProgram") = ViewState("CurrentProgramFilterValue")
        e.InputParameters("RequestedProgram") = ViewState("RequestedProgramFilterValue")
        e.InputParameters("CurrentOffice") = ViewState("CurrentOfficeFilterValue")
        e.InputParameters("RequestedOffice") = ViewState("RequestedOfficeFilterValue")
        e.InputParameters("ShowCanceled") = ViewState("ShowCanceledFilterValue")
    End Sub

    Protected Sub objDSTransfers_Updating(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDSTransfers.Updating
        e.InputParameters("TransferRequestLocations") = SelectedOffices
    End Sub

    Protected Sub rfTransfers_ApplyExpressions(sender As Object, e As RadFilterApplyExpressionsEventArgs) Handles rfTransfers.ApplyExpressions
        ' Applying the custom SQL queries for the RadFilter depending on what the user has selected
        Dim CurrentPositionFilterValues As New ArrayList
        Dim CurrentProgramFilterValues As New ArrayList
        Dim RequestedProgramFilterValues As New ArrayList
        Dim CurrentOfficeFilterValues As New ArrayList
        Dim RequestedOfficeFilterValues As New ArrayList

        ViewState("CurrentPositionFilterValue") = CurrentPositionFilterValues
        ViewState("CurrentProgramFilterValue") = CurrentProgramFilterValues
        ViewState("RequestedProgramFilterValue") = RequestedProgramFilterValues
        ViewState("CurrentOfficeFilterValue") = CurrentOfficeFilterValues
        ViewState("RequestedOfficeFilterValue") = RequestedOfficeFilterValues
        ViewState("ShowCanceledFilterValue") = False

        Dim queryProvider As New RadFilterSqlQueryProvider()
        queryProvider.ProcessGroup(e.ExpressionRoot)
        Dim s As String = queryProvider.Result
        Dim FilterFieldNames As New ArrayList
        Dim FilterValues As New ArrayList


        For Each expression As RadFilterExpression In e.ExpressionRoot.Expressions
            Select Case CType(expression, RadFilterEqualToFilterExpression(Of String)).FieldName
                Case "Current Position"
                    CurrentPositionFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Current Program"
                    CurrentProgramFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Requested Program"
                    RequestedProgramFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Current Office"
                    CurrentOfficeFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Requested Office"
                    RequestedOfficeFilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
                Case "Show Canceled"
                    ' No multiple values for show canceled so no need for an array...only one value to be held in ViewState...either true or false
                    ViewState("ShowCanceledFilterValue") = CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value
            End Select
            'FilterFieldNames.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).FieldName)
            'FilterValues.Add(CType(expression, Telerik.Web.UI.RadFilterEqualToFilterExpression(Of String)).Value)
        Next
        ViewState("CurrentPositionFilterValue") = CurrentPositionFilterValues
        ViewState("CurrentProgramFilterValue") = CurrentProgramFilterValues
        ViewState("RequestedProgramFilterValue") = RequestedProgramFilterValues
        ViewState("CurrentOfficeFilterValue") = CurrentOfficeFilterValues
        ViewState("RequestedOfficeFilterValue") = RequestedOfficeFilterValues
        'For i As Integer = 0 To FilterFieldNames.Count - 1
        '    Select Case FilterFieldNames(i)
        '        Case "Current Position"
        '            ViewState("CurrentPositionFilterValue") = FilterValues(i)
        '        Case "Current Program"
        '            ViewState("CurrentProgramFilterValue") = FilterValues(i)
        '        Case "Requested Program"
        '            ViewState("RequestedProgramFilterValue") = FilterValues(i)
        '        Case "Current Office"
        '            ViewState("CurrentOfficeFilterValue") = FilterValues(i)
        '        Case "Requested Office"
        '            ViewState("RequestedOfficeFilterValue") = FilterValues(i)
        '        Case "Show Canceled"
        '            ViewState("ShowCanceledFilterValue") = FilterValues(i)
        '    End Select
        'Next
        rgdTransfers.Rebind()
    End Sub

    Protected Sub rfTransfers_FieldEditorCreating(ByVal sender As Object, ByVal e As RadFilterFieldEditorCreatingEventArgs)
        Try
            If e.EditorType = "RadCustomFilterDropDownEditor" Then
                e.Editor = New CustomEditors.RadCustomFilterDropDownEditor()
            ElseIf e.EditorType = "MyRadFilterCheckBoxEditor" Then
                e.Editor = New MyRadFilterCheckBoxEditor
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Protected Sub rfTransfers_ItemCommand(sender As Object, e As RadFilterCommandEventArgs) Handles rfTransfers.ItemCommand
        ' This is a hack so that when the user removes an expression from the filter it will force the expression to actually be removed 
        ' and then reapply the filter
        If e.CommandName = RadFilter.RemoveExpressionCommandName Then
            Dim expr = e.ExpressionItem
            Dim grp = e.ExpressionItem.OwnerGroup
            grp.Expression.Expressions.RemoveAt(Array.IndexOf(grp.ChildItems.ToArray(), expr))
            rfTransfers.FireApplyCommand()
        End If
    End Sub

    Protected Sub objDSXferReasons_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSXferReasons.Selecting
        e.InputParameters("CurrentID") = CurrentRequestReasonID
    End Sub

    Protected Sub objDSCurrentPositions_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSCurrentPositions.Selecting
        e.InputParameters("CurrentID") = CurrentPositionID
    End Sub

    Protected Sub objDSPrograms_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSPrograms.Selecting
        e.InputParameters("CurrentID") = CurrentProgramID
    End Sub

    Protected Sub objDSRequestedPrograms_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSRequestedPrograms.Selecting
        e.InputParameters("CurrentID") = RequestedProgramID
    End Sub

    Protected Sub objDSOffices_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSOffices.Selecting
        e.InputParameters("CurrentID") = CurrentOfficeID
    End Sub

    Protected Sub objDSXferLocations_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSXferLocations.Selecting
        e.InputParameters("CurrentID") = CurrentTransferLocationID
    End Sub

    Protected Sub objDSCancelReasons_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSCancelReasons.Selecting
        e.InputParameters("CurrentID") = CurrentCancelReasonID
    End Sub

    Protected Sub CheckForDuplicateTransfer(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim InsertOrUpdate As String = String.Empty
        If TypeOf sender.parent.parent.parent Is GridEditFormInsertItem Then
            InsertOrUpdate = "Insert"
        Else
            InsertOrUpdate = "Update"
        End If
        Dim EmplID As String = CType(CType(sender.parent.parent.parent, GridEditFormItem).FindControl("txtEmplID"), RadTextBox).Text
        Dim TransferID As Int32
        If InsertOrUpdate = "Insert" Then
            TransferID = 0
        Else
            TransferID = CType(sender.parent.parent.parent, GridEditFormItem).GetDataKeyValue("ID")
        End If

        Dim RequestDate As Date = e.Value
        If Not IsDate(e.Value) Then
            e.IsValid = False
            Exit Sub
        End If
        If TransferRequests.CheckForDuplicateTransfer(TransferID, EmplID, RequestDate, InsertOrUpdate) Then
            e.IsValid = False
        Else
            e.IsValid = True
        End If
    End Sub

#Region "EmailCode"
    Protected Sub ConstructApprovalEmail(ByVal e As Telerik.Web.UI.GridCommandEventArgs)
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
        EmailText.Append(strEmailContent)
    End Sub

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

    Protected Sub btnAddEmailCC_Click(sender As Object, e As EventArgs)
        If ViewState("EmailCC") Is Nothing Then
            Exit Sub
        End If
        Dim txtEmailCC As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("txtEmailCC")
        txtEmailCC.Text = txtEmailCC.Text + ViewState("EmailCC").ToString.ToLower & "@riversidedpss.org;"
    End Sub


#End Region

    Protected Sub rdpRecdFromHR_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs)
        Try
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
                Dim srEmailContent As System.IO.StreamReader
                srEmailContent = System.IO.File.OpenText(Server.MapPath("~\App_Data\EmailTexts\") & "TransferApprovalEmail.html")
                Dim strEmailContent As String = srEmailContent.ReadToEnd()
                Dim ExpirationDate As Date = CType(CType(sender.parent, WebControl).FindControl("rdpExpirationDate"), RadDatePicker).SelectedDate.Value
                strEmailContent = Replace(strEmailContent, "&amp;&amp;ExpirationDate", ExpirationDate.ToString("MM/dd/yyyy"))
                txtEmail.Content = strEmailContent
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Protected Sub rgdTransfers_ItemInserted(sender As Object, e As GridInsertedEventArgs) Handles rgdTransfers.ItemInserted
        If Not SendApprovalEmail Then
            If Not CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailAttachments"), RadTextBox).Text = String.Empty Then
                Dim strEmailAttachments As String = CType(CType(e.Item, GridEditFormInsertItem).FindControl("txtEmailAttachments"), RadTextBox).Text
                strEmailAttachments = strEmailAttachments.Substring(0, strEmailAttachments.Length - 1)
                ViewState("EmailAttachments") = strEmailAttachments
                'Dim NumberOfAttachments = Split(ViewState("EmailAttachments").ToString, ";").Length - 1
                EmailAttachments = Split(ViewState("EmailAttachments"), ";")
                For i = 0 To EmailAttachments.Count - 1
                    If System.IO.File.Exists(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString) Then
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

    Protected Sub rgdTransfers_ItemUpdated(sender As Object, e As GridUpdatedEventArgs) Handles rgdTransfers.ItemUpdated
        If Not SendApprovalEmail Then
            If Not ViewState("EmailAttachments") Is Nothing Then
                'Dim NumberOfAttachments = Split(ViewState("EmailAttachments").ToString, ";").Length - 1
                EmailAttachments = Split(ViewState("EmailAttachments").ToString, ";")
                For i = 0 To EmailAttachments.Count - 1
                    If System.IO.File.Exists(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString) Then
                        System.IO.File.Delete(HttpRuntime.AppDomainAppPath & "App_Data\EmailAttachments\" & EmailAttachments(i).ToString)
                    End If
                Next
            End If
            Exit Sub
        End If

        If EmailAttachments Is Nothing Then
            ReDim EmailAttachments(0)
            EmailAttachments(0) = "None"
        Else
            If EmailAttachments(0) = String.Empty Then
                ReDim EmailAttachments(0)
                EmailAttachments(0) = "None"
            End If
        End If
        Utilities.SendEmail(EmailRecipients, EmailCC, EmailSubject, EmailText.ToString, EmailAttachments)
        ViewState("EmailAttachments") = Nothing
    End Sub


    Protected Sub btnExport_Click(sender As Object, e As ImageClickEventArgs)
        ' I KNOW I AM HARDCODING. THERE WAS NO WAY AROUND IT.
        Dim exportColumnNames As List(Of String) = New List(Of String)(New String() {"EmplID", "EMPLOYEE_NAME", "RequestDT", "ExpirationDT", "ToHRDT", "FromHRDT", "RequestReason",
                                                                       "CurrentPosition", "FirstLineSupervisorName", "SecondLineSupervisorName", "CurrentProgram", "RequestProgram", "CurrentOffice",
                                                                       "RequestedOffices", "TransferDT", "TransferLocation", "CancelReason", "Comments"})

        Dim exportColumnHeaders As List(Of String) = New List(Of String)(New String() {"Employee Number", "Employee Name", "Request Dt", "Expire Dt", "Rec'd by DART", "Date Distributed", "Request Reason",
                                                                       "Position Title", "1st Line Sup", "2nd Line Sup", "Curr Program", "Req Program", "Curr Office",
                                                                       "Req Offices", "Transfer Dt", "Location", "Canceled", "Comments"})

        Dim dt As New DataTable
        Dim clsTransfers As New StaffTransferRequests.TransferRequests
        dt = clsTransfers.GetTransfers(ViewState("CurrentPositionFilterValue"), ViewState("CurrentProgramFilterValue"), ViewState("RequestedProgramFilterValue"),
        ViewState("CurrentOfficeFilterValue"), ViewState("RequestedOfficeFilterValue"), ViewState("ShowCanceledFilterValue"))

        Response.ClearContent()
        Response.Clear()
        Response.ContentType = "text/csv"
        Response.AddHeader("Content-Disposition", "attachment; filename=Transfers.csv;")

        Dim sb As New StringBuilder()
        ' Get the total number of columns returned in the datatable. We won't be using all of these so we loop through and get the ones that we want.
        Dim columnCount As Int32 = dt.Columns.Count - 1

        'Column Headers
        'Dim columnNames(exportColumnHeaders.Count - 1) As String
        Dim columnNames As New ArrayList
        Dim i As Int32 = 0
        Dim j As Int32 = 0
        Do While j < exportColumnHeaders.Count
            If exportColumnHeaders(j) = "Canceled" Then
                If ViewState("ShowCanceled") = True Then
                    columnNames.Add(exportColumnHeaders(j))
                End If
            Else
                columnNames.Add(exportColumnHeaders(j))
            End If
            j += 1
        Loop
        sb.AppendLine(String.Join(",", columnNames.ToArray()))

        'Data Rows
        Dim k As Int32 = 0
        Dim l As Int32 = 0
        For Each dr As DataRow In dt.Rows
            'Dim columnValues(exportColumnNames.Count - 1) As String
            Dim columnValues As New ArrayList
            k = 0
            l = 0
            ' Loop through the columns in the datatable and grab the ones that are named in the hard code above ^^
            Do While k < columnCount
                ' More hard coding because the requested offices have to be retrieved from a separate datatable because there are multiple requested offices per transfer record
                If l = 13 Then
                    Dim sbOffices As New StringBuilder()
                    Dim TransferRequestID As Int32 = dr.Item("ID")
                    Dim TR As New TransferRequests
                    Dim dtRequestedOffices As New DataTable
                    dtRequestedOffices = TR.GetTransferRequestLocations(TransferRequestID)
                    For Each drOffice As DataRow In dtRequestedOffices.Rows
                        sbOffices.Append(drOffice.Item("RequestedOffice") + ",")
                    Next
                    ' remove last comma from list of offices
                    sbOffices.Length = sbOffices.Length - 1
                    columnValues.Add(sbOffices.ToString)
                    l += 1
                    Continue Do
                End If
                '
                If exportColumnNames.Contains(dt.Columns(k).ColumnName) Then
                    If dt.Columns(k).ColumnName = "CancelReason" Then
                        If ViewState("ShowCanceled") = True Then
                            'columnValues.Add(dr.Item(k).ToString())
                            ' hard coded to put Cancel Reason in column 16
                            columnValues.Insert(16, dr.Item(k).ToString())
                        End If
                    ElseIf IsDate(dr.Item(k)) Then
                        columnValues.Add(CType(dr.Item(k), Date).ToShortDateString)
                    Else
                        columnValues.Add(dr.Item(k).ToString())
                    End If
                    l += 1
                End If
                k += 1
            Loop
            sb.AppendLine("""" & String.Join(""",""", columnValues.ToArray()) & """") ' escape commas contained in data fields so they aren't mistaken for CSV seperators
        Next
        Response.Write(sb.ToString())
        Response.Flush()
        Response.End()
    End Sub
    'Protected Sub btnPDF_Click(sender As Object, e As ImageClickEventArgs)
    '    Dim dirIn As String = Server.MapPath("~\App_Data\Forms\")
    '    Dim dirOut As String = Server.MapPath("~\")
    '    Dim pdfDocument As New Document(dirIn & "DPSS 1546 New.pdf")
    '    Dim textBoxField As TextBoxField = TryCast(pdfDocument.Form("tx1"), TextBoxField)
    '    textBoxField.Value = "TEST"
    '    pdfDocument.Save(dirOut & "DPSS 1546_out.pdf")
    '    Response.Redirect("~/DPSS 1546_out.pdf")
    'End Sub
    Protected Sub rdpExpirationDate_SelectedDateChanged(sender As Object, e As Calendar.SelectedDateChangedEventArgs)
        Try
            If Not CType(sender, RadDatePicker).SelectedDate Is Nothing Then
                If CType(CType(sender.parent, WebControl).FindControl("pnlEmail"), Panel).Enabled = False Then
                    Exit Sub
                End If
                'CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor).Enabled = True
                'CType(CType(sender.parent, WebControl).FindControl("chkSendEmail"), CheckBox).Visible = True
                'Dim clsEmployees As New Employees

                'Dim dtEmployee As New System.Data.DataTable
                'Dim txtEmailCC As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailCC"), RadTextBox)
                'txtEmailCC.Text = ConfigurationManager.AppSettings("SiteEmailAccount").ToString & ";"

                'If Not CType(CType(sender.parent, WebControl).FindControl("rcbFirstLineSupervisor"), RadComboBox).Text = String.Empty Then
                '    Dim FirstLineSupervisor As String = CType(CType(sender.parent, WebControl).FindControl("rcbFirstLineSupervisor"), RadComboBox).SelectedValue
                '    Dim txtEmailRecipients As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailRecipients"), RadTextBox)
                '    dtEmployee.Clear()
                '    dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(FirstLineSupervisor))
                '    If dtEmployee.Rows.Count > 0 Then
                '        txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                '    End If
                'End If
                'If Not CType(CType(sender.parent, WebControl).FindControl("rcbSecondLineSupervisor"), RadComboBox).Text = String.Empty Then
                '    Dim SecondLineSupervisor As String = CType(CType(sender.parent, WebControl).FindControl("rcbSecondLineSupervisor"), RadComboBox).SelectedValue
                '    Dim txtEmailRecipients As RadTextBox = CType(CType(sender.parent, WebControl).FindControl("txtEmailRecipients"), RadTextBox)
                '    dtEmployee.Clear()
                '    dtEmployee = clsEmployees.GetEmployeeInformation(Server.HtmlDecode(SecondLineSupervisor))
                '    If dtEmployee.Rows.Count > 0 Then
                '        txtEmailRecipients.Text = txtEmailRecipients.Text & dtEmployee.Rows(0).Item("LOGIN_ID").ToString.ToLower & "@riversidedpss.org;"
                '    End If
                'End If
                Dim txtEmail As Telerik.Web.UI.RadEditor = CType(CType(sender.parent, WebControl).FindControl("MailEditor"), RadEditor)
                Dim srEmailContent As System.IO.StreamReader
                srEmailContent = System.IO.File.OpenText(Server.MapPath("~\App_Data\EmailTexts\") & "TransferApprovalEmail.html")
                Dim strEmailContent As String = srEmailContent.ReadToEnd()
                Dim ExpirationDate As Date = CType(sender, RadDatePicker).SelectedDate.Value
                strEmailContent = Replace(strEmailContent, "&amp;&amp;ExpirationDate", ExpirationDate.ToString("MM/dd/yyyy"))
                txtEmail.Content = strEmailContent
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
