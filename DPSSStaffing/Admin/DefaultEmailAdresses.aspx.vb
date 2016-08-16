Imports Telerik.Web.UI

Partial Class Admin_DefaultEmailAdresses
    Inherits System.Web.UI.Page
    Public _Active As Boolean
    Protected Sub objDefaultEmailAddresses_Inserting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDefaultEmailAddresses.Inserting
        e.InputParameters("EmailEmplID") = ViewState("EmailEmplID")
        e.InputParameters("Category") = rcbEmailAddressCategory.SelectedValue
        e.InputParameters("EmplID") = Session("EmplID")
        e.InputParameters("Active") = _Active
    End Sub

    Protected Sub objDefaultEmailAddresses_Updating(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles objDefaultEmailAddresses.Updating
        e.InputParameters("EmailEmplID") = ViewState("EmailEmplID")
        e.InputParameters("Category") = rcbEmailAddressCategory.SelectedValue
        e.InputParameters("EmplID") = Session("EmplID")
        e.InputParameters("Active") = _Active
    End Sub


    Protected Sub rcbEmailEmployeeName_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        ViewState("EmailEmplID") = e.Value
    End Sub

    Protected Sub rgDefaultEmailAddresses_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles rgDefaultEmailAddresses.ItemCommand
        Select e.CommandName
            Case RadGrid.InitInsertCommandName
                ' this prevents null exception for Canceled checkbox on inserts
                e.Canceled = True
                Dim newValues As New System.Collections.Specialized.ListDictionary()
                newValues("Active") = False
                e.Item.OwnerTableView.InsertItem(newValues)
            Case RadGrid.EditCommandName
                ViewState("EmailEmplID") = CType(CType(e.Item, GridEditableItem)("EmplID"), GridTableCell).Text
            Case RadGrid.UpdateCommandName
                _Active = CType(CType(e.Item, GridEditFormItem).FindControl("chkActive"), CheckBox).Checked
            Case RadGrid.PerformInsertCommandName
                _Active = CType(CType(e.Item, GridEditFormItem).FindControl("chkActive"), CheckBox).Checked
        End Select
    End Sub

    Protected Sub rgDefaultEmailAddresses_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles rgDefaultEmailAddresses.ItemDataBound
        If TypeOf e.Item Is GridEditFormItem And e.Item.IsInEditMode Then
            If Not TypeOf (e.Item) Is GridEditFormInsertItem Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
                If item("EMPLOYEE_NAME").Text <> "&nbsp;" Then
                    Dim comboEmailEmployee As RadComboBox = DirectCast(item.FindControl("rcbEmailEmployeeName"), RadComboBox)
                    Dim preselectedFirstItem As New RadComboBoxItem()
                    preselectedFirstItem.Text = Server.HtmlDecode(item("EMPLOYEE_NAME").Text)
                    preselectedFirstItem.Value = Server.HtmlDecode(item("EmplID").Text)
                    comboEmailEmployee.Items.Insert(0, preselectedFirstItem)
                    comboEmailEmployee.SelectedIndex = 0
                    comboEmailEmployee.DataBind()
                End If
            End If
        End If
    End Sub
End Class
