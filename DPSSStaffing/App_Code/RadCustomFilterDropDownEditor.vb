Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI

Namespace CustomEditors
    Public Class RadCustomFilterDropDownEditor
        Inherits RadFilterDataFieldEditor

        Protected Overrides Sub CopySettings(ByVal baseEditor As RadFilterDataFieldEditor)
            MyBase.CopySettings(baseEditor)
            Dim editor = TryCast(baseEditor, RadCustomFilterDropDownEditor)
            If editor IsNot Nothing Then
                DataSource = editor.DataSource
                DataTextField = editor.DataTextField
                DataValueField = editor.DataValueField
            End If
        End Sub

        Public Overrides Function ExtractValues() As ArrayList
            Dim list As New ArrayList()
            list.Add(_combo.SelectedValue)
            Return list
        End Function

        Public Overrides Sub InitializeEditor(container As Control)
            _combo = New RadComboBox()
            _combo.ID = "MyCombo"
            _combo.DataTextField = DataTextField
            _combo.DataValueField = DataValueField
            _combo.DataSource = DataSource
            _combo.DataBind()
            container.Controls.Add(_combo)
        End Sub

        Public Overrides Sub SetEditorValues(values As ArrayList)
            If values IsNot Nothing AndAlso values.Count > 0 Then
                If values(0) Is Nothing Then
                    Return
                End If
                Dim item = _combo.FindItemByValue(values(0).ToString())
                If item IsNot Nothing Then
                    item.Selected = True
                End If
            End If
        End Sub

        Public Property DataTextField() As String
            Get
                Return If(DirectCast(ViewState("DataTextField"), String), String.Empty)
            End Get
            Set(ByVal value As String)
                ViewState("DataTextField") = value
            End Set
        End Property
        Public Property DataValueField() As String
            Get
                Return If(DirectCast(ViewState("DataValueField"), String), String.Empty)
            End Get
            Set(ByVal value As String)
                ViewState("DataValueField") = value
            End Set
        End Property
        Public Property DataSource() As DataTable
            Get
                Return If(DirectCast(ViewState("DataSource"), DataTable), New DataTable())
            End Get
            Set(ByVal value As DataTable)
                ViewState("DataSource") = value
            End Set
        End Property

        Private _combo As RadComboBox
    End Class
End Namespace
