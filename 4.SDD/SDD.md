# SDD 索引

本目录是《重做达弗（DarvRelicPoolMod）》的软件设计文档：总体设计一份，模块级文档按需增补，编号三位递增。

| 编号 | 文档 | 说明 |
|---|---|---|
| 001 | [总体设计](001-总体设计.md) | 设计目标、系统架构、模块划分、达弗选项生成数据流、补丁策略、设置数据结构与写入语义、遗物实现要点、异常兜底、兼容策略、构建与测试方案、未验证项 |

## 模块划分（当前）

| 模块 | 对应源码 | 设计文档 |
|---|---|---|
| 入口装配 | `ModEntry.cs` | 001 §2、§5 |
| 选项生成 | `DarvOptionPool.cs`、`VanillaFallbackPool.cs`、`DarvAllPossibleOptions.cs` | 001 §4 |
| 遗物 | `DarvModRelicPool.cs`、`Relics/*.cs` | 001 §8 |
| 设置 | `DarvSettings.cs`、`DarvSettingsUi.cs`、`DeferredSaveBinding.cs` | 001 §6、§7 |
| 补丁 | `Patches/*.cs` | 001 §5 |
| 构建与资源 | `build.ps1`、`export_presets.cfg`、`project.godot` | 001 §11 |

> 模块划分如需增减，先在 2.PLANS/ 立项并在本表登记，再动代码。
