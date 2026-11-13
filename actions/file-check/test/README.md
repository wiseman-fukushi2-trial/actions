# actions/file-check/test
`actions/file-check/test` は、`actions/file-check` に対するユニットテストを実行するモジュールです。

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
