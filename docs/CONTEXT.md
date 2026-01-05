# 要件定義書（厳格版）
**対戦型バカラ（VB / Experiment.TcpSocket）**  
作成日: 2025-12-24　版: **v1.1（実装開始版・ルールウィンドウ反映）**

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

## 6. 通信プロトコル要件
通信は TCP 上のテキスト行（1行=1メッセージ）とし、改行（LF）をメッセージ終端とする。  
DataReceive はメッセージ境界を保証しないため、受信側は行フレーミング（バッファリング）を実装しなければならない。

### プロトコル要件一覧

| ID | 要件（shall） | 優先度 | 受入基準 | 備考 |
|---|---|---:|---|---|
| PR-001 | メッセージは「CMD,param1,param2,...」形式でなければならない。 | MUST | カンマ区切りで解析できる。 |  |
| PR-002 | メッセージ終端は LF（\n）でなければならない。 | MUST | 送信時に必ず末尾に \n が付与される。 | CRLF受信は許容 |
| PR-003 | 受信側は分割/結合受信に耐える行フレーミングを実装しなければならない。 | MUST | 複数行が一括受信/分割受信でも正しく処理される。 |  |
| PR-004 | 未知コマンド、パラメータ不足、型不正を検出した場合、ERROR を返さなければならない。 | MUST | ERROR,reason が送信されログに残る。 |  |
| PR-005 | サーバは受理できない操作に対して理由を返さなければならない。 | SHOULD | BET_ACK,false,reason 等で理由が返る。 | 簡略可 |

### 6.1 コマンド定義（最小セット）

| CMD | 方向 | 形式 | 説明 | 備考 |
|---|---|---|---|---|
| HELLO | C→S | HELLO,nickname | 接続後の自己申告。nicknameは必須。 |  |
| WELCOME | S→C | WELCOME,playerId,seed,maxRounds,initChips | 受付応答と初期値。 | seed等はプレースホルダ |
| READY | C→S | READY | 準備完了通知（全員READYで開始）。 |  |
| PHASE | S→C | PHASE,phase,round | フェーズ遷移通知。 |  |
| BET | C→S | BET,playerId,target,amount | ベット確定。 | playerIdはWELCOMEに従う |
| BET_ACK | S→C | BET_ACK,ok,reason | ベット受理/拒否。 | 予定（簡略可） |
| DEAL | S→C | DEAL,playerCards...,bankerCards... | 配札結果。 | カード表現はプレースホルダ |
| ROUND_RESULT | S→C | ROUND_RESULT,winner,payoutP1,payoutP2,chipsP1,chipsP2 | 勝敗・配当・所持金更新。 | 配当はプレースホルダ可 |
| GAME_OVER | S→C | GAME_OVER,winPlayerId,chipsP1,chipsP2 | 試合終了。 | 予定 |
| ERROR | S→C | ERROR,reason | エラー通知。 |  |
| BYE | C↔S | BYE | 切断通知。 | 予定 |

---

## 7. 非機能要件

| ID | 要件（shall） | 優先度 | 受入基準 | 備考 |
|---|---|---:|---|---|
| NF-001 | UIは通信処理でフリーズしてはならない。 | MUST | 接続中/受信中でもフォーム操作が応答する。 | 重い処理は避ける |
| NF-002 | UI更新は必ずUIスレッド上で行わなければならない。 | MUST | Cross-thread 例外が発生しない。 | SynchronizingObject設定 |
| NF-003 | ログは最低限「時刻（任意）・方向（送/受）・内容」を残さなければならない。 | SHOULD | 送受信が追跡可能。 | 時刻は任意 |
| NF-004 | 不正入力はクライアント側で予防し、サーバ側で必ず検証しなければならない。 | MUST | 改造クライアントでもサーバが破綻しない。 | 二重防御 |
| NF-005 | 他PCへコピーして実行できなければならない。 | MUST | bin 配下に DLL が同梱され、起動できる。 | Copy Local必須 |

---

## 8. 受入テスト（最低限）
受入基準は、要件IDに対応するテストケースの合格をもって満たす。

| TC | 目的 | 手順 | 期待結果 | 対応要件 |
|---|---|---|---|---|
| TC-001 | 接続/ハンドシェイク | サーバ起動→2クライアント接続→HELLO送信 | WELCOME受信、Status/Log更新 | F-001..F-005, UI-001..003 |
| TC-002 | 不正ニックネーム拒否 | nickname空でHELLO送信（改造/手動） | ERROR,INVALID_NAME | F-004, NF-004 |
| TC-003 | 行フレーミング | 複数メッセージを一括送信/分割送信 | 全メッセージが欠落なく処理 | PR-003 |
| TC-004 | Phase制約 | BETTING以外でBET送信 | 拒否（BET_ACK or ERROR） | F-008, UI-101 |
| TC-005 | ルールウィンドウ常時参照 | 全PhaseでRulesを開閉 | 非モーダルで参照でき、ゲーム進行に影響なし | F-013, UI-201..203 |

---

## 付録A. Phaseと許可操作（受入基準に直結）

| Phase | 説明 | クライアント許可操作 | サーバ処理 |
|---|---|---|---|
| LOBBY | 接続待ち | 接続/切断、（ルール参照） | 2人揃うまで待機 |
| BETTING | ベット受付 | ベット入力/確定、（ルール参照） | 全員確定でDEALINGへ |
| DEALING | 配札中 | 操作不可（ルール参照のみ） | 配札・判定・配当計算 |
| RESULT | 結果表示 | 次へ、（ルール参照） | 次ラウンドへ/終了判定 |
| GAMEOVER | 試合終了 | ロビーへ/再戦（予定） | 状態初期化（予定） |

---

## 付録B. プレースホルダ一覧

| 項目 | 現時点の扱い | 確定タイミング |
|---|---|---|
| ポート番号 | プレースホルダ（例: 20000/8888） | 実装開始時に定数化 |
| MaxRounds | プレースホルダ | ゲーム進行実装時 |
| 初期チップ | プレースホルダ | ゲーム進行実装時 |
| デック数/山札 | プレースホルダ | 配札実装時 |
| カード表現（DEAL） | プレースホルダ（例: S-13） | DEAL実装時 |
| 配当率 | 予定（ルールタブで明示） | 判定/配当実装時 |
| FormResult | 予定（省略可） | 完成度調整時 |
| 切断時の勝敗扱い | 予定 | 異常系実装時 |
