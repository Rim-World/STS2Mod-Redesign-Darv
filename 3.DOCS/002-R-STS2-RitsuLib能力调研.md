# 002-R STS2-RitsuLib 能力调研报告

> 状态：定稿 v1.0（2026-08-23）。基于本地仓库 `C:\Users\13167\Desktop\Mod\STS2-Reference\STS2-RitsuLib`（v0.5.10）＋解包 `ritsulib-decompiled-core`（与仓库 src 同版本 0.5.10，partial 拆分，无额外功能）＋`ritsulib-decompiled`（loader 侧）＋游戏 107/111 反编译源码；并核对工作坊实装版本与官方文档站。
> 结论先行：**RitsuLib 提供"向达弗追加新选项"的专门 API（`AncientOption<Darv>`），但"修改原有候选池/概率"无现成 API，需用其 Harmony 补丁脚手架自写 patch；塔1遗物本体可注册，进达弗池仍需 patch。** 与 BaseLib 不冲突、不互相依赖，可同时使用。

## 1. 项目概况

| 项 | 结论 |
|---|---|
| 定位 | 面向 STS2 mod 作者的共享框架库：内容注册、生命周期、Harmony 补丁、持久化、设置界面、本地化、音频、运行时 UI、诊断、兼容辅助 |
| 与 BaseLib 关系 | **不依赖 BaseLib、不替代 BaseLib**（"sits beside the base game API and other libraries such as BaseLib"）；提供 BaseLib 桥接（见 §3.7） |
| 版本 | 仓库 v0.5.10；**工作坊实装 v0.5.14**（用户环境），variant loader 的 `lib/` 下含 **0.107.1 / 0.109.0 / 0.110.0 / 0.111.0 四个兼容变体**——即工作坊版已适配 111 |
| 许可 | MIT（Copyright 2026 OLC） |
| 安装 | NuGet `STS2.RitsuLib`（编译）＋ manifest 依赖 `{"id":"STS2-RitsuLib"}`（运行时）；DLL-only（has_pck=false）；variant-pack 装一个 mods/STS2-RitsuLib/ 文件夹 |
| 版本下限 | `min_game_version: "0.107.1"`（0.107.0 不在官方目标列表；0.111.0 亦高于仓库源码声明，但工作坊 0.5.14 已带 0.111.0 变体） |
| 框架 | net9.0 / Godot.NET.Sdk 4.5.1 / Krafs.Publicizer |

## 2. 模块地图（src/，1205 个 .cs）

| 命名空间 | 职责 |
|---|---|
| `STS2RitsuLib`（根） | `RitsuLibFramework` 入口（2208 行，11 个分部类：PatcherSetup / DustyTome / OrobasAncientUpgrades / RelicVisibility / RightClick / UpdateChecks / ModRunRng / ModelIdentity / LifecycleOnce / Diagnostics） |
| `Scaffolding.Content` | 内容模板（ModCardTemplate/ModRelicTemplate/ModEventTemplate/ModAncientEventTemplate）＋`ModContentPackBuilder` |
| `Scaffolding.Ancients.Options` | **Ancient 初始选项注入：`ModAncientOptionRegistry` / `ModAncientOptionRule`** |
| `Content` | `ModContentRegistry`（核心注册器，含 ActEnter 槽位池、Ancient 目录、资产替换）＋ ModelDbContentPatches |
| `Relics` | 遗物注册、可见性、DustyTome/ArchaicTooth/TouchOfOrobas 专项注册器与 patch |
| `Lifecycle` / `Timeline` / `Unlocks` / `RunData` / `RunRngs` / `Saves` | 生命周期事件（80+）、时间线、解锁、跑局数据、跑局 RNG、存档 |
| `Patching` | `ModPatcher` / `IPatchMethod` / `DynamicPatch` / `PrivateAccess` / `HarmonyIl`（transpiler 工具） |
| `Compat` | `Sts2HostVersion` / `Sts2ApiCapabilityGate` / `ExternalFrameworkRegistry` / `RitsuModManager` |
| `Interop` | `ModTypeDiscoveryHub`、`AutoRegistration`（59 个注册属性）、mod 间互操作（JSON 通道/反射静态通道） |
| `Diagnostics` | 调试面板（**可强制触发任意 Ancient 并指定选项**）、DevConsole 补全、浏览器日志查看器、patch dump |
| `Settings.ModSettings` | 玩家设置页构建器；`Mirrors/` 下含 BaseLib/JmcModLib/ModConfig 设置**镜像** |
| `Data` / `Utils.Persistence` | ModDataStore、JSON 持久化/迁移/云镜像 |
| `Localization` | I18N、LocTable 桥、Ancient 对话本地化加载 |
| `Audio` / `Ui` / `TopBar` / `CardPiles` / `RuntimeInput` / `Networking` / `Platform` / `Updates` | FMOD 辅助、Shell 主题/Toast、顶栏按钮、自定义卡堆、快捷键、联机、更新检查 |

**loader 侧**（`components/loader/Bootstrap.cs`）：按 `Sts2HostVersion.Numeric` 探测宿主版本，从 `lib/<compat>/` 选匹配变体 DLL 加载。

## 3. 关键 API（按用途）

### 3.1 入口与初始化
- `RitsuLibFramework`（`[ModInitializer]`，`:407 Initialize()`）：Harmony 预载 → 设置存储 → 调试日志 → 注册 7 组框架 patch → `PatchAllRequired` → 发布 `FrameworkInitializedEvent`。
- mod 侧标准姿势：`ModTypeDiscoveryHub.RegisterModAssembly(modId, assembly)` → `EnsureGodotScriptsRegistered` → `CreateLogger(modId)` → `CreatePatcher(modId, name)` + `RegisterPatches<T>()` + `ApplyRequiredPatcher(patcher, DisableMod)`。

### 3.2 内容注册（三种方式等价）
1. **属性**：`[RegisterCard(typeof(Pool))]`、`[RegisterRelic(typeof(Pool))]`、`[RegisterSharedAncient]`、`[RegisterActAncient(typeof(Act))]`、`[RegisterSharedEvent]`、`[RegisterActEvent(typeof(Act))]` 等 59 个。
2. **Builder**：`ModContentPackBuilder`：`.Card<TPool,TCard>()`、`.Relic<TPool,TRelic>()`、`.SharedAncient<T>()`、`.ActAncient<TAct,TAncient>()`、`.AncientOption<TAncient>(rule)`、`.SharedRelicPool<T>()` … `Apply()`。
3. **注册器直调**：`ModContentRegistry.RegisterRelic/RegisterSharedAncient/RegisterActAncient/RegisterSharedRelicPool/...`（79 个方法）。
- 冻结规则：`IsFrozen` 后禁止注册（ModelDb 初始化前必须完成）。

### 3.3 修改"游戏内已有对象/逻辑"的能力（本次需求核心）

| 能力 | API | 说明 |
|---|---|---|
| **向既有 Ancient 追加选项** | `RitsuLibFramework.RegisterAncientOption<TAncient>(modId, rule)` / builder `.AncientOption<Darv>(ModAncientOptionRule.Single(ancient => new EventOption(...)))`；rule 支持 Condition/Priority/SkipDuplicateTextKeys | **只增不减**；注入点 = patch `AncientEventModel.GenerateInitialOptionsWrapper` 的 Postfix（`Scaffolding/Content/Patches/AncientEventInitialOptionsRegistryPatch.cs:43-64`） |
| **修改达弗候选池/概率** | ❌ 无现成 API | `_validRelicSets` 是 `private static`；须自写 patch（见 §4） |
| Act 进入槽位（替换/均匀池/加权池） | `RegisterActEnterForce/UniformPool/WeightedPool`（权重 = `Func<ActEnterResolveContext,double>`） | 只作用于 Act 槽位选择，**不是** Ancient 选项概率 |
| 通用 Harmony 补丁 | `CreatePatcher` → `IPatchMethod`、`RegisterDynamicPatch`、`ApplyRequiredPatcher`；`PrivateAccess.FieldRef/DeclaredGetterDelegate/DeclaredMethodDelegate`（访问私有成员）；`HarmonyIlRewriter/Pattern/AsyncIl`（transpiler） | 自写 patch 的完整脚手架 |
| 生命周期钩子 | `SubscribeLifecycle<TEvent>`（GameReady/RunStarted/CombatStarting/RelicObtained 等 80+） | — |
| 模型级 hook | HealHook、CardOnPlayHook、PlayerResourceHook、MaxHandSizeCalculator、ModelCapability | — |
| mod 专属 RNG | `ModRunRngRegistry.Get(runState, modId, streamId)` | 防污染跑局 RNG |
| 调试 | 调试面板可强制触发任意事件/Ancient 并指定选项；DevConsole 自动补全 | 开发验证利器 |

### 3.4 本地化 / 配置 / 存档
- 本地化：`CreateModLocalization(modId)` 系列（默认 `user://.../mod_data/{modId}/localization`）；I18N LocTable 桥；关键词注册；Ancient 对话 key 规则 `<ANCIENT>.talk.<CHARACTER>.<idx>-<line>.<speaker>`。
- 配置：`RegisterModSettings(modId, page => ...AddToggle/AddIntSlider/AddDropdown(..., ModSettingsValueBinding<...>))`；`GetDataStore(modId)`（SaveScope.Global/Profile/InMemory，可云同步）；`GetRunSavedDataStore`（跑局内数据，随存档/多人同步）。

### 3.5 版本兼容机制
- 编译期：`build/RitsuLib.CompatDefines.targets` 生成 `STS2_AT_LEAST_0_104_0`~`0_110_0` 常量，源码 `#if` 分支（如 LegacyInputMapCompat 仅 0.107.0≤x<0.108.0 生效）。
- 运行期：`Sts2HostVersion` 多级探测宿主版本 → loader 选 `lib/<compat>/` 变体（**用户环境 0.5.14 带 0.111.0 变体**）。
- 能力门控：`Sts2ApiCapabilityGate`、`ExternalFrameworkRegistry`。

### 3.6 BaseLib 互操作
- 类型探测识别 BaseLib（`ExternalFrameworkIds.BaseLib`）；功能桥：BaseLibHealthBarForecastBridge / VisualGraftBridge / MaxHandSizeBridge / TargetTypeBridge（检测到即接管或让位）。
- Harmony 顺序：`[HarmonyAfter(Const.BaseLibHarmonyId)]`；设置页镜像；动态枚举冲突检测告警。

## 4. 对"修改达弗事件"的可用性评估

| 需求 | ritsulib 现状 |
|---|---|
| 给达弗**追加**新选项（如塔1遗物） | ✅ `RegisterAncientOption<Darv>` / `.AncientOption<Darv>(rule)`（Postfix 追加到选项列表末尾；选项内可用 `CreateModRelicOption<T>` 授予遗物并完成 Ancient 流程） |
| **修改达弗原有候选池**（_validRelicSets 11 组） | ❌ 无 API → 自写 patch（`CreatePatcher` + `IPatchMethod` + `PrivateAccess` 改静态字段，或 Prefix 替换 `GenerateInitialOptions` 返回） |
| **修改概率/抽取数量**（NextBool 50%、Take 2/3） | ❌ 无 API → 同上自写 patch |
| 引入塔1遗物（新内容） | ✅ `RegisterRelic<TPool,TRelic>()` 注册本体；进达弗池仍需 patch |
| 达弗在 Act 的出现概率 | ⚠️ 无 API；出现权重在游戏侧 RunManager/ActModel，需自写 patch（本次需求不涉及） |
| 调试验证 | ✅ 调试面板强制触发 Ancient + 指定选项 |

**一句话总评**：RitsuLib 对达弗需求——加选项现成（只增不减），改池与概率需自写 patch（它有完整脚手架），塔1遗物本体可注册但要进池仍要 patch。

## 5. 示例与文档资源
- 仓库无 examples 目录；官方文档站源码完整保存在本地仓库 `docs/pages/guide/`（31 页中英双语：getting-started / content-authoring-toolkit / custom-events（含 `AncientOption<TheArchitect>` 示例）/ patching-guide / persistence-guide / mod-settings / diagnostics-and-compatibility / debug-log-viewer 等）。
- 文档站线上版：https://sts2-ritsulib.ritsukage.com/
- 配套模板（第三方 alkaid616）：RitsuLibModTemplate（`dotnet new install STS2.RitsuLib.ModTemplate` → `dotnet new ritsulibmod -n MyMod`），含示例角色/卡/遗物与版本对齐 checklist；Wiki 中文：https://github.com/alkaid616/RitsuLibModTemplate/wiki/Home
- 推荐中文教程：SlayTheSpire2 Modding Tutorials（GlitchedReme）：https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html
- 可选 Roslyn 分析器（第三方 Miooowo）：STS2RitsuLibModAnalyzers

## 6. 局限与注意事项
1. **Ancient 选项注入只增不减**：不能删除/替换既有选项或改抽取概率。
2. **注册时机严格**：Ancient 选项规则须在 `ModContentRegistry.IsFrozen` 前注册（ModelDb 初始化前），冻结后抛异常。
3. **达弗候选池是游戏私有实现**：`private static` + `CollectionsMarshal` 构建，版本间可能变动；patch 时建议以方法/字段存在性做 guard（用户已有 mod 的"懒评估守卫"模式可复用）。
4. **版本边界**：仓库 0.5.10 官方目标 0.110.0/0.109.0/0.108.0/0.107.1（0.107.0 无、0.111.0 无常量）；**工作坊 0.5.14 已带 0.111.0 变体**，用户环境 107/111 双版本均有对应变体可跑。
5. 无"整类事件替换"API；改既有事件行为走通用 Harmony patch 通道。
6. `ActEnterWeightedPool` 的权重是 Act 槽位语义，勿与 Ancient 选项概率混用。
7. BaseLib 桥为"检测到即生效"，行为随 BaseLib 版本变化。

## 附：相关链接
- 仓库：https://github.com/BAKAOLC/STS2-RitsuLib/（README 见本地副本；中文 README.zh.md）
- 文档：https://sts2-ritsulib.ritsukage.com/ ｜ NuGet：STS2.RitsuLib / STS2.RitsuLib.Compat.<api>
- 模板：https://github.com/NothingFumo/RitsuLibModTemplate
- 教程：https://github.com/GlitchedReme/SlayTheSpire2ModdingTutorials
