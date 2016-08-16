Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI

Namespace CustomEditors3
    Public Class RadFilterDatePickerEditor
        Inherits RadFilterDataFieldEditor

        Private _datePicker As RadDatePicker
        Public Overrides Sub InitializeEditor(container As Control)
            _datePicker = New RadDatePicker()
            _datePicker.ID = "rdpActualEndDate"
            container.Controls.Add(_datePicker)
        End Sub

        Public Overrides Sub SetEditorValues(values As ArrayList)
            If values IsNot Nothing AndAlso values.Count > 0 Then
                If values(0) Is Nothing Then
                    Return
                End If
                _datePicker.SelectedDate = CType(values(0), Date)
            End If
        End Sub

        Public Overrides Function ExtractValues() As ArrayList
            Dim list As New ArrayList()
            list.Add(_datePicker.SelectedDate)
            Return list
        End Function
    End Class
End Namespace
