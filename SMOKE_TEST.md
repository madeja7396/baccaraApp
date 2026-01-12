スモークテスト手順（Baccarat v1.0）

## セットアップ
- Visual Studio でソリューション `baccaraApp.sln` を開く
- ビルド成功を確認

## テスト手順

### 1. サーバ起動
- `Baccarat.Server` をスタートアッププロジェクトに設定し実行
- `FormServer` ウィンドウが表示され、`btnStart` をクリック
- ログに `Server start on 9999` が出ることを確認

### 2. クライアント 1 起動
- `Baccarat.Client` を実行（新しいインスタンスで）
- `FormLobby` で以下を入力:
  - IP: `127.0.0.1`
  - Nickname: `Player1`
- `Connect` ボタンをクリック
- ログに以下が出ることを確認:
  ```
  [CONNECT] 127.0.0.1:9999
  [SRV] WELCOME,1,0,10,1000
  [WELCOME] playerId=1, maxRounds=10, initChips=1000
  ```

### 3. クライアント 2 起動
- `Baccarat.Client` をもう一度実行
- `FormLobby` で以下を入力:
  - IP: `127.0.0.1`
  - Nickname: `Player2`
- `Connect` ボタンをクリック
- ログに同様に WELCOME が出ることを確認

### 4. READY フェーズ
**クライアント 1・2 の FormLobby に READY ボタンが見当たらない場合は以下で対応:**
- コンソール/テキストボックスから手動で `READY` を送信する（または FormLobby にボタン追加を実装予定）
- サーバのログに以下が出ることを確認:
  ```
  [SRV] PHASE,BETTING,1
  [PHASE] BETTING, round=1
  ```

### 5. BET フェーズ
**BET UI が FormGame にない場合は以下で対応:**
- クライアント 1 から以下を送信: `BET,1,PLAYER,100`
- クライアント 2 から以下を送信: `BET,2,BANKER,50`
- 各クライアントのログに以下が出ることを確認:
  ```
  [SRV] BET_ACK,true
  [BET_ACK] Accepted
  ```

### 6. DEAL/RESULT フェーズ
- 両クライアントのログに以下が出ることを確認:
  ```
  [SRV] PHASE,DEALING,1
  [PHASE] DEALING, round=1
  [SRV] DEAL,S-01|H-05,C-02|D-10
  [DEAL] Player: S-01|H-05, Banker: C-02|D-10
  [SRV] PHASE,RESULT,1
  [SRV] ROUND_RESULT,PLAYER,100,-50,1100,950
  [RESULT] Winner=PLAYER, Payout P1=100, P2=-50, Chips P1=1100, P2=950
  ```
- 次ラウンド開始：
  ```
  [SRV] PHASE,BETTING,2
  [PHASE] BETTING, round=2
  ```

### 7. 複数ラウンド実行
- 同じ BET を複数回繰り返し、MaxRounds(10) に達して GAME_OVER が出るまでテスト
- ログに以下が出ることを確認:
  ```
  [SRV] GAME_OVER,1,1600,400
  [GAME_OVER] Winner=Player1, Final Chips P1=1600, P2=400
  ```

## トラブルシューティング

| 事象 | 原因 | 対応 |
|---|---|---|
| "ROOM_FULL" が出て接続が切られる | 3 クライアント以上を接続しようとした | 2 クライアントのみを接続 |
| BET_ACK が "PHASE_MISMATCH" | BETTING フェーズでない時に BET を送った | PHASE=BETTING まで待つ |
| "NO_CHIPS" | ベット金額 > 所持チップ | 初期チップ(1000)以下の金額を賭ける |
| カード表示が出ない | BMP ファイルが assets/cards にない | 画像なしでもログ出力で確認可能 |

## 仕様補足

- プレイヤースコア ? 5 で 3 枚目を引く
- バンカーはプレイヤーの行動に応じて簡略判定
- 最大 10 ラウンド または チップ枯渇で終了

---

テスト完了後は GitHub にて進捗報告。
