# actions/checkout
```actions/checkout``` は、ワークフローを呼び出したリポジトリをチェックアウトし、対象のブランチ・コミットを出力するモジュールです。

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
