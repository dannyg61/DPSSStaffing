Imports System.Data
Imports Telerik.Web.UI
Imports System.Data.SqlClient
Partial Class Test
    Inherits System.Web.UI.Page

    Protected Sub RadGrid1_ItemCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles RadGrid1.ItemCommand
        If (e.CommandName = RadGrid.DownloadAttachmentCommandName) Then
            Dim args As GridDownloadAttachmentCommandEventArgs = TryCast(e, GridDownloadAttachmentCommandEventArgs)
            Dim DocumentId As Integer = DirectCast(args.AttachmentKeyValues("ID"), Integer)
            Dim cls1546Requests As New Req1546DAL.DAL1546
            Dim filename As String = ""
            Dim binarydata As Byte()

            Dim dt As DataTable

            dt = GetDocumentObject(DocumentId)
            If dt.Rows.Count > 0 Then
                filename = dt.Rows(0).Item("Filename")
                binarydata = dt.Rows(0).Item("DOC")

                Response.Clear()
                Response.ContentType = "application/octet-stream"
                Response.AddHeader("content-disposition", "attachment; filename=" + filename)
                Response.BinaryWrite(binarydata)
                Response.[End]()
            End If
        End If
    End Sub

    Public Function GetDocumentObject(ByVal ID As Int32) As Data.DataTable
        Dim dt As New DataTable
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
        Try

            Dim cm As New SqlClient.SqlCommand("uspDannyGetDocumentObject", cn)
            cm.Parameters.Add(New SqlParameter("@ID", Data.SqlDbType.Int))
            cm.Parameters("@ID").Value = ID
            cm.CommandType = CommandType.StoredProcedure

            Dim da As New SqlClient.SqlDataAdapter(cm)
            cn.Open()
            da.Fill(dt)
            cn.Close()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cn.State = ConnectionState.Open Then
                cn.Close()
            End If
        End Try
        Return dt

    End Function
End Class
