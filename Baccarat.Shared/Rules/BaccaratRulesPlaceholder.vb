Imports System
Imports System.Collections.Generic
Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol
Imports System.Linq

Namespace Rules
    ' バカラ簡易ルール（段階1：簡略第三カード）
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
            ' ===== 簡易第三カード規則（段階1）=====
            ' プレイヤー: 初期スコア <= 5 なら3枚目を引く
            ' バンカー:
            '   - プレイヤーが3枚目を引かなかった場合: バンカースコア <= 5 で3枚目を引く
            '   - プレイヤーが3枚目を引いた場合: 簡略版 → バンカースコア <= 6 で3枚目を引く
            ' 注: 厳密なバカラルール（バンカー3枚目の判定はプレイヤーの3枚目カード値に依存）はPhase2以降

            ' プレイヤーの3枚目判定
            If state.PlayerHand.Cards.Count = 2 Then
                Dim playerScore = ComputeScore(state.PlayerHand)
                If playerScore <= 5 Then
                    DrawToHand(state.Shoe, state.PlayerHand, 1)
                End If
            End If

            ' バンカーの3枚目判定
            If state.BankerHand.Cards.Count = 2 Then
                Dim bankerScore = ComputeScore(state.BankerHand)
                Dim drawBanker = False

                If state.PlayerHand.Cards.Count = 2 Then
                    ' プレイヤーが3枚目を引かなかった
                    drawBanker = (bankerScore <= 5)
                Else
                    ' プレイヤーが3枚目を引いた（簡略版：バンカースコア <= 6）
                    drawBanker = (bankerScore <= 6)
                End If

                If drawBanker Then
                    DrawToHand(state.Shoe, state.BankerHand, 1)
                End If
            End If
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
