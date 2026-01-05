```md
# 詳細設計書（DD: Detailed Design）
**対戦型バカラ（VB / Experiment.TcpSocket）**  
版: v1.0（詳細設計ドラフト）  
作成日: 2026-01-06  
対象: 要件定義書 v1.1（ルールウィンドウ反映）に基づく実装

---

## 0. 目的・設計方針

### 0.1 目的
本書は、VB（WinForms）+ Experiment.TcpSocket を用いた「2人対戦型バカラ」を、**実装がそのまま開始できる粒度**で詳細設計する。

### 0.2 設計方針（重要）
- **サーバ権威（Server Authoritative）**：Phase/配札/判定/チップはサーバが唯一の正
- **UIは状態機械に従う**：UI制御は `ApplyPhase()` を経由（Enabled/Visible を集中管理）
- **通信は行（LF）単位**：TcpSockets.DataReceive は境界保証なし → **行フレーミング必須**
- **二重防御**：クライアントで予防、サーバで必ず検証
- **未確定はプレースホルダ**：定数化し差し替え可能（Port/MaxRounds/配当/第三カード規則など）

---

## 1. 全体アーキテクチャ

### 1.1 コンポーネント構成（推奨ソリューション）
- `Baccarat.Server`（WinForms）
- `Baccarat.Client`（WinForms）
- `Baccarat.Shared`（Class Library）

> 共同開発で衝突を減らすため、**プロトコル/データ/共通関数は Shared に集約**。

### 1.2 依存関係
- Server → Shared, Experiment.TcpSocket
- Client → Shared, Experiment.TcpSocket
- Shared → 追加依存なし（純VB）

---

## 2. ディレクトリ/名前空間設計

### 2.1 物理構成（例）

```
src/
Baccarat.Shared/
Constants.vb
Protocol/
Commands.vb
Message.vb
Parser.vb
Framer.vb
Model/
Card.vb
Hand.vb
BetInfo.vb
ClientInfo.vb
GameState.vb
Rules/
IBaccaratRules.vb
BaccaratRulesPlaceholder.vb
PayoutCalculatorPlaceholder.vb
Util/
Logger.vb
Validate.vb

Baccarat.Server/
Forms/
FormServer.vb (or FormLobby兼用)
Net/
ServerHost.vb
ClientSession.vb

Baccarat.Client/
Forms/
FormLobby.vb
FormGame.vb
FormRules.vb
UI/
UiBinder.vb
ImageLoader.vb
assets/
cards/   (BMP置き場)
docs/
REQUIREMENTS.md
PROTOCOL.md

```

### 2.2 名前空間（例）
- `Baccarat.Shared.*`
- `Baccarat.Server.*`
- `Baccarat.Client.*`

---

## 3. 状態機械設計

### 3.1 ゲームPhase（サーバが唯一管理）
`Enum GamePhase`
- `LOBBY`
- `BETTING`
- `DEALING`
- `RESULT`
- `GAMEOVER`

**遷移**
- LOBBY → BETTING：2人揃う & 両者READY
- BETTING → DEALING：両者BET確定
- DEALING → RESULT：配札・判定・配当計算完了
- RESULT → BETTING：次ラウンド（継続条件）
- RESULT → GAMEOVER：終了条件達成（プレースホルダ）
- GAMEOVER → LOBBY：再戦/戻る（予定）

### 3.2 接続状態（クライアント側）
`Enum ConnState`
- `DISCONNECTED`
- `CONNECTING`
- `CONNECTED`
- `IN_GAME`

UIの有効/無効は `ConnState` と `GamePhase` の組で決定する。

---

## 4. 通信設計（プロトコル詳細）

### 4.1 文字列仕様
- エンコーディング：UTF-8（予定）
- 形式：`CMD,param1,param2,...\n`
- CMD：英大文字
- 区切り：`,`（カンマ）
- 終端：LF（`\n`）  
  受信側は CRLF も許容（`\r`をトリム）

### 4.2 行フレーミング（必須）
TcpSockets.DataReceive は「分割/結合」されるため、受信バッファで復元する。

**Framer仕様**
- 内部に `StringBuilder buffer`
- `OnData(chunk As String)` で append
- `\n` を含む間、1行ずつ取り出して `OnLine(line)` に渡す
- `line` 末尾の `\r` は除去

### 4.3 コマンド（最小セット）と責務
- `HELLO`：C→S（nickname必須）
- `WELCOME`：S→C（playerId/初期値）
- `READY`：C→S（両者揃いで開始）
- `PHASE`：S→C（phase/round）
- `BET`：C→S（phase=BETTINGのみ受理）
- `BET_ACK`：S→C（ok/理由）※簡略化可
- `DEAL`：S→C（配札内容）※カード表現はプレースホルダ
- `ROUND_RESULT`：S→C（winner/payout/chips）
- `GAME_OVER`：S→C（勝者/最終chips）※予定
- `ERROR`：S→C（理由）
- `BYE`：C↔S（予定）

### 4.4 パース規約（共通）
- `Split(","c)` で配列化（最低要素数チェック）
- 型変換は `Integer.TryParse` 等で厳格
- 不正形式は `ERROR,BAD_FORMAT`（サーバ→当該クライアント）

---

## 5. データモデル詳細（Shared）

### 5.1 定数（Constants）
- `Port As Integer = <プレースホルダ>`
- `NicknameMaxLen As Integer = <プレースホルダ>`
- `InitChips As Integer = <プレースホルダ>`
- `MaxRounds As Integer = <プレースホルダ>`
- `DeckCount As Integer = <プレースホルダ>`

### 5.2 モデル
#### ClientInfo
- `Handle`（TcpSocketsが返す接続識別子相当：型は実装に合わせる）
- `PlayerId As Integer`（1 or 2）
- `Nickname As String`
- `IsReady As Boolean`

#### BetInfo
- `Target As BetTarget`（Player/Banker/Tie）
- `Amount As Integer`
- `Locked As Boolean`

#### Card
- `Suit As Char`（S/H/D/C）
- `Rank As Integer`（1..13）
- `ToCode()`：`S-13` 形式（プレースホルダ仕様）

#### Hand
- `Cards As List(Of Card)`
- `Score()`：バカラ点（mod10）※第三カード規則は後述

#### GameState（サーバ保持）
- `Phase As GamePhase`
- `RoundIndex As Integer`
- `Clients(1..2) As ClientInfo`
- `Chips(playerId) As Integer`
- `Bets(playerId) As BetInfo`
- `Shoe As List(Of Card)`
- `PlayerHand As Hand`
- `BankerHand As Hand`

---

## 6. ルール・配当（差し替え可能設計）

### 6.1 インタフェース（差し替え前提）
`Interface IBaccaratRules`
- `Function ComputeScore(hand As Hand) As Integer`
- `Sub DealInitial(state As GameState)`  
- `Sub ApplyThirdCardRule(state As GameState)`（プレースホルダ可）
- `Function DetermineWinner(state As GameState) As Winner`（Player/Banker/Tie）

`Interface IPayoutCalculator`
- `Function CalcPayout(target As BetTarget, amount As Integer, winner As Winner) As Integer`  
  （±の増減 or 純増分、どちらに統一するかは実装で決めて固定）

### 6.2 初期実装（段階的）
- **段階1（実装開始用）**：第三カード規則なし（初期2枚だけで勝敗）
- **段階2（完成版）**：第三カード規則を実装（要件が許す範囲）
- 配当率：プレースホルダ（例：Player=2倍、Banker=1.95倍、Tie=8倍 等）

> ルールウィンドウには「配当（予定）」として明示。

---

## 7. サーバ詳細設計

### 7.1 サーバの責務
- クライアント最大2人受付・3人目拒否（予定）
- HELLO/READY 受付、nickname検証
- Phase管理と通知（PHASE）
- BETの検証・受理、両者確定でDEALINGへ
- 配札・判定・配当・チップ更新
- 異常系（不正入力/例外/切断）の検知と安全停止

### 7.2 ServerHost（中核）
**主要メソッド**
- `StartServer(port)`
- `StopServer()`
- `OnAccept(handle)`
- `OnDisconnect(handle)`
- `OnLineReceived(handle, line)`
- `Broadcast(line)`
- `SendTo(handle, line)`

**検証関数**
- `ValidateHello(nickname) -> ok/reason`
- `ValidateBet(handle, bet) -> ok/reason`
- `ValidatePhase(expectedPhase)`

### 7.3 3人目拒否（予定仕様）
- Accept時に `activeCount >= 2` なら
  - `SendTo(handle, "ERROR,ROOM_FULL\n")`
  - 直後に切断（TcpSocketsのAPIに合わせる）

### 7.4 切断時処理（予定）
- `Disconnect` 検知
- 相手に `ERROR,OPP_DISCONNECTED` を通知
- `Phase=GAMEOVER` 相当へ遷移（ゲーム停止）

---

## 8. クライアント詳細設計

### 8.1 クライアントの責務
- Lobbyで接続・切断・nickname必須入力
- 接続後に HELLO 送信
- WELCOME受信→playerId保存→READY送信
- PHASEに従いUIを制御
- BET確定送信（Phase=BETTINGのみUI許可）
- DEAL/RESULT表示
- ルールウィンドウをいつでも表示（非モーダル・単一インスタンス）

### 8.2 FormLobby（UI + Controller）
**イベント**
- `btnConnect_Click`
- `btnDisconnect_Click`
- `btnWait_Click`（方式によりServer専用なら省略）
- TcpSockets：`Connect/Disconnect/DataReceive`

**処理概要**
- 接続前検証：nickname空、IP形式不正を弾く
- 接続成功 → `HELLO,nickname\n`
- ログ追記：送受信/状態遷移

### 8.3 FormGame
**UI更新関数**
- `ApplyPhase(phase)`
- `UpdateStatus(round, chips, names)`
- `UpdateHands(playerCards, bankerCards)`（BMP表示）
- `UpdateResult(winner, payout, chips)`

**入力**
- `btnBetLock_Click`：BET送信
- `btnNext_Click`：次へ（クライアント→サーバに通知する場合は `NEXT` を追加するか、サーバ主導で自動遷移にするかを決める：プレースホルダ）
- `btnRules_Click`：FormRules表示（常時有効）

### 8.4 FormRules（単一インスタンス）
- 非モーダル `Show()`
- 既存インスタンスがあれば `Activate()`
- OwnerはFormGame（または呼び出し元）に設定し、親が閉じたら閉じる

---

## 9. UI詳細（コントロール設計：ツールボックス対応）

### 9.1 共通命名規約
- `txt*`, `lbl*`, `btn*`, `grp*`, `rad*`, `num*`, `tab*`, `pic*`

### 9.2 FormLobby（配置要素）
- `txtIp`（初期値 127.0.0.1）
- `txtNickname`（必須）
- `btnWait`（任意）
- `btnConnect`
- `btnDisconnect`
- `lblStatus`
- `txtLog`（Multiline/ReadOnly/VerticalScroll/WordWrap=False）
- `TcpSocketsLobby`（コンポーネントトレイ）
  - `SynchronizingObject = Me`

### 9.3 FormGame（配置要素）
- 状態：`lblRound`, `lblPhase`, `lblMyName`, `lblOppName`, `lblMyChips`, `lblOppChips`
- ベット：`grpBet`, `radPlayer`, `radBanker`, `radTie`, `numBetAmount`, `btnBetLock`, `lblBetStatus`
- 表示：`picPlayer1`, `picPlayer2`, `picBanker1`, `picBanker2`, （第三カード用は `picPlayer3`, `picBanker3` を予定で用意可）
- 結果：`lblWinner`, `lblPayout`
- 操作：`btnNext`, `btnRules`
- ログ：`txtLog`
- `TcpSocketsGame`（コンポーネントトレイ）
  - `SynchronizingObject = Me`

### 9.4 FormRules（配置要素）
- `tabRules`：タブ「基本」「配当（予定）」「操作」
- 各タブ内：`txtRulesBasic`, `txtRulesPayout`, `txtRulesHowto`（ReadOnly推奨）
- 任意：`lblNowPhase`, `lblNowRound`
- `btnClose`

---

## 10. BMPカード画像設計

### 10.1 画像配置
- `assets/cards/*.bmp` を想定（配置場所はプレースホルダ）
- 参照方法（いずれか）
  1. 実行フォルダ相対パス（`.\assets\cards\`）
  2. プロジェクトにContentとして同梱（Copy to Output Directory）

### 10.2 ファイル命名規則（推奨・プレースホルダ）
- 例：`S_01.bmp`（Spade Ace）
- `Suit_{Rank:00}.bmp`（S/H/D/C、01..13）
- 裏面：`BACK.bmp`

### 10.3 画像ローダ
`ImageLoader.GetCardBitmap(cardCode) -> Bitmap`
- 例外時：`BACK.bmp` を返す or null（ログに残す）

---

## 11. ロギング設計

### 11.1 形式
`[HH:mm:ss] [SND/RCV] [handle/playerId] message`
- 送信：`SND`
- 受信：`RCV`
- 重要イベント：`EVT`
- エラー：`ERR`

### 11.2 出力先
- `txtLog` に追記（UI）
- 可能ならファイルログ（予定）

---

## 12. エラーハンドリング設計

### 12.1 サーバ側
- 不正入力：`ERROR,<reason>` または `BET_ACK,false,<reason>`
- 例外：ログに残し、必要なら当該クライアント切断
- 切断：相手に通知しゲーム停止（予定）

### 12.2 クライアント側
- 受信ERROR：ログ + ダイアログ（要件に合わせる）
- Phase不整合：UI側で操作不可、受信側はサーバ指示に従って復帰

---

## 13. 実装順（最短で動く道筋）

### Sprint 0（通信スモーク）
1. Server：OpenAsServer / Accept / Disconnect / ログ
2. Client：OpenAsClient / Connect / Disconnect / ログ
3. 行フレーミング（Shared）

### Sprint 1（HELLO/WELCOME/READY）
4. HELLO送信・nickname検証
5. WELCOME応答・playerId付与
6. READYで開始、PHASE通知

### Sprint 2（BET～RESULT最小）
7. BET受理（Phase制約）
8. 配札（2枚）→勝敗→配当（プレースホルダ）→ROUND_RESULT

### Sprint 3（ルール窓・BMP表示）
9. FormRules（非モーダル/単一）
10. PictureBoxでBMPカード表示

---

## 14. 受入条件（DoD: Doneの定義）
- 2クライアント接続 → HELLO → WELCOME → READY → BETTING開始
- BET送信→RESULT表示→次ラウンドへ（または終了）
- ルールウィンドウが全Phaseで開閉可能
- ログで送受信とイベントが追跡可能
- bin配下に Experiment.TcpSocket.dll が同梱され、別PCでも起動可能（Copy Local）

---

## 付録：最小コード断片（実装の骨）

### A. 行フレーミング（Shared / Framer）
```vb
Public Class LineFramer
    Private ReadOnly _buf As New System.Text.StringBuilder()

    Public Event LineReceived(line As String)

    Public Sub Push(chunk As String)
        If String.IsNullOrEmpty(chunk) Then Return
        _buf.Append(chunk)

        While True
            Dim s As String = _buf.ToString()
            Dim idx As Integer = s.IndexOf(vbLf, StringComparison.Ordinal)
            If idx < 0 Then Exit While

            Dim line As String = s.Substring(0, idx).TrimEnd(ChrW(13)) ' remove CR
            _buf.Remove(0, idx + 1)
            RaiseEvent LineReceived(line)
        End While
    End Sub
End Class
````

### B. UIのPhase適用（Client / FormGame）

```vb
Private Sub ApplyPhase(p As GamePhase)
    grpBet.Enabled = (p = GamePhase.BETTING)
    btnNext.Enabled = (p = GamePhase.RESULT)
    btnRules.Enabled = True
End Sub
```

---
```

この「詳細設計」は、次にやるべき実装をほぼ直結させています。  
次の一手として最も効率がいいのは、**Sharedに「Protocol + Framer + Logger + Constants」だけ先に実装**して、Server/Clientはそれを呼ぶ形に固定することです。
```
