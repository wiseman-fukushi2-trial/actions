# actions/build
`actions/build` は、指定されたプロジェクトのビルドを実施し、その成否をサマリーとして出力するモジュールです。

## 使い方
```yaml
uses: wiseman-fukushi-dev/rv1.actions/actions/build@main
with:
  project-files: {{ .vbproj のリスト }}
```

### 呼び出し例
```yaml
jobs:
  build:
    runs-on: self-hosted
    steps:
      # ビルド
      - name: Build
        id: build
        uses: wiseman-fukushi-dev/rv1.actions/actions/build@main
        with:
          project-files: {{ .vbproj のリスト }}
```

## 入力
```yaml
project-files: .vbproj のリスト
```

## 出力
なし（サマリーの表示のみ）

## 実行環境
このモジュールは、実行マシン上に、以下の条件が整っていることを前提として動作します。
- **最新版** の `MSBuild` がインストールされており、システム環境変数 `MSBUILD` で参照できる。
- **2019版** の `MSBuild` がインストールされており、システム環境変数 `MSBUILD_2019` で参照できる。

以下では、**Build Tools for Visual Studio のインストールによって、MSBuild を取得する** 手順を紹介します。

1. **Build Tools for Visual Studio** のインストーラーをダウンロードする。
   - 最新版（執筆当時 2026）
   - 2019   
   [https://my.visualstudio.com/downloads](https://my.visualstudio.com/downloads?q=build%20tools%20for%20visual%20studio)
2. それぞれのインストーラーを実行し、インストールを行う。
3. それぞれの MSBuild が配置されているパスに、**システム環境変数** を設定する。(末尾のバックスラッシュは無し)
   `MSBUILD`
   ```
   C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin
   ```
   `MSBUILD_2019`
   ```
   MSBuild 2019 のパス
   C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin
   ```
4. **管理者実行** でコマンドプロンプトを実行し、以下のコマンドを実行する。
   ```cmd
   "%MSBUILD%\msbuild.exe" -version
   "%MSBUILD_2019%\msbuild.exe" -version
   # 出力例
   # 
   # C:\Windows\System32> "%MSBUILD%\msbuild.exe" -version
   # MSBuild のバージョン 18.9.1+a81b43525 (.NET Framework)
   # 18.9.1.35102
   # 
   # C:\Windows\System32> "%MSBUILD_2019%\msbuild.exe" -version
   # .NET Framework 向け Microsoft (R) Build Engine バージョン 16.0.462+g62fb89029d
   # Copyright (C) Microsoft Corporation.All rights reserved.
   # 16.0.462.64354
   ```
   
> [!NOTE]
> 以上の手順と同様に、**Visual Studio** や **SDK** をインストールすることで MSBuild を取得することも可能です。
