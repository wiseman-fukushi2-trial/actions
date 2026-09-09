# actions/build
```actions/build``` は、指定されたプロジェクトのビルドを実施し、その成否をサマリーとして出力するモジュールです。

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
