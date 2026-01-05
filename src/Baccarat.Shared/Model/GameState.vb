Imports System.Collections.Generic
Imports Baccarat.Shared.Protocol

Namespace Baccarat.Shared.Model
    Public Class GameState
        Public Property Phase As GamePhase = GamePhase.LOBBY
        Public Property RoundIndex As Integer = 1
        Public Property Clients As ClientInfo() = {Nothing, Nothing}
        Public Property Chips As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer) From {{1, Constants.InitChips}, {2, Constants.InitChips}}
        Public Property Bets As Dictionary(Of Integer, BetInfo) = New Dictionary(Of Integer, BetInfo)
        Public Property Shoe As List(Of Card) = New List(Of Card)()
        Public Property PlayerHand As Hand = New Hand()
        Public Property BankerHand As Hand = New Hand()
    End Class
End Namespace
