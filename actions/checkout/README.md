# actions/checkout
`actions/checkout` は、ワークフローを呼び出したリポジトリをチェックアウトし、対象のブランチ・コミットを出力するモジュールです。

## 使い方
```yaml
uses: wiseman-fukushi-dev/rv1.actions/actions/checkout@main
```

### 呼び出し例
```yaml
jobs:
  checkout:
    runs-on: self-hosted
    steps:
      # リポジトリをクローン・チェックアウト
      - name: Checkout
        id: checkout
        uses: wiseman-fukushi-dev/rv1.actions/actions/checkout@main
```

## 入力
なし

## 出力
```yaml
outputs.branch : 対象のブランチ名
outputs.sha : 対象のコミット SHA
```

## 実行環境
このモジュールは、実行マシン上に、以下の条件が整っていることを前提として動作します。
- `Git` がインストールされている。
- システム環境変数 Path に Git インストールディレクトリが追加されている。

以下では、`winget` でインストールを実行する方法を紹介します。

**管理者権限** で起動した `powershell` で以下を実行する。
```powershell
winget install --id Git.Git -e --source winget --scope machine
git -v
# 出力例
# git version 2.55.0.windows.3
```
