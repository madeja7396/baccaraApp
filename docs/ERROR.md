# ビルド時発生エラー
## 出力
```
4:29 で再構築が開始されました...
1>------ すべてのリビルド開始: プロジェクト:Baccarat.Shared, 構成: Debug Any CPU ------
D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Shared\Baccarat.Shared.vbproj を復元しました (22 ミリ秒)。
D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\Baccarat.Client.vbproj を復元しました (26 ミリ秒)。
D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Server\Baccarat.Server.vbproj を復元しました (26 ミリ秒)。
1>  Baccarat.Shared -> D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Shared\bin\Debug\net48\Baccarat.Shared.dll
2>------ すべてのリビルド開始: プロジェクト:Baccarat.Client, 構成: Debug Any CPU ------
3>------ すべてのリビルド開始: プロジェクト:Baccarat.Server, 構成: Debug Any CPU ------
3>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Server\Forms\FormServer.vb(1,9): warning BC40056: インポート 'Baccarat.Server.Net' で指定された名前空間または型が、パブリック メンバーを含んでいないか、あるいは見つかりません。名前空間または型が定義されていて、少なくとも 1 つのパブリック メンバーを含んでいることを確認してください。また、インポートされた要素名がエイリアスを使用していないことを確認してください。
3>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Server\Forms\FormServer.vb(7,26): error BC30002: 型 'ServerHost' は定義されていません。
3>D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Server\Program.vb(3,9): warning BC40056: インポート 'Baccarat.Server.Forms' で指定された名前空間または型が、パブリック メンバーを含んでいないか、あるいは見つかりません。名前空間または型が定義されていて、少なくとも 1 つのパブリック メンバーを含んでいることを確認してください。また、インポートされた要素名がエイリアスを使用していないことを確認してください。
2>  Baccarat.Client -> D:\wsl\Ubuntu\apps\Baccara\src\Baccarat.Client\bin\Debug\net48\Baccarat.Client.exe
========== すべて再構築: 2 正常終了、1 失敗、0 スキップ ==========
=========== リビルド は 4:29 で完了し、01.708 秒 掛かりました ==========


```

## エラー一覧
インポート 'Baccarat.Server.Net' で指定された名前空間または型が、パブリック メンバーを含んでいないか、あるいは見つかりません。名前空間または型が定義されていて、少なくとも 1 つのパブリック メンバーを含んでいることを確認してください。また、インポートされた要素名がエイリアスを使用していないことを確認してください。
型 'ServerHost' は定義されていません。
インポート 'Baccarat.Server.Forms' で指定された名前空間または型が、パブリック メンバーを含んでいないか、あるいは見つかりません。名前空間または型が定義されていて、少なくとも 1 つのパブリック メンバーを含んでいることを確認してください。また、インポートされた要素名がエイリアスを使用していないことを確認してください。