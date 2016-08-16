<%@ Page Language="VB" AutoEventWireup="false" CodeFile="OvertimeEntry.aspx.vb" Inherits="OvertimeEntry" %>
<%@ Register TagPrefix="uc" TagName="PayPeriodPicker" Src="~/OvertimePayPeriodPicker.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<%--<body onload="SetVariable();">--%>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
            <script type="text/javascript">
                
                <%--function SetVariable() {
                    window.myHidden = document.getElementById('<%= rcbFiscalYear.ClientID%>');
                }--%>

                function returnToParentUpdate(args) {
                    //if (document.getElementById("txtOTHours").value == '' || document.getElementById("rcbPayPeriod").value == '' ||
                    //    document.getElementById("rcbProgram").value == '') {
                    //    //keep this empty if option so that the page will post back and the window won't close 
                    //    //if any required values are not entered
                    //}
                    //else {
                        // force postback to execute the update logic in the code behind and then close the window and return to parent
                        __doPostBack('Update', '');
                        var oArg = new Object();
                        oArg.Action = 'Update;';
                        //oArg.OThours = document.getElementById("txtOTHours").value
                        var oWnd = GetRadWindow();
                        oWnd.close(oArg);
                        return false;
                    //}
                }

                function returnToParentCancel(args) {
                    var oArg = new Object();
                    oArg.Action = 'Cancel;';
                    var oWnd = GetRadWindow();
                    oWnd.close(oArg);
                    return false;
                }

                function GetRadWindow() {
                    var oWindow = null;
                    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog 
                    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well) 

                    return oWindow;
                }
            </script>
        </telerik:RadScriptBlock>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server"></telerik:RadAjaxManager>
        <div style="vertical-align: top">
            <asp:Panel ID="pnlOvertime" runat="server" GroupingText="Overtime Entry">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Pay Period:"></asp:Label></td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker1" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Program:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="rcbProgram1" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Hours:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="txtOTHours1" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker2" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label1" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram2" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label2" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours2" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker3" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label5" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram3" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label6" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours3" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker4" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label7" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram4" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label8" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours4" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker5" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label9" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram5" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label10" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours5" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker6" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label11" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram6" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label12" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours6" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker7" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label13" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram7" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label14" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours7" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker8" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label15" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram8" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label16" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours8" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker9" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label17" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram9" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label18" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours9" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker10" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label19" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram10" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label20" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours10" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker11" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label21" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram11" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label22" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours11" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker12" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label23" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram12" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label24" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours12" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker13" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label25" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram13" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label26" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours13" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc:PayPeriodPicker ID="PayPeriodPicker14" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label27" runat="server" Text="Program:"></asp:Label>--%>
                            <telerik:RadComboBox ID="rcbProgram14" runat="server" DataSourceID="objDSPrograms" DataTextField="DataValue" DataValueField="ID"></telerik:RadComboBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <%--<asp:Label ID="Label28" runat="server" Text="Hours:"></asp:Label>--%>
                            <telerik:RadNumericTextBox ID="txtOTHours14" Runat="server" Width="50px">
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                </table>
                <asp:ObjectDataSource ID="objDSPrograms" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOvertimeLookups" TypeName="OvertimeNamespace.Overtime">
                    <SelectParameters>
                        <asp:Parameter Name="HistOrCurr" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Overtime" ForeColor="Red" />
            </asp:Panel>
        </div>
        
        <div>
            <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClientClick="return returnToParentUpdate (false)" CausesValidation="true" ValidationGroup="Overtime" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return returnToParentCancel (false)" />
            <asp:HiddenField ID="hdnAttachments" runat="server" ClientIDMode="Static" />
        </div>
    </form>
</body>
</html>
