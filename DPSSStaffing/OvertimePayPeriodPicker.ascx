<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OvertimePayPeriodPicker.ascx.vb" Inherits="OvertimePayPeriodPicker" %>

        <telerik:RadComboBox ID="rcbPayPeriod" runat="server" Width="75px">
            <Items>
                <telerik:RadComboBoxItem runat="server" />
                <telerik:RadComboBoxItem runat="server" Text="1" Value="1" />
                <telerik:RadComboBoxItem runat="server" Text="2" Value="2" />
                <telerik:RadComboBoxItem runat="server" Text="3" Value="3" />
                <telerik:RadComboBoxItem runat="server" Text="4" Value="4" />
                <telerik:RadComboBoxItem runat="server" Text="5" Value="5" />
                <telerik:RadComboBoxItem runat="server" Text="6" Value="6" />
                <telerik:RadComboBoxItem runat="server" Text="7" Value="7" />
                <telerik:RadComboBoxItem runat="server" Text="8" Value="8" />
                <telerik:RadComboBoxItem runat="server" Text="9" Value="9" />
                <telerik:RadComboBoxItem runat="server" Text="10" Value="10" />
                <telerik:RadComboBoxItem runat="server" Text="11" Value="11" />
                <telerik:RadComboBoxItem runat="server" Text="12" Value="12" />
                <telerik:RadComboBoxItem runat="server" Text="13" Value="13" />
                <telerik:RadComboBoxItem runat="server" Text="14" Value="14" />
                <telerik:RadComboBoxItem runat="server" Text="15" Value="15" />
                <telerik:RadComboBoxItem runat="server" Text="16" Value="16" />
                <telerik:RadComboBoxItem runat="server" Text="17" Value="17" />
                <telerik:RadComboBoxItem runat="server" Text="18" Value="18" />
                <telerik:RadComboBoxItem runat="server" Text="19" Value="19" />
                <telerik:RadComboBoxItem runat="server" Text="20" Value="20" />
                <telerik:RadComboBoxItem runat="server" Text="21" Value="21" />
                <telerik:RadComboBoxItem runat="server" Text="22" Value="22" />
                <telerik:RadComboBoxItem runat="server" Text="23" Value="23" />
                <telerik:RadComboBoxItem runat="server" Text="24" Value="24" />
                <telerik:RadComboBoxItem runat="server" Text="25" Value="25" />
                <telerik:RadComboBoxItem runat="server" Text="26" Value="26" />
            </Items>
        </telerik:RadComboBox>
        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Pay Period is required" ControlToValidate="rcbPayPeriod" ValidationGroup="Overtime" ForeColor="Red">*</asp:RequiredFieldValidator>--%>