# 001-R BaseLib-StS2 能力调研报告

> 状态：定稿 v1.0（2026-08-23）。基于本地仓库 `C:\Users\<用户名>\Desktop\Mod\STS2-Reference\BaseLib-StS2`（v3.4.0，浅克隆）＋游戏 111 反编译源码＋ModTemplate-StS2 模板，并交叉核对 BaseLib 官方 Wiki（alchyr.github.io/BaseLib-Wiki）与 ModTemplate Wiki。
> 结论先行：**BaseLib 是"新增内容"导向的框架，没有修改既有 vanilla 事件/遗物/Ancient 的通用 API；修改达弗需要自写 Harmony patch。** BaseLib 的价值在于新遗物注册、配置界面、存档、本地化、跨版本兼容工具。

## 1. 项目概况

| 项 | 结论 |
|---|---|
| 定位 | 供其他 mod 依赖的基础库（base mod）：内容注册、配置、本地化、存档、Hook、场景转换等 |
| 版本 | 本地副本 v3.4.0（2026-07-31，HEAD 45c8549）；工作坊已安装（id 3737335127），用户环境就绪 |
| 许可 | MIT |
| 运行时依赖 | 无（`"dependencies": []`）；编译期引用游戏目录 sts2.dll / 0Harmony.dll / SmartFormat.dll / Steamworks.NET.dll，并用 Krafs.Publicizer 公开化 sts2 |
| 安装 | ①运行：dll+pck+json 放入游戏 mods 目录（或工作坊订阅）；②开发：NuGet `Alchyr.Sts2.BaseLib`；③mod manifest 声明 `{"id":"BaseLib","min_version":"3.3.0"}` |
| 版本下限 | `min_game_version: "0.107.1"`（**注意：官方声明是 0.107.1，不含 0.107.0**，0.107.0 支持未确认） |
| 框架 | net9.0 / C# 14 / Godot.NET.Sdk 4.5.1 |

## 2. 模块地图（src 目录）

- `Abstracts/`：全部 `Custom*Model` 抽象基类（Relic/Card/Event/Ancient/Encounter/Act/Character/Potion/Power/Orb/Monster/Pile/Reward/RestSiteOption/Badge/Modifier/Enchantment/Resource/TemporaryPower）+ `ILocalizationProvider` 与各 Loc record。
- `Patches/`：Harmony 补丁全集（Content 注册注入 / Localization / Saves 扩展存档 / UI / Hooks 触发点 / Features / Fixes / Audio / Networking / Compatibility / Utils）。
- `Hooks/`：BaseLib 自有 hook 接口（IAfterScryed、IModifyScryAmount、IHealAmountModifier、IMaxHandSizeModifier 等）＋分发器 `BaseLibHooks`（Dispatch/Modify/Aggregate/…）。
- `Utils/`：`OptionPools`/`AncientOption`/`WeightedList`（**带权重的 Ancient 选项池，仅自定义 Ancient 用**）、`CustomLocTableManager`、`SpireField`/`SavedSpireField`、`WhatMod`（mod 归属溯源）、`ReflectionUtils`、`ModInterop/`（跨 mod 软依赖）、`NodeFactories/`、`Patching/`（IL 匹配/Transpiler 工具）、`FmodAudio`、`CommonActions`、`MonsterActions`。
- `Config/`：`ModConfig` / `SimpleModConfig`（静态属性自动生成设置 UI）/ `ModConfigRegistry`。
- `Extensions/`：RelicModelExtensions、ModelDbExtensions、HarmonyExtensions 等。
- `Cards/`：BaseLibKeywords、自定义动态变量（ScryVar/ExhaustiveVar/PersistVar/RefundVar 等）。
- `BaseLib/`、`BaseLibScenes/`：打包资源（7 语言本地化含 zhs、图片、场景）、自定义 Godot 节点。
- `docs/`：auto_conversion.md（场景自动转换）、FmodAudio.md（695 行音频手册）。
- 入口：`BaseLibMain.cs` —— `[ModInitializer(nameof(Initialize))]`，初始化配置/本地化后 `MainHarmony.TryPatchAll(assembly)`。

## 3. 关键 API（按用途）

### 3.1 mod 声明与初始化
- 游戏侧 `[ModInitializer("方法名")]`（MegaCrit.Sts2.Core.Modding.ModInitializerAttribute）：标注后游戏调用指定静态方法；无标注则自动 Harmony.PatchAll。**BaseLib 没有 ModData/ModInfo 类**，身份由 manifest json 声明。
- `MainHarmony` 是 BaseLib internal，mod 应自建 `new Harmony(你的ModId)`。

### 3.2 内容注册（统一套路：继承 Custom* → 构造器 autoAdd 自动注册 → ID 自动加命名空间前缀，`[CustomID]` 可覆盖）
- 新遗物：`CustomRelicModel` + `[Pool(typeof(某RelicPool))]` + `CustomRelicPoolModel`（新池）。
- 新事件：`CustomEventModel`（`Acts` 属性决定 act 专属/共享；`Option()`/`LockedOption()` 构造选项；自动生成 loc key）。
- **新 Ancient：`CustomAncientModel`**（`MakeOptionPools` 定义 1~3 个选项池、`IsValidForAct` 限定幕、`ShouldForceSpawn` 强制出现、`DefineDialogues` 自动装配对话、自定义场景/图标路径）。`ActModelGenerateRoomsPatch.ForceAncientToSpawn` 可强制替换某房间的 Ancient。
- 新卡牌/卡池/遭遇/整幕/休息处选项/药水/能力/球/怪物/徽章/modifier/附魔/资源/卡堆/奖励/角色：同名 `Custom*Model`。
- 游戏侧底层：`ModHelper.AddModelToPool(poolType, modelType)`、`ModHelper.SubscribeForRunStateHooks/CombatStateHooks`。

### 3.3 修改既有内容的能力（本次需求的关键）
**BaseLib 没有"覆盖 vanilla 对象行为"的通用注册表**。多数 patch 是条件式 prefix（`if (__instance is not CustomXxx) return true`），只对自定义实例生效。定向能力仅：
- `RelicImageOverridePatch.AddOverride<TRelicType>(RelicIconData, Func<RelicModel,bool>?)`：给**已有遗物**（含 vanilla）替换图标，可加条件。
- 文本：`DescriptionOverrides.CustomizeDescription`（全局描述修饰）、`ExtraTooltips`/`CustomTooltips`；`FmodAudio.RegisterFileReplacement`（替换已有音效）。
- 游戏自身 Hook：`Hook.ShouldAllowAncient(IRunState, Player, AncientEventModel)` —— 只能整体允许/禁止进入某 Ancient，**不能改选项**。
- 自写 Harmony 配套：`HarmonyExtensions.TryPatchAll`、`Utils/Patching/`（InstructionPatcher/InstructionMatcher/CallMatcher/AsyncMethodCall Transpiler 辅助）。

### 3.4 配置与存档
- `SimpleModConfig`：静态属性 → 自动生成设置 UI（tickbox/slider/dropdown/lineedit/colorpicker/button）；`ModConfigRegistry.Register(modId, config)`；JSON 存 `OS.GetUserDataDir()/mod_configs/<名字>.cfg`。
- 存档：`ExtendedSaveHandlers<DataType, SerializableType>.RegisterSave<T>(id, getter, setter)`（T 实现 IPacketSerializable）；`[SavedProperty]`/`SavedSpireField<T1,T2>` 标注字段自动入档。

### 3.5 本地化
- 类内注入：实现 `ILocalizationProvider`，`Localization` 属性返回 `RelicLoc/CardLoc/EventLoc/...`，`ModelLocPatch` 在 ModelDb.Init 后注入。
- 整表注入：`CustomLocTableManager.Register("mytable.json")`。
- 游戏原生：`res://{modId}/localization/{lang}/*.json` 按语言探测（BaseLib 自带 zhs 等 7 语言）。
- Ancient 对话：`AncientDialogueUtil` + `AddAncientDialogues`（按 `{ancientId}.{characterId}` 键补对话）。

### 3.6 版本兼容（107 vs 111 重点）
- `Utils/BetaMainCompatibility.cs`：**分支级（main/beta）候选签名反射适配**，不是精确版本表：
  - `VariableMethod` / `VariableReference<T>`：一组候选签名，运行期反射选第一个命中的（例：`NControllerManager.IsUsingController → IsUsingDirectionalNavigation`，注释明确 109→110 变化）。
  - `BetaMainCompatibility.Version` 读 `ReleaseInfoManager.SemVer`，失败默认 999.999.999。
- `PostModInitPatch` 双阶段初始化：LocManager.Initialize 前（WhatMod 建表、CanModifyGameplay 判定）+ ModelDb.InitIds 前（SavedProperty/SavedSpireField 注册）。
- **实践结论**：BaseLib 的跨版本策略是"候选签名反射 + 惰性降级"；mod 自写 patch 需对 107/111 反编译产物分别核对目标方法签名（本地 `1.STS2-107Reference` 与 `2.STS2-111Reference` 可用）；游戏 v110→v111 mod 相关差异另见参考库 `v110-to-v111-compat.md`。

## 4. 对"修改达弗事件"的可用性评估

达弗 = `MegaCrit.Sts2.Core.Models.Events.Darv : AncientEventModel`（vanilla，非自定义）。当前实现（v0.111）：私有静态 `_validRelicSets`（11 组 `ValidRelicSet{filter, relics}`）→ `GenerateInitialOptions`（protected virtual）按 filter 过滤后逐集合 `Rng.NextItem` 均匀抽取 → shuffle → `NextBool()` 50% 追加 DustyTome（2+1 选项）否则 3 选项。**无权重概念。**

| BaseLib API | 能否作用于 vanilla Darv |
|---|---|
| `CustomAncientModel.MakeOptionPools`/`OptionPools`/`WeightedList`/`AncientOption<T>(weight)`（带权重选项池） | ❌ 仅新建自定义 Ancient |
| `CustomContentDictionary.AddAncient`/`ForceAncientToSpawn` | ❌ 只遍历自定义 Ancient 列表 |
| `AddCustomAncientSpawnCondition` | ❌ 仅被 OptionPools.Roll 消费 |
| `Hook.ShouldAllowAncient`（游戏自身） | ⚠️ 只能整体禁入，改不了选项 |
| BaseLib 对 EventModel/AncientEventModel 的全部 patch | ❌ 无任何 patch 触碰 GenerateInitialOptions/AllPossibleOptions |

**结论：修改达弗选项池与概率必须自写 Harmony patch**（推荐 prefix `Darv.GenerateInitialOptions()` 或替换 `AllPossibleOptions`，权重逻辑可参考 BaseLib 的 `WeightedList`/`AncientOption` 模式；Darv 的 protected `RelicOption(RelicModel,...)` 可复用——内含 RelicCmd.Obtain + Done()）。107/111 双版本需分别核对 `GenerateInitialOptions` 签名（经比对：**两版本该方法体一致**，仅静态构造器与过滤条件不同——此点已由达弗现状报告确认，修正了本调研最初"未确认"的标注）。

## 5. 示例与文档资源
- 仓库内无 samples/examples；官方样例 = ModTemplate-StS2（3 个 dotnet new 模板：Mod/Content/Character，均带 BaseLib 依赖）。
- Wiki：https://alchyr.github.io/BaseLib-Wiki/（Features List 为入口；CustomAncientModel / CustomRelicModel 页面已抓取留档）。
- ModTemplate Wiki：Adding-Ancients / Modding-Basics / Setup / Decompiling / Extracting-Assets-and-Text（前 3 页已抓取留档）。

## 6. 局限与注意事项
1. 无覆盖 vanilla 内容的通用 API；改既有行为基本都要自写 Harmony（BaseLib 只提供图标/文本/音效等零星定向工具）。
2. 官方声明 min_game_version 0.107.1（0.107.0 未确认）；跨版本自写 patch 需自己比对反编译。
3. 本地 git 为浅克隆（1 提交），无历史；v3.4.1 tag 对象缺失。
4. 编译要求：net9.0 + Godot 4.5.1 SDK + 游戏安装目录（Publicize 引用）；GodotPath 缺省只出 dll 不出 pck。
5. `ShouldForceSpawn` 官方警告乱用会造成 mod 冲突；自定义 Ancient 需用 `IsValidForAct` 收敛。

## 附：相关链接
- 仓库：https://github.com/Alchyr/BaseLib-StS2/（README 见本地副本）
- Wiki：https://alchyr.github.io/BaseLib-Wiki/ ｜ Features: https://alchyr.github.io/BaseLib-Wiki/docs/Features.html
- 模板：https://github.com/Alchyr/ModTemplate-StS2 ｜ Adding-Ancients: https://github.com/Alchyr/ModTemplate-StS2/wiki/Adding-Ancients
- 工作坊：https://steamcommunity.com/sharedfiles/filedetails/?id=3737335127
