# 現状報告と検証結果

日時: 自動生成

## 1. 概要
この文書はリポジトリ内の最新実装の進捗・整合性検証結果・仕様と未解決点、および次に行うべきTODOを正確にまとめたものです。検証は作業環境内でのビルド実行結果に基づいています。

## 2. 現状サマリ
- サーバ（`Baccarat.Server`）
  - `ServerHost` によりゲーム進行（HELLO → WELCOME → READY → PHASE → BET → DEAL → ROUND_RESULT）が実装されており、サーバログで期待されるコマンド列が出力されていることを確認済み。
- クライアント（`Baccarat.Client`）
  - ロビー側 (`FormLobby.Comms.vb`) は TCP 接続、フレーミング、メッセージ受信を処理。`WELCOME` 受信後に `FormGame` を表示する分岐を実装済。
  - ゲーム画面 (`FormGame.vb`) はベットUIを動的に構築し、`ShowDeal`（テキスト表示）と `ApplyRoundResult` を実装し、受信メッセージを UI に反映するための受け口がある。
- 行フレーマ（`LineFramer`）および共通モデルは `Baccarat.Shared` に実装済。

## 3. 実施した整合性検証
1. ビルド実行（`dotnet build Baccarat.sln`）
   - 結果: ビルド失敗（エラーは下記）
   - 主要エラー: `MSB3577: 2 つの出力ファイル名が同じ出力パスに解決されました:"obj\Debug\net10.0-windows\Baccarat.Client.Forms.FormLobby.resources"`
     - 原因: `Baccarat.Client\Forms` フォルダ内に `FormLobby.resx` と `FormLobby.Comms.resx` の 2 つの .resx が存在し、MSBuild が同一のリソース出力名に解決して衝突している。
2. 追加ビルド試行
   - 以前、実行中プロセス（`Baccarat.Client.exe` 等）でファイルロックが発生したため強制終了して再ビルドを行い、一時的にビルド成功した記録があるが、現在は上記の重複リソース問題が残存している。

検証ログ抜粋（要約）:
- `MSB3577: 2 つの出力ファイル名が同じ出力パスに解決されました:"obj\Debug\net10.0-windows\Baccarat.Client.Forms.FormLobby.resources"`

## 4. 整合性の評価
- 通信プロトコル面: サーバとクライアントのメッセージ形式は整合している（`DEAL` / `ROUND_RESULT` 等がサーバログに出力され、クライアント側に受け渡す実装がある）。
- UI 反映パス: `FormLobby` → `FormGame` の生成および `ShowDeal` / `ApplyRoundResult` の呼出し経路があるため、受信→表示の流れは概ね整っている。
- ただし現状ビルドはリソース衝突で失敗しているため、完全な実行検証（受信→UI描画の実機確認）は現時点でブロックされている。

## 5. 仕様確認（要点）
- 主要コマンド（現行実装）:
  - `HELLO,nickname` → `WELCOME,playerId,0,maxRounds,initChips` → クライアントは `READY` を返す
  - `PHASE,phase,round` → フェーズ制御
  - `BET,...` / `BET_ACK,true/false` → ベット処理
  - `DEAL,playerCodes,bankerCodes` → 配札（現在はテキストコード）
  - `ROUND_RESULT,winner,p1delta,p2delta,chips1,chips2` → 結果と所持チップ
- UI 要求: `FormGame` は `ApplyPhase` により操作有効/無効を切替、`ShowDeal` と `ApplyRoundResult` で配札・結果を表示する。

## 6. 未解決点（優先度付き）
1. (必須) リソース重複: `FormLobby.resx` と `FormLobby.Comms.resx` が同一の出力名に解決され MSBuild エラーを発生させる。→ ビルド不能。 (High)
2. (高) `FormGame` のデザイン要素が重なり見づらい。Designer レイアウト調整が必要。 (High)
3. (中) `btnNext` / `btnRules` 等の UI イベントが未実装（ルール表示や次ラウンド操作）。 (Med)
4. (中) `numAmount.Maximum` の所持チップ追従や二重送信防止の微調整。 (Med)
5. (低) カード画像アセット未実装（現在はテキスト表示）。 (Low)

## 7. 正確な TODO（ファイル単位、手順・コマンド明記）
- TODO-1: リソース重複の解消（最優先）
  - 対象ファイル: `Baccarat.Client/Forms/FormLobby.Comms.resx`
  - 手順1 (推奨: 削除)
    - 作業: このファイルが不要であれば削除する。
    - コマンド例: `git rm Baccarat.Client/Forms/FormLobby.Comms.resx` → `git commit -m "Remove duplicate resx"`
  - 手順2 (必要ならリネーム)
    - 作業: 必要なリソースなら `FormLobbyComms.resx` のようにリネームし、Designer 参照を修正。
  - 検証: `dotnet build Baccarat.sln` で MSB3577 が解消されること。

- TODO-2: クリーンビルドの実行
  - コマンド: `taskkill /F /IM Baccarat.Client.exe` (実行中なら), `dotnet clean`, `dotnet build Baccarat.sln`
  - 目的: ファイルロック・残渣をクリアして確実にビルド可能な状態へ。

- TODO-3: FormGame の Designer を安全なレイアウトに置換
  - 対象: `Baccarat.Client/Forms/FormGame.Designer.vb`
  - 具体変更: フォームサイズを大きくし、`pnlPlayer`/`pnlBanker` を左右に十分な余白で配置、`grpBet`/`txtLog` を下部に配置して重なりを排除する。
  - 検証: Form を起動して視認性が改善されること。

- TODO-4: FormGame の微修正
  - 対象: `Baccarat.Client/Forms/FormGame.vb`
  - 変更点:
    - `BuildBetControls`: `numAmount.Maximum` を `_state` の所持チップに追従させる。
    - `OnBetClick`: `btnBet.Enabled = False` を設定して二重送信を防止する。
  - 検証: 手動ベットで `BET` が送信され `BET_ACK` を受け、再送不可となること。

- TODO-5: UI イベント実装
  - 対象: `FormGame.vb`（`btnNext.Click`, `btnRules.Click`）
  - 目的: `btnNext` で `READY` を送信、`btnRules` で `FormRules` を表示する。

- TODO-6: 受信→UI 統合テスト
  - 手順: サーバ起動 → 2 クライアント接続 → 手動ベット → サーバの DEAL/ROUND_RESULT による UI 反映を確認。

## 8. 推奨次手順（即対応順）
1. リソース重複を解消（TODO-1）。
2. 完全クリーン→ビルドしてエラー消滅を確認（TODO-2）。
3. Designer のレイアウト置換（TODO-3）→ 再ビルド → 実行で見た目確認。
4. `FormGame` の小修正（TODO-4 / TODO-5）。
5. 手動統合テスト（TODO-6）。

---

このファイルは作業環境内での実行結果（ビルドログ）と、ソースコードの現在状態に基づいて作成しています。次に進めたい具体作業（例: 私が `FormGame.Designer.vb` を安全版に置換する、または `FormLobby.Comms.resx` を削除する等）を指示してください。指示があれば自動でファイル編集/削除/作成を行います。