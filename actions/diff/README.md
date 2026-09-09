# actions/diff
```actions/diff``` は、変更されたファイルと、それに対応する .vbproj のリストを出力するモジュールです。

## 使い方
```yaml
uses: wiseman-fukushi-dev/rv1.actions/actions/diff@main
```

### 呼び出し例
モジュールの呼び出し前に、[checkout モジュール](../checkout) を実行する必要があります。
```yaml
jobs:
  diff:
    runs-on: self-hosted
    steps:
      # リポジトリをクローン・チェックアウト
      - name: Checkout
        id: checkout
        uses: wiseman-fukushi-dev/rv1.actions/actions/checkout@main

      # 変更を取得
      - name: Get Diff
        id: diff
        uses: wiseman-fukushi-dev/rv1.actions/actions/diff@main
```

## 入力
なし（直前に実施された checkout を参照する）

## 出力
```yaml
outputs.changed_files : 差分ファイルのリスト
outputs.changed_projects : 差分プロジェクトの .vbproj のリスト
```
