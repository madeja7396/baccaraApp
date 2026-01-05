# Baccarat プロジェクト タスク表（v1.1 要件対応）

> スコープ: `Baccarat.Client` / `Baccarat.Server` / `Baccarat.Shared`
> 
> 目的: 要件定義書（`docs/CONTEXT.md` v1.1）との差分を埋め、TC-001?TC-005 を満たす最短経路で実装を進める。

---

## 優先度ルール
- **P0**: これが無いとデモ/疎通が成立しない（最優先）
- **P1**: 主要機能（ゲーム成立）
- **P2**: 仕上げ（異常系/UX/ドキュメント）

---

## 0. 基盤整備（壊れにくさ最優先）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T0-01 | P0 | `Experiment.TcpSocket.dll` の参照パスを固定化（`lib/` 等に配置し Client/Server で統一） | `Baccarat.Client.vbproj` / `Baccarat.Server.vbproj` | クリーン環境でも参照切れなし |
| T0-02 | P0 | DLL同梱（Copy Local / Private=True）を確認 | Client/Server 参照 | `bin/Debug...` に DLL が出力される |
| T0-03 | P0 | `Baccarat.Server` の `MainForm` 指定と `RootNamespace` の二重を解消 | `Baccarat.Server.vbproj` / `My Project/Application.myapp` / 自動生成 | `MainForm` が二重名でなく 1 本に統一 |

---

## 1. 行フレーミング（PR-003）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T1-01 | P0 | DataReceive の **行フレーミング**（分割/結合受信対応） | Client/Server 受信処理 | 1行単位で `Parser.TryParse` に渡せる |
| T1-02 | P0 | 送信メッセージ末尾に `\n` 統一 | Client/Server 送信処理 | `CMD,...\n` で送信される |
| T1-03 | P1 | パース失敗/未知コマンド時の `ERROR` 応答方針を実装 | Server | `ERROR,reason` が返る |

---

## 2. 接続・HELLO/WELCOME（TC-001 前半 / F-001?F-005）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T2-01 | P0 | サーバ待機開始（OpenAsServer）と Accept/Disconnect のログ化 | `FormServer`/TcpSocket | 接続イベントがログに出る |
| T2-02 | P0 | クライアント接続（OpenAsClient）と Connect/Disconnect のログ化 | `FormLobby`/TcpSocket | 接続イベントがログに出る |
| T2-03 | P0 | クライアント: 接続後 HELLO,nickname 送信 | `FormLobby` | サーバがHELLO受信 |
| T2-04 | P0 | サーバ: nickname検証（空/長すぎ）→ `ERROR,INVALID_NAME` | `ServerHost.HandleHello` | 不正名が拒否される |
| T2-05 | P0 | サーバ: playerId割当、`WELCOME` 返信（seed/maxRounds/initChips） | `ServerHost` | ClientがWELCOMEを解釈し状態更新 |

---

## 3. READY と Phase 制御（TC-001 中盤 / F-006?F-008）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T3-01 | P0 | READY を 2人分受理して開始 | `ServerHost.HandleReady` | 2人揃うまで開始しない |
| T3-02 | P0 | Phase 管理（LOBBY→BETTING→DEALING→RESULT→GAMEOVER） | `ServerHost`/`GameState` | サーバ権威でPhaseが進む |
| T3-03 | P0 | Phase遷移ごとに `PHASE,phase,round` を全員へ送信 | `ServerHost` | クライアントUIがPhase反映 |
| T3-04 | P0 | BETTING以外のBET拒否（BET_ACK/ERROR） | `ServerHost.HandleBet` | 不正操作が通らない |

---

## 4. BET/配札/結果（TC-001 後半 / F-009?F-011）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T4-01 | P0 | BET 受付・保持（`BetInfo` locked） | `GameState`/Server | 両者確定を検知 |
| T4-02 | P0 | Shoe生成、初期配札（placeholderで可） | Shared Rules/Model | DEAL可能 |
| T4-03 | P0 | 勝敗判定（Winner）と `ROUND_RESULT` 送信 | Server/Client | 結果が表示される |
| T4-04 | P0 | 配当計算（`IPayoutCalculator` placeholder）と chips 更新 | Server | chipsが更新される |
| T4-05 | P1 | GAME_OVER 条件（MaxRounds 等）と通知 | Server | `GAME_OVER` が届く |

---

## 5. UI要件（UI-001?UI-107 / ルールウィンドウ）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T5-01 | P0 | Lobby: nickname空なら接続不可 | `FormLobby` | UI-001 |
| T5-02 | P1 | Lobby: IP形式チェック | `FormLobby` | UI-002 |
| T5-03 | P0 | Game: Phaseに応じて入力制御 | `FormGame` | UI-101 |
| T5-04 | P0 | Game: bet額 Min/Max 制御 | `FormGame` | UI-102/103 |
| T5-05 | P0 | Game: bet確定後変更不可 | `FormGame` | UI-104 |
| T5-06 | P0 | Game: RESULT のみ Next 有効 | `FormGame` | UI-105 |
| T5-07 | P0 | Rules: 非モーダル・単一インスタンス | `FormRules` | UI-201/202 |
| T5-08 | P1 | Rules: 「基本/配当(予定)/操作」表示 | `FormRules` | UI-203 |

---

## 6. 異常系・堅牢化（NF/PR/F-015）
| ID | 優先 | タスク | 対象 | 完了条件 |
|---|---:|---|---|---|
| T6-01 | P0 | 未知コマンド/パラ不足/型不正→ERROR | Server | PR-004 |
| T6-02 | P1 | 3人目接続拒否（ROOM_FULL） | Server | F-002 |
| T6-03 | P1 | 切断検知→相手通知→安全停止 | Server/Client | F-015 |
| T6-04 | P2 | ログ形式統一（時刻/方向/内容） | Shared Logger | NF-003 |

---

## 受入テスト（チェック順）
- **TC-001** 接続/ハンドシェイク（HELLO/WELCOME/PHASE）
- **TC-003** 行フレーミング（分割/結合受信耐性）
- **TC-004** Phase制約（BETTING以外のBET拒否）
- **TC-005** ルールウィンドウ常時参照
