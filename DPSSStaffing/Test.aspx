<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Test.aspx.vb" Inherits="Test" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
            </Scripts>
        </telerik:RadScriptManager>
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" CellSpacing="0" GridLines="None" DataSourceID="SqlDannyDocs">
        <MasterTableView DataSourceID="SqlDannyDocs">
            <CommandItemSettings ExportToPdfText="Export to PDF">
            </CommandItemSettings>
            <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>
            <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="ID" DataType="System.Int32" FilterControlAltText="Filter ID column" HeaderText="ID" UniqueName="ID" SortExpression="ID">
                </telerik:GridBoundColumn>
                <telerik:GridAttachmentColumn AttachmentDataField="DOC" AttachmentKeyFields="ID" DataTextField="FileName" EditFormHeaderTextFormat="Upload File:" FileName="attachment" FileNameTextField="ID" FilterControlAltText="Filter Data column" HeaderText="Download" UniqueName="DOC">
                                    </telerik:GridAttachmentColumn>
            </Columns>
            <EditFormSettings>
                <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                </EditColumn>
            </EditFormSettings>
            <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
        </MasterTableView>
        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
        <FilterMenu EnableImageSprites="False">
        </FilterMenu>
    </telerik:RadGrid>

    <asp:SqlDataSource ID="SqlDannyDocs" runat="server" ConnectionString="<%$ ConnectionStrings:DPSSStaffing_SqlConnectionString %>" SelectCommand="uspDannyGetDocuments" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
</asp:Content>

