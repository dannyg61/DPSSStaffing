<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="1546.aspx.vb" Inherits="_1546" %>
<%@ MasterType VirtualPath="~/MasterPage.Master" %>
<%@ Register Namespace="CustomEditors" TagPrefix="custom" %>
<%@ Register Namespace="CustomEditors2" TagPrefix="custom2" %>
<%--<%@ Register TagPrefix="uc" TagName="Email" Src="~/Email.ascx" %>--%>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
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

                //function __doApprovalPostBack(sender, eventArgs) {
                //    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                //        theForm.__EVENTTARGET.value = 'rdpToHR';
                //        theForm.__EVENTARGUMENT.value = eventArgs._newValue;
                //        theForm.submit();
                //    }
                //}

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



                <%--function onTextEmailAttachmentsLoad(sender) {
                    window.textEmailAttachments = sender;
                    window.myHidden = document.getElementById('<%= hdnAttachments.ClientID %>');
                    //window.textEmailAttachments.value = window.myHidden.value;
                }--%>

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

                    //function PositionTitleIndexChanged(sender, eventArgs) {
                    //    sender.get_element().title = eventArgs.get_item().get_text();
                    //}
            </script>
        </telerik:RadCodeBlock>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <ClientEvents OnRequestStart="conditionalPostback" />
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="rgd1546">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rgd1546" LoadingPanelID="RadAjaxLoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="lblRecordCount" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
               <%-- <telerik:AjaxSetting AjaxControlID="rcbPositionTitle">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rcbPositionTitle" />
                    </UpdatedControls>
                </telerik:AjaxSetting>--%>
           </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>
        <%--<asp:HiddenField ID="hdnAttachments" runat="server" ClientIDMode="Static" />--%>
        <table style="width: 100%;">
            <tr>
                <td>
                    <asp:Panel ID="pnlSearch" runat="server" GroupingText="Search">
                        <table class="MainTable">
                            <tr>
                                <td style="width: 50%; text-align: left; text-wrap: none; vertical-align: text-top">
                                    <telerik:RadFilter ID="rf1546" runat="server" OnFieldEditorCreating="rf1546_FieldEditorCreating" Width="50%">
                                        <ClientSettings>
                                            <ClientEvents OnFilterCreated="FilterCreated" />
                                        </ClientSettings>
                                        <FieldEditors>
                                            <telerik:RadFilterTextFieldEditor FieldName="Position Number" />
                                            <telerik:RadFilterTextFieldEditor FieldName="Job ID" />
                                            <custom:RadCustomFilterDropDownEditor DataTextField="DataValue" DataValueField="ID" FieldName="Position Title" />
                                            <custom:RadCustomFilterDropDownEditor DataTextField="DataValue" DataValueField="ID" FieldName="Office" />
                                            <custom:RadCustomFilterDropDownEditor DataTextField="DataValue" DataValueField="ID" FieldName="Program" />
                                            <custom:RadCustomFilterDropDownEditor DataTextField="DataValue" DataValueField="ID" FieldName="Position Status" />
                                        </FieldEditors>
                                    </telerik:RadFilter>
                                </td>
                                <td style="width: 50%; text-align: right; text-wrap: none; vertical-align: text-top">
                                    &nbsp;
                                    <asp:ImageButton ID="btnExport" runat="server" ImageUrl="images/Excel_XLSX.PNG" OnClick="btnExport_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: left; text-wrap: none; vertical-align: text-top">
                                    <asp:Label ID="lblRecordCount" runat="server" Text="" CssClass="StandardLabel"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        <tr>
            <td>
                <telerik:RadGrid ID="rgd1546" runat="server" AllowPaging="True" AllowSorting="True" DataSourceID="objDS1546" Width="100%" HorizontalAlign="Center" PageSize="5" AutoGenerateColumns="False" AllowFilteringByColumn="True">
                    <ValidationSettings CommandsToValidate="PerformInsert,Update" ValidationGroup="Req1546" />
                    <MasterTableView DataSourceID="objDS1546" CommandItemDisplay="Top" AllowAutomaticInserts="true" 
                         AllowAutomaticUpdates="true" AllowAutomaticDeletes="true" IsFilterItemExpanded="True" DataKeyNames="ID" InsertItemPageIndexAction="ShowItemOnFirstPage" PageSize="10">
                        <PagerStyle AlwaysVisible="true" />
                        <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column">
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </RowIndicatorColumn>

                        <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column" Created="True">
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </ExpandCollapseColumn>

                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" FilterControlAltText="Filter EditCommandColumn column">
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn DataField="ID" DataType="System.Int32" FilterControlAltText="Filter ID column" UniqueName="ID" Visible="False">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="RequestDate" DataFormatString="{0:d}" FilterControlAltText="Filter RequestDate column" HeaderText="Request Date" UniqueName="RequestDate" DataType="System.DateTime" AllowFiltering="false" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridCheckBoxColumn DataField="Bilingual" DataType="System.Boolean" FilterControlAltText="Filter Bilingual column" HeaderText="Bilingual" UniqueName="Bilingual" AllowFiltering="False" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
                            </telerik:GridCheckBoxColumn>
                            <telerik:GridBoundColumn DataField="PositionNumber" HeaderText="Position Number" UniqueName="PositionNumber" FilterControlAltText="Filter PositionNumber column" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="JobID" HeaderText="Job ID" UniqueName="JobID" FilterControlAltText="Filter JobID column" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PositionTitle" FilterControlAltText="Filter PositionTitle column" HeaderText="Position Title" UniqueName="PositionTitle" AllowFiltering="False" HeaderStyle-CssClass="RadGridColumn75" ItemStyle-CssClass="RadGridColumn75" ItemStyle-VerticalAlign="Top">
                                <HeaderStyle CssClass="RadGridColumn75"></HeaderStyle>
                                <ItemStyle VerticalAlign="Top" CssClass="RadGridColumn75"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PositionStatus" FilterControlAltText="Filter PositionStatus column" HeaderText="Position Status" UniqueName="PositionStatus" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="StatusDate" DataFormatString="{0:d}" FilterControlAltText="Filter RequestDate column" HeaderText="Last Update" UniqueName="StatusDate" DataType="System.DateTime" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="DateToExecs" DataFormatString="{0:d}" FilterControlAltText="Filter RequestDate column" HeaderText="Date Sent to Execs" UniqueName="DateToExecs" DataType="System.DateTime" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="DateToHR" DataFormatString="{0:d}" FilterControlAltText="Filter RequestDate column" HeaderText="Date Sent to HR" UniqueName="DateToHR" DataType="System.DateTime" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Location" FilterControlAltText="Filter Location column" HeaderText="Office" UniqueName="Location" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Program" FilterControlAltText="Filter Program column" HeaderText="Program" UniqueName="Program" AllowFiltering="False" ItemStyle-VerticalAlign="Top">
                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="1st Line Sup/&lt;br /&gt;2nd Line Sup" UniqueName="Supervisor">
                                <ItemTemplate>
                                    <%# Eval("FirstLineSupervisorName")%>
                                    <br />
                                    <hr />
                                    <%# Eval("SecondLineSupervisorName")%>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle VerticalAlign="Top" Wrap="true"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="ContactNumberFormatted" FilterControlAltText="Filter ContactNumber column" UniqueName="ContactNumber" AllowFiltering="False" HeaderText="Contact Number" ItemStyle-VerticalAlign="Top">
                                <HeaderStyle CssClass="RadGridColumn75"></HeaderStyle>
                                <ItemStyle VerticalAlign="Top" CssClass="RadGridColumn75"></ItemStyle>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FirstLineSupervisorEmplID" FilterControlAltText="Filter FirstLineSupervisorEmplID column" UniqueName="FirstLineSupervisorEmplID" AllowFiltering="False" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FirstLineSupervisorName" FilterControlAltText="Filter FirstLineSupervisorName column" UniqueName="FirstLineSupervisorName" Display="false" AllowFiltering="False">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SecondLineSupervisorName" FilterControlAltText="Filter SecondLineSupervisorName column" UniqueName="SecondLineSupervisorName" Display="False" AllowFiltering="False">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SecondLineSupervisorEmplID" FilterControlAltText="Filter SecondLineSupervisorEmplID column" UniqueName="SecondLineSupervisorEmplID" AllowFiltering="False" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpPosition" FilterControlAltText="Filter tlkpPosition column" UniqueName="tlkpPosition" Visible="True" AllowFiltering="False" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpStatus" FilterControlAltText="Filter tlkpStatus column" UniqueName="tlkpStatus" Visible="True" Display="false" AllowFiltering="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpLocation" FilterControlAltText="Filter tlkpLocation column" UniqueName="tlkpLocation" Visible="True" Display="false" AllowFiltering="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpProgram" FilterControlAltText="Filter tlkpProgram column" UniqueName="tlkpProgram" Visible="True" Display="false" AllowFiltering="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn FilterControlAltText="Filter Comments column" HeaderText="Comments" UniqueName="Comments" ItemStyle-VerticalAlign="Top" AllowFiltering="false">
                                <ItemTemplate>
                                    <telerik:RadTextBox ID="txtComments" runat="server" Text='<%# Eval("Comments") %>' TextMode="MultiLine" BorderStyle="None" Height="75px"></telerik:RadTextBox>
                                </ItemTemplate>

                                <ItemStyle VerticalAlign="Top"></ItemStyle>
                            </telerik:GridTemplateColumn>
                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" ConfirmText="Are you sure you want to delete this 1546?" FilterControlAltText="Filter Delete1546 column" ImageUrl="~/images/RadDelete.gif" UniqueName="Delete1546Button">
                            </telerik:GridButtonColumn>
                            <telerik:GridBoundColumn DataField="ProtectStatus" FilterControlAltText="Filter ProtectStatus column" UniqueName="ProtectStatus" DataType="System.Boolean" Display="False">
                            </telerik:GridBoundColumn>
                        </Columns>

                        <EditFormSettings EditFormType="Template">
                            <EditColumn UniqueName="EditCommandColumn1" FilterControlAltText="Filter EditCommandColumn1 column"></EditColumn>
                            <FormTemplate>
                            <table class="TableCellspacing0">
                                <tr>
                                    <td style="width: 625px; text-align: left; vertical-align: text-top">
                                         <table class="TableCellspacing0">
                                             <tr>
                                                 <td style="width: 14%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                     <asp:Label ID="Label1" runat="server" Text="Request Date:"></asp:Label>
                                                 </td>
                                                 <td style="width: 31%; text-align: left; text-wrap: none; vertical-align: text-top">
                                                     <telerik:RadDatePicker ID="rdpRequestDate" runat="server" DbSelectedDate='<%# Bind("RequestDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                     <asp:RequiredFieldValidator ID="reqvalRequestDate" runat="server" ErrorMessage="Request date required." CssClass="ValidatorRed" ControlToValidate="rdpRequestDate" ValidationGroup="Req1546">*</asp:RequiredFieldValidator>
                                                 </td>
                                                 <td style="width: 20%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                     <asp:Label ID="Label2" runat="server" Text="Bilingual:"></asp:Label>
                                                 </td>
                                                 <td style="width: 35%; text-align: left; text-wrap: none; vertical-align: text-top">
                                                     <asp:CheckBox ID="chkBilingual" runat="server" Checked='<%# Bind("Bilingual")%>' />
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label16" runat="server" Text="Job ID:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadTextBox ID="txtJobID" runat="server" MaxLength="20" Text='<%# Bind("JobID") %>' Width="160px"></telerik:RadTextBox>
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label3" runat="server" Text="Position No:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadTextBox ID="txtPositionNumber" runat="server" MaxLength="8" Text='<%# Bind("PositionNumber") %>' Width="160px"></telerik:RadTextBox>
                                                     <asp:CustomValidator ID="valPositionNumber" runat="server" ErrorMessage="Active request already exists."
                                                         OnServerValidate="valPositionNumber_ServerValidate" CssClass="ValidatorRed" ValidationGroup="Req1546" ValidateEmptyText="True" ControlToValidate="txtPositionNumber">*</asp:CustomValidator>
                                                 </td>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label15" runat="server" Text="Date Sent To Execs:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadDatePicker ID="rdpToExecs" runat="server" DbSelectedDate='<%# Bind("DateToExecs")%>' Calendar-ShowRowHeaders="false" OnSelectedDateChanged="rdpToExecs_SelectedDateChanged" AutoPostBack="true"></telerik:RadDatePicker>
                                                 </td>
                                             </tr>
                                            <tr>
                                                <td style="text-align: right; vertical-align: text-top">
                                                    <asp:Label ID="Label4" runat="server" Text="Position Title:"></asp:Label>
                                                </td>
                                                <td style="text-align: left; vertical-align: text-top">
                                                        <telerik:RadComboBox ID="rcbPositionTitle" runat="server" DataSourceID="objDSVacantPositions" DataTextField="DataValue" AutoPostBack="true"
                                                        DataValueField="ID" SelectedValue='<%# Bind("tlkpPosition") %>' NoWrap="false" Height="200px" OnSelectedIndexChanged="rcbPositionTitle_SelectedIndexChanged">
                                                    </telerik:RadComboBox>
                                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Position title required." CssClass="ValidatorRed" ControlToValidate="rcbPositionTitle" ValidationGroup="Req1546">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="text-align: right; vertical-align: text-top">
                                                    <asp:Label ID="Label6" runat="server" Text="Date Sent To HR:"></asp:Label>
                                                </td>
                                                <td style="text-align: left; vertical-align: text-top">
                                                    <telerik:RadDatePicker ID="rdpToHR" runat="server" DbSelectedDate='<%# Bind("DateToHR")%>' Calendar-ShowRowHeaders="false" AutoPostBack="True" 
                                                        OnSelectedDateChanged="rdpToHR_SelectedDateChanged" Enabled='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, False, True)%>'></telerik:RadDatePicker>
                                                </td
                                            </tr>
                                             <tr>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label10" runat="server" Text="Program:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadComboBox ID="rcbProgram" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpProgram")%>'></telerik:RadComboBox>
                                                 </td>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label17" runat="server" Text="First Line Super:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadComboBox ID="rcbFirstLineSupervisor" runat="server" Width="160px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                         DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                         ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged" AllowCustomText="True">
                                                     </telerik:RadComboBox>
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label9" runat="server" Text="Office:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadComboBox ID="rcbLocation" runat="server" DataSourceID="objDSLocation" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpLocation")%>' Height="200px"></telerik:RadComboBox>
                                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Office required." CssClass="ValidatorRed" ControlToValidate="rcbLocation" ValidationGroup="Req1546">*</asp:RequiredFieldValidator>
                                                 </td>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label14" runat="server" Text="Number:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadMaskedTextBox ID="txtPhoneNumber" runat="server" Mask="(###) ###-####" Width="100px" Text='<%# Bind("ContactNumber")%>'></telerik:RadMaskedTextBox>
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label7" runat="server" Text="Position Status:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadComboBox ID="rcbStatus" runat="server" DataSourceID="objDSPositionStatus" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpStatus")%>'></telerik:RadComboBox>
                                                 </td>
                                                 <td style="text-align: right; vertical-align: text-top">
                                                     <asp:Label ID="Label12" runat="server" Text="Second Line Super:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top">
                                                     <telerik:RadComboBox ID="rcbSecondLineSupervisor" runat="server" Width="160px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                         DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                         ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged" AllowCustomText="True">
                                                     </telerik:RadComboBox>
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td style="text-align: right">
                                                     <asp:Label ID="Label13" runat="server" Text="Comments:"></asp:Label>
                                                 </td>
                                                 <td style="text-align: left; vertical-align: text-top" colspan="3">
                                                     <telerik:RadTextBox ID="txtComments" runat="server" LabelWidth="0px" Text='<%# Bind("Comments") %>' TextMode="MultiLine" MaxLength="500" Height="63px" Width="470px"></telerik:RadTextBox>
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td colspan="4">&nbsp;
                                                 </td>
                                             </tr>
                                                <tr>
                                                    <td style="text-align: center" colspan="4">
                                                        <asp:ImageButton ID="btnUpdate" runat="server" CommandName='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "PerformInsert", "Update") %>' ImageUrl='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "~/images/btnAdd.gif", "~/images/btnUpdate.gif") %>' ToolTip="Add" CausesValidation="true" ValidationGroup="Req1546" />
                                                        <asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ImageUrl="~/Images/btnCancel.gif" ToolTip="Cancel" CausesValidation="false" />
                                                    </td>
                                                </tr
                                <tr>
                                                <tr>
                                                    <td colspan="4" style="text-align: center">
                                                        <asp:Label ID="lblError" runat="server" ForeColor="Red" Text=""></asp:Label>
                                                        <telerik:RadTextBox ID="txtProtectStatus" runat="server" Text='<%# Eval("ProtectStatus")%>' Visible="false"></telerik:RadTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4" style="text-align: center">
                                                        <asp:CheckBox ID="chkSendEmail" runat="server" Text="Send Email" Checked="true" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <telerik:RadTextBox ID="txtFirstLineSupervisorEmplID" runat="server" Text='<%# Bind("FirstLineSupervisorEmplID")%>' Visible="false">
                                                        </telerik:RadTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <telerik:RadTextBox ID="txtSecondLineSupervisorEmplID" runat="server" Text='<%# Bind("SecondLineSupervisorEmplID")%>' Visible="false">
                                                        </telerik:RadTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="ValidatorRed" ValidationGroup="Req1546" />
                                                    </td>
                                                </tr>
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
                                                <asp:Label ID="Label5" runat="server" Text="To:"></asp:Label>
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
                                                <asp:Label ID="Label8" runat="server" Text="CC:"></asp:Label>
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
                                    <asp:Label ID="Label11" runat="server" Text="Subject:"></asp:Label>
                                    <telerik:RadTextBox ID="txtSubject" runat="server" EnableEmbeddedSkins="false"
                                        Text="RSVD 1546 Request" EnableViewState="false" Width="481px">
                                    </telerik:RadTextBox>
                                </div>
                                <div class="qsf-fb-group qsf-fb-attachments">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="images/qsf-demo-attachment-bg.png" />
                                   <asp:Button ID="btnSelectAttachments" runat="server" Text="Select Attachments" OnClientClick="return OpenAttachmentSelectionWindow()" CausesValidation="False" UseSubmitBehavior="False" />  
                                    <telerik:RadTextBox ID="txtEmailAttachments" runat="server" TextMode="MultiLine" ClientIDMode="Static" Text="" Height="68px" Width="342px"></telerik:RadTextBox>
                                    <%--ClientEvents-OnLoad="onTextEmailAttachmentsLoad"--%>         
                                </div>
                                <div>

                                    <telerik:RadEditor ID="MailEditor" runat="server" Width="100%" Height="150px" EditModes="All" EnableResize="False">
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
                        </EditFormSettings>

                        <PagerStyle PageSizeControlType="RadComboBox" AlwaysVisible="True"></PagerStyle>

                        <DetailTables>
                            <telerik:GridTableView runat="server" AllowAutomaticDeletes="True" Width="100%"
                                AutoGenerateColumns="False" CommandItemDisplay="Top" DataSourceID="obj1546Documents" ShowHeadersWhenNoRecords="true" DataKeyNames="ID" AllowFilteringByColumn="False" AllowSorting="False" NoDetailRecordsText="No documents attached.">
                                <ParentTableRelation>
                                    <telerik:GridRelationFields DetailKeyField="Request1546ID" MasterKeyField="ID" />
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
                                    <telerik:GridBoundColumn DataField="Request1546ID" DataType="System.Int32" FilterControlAltText="Filter Request1546ID column" HeaderText="1546 Req ID" UniqueName="TransferRequestID" Visible="false" ReadOnly="true">
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
                    <PagerStyle PageSizeControlType="RadComboBox" AlwaysVisible="True"></PagerStyle>

                    <FilterMenu EnableImageSprites="False"></FilterMenu>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
        <asp:ObjectDataSource ID="objDS1546" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Get1546s" TypeName="Req1546DAL.DAL1546" InsertMethod="Insert1546" UpdateMethod="Update1546" DeleteMethod="Delete1546">
            <DeleteParameters>
                <asp:Parameter Name="Original_ID" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="RequestDate" Type="DateTime" />
                <asp:Parameter Name="Bilingual" Type="Boolean" />
                <asp:Parameter Name="PositionNumber" Type="String" />
                <asp:Parameter Name="tlkpPosition" Type="Int32" />
                <asp:Parameter Name="tlkpStatus" Type="Int32" />
                <asp:Parameter Name="DateToExecs" Type="DateTime" />
                <asp:Parameter Name="DateToHR" Type="DateTime" />
                <asp:Parameter Name="tlkpLocation" Type="Int32" />
                <asp:Parameter Name="tlkpProgram" Type="Int32" />
                <asp:Parameter Name="FirstLineSupervisorEmplID" Type="String" />
                <asp:Parameter Name="SecondLineSupervisorEmplID" Type="String" />
                <asp:Parameter Name="Comments" Type="String" />
                <asp:Parameter Name="ContactTypeIDs" Type="Object" />
                <asp:Parameter Name="ContactNumber" Type="String" />
                <asp:Parameter Name="JobID" Type="String" />
            </InsertParameters>
            <SelectParameters>
                <asp:Parameter Name="VacantPosition" Type="Object" />
                <asp:Parameter Name="Program" Type="Object" />
                <asp:Parameter Name="Office" Type="Object" />
                <asp:Parameter Name="PositionStatus" Type="Object" />
                <asp:Parameter Name="PositionNumber" Type="Object" />
                <asp:Parameter Name="JobID" Type="Object" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="RequestDate" Type="DateTime" />
                <asp:Parameter Name="Bilingual" Type="Boolean" />
                <asp:Parameter Name="PositionNumber" Type="String" />
                <asp:Parameter Name="tlkpPosition" Type="Int32" />
                <asp:Parameter Name="tlkpStatus" Type="Int32" />
                <asp:Parameter Name="DateToExecs" Type="DateTime" />
                <asp:Parameter Name="DateToHR" Type="DateTime" />
                <asp:Parameter Name="tlkpLocation" Type="Int32" />
                <asp:Parameter Name="tlkpProgram" Type="Int32" />
                <asp:Parameter Name="FirstLineSupervisorEmplID" Type="String" />
                <asp:Parameter Name="SecondLineSupervisorEmplID" Type="String" />
                <asp:Parameter Name="Comments" Type="String" />
                <asp:Parameter Name="Original_ID" Type="Int32" />
                <asp:Parameter Name="ContactTypeIDs" Type="Object" />
                <asp:Parameter Name="ContactNumber" Type="String" />
                <asp:Parameter Name="JobID" Type="String" />
            </UpdateParameters>
        </asp:ObjectDataSource>
         <asp:ObjectDataSource ID="objDSVacantPositions" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Vacant Position" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objDSPositionStatus" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Pos Status" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objDSLocation" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Office Locations" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objDSPrograms" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="Program" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objDSContactTypes" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookups" TypeName="StaffingUtilities.Utilities">
            <SelectParameters>
                <asp:Parameter DefaultValue="PhoneType" Name="Category" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="obj1546Documents" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Get1546Documents" TypeName="Req1546DAL.DAL1546" DeleteMethod="Delete1546Document">
            <SelectParameters>
                <asp:Parameter Name="Request1546ID" Type="Int32" />
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

