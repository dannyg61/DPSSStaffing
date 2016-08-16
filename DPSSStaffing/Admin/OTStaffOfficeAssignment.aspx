<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="OTStaffOfficeAssignment.aspx.vb" Inherits="Admin_OTStaffOfficeAssignment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgOTStaffOfficeAssignments">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgOTStaffOfficeAssignments" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>
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
    <table  class="MainTable">
        <tr>
            <td>
                <telerik:RadGrid ID="rgOTStaffOfficeAssignments" runat="server" AutoGenerateColumns="False" CellSpacing="0" DataSourceID="objDSOTOfficeAssignment" GridLines="None" AllowAutomaticDeletes="True" AllowAutomaticInserts="True" AllowAutomaticUpdates="True" AllowFilteringByColumn="True">

                    <MasterTableView DataSourceID="objDSOTOfficeAssignment" CommandItemDisplay="Top" datakeynames="ID">
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
                            <telerik:GridBoundColumn DataField="ID" Display="False" FilterControlAltText="Filter column column" UniqueName="ID" ReadOnly="true" AllowFiltering="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Emplid" UniqueName="Emplid" FilterControlAltText="Filter Emplid column" Display="False" AllowFiltering="false"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="tlkpOverTimeLocation" UniqueName="tlkpOverTimeLocation" FilterControlAltText="Filter tlkpOverTimeLocation column" Display="False" AllowFiltering="false"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Employee_Name" FilterControlAltText="Filter DataValue column" HeaderText="Name" UniqueName="Employee_Name" FilterControlWidth="200px">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Office" FilterControlAltText="Filter Description column" HeaderText="Description" UniqueName="Office" FilterControlWidth="200px">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" ConfirmText="Are you sure you want to delete this employee?" FilterControlAltText="Filter DeleteEmployee column" ImageUrl="~/images/RadDelete.gif" UniqueName="DeleteEmployeeButton">
                            </telerik:GridButtonColumn>
                        </Columns>

                        <EditFormSettings EditFormType="Template">
                            <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                            <FormTemplate>
                                <table class="TableCellspacing0">
                                    <tr>
                                        <td style="width: 625px; text-align: left; vertical-align: text-top">
                                            <table class="TableCellspacing0">
                                                <tr>
                                                    <td style="width: 14%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                        <asp:Label ID="Label1" runat="server" Text="Employee:"></asp:Label>
                                                    </td>
                                                    <td style="width: 31%; text-align: left; text-wrap: none; vertical-align: text-top">
                                                        <asp:Panel ID="pnlEdit" runat="server" Visible='<%# ShowControlForEdit(CType(Container, GridItem).OwnerTableView.IsItemInserted)%> '>
                                                            <asp:Label ID="lblEmployee" runat="server" Text='<%# Eval("Employee_Name")%>'></asp:Label>
                                                        </asp:Panel>
                                                        <asp:Panel ID="pnlInsert" runat="server" Visible='<%# ShowControlForInsert(CType(Container, GridItem).OwnerTableView.IsItemInserted)%> '>
                                                            <telerik:RadComboBox ID="rcbEmployee" runat="server" Width="200px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                                DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True" OnSelectedIndexChanged="rcbEmployee_SelectedIndexChanged"
                                                                ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" AllowCustomText="True">
                                                            </telerik:RadComboBox>
                                                            <asp:RequiredFieldValidator ID="reqvalEmployee" runat="server" ErrorMessage="Employee required." CssClass="ValidatorRed" ControlToValidate="rcbEmployee" ValidationGroup="OvertimeOffice">*</asp:RequiredFieldValidator>
                                                        </asp:Panel>
                                                    </td>
                                                    <td style="width: 20%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                        <asp:Label ID="Label2" runat="server" Text="Office:"></asp:Label>
                                                    </td>
                                                    <td style="width: 35%; text-align: left; text-wrap: none; vertical-align: text-top">
                                                        <telerik:RadComboBox ID="rcbOffice" runat="server" DataSourceID="objDSOffices" DataTextField="DataValue" SelectedValue='<%# Bind("tlkpOverTimeLocation")%>'
                                                            DataValueField="ID" AutoPostBack="True" OnSelectedIndexChanged="rcbOffice_SelectedIndexChanged">
                                                        </telerik:RadComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: center" colspan="4">
                                                        <asp:ImageButton ID="btnUpdate" runat="server" CommandName='<%# IIf(CType(Container,GridItem).OwnerTableView.IsItemInserted ,"PerformInsert","Update") %>' ImageUrl='<%# IIf(CType(Container,GridItem).OwnerTableView.IsItemInserted,"~/images/btnAdd.gif","~/images/btnUpdate.gif") %>' ToolTip="Add" CausesValidation="true" />
                                                        <asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ImageUrl="~/Images/btnCancel.gif" ToolTip="Cancel" CausesValidation="false" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="ValidatorRed" ValidationGroup="OvertimeOffice" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                </table>
                            </FormTemplate>
                        </EditFormSettings>

                        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                    </MasterTableView>

                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                    <FilterMenu EnableImageSprites="False"></FilterMenu>

                </telerik:RadGrid>
                <asp:ObjectDataSource ID="objDSOTOfficeAssignment" runat="server" DeleteMethod="DeleteOvertimeOfficeAssigment" InsertMethod="InsertOvertimeOfficeAssigment" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeOfficeAssigments" TypeName="StaffingUtilities.Utilities" UpdateMethod="UpdateOvertimeOfficeAssigment">
                    <DeleteParameters>
                        <asp:Parameter Name="Original_ID" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:Parameter Name="EmplID" Type="String" />
                        <asp:Parameter Name="tlkpOverTimeLocation" Type="Int32" />
                    </InsertParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="Original_ID" Type="Int32" />
                        <asp:Parameter Name="EmplID" Type="String" />
                        <asp:Parameter Name="tlkpOverTimeLocation" Type="Int32" />
                    </UpdateParameters>
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="objDSOffices" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeLookupsNoBlankRow" TypeName="OvertimeNamespace.Overtime">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="Location" Name="SubCategory" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>                
                <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

