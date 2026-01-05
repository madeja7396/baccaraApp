# Protocol Quick Reference

文字列フォーマット: `CMD,param1,param2,...\n` （UTF-8, 区切りはカンマ, 終端は LF）

| CMD | 方向 | 説明 |
|---|---|---|
| HELLO,nickname | C→S | 接続直後に送信。nickname 必須。 |
| WELCOME,playerId,... | S→C | playerId 付与と初期値通知。 |
| READY | C→S | プレイヤー準備完了通知。2人揃いで開始。 |
| PHASE,phase,round | S→C | サーバ権威の Phase 遷移通知。 |
| BET,target,amount | C→S | Phase=BETTING のみ受理。 |
| BET_ACK,ok,reason | S→C | BET の検証結果（省略可）。 |
| DEAL,pCards,bCards | S→C | 配札内容（表現はプレースホルダ）。 |
| ROUND_RESULT,winner,payout,chipsP1,chipsP2 | S→C | 勝敗と所持チップ更新。 |
| GAME_OVER,winner | S→C | 終了条件を満たした場合。 |
| ERROR,reason | S→C | 不正入力やサーバ例外通知。 |
| BYE | 双方向 | 切断予告（予定）。 |

詳細・例外の扱いは `AGENTS.md` セクション4を参照。
