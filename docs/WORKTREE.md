# WORKTREE ガイド

`AGENTS.md` で示された構成を実際のフォルダに落とし込んだものです。Visual Studio でソリューションを作る際のマッピングにも使えます。

## ルート
- `README.md` : プロジェクト概要
- `AGENTS.md` : 詳細設計ドラフト（必読）
- `docs/CONTEXT.md` : 要件定義書
- `docs/WORKTREE.md` : 本ファイル
- `lib/` : 外部ライブラリ置き場（Experiment.TcpSocket.dll をここへ）
- `src/` : ソースコード（Shared/Server/Client）

## Shared (`src/Baccarat.Shared`)
- `Constants.vb` : 共有定数（Port/MaxRounds などプレースホルダ）
- `Protocol/` : コマンド定義、行フレーミング、パーサ
- `Model/` : Card/Hand/BetInfo/GameState などドメインモデル
- `Rules/` : ルール・配当インタフェースとプレースホルダ実装
- `Util/` : Logger、入力検証、共通ヘルパ

## Server (`src/Baccarat.Server`)
- `Forms/` : FormServer（ロビー兼用予定）のコードビハインド
- `Net/` : ServerHost、ClientSession などサーバ側の接続管理

## Client (`src/Baccarat.Client`)
- `Forms/` : FormLobby / FormGame / FormRules
- `UI/` : UiBinder, ImageLoader（BMP読込）
- `assets/cards/` : カード画像（BMP）。`BACK.bmp` と `S_01.bmp` 形式を想定
- `docs/` : クライアント用メモ（プロトコル・UI仕様の写経用）

## 今後の作業メモ
- `.sln` と `.vbproj` を作成し、上記フォルダをプロジェクトに割り当てる
- `lib/Experiment.TcpSocket.dll` を参照に追加し、`Copy Local=True`
- Shared をクラスライブラリ、Server/Client を WinForms アプリとして設定
- `assets/cards/` 配下に BMP を配置し、`ImageLoader` で相対パス読み込み
