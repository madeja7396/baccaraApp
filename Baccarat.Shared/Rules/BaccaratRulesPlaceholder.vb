Imports System
Imports System.Collections.Generic
Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol
Imports System.Linq

Namespace Rules
    ' 初期段階の簡易ルール（第三カードなし）
    Public Class BaccaratRulesPlaceholder
        Implements IBaccaratRules

        Public Function ComputeScore(hand As Hand) As Integer Implements IBaccaratRules.ComputeScore
            Dim total As Integer = hand.Cards.Sum(Function(c) CardValue(c))
            Return total Mod 10
        End Function

        Public Sub DealInitial(state As GameState) Implements IBaccaratRules.DealInitial
            state.PlayerHand = New Hand()
            state.BankerHand = New Hand()
            DrawToHand(state.Shoe, state.PlayerHand, 2)
            DrawToHand(state.Shoe, state.BankerHand, 2)
        End Sub

        Public Sub ApplyThirdCardRule(state As GameState) Implements IBaccaratRules.ApplyThirdCardRule
            ' TODO: add third-card rule in later sprint
        End Sub

        Public Function DetermineWinner(state As GameState) As Winner Implements IBaccaratRules.DetermineWinner
            Dim playerScore = ComputeScore(state.PlayerHand)
            Dim bankerScore = ComputeScore(state.BankerHand)

            If playerScore > bankerScore Then Return Winner.Player
            If bankerScore > playerScore Then Return Winner.Banker
            Return Winner.Tie
        End Function

        Private Shared Function CardValue(card As Card) As Integer
            Dim rank = card.Rank
            If rank >= 10 Then Return 0
            Return rank
        End Function

        Private Shared Sub DrawToHand(shoe As List(Of Card), hand As Hand, count As Integer)
            For i = 1 To count
                If shoe.Count = 0 Then Exit For
                hand.Cards.Add(shoe(0))
                shoe.RemoveAt(0)
            Next
        End Sub
    End Class
End Namespace
