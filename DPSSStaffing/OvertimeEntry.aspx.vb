
Imports Telerik.Web.UI
Imports StaffEmployees

Partial Class OvertimeEntry
    Inherits System.Web.UI.Page
    Private EndingCalendarYear As String = String.Empty
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim EmployeeNumber As String
            Dim LoginID As String
            Dim Office As Int32 = Request.QueryString("Office")
            Dim FiscalYear As String = Request.QueryString("FiscalYear")
            EndingCalendarYear = FiscalYear.Substring(5, 4)
            Dim StartingIndex As Int32 = Me.User.Identity.Name.IndexOf("\") + 1
            LoginID = Me.User.Identity.Name.Substring(StartingIndex).ToUpper
            Dim clsEmployee As New Employees
            EmployeeNumber = clsEmployee.GetEmployeeNumberByLoginID(LoginID)

            If Request.Form("__EVENTTARGET") = "Update" Then

                Dim PayPeriodPickerID As String = String.Empty
                Dim ProgramPickerID As String = String.Empty
                Dim OTHoursTextBoxID As String = String.Empty
                Dim myForm As HtmlForm
                ' Loop through all 14 possible entry rows on the form
                For i As Int32 = 1 To 14
                    PayPeriodPickerID = "PayPeriodPicker" & i
                    ProgramPickerID = "rcbProgram" & i
                    OTHoursTextBoxID = "txtOTHours" & i

                    ' PayPeriodPickerX is the user control with the radcombobox in it that contains the pay period selected
                    ' find the htmlform
                    For j As Int32 = 0 To sender.controls.count - 1
                        If TypeOf sender.controls(j) Is HtmlForm Then
                            myForm = sender.controls(j)
                        End If
                    Next

                    ' controls(7) within the form is a panel
                    Dim myPanel As Panel = myForm.Controls(7)

                    Dim payPeriodComboBox As Telerik.Web.UI.RadComboBox = myPanel.FindControl(PayPeriodPickerID).Controls(0)
                    If payPeriodComboBox.SelectedItem.Text <> String.Empty Then
                        Dim CalendarYear As String = String.Empty
                        Dim PayPeriod As Int32
                        PayPeriod = CInt(CType(myPanel.FindControl(PayPeriodPickerID).Controls(0), RadComboBox).SelectedItem.Text)
                        Dim Program As Int32 = 0
                        Dim OTHours As Decimal = 0
                        If CType(myPanel.FindControl(ProgramPickerID), RadComboBox).SelectedValue = 0 Then
                            Continue For
                        Else
                            Program = CType(myPanel.FindControl(ProgramPickerID), RadComboBox).SelectedValue
                        End If
                        If CType(myPanel.FindControl(OTHoursTextBoxID), RadNumericTextBox).Text = String.Empty Then
                            Continue For
                        Else
                            OTHours = CDec(CType(myPanel.FindControl(OTHoursTextBoxID), RadNumericTextBox).Text)
                        End If
                        If PayPeriod > 14 Then
                            CalendarYear = FiscalYear.Substring(0, 4)
                        Else
                            CalendarYear = FiscalYear.Substring(5, 4)
                        End If
                        Dim CalendarYearValue As Int32 = OvertimeNamespace.Overtime.GetOvertimeCalendarYearValue(CalendarYear)
                        Dim FiscalYearValue As Int32 = OvertimeNamespace.Overtime.GetOvertimeFiscalYear(FiscalYear)
                        OvertimeNamespace.Overtime.InsertUpdateOvertime(FiscalYearValue, Office, CalendarYearValue, PayPeriod, Program, OTHours, EmployeeNumber)
                    End If
                Next
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Sub objDSPrograms_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSPrograms.Selecting
        If EndingCalendarYear < "2017" Then
            e.InputParameters("HistOrCurr") = "Hist"
        Else
            e.InputParameters("HistOrCurr") = "Curr"
        End If
    End Sub
End Class

