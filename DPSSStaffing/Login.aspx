<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" MasterPageFile="~/MasterPage.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
        </Scripts>
    </telerik:RadScriptManager>
        <table style="width: 100%">
            <tr>
                &nbsp;
            </tr>
            <tr>
                <td align="center">
                    <asp:Login ID="Login1" runat="server" Width="394px" DisplayRememberMe="False" 
                        PasswordLabelText="Password:  " UserNameLabelText="User Name:  "
                        BorderPadding="2" BorderStyle="Double" BorderWidth="1px" Height="135px"
                        LoginButtonText="Login" TitleText="Log into DPSS Staffing"
                        BackColor="LightGray" BorderColor="#000099" FailureText="Your login attempt was not successful." VisibleWhenLoggedIn="False">
                        <TitleTextStyle Font-Bold="True" />
                    </asp:Login>
                </td>
            </tr>
        </table>
</asp:Content>

