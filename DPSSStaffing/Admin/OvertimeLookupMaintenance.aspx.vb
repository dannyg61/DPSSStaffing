
Partial Class Admin_OvertimeLookupMaintenance
    Inherits System.Web.UI.Page

    Protected Sub objOvertimeLookups_Inserting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objOvertimeLookups.Inserting
        e.InputParameters("SubCategory") = rcbOvertimeLookupSubCategory.SelectedValue
        e.InputParameters("EmplID") = Session("EmplID")
    End Sub


    Protected Sub objOvertimeLookups_Updating(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objOvertimeLookups.Updating
        e.InputParameters("SubCategory") = rcbOvertimeLookupSubCategory.SelectedValue
        e.InputParameters("EmplID") = Session("EmplID")
    End Sub

    Protected Sub rgOvertimeLookups_DeleteCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles rgOvertimeLookups.DeleteCommand

    End Sub
End Class
