<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TAP.aspx.vb" Inherits="TAP" %>
<%@ MasterType VirtualPath="~/MasterPage.Master" %>
<%@ Register Namespace="CustomEditors" TagPrefix="custom" %>
<%@ Register Namespace="CustomEditors3" TagPrefix="custom3" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <div>
         <br />
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

                function FilterCreating(sender, eventArgs) {
                    var filterMenu = sender.get_contextMenu();
                    filterMenu.add_showing(FilterMenuShowing);
                }

                // Removing options Or, Not And, and Not Or and only allowing simple AND queries on the RadFilter
                //function FilterCreated(sender, eventArgs) {
                //    var filterMenu = sender.get_contextMenu();
                //    filterMenu.add_showing(FilterMenuShowing);
                //}

                function FilterMenuShowing(sender, eventArgs) {
                    var filter = $find("<%=rfTAP.ClientID %>");
                    var currentExpandedItem = sender.get_attributes()._data.ItemHierarchyIndex;
                    var fieldName = filter._expressionItems[currentExpandedItem];
                    //if (fieldName == "Actual End Date") {
                    //    sender.findItemByValue("Or").set_visible(false);
                    //    sender.findItemByValue("NotAnd").set_visible(false);
                    //    sender.findItemByValue("NotOr").set_visible(false);
                    //    sender.findItemByValue("Contains").set_visible(false);
                    //    sender.findItemByValue("DoesNotContain").set_visible(false);
                    //    sender.findItemByValue("StartsWith").set_visible(false);
                    //    sender.findItemByValue("EndsWith").set_visible(false);
                    //    //sender.findItemByValue("EqualTo").set_visible(false);
                    //    sender.findItemByValue("NotEqualTo").set_visible(false);
                    //    //sender.findItemByValue("GreaterThan").set_visible(false);
                    //    //sender.findItemByValue("LessThan").set_visible(false);
                    //    sender.findItemByValue("GreaterThanOrEqualTo").set_visible(false);
                    //    sender.findItemByValue("LessThanOrEqualTo").set_visible(false);
                    //    sender.findItemByValue("Between").set_visible(false);
                    //    sender.findItemByValue("NotBetween").set_visible(false);
                    //    sender.findItemByValue("IsEmpty").set_visible(false);
                    //    sender.findItemByValue("IsNull").set_visible(false);
                    //    sender.findItemByValue("NotIsNull").set_visible(false);
                    //    sender.findItemByValue("NotIsEmpty").set_visible(false);
                    //}
                    //else {
                        sender.findItemByValue("Or").set_visible(false);
                        sender.findItemByValue("NotAnd").set_visible(false);
                        sender.findItemByValue("NotOr").set_visible(false);
                        sender.findItemByValue("Contains").set_visible(false);
                        sender.findItemByValue("DoesNotContain").set_visible(false);
                        sender.findItemByValue("StartsWith").set_visible(false);
                        sender.findItemByValue("EndsWith").set_visible(false);
                        //sender.findItemByValue("EqualTo").set_visible(false);
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
                    //}
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

                //function PositionTitleIndexChanged(sender, eventArgs) {
                //    sender.get_element().title = eventArgs.get_item().get_text();
                //}
            </script>


        </telerik:RadCodeBlock>
         <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <ClientEvents OnRequestStart="conditionalPostback" />
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="rgdTAP">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rgdTAP" LoadingPanelID="RadAjaxLoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="lblRecordCount" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <%--<telerik:AjaxSetting AjaxControlID="rcbPositionTitle">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rcbPositionTitle" />
                    </UpdatedControls>
                </telerik:AjaxSetting>--%>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>
         <table style="width: 100%;">
            <tr>
                <td>
                    <asp:Panel ID="pnlSearch" runat="server" GroupingText="Search">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width:25%; text-align:left">
                                    <telerik:RadFilter ID="rfTAP" runat="server" OnFieldEditorCreating="rfTAP_FieldEditorCreating" ExpressionPreviewPosition="None"
                                        ShowApplyButton="true" Width="100%">
                                        <ClientSettings>
                                            <ClientEvents OnFilterCreating="FilterCreating" />
                                        </ClientSettings>
                                        <FieldEditors>
                                            <custom:RadCustomFilterDropDownEditor FieldName="Position Title" DataTextField="DataValue" DataValueField="ID" />
                                            <custom:RadCustomFilterDropDownEditor FieldName="Office" DataTextField="DataValue" DataValueField="ID" />
                                            <custom:RadCustomFilterDropDownEditor FieldName="Program" DataTextField="DataValue" DataValueField="ID" />
                                            <%--<custom3:RadFilterDatePickerEditor FieldName="Actual End Date" />--%>
                                        </FieldEditors>
                                    </telerik:RadFilter>
                                    <asp:Label ID="lblRecordCount" runat="server" Text="" CssClass="StandardLabel"></asp:Label>
                                </td>
                                <td style="width: 50%; text-align: left; vertical-align: top">
                                    <table style="width: 99%">
                                        <tr>
                                            <td style="text-align:center">
                                                <asp:Label ID="Label27" runat="server" Text="Search closed requests with an Actual End Date"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:center">
                                                <asp:Label ID="Label25" runat="server" Text="From:"></asp:Label>
                                                <telerik:RadDatePicker ID="rdpBeginActualEndDate" runat="server"></telerik:RadDatePicker>
                                                <asp:Label ID="Label26" runat="server" Text="To:"></asp:Label>
                                                <telerik:RadDatePicker ID="rdpEndActualEndDate" runat="server"></telerik:RadDatePicker>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 25%; text-align:right; vertical-align:top">
                                     &nbsp;
                                    <asp:ImageButton ID="btnExport" runat="server" ImageUrl="images/Excel_XLSX.PNG" OnClick="btnExport_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>                
            </tr>
             <tr>
                 <td>
                     <telerik:RadGrid ID="rgdTAP" runat="server" AllowPaging="True" AllowSorting="True" DataSourceID="objTAPs" AutoGenerateColumns="False" Width="100%" 
                         AllowFilteringByColumn="True" HorizontalAlign="Center" PageSize="5" GroupingSettings-CaseSensitive="false">
                         <GroupingSettings CaseSensitive="False"></GroupingSettings>
                          <ValidationSettings CommandsToValidate="PerformInsert,Update" ValidationGroup="ReqTAP" />
                         <MasterTableView DataSourceID="objTAPs" CommandItemDisplay="Top" AllowAutomaticInserts="true" 
                         AllowAutomaticUpdates="true" AllowAutomaticDeletes="true" IsFilterItemExpanded="True" InsertItemPageIndexAction="ShowItemOnFirstPage" PageSize="10" DataKeyNames="ID">
                             
                             <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                             <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                                 <HeaderStyle Width="20px"></HeaderStyle>
                             </RowIndicatorColumn>

                             <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                                 <HeaderStyle Width="20px"></HeaderStyle>
                             </ExpandCollapseColumn>

                             <Columns>
                                 <%--<telerik:GridTemplateColumn FilterControlAltText="Filter Extension column" UniqueName="Extension" HeaderText="Ext Approval/&lt;br /&gt;Ext Start Dt/&lt;br /&gt;Actual End Dt" AllowFiltering="false">
                                     <HeaderStyle Wrap="False" />
                                     <ItemStyle VerticalAlign="Top" />
                                     <ItemTemplate>
                                         <%# Eval("ExtApproval")%>
                                         <br />
                                         <%# Eval("ExtStartDate", "{0:d}")%>
                                         <br />
                                         <%# Eval("ActualEndDate", "{0:d}")%>
                                     </ItemTemplate>
                                      <ItemStyle VerticalAlign="Top" />
                                 </telerik:GridTemplateColumn>--%>
                                 
                                 <telerik:GridEditCommandColumn ButtonType="ImageButton" FilterControlAltText="Filter EditCommandColumn column">
                                 </telerik:GridEditCommandColumn>
                                 <telerik:GridBoundColumn DataField="ID" DataType="System.Int32" FilterControlAltText="Filter ID column" UniqueName="ID" Visible="True" HeaderText="Request #">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter Request column" HeaderText="Request Date/&lt;br /&gt;Bilingual" UniqueName="Request" AllowFiltering="False">
                                     <ItemTemplate>
                                         <%# Eval("RequestDate", "{0:d}")%>
                                         <br />
                                         <hr />
                                         <asp:CheckBox ID="cbBilingual" runat="server" Enabled="false" Checked='<%# Eval("Bilingual") %>' />
                                     </ItemTemplate>
                                     <ItemStyle VerticalAlign="Top" />
                                     <HeaderStyle Wrap="False" />
                                 </telerik:GridTemplateColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter PositionAndTitle column" HeaderText="Position/&lt;br /&gt;Reason" UniqueName="PositionAndTitle" AllowFiltering="False">
                                     <ItemTemplate>
                                         <%# Eval("PositionTitle")%>
                                         <br />
                                         <hr />
                                         <%# Eval("RequestReason")%>
                                     </ItemTemplate>
                                     <ItemStyle VerticalAlign="Top" />
                                     <HeaderStyle Wrap="False" />
                                 </telerik:GridTemplateColumn>
                                  <telerik:GridBoundColumn DataField="PositionTitle" DataType="System.String" Display="False" FilterControlAltText="Filter PositionTitle column" UniqueName="PositionTitle" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="tlkpPosition" DataType="System.Int32" Display="False" FilterControlAltText="Filter tlkpPosition column" UniqueName="tlkpPosition" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="tlkpRequestReason" DataType="System.Int32" Display="False" FilterControlAltText="Filter tlkpRequestReason column" UniqueName="tlkpRequestReason" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="tlkpLocation" DataType="System.Int32" Display="False" FilterControlAltText="Filter tlkpLocation column" UniqueName="tlkpLocation" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="tlkpProgram" DataType="System.Int32" Display="False" FilterControlAltText="Filter tlkpProgram column" UniqueName="tlkpProgram" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter LocationProgram column" HeaderText="Location/&lt;br /&gt;Program" UniqueName="LocationProgram" AllowFiltering="False">
                                     <ItemTemplate>
                                         <%# Eval("Location")%>
                                         <br />
                                         <hr />
                                         <%# Eval("Program")%>
                                     </ItemTemplate>
                                      <ItemStyle VerticalAlign="Top" />
                                     <HeaderStyle Wrap="False" />
                                 </telerik:GridTemplateColumn>
                                 <telerik:GridBoundColumn DataField="FirstLineSupervisorEmplID" Display="False" FilterControlAltText="Filter FirstLineSupervisorEmplID column" UniqueName="FirstLineSupervisorEmplID" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="SecondLineSupervisorEmplID" Display="False" FilterControlAltText="Filter SecondLineSupervisorEmplID column" UniqueName="SecondLineSupervisorEmplID" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="FirstLineSupervisorName" FilterControlAltText="Filter FirstLineSupervisorName column" UniqueName="FirstLineSupervisorName" Display="false" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="SecondLineSupervisorName" FilterControlAltText="Filter SecondLineSupervisorName column" UniqueName="SecondLineSupervisorName" Display="False" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter SupervisorName column" HeaderText="1st Line Sup/&lt;br /&gt;2nd Line Sup" UniqueName="SupervisorName" AllowFiltering="False">
                                     <ItemTemplate>
                                         <%# Eval("FirstLineSupervisorName")%>
                                         <br />
                                         <hr />
                                         <%# Eval("SecondLineSupervisorName")%>
                                     </ItemTemplate>
                                     <ItemStyle VerticalAlign="Top" />
                                     <HeaderStyle Wrap="False" />
                                 </telerik:GridTemplateColumn>
                                 <telerik:GridBoundColumn DataField="ContactNumberFormatted" FilterControlAltText="Filter ContactNumberFormatted column" UniqueName="ContactNumber" AllowFiltering="False" HeaderText="Contact Number" ItemStyle-VerticalAlign="Top">
                                    <ItemStyle VerticalAlign="Top"></ItemStyle>
                                 </telerik:GridBoundColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter RequestSentDate column" UniqueName="RequestSentDate" HeaderText="Dt To Execs/&lt;br /&gt;Dt To HR" AllowFiltering="false">
                                     <HeaderStyle Wrap="False" />
                                     <ItemStyle VerticalAlign="Top" />
                                     <ItemTemplate>
                                         <%# Eval("DateToExecs", "{0:d}")%>
                                         <br />
                                         <%# Eval("DateToHR", "{0:d}")%>
                                     </ItemTemplate>
                                     <ItemStyle VerticalAlign="Top" />
                                 </telerik:GridTemplateColumn>
                                 <telerik:GridBoundColumn DataField="FirstName" Display="False" FilterControlAltText="Filter FirstName column" UniqueName="FirstName" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn DataField="LastName" Display="False" FilterControlAltText="Filter LastName column" UniqueName="LastName" AllowFiltering="false">
                                 </telerik:GridBoundColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter RequestStartAndEndDate column" UniqueName="RequestStartAndEndDate" HeaderText="Start Dt/&lt;br /&gt;Exp End Dt/&lt;br /&gt;Actual End Dt" AllowFiltering="false">
                                     <HeaderStyle Wrap="False" />
                                     <ItemStyle VerticalAlign="Top" />
                                     <ItemTemplate>                                         
                                         <%# Eval("StartDate", "{0:d}")%>
                                         <br />
                                         <%# Eval("AntcpEndDate", "{0:d}")%>
                                         <br />
                                         <%# Eval("ActualEndDate", "{0:d}")%>
                                     </ItemTemplate>
                                      <ItemStyle VerticalAlign="Top" />
                                 </telerik:GridTemplateColumn>
                                 
                                 <%--<telerik:GridTemplateColumn FilterControlAltText="Filter Extension column" UniqueName="Name1" HeaderText="Name" AllowFiltering="False">
                                     <HeaderStyle Wrap="False" />
                                     <ItemStyle VerticalAlign="Top" />
                                     <ItemTemplate>
                                         <%# Eval("Name")%>
                                     </ItemTemplate>
                                      <ItemStyle VerticalAlign="Top" />
                                 </telerik:GridTemplateColumn>--%>
                                 <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Filter Name2 column" HeaderText="Name" UniqueName="Name2" AllowFiltering="true" ItemStyle-VerticalAlign="Top">
                                        <ItemStyle VerticalAlign="Top"></ItemStyle>
                                 </telerik:GridBoundColumn>
                                 <telerik:GridTemplateColumn FilterControlAltText="Filter Comments column" HeaderText="Comments" UniqueName="Comments" ItemStyle-VerticalAlign="Top" AllowFiltering="false" ReadOnly="true">
                                     <ItemTemplate>
                                         <telerik:RadTextBox ID="txtComments" runat="server" Text='<%# Eval("Comments") %>' TextMode="MultiLine" BorderStyle="None" Height="75px"></telerik:RadTextBox>
                                     </ItemTemplate>
                                     <ItemStyle VerticalAlign="Top"></ItemStyle>
                                 </telerik:GridTemplateColumn>
                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" ConfirmText="Are you sure you want to delete this TAP request?" FilterControlAltText="Filter DeleteTAP column" ImageUrl="~/images/RadDelete.gif" UniqueName="DeleteTAPButton">
                            </telerik:GridButtonColumn>
                             </Columns>

                             <EditFormSettings EditFormType="Template">
                                 <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                                 <FormTemplate>
                                    <table class="TableCellspacing0" style="width: 100%">
                                        <tr>
                                            <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                <%-- Data entry table on left side of edit form  --%>
                                                 <table class="TableCellspacing0">
                                                     <tr>
                                                         <td style="width: 20%; text-align: right; text-wrap: none" vertical-align: text-top">
                                                             <asp:Label ID="Label1" runat="server" Text="Request Date:"></asp:Label>
                                                         </td>
                                                         <td style="width: 30%; text-align: left; vertical-align: text-top">
                                                             <telerik:RadDatePicker ID="rdpRequestDate" runat="server" DbSelectedDate='<%# Bind("RequestDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                             <asp:RequiredFieldValidator ID="reqvalRequestDate" runat="server" ErrorMessage="Request date required." CssClass="ValidatorRed" ControlToValidate="rdpRequestDate" ValidationGroup="ReqTAP">*</asp:RequiredFieldValidator>
                                                         </td>
                                                         <td style="width: 22%; text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label2" runat="server" Text="Bilingual:"></asp:Label>
                                                         </td>
                                                         <td style="width: 28%; text-align: left; vertical-align: text-top">
                                                             <asp:CheckBox ID="chkBilingual" runat="server" Checked='<%# Bind("Bilingual")%>' />
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label23" runat="server" Text="Request #:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                             <telerik:RadTextBox ID="txtRequestNumber" runat="server" Text='<%# Eval("ID")%>' ReadOnly="True"></telerik:RadTextBox>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label7" runat="server" Text="Date Sent To Execs:"></asp:Label>
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
                                                             <%--<telerik:RadComboBox ID="rcbPositionTitle" runat="server" DataSourceID="objDSVacantPositions" DataTextField="DataValue" DataValueField="ID" 
                                                                 SelectedValue='<%# Bind("tlkpPosition") %>' Height="200px" NoWrap="false" OnClientSelectedIndexChanged="PositionTitleIndexChanged"></telerik:RadComboBox>--%>
                                                             <telerik:RadComboBox ID="rcbPositionTitle" runat="server" DataSourceID="objDSVacantPositions" DataTextField="DataValue" DataValueField="ID"  AutoPostBack="true"
                                                                 SelectedValue='<%# Bind("tlkpPosition") %>' Height="200px" NoWrap="false" OnSelectedIndexChanged="rcbPositionTitle_SelectedIndexChanged"></telerik:RadComboBox>
                                                             <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Position title required." CssClass="ValidatorRed" ControlToValidate="rcbPositionTitle" ValidationGroup="ReqTAP">*</asp:RequiredFieldValidator>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label8" runat="server" Text="Date Sent To HR:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                             <telerik:RadDatePicker ID="rdpToHR" runat="server" DbSelectedDate='<%# Bind("DateToHR")%>' Calendar-ShowRowHeaders="false" AutoPostBack="True" 
                                                                 OnSelectedDateChanged="rdpToHR_SelectedDateChanged" Enabled='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, False, True)%>'></telerik:RadDatePicker>
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label3" runat="server" Text="Reason:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                            <telerik:RadComboBox ID="rcbRequestReason" runat="server" DataSourceID="objRequestReasons" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpRequestReason")%>'></telerik:RadComboBox>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label17" runat="server" Text="First Line Super:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                             <telerik:RadComboBox ID="rcbFirstLineSupervisor" runat="server" Width="155px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                 DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                 ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged" AllowCustomText="True">
                                                             </telerik:RadComboBox>
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label6" runat="server" Text="Program:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                              <telerik:RadComboBox ID="rcbProgram" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpProgram")%>'></telerik:RadComboBox>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label11" runat="server" Text="Contact Number:"></asp:Label>
                                                         </td>                                            
                                                         <td style="text-align: left; vertical-align: text-top">
                                                             <telerik:RadMaskedTextBox ID="txtPhoneNumber" runat="server" Mask="(###) ###-####" Width="100px" Text='<%# Bind("ContactNumber")%>'></telerik:RadMaskedTextBox>
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label5" runat="server" Text="Office:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                              <telerik:RadComboBox ID="rcbLocation" runat="server" DataSourceID="objDSLocation" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpLocation")%>' Height="200px"></telerik:RadComboBox>
                                                             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Office required." CssClass="ValidatorRed" ControlToValidate="rcbLocation" ValidationGroup="ReqTAP">*</asp:RequiredFieldValidator>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: text-top">
                                                             <asp:Label ID="Label12" runat="server" Text="Second Line Super:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top">
                                                             <telerik:RadComboBox ID="rcbSecondLineSupervisor" runat="server" Width="155px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                 DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                 ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmployees_SelectedIndexChanged" AllowCustomText="True">
                                                             </telerik:RadComboBox>
                                                        </td>
                                                     </tr>
                                                     <%-- Start Name Panel --%>
                                                     <tr>
                                                         <td colspan="4">
                                                             <asp:panel runat="server" ID="pnlNameInfo" BackColor="#CCCCCC">
                                                             <table class="TableCellspacing0">
                                                                 <tr>
                                                                     <td style="width: 20%; text-align: right; text-wrap: none">
                                                                         <asp:Label ID="Label9" runat="server" Text="First Name:"></asp:Label>
                                                                     </td>
                                                                     <td style="width: 30%; text-align: left; text-wrap: none">
                                                                         <telerik:RadTextBox ID="txtFirstName" runat="server" Text='<%# Bind("FirstName")%>'></telerik:RadTextBox>
                                                                     </td>
                                                                     <td style="width: 22%; text-align: right; text-wrap: none">
                                                                         <asp:Label ID="Label15" runat="server" Text="StartDate:"></asp:Label>
                                                                     </td>
                                                                     <td style="width: 28%; text-align: left; text-wrap: none">
                                                                         <telerik:RadDatePicker ID="rdpStartDate" runat="server" DbSelectedDate='<%# Bind("StartDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                                     </td>
                                                                 </tr>
                                                                 <tr>
                                                                     <td style="text-align: right; vertical-align: text-top">
                                                                         <asp:Label ID="Label10" runat="server" Text="Last Name:"></asp:Label>
                                                                     </td>
                                                                     <td style="text-align: left; vertical-align: text-top">
                                                                         <telerik:RadTextBox ID="txtLastName" runat="server" Text='<%# Bind("LastName")%>'></telerik:RadTextBox>
                                                                     </td>
                                                                     <td style="text-align: right; vertical-align: text-top">
                                                                         <asp:Label ID="Label24" runat="server" Text="Anticipated End Date:"></asp:Label>
                                                                     </td>
                                                                     <td style="text-align: left; vertical-align: text-top">
                                                                         <telerik:RadDatePicker ID="rdpAntcpEndDate" runat="server" DbSelectedDate='<%# Bind("AntcpEndDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                                     </td>
                                                                 </tr>
                                                                 <tr>
                                                                     <td style="text-align: right; vertical-align: text-top" colspan="3">
                                                                         <asp:Label ID="Label16" runat="server" Text="Actual End Date:"></asp:Label>
                                                                     </td>
                                                                     <td style="text-align: left; vertical-align: text-top">
                                                                         <telerik:RadDatePicker ID="rdpActualEndDate" runat="server" DbSelectedDate='<%# Bind("ActualEndDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                                     </td>
                                                                 </tr>
                                                             </table>
                                                            </asp:panel>
                                                         </td>
                                                     </tr>
                                                     <%-- End name panel --%>
                                                     <tr>
                                                         <td colspan="4">
                                                             <asp:Panel runat="server" ID="pnlExtensions" GroupingText="Extensions" BorderStyle="Solid" BorderWidth="1px">
                                                                   <table class="TableCellspacing0" style="width: 100%">
                                                                       <tr>
                                                                           <td>
                                                                               <telerik:RadGrid ID="rgdExtensions" runat="server" AllowPaging="True" AllowSorting="True" DataSourceID="objDSExtensions" 
                                                                                   AutoGenerateColumns="False" CellSpacing="0" GridLines="None" Width="100%" HorizontalAlign="Center" PageSize="5" 
                                                                                   AllowAutomaticDeletes="True" AllowAutomaticInserts="True" AllowAutomaticUpdates="True" MasterTableView-CommandItemDisplay="Top"
                                                                                    OnItemInserted="rgdExtensions_ItemInserted" OnItemDataBound="rgdExtensions_ItemDataBound" OnItemUpdated="rgdExtensions_ItemUpdated">
                                                                                    <ValidationSettings CommandsToValidate="PerformInsert,Update" ValidationGroup="TAP" />

                                                                                    <MasterTableView AllowFilteringByColumn="True" DataKeyNames="ID,tbl1546ID" DataSourceID="objDSExtensions" EditMode="PopUp">
                                                                                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                                                                                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                                                                            <HeaderStyle Width="20px" />
                                                                                        </RowIndicatorColumn>
                                                                                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                                                                            <HeaderStyle Width="20px" />
                                                                                        </ExpandCollapseColumn>
                                                                                        <Columns>
                                                                                            <telerik:GridEditCommandColumn FilterControlAltText="Filter EditCommandColumn column">
                                                                                            </telerik:GridEditCommandColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="ID" DataType="System.Int32" Display="False" FilterControlAltText="Filter ID column" UniqueName="ID">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="tbl1546ID" DataType="System.Int32" Display="False" FilterControlAltText="Filter tbl1546ID column" UniqueName="tbl1546ID">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="tlkpExtensionApproval" DataType="System.Int32" Display="False" FilterControlAltText="Filter tlkpExtensionApproval column" UniqueName="tlkpExtensionApproval">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="ExtensionApproval" FilterControlAltText="Filter ExtensionApproval column" HeaderText="Extension Status" UniqueName="ExtensionApproval">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="DateToExecs" DataFormatString="{0:d}" DataType="System.DateTime" FilterControlAltText="Filter DateToExecs column" HeaderText="Dt To Execs" UniqueName="DateToExecs">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="DateToHR" DataFormatString="{0:d}" DataType="System.DateTime" FilterControlAltText="Filter DateToHR column" HeaderText="Dt To HR" UniqueName="DateToHR">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="ExtStartDate" DataFormatString="{0:d}" DataType="System.DateTime" FilterControlAltText="Filter ExtStartDate column" HeaderText="Extension Start Date" UniqueName="ExtStartDate">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridBoundColumn AllowFiltering="False" DataField="AntcpEndDate" DataFormatString="{0:d}" DataType="System.DateTime" FilterControlAltText="Filter AntcpEndDate column" HeaderText="Anticipated End Date" UniqueName="AntcpEndDate">
                                                                                            </telerik:GridBoundColumn>
                                                                                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" ConfirmDialogType="RadWindow" ConfirmText="Delete this extension?" ConfirmTitle="Delete" FilterControlAltText="Filter DeleteColumn column" HeaderText="Delete" ImageUrl="~/images/RadDelete.gif" Text="Delete" UniqueName="DeleteColumn">
                                                                                            </telerik:GridButtonColumn>
                                                                                        </Columns>
                                                                                        <EditFormSettings EditFormType="Template" InsertCaption="New Extension Request" CaptionFormatString="Edit Extension Request">
                                                                                            <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                                                                            </EditColumn>
                                                                                            <FormTemplate>
                                                                                                <table class="TableCellspacing0">
                                                                                                    <tr>
                                                                                                        <td style="width: 50%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                                                                             <asp:Label ID="Label18" runat="server" Text="Extension Status:"></asp:Label>
                                                                                                        </td>
                                                                                                         <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                                                                             <telerik:RadComboBox ID="rcbExtensionApproval" runat="server" DataSourceID="objDSExtensionApproval" DataTextField="DataValue" DataValueField="ID" SelectedValue='<%# Bind("tlkpExtensionApproval")%>' Height="200px"></telerik:RadComboBox>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 50%; text-align: right; text-wrap: none" vertical-align: text-top">
                                                                                                            <asp:Label ID="Label19" runat="server" Text="Date To Execs:"></asp:Label>
                                                                                                        </td>
                                                                                                        <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                                                                            <telerik:RadDatePicker ID="rdpDateToExecs" runat="server" DbSelectedDate='<%# Bind("DateToExecs")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 50%; text-align: right; text-wrap: none" vertical-align: text-top">
                                                                                                            <asp:Label ID="Label20" runat="server" Text="Date To HR:"></asp:Label>
                                                                                                        </td>
                                                                                                        <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                                                                            <telerik:RadDatePicker ID="rdpDateToHR" runat="server" DbSelectedDate='<%# Bind("DateToHR")%>' Calendar-ShowRowHeaders="false" 
                                                                                                                OnSelectedDateChanged="rdpDateToHR_SelectedDateChanged" Enabled='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, False, True)%>'></telerik:RadDatePicker>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 50%; text-align: right; text-wrap: none" vertical-align: text-top">
                                                                                                            <asp:Label ID="Label1" runat="server" Text="Extension Start Date:"></asp:Label>
                                                                                                        </td>
                                                                                                        <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                                                                            <telerik:RadDatePicker ID="rdpExtStartDate" runat="server" DbSelectedDate='<%# Bind("ExtStartDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 50%; text-align: right; vertical-align: text-top">
                                                                                                            <asp:Label ID="Label2" runat="server" Text="Anticipated End Date:"></asp:Label>
                                                                                                        </td>
                                                                                                        <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                                                                            <telerik:RadDatePicker ID="rdpAntcpEndDate" runat="server" DbSelectedDate='<%# Bind("AntcpEndDate")%>' Calendar-ShowRowHeaders="false"></telerik:RadDatePicker>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                     <tr>
                                                                                                         <td style="text-align: center" colspan="2">
                                                                                                             <asp:ImageButton ID="btnUpdate" runat="server" CommandName='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "PerformInsert", "Update") %>' ImageUrl='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "~/images/btnAdd.gif", "~/images/btnUpdate.gif") %>' ToolTip="Add" CausesValidation="true" ValidationGroup="ReqTAP" />
                                                                                                             <asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ImageUrl="~/Images/btnCancel.gif" ToolTip="Cancel" CausesValidation="false" />
                                                                                                         </td>
                                                                                                     </tr>
                                                                                                </table>
                                                                                            </FormTemplate>
                                                                                        </EditFormSettings>
                                                                                        <PagerStyle PageSizeControlType="RadComboBox" />
                                                                                    </MasterTableView>
                                                                                    <PagerStyle PageSizeControlType="RadComboBox" />
                                                                                    <FilterMenu EnableImageSprites="False">
                                                                                    </FilterMenu>

                                                                               </telerik:RadGrid>
                                                                           </td>
                                                                       </tr>
                                                                   </table>
                                                              </asp:Panel>
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td style="text-align: right">
                                                             <asp:Label ID="Label13" runat="server" Text="Comments:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: text-top" colspan="3">
                                                             <telerik:RadTextBox ID="txtComments" runat="server" LabelWidth="0px" Text='<%# Bind("Comments") %>' TextMode="MultiLine" MaxLength="500" Height="63px" Width="460px"></telerik:RadTextBox>
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td style="text-align: center" colspan="4">
                                                             <asp:ImageButton ID="btnUpdate" runat="server" CommandName='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "PerformInsert", "Update") %>' ImageUrl='<%# IIf(CType(Container, GridItem).OwnerTableView.IsItemInserted, "~/images/btnAdd.gif", "~/images/btnUpdate.gif") %>' ToolTip="Add" CausesValidation="true" ValidationGroup="ReqTAP" />
                                                             <asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ImageUrl="~/Images/btnCancel.gif" ToolTip="Cancel" CausesValidation="false" />
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td colspan="4" style="text-align: center">
                                                             <asp:Label ID="lblError" runat="server" ForeColor="Red" Text=""></asp:Label>
                                                             
                                                         </td>
                                                     </tr>
                                                     <tr>
                                                         <td colspan="4" style="text-align: center">
                                                             <asp:CheckBox ID="chkSendEmail" runat="server" Text="Send Email" Checked="true" />
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
                                                             <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="ValidatorRed" ValidationGroup="ReqTAP" />
                                                         </td>
                                                     </tr>
                                                 </table>
                                            </td>
                                            <td style="width: 50%; text-align: left; vertical-align: text-top">
                                                <%-- Email table on right side of edit form --%>
                                                <asp:Panel runat="server" ID="pnlEmail" GroupingText="Email" BorderStyle="Solid" BorderWidth="1px">
                                                <table class="TableCellspacing0">
                                                    <tr>
                                                        <td style="width: 15%">
                                                        </td>
                                                        <td style="width: 25%">
                                                        </td>
                                                        <td style="width: 30%">
                                                        </td>
                                                        <td style="width: 30%">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                         <td style="text-align: left; vertical-align: top">
                                                             <telerik:RadComboBox ID="rcbEmailRecipients" runat="server" Width="160px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                 DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                 ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailRecipient_SelectedIndexChanged" AllowCustomText="True">
                                                             </telerik:RadComboBox>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: top">
                                                             <asp:Label ID="Label14" runat="server" Text="To:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: top" rowspan="2">
                                                             <telerik:RadTextBox ID="txtEmailRecipients" runat="server" ClientIDMode="Static"
                                                                 EnableEmbeddedSkins="false" EnableViewState="false" TextMode="MultiLine" Width="400px" Height="50px">
                                                             </telerik:RadTextBox>
                                                         </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                         <td colspan="2" style="text-align: left; vertical-align: top">
                                                             <telerik:RadButton ID="btnAddEmailRecipient" runat="server" Text="Add" OnClick="btnAddEmailRecipient_Click" Width="70px"></telerik:RadButton>
                                                         </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                         <td style="text-align: left; vertical-align: top">
                                                             <telerik:RadComboBox ID="rcbEmailCC" runat="server" Width="160px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                 DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                                 ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailCC_SelectedIndexChanged" AllowCustomText="True">
                                                             </telerik:RadComboBox>
                                                         </td>
                                                         <td style="text-align: right; vertical-align: top">
                                                             <asp:Label ID="Label21" runat="server" Text="CC:"></asp:Label>
                                                         </td>
                                                         <td style="text-align: left; vertical-align: top" rowspan="2">
                                                             <telerik:RadTextBox ID="txtEmailCC" runat="server"
                                                                 EnableEmbeddedSkins="false" EnableViewState="false" TextMode="MultiLine" Width="400px" Height="50px">
                                                             </telerik:RadTextBox>
                                                         </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                         <td colspan="2" style="text-align: left; vertical-align: top">
                                                             <telerik:RadButton ID="btnAddEmailCC" runat="server" Text="Add" OnClick="btnAddEmailCC_Click" Width="70px"></telerik:RadButton>
                                                         </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                         <td colspan="3" style="text-align: left; vertical-align: text-top">
                                                             <asp:Label ID="Label22" runat="server" Text="Subject:"></asp:Label>
                                                             <telerik:RadTextBox ID="txtSubject" runat="server" EnableEmbeddedSkins="false"
                                                                 Text="HR 3318 TAP Job Order" EnableViewState="false" Width="542px">
                                                             </telerik:RadTextBox>
                                                         </td>
                                                    </tr>
                                                    <tr>  
                                                        <td>
                                                            &nbsp;
                                                        </td>                                                      
                                                        <td colspan="2" style="text-align: right; vertical-align: text-top">
                                                             <asp:Image ID="Image1" runat="server" ImageUrl="images/qsf-demo-attachment-bg.png" />
                                                            <asp:Button ID="btnSelectAttachments" runat="server" Text="Attachments" OnClientClick="return OpenAttachmentSelectionWindow()" CausesValidation="False" UseSubmitBehavior="False" />
                                                        </td>
                                                         <td colspan="2" rowspan="3">
                                                            <telerik:RadTextBox ID="txtEmailAttachments" runat="server" TextMode="MultiLine" ClientIDMode="Static" Text="" Height="68px" Width="400px"></telerik:RadTextBox>
                                                         </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                         <td style="text-align: left; vertical-align: top" colspan="3" rowspan="3">
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
                                                         </td>
                                                    </tr>
                                                </table>
                                                </asp:Panel>
                                            </td>
                                            <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
                                        </tr>
                                    </table>
                                 </FormTemplate>
                             </EditFormSettings>

                             <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                             <DetailTables>
                            <telerik:GridTableView runat="server" AllowAutomaticDeletes="True" Width="100%"
                                AutoGenerateColumns="False" CommandItemDisplay="Top" DataSourceID="objTAPDocuments" ShowHeadersWhenNoRecords="true" DataKeyNames="ID" AllowFilteringByColumn="False" AllowSorting="False" NoDetailRecordsText="No documents attached.">
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
                         <PagerStyle PageSizeControlType="RadComboBox" Position="TopAndBottom"></PagerStyle>

                         <FilterMenu EnableImageSprites="False"></FilterMenu>
                     </telerik:RadGrid>
                 </td>
             </tr>
         </table>
         <asp:ObjectDataSource ID="objTAPs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetTAPs" TypeName="TAPDAL.TAP" DeleteMethod="DeleteTAP" InsertMethod="InsertTAP" UpdateMethod="UpdateTAP">
             <DeleteParameters>
                 <asp:Parameter Name="Original_ID" Type="Int32" />
             </DeleteParameters>
             <InsertParameters>
                 <asp:Parameter Name="RequestDate" Type="DateTime" />
                 <asp:Parameter Name="Bilingual" Type="Boolean" />
                 <asp:Parameter Name="tlkpPosition" Type="Int32" />
                 <asp:Parameter Name="tlkpRequestReason" Type="Int32" />
                 <asp:Parameter Name="DateToExecs" Type="DateTime" />
                 <asp:Parameter Name="DateToHR" Type="DateTime" />
                 <asp:Parameter Name="FirstName" Type="String" />
                 <asp:Parameter Name="LastName" Type="String" />
                 <asp:Parameter Name="tlkpLocation" Type="Int32" />
                 <asp:Parameter Name="tlkpProgram" Type="Int32" />
                 <asp:Parameter Name="FirstLineSupervisorEmplID" Type="String" />
                 <asp:Parameter Name="SecondLineSupervisorEmplID" Type="String" />
                 <asp:Parameter Name="StartDate" Type="DateTime" />
                 <asp:Parameter Name="ActualEndDate" Type="DateTime" />
                 <asp:Parameter Name="Comments" Type="String" />
                 <asp:Parameter Name="ContactNumber" Type="String" />
                 <asp:Parameter Name="AntcpEndDate" Type="DateTime" />
             </InsertParameters>
             <SelectParameters>
                 <asp:Parameter Name="Position" Type="Object" />
                 <asp:Parameter Name="Program" Type="Object" />
                 <asp:Parameter Name="Office" Type="Object" />
                 <asp:Parameter Name="BeginActualEndDate" Type="DateTime" />
                 <asp:Parameter Name="EndActualEndDate" Type="DateTime" />
             </SelectParameters>
             <UpdateParameters>
                 <asp:Parameter Name="Original_ID" Type="Int32" />
                 <asp:Parameter Name="RequestDate" Type="DateTime" />
                 <asp:Parameter Name="Bilingual" Type="Boolean" />
                 <asp:Parameter Name="tlkpPosition" Type="Int32" />
                 <asp:Parameter Name="tlkpRequestReason" Type="Int32" />
                 <asp:Parameter Name="DateToExecs" Type="DateTime" />
                 <asp:Parameter Name="DateToHR" Type="DateTime" />
                 <asp:Parameter Name="FirstName" Type="String" />
                 <asp:Parameter Name="LastName" Type="String" />
                 <asp:Parameter Name="tlkpLocation" Type="Int32" />
                 <asp:Parameter Name="tlkpProgram" Type="Int32" />
                 <asp:Parameter Name="FirstLineSupervisorEmplID" Type="String" />
                 <asp:Parameter Name="SecondLineSupervisorEmplID" Type="String" />
                 <asp:Parameter Name="StartDate" Type="DateTime" />
                 <asp:Parameter Name="ActualEndDate" Type="DateTime" />
                 <asp:Parameter Name="Comments" Type="String" />
                 <asp:Parameter Name="ContactNumber" Type="String" />
                 <asp:Parameter Name="AntcpEndDate" Type="DateTime" />
             </UpdateParameters>
         </asp:ObjectDataSource>
         <asp:ObjectDataSource ID="objDSVacantPositions" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="TAP Position" Name="Category" Type="String" />
                   <asp:Parameter Name="CurrentID" Type="Int32" />
               </SelectParameters>
           </asp:ObjectDataSource>
         <asp:ObjectDataSource ID="objRequestReasons" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
               <SelectParameters>
                   <asp:Parameter DefaultValue="TAP Request Reason" Name="Category" Type="String" />
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
         
        <asp:ObjectDataSource ID="objTAPDocuments" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Get1546Documents" TypeName="Req1546DAL.DAL1546" DeleteMethod="Delete1546Document">
            <SelectParameters>
                <asp:Parameter Name="Request1546ID" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
         <asp:ObjectDataSource ID="objDSExtensionApproval" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForUpdateForm" TypeName="StaffingUtilities.Utilities">
            <SelectParameters>
                <asp:Parameter DefaultValue="Extension Approval" Name="Category" Type="String" />
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
         <asp:ObjectDataSource ID="objDSExtensions" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetTAPExtensions" TypeName="TAPDAL.TAP" DeleteMethod="DeleteTAPExtension" InsertMethod="InsertTAPExtension" UpdateMethod="UpdateTAPExtension">
             <DeleteParameters>
                 <asp:Parameter Name="Original_ID" Type="Int32" />
                 <asp:Parameter Name="original_tbl1546ID" Type="Int32" />
             </DeleteParameters>
             <InsertParameters>
                 <asp:Parameter Name="tbl1546ID" Type="Int32" />
                 <asp:Parameter Name="ExtStartDate" Type="DateTime" />
                 <asp:Parameter Name="AntcpEndDate" Type="DateTime" />
                 <asp:Parameter Name="tlkpExtensionApproval" Type="Int32" />
                 <asp:Parameter Name="DateToExecs" Type="DateTime" />
                 <asp:Parameter Name="DateToHR" Type="DateTime" />
             </InsertParameters>
             <SelectParameters>
                 <asp:Parameter Name="tbl1546ID" Type="Int32" />
             </SelectParameters>
             <UpdateParameters>
                 <asp:Parameter Name="Original_ID" Type="Int32" />
                 <asp:Parameter Name="original_tbl1546ID" Type="Int32" />
                 <asp:Parameter Name="ExtStartDate" Type="DateTime" />
                 <asp:Parameter Name="AntcpEndDate" Type="DateTime" />
                 <asp:Parameter Name="tlkpExtensionApproval" Type="Int32" />
                 <asp:Parameter Name="DateToExecs" Type="DateTime" />
                 <asp:Parameter Name="DateToHR" Type="DateTime" />
             </UpdateParameters>
         </asp:ObjectDataSource>
    </div>
</asp:Content>

