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

## 1. 通信フレーミング (PR-003)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T1-00 | P0 | DataReceiveEventArgs のプロパティ名確認 | Client/Server | **完了：`e.Data` 確定** |
| T1-01 | P0 | DataReceive での行フレーミング実装 | Client/Server | Server: 済、Client: 済（Lobby.Comms） |
| T1-02 | P0 | 送信メッセージへの `\n` 付与 | Client/Server | Server: 済、Client: 済（HELLO他） |
| T1-03 | P1 | パースエラー時の `ERROR` 返信 | Server | 一部実装（BAD_FORMAT/UNSUPPORTED） |

---

## 2. 接続・HELLO/WELCOME (TC-001 / F-001～005)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T2-01 | P0 | Server: OpenAsServer / Accept / Disconnect ログ | Server | 済 |
| T2-02 | P0 | Client: OpenAsClient / Connect / Disconnect ログ | Client | 済（Lobby.Comms） |
| T2-03 | P0 | Client: HELLO,nickname 送信 | Client | 済（Connect時自動送信） |
| T2-04 | P0 | Server: nickname検証 / ERROR応答 | Server | 済（INVALID_NAME, ROOM_FULL） |
| T2-05 | P0 | Server: WELCOME,playerId 送信 | Server | 済 |

---

## 3. READY・Phase管理 (TC-001 / F-006～008)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T3-01 | P0 | READY 受信と2人待機 | Server | 済（両者READY→BETTING） |
| T3-02 | P0 | Phase管理 (LOBBY->BETTING...) | Server | 初期ルート実装 |
| T3-03 | P0 | PHASE,phase,round 送信 | Server | 済 |
| T3-04 | P0 | Phase外のBET拒否 | Server | 一部実装（BET_ACK,false,PHASE_MISMATCH） |

---

## 4. BET・判定・配当 (F-009～011)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T4-01 | P0 | BET 受信と確定(Lock) | Server | 済（重複防止/所持確認） |
| T4-02 | P0 | 配札 (DEAL) と勝敗判定 | Server | 済（プレースホルダルール呼出） |
| T4-03 | P0 | ROUND_RESULT 送信 | Server | 済 |
| T4-04 | P0 | チップ計算と更新通知 | Server | 済 |
| T4-05 | P1 | GAME_OVER 判定 | Server | 未実装 |

---

## 5. UI実装 (UI-001～)
| ID | 優先度 | タスク | 対象 | 備考 |
|---|---:|---|---|---|
| T5-01 | P0 | Lobby: ニックネーム必須制御 | Client | 済（最低限） |
| T5-02 | P1 | Lobby: IP形式チェック | Client | 未着手 |
| T5-03 | P0 | Game: Phaseによる入力制御 | Client | 未着手 |
| T5-04 | P0 | Game: ベット額 Min/Max/所持金制御 | Client | 未着手 |
| T5-05 | P0 | Game: ベット確定後の変更不可 | Client | 未着手 |
| T5-06 | P0 | Game: Result時のNextボタン制御 | Client | 未着手 |
| T5-07 | P0 | Rules: ルールウィンドウ表示 | Client | 未着手 |

---

テストベース
- 2クライアントで接続→HELLO/WELCOME→両者READY→PHASE=BETTING 受信
- クライアントから `BET,playerId,target,amount`（または `BET,target,amount`）を双方送信
- サーバが `BET_ACK,true` を返し、両者Lockで DEAL→RESULT→PHASE=BETTING(round+1) を Broadcast
- 異常パス: Phase外BET, 所持超過, BAD_ARGS は `BET_ACK,false,<reason>` を返す