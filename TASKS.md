# Baccarat プロジェクト タスク一覧 (v1.1 実装対応)

> Scope: `Baccarat.Client` / `Baccarat.Server` / `Baccarat.Shared`
> 
> 目的: 要件定義書 (v1.1) に基づき、TC-001～TC-005 を満たす実装を行う。

---

## 優先度定義
- **P0**: これがないとデモ/対戦が成立しない (最優先)
- **P1**: 要件必須機能 (ゲームとして成立)
- **P2**: 仕上げ (例外処理、UX向上)

---

## 0. 環境構築 (P0 - 完了)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T0-01 | P0 | `Experiment.TcpSocket.dll` の参照パスを `lib/` 基準に統一 | Client/Server | **完了** (ルート直下構成に変更済) |
| T0-02 | P0 | DLL参照の「ローカルにコピー(Copy Local)」をTrueに設定 | Client/Server | **完了** |
| T0-03 | P0 | `Baccarat.Server` の `RootNamespace` を適切に設定 | Server | **完了** |

---

## 1. 通信フレーミング (PR-003)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T1-01 | P0 | DataReceive での行フレーミング実装 | Client/Server | `Framer.vb` の結合 |
| T1-02 | P0 | 送信メッセージへの `\n` 付与 | Client/Server | `Parser` 利用 |
| T1-03 | P1 | パースエラー時の `ERROR` 返信 | Server | |

---

## 2. 接続・HELLO/WELCOME (TC-001 / F-001～005)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T2-01 | P0 | Server: OpenAsServer / Accept / Disconnect ログ | Server | TCPイベント実装 |
| T2-02 | P0 | Client: OpenAsClient / Connect / Disconnect ログ | Client | TCPイベント実装 |
| T2-03 | P0 | Client: HELLO,nickname 送信 | Client | 接続完了時 |
| T2-04 | P0 | Server: nickname検証 / ERROR応答 | Server | |
| T2-05 | P0 | Server: WELCOME,playerId 送信 | Server | 初期化データ含む |

---

## 3. READY・Phase管理 (TC-001 / F-006～008)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T3-01 | P0 | READY 受信と2人待機 | Server | 全員READYで開始 |
| T3-02 | P0 | Phase管理 (LOBBY->BETTING...) | Server | GameStateで管理 |
| T3-03 | P0 | PHASE,phase,round 送信 | Server | 状態変化通知 |
| T3-04 | P0 | Phase外のBET拒否 | Server | 不正操作防止 |

---

## 4. BET・判定・配当 (F-009～011)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T4-01 | P0 | BET 受信と確定(Lock) | Server | |
| T4-02 | P0 | 配札 (DEAL) と勝敗判定 | Server | プレースホルダ可 |
| T4-03 | P0 | ROUND_RESULT 送信 | Server | 結果通知 |
| T4-04 | P0 | チップ計算と更新通知 | Server | |
| T4-05 | P1 | GAME_OVER 判定 | Server | |

---

## 5. UI実装 (UI-001～)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T5-01 | P0 | Lobby: ニックネーム必須制御 | Client | |
| T5-02 | P1 | Lobby: IP形式チェック | Client | |
| T5-03 | P0 | Game: Phaseによる入力制御 | Client | `ApplyPhase` |
| T5-04 | P0 | Game: ベット額 Min/Max/所持金制御 | Client | |
| T5-05 | P0 | Game: ベット確定後の変更不可 | Client | |
| T5-06 | P0 | Game: Result時のNextボタン制御 | Client | |
| T5-07 | P0 | Rules: ルールウィンドウ表示 | Client | 非モーダル |

---

## 6. その他 (異常系)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T6-01 | P0 | 不正コマンド対応 | Server | |
| T6-02 | P1 | 3人目接続拒否 (ROOM_FULL) | Server | |
| T6-03 | P1 | 切断時の対戦中断処理 | Server/Client | |