# 重做达弗（STS2Mod-Redesign-Darv）

《杀戮尖塔2》mod：重构先古之民「达弗（Darv）」的三个选项，把原版的随机集合换成固定三槽位结构，并引入《杀戮尖塔1》的 BOSS 遗物与储君/亡灵契约师的先古遗物。

Redesigns the three options of Ancient Darv in *Slay the Spire 2*: a fixed three-slot layout replaces the vanilla random relic sets, with Boss relics ported from *Slay the Spire 1* and new Ancient relics for the Regent and Necrobinder.

| 项 | 值 |
|---|---|
| Mod id | `DarvRelicPoolMod` |
| 显示名 | 重做达弗（Redesign Darv） |
| 版本 | 1.0.0 |
| 作者 | 再玩小卡组找人弄你 |
| 依赖 | `STS2-RitsuLib ≥ 0.5.14` |
| 最低游戏版本 | 0.107.0（支持 default 0.107.x 与 public beta 0.111.0+） |

## 功能

### 三槽位结构

- **选项 1**：星盘 / 潘多拉魔盒 / 黑星 / 召唤铃铛 / 空鸟笼 五选一；符文金字塔先做一次 **20%** 判定占位。
- **选项 2**：添水 / 灵体外质 / 贤者之石 / 天鹅绒颈圈 / 异蛇之眼 五选一；仅在选项 1 未命中时再对符文金字塔做一次 **20%** 判定。两槽位互斥，金字塔合计概率约 **36%**。
- **选项 3**：尘封魔典 **50%** / 职业专属遗物池 **50%**（池内均等）。

### 职业专属池

| 职业 | 池内容 |
|---|---|
| 铁甲战士 | 符文立方体、痛楚印记 |
| 静默猎手 | 悬浮风筝、袖剑 |
| 故障机器人 | 核能电池、机械臂 |
| 储君 | 先古月亮糕点、先古迷你储君、先古橙色团块 |
| 亡灵契约师 | 先古书签、先古大帽子、先古象牙麻将牌 |

前三个职业的 6 件来自《杀戮尖塔1》，效果照搬；储君/亡灵的 6 件是其稀有遗物的加强版，与稀有版**双向互斥**：已持有稀有版则达弗不再给出对应先古版，获得先古版则本局该稀有版不再从任何渠道出现。

### 设置页（RitsuLib）

1. 5 个职业开关，关闭后该职业三个选项完全回退到**该版本原版**逻辑；
2. 铁甲战士「痛楚印记」与 故障机器人「核能电池」互换（塔2 把塔1 的状态牌体系给了故障机器人）；
3. 5 个「槽位 2 异蛇之眼出现开关」按职业分开（蛇眼的加费逻辑对储君适配很差）；
4. 联机说明；
5. **保存并退出**：只有点击该按钮才写盘，未保存的改动不会生效，退出设置界面即丢弃。

## 兼容性

- 支持 default 0.107.x 与 public beta 0.111.0 及以上；两版本存在虚方法签名差异的接口通过可选补丁 + 运行时反射自适应，效果一致。
- 仅对原版五职业生效；mod 角色与未来新角色自动回退原版达弗逻辑。
- 联机兼容性未经仔细测试（各玩家按自己的设置生成选项）。
- 不保证与盗版、修改版、移动版、老旧版本、冷门 mod、次级 mod 的兼容性。
- 语言：简中（zhs）/ 繁中（zht）/ 英文（eng），其他语言回退英文。

## 安装

1. 通过创意工坊订阅 `STS2-RitsuLib`（最低 0.5.14）；
2. 把发布包内的 `DarvRelicPoolMod.dll`、`DarvRelicPoolMod.pck`、`DarvRelicPoolMod.json` 放入
   `Slay the Spire 2/mods/DarvRelicPoolMod/`；
3. 启动游戏，在 mod 列表中确认已加载。

## 构建

前置：.NET SDK（net9.0）、Godot 4.5.1（带 .NET 支持）、本机已安装《杀戮尖塔2》。

```powershell
cd 9.SRC/DarvRelicPoolMod
./build.ps1
```

`build.ps1` 依次执行 `dotnet build` → Godot `--headless --import` → `--export-pack`，最后把 dll/pdb/pck/json 复制到游戏 `mods/DarvRelicPoolMod/`。

两条必须遵守的约束：

- pck 内所有资源必须带 `res://DarvRelicPoolMod/` 前缀，否则会覆盖游戏同名资源且不会被本地化合并；
- `export_presets.cfg` 的 `export_filter` 必须是 `selected_resources`。

## 目录结构

```
STS2Mod-达弗修改/
├── AGENTS.md            # 工作区通用规范（拷贝自上级目录）
├── README.md            # 本文件
├── CHANGELOG.md         # Keep a Changelog 单文件更新日志
├── LICENSE              # MIT
├── 1.ADR/               # 架构决策记录（索引 ADR.md）
├── 2.PLANS/             # 施工计划与回填（索引 PLANS.md）
├── 3.DOCS/              # 知识与调研报告（索引 DOCS.md）
├── 4.SDD/               # 软件设计文档（索引 SDD.md）
├── 5.ChangeLog/         # 版本日志分片（索引 ChangeLog.md）
├── 9.SRC/               # 源码工程 DarvRelicPoolMod/
├── 10.OUTPUT/           # 构建产物与发布包（索引 output.md，其余不入库）
├── PRIVACY/             # 隐私与机密（索引 PRIVACY.md，不入库）
├── TMP/                 # 临时与过程文件（索引 TMP.md，其余不入库）
├── TOOLS/               # 自写脚本（索引 TOOLS.md）
├── description/         # 创意工坊/发布文案（中英文）
└── pics/                # 封面与素材图
```

## 文档索引

- 决策：[1.ADR/ADR.md](1.ADR/ADR.md)
- 计划：[2.PLANS/PLANS.md](2.PLANS/PLANS.md)
- 知识与报告：[3.DOCS/DOCS.md](3.DOCS/DOCS.md)
- 设计：[4.SDD/001-总体设计.md](4.SDD/001-总体设计.md)
- 版本日志：[5.ChangeLog/ChangeLog.md](5.ChangeLog/ChangeLog.md)
- 发布文案：[description/重做达弗_中文.md](description/重做达弗_中文.md)、[description/重做达弗_英文.md](description/重做达弗_英文.md)

## 版本

当前 `1.0.0`（2026-08-29）。详见 [CHANGELOG.md](CHANGELOG.md)。

## 致谢

- 使用 Deepseek Harness（WebUI）+ DeepSeek V4 Flash（0731 GA）开发；感谢 Saint Liang。
- 感谢《杀戮尖塔2》团队让 mod 制作对玩家更友好。
- 感谢 BaseLib 与 RitsuLib 的开发者提供的开源实现与文档。

## 开源地址

https://github.com/Rim-World/STS2Mod-Redesign-Darv

许可：[MIT](LICENSE)。欢迎在仓库 issue 与创意工坊评论区反馈。
