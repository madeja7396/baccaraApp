# Baccarat プロジェクト タスク一覧 (v1.2 実装反映)

> Scope: `Baccarat.Client` / `Baccarat.Server` / `Baccarat.Shared`

---

## 完了/進行状況（Backend）
- HELLO/WELCOME: 済
- READY → PHASE(BETTING): 済
- BET → SETTLE（DEALING/RESULT） → 次ラウンド: 済（最小）
- 異常応答（BAD_FORMAT/UNSUPPORTED/BET_ACK各種）: 最小実装

---

## 残タスク（優先度順）
1. GAME_OVER 判定（P1）
   - MaxRounds 到達、チップ枯渇時の終了と `GAME_OVER` 送信
2. DEAL 詳細（P1）
   - `DEAL,playerCards...,bankerCards...` 形式で内容を送信
3. 3人目拒否の完了形（P1）
   - Accept 時に `ERROR,ROOM_FULL` を返し、ソケットをクローズ
4. 異常系の統一（P1）
   - バリデーションヘルパで理由表現を共通化
5. ログ強化（P2）
   - ラウンドごとサマリ/例外詳細

---

## クライアント側（次）
- `WELCOME/PHASE/BET_ACK/ROUND_RESULT` のパース→UI反映（Lobby→Game 遷移含む）
- READY ボタン/自動送信導線の実装
- BET UI と送信ロジックの接続