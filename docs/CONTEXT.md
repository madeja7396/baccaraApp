# 要件定義書（厳格版）
**対戦型バカラ（VB / Experiment.TcpSocket）**  
作成日: 2025-12-24　版: **v1.3（GAME_OVER/ROOM_FULL 反映）**

---

## 1. 基本情報

### 1.1 アプリケーション情報

| 項目 | 内容 |
|---|---|
| アプリケーション名 | バカラ |
| 開発者名 | （プレースホルダ：氏名を記入） |
| 開発形態 | 共同開発（2名） |

### 1.2 文書の目的と範囲
本書は、Visual Basic（VB）で実装する「対戦型バカラ」ネットワークアプリケーションの要件を、実装可能な粒度で定義する。  
対象は「接続（サーバ/クライアント）」「2人対戦の進行」「ベット、配札、判定、配当」「ログ」「不正入力/例外通知」「ルール参照ウィンドウ」である。  
未確定事項は「予定」または「プレースホルダ」として明示し、定数・設定値で差し替え可能にする。

### 1.3 用語
- **サーバ権威**：ゲーム状態（Phase/配札/判定/チップ）をサーバが唯一の正として保持する方式。
- **Phase**：ゲーム進行状態（LOBBY/BETTING/DEALING/RESULT/GAMEOVER）。
- **ルールウィンドウ**：ゲーム中いつでも参照できるルール表示用フォーム（FormRules）。

---

## 2. システム概要

### 2.1 システム構成
- クライアント-サーバ方式とし、サーバ1台とクライアント2台で対戦する（最大2人）。
- 通信は TCP を用い、Experiment.TcpSocket コンポーネント（TcpSockets）を利用する。

### 2.2 実行環境（開発/動作）
- 開発環境：Visual Studio（VB、Windows Forms）
- 通信部品：Experiment.TcpSocket.dll（TcpSockets コンポーネント）
- ターゲット：.NET Framework（版はプレースホルダ。TcpSocket.dll が参照可能な版を採用）
- 配布：参照の「ローカルにコピーする=True」を必須とする（DLL同梱）。
- スレッド：TcpSockets の SynchronizingObject をフォームに設定し、UIスレッドでイベントを扱う。

### 2.3 制約・前提
- 同時接続は最大2クライアント。3人目は拒否（予定：ERROR,ROOM_FULL 送信後切断）。
- 通信文字列は UTF-8（予定）。コマンドは英大文字、区切りはカンマ、終端は改行。
- バカラ厳密ルール（第三カード規則）と配当率はプレースホルダ可。差し替え可能に実装する。

---

## 3. 開発スケジュール
開発はアジャイル方式（短い反復で統合・デバッグ）で進める。

| 期間 | 目的 | 主要タスク | 成果物/完了条件 |
|---|---|---|---|
| 3週目 | アイデア出し/要件固め | 要求のチェックリスト化、プロトコル確定（仮）、UI骨組み作成、TcpSocket組込み | 接続/切断/ログのスモークテストが通る |
| 4週目 | 実装・デバッグ（反復） | 通信フレーミング、HELLO/WELCOME、Phase制御、ベット/結果、ルールウィンドウ追加 | 2人対戦が一連で成立し、ログで追跡できる |
| 予備 | 調整/資料 | 未確定項目（配当/ラウンド等）の差し替え、異常系強化、発表資料 | デモ手順書と最終動作確認（予定） |

---

## 4. 機能要件・UI要件

### 4.1 機能要件（システム動作）
本節は、ネットワーク対戦として成立するための必須機能を定義する（サーバ権威）。

#### 機能要件一覧

| ID | 要件（shall） | 優先度 | 受入基準 | 備考 |
|---|---|---:|---|---|
| F-001 | サーバは指定ポートで待機開始できなければならない。 | MUST | OpenAsServer(port) が成功し、Accept を受け取れる。 | portはプレースホルダ |
| F-002 | サーバは接続クライアントを最大2人まで受け入れなければならない。 | MUST | 2人目まで接続可能、3人目は拒否される（予定）。 |  |
| F-003 | 接続成立後、クライアントは HELLO,nickname を送信しなければならない。 | MUST | 接続後に HELLO が送信される。 |  |
| F-004 | サーバは nickname が空/規定長超過の場合、接続を拒否し ERROR を返さなければならない。 | MUST | ERROR,INVALID_NAME が送信され、ログに残る。 | 上限はプレースホルダ |
| F-005 | サーバは各クライアントに playerId を割り当て、WELCOME を送信しなければならない。 | MUST | WELCOME,playerId,... が受信できる。 | seed等はプレースホルダ |
| F-006 | 2人が揃い、両者が READY を送信した場合にのみゲームを開始しなければならない。 | MUST | PHASE,BETTING,1 が双方に送信される。 |  |
| F-007 | サーバは Phase を唯一の正として管理し、遷移時に PHASE を全員へ通知しなければならない。 | MUST | Phase変更ごとに PHASE が送信される。 |  |
| F-008 | サーバは Phase=BETTING 以外の BET を拒否しなければならない。 | MUST | BET_ACK,false,PHASE_MISMATCH が返る。 | ACKは簡略化可 |
| F-009 | 両者のベット確定後、サーバは配札・判定・配当計算を行い、結果を全員へ通知しなければならない。 | MUST | DEAL と ROUND_RESULT が双方に届く。 | 判定/配当はプレースホルダ可 |
| F-010 | サーバは所持チップを更新し、更新結果をクライアントへ通知しなければならない。 | MUST | ROUND_RESULT に chipsP1/chipsP2 が含まれる。 |  |
| F-011 | サーバはゲーム終了条件を判定し、満たした場合 GAME_OVER を通知しなければならない。 | SHOULD | GAME_OVER が届き、Phase=GAMEOVER になる。 | 終了条件はプレースホルダ |
| F-012 | サーバ/クライアントは送受信と重要イベントをログへ追記しなければならない。 | MUST | HELLO/PHASE/BET/RESULT がログで追える。 |  |
| F-013 | ルールウィンドウはゲーム中いつでも参照できなければならない。 | MUST | どのPhaseでも FormRules を開閉できる。 |  |
| F-014 | 通信例外や不正入力を検出した場合、利用者へ通知しなければならない。 | MUST | エラーがダイアログまたはログに表示される。 |  |
| F-015 | 切断を検出した場合、相手へ通知し、ゲームを安全に停止しなければならない。 | SHOULD | Disconnect でログが残り、GAMEOVER扱い（予定）となる。 | 予定 |

### 4.2 画面設計（フォーム構成）

| フォーム名 | 役割 | 必須/予定 | 備考 |
|---|---|---|---|
| FormLobby | 接続/待機/切断、IP・ニックネーム入力、ログ表示、（ルール参照：推奨） | 必須 | 「待機」はサーバ専用UIへ分離してもよい（予定） |
| FormGame | ベット入力、カード/結果表示、ラウンド進行、ログ、ルール参照 | 必須 | Phase により操作可能要素を制御する |
| FormRules | ルール参照（いつでも表示可能） | 必須 | 非モーダル、単一インスタンス、Owner=現フォーム |
| FormResult | 試合終了表示、再戦/終了 | 予定 | 最初は FormGame 内で代替可 |

### 4.3 UI要件（不正入力対策を含む）
UI要件は、操作可能タイミングを Phase によって厳格に制御し、不正入力・二重送信・状態不整合を防止する。

#### FormLobby 要件

| ID | 要件（shall） | 優先度 | 受入基準 | 備考 |
|---|---|---:|---|---|
| UI-001 | ニックネーム未入力の場合、接続操作を実行してはならない。 | MUST | txtNickname が空のとき btnConnect が無効、または押下でエラー表示。 |  |
| UI-002 | IP アドレスが形式不正の場合、接続操作を実行してはならない。 | MUST | 形式不正で btnConnect が無効、または押下でエラー表示。 | IPv4想定（予定） |
| UI-003 | 接続状態を lblStatus とログに表示しなければならない。 | MUST | Connected/Waiting/Disconnected が視認できる。 |  |
| UI-004 | 接続中は、IP とニックネームを変更できないようにしなければならない。 | SHOULD | 接続後に txtIp/txtNickname が無効化される。 | 予定 |
| UI-005 | ルールウィンドウをロビーから参照できるようにしてよい。 | MAY | btnRules（またはメニュー）で FormRules が表示できる。 | UI負荷と相談 |

#### FormGame 要件

| ID | 要件（shall） | 優先度 | 受入基準 | 備考 |
|---|---|---:|---|---|
| UI-101 | Phase=BETTING のときのみ、賭け先と賭け額の入力を可能にしなければならない。 | MUST | BETTING以外で grpBet が無効。 |  |
| UI-102 | 賭け額は 1 以上でなければならない。 | MUST | numBetAmount.Min=1。 |  |
| UI-103 | 賭け額は自分の所持チップ以下でなければならない。 | MUST | numBetAmount.Max が MyChips に追随。 |  |
| UI-104 | 賭け確定後は同一ラウンド内でベット内容を変更できないようにしなければならない。 | MUST | btnBetLock 押下後、grpBet が無効。 |  |
| UI-105 | Phase=RESULT のときのみ「次へ」操作を可能にしなければならない。 | MUST | RESULT以外で btnNext が無効。 |  |
| UI-106 | ルールウィンドウはゲーム中いつでも表示できなければならない。 | MUST | btnRules が常時有効で FormRules を表示。 |  |
| UI-107 | ログは時系列に追記され、スクロール可能でなければならない。 | MUST | txtLog に追記され、スクロールできる。 |  |

#### FormRules 要件

| ID | 要件（shall） | 優先度 | 受入基準 | 備考 |
|---|---|---:|---|---|
| UI-201 | ルールウィンドウは非モーダルで表示されなければならない。 | MUST | FormRules.Show() で表示し、ゲーム操作を妨げない。 |  |
| UI-202 | ルールウィンドウは単一インスタンスでなければならない。 | MUST | 開いている場合は Activate()、多重起動しない。 |  |
| UI-203 | ルールウィンドウには「基本」「配当（予定）」「操作手順」を表示しなければならない。 | MUST | タブまたはセクションで3ブロックが存在。 | 配当はプレースホルダ可 |
| UI-204 | ルールウィンドウは現在の Phase とラウンドを表示してよい。 | MAY | Phase/Round が表示される（任意）。 | 任意 |

### 4.4 Visual Studio（ツールボックス）準拠のフォーム実装手順（実装者ガイド）
本節は、Visual Studio のツールボックスを用いて本システムのフォームを実装するための具体手順を示す。実装者が迷いやすいポイント（参照追加、コンポーネント貼り付け、プロパティ設定、イベント接続、ログ表示）をチェックリスト化する。

#### 4.4.1 事前準備：Experiment.TcpSocket の登録と参照設定
1. Experiment.TcpSocket.dll と Experiment.TcpSocket.xml（IntelliSense用）を所定の場所に配置する（学内手順に従う）。
2. Visual Studio の **[ツール]→[ツールボックス アイテムの選択]**（またはツールボックス右クリック→**[アイテムの選択]**）を開く。
3. 「.NET Framework コンポーネント」から Experiment.TcpSocket を追加し、チェックを付けて登録する（以後ツールボックスから貼り付け可能）。
4. プロジェクトの参照に「Experiment.TcpSocket」を追加する（My Project / プロジェクトのプロパティ → 参照）。
5. 参照のプロパティで「ローカルにコピーする（Copy Local / Private）」を True にする（重要：配布・別環境実行時の DLL 不足を防ぐ）。
6. My Project → 「インポートされた名前空間」で Experiment.TcpSocket にチェックを付ける（任意：Imports を省略できる）。

**注意**：Visual Studio の表示が異なる場合は、参照（References）内の「Experiment.TcpSocket」を選択してプロパティ ウィンドウ（F4）を確認する。「ローカルにコピーする」が見当たらない場合は、DLL をソリューション配下（例：lib/）に置き、「参照の追加→参照（Browse）」で直接参照する運用を推奨する。

#### 4.4.2 共通UI実装規約（命名・配置・ログ）
- 命名規約：txt（TextBox）、lbl（Label）、btn（Button）、grp（GroupBox）、rad（RadioButton）、num（NumericUpDown）、tab（TabControl）を接頭辞にする。
- TabIndex：入力の流れ（IP→ニックネーム→接続）に沿って TabIndex を設定する。
- ログTextBox：Multiline=True、ReadOnly=True、ScrollBars=Vertical、WordWrap=False（推奨）。ログ追記は末尾に追加し、必要なら選択位置を末尾へ移動する。
- 初期値：txtIp は 127.0.0.1（既定）とし、ポートは Constants.Port（プレースホルダ）に集約する。
- 操作の有効/無効：Phase や接続状態に応じて Enabled を切り替える（不正入力・二重送信をUIで予防）。

#### 4.4.3 FormLobby の作成手順（接続・待機・ログ）
1. 新規フォームを追加し、名前を FormLobby にする（フォームの Text は「Baccarat - Lobby」など）。
2. ツールボックスから TextBox（txtIp, txtNickname）、Button（btnWait, btnConnect, btnDisconnect）、Label（lblStatus）、ログ用 TextBox（txtLog）を配置する。
3. txtLog に共通規約のプロパティ（Multiline/ReadOnly/ScrollBars）を設定する。
4. ツールボックスから TcpSockets コンポーネントをフォームに貼り付ける（フォーム下部のコンポーネントトレイに配置される）。
5. TcpSockets のプロパティ SynchronizingObject を FormLobby（Me）に設定する（重要：イベントをUIスレッドで受ける）。
6. btnWait で OpenAsServer(Constants.Port) を呼び、待機開始をログに出す（方式Aでサーバ別EXEの場合は ServerForm に同等機能を移す：予定）。
7. btnConnect で OpenAsClient(txtIp.Text, Constants.Port) を呼ぶ。txtNickname が空の場合は接続させない（必須）。
8. Accept/Connect/Disconnect/DataReceive のイベントハンドラを作成し、接続・切断・受信を txtLog に追記する。

**DoD（受入基準）**：サーバ待機→クライアント接続→切断がログに出る。ニックネーム未入力はUIで接続できず、サーバ側でも空HELLOを拒否できる。

#### 4.4.4 FormGame の作成手順（ベット・表示・ルールボタン）
1. 新規フォームを追加し、名前を FormGame にする（Text は「Baccarat - Game」など）。
2. 状態表示（lblRound, lblPhase, lblMyName/lblOppName, lblMyChips/lblOppChips）を上部に配置する。
3. ベット入力を GroupBox（grpBet）で作り、radPlayer/radBanker/radTie、numBetAmount、btnBetLock、lblBetStatus を配置する。
4. テーブル表示（Player/Banker の手札、勝敗、配当）を中央に配置する（初期は文字表示でも可）。**カード画像はBMPファイルで用意しているため、PictureBox等で表示する**（画像ファイルの配置場所・ファイル名規則はプロジェクトに合わせて設定：プレースホルダ）。
5. 操作ボタンとして btnNext（次へ）と btnRules（ルール表示）を配置する。btnRules は常に有効にする。
6. Phase に応じて grpBet と btnNext の Enabled を切り替える（ApplyPhase(phase) のように一括管理する）。
7. ログ用 txtLog を配置し、送受信・状態遷移・エラーを追記する。

**実装メモ**：numBetAmount の Max は MyChips に同期して更新する（UIバリデーション）。サーバ側でも必ず同じ検証を行う（二重防御）。

#### 4.4.5 FormRules の作成手順（いつでも参照できるルールウィンドウ）
1. 新規フォームを追加し、名前を FormRules にする（Text は「Baccarat - Rules」など）。
2. TabControl（tabRules）を配置し、タブ「基本」「配当（予定）」「操作」を作る。各タブ内は Label または ReadOnly の TextBox で説明文を置く（静的で可）。
3. 現在状態の表示（lblNowPhase, lblNowRound など）は任意。実装する場合は FormGame から現在値を渡して更新する。
4. FormGame の btnRules クリックで FormRules を非モーダル（Show）で開く。既に開いている場合は新規作成せず Activate する（多重起動防止）。
5. Owner を FormGame に設定し、FormGame が閉じたら FormRules も閉じる（参照ウィンドウの孤立を防止）。

**DoD**：ゲーム中いつでもルールウィンドウを開閉でき、ベット/次へ等の操作を妨げない。

#### 4.4.6 実装後の確認（ツールボックス/参照/実行フォルダ）
- ビルド後、出力フォルダ（bin\Debug 等）に Experiment.TcpSocket.dll が存在すること。
- TcpSockets の SynchronizingObject がフォーム（Me）になっていること。
- DataReceive はメッセージ境界を保証しないため、受信側で行フレーミング（改行で分割するバッファ）を実装していること。

---

## 5. 論理データ設計

### 5.1 主要データ構造
サーバはゲーム状態を一元管理し、クライアントは表示用のキャッシュのみを持つ。

| 区分 | データ | 型（例） | 説明 |
|---|---|---|---|
| Server | Clients | List\<ClientInfo\> | 接続中クライアント（最大2）。handle, nickname を保持。 |
| Server | Phase | Enum | LOBBY/BETTING/DEALING/RESULT/GAMEOVER。 |
| Server | RoundIndex/MaxRounds | Integer | ラウンド管理（MaxRoundsはプレースホルダ）。 |
| Server | Chips | Dictionary\<PlayerId, Integer\> | 所持チップ。 |
| Server | Bets | Dictionary\<PlayerId, BetInfo\> | ベット内容（target, amount, locked）。 |
| Server | Shoe | List\<Card\> | 山札（デック数はプレースホルダ）。 |
| Server | Hands | List\<Card\> | Player/Banker の手札。 |
| Client | ViewState | Class | サーバから受け取った Phase/Round/Hands/Chips の表示用。 |

---

## 6. 通信プロトコル要件（更新）
通信は TCP 上のテキスト行（LF区切り）。以下は実装で確定した挙動の追記。

### 6.1 コマンド定義（最小セット・確定）

| CMD | 方向 | 形式 | 説明 |
|---|---|---|---|
| HELLO | C→S | HELLO,nickname | サーバが nickname を検証（空/長すぎは拒否）。満席時は `ERROR,ROOM_FULL`（直後にクローズ）。|
| WELCOME | S→C | WELCOME,playerId,seed,maxRounds,initChips | 受付応答。seedはプレースホルダ。|
| READY | C→S | READY | 両者READYで `PHASE,BETTING,round` を両者に通知。|
| PHASE | S→C | PHASE,phase,round | フェーズ遷移通知（BETTING/DEALING/RESULT/GAMEOVER）。|
| BET | C→S | BET,playerId,target,amount または BET,target,amount | サーバは handle から playerId を特定するため、どちらの形式も許容。|
| BET_ACK | S→C | BET_ACK,ok[,reason] | 受理時 `true`、拒否時 `false,reason`（PHASE_MISMATCH/BAD_ARGS/BAD_PLAYER/BAD_TARGET/BAD_AMOUNT/NO_CHIPS/ALREADY_LOCKED）。|
| DEAL | S→C | DEAL | 配札完了通知（詳細表現は今後拡張）。|
| ROUND_RESULT | S→C | ROUND_RESULT,winner,payoutP1,payoutP2,chipsP1,chipsP2 | 勝敗と配当、最新所持金。|
| GAME_OVER | S→C | GAME_OVER,winPlayerId,chipsP1,chipsP2 | MaxRounds 到達またはチップ枯渇時。引き分けは `winPlayerId=0`。|
| ERROR | S→C | ERROR,reason | BAD_FORMAT/UNSUPPORTED/INVALID_NAME/ROOM_FULL など。`ROOM_FULL` の場合は直後に切断。|

### 6.2 フェーズ遷移（確定した最小ルート）
- LOBBY（初期）
  - 両者 HELLO→WELCOME→READY 完了で BETTING(round=1)
- BETTING
  - 双方が BET を確定（Lock）した時点で DEALING
- DEALING
  - 配札/判定/配当後、RESULT
- RESULT
  - GAME_OVER 条件（MaxRounds 到達 or チップ枯渇）なら `GAME_OVER` 送信して終了
  - それ以外は内部リセット（bets/ready）し Round を +1、BETTING(round+1) へ自動遷移

---

## 8. 受入テスト（更新）
- 正常系: 2クライアント→HELLO/WELCOME→両者READY→PHASE=BETTING→両者BET→DEALING→RESULT→（GAME_OVER or 次ラウンド）
- 異常系: Phase外BET、金額不正、所持超過、重複BET は `BET_ACK,false,<reason>` を返すこと。
- 形式不正/未対応コマンド: `ERROR,BAD_FORMAT/UNSUPPORTED`
- 満席時: 3人目の接続は `ERROR,ROOM_FULL` 送信後にサーバ側でクローズされること。

---

注意: DEAL の詳細表現（カード配列）はプレースホルダのまま。今後 `DEAL,playerCards...,bankerCards...` 形式へ拡張予定。
