<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="LookupMaintenance.aspx.vb" Inherits="Admin_LookupMaintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rcbLookupCategory">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgLookupMaintenance" />
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
                <asp:Label ID="lblCategory" runat="server" Text="Lookup Category:"></asp:Label>
                <telerik:RadComboBox ID="rcbLookupCategory" runat="server" DataSourceID="objLookupCategory" DataTextField="Category" DataValueField="Category" AutoPostBack="True"></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadGrid ID="rgLookupMaintenance" runat="server" AllowPaging="True" AllowSorting="True" CellSpacing="0" DataSourceID="objLookupMaintenance" GridLines="None" AutoGenerateColumns="False" AllowAutomaticDeletes="True" AllowAutomaticInserts="True" AllowAutomaticUpdates="True">
                    <MasterTableView DataSourceID="objLookupMaintenance" CommandItemDisplay="Top" DataKeyNames="ID">
                        <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column">
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
    <asp:ObjectDataSource ID="objLookupCategory" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupCategories" TypeName="StaffingUtilities.Utilities"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="objLookupMaintenance" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLookupsForLookupMaintenance" TypeName="StaffingUtilities.Utilities" DeleteMethod="DeleteLookup" InsertMethod="InsertLookup" UpdateMethod="UpdateLookup">
        <DeleteParameters>
            <asp:Parameter Name="Original_ID" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Category" Type="String" />
            <asp:Parameter Name="DataValue" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Active" Type="Boolean" />
            <asp:Parameter Name="EmplID" Type="String" />
        </InsertParameters>
        <SelectParameters>
            <asp:Parameter Name="Category" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Original_ID" Type="Int32" />
            <asp:Parameter Name="Category" Type="String" />
            <asp:Parameter Name="DataValue" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Active" Type="Boolean" />
            <asp:Parameter Name="EmplID" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
</asp:Content>


