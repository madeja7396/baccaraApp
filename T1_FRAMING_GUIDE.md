# T1 フレーミング実装ガイド

> 前提：Framer は実装済み。UI と通信の接続が必要。

## 概要

### 1. 役割
- `Baccarat.Shared.Protocol.LineFramer`：行分割
  - `Push(chunk)` で TCP データを受け取る
  - `\n` ごとに行を抽出
  - `LineReceived` イベントを発火

### 2. 実装箇所
- Server/Client の UI（FormServer / FormLobby）で Framer を使う
- DataReceive イベント → `Framer.Push()` → ハンドラ へつなぐ

---

## 実装手順（T1-01 / T1-02 相当）

### Step 1: FormServer で Framer を使う

**FormServer.vb に追加：**

```visualbasic
Imports Experiment.TcpSocket
Imports Baccarat.Shared.Protocol
Imports System.Text

Namespace Forms
    Public Class FormServer
        ' TcpSocket コンポーネント（デザイナで追加済み）
        ' Private tcpSockets As TcpSockets
        
        ' クライアントごとのフレーマー（Key: Handle）
        Private _framers As New Dictionary(Of Integer, LineFramer)
        
        Private Sub FormServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ' TcpSocket プロパティ設定（重要）
            ' tcpSockets.SynchronizingObject = Me
            
            ' イベント接続
            AddHandler tcpSockets.Accept, AddressOf TcpSocket_Accept
            AddHandler tcpSockets.DataReceive, AddressOf TcpSocket_DataReceive
            AddHandler tcpSockets.Disconnect, AddressOf TcpSocket_Disconnect
            
            ' サーバ起動
            Try
                tcpSockets.OpenAsServer(9000)  ' ポートはプレースホルダ
                AppendLog("Server listening on port 9000")
            Catch ex As Exception
                AppendLog($"Server startup failed: {ex.Message}")
            End Try
        End Sub
        
        Private Sub TcpSocket_Accept(sender As Object, e As AcceptEventArgs)
            AppendLog($"[ACCEPT] handle={e.ClientHandle}")
            ' このハンドル用のフレーマーを作成
            _framers(e.ClientHandle) = New LineFramer()
        End Sub
        
        Private Sub TcpSocket_DataReceive(sender As Object, e As DataReceiveEventArgs)
            ' 注意: e.Data (仮称) は Byte() 型
            ' 仕様書 exprement.md に従い、プロパティ名を IntelliSense で確認すること

            ' 仮に e.Data として実装
            Dim text As String = Encoding.UTF8.GetString(e.Data)
            AppendLog($"[RCV] handle={e.Handle}, {e.Data.Length} bytes")
            
            If Not _framers.ContainsKey(e.Handle) Then
                AppendLog($"[WARN] Unknown handle {e.Handle}")
                Return
            End If
            
            ' Framer に流して、行に分割させる
            Dim framer = _framers(e.Handle)
            
            ' 初回のみイベントハンドラをつける（または Framer 作成時につける）
            ' ここでは簡略化のため毎回追加しないように注意が必要
            ' → Accept時にハンドラをつけるのがベスト
        End Sub
        
        ' 修正版 Accept
        ' Private Sub TcpSocket_Accept(sender As Object, e As AcceptEventArgs)
        '     Dim framer = New LineFramer()
        '     AddHandler framer.LineReceived, Sub(line) OnLineReceived(e.ClientHandle, line)
        '     _framers(e.ClientHandle) = framer
        ' End Sub
        
        Private Sub OnLineReceived(handle As Integer, line As String)
            AppendLog($"[LINE] handle={handle}: {line}")
            ' ここから ServerHost.OnLineReceived() を呼ぶ（次のステップ）
        End Sub
        
        Private Sub TcpSocket_Disconnect(sender As Object, e As DisconnectEventArgs)
            AppendLog($"[DISCONNECT] handle={e.Handle}")
            _framers.Remove(e.Handle)
        End Sub
        
        Private Sub AppendLog(message As String)
            If txtLog.InvokeRequired Then
                txtLog.Invoke(Sub() AppendLog(message))
                Return
            End If
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}" & vbCrLf)
        End Sub
    End Class
End Namespace
```

### Step 2: 送信時に `\n` をつける

**ServerHost / Client のどこで送信するとしても：**

```visualbasic
Private Sub SendMessage(handle As Integer, message As String)
    ' 末尾に LF をつけて送信
    Dim data As Byte() = Encoding.UTF8.GetBytes(message & vbLf)
    tcpSockets.Send(handle, data)
    AppendLog($"[SND] {message}")
End Sub
```

---

## 完了基準（TC-003）

- 短いメッセージを連打しても、まとめて送信しても
- 受信側（1行処理）では、`\n` 区切りで正しく1行として復元される

---

## 次のステップ

T1 が完了したら、T2-01（サーバ待機・ログ）に進む。

```