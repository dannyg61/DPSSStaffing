Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI

Namespace CustomEditors2
    Public Class MyRadFilterCheckBoxEditor
        Inherits RadFilterDataFieldEditor

        Private _checkBox As CheckBox

        Public Overrides Function ExtractValues() As ArrayList
            Dim list As New ArrayList()
            list.Add(_checkBox.Checked)
            Return list
        End Function

        Public Overrides Sub InitializeEditor(container As Control)
            _checkBox = New CheckBox
            _checkBox.ID = "chkShowCanceled"
            container.Controls.Add(_checkBox)
        End Sub

        Public Overrides Sub SetEditorValues(values As ArrayList)
            If values IsNot Nothing AndAlso values.Count > 0 Then
                If values(0) Is Nothing Then
                    Return
                End If
                _checkBox.Checked = values(0)
            End If
        End Sub
    End Class
End Namespace
