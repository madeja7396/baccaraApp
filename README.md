# Baccarat（対戦型バカラ）

このリポジトリは、VB（Windows Forms）で作る **2人対戦のバカラ** です。  
サーバ1台 + クライアント2台で遊ぶ想定です。

- 通信: TCP（`Experiment.TcpSocket` を使用）
- 画面: Windows Forms
- 方式: **サーバがゲームの正（状態・勝敗・チップ）を管理**します

> 仕様の元資料: `docs/CONTEXT.md`

---

## プロジェクト構成

- `Baccarat.Shared` 共有コード（プロトコル、モデル、ルール、ログ）
- `Baccarat.Server` サーバ（ホスト）
- `Baccarat.Client` クライアント（プレイヤー）

---

## まずやること（共同開発者向け）

### 1) 必要なDLL
このアプリは `Experiment.TcpSocket.dll` を参照します。

- 既にプロジェクト参照に入っています
- 実行フォルダ（`bin\Debug...`）に DLL がコピーされない場合は、参照設定の **Copy Local（ローカルにコピー）** を確認してください

> ※ DLL の置き場所を `lib/` などにまとめる整理は今後のタスクです（`TASKS.md` 参照）

### 2) ビルド
Visual Studio でソリューションを開き、
- **リビルド**（Rebuild）

ビルドが通ればOKです。

### 3) 起動（現状）
- `Baccarat.Server` を起動（サーバ画面）
- `Baccarat.Client` を2つ起動（クライアント画面を2人分）

> いまは「通信・ゲーム進行」は段階的に実装中です。進捗は `TASKS.md` を見てください。

---

## 開発の進め方

- 仕様は `docs/CONTEXT.md` を基準にします
- TODO/計画は `TASKS.md` にまとめています

### ブランチ運用（簡単版）
- 1タスク = 1ブランチ（例: `feature/framer`）
- 途中でも小さくコミット
- PR（または差分レビュー）してから `main` に取り込み

---

## UI（フォーム）の触り方

### 場所と役割
- `Baccarat.Server/Forms/FormServer.vb` サーバの UI（待機、ログ表示）
- `Baccarat.Client/Forms/FormLobby.vb` クライアント接続画面（IP入力、ニックネーム、接続ボタン）
- `Baccarat.Client/Forms/FormGame.vb` ゲーム進行画面（ベット、結果表示）
- `Baccarat.Client/Forms/FormRules.vb` ルール参照ウィンドウ（常時表示可能）

### フォーム編集の手順
1. **Visual Studio でフォームを開く**
   - ソリューション エクスプローラー → 該当フォーム（例: `FormServer.vb`） → ダブルクリック
   - デザインビューが開きます

2. **コントロール追加**
   - ツールボックスから TextBox / Button / Label などを、フォームにドラッグして配置
   - プロパティウィンドウ（F4）で Name / Text などを設定

3. **命名規約**
   - TextBox: `txt...` （例: `txtNickname`）
   - Button: `btn...` （例: `btnConnect`）
   - Label: `lbl...` （例: `lblStatus`）
   - GroupBox: `grp...` （例: `grpBet`）
   - TabControl: `tab...` （例: `tabRules`）

4. **イベント接続**
   - ボタンをダブルクリック → `btnXxx_Click` ハンドラが自動生成される
   - この中に処理を書く（例: 接続ボタンを押したら `openAsClient(...)` を呼ぶ）

### ログ表示の例（コード）
```visualbasic
' TextBox にログを追記する（FormServer / FormLobby 等で使用）
Private Sub AppendLog(message As String)
    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}" & vbCrLf)
    ' 末尾にスクロール
    txtLog.SelectionStart = txtLog.TextLength
    txtLog.ScrollToCaret()
End Sub
```

### Phase による UI の有効/無効（例）
```visualbasic
' FormGame で Phase に応じてボタンを制御
Private Sub ApplyPhase(phase As String)
    Select Case phase
        Case "BETTING"
            grpBet.Enabled = True  ' ベット入力を有効化
            btnNext.Enabled = False
        Case "RESULT"
            grpBet.Enabled = False
            btnNext.Enabled = True  ' 次へボタンを有効化
        Case Else
            grpBet.Enabled = False
            btnNext.Enabled = False
    End Select
End Sub
```

### TcpSocket の接続（FormServer 例）
```visualbasic
' FormServer.Designer.vb の Load で、TcpSocket を貼り付けて SynchronizingObject を設定
Private Sub FormServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    ' TcpSocket コンポーネント（デザイナで追加）
    ' tcpSockets.SynchronizingObject = Me  （重要: UI スレッドでイベントを受ける）
    
    ' サーバ待機開始
    tcpSockets.OpenAsServer(9000)  ' ポート番号はプレースホルダ
    AppendLog("Server waiting...")
End Sub
```

---

## 連絡・注意

- UI と通信は同時に触ると壊れやすいので、
  可能なら「通信タスク」「UIタスク」を分けて進めるのがおすすめです。
- 受信イベントは、受信が分割/結合することがあります。まず **行フレーミング** を優先で作る方針です。
- フォーム追加・削除時は、`My Project/Application.myapp` の設定がズレないか確認してください。

---

## ライセンス
必要なら記載してください。
