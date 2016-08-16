
Partial Class Admin_LookupMaintenance
    Inherits System.Web.UI.Page

    Protected Sub rcbLookupCategory_SelectedIndexChanged(sender As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles rcbLookupCategory.SelectedIndexChanged
        objLookupMaintenance.SelectParameters("Category").DefaultValue = rcbLookupCategory.SelectedValue
        rgLookupMaintenance.Rebind()
    End Sub

    Protected Sub objLookupMaintenance_Inserting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objLookupMaintenance.Inserting
        e.InputParameters("Category") = rcbLookupCategory.SelectedValue
        e.InputParameters("EmplID") = Session("EmplID")
    End Sub

    Protected Sub objLookupMaintenance_Updating(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objLookupMaintenance.Updating
        e.InputParameters("Category") = rcbLookupCategory.SelectedValue
        e.InputParameters("EmplID") = Session("EmplID")
    End Sub
End Class
