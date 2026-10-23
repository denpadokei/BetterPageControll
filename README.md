# BetterPageControll

Beat Saber 1.40.8 のプレイリスト画面を、1.29.1 相当のグリッド表示に戻す HarmonyX パッチです。

> A HarmonyX mod for Beat Saber 1.40.8 that restores the 1.29.1 playlist grid layout (6 columns, vertical scrolling) and keeps the page dots clickable when there are many playlists.

## 概要

- プレイリストを 1.29.1 と同じ **6列** で表示
- **縦方向のみ**スクロール（横スクロールなし）
- プレイリストが多いときに **PageControl のページドットが反応しなくなる問題**を回避

1.40.8 ではプレイリストの `GridView` が「7列（表示6列）」でシリアライズされ、`AnimateOpen` がビューポートを横方向にも広げます。その結果、透明な `ContentInteractionArea` が `PageControl` の領域まで覆い、ページドットのポインター判定を妨げることがあります。本MODは 1.29.1 のレイアウト計算に戻すことでこれを防ぎます。

## 動作環境

| 項目 | 要件 |
| --- | --- |
| Beat Saber | 1.40.8 |
| BSIPA | 4.2.0 以上 |
| HarmonyX | BSIPA 環境の `Libs/0Harmony.dll` |
| 設定ファイル | なし |

> 1.40.8 専用です。他のバージョンでは private フィールド名やレイアウトが異なるため、動作しない可能性があります。

## インストール

1. Beat Saber を終了します。
2. `BetterPageControll.dll` を `Beat Saber/Plugins/` にコピーします。
3. ゲームを起動します。

`BetterPageControll.pdb` はデバッグ用です。通常のプレイには不要です。

## ビルド

### 必要なもの

- .NET SDK
- Beat Saber 1.40.8
- NuGet パッケージの復元（初回ビルド時）

### 手順

1. `BetterPageControll/BetterPageControll.csproj.user` に `GameDirectory` を設定します。ファイルが無い場合は、次の内容で作成してください。

   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
     <PropertyGroup>
       <GameDirectory>D:\BeatSaber\BSManager\BSInstances\1.40.8</GameDirectory>
     </PropertyGroup>
   </Project>
   ```

   `*.csproj.user` は Git 管理外のローカル設定です。

2. ソリューションをビルドします。

   ```powershell
   dotnet build BetterPageControll.sln
   ```

   `GameDirectory` をファイルに書かずに指定する場合は、次のようにビルドできます。

   ```powershell
   dotnet build BetterPageControll.sln -p:GameDirectory="D:\BeatSaber\BSManager\BSInstances\1.40.8"
   ```

3. ビルドに成功すると、次の DLL が生成され、`<GameDirectory>/Plugins` に自動コピーされます。

   ```text
   BetterPageControll/bin/Debug/BetterPageControll.dll
   ```

## CI

`.github/workflows/build.yml` で GitHub Actions による自動ビルドを行います。

- `BetterPageControll/**`、`BetterPageControll.sln`、ワークフロー自身の変更で実行
- `nicoco007/setup-beat-saber@v1` で Beat Saber 1.40.8 の参照アセンブリと BSIPA を取得
- Release ビルド後、`BetterPageControll/bin/Release/Artifact` を Artifact としてアップロード
  - 中身は `Plugins/BetterPageControll.dll` と `Plugins/BetterPageControll.pdb`
  - GitHub Actions 上では BSMT の既定動作で zip 作成が無効になるため、zip ではなく `Artifact` フォルダをアップロード
- 手動実行は GitHub の **Actions** タブ → **Build** → **Run workflow** から

## 仕組み

本MODはゲーム本体のファイルを書き換えず、HarmonyX で実行時に次の3点を変更します。

1. **6列に固定**
   `AnnotatedBeatmapLevelCollectionsGridView.SetData` の Prefix で、`GridView` の `_columnCount` と `_visibleColumnCount` を 1.29.1 と同じ 6 に設定します。

2. **横スクロールを無効化**
   `AnnotatedBeatmapLevelCollectionsGridViewAnimator.GetContentXOffset` の Prefix で常に `0` を返します。

3. **1.29.1 と同じ開閉アニメーション**
   `AnimateOpen` の Prefix で一時的に `_columnCount = _visibleColumnCount - 2`、`_padding = 0` にし、1.29.1 と同じ幅・高さで開きます。Postfix で元の値に戻します。

## ログ

初回のレイアウト適用時に、`Logs/_latest.log` に次のようなログを1回だけ出力します。

```text
[BetterPageControll] Legacy playlist grid: 6 columns (grid width ..., cell width ...).
```

## プロジェクト構成

```text
BetterPageControll/
├─ .github/
│  └─ workflows/
│     └─ build.yml
├─ .gitignore
├─ README.md
├─ BetterPageControll.sln
└─ BetterPageControll/
   ├─ BetterPageControll.csproj
   ├─ BetterPageControll.csproj.user  (ローカル設定・Git管理外)
   ├─ Plugin.cs
   ├─ Patches/
   │  ├─ AnnotatedBeatmapLevelCollectionsGridViewAnimatorPatch.cs
   │  └─ AnnotatedBeatmapLevelCollectionsGridViewPatch.cs
   ├─ Properties/
   │  └─ AssemblyInfo.cs
   ├─ manifest.json
   └─ Directory.Build.props
```

## 既知の制限

- Beat Saber 1.40.8 専用です。
- ゲーム更新で `GridView` / アニメーターの private フィールド名が変わると、パッチが適用されない可能性があります。
- 設定UIや設定ファイルはありません。

## 使用ライブラリ

- [BSIPA](https://github.com/bsmg/BeatSaber-IPA-Reloaded)
- [HarmonyX](https://github.com/BepInEx/HarmonyX)

## 免責

本MODは非公式のMODであり、Beat Games / Beat Saber とは関係ありません。導入は自己責任で行ってください。

## 備考

プロジェクト名の `Controll` は、既存のプロジェクト名・アセンブリ名に合わせています。
