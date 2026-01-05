# T1（行フレーミング）実装ガイド

> 状態：Framer は完成済み。UI と通信の接続が必要。

## 現状

### ? 実装済
- `Baccarat.Shared.Protocol.LineFramer`：完成
  - `Push(chunk)` で TCP データを受け取る
  - `\n` ごとに行を抽出
  - `LineReceived` イベントを発火

### ? 未実装
- Server/Client の UI（FormServer / FormLobby）で Framer を使う
- DataReceive イベント → `Framer.Push()` → ハンドラ へつなぐ

---

## 実装手順（T1-01 / T1-02 完了）

### Step 1: FormServer で Framer を使う

**FormServer.vb に追加：**

```visualbasic
Imports Experiment.TcpSocket
Imports Baccarat.Shared.Protocol

Namespace Baccarat.Server.Forms
    Public Class FormServer
        ' TcpSocket コンポーネント（デザイナで追加済み）
        ' Private tcpSockets As TcpSockets
        
        ' 行フレーマー（handle ごと）
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
        
        Private Sub TcpSocket_Accept(handle As Integer)
            AppendLog($"[ACCEPT] handle={handle}")
            ' このハンドル用のフレーマーを作成
            _framers(handle) = New LineFramer()
        End Sub
        
        Private Sub TcpSocket_DataReceive(handle As Integer, data As String)
            AppendLog($"[RCV] handle={handle}, {data.Length} bytes")
            
            If Not _framers.ContainsKey(handle) Then
                AppendLog($"[WARN] Unknown handle {handle}")
                Return
            End If
            
            ' Framer に渡して、行に分割させる
            Dim framer = _framers(handle)
            AddHandler framer.LineReceived, Sub(line As String)
                                              OnLineReceived(handle, line)
                                          End Sub
            framer.Push(data)
        End Sub
        
        Private Sub OnLineReceived(handle As Integer, line As String)
            AppendLog($"[LINE] handle={handle}: {line}")
            ' ここで ServerHost.OnLineReceived() を呼ぶ（次のステップ）
        End Sub
        
        Private Sub TcpSocket_Disconnect(handle As Integer)
            AppendLog($"[DISCONNECT] handle={handle}")
            _framers.Remove(handle)
        End Sub
        
        Private Sub AppendLog(message As String)
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}" & vbCrLf)
            txtLog.SelectionStart = txtLog.TextLength
            txtLog.ScrollToCaret()
        End Sub
    End Class
End Namespace
```

### Step 2: 送信時に `\n` を付ける

**ServerHost / Client のどこで送信するときでも：**

```visualbasic
Private Sub SendMessage(handle As Integer, message As String)
    ' 末尾に LF を付けて送信
    tcpSockets.Send(handle, message & vbLf)
    AppendLog($"[SND] {message}")
End Sub
```

---

## 受入基準（TC-003）

- 複数メッセージを一括受信しても、各行が正しく分割される
- 分割受信（1文字ずつ等）でも、`\n` が来て初めて1行として完成する

---

## 次のステップ

T1 が完了したら、T2-01（サーバ待機・ログ）に進む。
