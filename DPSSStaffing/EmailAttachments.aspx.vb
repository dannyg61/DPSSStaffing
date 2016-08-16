
Partial Class EmailAttachments
    Inherits System.Web.UI.Page

    'Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
    '    Dim EmailAttachments As New ArrayList
    '    Dim NumberOfAttachments As Int32 = RadAsyncUpload1.UploadedFiles.Count
    '    If NumberOfAttachments > 0 Then
    '        EmailAttachments.Add("NoCancel;")
    '        For i = 0 To RadAsyncUpload1.UploadedFiles.Count - 1
    '            EmailAttachments.Add(RadAsyncUpload1.UploadedFiles(i).FileName & ";")
    '        Next
    '        For i = 0 To EmailAttachments.Count - 1
    '            hdnAttachments.Value = hdnAttachments.Value + EmailAttachments(i)
    '        Next
    '    Else
    '        EmailAttachments.Add("Cancel")
    '    End If
    'End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        hdnAttachments.Value = String.Empty
    End Sub

  
    'Protected Sub RadAsyncUpload1_FileUploaded(sender As Object, e As Telerik.Web.UI.FileUploadedEventArgs) Handles RadAsyncUpload1.FileUploaded
    '    Dim newFileName As String = e.File.GetNameWithoutExtension() + User.Identity.Name.Replace("\\", String.Empty) + e.File.GetExtension()
    '    e.File.SaveAs(System.IO.Path.Combine(Server.MapPath(RadAsyncUpload1.TargetFolder), newFileName))
    'End Sub
End Class
