プロジェクト完了に向けた振興計画（TODO設計）v1.2

方針（絶対順守）
- 単純（Simple） / 可読（Readable） / 簡単（Easy）

進捗サマリ
- M1: クライアント受信パース・状態保持 完了（FormLobby.Comms + ClientGameState）
- M2: サーバ第三カード（簡易） 完了（BaccaratRulesPlaceholder.ApplyThirdCardRule）
- M3: BET堅牢化 完了（拒否理由定数化・30sタイムアウト）
- M4: スモーク手順ドキュメント 完了（SMOKE_TEST.md）
- 次: M5（最小 Game UI）

ゴール
- 2 クライアントで一連ゲームが安定動作（ログで追跡可能）
- 画像は後投入でOK

優先順位（高→低）
1. M5: 最小 Game UI（P1）
2. ログ強化・運用設定（P2）

---

## M5 - 最小 Game UI（実装 1日想定）
現状:
- FormGame のスケルトンあり（ApplyPhase/ログ枠）
- 送信は Lobby 側に集約中（Connect/受信パース済）

方針（最小）:
- 初回は FormGame を後回し。Lobby のログで受入可とする（Simple/Easy を優先）
- 時間に余裕があれば、以下の最小要素のみ追加:
  - BET UI: Radio(Player/Banker/Tie) + NumericUpDown(1..チップ) + 送信ボタン
  - READY ボタン（WELCOME受信後のみ有効化）

Acceptance（どちらか満たせば可）:
- A) Lobby のログのみでスモーク手順に従いゲーム完走できる
- B) FormGame にて BET送信でき、BET_ACK/PHASE/RESULT をログで確認できる

Fallback（時間切れ時）:
- READY/BET は当面テキスト送信（将来 UI 化）。現在はサーバ側が全フローを主導

---

実装ルール（再掲）
- 1マイルストーンずつ小さく commit
- 関数は ~20行目安 / 例外はログへ / コメントは"なぜ"だけ
- BMP は最後に追加

---

次のアクション
- M5 を B（FormGame 簡易）で着手 or A（Lobbyのみ）で受入を通す
- 直近は A 選択（時短）。必要に応じ B へ拡張