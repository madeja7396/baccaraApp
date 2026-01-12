# インシデントレポート（2026-01-12）

## 概要
- 対象: Baccarat クライアント/サーバ（VB / WinForms）
- 事象: クライアント側ビルド失敗（リソース重複/Partial競合）、FormGame が表示されない、サーバ再起動後の再接続不可
- 影響: 実行不能な時間帯が発生し、M5（最小UI）検証が停滞

## 影響範囲
- クライアント: `Baccarat.Client` プロジェクトのビルド失敗（MSB3577）、実行時の UI 非表示
- サーバ: `ROOM_FULL` 応答の直後クローズで送信エラー、再起動後に再接続不可

## タイムライン（抜粋）
- 12:xx: FormGame 手動BET UI 追加中に `.resx` 重複（MSB3577）発生
- 13:xx: Partial クラスを複数ファイルに分割した結果、メンバ重複（BC30260/BC31429）
- 14:xx: `FormLobby.Comms.resx` と `FormGame.Code.resx` の重複を特定
- 15:xx: サーバで GAME_OVER 後、再起動→再接続で `ERROR,ROOM_FULL` + 送信ハンドル例外
- 16:xx: 修正実施と再ビルド成功、FormGame 表示と手動BETの最小経路確立

## 技術的詳細（主なエラーメッセージ）
- MSB3577: 出力リソース重複
  - `obj\Debug\net10.0-windows\Baccarat.Client.Forms.FormGame.resources`
  - `obj\Debug\net10.0-windows\Baccarat.Client.Forms.FormLobby.resources`
- BC30269/BC30521: メソッド重複/最も固有なメンバー解決不可
- BC30260/BC31429: 同名メンバ重複（Partial クラスでの複数定義）

## 原因
1) Designer とは別の Partial ファイル（`FormGame.Code.vb`, `FormLobby.Comms.vb`）に `.resx` が付与されたまま増殖し、同一リソース名を複数生成
2) `FormLobby` を `Comms` と `Net` 2ファイルへ分割し、同一フィールド（`_tcp`, `_handle`, `_framer`, `_gameState`）を重複定義
3) サーバ側で `ROOM_FULL` 応答直後にクローズし Send と競合、さらに再起動時に内部状態のリセット不足

## 対応（実施済）
- クライアント
  - `FormGame.Code.vb` / `FormGame.Code.resx` を削除し、`FormGame.vb` 側へ統合
  - `FormLobby.Comms.resx` を削除（重複リソース回避）
  - 通信ロジックを `FormLobby.Comms.vb` へ一本化（`FormLobby.Net.vb` は撤去）
  - `WELCOME` 受信時に `FormGame` を表示、`PHASE/RESULT/GAME_OVER` で `ApplyPhase/UpdateHeader` 反映
  - `FormGame` に手動BET（Radio+NumericUpDown+Bet）を実装、送信は `Action(Of String)` コールバック
- サーバ
  - `ResetState()` を `StartServer/StopServer` で実行し、再起動後の状態を初期化
  - `ROOM_FULL` は `SendTo` 後に 50ms 遅延クローズ（`ThreadPool.QueueUserWorkItem`）
  - BETTING 開始時に `_betStartTime` を更新（タイムアウト判定の安定化）

## 再発防止
- Designer 以外の Partial に `.resx` を付与しない運用（ファイル追加時に確認）
- Partial クラス分割時は、フィールドの単一出所（Single Source）を徹底
- CI に MSBuild エラー（MSB3577/BC30260）検出の早期警告を追加

## 残課題
- `FormLobby.Comms.vb` の自動BETは、手動BET検証時はオフにできるトグルを用意（将来）
- FormGame 表示起点をメニューまたは設定に（UX改善）
- 単体テスト最小追加（Parser/Rules）

## 受入確認
- ビルド成功
- 2 クライアント接続→WELCOME→FormGame 表示
- BETTING で手動BET送信→BET_ACK 受信→DEAL/RESULT→GAME_OVER まで遷移
- サーバ再起動後に再接続が可能、3台目は `ROOM_FULL` で安全に切断

---
このレポートは `docs/INCIDENT_REPORT_20260112.md` に保存されています。今後の PR に添付可能です。
