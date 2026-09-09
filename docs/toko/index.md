# .github/workflows/toko.yml

## 概要
[.github/workflows/toko.yml](https://github.com/wiseman-fukushi-dev/rv1.actions/blob/main/.github/workflows/toko.yml) は、従来のビルド投稿作業に相当する成果物チェックを実行する CI モジュールです。

## フロー
```mermaid
flowchart TD
    Start([workflow_call])
    
    CheckoutGroup[リポジトリのチェックアウト]
    Checkout[Checkout Caller Repository]
    checkout[[actions/checkout]]
    
    DiffGroup[差分取得]
    Diff[Get Diff]
    diff[[actions/diff]]
    
    BuildGroup[ビルド]
    Build[Build]
    build[[actions/build]]
    
    FileCheckGroup[ファイルチェック]
    FileCheck[File Check]
    filecheck[[actions/file-check]]
    End([end])
    
    Start --> CheckoutGroup
    
    subgraph CheckoutGroup[リポジトリのチェックアウト]
    direction TB
    Checkout --> checkout
    end
    
    CheckoutGroup --> DiffGroup
    
    subgraph DiffGroup[差分取得]
    direction TB
    Diff --> diff
    end
    
    DiffGroup -- changed_projects --> BuildGroup
    
    subgraph BuildGroup[ビルド]
    direction TB
    Build --> build
    end
    
    DiffGroup -- changed_projects<br>changed_files --> FileCheckGroup
    
    subgraph FileCheckGroup[ファイルチェック]
    direction TB
    FileCheck --> filecheck
    end
    
    BuildGroup --> End
    FileCheckGroup --> End

    click Start "https://github.com/wiseman-fukushi-dev/rv1.actions/blob/main/.github/workflows/toko.yml#L5"
    click Checkout "https://github.com/wiseman-fukushi-dev/rv1.actions/blob/main/.github/workflows/toko.yml#L12"
    click checkout "https://github.com/wiseman-fukushi-dev/rv1.actions/tree/main/actions/checkout"
    click Diff "https://github.com/wiseman-fukushi-dev/rv1.actions/blob/main/.github/workflows/toko.yml#L18"
    click diff "https://github.com/wiseman-fukushi-dev/rv1.actions/tree/main/actions/diff"
    click Build "https://github.com/wiseman-fukushi-dev/rv1.actions/blob/main/.github/workflows/toko.yml#L23"
    click build "https://github.com/wiseman-fukushi-dev/rv1.actions/tree/main/actions/build"
    click FileCheck "https://github.com/wiseman-fukushi-dev/rv1.actions/blob/main/.github/workflows/toko.yml#L47"
    click filecheck "https://github.com/wiseman-fukushi-dev/rv1.actions/tree/main/actions/file-check"
```

## 環境構築
### GitHub
#### Actions 内で Pull Request を参照できるようにする
Organization と 当リポジトリ それぞれで、以下にチェックを入れる。
```
Settings > Actions > General
Workflow permissions > Allow GitHub Actions to create and approve pull requests
```

### Server
このモジュールは、self-hosted runner 上で実行される。  
ランナーを登録したサーバーで以下の環境を構築する。
#### PowerShell7(pws) をインストールする
```powershell
winget install --id Microsoft.PowerShell --source winget --installer-type wix
pwsh -v
```

#### dotnet-script(csx) をインストールする
```powershell
winget install Microsoft.DotNet.SDK.10
dotnet --list-sdks

dotnet tool install dotnet-script --tool-path [PATH]
# システム環境変数に ```[PATH]``` を追加
dotnet-script -v
```
