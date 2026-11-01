# 007-T RitsuLib 设置多语言与重启方案

> 状态：定稿 v1.0（2026-08-23）。来源：`STS2-RitsuLib` 仓库 src/ 与 docs/pages/guide/、`ritsulib-decompiled-core`、游戏反编译 `2.STS2-111Reference\decompiled`。

## 1. 结论速览

1. **ModSettings 设置界面文本支持按玩家语言自动切换**（含游戏内即时切换，无需重启）——官方推荐做法。
2. **mod 可在运行时读取玩家语言**：`I18N.ResolveCurrentLanguageCode()` 或 `LocManager.Instance.Language`（三字母代码）。
3. **"保存后自动重启"技术上可行，但游戏无内置重启 API**；自实现需 `OS.CreateProcess` 重拉 exe（有 Steam 绕过风险）；JmcModLib 提供现成实现（ritsulib 有集成入口，用户工作坊已装 JmcModLib）。

## 2. ModSettings 多语言支持（API 证据）

### 2.1 文本参数类型 = 延迟解析的 ModSettingsText
- `AddToggle/AddIntSlider/AddChoice/AddButton` 的 label/description 参数均为 `ModSettingsText`（`src\Settings\ModSettings\Core\ModSettingsBuilders.cs:668-813, 1164-1175`）；页面/节标题同理（`:117-131, 413-427`）。
- UI 渲染时逐条 `ModSettingsUiContext.Resolve(text)` → `text.Resolve()` 取当前语言字符串（`src\Settings\ModSettingsUi\Core\ModSettingsUiContext.cs:36-40`）。

### 2.2 四种文本工厂（`src\Settings\ModSettings\Text\ModSettingsText.cs`）
| 工厂 | 行为 |
|---|---|
| `Literal(string)` | 固定文本，不本地化 |
| `Dynamic(Func<string>)` | 每次 Resolve 重新计算（可自定义任意取词逻辑） |
| `LocString(table, key, fallback)` | 按游戏 LocTable 查词，失败回退 fallback |
| `I18N(I18N, key, fallback)` | 通过 ritsulib I18N 系统解析（推荐） |

### 2.3 语言切换自动刷新链路
- `I18N` 构造时订阅 `LocManager.Instance.SubscribeToLocaleChange`（`src\Utils\I18N.cs:286-310`），切语言后 `ForceReload()` 重载（`:339-347`）。
- 设置 UI 订阅语言变更，`OnLocaleChanged` 后**全量重建**设置界面（`src\Settings\ModSettingsUi\Core\RitsuModSettingsSubmenu.cs:3324-3351`）。
- I18N 回退链：当前语言 → eng（`I18N.cs:462-468`）——**恰好实现"zhs/zht 显示中文，其余语言显示英文"**（只需提供 eng/zhs/zht 三份 json）。

### 2.4 推荐用法
```csharp
var i18n = RitsuLibFramework.CreateModLocalization(modId, "settings",
    pckFolders: ["res://MyMod/localization/settings"]);
RitsuLibFramework.RegisterModSettings("MyMod", page => page
    .WithTitle(ModSettingsText.I18N(i18n, "settings.page.title", "Fallback"))
    .AddToggle("ironclad",
        ModSettingsText.I18N(i18n, "settings.ironclad.label", "Ironclad"),
        new ModSettingsValueBinding<MySettings, bool>(...),
        ModSettingsText.I18N(i18n, "settings.ironclad.description", "...")));
```
- 文件：`localization/settings/{eng,zhs,zht}.json`，key 同名。

## 3. 语言查询 API

- 游戏侧：`LocManager.Instance.Language`（三字母代码，`LocManager.cs:141`）；`CultureInfo`（`:145`）；`SubscribeToLocaleChange`（`:597-615`）。
- ritsulib 侧：`I18N.ResolveCurrentLanguageCode()`（public static，`I18N.cs:607-633`，含 `zh_CN→zhs`、`en→eng` 等规范化，zht 原样保留）；`I18N.NormalizeLanguageCode`（`:651-672`）。
- 判断"中文玩家"：`I18N.ResolveCurrentLanguageCode() is "zhs" or "zht"`。
- 游戏内切语言即时生效（`NLanguageDropdown.cs:128-140`：SetLanguage + Relocalize），**无需重启**。

## 4. 保存回调与自动重启

### 4.1 保存时机
- ritsulib 设置 UI 是**防抖自动保存**（`FlushDirtyBindings`，`RitsuModSettingsSubmenu.cs:3297-3322`），无页面级 OnSave 钩子。
- 显式保存：`binding.Save()`（`ModSettingsValueBindings.cs:109-112` → GetDataStore.Save）。
- 按钮回调：`AddButton` 的 `Action`（可执行任意代码，`ModSettingsBuilders.cs:1164-1175`）。

### 4.2 游戏侧退出/重启
- 优雅退出：`NGame.Instance.Quit()`（`NGame.cs:978-1001`，保存后 GetTree().Quit）。
- **无内置重启 API**（全解包无 GameRestart/Restart 相关）。
- 自实现：`OS.CreateProcess(OS.GetExecutablePath(), OS.GetCmdlineUserArgs())` → `GetTree().Quit(0)`。
  - 风险：Steam 启动流程绕过（AppID 环境变量/Overlay/DRM）；需保留原命令行参数（`--force-steam`、`+connect_lobby` 等）；工作目录需一致。
- **JmcModLib 现成方案**：`JmcModLib.Utils.GameRestart.ShowRestartConfirmationAsync(true, assembly)`（ritsulib 的 `JmcModLibMirrorSource.cs:871-897` 已集成该入口；用户工作坊已装 JmcModLib，3747526103）。

## 5. 对"zhs/zht→中文，其他→英文"的落地方案（结论）

- **首选**：eng.json + zhs.json + zht.json 三份 + `ModSettingsText.I18N`——零自定义逻辑，自动切换（含游戏内即时切换）。
- 备选（只维护两份时）：`ModSettingsText.Dynamic(() => I18N.ResolveCurrentLanguageCode() is "zhs" or "zht" ? zh : en)`。
- ⚠️ 不要用 `CreateModLocalizationWithFallback(fallbackLanguage: "zhs")`——fallback 对英文玩家同样生效，会造成英文玩家缺键看到中文。
- ⚠️ 游戏文本（新遗物 title/description）同理：mod pck 内 `localization/{eng,zhs,zht}/*.json`（游戏原生机制，两库均支持）。

## 6. 关键文件索引
- `src\Settings\ModSettings\Text\ModSettingsText.cs`、`Core\ModSettingsBuilders.cs`、`Binding\ModSettingsValueBindings.cs`、`ModSettingsBindingWriteEvents.cs`
- `src\Utils\I18N.cs`；`src\Settings\ModSettingsUi\Core\RitsuModSettingsSubmenu.cs:3324-3351`
- `src\Settings\ModSettings\Mirrors\JmcModLib\JmcModLibMirrorSource.cs:871-897`
- 游戏：`LocManager.cs`、`LocString.cs`、`NGame.cs:978-1001`、`NLanguageDropdown.cs:128-140`

**未确认项**：JmcModLib GameRestart 内部实现（未解包）；Steam RestartAppIfNecessary 可用性；LocManager 遥测期临时覆盖为 eng 的干扰频率。
