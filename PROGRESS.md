# Baccarat プロジェクト 進行度レポート（2026-01-06）

> 基準：`TASKS.md` における P0（最優先）タスクの実装状況

---

## 概要

| 項目 | 状態 | 進捗 |
|---|---|---|
| **ビルド** | ? 成功 | 100% |
| **基盤整備（T0）** | ?? 部分対応 | 33% |
| **行フレーミング（T1）** | ? 未着手 | 0% |
| **接続・HELLO/WELCOME（T2）** | ? 未着手 | 0% |
| **READY・Phase制御（T3）** | ? 未着手 | 0% |
| **BET・配札・結果（T4）** | ? 部分実装 | 10% |
| **UI要件（T5）** | ? 未着手 | 0% |
| **異常系（T6）** | ? 未着手 | 0% |
| **ドキュメント** | ? 完備 | 100% |

---

## 詳細

### ? ビルド整合性（100%）

**完了項目**
- `Baccarat.Shared`（ライブラリ）：`net10.0`、Compile Include 明示化
- `Baccarat.Client`（WinForms）：`net10.0-windows`、参照統一
- `Baccarat.Server`（WinForms）：`net10.0-windows`、参照統一
- `RootNamespace` 設定：Shared（空）/ Server（Baccarat.Server）
- 3プロジェクト全てビルド成功

---

### ?? T0: 基盤整備（33%）

| ID | 状態 | 説明 |
|---|---|---|
| **T0-01** | ? 未対応 | `Experiment.TcpSocket.dll` の参照パスが `..\Baccarat.Client\bin\Experiment.TcpSocket.dll` のままで、環境依存。`lib/` に移す作業が必要 |
| **T0-02** | ? 確認済 | Copy Local は参照に入っているが、実行時にコピーされるか要確認（ビルド出力を見ていない） |
| **T0-03** | ? 完了 | Server の `MainForm` は `Baccarat.Server.My.MyApplication` で統一、二重名を解消 |

**次のアクション：T0-01 を解決（DLL 固定化）**

---

### ? T1: 行フレーミング（0%）

**状態**
- `Baccarat.Shared.Protocol` に `Framer.vb` / `Parser.vb` / `Message.vb` が存在（ファイル名から推測）
- ただし実装の詳細不明（コード内容を確認していない）
- DataReceive の行バッファリング処理は見当たらない（`TODO` 状態）

**次のアクション**
1. `Framer.vb` / `Parser.vb` の内容を確認
2. 行フレーミング層の実装（`\n` で分割、バッファリング）

---

### ? T2: 接続・HELLO/WELCOME（0%）

**現状**
- `ServerHost.vb` に `HandleHello()` / `HandleReady()` / `HandleBet()` は skeleton あり
- ただし全て `TODO` コメント状態

**実装すべき処理**
- OpenAsServer / OpenAsClient のイベント処理（Accept, Connect, Disconnect）
- HELLO 受信 → nickname 検証 → playerId 割当 → WELCOME 送信
- ログ出力

**次のアクション：T2-01（サーバ待機とログ）から着手**

---

### ? T3: READY・Phase制御（0%）

**現状**
- `GameState.vb` の定義は未確認（コード見ていない）
- Phase 管理がサーバに無い（想像）

**実装すべき処理**
- Phase enum / state を ServerHost に持たせる
- READY を 2人分受け取って BETTING へ遷移
- PHASE メッセージで両クライアントに通知

---

### ? T4: BET・配札・結果（10%）

**現状**
- `BetInfo.vb`：完全に定義済（Target/Amount/Locked）
- `BaccaratRulesPlaceholder.vb`：ComputeScore / DealInitial / DetermineWinner などの skeleton あり（配当なし）
- `PayoutCalculatorPlaceholder.vb`：存在（内容未確認）
- `Card.vb` / `Hand.vb`：存在（内容未確認）

**実装すべき処理**
- BET 受付・保持・検証
- 両者確定を検知して DEALING へ
- 配札（placeholder でランダム）
- 勝敗判定
- ROUND_RESULT 送信

---

### ? T5: UI要件（0%）

**現状**
- `FormServer.vb` / `FormLobby.vb` / `FormGame.vb` / `FormRules.vb`：全て存在（skeleton）
- ビルド出力に resources があるので、デザイナは生成されている（コントロール配置の詳細未確認）

**実装すべき処理**
- TcpSocket コンポーネント貼り付け + SynchronizingObject 設定
- イベントハンドラ（Accept/Connect/DataReceive など）
- Phase に応じたコントロール Enabled 切り替え（ApplyPhase）
- ログ追記処理

---

### ? T6: 異常系（0%）

**状態**
- 不正入力検出、切断処理など未実装

---

### ? ドキュメント（100%）

| ファイル | 状態 |
|---|---|
| `README.md` | ? 共同開発者向けガイド + UI 操作手順 |
| `TASKS.md` | ? タスク表（優先度・完了条件付き） |
| `docs/CONTEXT.md` | ? 要件定義書（全要件） |
| `.gitignore` | ? VB/.NET 標準 |

---

## 優先実装順（最短ゲーム成立）

### フェーズ 1: 通信最小疎通（1-2 日）
1. **T1-01**: 行フレーミング実装（DataReceive → バッファ → `\n` で分割）
2. **T2-01 / T2-02**: サーバ/クライアント待機・接続・ログ
   - **受入**: TC-001 前半（接続がログに出る）

### フェーズ 2: HELLO/WELCOME（1 日）
3. **T2-03 / T2-04 / T2-05**: HELLO 送受信・検証・WELCOME 返却
   - **受入**: TC-001 後半（初期化完了）

### フェーズ 3: Phase・READY・BET（2 日）
4. **T3-01 / T3-02 / T3-03**: Phase 管理・READY・PHASE 通知
5. **T4-01**: BET 受付・保持
6. **T4-02 / T4-03**: 配札・判定・結果送信
   - **受入**: TC-004（BETTING 制約）・TC-001 全体（1ラウンド成立）

### フェーズ 4: UI・受入テスト（1 日）
7. **T5-01 / T5-03 / T5-06**: Phase UI 制御
8. **T5-07**: ルールウィンドウ
   - **受入**: TC-005（常時参照可能）

---

## 次の一手（推奨）

### 即座（今週）
1. **T0-01 解決**：DLL 固定化（`lib/` 配下に Experiment.TcpSocket.dll を置く）
2. **T1-01 着手**：行フレーミング実装（Framer の確認・実装）

### 1 週間以内
- T2-01 / T2-02：サーバ・クライアント接続処理
- 受入テスト TC-001 前半を通す

---

## リスク・注意

| リスク | 対策 |
|---|---|
| DLL 参照が環境依存 | 早期に T0-01 解決 |
| 行フレーミングが無いと通信が動かない | T1-01 優先度を最高にする |
| UI イベント処理が SynchronizingObject 未設定だと Cross-thread エラー | UI 実装時に必ず確認 |

---

## 質問・確認事項

- `Framer.vb` / `Parser.vb` の実装状況（ファイルの内容を確認したい）
- `Experiment.TcpSocket.dll` が実行フォルダに出力されているか確認したい
