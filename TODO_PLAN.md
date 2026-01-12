プロジェクト完了に向けた振興計画（TODO設計）v1.1

方針（絶対順守）
- 単純（Simple）: 最短で動く最小実装を優先
- 可読（Readable）: 関数/変数は説明的な名付け。冗長な抽象化は避ける
- 簡単（Easy）: コードは直感的に追える構成。テスト/デバッグを容易に

ゴール
- 2 クライアントでの一連ゲーム（HELLO→WELCOME→READY→BET→DEAL→ROUND_RESULT→次ラウンド/GAME_OVER）が安定して動作する。
- 最低限の UI: ログとボタンで動作確認可能（既存の簡易UI維持）。
- カード画像はあとから投入できる設計を維持。

優先順位（高→低）
1. クライアント受信パースと状態保持（P0）← **最初**
2. 第三カード規則の段階的実装（P0）
3. BET 周りの堅牢化（タイムアウト/同時操作/エッジケース）（P0）
4. 単体テスト & 統合スモークテストの整備（P1）
5. 最小限の Game UI（FormGame）実装（P1）
6. ログ強化と運用設定（P2）

---

## マイルストーン & タスク（実装順序を明確化）

### M1 - クライアント受信実装（1日）
**現状**: `FormLobby.Comms.vb` はログまで。状態保持なし。
**必要**: メッセージパーサ ＋ 状態管理クラス

**タスク M1.1**: 受信状態を管理するクラスを作成
```
新規ファイル: Baccarat.Client/Model/ClientGameState.vb
- 役割: playerId, phase, chips[2], lastDeal[], maxRounds を保持
- 構造: シンプルなプロパティだけ（ロジックなし）
```

**タスク M1.2**: `FormLobby.Comms.vb` にメッセージパーサを追加
```
追加: OnLineReceived メソッド内で各コマンド別に分岐
- WELCOME: playerId, maxRounds, initChips を ClientGameState に保存
- PHASE: phase と round を保存
- BET_ACK: ok フラグと reason をログに出す
- DEAL: カードコード文字列をそのままログ出力（BMP 読込はM5以降）
- ROUND_RESULT: winner, payout, chips を保存・ログ
- GAME_OVER: phase = GAMEOVER に設定、ゲーム終了ログ
- ERROR: reason をログに赤字表示（又は [ERROR] 接頭）
```

**Acceptance**: 
- [ ] 2 クライアント起動 → サーバ Start
- [ ] クライアント1: Connect → WELCOME 受信 → ログに playerId=1 が出る
- [ ] クライアント2: Connect → WELCOME 受信 → ログに playerId=2 が出る
- [ ] 両者 READY 送信 → ログに `PHASE,BETTING,1` が出る
- [ ] ビルド成功、スモーク確認後 git commit

---

### M2 - サーバ：第三カード規則（実装 1.5日）
**現状**: `BaccaratRulesPlaceholder.ApplyThirdCardRule` は空（TODO）
**必要**: 簡易ルール実装

**タスク M2.1**: 簡易第三カードルールをコメント化して仕様を確定
```
Baccarat.Shared/Rules/BaccaratRulesPlaceholder.vb に以下を追記:
' ===== 簡易第三カード規則（段階1）=====
' プレイヤー: 初期スコア <= 5 なら3枚目を引く
' バンカー: 
'   - プレイヤーが3枚目を引かなかった場合: バンカースコア <= 5 で3枚目を引く
'   - プレイヤーが3枚目を引いた場合: 簡略版 → バンカースコア <= 6 で3枚目を引く
' 注: 厳密なバカラルール（バンカー3枚目の判定はプレイヤーの3枚目カード値に依存）はPhase2以降
```

**タスク M2.2**: `ApplyThirdCardRule` を実装
```
Sub ApplyThirdCardRule(state As GameState)
  If state.PlayerHand.Cards.Count = 2 Then
    Dim playerScore = ComputeScore(state.PlayerHand)
    If playerScore <= 5 Then
      DrawToHand(state.Shoe, state.PlayerHand, 1)
    End If
  End If
  
  If state.BankerHand.Cards.Count = 2 Then
    Dim bankerScore = ComputeScore(state.BankerHand)
    Dim drawBanker = False
    If state.PlayerHand.Cards.Count = 2 Then
      ' プレイヤーが引かなかった
      drawBanker = (bankerScore <= 5)
    Else
      ' プレイヤーが引いた（簡略版）
      drawBanker = (bankerScore <= 6)
    End If
    If drawBanker Then
      DrawToHand(state.Shoe, state.BankerHand, 1)
    End If
  End If
End Sub
```

**Acceptance**:
- [ ] ビルド成功
- [ ] サーバで両者 BET ロック → DEAL に3枚目が含まれるケースが発生するか手動確認
- [ ] ログで `DEAL,S-01|H-05|D-03,C-02|H-10` のように3カードが出ているか確認

---

### M3 - BET の堅牢化（実装 1日）
**現状**: BET受理は動く。ただしタイムアウト・同時操作への対応がない。
**必要**: 原子性の確保とタイムアウト機構

**タスク M3.1**: BET 受付タイムアウト機構を追加
```
ServerHost に以下を追加:
- _betStartTime As DateTime （BETTINGフェーズ開始時刻）
- BET_TIMEOUT_SEC As Integer = 30 （定数）

HandleBet メソッド先頭に:
If DateTime.Now.Subtract(_betStartTime).TotalSeconds > BET_TIMEOUT_SEC Then
  ' タイムアウト：両者のBET状態をリセット（自動DEAL へ）
  AppendLog("[TIMEOUT] BET phase exceeded 30s. Auto-settling...")
  SettleRound()
  Return
End If
```

**タスク M3.2**: BET 理由コードを定数化（整理）
```
Baccarat.Shared/Protocol/Commands.vb に以下を追加:
Public Module BetRejectReasons
  Public Const PHASE_MISMATCH = "PHASE_MISMATCH"
  Public Const BAD_ARGS = "BAD_ARGS"
  Public Const BAD_PLAYER = "BAD_PLAYER"
  Public Const BAD_TARGET = "BAD_TARGET"
  Public Const BAD_AMOUNT = "BAD_AMOUNT"
  Public Const NO_CHIPS = "NO_CHIPS"
  Public Const ALREADY_LOCKED = "ALREADY_LOCKED"
End Module
```
ServerHost の HandleBet で使用（読みやすさ向上）

**Acceptance**:
- [ ] 30秒以上 BET を送らない → サーバが自動で DEAL へ進むか確認
- [ ] 同時に両者が BET 送信 → 両方受理されるか、片方エラーになるか確認（整合性あり）

---

### M4 - テスト整備（実装 1日）
**現状**: スモーク手動のみ。スクリプト/自動テストなし。
**必要**: 最小限の手順書と簡単なテスト

**タスク M4.1**: スモークテスト手順を README に記載
```
新規ファイル: SMOKE_TEST.md (または README に追記)
内容:
1. サーバ起動（Baccarat.Server Start ボタン）
2. クライアント1/2 起動、IP=127.0.0.1, Nick=Player1/Player2
3. Connect → [CONNECT] ログと WELCOME が出るか確認
4. 両者 READY（サーバ側で送信ボタン or FormLobby に READY ボタンを用意）
5. BET: クライアント1 → BET,1,PLAYER,100 を送信（テキストボックス or 将来 UI）
6. BET: クライアント2 → BET,2,BANKER,50 を送信
7. DEAL/ROUND_RESULT が両者に届くか確認
8. 次ラウンドへ自動遷移するか確認
```

**タスク M4.2**: 簡単な単体テスト（Rules）
```
新規ファイル: Baccarat.Shared.Tests/TestBaccaratRules.vb (xUnit 或いは MSTest)
- ComputeScore: テストカード (S-01=1, H-09=9) のスコア計算確認
- DetermineWinner: プレイヤー勝ち、バンカー勝ち、タイ各1ケース
```

**Acceptance**:
- [ ] SMOKE_TEST.md が読みやすく、手順を追うだけで動作確認できるか
- [ ] テストが最低1つ通るか（ビルド時に実行）

---

### M5 - 最小 Game UI（実装 1日）
**現状**: FormGame は骨組みのみ。受信ロジックなし。
**必要**: BET 送信 UI と結果表示

**タスク M5.1**: FormGame に BETTING UI を最小化
```
- 金額入力: NumericUpDown (値: 1～999)
- 賭け先: RadioButton 3 個 (Player/Banker/Tie)
- 確定ボタン: btnBetLock → BET コマンド送信 (Form→Server→FormLobby.Comms 経由)
- 結果表示: Label × 3 (winner, payout, newChips)
```

**タスク M5.2**: FormGame への遷移ロジック
```
FormLobby で WELCOME 受信時:
- new FormGame(ClientGameState) で FormGame を表示
- FormGame は ClientGameState を参照して Phase に応じて UI を制御
```

**Acceptance**:
- [ ] FormGame が表示され、BETTING フェーズで BET UI が有効になるか
- [ ] BET 送信後 BET_ACK を受け取り、ログに表示されるか
- [ ] ROUND_RESULT で結果ラベルが更新されるか

---

## 実装上の重要ルール（M1～M5 全体）
1. **一度に1 MB（マイルストーン）だけ実装** → ビルド確認 → commit → 次へ
2. **コードは20行以内の関数を心がける**（読みやすさ）
3. **例外は最小限** → ログに記録 → UI では日本語メッセージは避ける
4. **日本語コメント** で "なぜ" だけを記す（"何を" はコードで）
5. **BMP ファイルは M5 以降で手投入**（UI 実装時）

---

## 全体スケジュール（推奨）
- M1: 半日 → commit（受信パース完成）
- M2: 1日 → commit（第三カード仕様確定・実装）
- M3: 半日 → commit（タイムアウト・定数化）
- M4: 半日 → commit（テスト・スモーク手順）
- M5: 1日 → commit（UI 最小実装）
- **合計約3.5～4日で完成**

---

## 妥協点（実装を軽くする工夫）
- BMP は最後（M5後）に追加予定のため、画像表示コードは FormGame で簡潔に（存在時のみ）
- ログで視認できれば十分（グラフィカルな画面は以降の改善）
- DB は不要（ローカルのみ）、設定ファイルも v1.0 では最小限（Constants で固定値）

---

次ステップ: M1.1 から開始してください（ClientGameState.vb の作成）。