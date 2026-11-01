# 006-R RelicGrabBag 去重机制调研

> 状态：定稿 v1.0（2026-08-23）。来源：`2.STS2-111Reference\decompiled`（基准）+ `1.STS2-107Reference\decompiled`（差异对比）。两版机制**完全一致**。

## 1. 机制本质

- `RelicGrabBag`（`Runs\RelicGrabBag.cs`）= **按稀有度分桶（Common/Uncommon/Rare/Shop）的双端队列**，局内"抽取不放回"的遗物供应池。注释："The pulled relic will never be seen again on this run"。
- 每玩家一个 `Player.RelicGrabBag`（精英/商店/事件抽取，不可刷新）；每局一个共享 `RunState.SharedRelicGrabBag`（宝箱抽取，桶抽空可刷新）。
- 开局填充：SharedRelicPool（118 件）+ 角色池（各自按解锁过滤）→ 过滤稀有度四档 → 洗牌装袋。
- 抽取侧稀有度权重（`RelicFactory.RollRarity`）：Common 50% / Uncommon 33% / Rare 17%；Shop 固定一档。
- **任何来源 `RelicCmd.Obtain` 非堆叠遗物 → 自动从双袋移除**（`RelicCmd.cs:41-45`）→ 同局内不再出现于宝箱/商店/精英。

## 2. 与各遗物来源的关系

| 来源 | 机制 |
|---|---|
| 宝箱 | 共享袋 PullFromFront |
| 精英掉落 / 事件随机遗物 | 玩家袋 PullFromFront（RelicFactory） |
| 商店 | 玩家袋 PullFromBack（3 货架） |
| 事件指定遗物 / **Ancient（达弗）** | 直接 RelicCmd.Obtain，不走袋 |

## 3. 达弗与去重（明确结论）

- 达弗 12 件遗物全部 `RelicRarity.Ancient` → **被 `_rarities` 过滤，根本不在任何袋中**。
- 达弗 = "白名单内随机指定遗物"（`_validRelicSets` + `GenerateInitialOptions`），**与 RelicGrabBag 完全无关**；Obtain 时的袋移除是空操作。
- 因此：官方达弗遗物永远不会出现在宝箱/商店/精英随机池（稀有度天然排除）；同一局内玩家可能重复获得同一达弗遗物（游戏现状，达弗不查袋）。
- Neow 同理。

## 4. mod 新遗物的接入（事实层面）

**纳入去重的充分条件**（两者都要）：
1. 稀有度 ∈ {Common, Uncommon, Rare, Shop}；
2. 注册进 SharedRelicPool 或角色池（`ModHelper.AddModelToPool<SharedRelicPool, T>()`，初始化前）。

满足后**自动获得去重**（mod 无需写去重代码）：进双袋 → 精英/商店/宝箱可随机出；达弗（或任何事件）发放后 `Obtain` 自动从双袋移除 → 同局不重复。

若设为 **Ancient 稀有度**（与官方达弗一致）：不进袋 → 只在达弗出现、随机池永不出；无重复问题（也谈不上利用袋）。

**其他事实**：
- `RelicChoices` = run history，非去重；`MarkRelicAsSeen` = 图鉴收集进度，非局内去重。
- 边界（官方同样存在）：共享袋某稀有度桶抽空后刷新会把已获得遗物放回（宝箱极端情况下可能重复）；袋空回退 Circlet。
- 存档：袋存于 `SerializablePlayer.RelicGrabBag` / `SerializableRun.SerializableSharedRelicGrabBag`（JSON 按 ModelId 列表）；mod 遗物 id 需稳定；mod 卸载后读旧档，袋内 mod 遗物条目被静默跳过（不崩溃）；107→111 存档互通。
- 多人：袋随 NetFullCombatState / SyncRngMessage 同步，mod 接入后多人自动生效。

## 5. 对达弗 mod 的启示

- 若 6 件塔1 遗物采用**普通稀有度 + 注册进池**：达弗发放后自动进入去重闭环（同局不再从宝箱/商店/精英出现）——"尽可能利用现成机制"的正解。
- 若采用 **Ancient 稀有度**：与官方达弗一致、只从达弗获得，无去重需求——但玩家同局可从达弗重复获得同一件（官方现状）。
- 达弗"内部去重"（同局达弗不给已拥有的遗物）是另一回事：需 mod 在选项生成时自行查 `player.Relics`/袋（官方不做）。
