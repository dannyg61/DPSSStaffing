
<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Overtime.aspx.vb" Inherits="Overtime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
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
                 function OpenOvertimeEntryWindow() {
                     try {
                         var comboOffice = $find("<%= rcbOffice.ClientID%>");
                         var officeValue = comboOffice.get_selectedItem().get_value();
                         var comboFiscalYear = $find("<%= rcbFiscalYear.ClientID%>");
                         var fiscalYearValue = comboFiscalYear.get_selectedItem().get_text();
                         var url = "OvertimeEntry.aspx?Office=" + officeValue + "&FiscalYear=" + fiscalYearValue;
                         var oWnd = window.radopen(url, "winOvertimeEntry");
                     } catch (e) {
                         alert(e);
                     }
                     return false;
                 }

                 function OnClientOvertimeEntryClose(sender, eventArgs) {
                     var arg = eventArgs.get_argument();
                     if (arg.Action.substring(0, 6) != 'Cancel') {
                         __doPostBack('OvertimeUpdated', 'OvertimeUpdated');
                     }
                 }

                 //function __doOvertimeEntryPostBack(sender, eventArgs) {
                 //    theForm.__EVENTTARGET.value = 'OvertimeUpdated';
                 //    theForm.__EVENTARGUMENT.value = eventArgs._newValue;
                 //    theForm.submit();
                 //}

             </script>
        </telerik:RadCodeBlock>

        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="rpgOvertime">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rpgOvertime" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="rcbFiscalYear">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rpgOvertime" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>

        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>

        <div>
            <table style="width: 100%">
                <tr>
                    <td style="width: 15%; text-align:left">
                        <telerik:RadComboBox ID="rcbFiscalYear" runat="server" DataSourceID="objDSFiscalYears" DataTextField="DataValue" DataValueField="ID" Label="Fiscal Year:" AutoPostBack="True"></telerik:RadComboBox>
                    </td>
                    <td style="width: 15%; text-align:left">
                        <telerik:RadComboBox ID="rcbOffice" runat="server" DataSourceID="objDSOffices" DataTextField="DataValue" DataValueField="ID" Label="Office:" AutoPostBack="True"></telerik:RadComboBox>
                    </td>
                    <td style="width: 70%; text-align:right">                        
                         <asp:Button ID="btnOvertimeEntry" runat="server" Text="Overtime Entry" OnClientClick="return OpenOvertimeEntryWindow()" CausesValidation="False" UseSubmitBehavior="False" />
                        <asp:ImageButton ID="btnExport" runat="server" ImageUrl="images/Excel_XLSX.PNG" OnClick="btnExport_Click" />   
                    </td>
                </tr>
            </table>
        </div>
        <div>

            <telerik:RadPivotGrid ID="rpgOvertime" runat="server" DataSourceID="objDSOvertime" FilterHeaderZoneText="" Skin="Simple">
                <PagerStyle ChangePageSizeButtonToolTip="Change Page Size" PageSizeControlType="RadComboBox"></PagerStyle>
                
                <OlapSettings>
                </OlapSettings>
                <Fields>
                    <telerik:PivotGridColumnField DataField="Program" UniqueName="Program">
                    </telerik:PivotGridColumnField>
                    <telerik:PivotGridRowField DataField="YearAndPayPeriod" UniqueName="YeahAndPayPeriod" Caption="Pay Period">
                    </telerik:PivotGridRowField>
                    <%--<telerik:PivotGridAggregateField DataField="OTHours" UniqueName="OTHours">
                        <CellTemplate>
                            <asp:Label ID="Label1" runat="server">
                                <%# Container.DataItem%>
                            </asp:Label>
                        </CellTemplate>
                    </telerik:PivotGridAggregateField>--%>
                    <telerik:PivotGridAggregateField DataField="OTHours" GrandTotalAggregateFormatString="" UniqueName="OTHours" DataFormatString="{0:#,##0.##}">
                    </telerik:PivotGridAggregateField>
                </Fields>

                <ConfigurationPanelSettings EnableOlapTreeViewLoadOnDemand="True"></ConfigurationPanelSettings>
            </telerik:RadPivotGrid>

        </div>
        <asp:ObjectDataSource ID="objDSOvertime" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertime" TypeName="OvertimeNamespace.Overtime">
            <SelectParameters>
                <asp:Parameter Name="tlkpOtFY" Type="Int32" />
                <asp:Parameter Name="FiscalYear" Type="String" />
                <asp:Parameter Name="tlkpOtOffice" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objDSFiscalYears" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeLookupsNoBlankRow" TypeName="OvertimeNamespace.Overtime">
            <SelectParameters>
                <asp:Parameter DefaultValue="FY" Name="SubCategory" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="objDSOffices" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeEmployeeOffices" TypeName="OvertimeNamespace.Overtime">
            <SelectParameters>
                <asp:Parameter Name="EmplID" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
        <Windows>
                <telerik:RadWindow runat="server" ID="winOvertimeEntry" VisibleOnPageLoad="false" Title="Overtime Entry" 
                    Width="600px" Height="550px" OnClientClose="OnClientOvertimeEntryClose" Behaviors="Move" Modal="true">
                </telerik:RadWindow>
            </Windows>
    </telerik:RadWindowManager>
</asp:Content>

