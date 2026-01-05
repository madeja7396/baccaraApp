# ビルド時発生エラー
## 出力
```
4:26 で再構築が開始されました...
1>------ すべてのリビルド開始: プロジェクト:Baccarat.Shared, 構成: Debug Any CPU ------
D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Shared\Baccarat.Shared.vbproj を復元しました (6 ミリ秒)。
D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Server\Baccarat.Server.vbproj を復元しました (9 ミリ秒)。
D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Baccarat.Client.vbproj を復元しました (9 ミリ秒)。
1>  Baccarat.Shared -> D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Shared\bin\Debug\net48\Baccarat.Shared.dll
2>------ すべてのリビルド開始: プロジェクト:Baccarat.Client, 構成: Debug Any CPU ------
3>------ すべてのリビルド開始: プロジェクト:Baccarat.Server, 構成: Debug Any CPU ------
3>  Baccarat.Server -> D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Server\bin\Debug\net48\Baccarat.Server.exe
2>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Program.vb(3,9): warning BC40056: インポート 'Baccarat.Client.Forms' で指定された名前空間または型が、パブリック メンバーを含んでいないか、あるいは見つかりません。名前空間または型が定義されていて、少なくとも 1 つのパブリック メンバーを含んでいることを確認してください。また、インポートされた要素名がエイリアスを使用していないことを確認してください。
2>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Program.vb(10,29): error BC30002: 型 'FormLobby' は定義されていません。
2>vbc : error BC30738: 'Sub Main' が 'Baccarat.Client' で複数回宣言されています: Baccarat.Client.My.MyApplication.Main(Args As String()), Baccarat.Client.Program.Main()
========== すべて再構築: 2 正常終了、1 失敗、0 スキップ ==========
=========== リビルド は 4:27 で完了し、04.045 秒 掛かりました ==========


```

## エラー一覧
'Sub Main' が 'Baccarat.Client' で複数回宣言されています: Baccarat.Client.My.MyApplication.Main(Args As String()), Baccarat.Client.Program.Main()
型 'FormLobby' は定義されていません。
インポート 'Baccarat.Client.Forms' で指定された名前空間または型が、パブリック メンバーを含んでいないか、あるいは見つかりません。名前空間または型が定義されていて、少なくとも 1 つのパブリック メンバーを含んでいることを確認してください。また、インポートされた要素名がエイリアスを使用していないことを確認してください。