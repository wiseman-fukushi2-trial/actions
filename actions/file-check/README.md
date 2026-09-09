# actions/build
```actions/build``` は、指定されたファイルのバリデーションを実施し、その結果をサマリーとして出力するモジュールです。

## 使い方
```yaml
uses: wiseman-fukushi-dev/rv1.actions/actions/file-check@main
with:
  project-files: {{ .vbproj のリスト }}
```

### 呼び出し例
モジュールの呼び出し前に、[checkout モジュール](../checkout) を実行し、ソースコードをチェックアウトする必要があります。
```yaml
jobs:
  file-check:
    runs-on: self-hosted
    steps:
      # ファイルチェック
      - name: File Check
        id: file-check
        uses: wiseman-fukushi-dev/rv1.actions/actions/file-check@main
        with:
          branch: {{ ブランチ名 }}
          sha: {{ コミット SHA }}
          files: {{ 処理対象ファイルのリスト }}
          project-files: {{ 処理対象 .vbproj のリスト }}
```

## 入力
```yaml
branch: ブランチ名
sha: コミット SHA
files: 処理対象ファイルのリスト
project-files: 処理対象 .vbproj のリスト
```

## 出力
なし（サマリーの表示のみ）
