Imports System

Namespace Protocol
    Public Enum GamePhase
        LOBBY
        BETTING
        DEALING
        RESULT
        GAMEOVER
    End Enum

    Public Enum ConnState
        DISCONNECTED
        CONNECTING
        CONNECTED
        IN_GAME
    End Enum

    Public Enum BetTarget
        Player
        Banker
        Tie
    End Enum

    Public Enum Winner
        Player
        Banker
        Tie
    End Enum

    Public Module CommandNames
        Public Const HELLO As String = "HELLO"
        Public Const WELCOME As String = "WELCOME"
        Public Const READY As String = "READY"
        Public Const PHASE As String = "PHASE"
        Public Const BET As String = "BET"
        Public Const BET_ACK As String = "BET_ACK"
        Public Const DEAL As String = "DEAL"
        Public Const ROUND_RESULT As String = "ROUND_RESULT"
        Public Const GAME_OVER As String = "GAME_OVER"
        Public Const [ERROR] As String = "ERROR"
        Public Const BYE As String = "BYE"
    End Module

    Public Module BetRejectReasons
        Public Const PHASE_MISMATCH As String = "PHASE_MISMATCH"
        Public Const BAD_ARGS As String = "BAD_ARGS"
        Public Const BAD_PLAYER As String = "BAD_PLAYER"
        Public Const BAD_TARGET As String = "BAD_TARGET"
        Public Const BAD_AMOUNT As String = "BAD_AMOUNT"
        Public Const NO_CHIPS As String = "NO_CHIPS"
        Public Const ALREADY_LOCKED As String = "ALREADY_LOCKED"
    End Module
End Namespace
