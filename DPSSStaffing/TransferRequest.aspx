<%@ Page Title="" Language="VB"  MasterPageFile="~/MasterPage.Master" AutoEventWireup="false" CodeFile="TransferRequest.aspx.vb" Inherits="TransferRequest" %>
<%@ MasterType VirtualPath="~/MasterPage.Master" %>
<%@ Register Namespace="CustomEditors" TagPrefix="custom" %>
<%@ Register Namespace="CustomEditors2" TagPrefix="custom2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
       <div>
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
            </asp:ScriptReference>
        </Scripts>
    </telerik:RadScriptManager>
    
     <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">
                //<![CDATA[
              
                function gridCommand(sender, args) {
                    // must turn off Ajax during download of document
                    if (args.get_commandName() == "DownloadAttachment") {
                        var manager = $find('<%= RadAjaxManager1.GetCurrent(Page).ClientID %>');
                        manager.set_enableAJAX(false);

                        setTimeout(function () {
                            manager.set_enableAJAX(true);
                        }, 0);
                    }
                }
                function conditionalPostback(sender, eventArgs) {
                    var eventArgument = eventArgs.get_eventArgument();
                    // must turn off Ajax during upload of document
                    if (eventArgument.indexOf("Update") > -1 || eventArgument.indexOf("PerformInsert") > -1) {
                        if (upload && upload.getFileInputs()[0].value != "") {
                            eventArgs.set_enableAjax(false);
                        }
                    }
                }

                // global variable container for the document object being uploaded
                var upload = null;

                function uploadFileSelected(sender, args) {
                    // grab the document object
                    upload = sender;
                    // this function automatically populates the FileName textbox with the name of the file selected to be uploaded
                    var uploadContainer = sender.get_element();
                    var editTable = uploadContainer.parentNode.parentNode.parentNode.parentNode;
                    // grab the file name from the edit forms FileName textbox which is in the 1 (index) row and the 1 (index) cell
                    // If your FileName textbox is in another location then these indexes have to be adjusted accordingly
                    // The rows index includes the header row of the editform which is the 0 index row so start counting down from there to find the FileName textbox
                    var fileNameTextBox = editTable.rows[1].cells[1].getElementsByTagName('input')[0];

                    //                fileNameTextBox.value = args.get_fileInputField().title;
                    fileNameTextBox.value = /([^\\]+)$/.exec(args.get_fileInputField().title)[1];
                }

                // Removing options Or, Not And, and Not Or and only allowing simple AND queries on the RadFilter
                function FilterCreated(sender, eventArgs) {
                    var filterMenu = sender.get_contextMenu();
                    filterMenu.add_showing(FilterMenuShowing);
                }

                function FilterMenuShowing(sender, eventArgs) {
                    sender.findItemByValue("Or").set_visible(false);
                    sender.findItemByValue("NotAnd").set_visible(false);
                    sender.findItemByValue("NotOr").set_visible(false);
                    sender.findItemByValue("Contains").set_visible(false);
                    sender.findItemByValue("DoesNotContain").set_visible(false);
                    sender.findItemByValue("StartsWith").set_visible(false);
                    sender.findItemByValue("EndsWith").set_visible(false);
                    sender.findItemByValue("EqualTo").set_visible(false);
                    sender.findItemByValue("NotEqualTo").set_visible(false);
                    sender.findItemByValue("GreaterThan").set_visible(false);
                    sender.findItemByValue("LessThan").set_visible(false);
                    sender.findItemByValue("GreaterThanOrEqualTo").set_visible(false);
                    sender.findItemByValue("LessThanOrEqualTo").set_visible(false);
                    sender.findItemByValue("Between").set_visible(false);
                    sender.findItemByValue("NotBetween").set_visible(false);
                    sender.findItemByValue("IsEmpty").set_visible(false);
                    sender.findItemByValue("IsNull").set_visible(false);
                    sender.findItemByValue("NotIsNull").set_visible(false);
                    sender.findItemByValue("NotIsEmpty").set_visible(false);
                }

                function EmailRecipientsSelectedIndexChanged(sender, eventArgs) {
                    var item = eventArgs.get_item();
                    $get("txtEmailRecipients").innerHTML = item.get_text();
                }

                function OpenAttachmentSelectionWindow() {
                    try {
                        var oWnd = window.radopen(null, "winSelectEmailAttachments");
                    } catch (e) {
                        alert(e);
                    }
                    return false;
                }

                function OnClientEmailAttachmentsClose(sender, eventArgs) {
                    var arg = eventArgs.get_argument();
                    if (arg.AttachmentList.substring(0, 6) != 'Cancel') {
                        //window.myHidden.value = window.myHidden.value + arg.AttachmentList;
                        $get("txtEmailAttachments").innerHTML = arg.AttachmentList;
                    }
                }

                function isNumericKey(e) {
                    var charInp = window.event.keyCode;
                    if (charInp > 31 && (charInp < 48 || charInp > 57)) {
                        return false;
                    }
                    return true;
                }


            </script>
        </telerik:RadCodeBlock>
 
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <ClientEvents OnRequestStart="conditionalPostback" />
            <Ajaxsettings>
                <telerik:AjaxSetting AjaxControlID="rgdTransfers">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rgdTransfers" LoadingPanelID="RadAjaxLoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="lblRecordCount" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </Ajaxsettings>
        </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>
    <table style="width: 100%;">
        <tr>
            <td>
                <asp:Panel ID="pnlSearch" runat="server" GroupingText="Search">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 25%; text-align: left">
                                <telerik:RadFilter ID="rfTransfers" runat="server" OnFieldEditorCreating="rfTransfers_FieldEditorCreating" CssClass="RadFilter RadFilter_Default ">
                                    <ClientSettings>
                                        <ClientEvents OnFilterCreated="FilterCreated" />
                                    </ClientSettings>
                                    <FieldEditors>
                                        <custom:RadCustomFilterDropDownEditor FieldName="Current Position" DataTextField="DataValue" DataValueField="ID" />
                                        <custom:RadCustomFilterDropDownEditor FieldName="Current Program" DataTextField="DataValue" DataValueField="ID" />
                                        <custom:RadCustomFilterDropDownEditor FieldName="Requested Program" DataTextField="DataValue" DataValueField="ID" />
                                        <custom:RadCustomFilterDropDownEditor FieldName="Current Office" DataTextField="DataValue" DataValueField="ID" />
                                        <custom:RadCustomFilterDropDownEditor FieldName="Requested Office" DataTextField="DataValue" DataValueField="ID" />
                                        <custom2:MyRadFilterCheckBoxEditor FieldName="Show Canceled" />
                                    </FieldEditors>
                                </telerik:RadFilter>
                                <asp:Label ID="lblRecordCount" runat="server" Text="Label" CssClass="StandardLabel"></asp:Label>
                            </td>
                            <td style="width: 75%; text-align:right; vertical-align:top">
                                <asp:ImageButton ID="btnExport" runat="server" ImageUrl="images/Excel_XLSX.PNG" OnClick="btnExport_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td> 
                <telerik:RadGrid ID="rgdTransfers" runat="server" AllowPaging="True" AllowSorting="True" DataSourceID="objDSTransfers" Width="100%"  HorizontalAlign="Center" AutoGenerateColumns="False" 
                    AllowFilteringByColumn="True" GroupingSettings-CaseSensitive="False">
                    <GroupingSettings CaseSensitive="False"></GroupingSettings>

                    <HierarchySettings ExpandTooltip="Documents" />
                    <MasterTableView DataSourceID="objDSTransfers" DataKeyNames="ID" commanditemdisplay="Top" AllowAutomaticInserts="True" 
                        AllowAutomaticUpdates="True" IsFilterItemExpanded="True" AllowAutomaticDeletes="True" InsertItemPageIndexAction="ShowItemOnFirstPage">
                    <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                    <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column">
                    <HeaderStyle></HeaderStyle>
                    </RowIndicatorColumn>

                    <ExpandCollapseColumn Visible="True" Created="True">
                    <HeaderStyle Width="10px"></HeaderStyle>
                    </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridEditCommandColumn FilterControlAltText="Filter EditCommandColumn column" ButtonType="ImageButton">
                                <HeaderStyle Wrap="False" />
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn DataField="ID" DataType="System.Int32" Display="False" FilterControlAltText="Filter ID column" UniqueName="ID" Visible="False" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EmplID" FilterControlAltText="" HeaderText="Employee<br />Number" UniqueName="EmplID" ItemStyle-VerticalAlign="Top" >
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EMPLOYEE_NAME" FilterControlAltText="Filter EMPLOYEE_NAME column" HeaderText="Employee<br />Name" UniqueName="EMPLOYEE_NAME" ItemStyle-VerticalAlign="Top">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn FilterControlAltText="Filter RequestExpirationDate column" UniqueName="RequestExpirationDate" HeaderText="Request Dt/&lt;br /&gt;Expire Dt" AllowFiltering="false" >
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top" />
                                 <ItemTemplate>
                                    <%# Eval("RequestDT", "{0:d}")%>
                                    <br />
                                    <%# Eval("ExpirationDT", "{0:d}")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="FromHRDT" DataFormatString="{0:d}" FilterControlAltText="Filter RequestDate column" HeaderText="" UniqueName="FromHRDT" DataType="System.DateTime" AllowFiltering="False" ItemStyle-VerticalAlign="Top" Display="false">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" UniqueName="HRDates" HeaderText="Rec'd by DART/&lt;br /&gt;Date Distributed" AllowFiltering="false">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top" />
                                <ItemTemplate>
                                    <%# Eval("ToHRDT", "{0:d}")%>
                                    <br />
                                    <%# Eval("FromHRDT", "{0:d}")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="tlkpRequestReason"  HeaderText="" UniqueName="tlkpRequestReason" Display="false" ItemStyle-VerticalAlign="Top" AllowFiltering="False">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top" Wrap="true"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="RequestReason"  HeaderText="Request Reason" UniqueName="RequestReason"  ItemStyle-VerticalAlign="Top" AllowFiltering="False">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top" Width="50px"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <%--<telerik:GridBoundColumn DataField="OasisPosition"  HeaderText="OASIS Position" UniqueName="OasisPosition" ItemStyle-VerticalAlign="Top" AllowFiltering="False">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>--%>
                            <telerik:GridBoundColumn DataField="CurrentPosition"  HeaderText="Position<br />Title" UniqueName="CurrentPosition" ItemStyle-VerticalAlign="Top" AllowFiltering="False">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="1st Line Sup/&lt;br /&gt;2nd Line Sup" UniqueName="Supervisor">
                                <ItemTemplate>
                                    <%# Eval("FirstLineSupervisorName")%>
                                    <br /><hr />
                                    <%# Eval("SecondLineSupervisorName")%>
                                </ItemTemplate>

                                <HeaderStyle Wrap="False" />

                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="ExpirationDT"  UniqueName="ExpirationDT" AllowFiltering="False" Display="False" ItemStyle-VerticalAlign="Top">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpProgram"  UniqueName="tlkpProgram" AllowFiltering="False" Display="False" ItemStyle-VerticalAlign="Top">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpReqProgram"  HeaderText="" UniqueName="tlkpReqProgram" Display="false" AllowFiltering="False">
                                <HeaderStyle Wrap="False" />
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Curr Program/<br />Req Program" UniqueName="Program" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <b>Current:</b><br /><%# Eval("CurrentProgram")%><br /><b>Requested:</b><br /><%# Eval("RequestProgram")%>
                                </ItemTemplate>

                                <HeaderStyle Wrap="False" />

                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="tlkpOffice" HeaderText="" UniqueName="tlkpOffice" Display="false" AllowFiltering="False">
                                <HeaderStyle Wrap="False" />
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Curr Office/<br />Req Offices" UniqueName="Office" AllowFiltering="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <b>Current:</b><br /> <%# Eval("CurrentOffice")%>
                                    <br />
                                    <b>Requested:</b><br />
                                    <telerik:RadListBox ID="lbRequestedOfficesDisplay" runat="server" Width="100px" CssClass="RadListBox_Default"></telerik:RadListBox>
                                </ItemTemplate>

                                <HeaderStyle Wrap="False" />

                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="tlkpTransLocation" HeaderText="" UniqueName="tlkpTransLocation" Display="false" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="Transfer Dt/<br />Location" AllowFiltering="False" ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <%# Eval("TransferDT", "{0:d}")%>
                                    <br />
                                    <%# Eval("TransferLocation")%>
                                </ItemTemplate>
                                <HeaderStyle Wrap="false" />
                                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="tlkpCancelReason" UniqueName="tlkpCancelReason" AllowFiltering="False" ItemStyle-VerticalAlign="Top" Display="False">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Canceled" UniqueName="Cancelled" >
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbCanceled" runat="server" Enabled="false" Checked='<%# Eval("Cancelled") %>' /><br /><%# Eval("CancelReason") %> 
                                </ItemTemplate>
                                 <HeaderStyle Wrap="False" />
                                 <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Wrap="False"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Comments" AllowFiltering="false" HeaderText="Comments" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <telerik:RadTextBox ID="txtComments" runat="server" Text='<%# Eval("Comments") %>' TextMode="MultiLine" BorderStyle="None" Height="75px"></telerik:RadTextBox>                                    
                                </ItemTemplate>
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="FirstLineSupervisorEmplID" FilterControlAltText="Filter FirstLineSupervisorEmplID column" UniqueName="FirstLineSupervisorEmplID" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SecondLineSupervisorEmplID" FilterControlAltText="Filter SecondLineSupervisorEmplID column" UniqueName="SecondLineSupervisorEmplID" Display="False">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FirstLineSupervisorName" FilterControlAltText="Filter FirstLineSupervisorName column" UniqueName="FirstLineSupervisorName" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SecondLineSupervisorName" FilterControlAltText="Filter SecondLineSupervisorName column" UniqueName="SecondLineSupervisorName" Display="False">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpPosition" DataType="System.Int32" Display="False" FilterControlAltText="Filter tlkpPosition column" UniqueName="tlkpPosition">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" FilterControlAltText="Filter DeleteTransferButton column" 
                                ConfirmText="Delete this transfer and all related documents?" ConfirmDialogType="RadWindow" ImageUrl="~/images/RadDelete.gif" Text="Delete" UniqueName="DeleteTransferButton">
                            </telerik:GridButtonColumn>
                        </Columns>
                    
                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            
                            <table class="TableCellspacing0">
                                <tr>
                                    <td style="width: 700px; text-align: left; vertical-align: text-top">
                                        <table class="TableCellspacing0">
                                            <tr>                                                
                                                <td style="width: 20%; text-align: right"></td>
                                                <td style="width: 30%; text-align: left"></td>
                                                <td style="width: 20%; text-align: right"></td>
                                                <td style="width: 30%; text-align: left"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Panel ID="pnlInsertEmployeeInfo" runat="server" BorderStyle="None" BorderWidth="0px"
                                                        Visible='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "true", "false")%>'>
                                                        <table class="TableCellspacing0" style="width: 99%">
                                                            <tr>
                                                                <td style="width: 19%"></td>
                                                                <td style="width: 31%"></td>
                                                                <td style="width: 19%"></td>
                                                                <td style="width: 31%"></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="text-align: right">
                                                                    <asp:Label ID="Label12" runat="server" Text="Employee Name:"></asp:Label>
                                                                </td>
                                                                <td style="text-align: left">
                                                                    <telerik:RadComboBox ID="rcbEmployees" runat="server" Width="160px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                        DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                        ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" CausesValidation="false" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged">
                                                                    </telerik:RadComboBox>
                                                                    <asp:RequiredFieldValidator ID="reqvalEmployee" runat="server" ErrorMessage="Employee name required." ControlToValidate="rcbEmployees" CssClass="ValidatorRed" ValidationGroup="Transfers">*</asp:RequiredFieldValidator>
                                                                </td>
                                                                <td style="text-align: right">
                                                                    <asp:Label ID="Label13" runat="server" Text="Employee Number:"></asp:Label>
                                                                </td>
                                                                <td style="text-align: left">
                                                                    <telerik:RadTextBox ID="txtEmployeeNumber" runat="server" ReadOnly="True" Enabled="False"></telerik:RadTextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="text-align: right">
                                                                    <asp:Label ID="lblOasisPosition" runat="server" Text="OASIS Position:"></asp:Label>
                                                                </td>
                                                                <td style="text-align: left">
                                                                    <telerik:RadTextBox ID="txtOasisPosition" runat="server" ReadOnly="true" Enabled="False"></telerik:RadTextBox>
                                                                </td>
                                                                <td style="text-align: right">
                                                                    <asp:Label ID="Label14" runat="server" Text="Supervisor:"></asp:Label>
                                                                </td>
                                                                <td style="text-align: left">
                                                                    <telerik:RadTextBox ID="txtOasisSupervisor" runat="server" ReadOnly="true" Enabled="False"></telerik:RadTextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%; text-align: right">
                                                    <asp:Label ID="Label16" runat="server" Text="Current Position:"></asp:Label>
                                                </td>
                                                <td style="width: 30%; text-align: left">
                                                    <telerik:RadComboBox ID="rcbCurrentPosition" runat="server" DataSourceID="objDSCurrentPositions" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%#Bind("tlkpPosition")%>'></telerik:RadComboBox>
                                                    <asp:RequiredFieldValidator ID="reqvalCurrentPosition" runat="server" ErrorMessage="Current position required." ControlToValidate="rcbCurrentPosition" CssClass="ValidatorRed" ValidationGroup="Transfers">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="width: 20%; text-align: right">
                                                    <asp:Label ID="Label17" runat="server" Text="First Line Super:"></asp:Label>
                                                </td>
                                                <td style="width: 30%; text-align: left">
                                                    <telerik:RadComboBox ID="rcbFirstLineSupervisor" runat="server" Width="160px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                        DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                        ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged">
                                                    </telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label7" runat="server" Text="Current Office:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadComboBox ID="rcbCurrentOffice" runat="server" DataSourceID="objDSOffices" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%#Bind("tlkpOffice")%>' Height="250px"></telerik:RadComboBox>
                                                    <asp:RequiredFieldValidator ID="reqvalCurrentOffice" runat="server" ErrorMessage="Current office required." ControlToValidate="rcbCurrentOffice" CssClass="ValidatorRed" ValidationGroup="Transfers">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label18" runat="server" Text="Second Line Super:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadComboBox ID="rcbSecondLineSupervisor" runat="server" Width="160px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                        DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                        ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged">
                                                    </telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label3" runat="server" Text="Current Program:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadComboBox ID="rcbCurrentProgram" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%#Bind("tlkpProgram")%>'></telerik:RadComboBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label4" runat="server" Text="Requested Program:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadComboBox ID="rcbRequestProgram" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%#Bind("tlkpReqProgram")%>'></telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%; text-align: right">
                                                    <asp:Label ID="lblRequestDate" runat="server" Text="Request Date:"></asp:Label>
                                                </td>
                                                <td style="width: 30%; text-align: left; text-wrap: none">
                                                    <telerik:RadDatePicker ID="rdpRequestDate" runat="server" MinDate="1/1/1990" DbSelectedDate='<%# Bind("RequestDT")%>' Calendar-ShowRowHeaders="False"></telerik:RadDatePicker>
                                                    <asp:RequiredFieldValidator ID="reqvalRequestDate" runat="server" ErrorMessage="Request date required." CssClass="ValidatorRed" ControlToValidate="rdpRequestDate" ValidationGroup="Transfers">*</asp:RequiredFieldValidator>
                                                    <asp:CustomValidator ID="valRequestDate" runat="server" ErrorMessage="Duplicate request date."
                                                        OnServerValidate="CheckForDuplicateTransfer" ControlToValidate="rdpRequestDate" CssClass="ValidatorRed" ValidationGroup="Transfers">*</asp:CustomValidator>
                                                </td>
                                                <td style="width: 20%; text-align: right">
                                                    <asp:Label ID="Label15" runat="server" Text="Request Reason:"></asp:Label>
                                                </td>
                                                <td style="width: 30%; text-align: left">
                                                    <telerik:RadComboBox ID="rcbRequestReason" runat="server" DataSourceID="objDSXferReasons" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpRequestReason") %>'></telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%; text-align: right">
                                                    <asp:Label ID="lblExpirationDate" runat="server" Text="Expiration Date:"></asp:Label>
                                                </td>
                                                <td style="width: 30%; text-align: left; text-wrap: none">
                                                    <telerik:RadDatePicker ID="rdpExpirationDate" runat="server" MinDate="1/1/1990" DbSelectedDate='<%# Bind("ExpirationDt")%>'  Calendar-ShowRowHeaders="False" OnSelectedDateChanged="rdpExpirationDate_SelectedDateChanged" AutoPostBack="true"></telerik:RadDatePicker>
                                                    <asp:RequiredFieldValidator ID="reqvalExpirationDate" runat="server" ErrorMessage="Expiration date required." CssClass="ValidatorRed" ControlToValidate="rdpExpirationDate" ValidationGroup="Transfers">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td colspan="2">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label1" runat="server" Text="Rec'd by DART:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadDatePicker ID="rdpDateSentToHR" runat="server" DbSelectedDate='<%# Bind("ToHRDT")%>' Calendar-ShowRowHeaders="False"></telerik:RadDatePicker>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label2" runat="server" Text="Date Distributed:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadDatePicker ID="rdpRecdFromHR" runat="server" DbSelectedDate='<%# Bind("FromHRDT")%>' Calendar-ShowRowHeaders="False" AutoPostBack="True" OnSelectedDateChanged="rdpRecdFromHR_SelectedDateChanged">
                                                        <Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" ShowRowHeaders="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
                                                        </Calendar>
                                                        <DateInput AutoPostBack="True" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelWidth="40%">
                                                            <EmptyMessageStyle Resize="None" />
                                                            <ReadOnlyStyle Resize="None" />
                                                            <FocusedStyle Resize="None" />
                                                            <DisabledStyle Resize="None" />
                                                            <InvalidStyle Resize="None" />
                                                            <HoveredStyle Resize="None" />
                                                            <EnabledStyle Resize="None" />
                                                        </DateInput>
                                                        <DatePopupButton HoverImageUrl="" ImageUrl="" />
                                                    </telerik:RadDatePicker>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label5" runat="server" Text="Available Offices:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadListBox ID="rlbAvailableOffices"
                                                        Skin="Vista"
                                                        Width="190px"
                                                        Height="100px"
                                                        AllowTransfer="True"
                                                        TransferToID="rlbSelectedOffices"
                                                        SelectionMode="Multiple"
                                                        runat="server" DataTextField="DataValue" DataValueField="ID" AutoPostBackOnTransfer="True" DataSortField="DataValue" Style="top: 0px; left: 0px" OnTransferred="rlbAvailableOffices_Transferred">
                                                        <ButtonSettings ShowDelete="false" ShowReorder="false" ShowTransferAll="false" />
                                                    </telerik:RadListBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label19" runat="server" Text="Selected Offices:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadListBox ID="rlbSelectedOffices"
                                                        Skin="Vista"
                                                        Width="190px"
                                                        Height="100px"
                                                        AllowReorder="True"
                                                        SelectionMode="Multiple"
                                                        runat="server" DataSourceID="" DataTextField="DataValue" DataValueField="ID" Style="top: 0px; left: 0px">
                                                        <ButtonSettings ShowDelete="false" ShowReorder="True" ReorderButtons="Common" RenderButtonText="False" />
                                                    </telerik:RadListBox>
                                                    <asp:CustomValidator ID="valSelectedOffices" runat="server" ErrorMessage="Requested office(s) required."
                                                        ControlToValidate="rlbSelectedOffices" CssClass="ValidatorRed" ValidationGroup="Transfers">*</asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label6" runat="server" Text="Transfer Date:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadDatePicker ID="rdpTransferDate" runat="server" Culture="en-US" DbSelectedDate='<%# Bind("TransferDT") %>' Calendar-ShowRowHeaders="False"></telerik:RadDatePicker>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label8" runat="server" Text="Transfer Location:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadComboBox ID="rcbTransferLocation" runat="server" DataSourceID="objDSOffices" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpTransLocation")%>'></telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label9" runat="server" Text="Request Canceled:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <asp:CheckBox ID="chkCanceled" runat="server" Checked='<%# Bind("Cancelled")%>' />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label10" runat="server" Text="Cancel Reason:"></asp:Label>
                                                </td>
                                                <td style="text-align: left">
                                                    <telerik:RadComboBox ID="rcbCancelReason" runat="server" DataSourceID="objDSCancelReasons" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpCancelReason")%>'></telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:Label ID="Label11" runat="server" Text="Comments:"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <telerik:RadTextBox ID="txtComments" runat="server" LabelWidth="0px" Text='<%# Bind("Comments") %>' Width="510px" TextMode="MultiLine"></telerik:RadTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: center" colspan="4">
                                                    <asp:ImageButton ID="btnUpdate" runat="server" CommandName='<%# IIf(CType(Container,GridItem).OwnerTableView.IsItemInserted ,"PerformInsert","Update") %>' ImageUrl='<%# IIf(CType(Container,GridItem).OwnerTableView.IsItemInserted,"~/images/btnAdd.gif","~/images/btnUpdate.gif") %>' ToolTip="Add" CausesValidation="true" ValidationGroup="Transfers" />
                                                    <asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ImageUrl="~/Images/btnCancel.gif" ToolTip="Cancel" CausesValidation="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="text-align: center">
                                                    <asp:CheckBox ID="chkSendEmail" runat="server" Text="Send Email" Checked="true" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <telerik:RadTextBox ID="txtEmplID" runat="server" Visible="false" Text='<%# Bind("EmplID")%>'></telerik:RadTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <telerik:RadTextBox ID="txtFirstLineSupervisorEmplID" runat="server" Visible="false" Text='<%# Bind("FirstLineSupervisorEmplID")%>'></telerik:RadTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <telerik:RadTextBox ID="txtSecondLineSupervisorEmplID" runat="server" Visible="false" Text='<%# Bind("SecondLineSupervisorEmplID")%>'></telerik:RadTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="ValidatorRed" ValidationGroup="Transfers" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="text-align: left; vertical-align: text-top; align-content: stretch">
                                        <asp:Panel runat="server" ID="pnlEmail" GroupingText="Email" BorderStyle="Solid" BorderWidth="1px">
                                            <input runat="server" type="hidden" id="UploadedFilesJson" />


                                            <div class="qsf-demo-canvas">
                                                <div class="qsf-fb">
                                                    <table style="border-collapse: collapse; border-spacing: 0; width: 100%">
                                                        <tr>
                                                            <td style="text-align: left; vertical-align: top">
                                                                <telerik:RadComboBox ID="rcbEmailRecipients" runat="server" Width="200px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                    DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                    ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailRecipient_SelectedIndexChanged" AllowCustomText="True">
                                                                </telerik:RadComboBox>
                                                            </td>
                                                            <td style="text-align: right; vertical-align: top">
                                                                <asp:Label ID="Label20" runat="server" Text="To:"></asp:Label>
                                                            </td>
                                                            <td style="text-align: left; vertical-align: top" rowspan="2">
                                                                <telerik:RadTextBox ID="txtEmailRecipients" runat="server" ClientIDMode="Static"
                                                                    EnableEmbeddedSkins="false" EnableViewState="false" TextMode="MultiLine" Width="300px" Height="50px">
                                                                </telerik:RadTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="text-align: left; vertical-align: top">
                                                                <telerik:RadButton ID="btnAddEmailRecipient" runat="server" Text="Add" OnClick="btnAddEmailRecipient_Click" Width="70px"></telerik:RadButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="qsf-fb">
                                                    <table style="border-collapse: collapse; border-spacing: 0; width: 100%">
                                                        <tr>
                                                            <td style="text-align: left; vertical-align: top">
                                                                <telerik:RadComboBox ID="rcbEmailCC" runat="server" Width="200px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                    DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                    ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailCC_SelectedIndexChanged" AllowCustomText="True">
                                                                </telerik:RadComboBox>
                                                            </td>
                                                            <td style="text-align: right; vertical-align: top">
                                                                <asp:Label ID="Label21" runat="server" Text="CC:"></asp:Label>
                                                            </td>
                                                            <td style="text-align: left; vertical-align: top" rowspan="2">
                                                                <telerik:RadTextBox ID="txtEmailCC" runat="server"
                                                                    EnableEmbeddedSkins="false" EnableViewState="false" TextMode="MultiLine" Width="300px" Height="50px">
                                                                </telerik:RadTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="text-align: left; vertical-align: top">
                                                                <telerik:RadButton ID="btnAddEmailCC" runat="server" Text="Add" OnClick="btnAddEmailCC_Click" Width="70px"></telerik:RadButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="qsf-fb">
                                                    <asp:Label ID="Label22" runat="server" Text="Subject:"></asp:Label>
                                                    <telerik:RadTextBox ID="txtSubject" runat="server" EnableEmbeddedSkins="false"
                                                        Text="Transfer Request Notification (Classification – Employee)" EnableViewState="false" Width="481px">
                                                    </telerik:RadTextBox>
                                                </div>
                                                <div class="qsf-fb-group qsf-fb-attachments">
                                                    <asp:Image ID="Image1" runat="server" ImageUrl="images/qsf-demo-attachment-bg.png" />
                                                    <asp:Button ID="btnSelectAttachments" runat="server" Text="Select Attachments" OnClientClick="return OpenAttachmentSelectionWindow()" CausesValidation="False" UseSubmitBehavior="False" />
                                                    <telerik:RadTextBox ID="txtEmailAttachments" runat="server" TextMode="MultiLine" ClientIDMode="Static" Text="" Height="68px" Width="342px"></telerik:RadTextBox>
                                                    <%--ClientEvents-OnLoad="onTextEmailAttachmentsLoad"--%>
                                                </div>
                                                <div>

                                                    <telerik:RadEditor ID="MailEditor" runat="server" Width="100%" Height="175px" EditModes="All" EnableResize="False">
                                                        <Tools>
                                                            <telerik:EditorToolGroup Tag="MainToolbar">
                                                                <telerik:EditorTool Name="SpellCheck" />
                                                                <telerik:EditorTool Name="FindAndReplace" />
                                                                <telerik:EditorSeparator />
                                                                <telerik:EditorSplitButton Name="Undo">
                                                                </telerik:EditorSplitButton>
                                                                <telerik:EditorSplitButton Name="Redo">
                                                                </telerik:EditorSplitButton>
                                                                <telerik:EditorSeparator />
                                                                <telerik:EditorTool Name="Cut" />
                                                                <telerik:EditorTool Name="Copy" />
                                                                <telerik:EditorTool Name="Paste" ShortCut="CTRL+V" />
                                                            </telerik:EditorToolGroup>
                                                            <telerik:EditorToolGroup Tag="Formatting">
                                                                <telerik:EditorTool Name="Bold" />
                                                                <telerik:EditorTool Name="Italic" />
                                                                <telerik:EditorTool Name="Underline" />
                                                                <telerik:EditorSeparator />
                                                                <telerik:EditorSplitButton Name="ForeColor">
                                                                </telerik:EditorSplitButton>
                                                                <telerik:EditorSplitButton Name="BackColor">
                                                                </telerik:EditorSplitButton>
                                                                <telerik:EditorSeparator />
                                                                <telerik:EditorDropDown Name="FontName">
                                                                </telerik:EditorDropDown>
                                                                <telerik:EditorDropDown Name="RealFontSize">
                                                                </telerik:EditorDropDown>
                                                            </telerik:EditorToolGroup>
                                                        </Tools>
                                                        <Content>
                                                        </Content>

                                                        <TrackChangesSettings CanAcceptTrackChanges="False"></TrackChangesSettings>
                                                    </telerik:RadEditor>

                                                </div>
                                        </asp:Panel>
                                        <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </FormTemplate>
                    <EditColumn></EditColumn>
                    </EditFormSettings>

                    <PagerStyle PageSizeControlType="RadComboBox" AlwaysVisible="True"></PagerStyle>

                        <DetailTables>
                            <telerik:GridTableView runat="server" AllowAutomaticDeletes="True" Width="100%"
                                AutoGenerateColumns="False" CommandItemDisplay="Top" DataSourceID="objTransferDocuments" ShowHeadersWhenNoRecords="true" DataKeyNames="ID" AllowFilteringByColumn="False" AllowSorting="False" NoDetailRecordsText="No documents attached.">
                                <ParentTableRelation>
                                    <telerik:GridRelationFields DetailKeyField="TransferRequestID" MasterKeyField="ID" />
                                </ParentTableRelation>
                                <CommandItemSettings ExportToPdfText="Export to PDF" AddNewRecordText="Add new document" />
                                <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                    <HeaderStyle Width="20px" />
                                </RowIndicatorColumn>
                                <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                    <HeaderStyle Width="20px" />
                                </ExpandCollapseColumn>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="ID" DataType="System.Int32" FilterControlAltText="Filter ID column" HeaderText="ID" UniqueName="ID" Visible="false" ReadOnly="true">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="TransferRequestID" DataType="System.Int32" FilterControlAltText="Filter TransferRequestID column" HeaderText="Transfer Req ID" UniqueName="TransferRequestID" Visible="false" ReadOnly="true">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="FileName" FilterControlAltText="Filter FileName column" HeaderText="File Name" UniqueName="FileName">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridDateTimeColumn DataField="TimeLoaded" FilterControlAltText="Filter TimeLoaded column" HeaderText="Time Loaded" UniqueName="TimeLoaded" ReadOnly="true">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridBoundColumn DataField="LoadedBy" FilterControlAltText="Filter LoadedBy column" HeaderText="Loaded By" UniqueName="LoadedBy" ReadOnly="true">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridAttachmentColumn AttachmentDataField="Data" AttachmentKeyFields="ID" DataTextField="FileName" EditFormHeaderTextFormat="Upload File:" FileName="attachment" FileNameTextField="FileName" FilterControlAltText="Filter Data column" HeaderText="Download" UniqueName="Data">
                                    </telerik:GridAttachmentColumn>
                                    <telerik:GridButtonColumn ConfirmText="Delete this document?" ConfirmDialogType="RadWindow"
                                        ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete" ImageUrl="~/images/RadDelete.gif"
                                        UniqueName="DeleteColumn">
                                        <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton"></ItemStyle>
                                    </telerik:GridButtonColumn>
                                </Columns>
                                <EditFormSettings>
                                    <EditColumn FilterControlAltText="Filter EditCommandColumn column" InsertImageUrl="images/btnAdd.gif" CancelImageUrl="images/btnCancel.gif" ButtonType="ImageButton">
                                    </EditColumn>
                                </EditFormSettings>
                                <PagerStyle PageSizeControlType="RadComboBox" />
                            </telerik:GridTableView>
                        </DetailTables>

                    </MasterTableView>
                    <ClientSettings>
                        <ClientEvents OnCommand="gridCommand"></ClientEvents>
                    </ClientSettings>
                    <PagerStyle PageSizeControlType="RadComboBox" AlwaysVisible="True" PageSizes="5;10;20"></PagerStyle>

                    <FilterMenu EnableImageSprites="False"></FilterMenu>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
           <asp:ObjectDataSource ID="objDSTransfers" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetTransfers" TypeName="StaffTransferRequests.TransferRequests" InsertMethod="InsertTransfer" UpdateMethod="UpdateTransfer" DeleteMethod="DeleteTransfer">
               <DeleteParameters>
                   <asp:Parameter Name="Original_ID" Type="Int32" />
               </DeleteParameters>
               <InsertParameters>
                   <asp:Parameter Name="EmplID" Type="Int32" />
                   <asp:Parameter Name="RequestDT" Type="DateTime" />
                   <asp:Parameter Name="ToHRDT" Type="DateTime" />
                   <asp:Parameter Name="ExpirationDT" Type="DateTime" />
                   <asp:Parameter Name="tlkpRequestReason" Type="Int32" />
                   <asp:Parameter Name="tlkpPosition" Type="Int32" />
                   <asp:Parameter Name="FirstLineSupervisorEmplID" Type="String" />
                   <asp:Parameter Name="SecondLineSupervisorEmplID" Type="String" />
                   <asp:Parameter Name="FromHRDT" Type="DateTime" />
                   <asp:Parameter Name="tlkpOffice" Type="Int32" />
                   <asp:Parameter Name="tlkpProgram" Type="Int32" />
                   <asp:Parameter Name="tlkpReqProgram" Type="Int32" />
                   <asp:Parameter Name="Cancelled" Type="Boolean" />
                   <asp:Parameter Name="tlkpCancelReason" Type="Int32" />
                   <asp:Parameter Name="TransferDT" Type="DateTime" />
                   <asp:Parameter Name="tlkpTransLocation" Type="Int32" />
                   <asp:Parameter Name="Comments" Type="String" />
                   <asp:Parameter Name="TransferRequestLocations" Type="Object" />
               </InsertParameters>
               <SelectParameters>
                   <asp:Parameter Name="CurrentPosition" Type="Int32" />
                   <asp:Parameter Name="CurrentProgram" Type="Int32" />
                   <asp:Parameter Name="RequestedProgram" Type="Int32" />
                   <asp:Parameter Name="CurrentOffice" Type="Int32" />
                   <asp:Parameter Name="RequestedOffice" Type="Int32" />
                   <asp:Parameter Name="ShowCanceled" Type="Boolean" />
               </SelectParameters>
               <UpdateParameters>
                   <asp:Parameter Name="EmplID" Type="Int32" />
                   <asp:Parameter Name="RequestDT" Type="DateTime" />
                   <asp:Parameter Name="ToHRDT" Type="DateTime" />
                   <asp:Parameter Name="ExpirationDT" Type="DateTime" />
                   <asp:Parameter Name="tlkpRequestReason" Type="Int32" />
                   <asp:Parameter Name="tlkpPosition" Type="Int32" />
                   <asp:Parameter Name="FirstLineSupervisorEmplID" Type="String" />
                   <asp:Parameter Name="SecondLineSupervisorEmplID" Type="String" />
                   <asp:Parameter Name="FromHRDT" Type="DateTime" />
                   <asp:Parameter Name="tlkpOffice" Type="Int32" />
                   <asp:Parameter Name="tlkpProgram" Type="Int32" />
                   <asp:Parameter Name="tlkpReqProgram" Type="Int32" />
                   <asp:Parameter Name="Cancelled" Type="Boolean" />
                   <asp:Parameter Name="tlkpCancelReason" Type="Int32" />
                   <asp:Parameter Name="TransferDT" Type="DateTime" />
                   <asp:Parameter Name="tlkpTransLocation" Type="Int32" />
                   <asp:Parameter Name="Comments" Type="String" />
                   <asp:Parameter Name="TransferRequestLocations" Type="Object" />
                   <asp:Parameter Name="Original_ID" Type="Int32" />
               </UpdateParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSXferReasons" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Request Reasons" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSOffices" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Office Locations" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSProgramsDisplayOnly" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetTransferRequestLocations" TypeName="StaffTransferRequests.TransferRequests">
               <SelectParameters>
                   <asp:Parameter DefaultValue="" Name="TransferID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSPrograms" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Program" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSCancelReasons" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Cancel Reason" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSXferLocations" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Office Locations" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSRequestedOffices" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookups" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Office Locations" Name="Category" Type="String" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objTransferDocuments" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetTransferDocuments" TypeName="StaffTransferRequests.TransferRequests" DeleteMethod="DeleteTransferDocument">
               <DeleteParameters>
               </DeleteParameters>
               <SelectParameters>
                   <asp:Parameter Name="TransferRequestID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objTransferAvaliableLocations" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetTransferAvailableLocations" TypeName="StaffTransferRequests.TransferRequests">
               <SelectParameters>
                   <asp:Parameter Name="TransferID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSCurrentPositions" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Current Position" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="objDSRequestedPrograms" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Program" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
           <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
            <Windows>
                <telerik:RadWindow runat="server" ID="winSelectEmailAttachments" VisibleOnPageLoad="false" Title="Select Attachments" NavigateUrl="EmailAttachments.aspx"
                    Width="650px" Height="400" OnClientClose="OnClientEmailAttachmentsClose" Behaviors="Move" Modal="true">
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>
    </div>
</asp:Content>

