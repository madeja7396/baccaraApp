# Experiment.TcpSocket 仕様書（堅牢版・整合性検証込み）

本書は、授業資料に記載された **Experiment.TcpSocket 名前空間**の記述を一次仕様（Normative）として採用し、サンプル実装で必要になる運用上の規約（Informative）を追加して「壊れにくい仕様書」に再設計したものである。

---

## 0. 設計方針（堅牢化ポリシー）

### 0.1 用語
- **Normative（一次仕様）**: 授業資料の記述そのもの。ここに書かれている内容を最優先の事実として扱う。
- **Informative（運用仕様）**: 一次仕様に矛盾しない範囲で、アプリ実装を安定させるための推奨・規約。

### 0.2 整合性の採用ルール
- 文字列の大小・誤字（例: `IsynchronizeInvoke`）は **.NET の実在型**に正規化する。
- 一次仕様で曖昧な表現（例: `Public ReadOnly Property Byte()`）は、
  - 「型＝`Byte()` の受信データプロパティが存在する」ことを Normative で確定
  - 具体名は、実コード側（IntelliSense / コンパイル）で確定するまで **未確定** として扱う
  - 運用上は `e.Data` を推奨名として定義するが、環境によっては別名の可能性を残す

---

## 1. 名前空間: `Experiment.TcpSocket`

### 1.1 構成クラス
- `TcpSockets`（通信コンポーネント本体）
- `AcceptEventArgs`
- `ConnectEventArgs`
- `DisconnectEventArgs`
- `DataReceiveEventArgs`

---

## 2. TcpSockets クラス（Normative）

### 2.1 概要
ソケットベースの TCP 通信を提供するコンポーネント。

### 2.2 主なメソッド

#### 2.2.1 `OpenAsServer(port As Integer) As Long`
- **機能**: クライアントからの接続要求の待機を開始する。
- **引数**: `port`（ローカルポート）
- **戻り値**: `Long`（ハンドル）

#### 2.2.2 `OpenAsClient(hostname As String, port As Integer) As Long`
- **機能**: サーバへの接続を開始する。
- **引数**:
  - `hostname`（リモートホスト）
  - `port`（リモートポート）
- **戻り値**: `Long`（ハンドル）

#### 2.2.3 `Close(handle As Long)`
- **機能**: 接続要求の待機を終了する、または接続を切断する。
- **引数**: `handle`（ハンドル）

#### 2.2.4 `IsOpened(handle As Long) As Boolean`
- **機能**: 接続要求を待機しているか否か、または接続されているか否かを取得する。
- **引数**: `handle`（ハンドル）
- **戻り値**: `Boolean`

#### 2.2.5 `Send(handle As Long, buffer As Byte())`
- **機能**: データを送信する。
- **引数**:
  - `handle`（ハンドル）
  - `buffer`（バイト配列）

#### 2.2.6 `Send(handle As Long, buffer As Byte(), offset As Integer, size As Integer)`
- **機能**: データを送信する（部分送信）。
- **引数**:
  - `handle`（ハンドル）
  - `buffer`（バイト配列）
  - `offset`（バイト配列のオフセット）
  - `size`（送信するバイト数）

### 2.3 プロパティ

#### 2.3.1 `ReceiveBufferSize As Integer`
- **機能**: 受信バッファのサイズ。
- **既定値**: 8192

#### 2.3.2 `SendBufferSize As Integer`
- **機能**: 送信バッファのサイズ。
- **既定値**: 8192

#### 2.3.3 `SynchronizingObject As System.ComponentModel.ISynchronizeInvoke`
- **機能**: イベントハンドラ呼び出しのときに同期をとるコントロールのインスタンス。
- **通常**: この TcpSockets インスタンスが属するフォームを指定する。
- **既定値**: null 参照

> 文字列の正規化: 授業資料の `IsynchronizeInvoke` は .NET 型 `ISynchronizeInvoke` を指すと解釈して正規化する。

### 2.4 イベント

#### 2.4.1 `Accept`
- 新しい接続要求を受け入れたときに発生。

#### 2.4.2 `Connect`
- リモートホストへ接続したときに発生。

#### 2.4.3 `Disconnect`
- リモートホストへの接続が切断したときに発生。

#### 2.4.4 `DataReceive`
- リモートホストからデータを受信したときに発生。

---

## 3. EventArgs クラス（Normative）

### 3.1 `AcceptEventArgs`
- **概要**: Accept イベントデータ。
- **プロパティ**:
  - `ServerHandle As Long`（サーバハンドル）
  - `ClientHandle As Long`（クライアントハンドル）
  - `RemoteEndPoint As System.Net.IPEndPoint`（リモートホストのエンドポイント）

### 3.2 `ConnectEventArgs`
- **概要**: Connect イベントデータ。
- **プロパティ**:
  - `Handle As Long`（ハンドル）
  - `RemoteEndPoint As System.Net.IPEndPoint`（リモートホストのエンドポイント）

### 3.3 `DisconnectEventArgs`
- **概要**: Disconnect イベントデータ。
- **プロパティ**:
  - `Handle As Long`（ハンドル）
  - `RemoteEndPoint As System.Net.IPEndPoint`（リモートホストのエンドポイント）

### 3.4 `DataReceiveEventArgs`
- **概要**: DataReceive イベントデータ。
- **プロパティ**:
  - `Handle As Long`（ハンドル）
  - `RemoteEndPoint As System.Net.IPEndPoint`（リモートホストのエンドポイント）
  - `??? As Byte()`（受信データ）

#### 3.4.1 受信データプロパティ名の扱い（堅牢化）
- 一次仕様は「`Byte()` 型の読み取り専用プロパティが存在する」までを保証している。
- **プロパティ名は資料上欠落しているため未確定**。
- 運用仕様では、サンプル実装で一般的に使用される `Data` を推奨名として採用する。

---

## 4. ハンドル（handle）の意味とライフサイクル（Informative）

### 4.1 ハンドルの種類
- **サーバハンドル**: `OpenAsServer` の戻り値。接続待機（listen）状態を表す。
- **接続ハンドル**:
  - クライアント側: `OpenAsClient` の戻り値
  - サーバ側: `AcceptEventArgs.ClientHandle`

### 4.2 有効性判定
- ハンドルが有効かどうかは `IsOpened(handle)` で確認可能。
- UI では便宜上 `-1` を「無効」を示す値として使用してよい（ライブラリ仕様ではないが運用上便利）。

### 4.3 マルチクライアント運用
- サーバは Accept が複数回発生し得る。
- **ブロードキャストや個別送信を実現するには**、接続ハンドルを `List(Of Long)` 等で保持する。
- 切断時（Disconnect）には `e.Handle` をキーに除去する。

---

## 5. イベント順序と責務（Informative）

### 5.1 典型シーケンス
- サーバ: `OpenAsServer` → Accept(新規接続ごと) → Connect(接続成立) → DataReceive(受信) → Disconnect(切断)
- クライアント: `OpenAsClient` → Connect(接続成立) → DataReceive(受信) → Disconnect(切断)

### 5.2 イベントのスレッド
- `SynchronizingObject` が設定されている場合、イベントはその同期対象（通常フォーム）のスレッドで呼ばれる設計になる。
- 未設定（null）の場合、イベントは UI スレッド以外から呼ばれる可能性があるため、WinForms では `InvokeRequired` パターンを採用する。

---

## 6. データ送受信の実装規約（Informative）

### 6.1 エンコーディング
- 送受信データが文字列の場合は `Encoding` を固定して使用する。
- `Encoding.Default` は OS 依存であるため、授業用途で Shift-JIS を前提とするなら `Encoding.GetEncoding("shift_jis")` を推奨（明示してズレを消す）。

### 6.2 TCP のメッセージ境界（重要な注意）
- TCP はストリームであり、1回の `Send` が1回の `DataReceive` と一致する保証は一般にはない。
- ただし授業用コンポーネントが内部でパケット境界を保つ可能性もある。
- **堅牢化方針**:
  - 簡単さ優先なら「短いテキスト＋実験範囲では 1:1 とみなす」でもよい
  - 破綻しない設計にするなら「区切り文字（例: \n）または長さヘッダ」をアプリ側プロトコルに導入する

---

## 7. 仕様整合性チェック表（今回提示された記述との突合）

| 項目 | 一次仕様の記述 | 本仕様書の扱い | コメント |
|---|---|---|---|
| `OpenAsServer` | あり | 採用 | そのまま |
| `OpenAsClient` | あり | 採用 | そのまま |
| `Close` | あり | 採用 | そのまま |
| `IsOpened` | あり | 採用 | そのまま |
| `Send` 2種 | あり | 採用 | そのまま |
| BufferSize 既定値 | 8192 | 採用 | そのまま |
| `SynchronizingObject` 型 | `IsynchronizeInvoke` | `ISynchronizeInvoke` に正規化 | .NET 実在型へ補正 |
| `RemoteEndPoint` | 全 EventArgs で明記 | 採用 | IP 表示に使用可能 |
| DataReceive の受信データ | `Byte()` とだけ記載 | 型は確定、名前は未確定 | 実コードで確定するまで保留 |

---

## 8. 最小実装テンプレ（Informative）

### 8.1 サーバ（複数クライアント保持）
- `Accept`: `clients.Add(e.ClientHandle)`
- `Disconnect`: `clients.Remove(e.Handle)`
- `Send`: `For Each h In clients : Send(h, bytes)`

### 8.2 クライアント
- `OpenAsClient`
- `Connect` 以降に送信開始（推奨）

---

## 9. 仕様の未確定点と確定手順（最短）

### 9.1 未確定点
- `DataReceiveEventArgs` の受信データプロパティ名（例: `Data` なのか別名なのか）

### 9.2 確定手順
- IDE で `DataReceive` ハンドラ内で `e.` を打って IntelliSense を確認し、プロパティ名を採録する。
- その名称を本仕様書 3.4 に反映し、以後は Normative とする。

## 10 要検証

整合性、検証して 「授業資料の記述を一次仕様として固定」→「曖昧点だけを未確定として隔離」 する形で、仕様書を 堅牢版に再設計してキャンバスに作り直しました（タイトル：Experiment.TcpSocket 仕様書（堅牢版・整合性検証込み））。

今回あなたが貼った仕様（名前空間説明）と突き合わせて、特に危ない点はここです：

整合性の問題点（潰し込み済み）
1) SynchronizingObject の型名が誤字っぽい

資料：System.ComponentModel.IsynchronizeInvoke

.NET 実在型：System.ComponentModel.ISynchronizeInvoke
→ 仕様書では ISynchronizeInvoke に正規化（大小文字の問題ではなく、型名として実在する方に寄せる）。

2) DataReceiveEventArgs の「受信データ」プロパティ名が欠落

資料：Public ReadOnly Property Byte()（型しか書かれてない）
→ “Byte() 型の読み取り専用プロパティが存在する” までは確定
→ ただし プロパティ名は未確定として隔離し、仕様書では ??? As Byte() として明示的に保留にした。

ここを推測で「Dataだ！」と断言すると、環境差で死ぬので、堅牢版は「未確定」を仕様として扱う作りにしてある。

仕様書を堅牢にしたポイント（設計）

Normative / Informative を分離

Normative：授業資料に書いてあることだけ

Informative：実装上の推奨（複数クライアント保持、UI同期、文字コード、TCP境界など）

曖昧点は“未確定”として仕様に隔離し、確定手順（IntelliSenseで e. を見る）まで書いた

「TCPはストリームで境界が保証されない」問題も、透明性重視で 注意 として明記（壊れにくくするなら区切り/長さヘッダを推奨）

次にやるべき“仕様確定”は1点だけ

DataReceiveEventArgs の受信データプロパティ名を確定させること。

最短の確定手順（仕様書にも書いた）：

DataReceive ハンドラで e. を打って IntelliSense を見る

出てきた Byte() のプロパティ名（例：Data なのか Byte0 なのか）を採録

その名前で仕様書の 3.4 を 確定版に更新する

これが確定すると、あなたのチャット/ゲーム実装の土台が一気に安定します。

