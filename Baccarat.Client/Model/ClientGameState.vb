Imports Baccarat.Shared.Protocol

Namespace Model
    ''' <summary>
    ''' クライアント側のゲーム状態管理
    ''' </summary>
    ''' <remarks>
    ''' サーバから受信した情報を保持する最小限のデータモデル。
    ''' ロジックを含まず、プロパティのみで構成。
    ''' </remarks>
    Public Class ClientGameState
        Public Property PlayerId As Integer = 0
        Public Property Phase As GamePhase = GamePhase.LOBBY
        Public Property RoundIndex As Integer = 0
        Public Property MaxRounds As Integer = 10
        Public Property ChipsP1 As Integer = 1000
        Public Property ChipsP2 As Integer = 1000
        Public Property LastPlayerCards As String = "" ' "S-01|H-05" 形式
        Public Property LastBankerCards As String = ""
        Public Property LastWinner As Winner = Winner.Tie
        Public Property LastPayoutP1 As Integer = 0
        Public Property LastPayoutP2 As Integer = 0
    End Class
End Namespace
