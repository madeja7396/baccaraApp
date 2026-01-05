# ビルド時発生エラー
## 出力
```
3:47 でビルドが開始されました...
1>------ ビルド開始: プロジェクト: Baccarat.Client, 構成: Debug Any CPU ------
1>  ビルドの速度を上げるために、アナライザーをスキップしています。'ビルド' または '再ビルド' コマンドを実行してアナライザーを実行できます。
1>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\UI\UiBinder.vb(8,13): error BC30456: 'lblPhase' は 'FormGame' のメンバーではありません。
1>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\UI\UiBinder.vb(9,13): error BC30456: 'lblRound' は 'FormGame' のメンバーではありません。
1>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Forms\FormGame.vb(16,13): error BC30451: 'grpBet' は宣言されていません。アクセスできない保護レベルになっています。
1>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Forms\FormGame.vb(17,13): error BC30451: 'btnNext' は宣言されていません。アクセスできない保護レベルになっています。
1>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Forms\FormGame.vb(18,13): error BC30451: 'btnRules' は宣言されていません。アクセスできない保護レベルになっています。
1>vbc : error BC30420: 'Sub Main' が、'Baccarat.Client' に見つかりませんでした。
========== ビルド: 成功 0、失敗 1、最新の状態 1、スキップ 0 ==========
=========== ビルド は 3:47 で完了し、00.958 秒 掛かりました ==========


```

## エラー一覧
'lblPhase' は 'FormGame' のメンバーではありません。
'grpBet' は宣言されていません。アクセスできない保護レベルになっています。
'btnNext' は宣言されていません。アクセスできない保護レベルになっています。
'btnRules' は宣言されていません。アクセスできない保護レベルになっています。
'lblRound' は 'FormGame' のメンバーではありません。
'Sub Main' が、'Baccarat.Client' に見つかりませんでした。