プロジェクト報告書: Baccarat（現状サマリ）

作成日: 2026-01-12
作成者: 自動生成（GitHub Copilot）

1. 概要
- 名称: 対戦型バカラ（VB.NET / WinForms / Experiment.TcpSocket）
- 構成: 3プロジェクト
  - `Baccarat.Shared` : プロトコル、モデル、ルールインタフェース、共通ユーティリティ
  - `Baccarat.Server` : サーバ実装（簡易UIあり）、通信ハンドリング、ゲーム進行
  - `Baccarat.Client` : クライアント（簡易ロビーUI / 通信部分 / 画像ローダ）
- 目的: ローカルネットワーク上で 2 人対戦のバカラを動作させるプロトタイプを早期に構築する。

2. 要件（主要）
- サーバは TCP 待受けを行い最大2クライアントを受け入れること
- クライアントは接続後 `HELLO,nickname` を送り、サーバは `WELCOME` で playerId を返す
- 両者が `READY` を送ると `PHASE,BETTING,round` が通知され賭けを受け付ける
- `BET`→両者確定で配札→判定→`ROUND_RESULT` → 次ラウンド（または `GAME_OVER`）
- メッセージは UTF-8、1行(LF) 単位で扱う

3. 通信プロトコル（現状確定）
- HELLO: `HELLO,nickname`
- WELCOME: `WELCOME,playerId,seed,maxRounds,initChips`
- READY: `READY`
- PHASE: `PHASE,phase,round`
- BET: `BET,playerId,target,amount` または `BET,target,amount`（両対応）
- BET_ACK: `BET_ACK,ok[,reason]`
- DEAL: `DEAL,playerCodes,bankerCodes`（カードコードは `S-01` などを `|` 区切り）
- ROUND_RESULT: `ROUND_RESULT,winner,payoutP1,payoutP2,chipsP1,chipsP2`
- GAME_OVER: `GAME_OVER,winPlayerId,chipsP1,chipsP2`
- ERROR: `ERROR,reason`（`ROOM_FULL` は送出後にクローズ）

4. 仕様（実装上の重要点）
- 行フレーミング実装（`LineFramer`）で DataReceive の分割/結合対策済
- `GameState` がサーバの単一の真実（Single Source of Truth）
- ルールは `IBaccaratRules` による差し替え可能設計。現状は `BaccaratRulesPlaceholder`（簡易ルール、第三カード省略）
- 配当は `IPayoutCalculator` で計算（プレースホルダ実装あり）
- 山札（Shoe）は `DeckCount` デックで生成・シャッフル。残数が少なければ再生成
- 画像: カード BMP を `Baccarat.Client/assets/cards` に配置しビルド時に出力へコピーする仕組みを用意

5. 現在の実装進捗（重要実装済み）
- 共通 (Baccarat.Shared)
  - `Constants`, `Protocol`（コマンド定義、Message/Parser/Framer）、モデル(`Card`,`Hand`,`ClientInfo`,`GameState`) 実装
  - `BaccaratRulesPlaceholder` と `PayoutCalculatorPlaceholder` を用意
  - `Logger`, `Validate` 等ユーティリティの最小実装
- サーバ (Baccarat.Server)
  - `FormServer`（簡易UI）: `TcpSockets` と `LineFramer` を接続、`ServerHost` と連携
  - `ServerHost` 実装:
    - 接続管理（Accept/Disconnect）
    - `HELLO` 検証 / `WELCOME` 応答
    - `READY` → `PHASE,BETTING` のブロードキャスト
    - `BET` 検証/受理 (`BET_ACK`)、両者ベット確定時に `DEAL`、`ROUND_RESULT` を送信
    - `GAME_OVER` 判定（MaxRoundsまたはチップ枯渇）と通知
    - `ROOM_FULL` 拒否（送信後にクローズ）
    - 山札生成/シャッフル、`DEAL` メッセージにカードコード添付
- クライアント (Baccarat.Client)
  - `FormLobby`（簡易UI）: IP/ニックネーム入力、Connect ボタン、ログ表示
  - `FormLobby.Comms.vb`: `TcpSockets` 初期化、`HELLO` 送信、フレーミング受信、受信ログ化
  - `ImageLoader`（カードBMP読み込み）とビルドコピー設定

6. テストベース（手動で確認済み/想定）
- スモークテスト済み: 2 クライアント接続 → HELLO/WELCOME → 両者 READY → PHASE=BETTING が届く
- BET の正常/異常パス検証（サーバは `BET_ACK` を返す）
- 両者の BET 確定で DEAL/ROUND_RESULT/PHASE(次ラウンド) の一連が発生
- ROOM_FULL と GAME_OVER の通知ロジック確認

7. 未実装・残タスク（優先度付き）
- (P0) クライアント側: 受信メッセージを UI に反映（WELCOME/PHASE/BET_ACK/DEAL/ROUND_RESULT/GAME_OVER）
- (P0) サーバ側: 第三カードルールの完全実装（`ApplyThirdCardRule` の強化）
- (P0) サーバ側: BET の細かいバリデーションとエッジケースの整理（同時操作、タイムアウト）
- (P1) サーバ側: ログ情報の詳細化（ラウンドサマリ、監査ログ）
- (P1) クライアント側: `FormGame` の UI 実装（画像表示、BET UI、Next 等）
- (P1) テスト: 単体テストと統合テストスクリプトの整備
- (P2) 運用: 設定ファイル化（ポート/初期チップ/MaxRounds）、デプロイ手順

8. 開発上の注意点 / 合意事項
- 通信は必ず LF で終端すること（サーバ/クライアントともに送信時に `\n` を付与）
- クライアントは UI 側で予防するが、サーバ側で必ず検証を行う（二重防御）
- すべての外部依存（Experiment.TcpSocket.dll）は `lib/` に置き、プロジェクト参照でコピーされる
- カード画像は後日追加する前提で読み込みロジックとビルド手順を整備済

9. 実行方法（開発者向け簡易手順）
- 依存: Visual Studio / .NET SDK 対応版
- レポジトリルートでソリューションを開き、`Baccarat.Server` を実行してサーバを起動
- `Baccarat.Client` を複数起動し IP と Nickname を入力して Connect
- ログを見てメッセージの送受信を確認

10. 次の推奨アクション（短期）
- Client: 受信パース → ログ以上に最低限の状態保持（playerId, phase, chips）を行う
- Server: 第三カードルール（段階的）を実装し、既存決済ロジックの単体テストを追加
- 共通: ドキュメントにメッセージ事例（サンプルシナリオ）を追記

---

このレポートはワーキングコピーの現状をまとめたものです。詳細は各ソースファイル（`Baccarat.Server/Net/ServerHost.vb`, `Baccarat.Client/Forms/FormLobby.Comms.vb`, `Baccarat.Shared/*`）を参照してください。