Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol
Imports Baccarat.Shared.Rules
Imports Baccarat.Shared.Util

Namespace Net
    ''' <summary>
    ''' サーバー側のメイン制御クラス
    ''' </summary>
    ''' <remarks>
    ''' 【バックエンド開発者向け実装ガイド】
    ''' 
    ''' このクラスは、以下の役割を担います：
    ''' 1. TCP接続の管理 (Experiment.TcpSocket.Server のラッパー)
    ''' 2. クライアントからのコマンド受信と振り分け (HandleHello, HandleBet 等)
    ''' 3. ゲーム進行 (Phase) の管理と、全クライアントへの状態通知 (Broadcast)
    ''' 
    ''' [実装の流れ]
    ''' - StartServer: 指定ポートでリッスン開始。
    ''' - OnAccept: クライアント接続を受け入れ、一時リストに追加。
    ''' - OnLineReceived: 受信した文字列を Parser で解析し、コマンドごとにメソッドを呼び出す。
    ''' - HandleXxx: 各コマンドのロジックを実装。バリデーション(フェーズ確認、チップ残高確認)はここで行う。
    ''' - Broadcast: 状態変化があれば、接続中の全クライアントに新しい情報を送信する。
    ''' </remarks>
    Public Class ServerHost
        Private ReadOnly _logger As Logger
        Private ReadOnly _rules As IBaccaratRules
        Private ReadOnly _payout As IPayoutCalculator
        Private ReadOnly _state As GameState = New GameState()

        Public Sub New(logger As Logger, rules As IBaccaratRules, payout As IPayoutCalculator)
            _logger = logger
            _rules = rules
            _payout = payout
        End Sub

        Public Sub StartServer(port As Integer)
            _logger.Info($"Server start on {port}")
            ' TODO: TcpSockets.OpenAsServer(port)
        End Sub

        Public Sub StopServer()
            _logger.Info("Server stop")
            ' TODO: close sockets and cleanup
        End Sub

        Public Sub OnAccept(handle As Integer)
            _logger.Info($"Accept handle={handle}")
            ' TODO: enqueue until HELLO arrives
        End Sub

        Public Sub OnDisconnect(handle As Integer)
            _logger.Info($"Disconnect handle={handle}")
            ' TODO: notify opponent & transition to GAMEOVER
        End Sub

        Public Sub OnLineReceived(handle As Integer, line As String)
            _logger.Receive(handle.ToString(), line)
            Dim msg As Message = Nothing
            If Not Parser.TryParse(line, msg) Then
                _logger.Error("Bad format")
                Return
            End If

            Select Case msg.Command
                Case CommandNames.HELLO : HandleHello(handle, msg)
                Case CommandNames.READY : HandleReady(handle)
                Case CommandNames.BET : HandleBet(handle, msg)
                Case Else
                    _logger.Error($"Unsupported command {msg.Command}")
            End Select
        End Sub

        Private Sub HandleHello(handle As Integer, msg As Message)
            ' TODO: validate nickname and assign player id
        End Sub

        Private Sub HandleReady(handle As Integer)
            ' TODO: set ready flag and maybe advance phase
        End Sub

        Private Sub HandleBet(handle As Integer, msg As Message)
            ' TODO: validate phase and bet amount
        End Sub
    End Class
End Namespace
