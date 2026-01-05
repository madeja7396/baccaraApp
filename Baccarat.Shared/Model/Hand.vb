Imports System.Collections.Generic
Imports Baccarat.Shared.Rules

Namespace Model
    Public Class Hand
        Public ReadOnly Property Cards As List(Of Card)

        Public Sub New()
            Cards = New List(Of Card)()
        End Sub

        Public Function Score(rules As IBaccaratRules) As Integer
            Return rules.ComputeScore(Me)
        End Function
    End Class
End Namespace
