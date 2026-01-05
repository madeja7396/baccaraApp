Imports Baccarat.Shared.Protocol

Namespace Rules
    Public Interface IPayoutCalculator
        Function CalcPayout(target As BetTarget, amount As Integer, winner As Winner) As Integer
    End Interface
End Namespace
