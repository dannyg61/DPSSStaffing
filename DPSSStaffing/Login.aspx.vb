Imports StaffEmployees

Partial Class Login
    Inherits System.Web.UI.Page


    Protected Sub Login1_LoggingIn(sender As Object, e As LoginCancelEventArgs) Handles Login1.LoggingIn
        Dim clsEmployee As New Employees
        Session("EmplID") = clsEmployee.GetEmployeeNumberByLoginID(Login1.UserName)
    End Sub
End Class
