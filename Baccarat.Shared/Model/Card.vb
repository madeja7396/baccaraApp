Namespace Baccarat.Shared.Model
    Public Class Card
        Public Property Suit As Char ' S/H/D/C
        Public Property Rank As Integer ' 1..13

        Public Sub New()
        End Sub

        Public Sub New(suit As Char, rank As Integer)
            Me.Suit = suit
            Me.Rank = rank
        End Sub

        Public Function ToCode() As String
            Return $"{Suit}-{Rank:00}"
        End Function
    End Class
End Namespace
