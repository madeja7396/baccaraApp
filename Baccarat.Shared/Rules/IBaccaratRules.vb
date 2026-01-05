Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol

Namespace Rules
    Public Interface IBaccaratRules
        Function ComputeScore(hand As Hand) As Integer
        Sub DealInitial(state As GameState)
        Sub ApplyThirdCardRule(state As GameState)
        Function DetermineWinner(state As GameState) As Winner
    End Interface
End Namespace
