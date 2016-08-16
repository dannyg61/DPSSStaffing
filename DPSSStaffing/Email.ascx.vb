Imports System.Runtime.Serialization.Json
Imports System.IO
Imports System.Web.Services
Imports System.Runtime.Serialization
Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports StaffEmployees

Partial Class Email
    Inherits System.Web.UI.UserControl
    Private _uploadedFiles As New List(Of CustomUploadedFileInfo)()
    Public Property UploadedFiles() As List(Of CustomUploadedFileInfo)
        Get
            Return _uploadedFiles
        End Get
        Set(ByVal value As List(Of CustomUploadedFileInfo))
            _uploadedFiles = value
        End Set
    End Property

    Public Class CustomUploadedFileInfo
        Public Property FileName() As String
            Get
                Return m_FileName
            End Get
            Set(value As String)
                m_FileName = value
            End Set
        End Property
        Private m_FileName As String
        Public Property FileExtension() As String
            Get
                Return m_FileExtension
            End Get
            Set(value As String)
                m_FileExtension = value
            End Set
        End Property
        Private m_FileExtension As String
        Public Property ContentLength() As Integer
            Get
                Return m_ContentLength
            End Get
            Set(value As Integer)
                m_ContentLength = value
            End Set
        End Property
        Private m_ContentLength As Integer
    End Class

    Private Sub PopulateUploadedFilesList()
        For Each file As UploadedFile In RadAsyncUpload1.UploadedFiles
            Dim uploadedFileInfo As New CustomUploadedFileInfo()

            uploadedFileInfo.FileName = file.GetName()
            uploadedFileInfo.FileExtension = file.GetExtension().Replace(".", String.Empty)
            uploadedFileInfo.ContentLength = file.ContentLength / 1024
            UploadedFiles.Add(uploadedFileInfo)
        Next
    End Sub

    Protected Sub rcbEmailRecipient_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim clsEmployees As New StaffEmployees.Employees
        Dim dtEmployee As System.Data.DataTable
        dtEmployee = clsEmployees.GetEmployeeInformation(CType(sender, RadComboBox).SelectedValue)
        If dtEmployee.Rows.Count = 0 Then
            ViewState("EmailRecipient") = Nothing
            Exit Sub
        End If
        ViewState("EmailRecipient") = dtEmployee.Rows(0).Item("LOGIN_ID")
    End Sub

    Protected Sub rcbEmailCC_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim clsEmployees As New StaffEmployees.Employees
        Dim dtEmployee As System.Data.DataTable
        dtEmployee = clsEmployees.GetEmployeeInformation(CType(sender, RadComboBox).SelectedValue)
        If dtEmployee.Rows.Count = 0 Then
            ViewState("EmailCC") = Nothing
            Exit Sub
        End If
        ViewState("EmailCC") = dtEmployee.Rows(0).Item("LOGIN_ID")
    End Sub

    Protected Sub btnAddEmailRecipient_Click(sender As Object, e As EventArgs)
        If ViewState("EmailRecipient") Is Nothing Then
            Exit Sub
        End If
        Dim txtEmailRecipients As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("txtEmailRecipients")
        txtEmailRecipients.Text = txtEmailRecipients.Text + ViewState("EmailRecipient").ToString.ToLower & "@riversidedpss.org;"
        'Dim itmRecipient As New RadListBoxItem()
        'itmRecipient.Text = ViewState("EmailRecipient").ToString.ToLower & "@riversidedpss.org;"
        'rlbEmailRecipients.Items.Add(itmRecipient)
        'rlbEmailRecipients.CssClass = "RadListBox_Border"
    End Sub

    ' Not giving the user a button to remove recipients. Just have them delete it manually with the keyboard
    'Protected Sub btnRemoveEmailRecipient_Click(sender As Object, e As EventArgs)
    '    Dim rlbEmailRecipients As RadListBox = CType(CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("rlbEmailRecipients"), RadListBox)
    '    If rlbEmailRecipients.Items.Count > 0 Then
    '        If rlbEmailRecipients.SelectedIndex < 0 Then
    '            Exit Sub
    '        End If
    '        Dim i As Integer = rlbEmailRecipients.SelectedIndex
    '        rlbEmailRecipients.SelectedItem.Remove()
    '        If rlbEmailRecipients.Items.Count = 0 Then
    '            rlbEmailRecipients.CssClass = "RadListBox_Default"
    '        End If
    '    End If
    'End Sub

    Protected Sub btnAddEmailCC_Click(sender As Object, e As EventArgs)
        If ViewState("EmailCC") Is Nothing Then
            Exit Sub
        End If
        Dim txtEmailCC As RadTextBox = CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("ucEmailControl").FindControl("txtEmailCC")
        txtEmailCC.Text = txtEmailCC.Text + ViewState("EmailCC").ToString.ToLower & "@riversidedpss.org;"
        'Dim itmCC As New RadListBoxItem()
        'itmCC.Text = ViewState("EmailCC").ToString.ToLower & "@riversidedpss.org;"
        'rlbEmailCCs.Items.Add(itmCC)
        'rlbEmailCCs.CssClass = "RadListBox_Border"
    End Sub

    ' Not giving the user a button to remove recipients. Just have them delete it manually with the keyboard
    'Protected Sub btnRemoveEmailCC_Click(sender As Object, e As EventArgs)
    '    Dim rlbEmailCCs As RadListBox = CType(CType(sender.parent.parent.parent.parent, GridEditFormItem).FindControl("rlbEmailCCs"), RadListBox)
    '    If rlbEmailCCs.Items.Count > 0 Then
    '        If rlbEmailCCs.SelectedIndex < 0 Then
    '            Exit Sub
    '        End If
    '        Dim i As Integer = rlbEmailCCs.SelectedIndex
    '        rlbEmailCCs.SelectedItem.Remove()
    '        If rlbEmailCCs.Items.Count = 0 Then
    '            rlbEmailCCs.CssClass = "RadListBox_Default"
    '        End If
    '    End If
    'End Sub
End Class
