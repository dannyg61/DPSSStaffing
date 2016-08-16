
Partial Class MasterPage
    Inherits System.Web.UI.MasterPage


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
            Dim adminMenuItem As Telerik.Web.UI.RadMenuItem
            adminMenuItem = mnuMain.FindItemByText("Admin")
            adminMenuItem.Visible = False
            adminMenuItem.Enabled = False
        End If
    End Sub
End Class

