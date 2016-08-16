<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="OvertimeLookupMaintenance.aspx.vb" Inherits="Admin_OvertimeLookupMaintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rcbOvertimeLookupSubCategory">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgOvertimeLookups" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgOvertimeLookups">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgOvertimeLookups" LoadingPanelID="RadAjaxLoadingPanel1" />
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
                <asp:Label ID="lblCategory" runat="server" Text="Lookup Category:"></asp:Label>
                <telerik:RadComboBox ID="rcbOvertimeLookupSubCategory" runat="server" DataSourceID="objLookupSubCategory" DataTextField="SubCategory" DataValueField="SubCategory" AutoPostBack="True"></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadGrid ID="rgOvertimeLookups" runat="server" AllowAutomaticDeletes="True" AllowAutomaticInserts="True" AllowAutomaticUpdates="True" AutoGenerateColumns="False" CellSpacing="0" DataSourceID="objOvertimeLookups" GridLines="None">
                    <MasterTableView DataSourceID="objOvertimeLookups" CommandItemDisplay="Top" DataKeyNames="ID">
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
                            <telerik:GridBoundColumn DataField="DataValue" FilterControlAltText="Filter DataValue column" HeaderText="Name" UniqueName="DataValue">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Filter Description column" HeaderText="Description" UniqueName="Description">
                            </telerik:GridBoundColumn>
                            <telerik:GridCheckBoxColumn DataField="Active" DataType="System.Boolean" FilterControlAltText="Filter Active column" HeaderText="Active" UniqueName="Active">
                            </telerik:GridCheckBoxColumn>
                        </Columns>

                        <EditFormSettings>
                            <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                        </EditFormSettings>

                        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                    </MasterTableView>

                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                    <FilterMenu EnableImageSprites="False"></FilterMenu>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="objOvertimeLookups" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeLookupsForLookupMaintenance" TypeName="StaffingUtilities.Utilities" DeleteMethod="DeleteOvertimeLookup" InsertMethod="InsertOvertimeLookup" UpdateMethod="UpdateOvertimeLookup">
        <DeleteParameters>
            <asp:Parameter Name="Original_ID" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="SubCategory" Type="String" />
            <asp:Parameter Name="DataValue" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Active" Type="Boolean" />
            <asp:Parameter Name="EmplID" Type="String" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="rcbOvertimeLookupSubCategory" Name="SubCategory" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Original_ID" Type="Int32" />
            <asp:Parameter Name="SubCategory" Type="String" />
            <asp:Parameter Name="DataValue" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Active" Type="Boolean" />
            <asp:Parameter Name="EmplID" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="objLookupSubCategory" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeLookupSubCategories" TypeName="StaffingUtilities.Utilities">
        <SelectParameters>
            <asp:Parameter Name="SubCategory" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

