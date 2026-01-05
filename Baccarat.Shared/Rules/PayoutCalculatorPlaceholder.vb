Imports Baccarat.Shared.Protocol

Namespace Rules
    ' シンプルな配当計算（プレースホルダ）
    Public Class PayoutCalculatorPlaceholder
        Implements IPayoutCalculator

        ' 戻り値: 純増減額（負の場合は損失）
        Public Function CalcPayout(target As BetTarget, amount As Integer, winner As Winner) As Integer Implements IPayoutCalculator.CalcPayout
            If amount <= 0 Then Return 0

            If winner = Winner.Tie Then
                If target = BetTarget.Tie Then
                    Return CInt(amount * 8) ' 仮の倍率
                Else
                    Return -amount
                End If
            End If

            If target = BetTarget.Tie Then
                Return -amount
            End If

            If (winner = Winner.Player AndAlso target = BetTarget.Player) OrElse (winner = Winner.Banker AndAlso target = BetTarget.Banker) Then
                Dim multiplier As Double = If(target = BetTarget.Player, 1.0, 0.95) ' Banker控除を仮置き
                Return CInt(Math.Round(amount * multiplier))
            End If

            Return -amount
        End Function
    End Class
End Namespace
