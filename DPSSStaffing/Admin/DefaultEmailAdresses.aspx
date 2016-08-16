<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="DefaultEmailAdresses.aspx.vb" Inherits="Admin_DefaultEmailAdresses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rcbEmailAddressCategory">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgDefaultEmailAddresses" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgDefaultEmailAddresses">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgDefaultEmailAddresses" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
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
                <asp:Label ID="lblEmailAddressCategory" runat="server" Text="Email Address Category:"></asp:Label>
                <telerik:RadComboBox ID="rcbEmailAddressCategory" runat="server" DataSourceID="objEmailAddressCategory" DataTextField="Category" 
                    DataValueField="Category" AutoPostBack="True"></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadGrid ID="rgDefaultEmailAddresses" runat="server" AllowAutomaticInserts="True" AllowAutomaticUpdates="True" AutoGenerateColumns="False" CellSpacing="0" GridLines="None" DataSourceID="objDefaultEmailAddresses" AllowAutomaticDeletes="True">
                     <MasterTableView DataSourceID="objDefaultEmailAddresses" CommandItemDisplay="Top" DataKeyNames="ID">
                        <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                        <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </RowIndicatorColumn>

                        <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </ExpandCollapseColumn>

                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" FilterControlAltText="Filter EditCommandColumn column">
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn DataField="ID" Display="False" FilterControlAltText="Filter column column" UniqueName="column" Visible="False" ReadOnly="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EmplID" FilterControlAltText="Filter DataValue column" HeaderText="EmplID" UniqueName="EmplID" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EMPLOYEE_NAME" FilterControlAltText="Filter DataValue column" HeaderText="Name" UniqueName="EMPLOYEE_NAME">
                            </telerik:GridBoundColumn>
                            <telerik:GridCheckBoxColumn DataField="Active" DataType="System.Boolean" FilterControlAltText="Filter Active column" HeaderText="Active" UniqueName="Active">
                            </telerik:GridCheckBoxColumn>
                        </Columns>

                        <EditFormSettings EditFormType="Template">
                            <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                            <FormTemplate>
                                <table class="TableCellspacing0">
                                    <tr>
                                        <td style="width: 625px; text-align: left; vertical-align: text-top">
                                            <table class="TableCellspacing0">
                                                <tr>
                                                    <td style="width: 25%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                        <asp:Label ID="Label1" runat="server" Text="Email Employee Name:"></asp:Label>
                                                    </td>
                                                    <td style="width: 25%; text-align: left; text-wrap: none; vertical-align: text-top">
                                                     <telerik:RadComboBox ID="rcbEmailEmployeeName" runat="server" Width="160px" Height="200px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                                                         DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                                                         ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailEmployeeName_SelectedIndexChanged" AllowCustomText="True">
                                                     </telerik:RadComboBox>
                                                    </td>
                                                     <td style="width: 25%; text-align: right; text-wrap: none; vertical-align: text-top">
                                                        <asp:Label ID="Label2" runat="server" Text="Active:"></asp:Label>
                                                    </td>
                                                    <td style="width: 25%; text-align: left; text-wrap: none; vertical-align: text-top">
                                                        <asp:CheckBox ID="chkActive" runat="server" Checked='<%# Bind("Active")%>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: center" colspan="2">
                                                        <asp:ImageButton ID="btnUpdate" runat="server" CommandName='<%# IIf(CType(Container,GridItem).OwnerTableView.IsItemInserted ,"PerformInsert","Update") %>' ImageUrl='<%# IIf(CType(Container,GridItem).OwnerTableView.IsItemInserted,"~/images/btnAdd.gif","~/images/btnUpdate.gif") %>' ToolTip="Add" CausesValidation="true"  />
                                                        <asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ImageUrl="~/Images/btnCancel.gif" ToolTip="Cancel" CausesValidation="false" />
                                                    </td>
                                                </tr
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </FormTemplate>
                        </EditFormSettings>

                        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                    </MasterTableView>

                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                    <FilterMenu EnableImageSprites="False"></FilterMenu>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
    
    <asp:ObjectDataSource ID="objDefaultEmailAddresses" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetDefaultEmailAddressesLookupMaintenance" TypeName="StaffingUtilities.Utilities" InsertMethod="InsertDefaultEmailAddress" UpdateMethod="UpdateDefaultEmailAddress">
        <InsertParameters>
            <asp:Parameter Name="Category" Type="String" />
            <asp:Parameter Name="EmailEmplID" Type="String" />
            <asp:Parameter Name="Active" Type="Boolean" />
            <asp:Parameter Name="EmplID" Type="String" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="rcbEmailAddressCategory" Name="Category" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Original_ID" Type="Int32" />
            <asp:Parameter Name="Category" Type="String" />
            <asp:Parameter Name="EmailEmplID" Type="String" />
            <asp:Parameter Name="Active" Type="Boolean" />
            <asp:Parameter Name="EmplID" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="objEmailAddressCategory" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetDefaultEmailAddressCategories" TypeName="StaffingUtilities.Utilities">
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
</asp:Content>

