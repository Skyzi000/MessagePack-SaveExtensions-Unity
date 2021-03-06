# MessagePack SaveExtensions Unity

[![GitHub package.json version (subfolder of monorepo)](https://img.shields.io/github/package-json/v/Skyzi000/MessagePack-SaveExtensions-Unity?filename=LocalSave%2Fpackage.json&label=LocalSave)](https://github.com/Skyzi000/MessagePack-SaveExtensions-Unity/tags)
[![GitHub License](https://img.shields.io/github/license/Skyzi000/MessagePack-SaveExtensions-Unity)](https://github.com/Skyzi000/MessagePack-SaveExtensions-Unity/blob/main/LICENSE.md)

[MessagePack for C#](https://github.com/neuecc/MessagePack-CSharp) を使ったUnity用セーブシステムの実験的実装。

## LocalSaveモジュールの特徴

- クラスやインスタンス単位での保存ディレクトリ名・ファイル名管理
- 自動バックアップ・リストア機能
- 書き込み時の検証機能
- 保存先パスの自動記憶システム

## インストール

[MessagePack for C#](https://github.com/neuecc/MessagePack-CSharp)とUnity 2020.3以降が必要です。

1. Window > Package Managerを開く
2. 左上の＋ボタンを押す
3. "Add package from git URL..."を選択
4. `https://github.com/Skyzi000/MessagePack-SaveExtensions-Unity.git`を入力

## 使い方

1. ILocalSaveDataを実装し、MessagePackObjectAttributeを付けたクラスを用意
2. 拡張メソッドで簡単セーブ&ロード！

## サードパーティライセンス

以下のライブラリが必要です（同梱はしていないので別途導入してください）。

[MessagePack for C#](https://github.com/neuecc/MessagePack-CSharp)

MIT License

Copyright (c) 2017 Yoshifumi Kawai and contributors  
<https://github.com/neuecc/MessagePack-CSharp/blob/master/LICENSE>
