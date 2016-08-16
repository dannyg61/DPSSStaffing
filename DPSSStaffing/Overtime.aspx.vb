Imports Telerik.Web.UI
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Linq

Imports OvertimeNamespace.Overtime
Imports StaffEmployees

Partial Class Overtime
    Inherits System.Web.UI.Page

    Dim ColumnHeaderCounter As Int32 = 0
    Public dtOvertimeOfficeLocations As DataTable

    Protected Sub Page_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        Dim mpSkinManager As RadSkinManager
        mpSkinManager = CType(Master.FindControl("RadSkinManager1"), RadSkinManager)
        'mpSkinManager.SkinID = "Simple"
        mpSkinManager.Visible = False

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
            'If not Admin then overtime office will be restricted to the office that the employee is assigned to
            ViewState("Emplid") = "All"
            btnExport.Visible = False
            If HttpContext.Current.User.IsInRole("SS Staff OT Update") Then
                'Dim EmployeeNumber As String
                Dim LoginID As String
                Dim StartingIndex As Int32 = Me.User.Identity.Name.IndexOf("\") + 1
                LoginID = Me.User.Identity.Name.Substring(StartingIndex).ToUpper
                Dim clsEmployee As New Employees
                'EmployeeNumber = clsEmployee.GetEmployeeNumberByLoginID(LoginID)
                ViewState("Emplid") = clsEmployee.GetEmployeeNumberByLoginID(LoginID)
                dtOvertimeOfficeLocations = OvertimeNamespace.Overtime.GetOvertimeEmployeeOffices(ViewState("Emplid"))
                If dtOvertimeOfficeLocations.Rows.Count = 0 Then
                    Response.Redirect("Default.aspx")
                End If
                'rcbOffice.SelectedValue = tlkpOverTimeLocation
                'rcbOffice.Enabled = False
            Else
                Response.Redirect("Default.aspx")
            End If
        Else
            ViewState("Emplid") = "All"
        End If

        If Not Page.IsPostBack Then
            'Set up initial values for grid data selection
            'Get current fiscal year
            If Now.Month > 6 Then
                ViewState("FiscalYear") = Now.Year.ToString & "/" & (Now.Year + 1).ToString
            Else
                ViewState("FiscalYear") = (Now.Year - 1).ToString & "/" & Now.Year.ToString
            End If
            ViewState("tlkpOtFY") = OvertimeNamespace.Overtime.GetOvertimeFiscalYear(ViewState("FiscalYear"))
            Dim EmplID As String = ViewState("Emplid")
            Dim tlkpOtOffice As Int32 = 0
            tlkpOtOffice = GetOvertimeFirstOffice(EmplID)
            tlkpOtOffice =
            ViewState("tlkpOtOffice") = tlkpOtOffice
            'ViewState("tlkpOtOffice") = OvertimeNamespace.Overtime.GetOvertimeFirstOffice(EmplID)
            'If Not HttpContext.Current.User.IsInRole("SS Staff Track Admin") Then
            '    btnOvertimeEntry.Visible = False
            '    btnOvertimeEntry.Visible = False
            'End If
        End If

        ' Refresh the grid after overtime entered via OvertimeEntry.aspx window
        If Request.Form("__EVENTTARGET") = "OvertimeUpdated" Then
            rpgOvertime.DataBind()
        End If
    End Sub

    Protected Sub rpgOvertime_CellDataBound(sender As Object, e As PivotGridCellDataBoundEventArgs) Handles rpgOvertime.CellDataBound
        Dim curBackColor As System.Drawing.Color
        If TypeOf e.Cell Is PivotGridColumnHeaderCell Then
            ColumnHeaderCounter += 1

            Select Case ColumnHeaderCounter
                Case 1, 2
                    curBackColor = System.Drawing.Color.Yellow
                Case 3, 4
                    curBackColor = System.Drawing.Color.PeachPuff
                Case 5, 6
                    curBackColor = System.Drawing.Color.PowderBlue
                Case 7, 8
                    curBackColor = System.Drawing.Color.Pink
                Case 9, 10
                    curBackColor = System.Drawing.Color.LightGray
                Case 11, 12
                    curBackColor = System.Drawing.Color.LightGreen
                Case 13, 14
                    curBackColor = System.Drawing.Color.LightBlue
                Case 15, 16
                    curBackColor = System.Drawing.Color.Bisque
                Case 17, 18
                    curBackColor = System.Drawing.Color.Beige
                Case 19, 20
                    curBackColor = System.Drawing.Color.Aquamarine
                Case Else
                    curBackColor = System.Drawing.Color.White
            End Select
            e.Cell.BackColor = curBackColor
            e.Cell.ForeColor = Drawing.Color.Black
            'ElseIf TypeOf e.Cell Is PivotGridRowHeaderCell Then
            '    e.Cell.BackColor = Drawing.Color.Yellow
            '    e.Cell.ForeColor = Drawing.Color.Black
        End If
        ' Right align the numeric data cells
        If Not (TypeOf (e.Cell) Is PivotGridRowHeaderCell) Then
            e.Cell.HorizontalAlign = HorizontalAlign.Right
        End If

        'If e.Cell.Text = "CalFRESH" Then
        '    Dim that As String = String.Empty
        'End If
    End Sub


    Protected Sub rcbOffice_DataBound(sender As Object, e As EventArgs) Handles rcbOffice.DataBound
        rcbOffice.SelectedValue = ViewState("tlkpOtOffice")
    End Sub

    Protected Sub rcbOffice_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles rcbOffice.SelectedIndexChanged
        ViewState("FiscalYear") = rcbFiscalYear.SelectedItem.Text
        ViewState("tlkpOtFY") = rcbFiscalYear.SelectedValue
        ViewState("tlkpOtOffice") = rcbOffice.SelectedValue
        rpgOvertime.DataBind()
    End Sub

    Protected Sub rcbFiscalYear_DataBound(sender As Object, e As EventArgs) Handles rcbFiscalYear.DataBound
        rcbFiscalYear.SelectedValue = ViewState("tlkpOtFY")
    End Sub

    Protected Sub rcbFiscalYear_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles rcbFiscalYear.SelectedIndexChanged
        ViewState("FiscalYear") = rcbFiscalYear.SelectedItem.Text
        ViewState("tlkpOtFY") = rcbFiscalYear.SelectedValue
        ViewState("tlkpOtOffice") = rcbOffice.SelectedValue
        rpgOvertime.DataBind()
    End Sub

    Protected Sub objDSOvertime_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSOvertime.Selecting
        e.InputParameters("tlkpOtFY") = ViewState("tlkpOtFY")
        e.InputParameters("FiscalYear") = ViewState("FiscalYear")
        e.InputParameters("tlkpOtOffice") = ViewState("tlkpOtOffice")
    End Sub

    Protected Sub objDSOffices_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles objDSOffices.Selecting
        e.InputParameters("Emplid") = ViewState("Emplid")
    End Sub

    Private Function GetOvertimeFirstOffice(EmplID As String) As Integer
        Dim OverTimeOffice As Integer
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DPSSStaffing_SqlConnectionString").ConnectionString)
        Dim cm As New SqlCommand("uspGetOvertimeFirstOffice", cn)
        cm.CommandType = CommandType.StoredProcedure
        Dim parmEmplID As New SqlParameter("@EmplID", EmplID)
        cm.Parameters.Add(parmEmplID)
        cn.Open()

        Dim dtLookups As New DataTable
        Dim da As New SqlDataAdapter(cm)
        da.Fill(dtLookups)
        cn.Close()
        OverTimeOffice = dtLookups.Rows(0).Item("ID")
        Return OverTimeOffice
        'Return dtLookups.Rows(0).Item("ID")
    End Function

    Protected Sub btnExport_Click(sender As Object, e As ImageClickEventArgs)
        rpgOvertime.ExportSettings.Excel.Format = PivotGridExcelFormat.Biff
        rpgOvertime.ExportSettings.IgnorePaging = False
        rpgOvertime.ExportToExcel()
    End Sub

    Private Sub rpgOvertime_PivotGridCellExporting(sender As Object, e As PivotGridCellExportingArgs) Handles rpgOvertime.PivotGridCellExporting
        Dim modelDataCell As PivotGridBaseModelCell = CType(e.PivotGridModelCell, PivotGridBaseModelCell)
        Dim this As String = String.Empty
    End Sub
End Class
