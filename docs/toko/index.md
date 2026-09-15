# .github/workflows/toko.yml

## 概要
[.github/workflows/toko.yml]({{ site.github.repository_url }}/blob/{{ site.github.build_revision }}/.github/workflows/toko.yml) は、従来のビルド投稿作業に相当する成果物チェックを実行する CI モジュールです。

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

    click Start "{{ site.github.repository_url }}/blob/{{ site.github.build_revision }}/.github/workflows/toko.yml#L5"
    click Checkout "{{ site.github.repository_url }}/blob/{{ site.github.build_revision }}/.github/workflows/toko.yml#L12"
    click checkout "{{ site.github.repository_url }}/tree/{{ site.github.build_revision }}/actions/checkout"
    click Diff "{{ site.github.repository_url }}/blob/{{ site.github.build_revision }}/.github/workflows/toko.yml#L18"
    click diff "{{ site.github.repository_url }}/tree/{{ site.github.build_revision }}/actions/diff"
    click Build "{{ site.github.repository_url }}/blob/{{ site.github.build_revision }}/.github/workflows/toko.yml#L23"
    click build "{{ site.github.repository_url }}/tree/{{ site.github.build_revision }}/actions/build"
    click FileCheck "{{ site.github.repository_url }}/blob/{{ site.github.build_revision }}/.github/workflows/toko.yml#L47"
    click filecheck "{{ site.github.repository_url }}/tree/{{ site.github.build_revision }}/actions/file-check"
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
#### PowerShell 7(pws) をインストールする
`powershell` で以下を実行する。
```powershell
winget install --id Microsoft.PowerShell -e --source winget --scope machine --installer-type wix
pwsh -v
# 出力例
# PowerShell 7.6.5
```

#### dotnet-script(csx) をインストールする
**管理者権限** で起動した `powershell` で以下を実行する。
```powershell
# dotnet-script をインストールするディレクトリを指定する（ユーザーディレクトリは不可）
# 入力例：C:\tool
$new_path = "path/to/dotnet-script/dir"

# .Net SDK 10 をインストール
winget install --id=Microsoft.DotNet.SDK.10 -e --source winget --scope machine

# インストール済み SDK を確認
dotnet --list-sdks
# 出力例
# 10.0.400 [C:\Program Files\dotnet\sdk]

# dotnet-script をインストール
dotnet tool install dotnet-script --tool-path $new_path

# システム環境変数にパスを追加
$paths = [Environment]::GetEnvironmentVariable("Path", "Machine")
[Environment]::SetEnvironmentVariable("Path", "$paths;$new_path", "Machine")

# インストール確認
dotnet-script -v
# 出力例 : 2.0.1
```

#### actions/checkout に関する実行環境を構築する.
[actions/checkout#実行環境]({{ site.github.repository_url }}/tree/{{ site.github.build_revision }}/actions/checkout#%E5%AE%9F%E8%A1%8C%E7%92%B0%E5%A2%83)

#### actions/build に関する実行環境を構築する
[actions/build#実行環境]({{ site.github.repository_url }}/tree/{{ site.github.build_revision }}/actions/build#%E5%AE%9F%E8%A1%8C%E7%92%B0%E5%A2%83)
