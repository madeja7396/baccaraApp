# Baccarat (WinForms VB)

対戦型バカラ（2人用・サーバ権威）を Visual Basic + Experiment.TcpSocket で実装するためのワークツリーです。現時点では骨組みのみを配置し、詳細は `AGENTS.md` と `docs/CONTEXT.md` に従います。

## 現在の状態
- ディレクトリ構成と主要コンポーネントのスタブを作成済み
- 共通ライブラリ（Shared）、サーバ、クライアントの3層に分離
- プロトコル/モデル/ルール/ユーティリティの最小インタフェースを用意

## 参照ドキュメント
- `AGENTS.md` : 詳細設計ドラフト
- `docs/CONTEXT.md` : 要件定義書 v1.1
- `docs/WORKTREE.md` : 本リポジトリの構成ガイド

## 次の一手（推奨）
1. `lib/` に Experiment.TcpSocket.dll を配置し、VB プロジェクトから参照（Copy Local=True）
2. Visual Studio で `Baccarat.sln` を作成し、`src/` 配下をプロジェクトとして追加
3. Shared の Framer/Parser/Logger を先に実装し、Server/Client から呼び出す
4. Sprint 0 スモークテスト（接続・切断・ログ）から着手
