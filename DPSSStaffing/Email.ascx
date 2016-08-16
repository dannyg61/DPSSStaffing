<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Email.ascx.vb" Inherits="Email" %>
<link href="App_Themes/Default/Email.css" rel="stylesheet" />

<telerik:RadCodeBlock ID="EmailCodeBlock" runat="server">
                       
            <script type="text/javascript">
                var $ = $telerik.$;
                var uploadsInProgress = 0;

                function onFileSelected(sender, args) {
                    if (!uploadsInProgress)
                        $("#SaveButton").attr("disabled", "disabled");

                    uploadsInProgress++;

                    var row = args.get_row();

                    $(row).addClass("file-row");
                }

                function onFileUploaded(sender, args) {
                    decrementUploadsInProgress();
                }

                function onUploadFailed(sender, args) {
                    decrementUploadsInProgress();
                }

                function decrementUploadsInProgress() {
                    uploadsInProgress--;

                    if (!uploadsInProgress)
                        $("#SaveButton").removeAttr("disabled");
                }
            </script>


</telerik:RadCodeBlock>

<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">

</telerik:RadAjaxManagerProxy>

<input runat="server" type="hidden" id="UploadedFilesJson" />


    <div class="qsf-demo-canvas">
        <div class="qsf-fb">
            <table style="border-collapse: collapse; border-spacing: 0; width: 100%">
                <tr>
                    <td style="text-align: left; vertical-align: top">
                        <telerik:RadComboBox ID="rcbEmailRecipients" runat="server" Width="200px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                            DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                            ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailRecipient_SelectedIndexChanged" AllowCustomText="True">
                        </telerik:RadComboBox>
                    </td>
                    <td style="text-align: right; vertical-align: top">
                        <asp:Label ID="Label1" runat="server" Text="To:"></asp:Label>
                    </td>
                    <td style="text-align: left; vertical-align: top" rowspan="2">
                        <telerik:RadTextBox ID="txtEmailRecipients" runat="server"
                            EnableEmbeddedSkins="false" EnableViewState="false" TextMode="MultiLine" Width="300px" Height="50px">
                        </telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left; vertical-align: top">
                        <telerik:RadButton ID="btnAddEmailRecipient" runat="server" Text="Add" OnClick="btnAddEmailRecipient_Click" Width="70px"></telerik:RadButton>
                    </td>
                </tr>
            </table>
        </div>
         <div class="qsf-fb">
             <table style="border-collapse: collapse; border-spacing: 0; width: 100%">
                <tr>
                    <td style="text-align: left; vertical-align: top">
                        <telerik:RadComboBox ID="rcbEmailCC" runat="server" Width="200px" Height="70px" DataSourceID="objEmployeeSearch" DataTextField="NAME"
                            DataValueField="EMPLID" EnableAutomaticLoadOnDemand="True" EnableVirtualScrolling="True"
                            ItemsPerRequest="30" ShowMoreResultsBox="True" AutoPostBack="True" OnSelectedIndexChanged="rcbEmailCC_SelectedIndexChanged" AllowCustomText="True">
                        </telerik:RadComboBox>
                    </td>
                    <td style="text-align: right; vertical-align: top">
                        <asp:Label ID="Label3" runat="server" Text="CC:"></asp:Label>
                    </td>
                    <td style="text-align: left; vertical-align: top" rowspan="2">
                        <telerik:RadTextBox ID="txtEmailCC" runat="server"
                            EnableEmbeddedSkins="false" EnableViewState="false" TextMode="MultiLine" Width="300px" Height="50px">
                        </telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left; vertical-align: top">
                        <telerik:RadButton ID="btnAddEmailCC" runat="server" Text="Add" OnClick="btnAddEmailCC_Click" Width="70px"></telerik:RadButton>
                    </td>
                </tr>
            </table>
        </div>
        <div class="qsf-fb">
            <asp:Label ID="Label2" runat="server" Text="Subject:"></asp:Label>
            <telerik:RadTextBox ID="txtSubject" runat="server" EnableEmbeddedSkins="false" 
                Text="1546 Request"  EnableViewState="false" Width="400px">
            </telerik:RadTextBox>
            </div>
            <div class="qsf-fb-group qsf-fb-attachments">
                <asp:Image ID="Image1" runat="server" ImageUrl="images/qsf-demo-attachment-bg.png" />
                <strong>Attachments</strong>
                <telerik:RadAsyncUpload ID="RadAsyncUpload1" runat="server" HideFileInput="true"
                    MultipleFileSelection="Automatic"
                    AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.pdf"
                    OnClientFileUploadFailed="onUploadFailed" OnClientFileSelected="onFileSelected"
                    OnClientFileUploaded="onFileUploaded">
                </telerik:RadAsyncUpload>
                <span class="allowed-attachments">Select files to upload
                </span>

            </div>

            <div>
                
                <telerik:RadEditor ID="MailEditor" Runat="server" Width="100%" Height="150px" EditModes="All" EnableResize="False">
                    <Tools>
                        <telerik:EditorToolGroup Tag="MainToolbar">
                            <telerik:EditorTool Name="SpellCheck" />
                            <telerik:EditorTool Name="FindAndReplace" />
                            <telerik:EditorSeparator />
                            <telerik:EditorSplitButton Name="Undo">
                            </telerik:EditorSplitButton>
                            <telerik:EditorSplitButton Name="Redo">
                            </telerik:EditorSplitButton>
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="Cut" />
                            <telerik:EditorTool Name="Copy" />
                            <telerik:EditorTool Name="Paste" ShortCut="CTRL+V" />
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup Tag="Formatting">
                            <telerik:EditorTool Name="Bold" />
                            <telerik:EditorTool Name="Italic" />
                            <telerik:EditorTool Name="Underline" />
                            <telerik:EditorSeparator />
                            <telerik:EditorSplitButton Name="ForeColor">
                            </telerik:EditorSplitButton>
                            <telerik:EditorSplitButton Name="BackColor">
                            </telerik:EditorSplitButton>
                            <telerik:EditorSeparator />
                            <telerik:EditorDropDown Name="FontName">
                            </telerik:EditorDropDown>
                            <telerik:EditorDropDown Name="RealFontSize">
                            </telerik:EditorDropDown>
                        </telerik:EditorToolGroup>
                    </Tools>
                    <Content>
</Content>

<TrackChangesSettings CanAcceptTrackChanges="False"></TrackChangesSettings>
                </telerik:RadEditor>
                
            </div>
        <asp:ObjectDataSource ID="objEmployeeSearch" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetEmployees" TypeName="StaffEmployees.Employees"></asp:ObjectDataSource>
        </div>
