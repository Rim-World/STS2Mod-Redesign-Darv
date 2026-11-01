# DOCS 索引

本目录沉淀可复用的知识、调研结论与报告：T = 任务执行沉淀的知识与失败经验，R = 用户要求出具的报告（可行性、调研、审查），E = 关键证据陈列。工程决策见 1.ADR/，施工计划见 2.PLANS/，设计文档见 4.SDD/。

## 文档清单

| 编号 | 分类 | 文档 | 说明 |
|---|---|---|---|
| 001 | R | [BaseLib-StS2 能力调研](001-R-BaseLib-StS2能力调研.md) | 基础库能力地图、关键 API、对「修改达弗」可用性评估（无覆盖 vanilla 的 API，需自写 Harmony）、版本兼容策略 |
| 002 | R | [STS2-RitsuLib 能力调研](002-R-STS2-RitsuLib能力调研.md) | 基础库能力地图、`AncientOption<Darv>` 追加选项 API、patch 脚手架、版本兼容（工作坊 0.5.14 已带 0.111.0 变体） |
| 003 | T | [塔1遗物对照表](003-T-塔1遗物对照表.md) | DustyTome 溯源（塔2 原创）、达弗 12 候选的塔1/塔2 对照（11 件塔1 BOSS 遗物移植）、塔1独有遗物精筛 77 件清单、18 件移植候选推荐 |
| 004 | T | [塔1遗物候选细节](004-T-塔1遗物候选细节.md) | 18 件候选的逐件机制说明与塔2 近似锚点，供选型比较 |
| 005 | T | [达弗现状机制与双版本差异](005-T-达弗现状机制与双版本差异.md) | 达弗事件完整机制：继承链、选项生成流程与精确概率、107/111 的 9/11 集合差异、出现与解锁机制、本地化（两版 50 键一致）、`RelicOption`/`DustyTome` 细节 |
| 006 | R | [RelicGrabBag 去重机制调研](006-R-RelicGrabBag去重机制调研.md) | 塔2「遗物不重复」机制全景：双袋按稀有度分桶、抽取不放回、Obtain 自动移除；达弗（Ancient 稀有度）与袋无关的明确结论；mod 新遗物接入条件 |
| 007 | T | [RitsuLib 设置多语言与重启方案](007-T-RitsuLib设置多语言与重启方案.md) | `ModSettingsText.I18N` 自动语言切换（含游戏内即时切换）、语言查询 API、「保存并重启」方案对比与 Steam 风险 |
| 008 | T | [塔1六遗物塔2机制映射](008-T-塔1六遗物塔2机制映射.md) | 符文立方体/痛楚印记/悬浮风筝/袖剑/核能电池/机械臂 → 塔2 机制对位（全部直接可行，附现成模板与行号）；塔2 状态牌体系说明 |
| 009 | R | [塔1 BOSS 遗物移植评估报告](009-R-塔1Boss遗物移植评估报告.md) | 塔1 BOSS 遗物整体移植到塔2 的可行性、工作量与风险分级 |
| 010 | T | [手动测试方案](010-T-手动测试方案.md) | 完整手动测试流程：安装、槽位分布统计、六遗物效果、设置开关、递进逻辑、图鉴、语言、边界、107 双版本；含控制台 `ancient DARV` 用法与统计模板 |
| 011 | T | [pck 解包方法与达弗 CG 定位](011-T-pck解包方法与达弗CG定位.md) | Godot pack format 3 头部与目录结构、ctex→WebP→PNG 提取法；达弗全部资源路径（事件 CG、纪元立绘、图标）；产出 `pics/darv.png` |

## 第三方资料留档

`3.DOCS/online-docs/` 保存调研期抓取的官方/库文档原文，属于引用证据，不做改写：

- `BaseLib-Wiki-CustomAncientModel.html`、`BaseLib-Wiki-CustomRelic.html`、`BaseLib-Wiki-Features.html`：BaseLib Wiki 页面
- `ModTemplate-Wiki-Adding-Ancients.md`、`ModTemplate-Wiki-Modding-Basics.md`、`ModTemplate-Wiki-Setup.md`：Alchyr 官方 mod 模板 Wiki
- `RitsuLib-README.md`、`RitsuLibModTemplate-README.md`：RitsuLib 与模板仓库 README
- `SpireCodex-Darv-Event.html`：SpireCodex 上的达弗事件条目

## 参考库（本机路径）

- `C:\Users\<用户名>\Desktop\Mod\STS2-Reference\0.STS1-Reference\`：杀戮尖塔 1 参考（Java/libGDX，CFR 反编译 + 本地化）。
- `C:\Users\<用户名>\Desktop\Mod\STS2-Reference\1.STS2-107Reference\`：v0.107（default）反编译源码 + 14 语言本地化。
- `C:\Users\<用户名>\Desktop\Mod\STS2-Reference\2.STS2-111Reference\`：v0.111（beta）反编译源码 + 16 语言本地化。
- `C:\Users\<用户名>\Desktop\Mod\STS2-Reference\BaseLib-StS2\`、`STS2-RitsuLib\`：两个基础库的本地仓库副本；`ritsulib-decompiled*`：RitsuLib 解包产物。
- `C:\Users\<用户名>\Desktop\Mod\STS2-Reference\ModTemplate-StS2\`：Alchyr 官方 mod 模板（BaseLib 依赖）。
- 线上：BaseLib Wiki（alchyr.github.io/BaseLib-Wiki）、RitsuLib 文档（sts2-ritsulib.ritsukage.com）、ModTemplate Wiki。
- 游戏：`C:\Game\Steam\steamapps\common\Slay the Spire 2`（当前 v0.111.0；工作坊含 BaseLib v3.4.5、RitsuLib v0.5.14）。

## 目录结构

- `1.ADR/` — 架构决策记录（索引 `ADR.md`）
- `2.PLANS/` — 施工计划与施工记录回填（索引 `PLANS.md`）
- `3.DOCS/` — 本索引、知识与报告（`DOCS.md`）
- `4.SDD/` — 软件设计文档（索引 `SDD.md`）
- `5.ChangeLog/` — 版本日志分片（索引 `ChangeLog.md`）
- `9.SRC/DarvRelicPoolMod/` — 源码工程
- `10.OUTPUT/` — 构建产物与发布包（索引 `output.md`）
