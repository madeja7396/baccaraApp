Imports System.Collections.Generic
Imports Protocol

Namespace Model
    ''' <summary>
    ''' ゲームの状態を一元管理するクラス
    ''' </summary>
    ''' <remarks>
    ''' 【バックエンド開発者向け実装ガイド】
    ''' 
    ''' - このクラスのインスタンスは ServerHost が保持し、"唯一の正解 (Single Source of Truth)" となります。
    ''' - クライアントには、この状態の一部または計算結果(勝敗など)がメッセージとして送信されます。
    ''' 
    ''' [主要プロパティ]
    ''' - Phase: 現在の進行状況 (LOBBY -> BETTING -> DEALING -> RESULT)。
    ''' - Clients: 接続中のプレイヤー情報 (最大2名)。
    ''' - Chips: 各プレイヤーの所持チップ。サーバー側で厳密に計算・更新します。
    ''' - Bets: 現在のラウンドでのベット状況。Phase=BETTING 終了時に確定(Lock)します。
    ''' - Shoe: 山札。カードを配るたびに減っていきます。
    ''' </remarks>
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
