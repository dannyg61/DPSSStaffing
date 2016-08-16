<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EmailAttachments.aspx.vb" Inherits="EmailAttachments" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email Attachments</title>
</head>
<body onload="SetVariable();">
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server" ClientIDMode="Static">
            <script type="text/javascript">
                
                function SetVariable() {
                    window.myHidden = document.getElementById('<%= hdnAttachments.ClientID %>');
                }

                function GetRadWindow() {
                    var oWindow = null;
                    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog 
                    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well) 

                    return oWindow;
                }

                              
                function returnToParent(args) {
                    __doPostBack('', '');
                    var oArg = new Object();
                    oArg.AttachmentList = window.myHidden.value;
                    var oWnd = GetRadWindow();
                    oWnd.close(oArg);
                    return false;
                }

                function returnToParentCancel(args) {
                    var oArg = new Object();
                    oArg.AttachmentList = 'Cancel;';
                    var oWnd = GetRadWindow();
                    oWnd.close(oArg);
                    return false;
                }

                function OnClientFileSelected(sender, args) {
                    window.myHidden.value = window.myHidden.value + args.get_fileName() + ';';

                }

                function testThis(oArg1, oArg2) {
                    alert(oArg1 + ' ' + oArg2);
                }
            </script>
        </telerik:RadScriptBlock>
        <div style="vertical-align: top">
            <asp:Panel ID="Panel1" runat="server" GroupingText="Select Attachments">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/qsf-demo-attachment-bg.png" />
                <telerik:RadAsyncUpload ID="RadAsyncUpload1" runat="server" HideFileInput="true" OnClientFileSelected="OnClientFileSelected"
                    MultipleFileSelection="Automatic" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.pdf" TemporaryFolder="~/App_Data/RadUploadTemp" TargetFolder="~/App_Data/EmailAttachments" Height="200px" Localization-Select="Select Files">
                </telerik:RadAsyncUpload>
            </asp:Panel>
        </div>
        <div>
            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClientClick="return returnToParent (false)" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return returnToParentCancel (false)" />
            <asp:HiddenField ID="hdnAttachments" runat="server" ClientIDMode="Static" />
        </div>
    </form>
</body>
</html>
