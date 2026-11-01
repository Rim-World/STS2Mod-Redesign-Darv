# 009-R 塔1 BOSS 遗物移植评估报告

- 日期：2026-08-25
- 性质：纯调研报告（未启动游戏、未修改任何已存在文件；证据来自 `STS2-Reference` 塔1/塔2 反编译源码）
- 触发：用户提供 Steam 工作坊 mod（id=3785377312）作者在简介中披露的移植问题

---

## 一、结论摘要（人话版）

1. **塔2 没有"Boss 遗物"这个身份**：塔2 没有 Boss 稀有度，也没有 Boss 战三选一遗物的机制。塔1 Boss 遗物进塔2 的现实归属只有"Ancient（先古）稀有度 + 达弗事件发放"（本 mod 已验证的路径）；Ancient 不进宝箱房/商店（RelicGrabBag 只装 Common/Uncommon/Rare/Shop 四档）。
2. **作者披露的 6 个问题，5 个是塔2 机制差异导致、1 个是他自己的实现细节**：
   - 诅咒钥匙（进宝箱房给愧疚）← 塔2 没有"开箱"概念与钩子，无法复刻"不开箱就不付代价"的塔1 语义；
   - 符文圆顶（联机全玩家看不到意图）← 他 patch 了**数据层**（意图对象被联机广播）；正确做法是 patch **显示层**（只影响本地玩家）；
   - 神圣树皮（改不全药水）← 塔2 药水**没有统一的数值管道**（塔1 有 potency 字段），只能逐个药水处理；
   - 融合之锤+咖啡滤纸（禁止同时获得）← 塔2 火堆默认只有 2 个选项（休息/锻造），两个都禁后无可用选项，实测跳过路径会卡死；
   - 破碎金冠（把界面强制改成 1 张）← 这是他实现时把选项数写死成 1，塔2 机制本身支持数量修改；
   - 痛楚印记（伤口沉底）← 他没传随机塞入参数，塔2 的 `CardPileCmd.AddToCombatAndPreview` 支持 `CardPilePosition.Random`（本 mod 已用）。
3. **塔2 机制差异最深的三个坑**：宝箱房语义、意图显示层级、药水数值管道——这三个决定了诅咒钥匙、符文圆顶、神圣树皮无法"照搬"。

---

## 二、塔1 Boss 遗物全景 vs 塔2 现状对照

塔1 Boss 遗物 = 25 件通用 + 4 件职业替换（数据源：塔1 反编译 `com/megacrit/cardcrawl/relics/*.java` + 塔2 反编译 `MegaCrit.Sts2.Core.Models.Relics/*.cs`）：

| 塔1 Boss 遗物 | 塔2 现状 | 移植难度/备注 |
|---|---|---|
| 星盘 Astrolabe | ✅ 已有（Ancient） | — |
| 黑星 Black Star | ✅ 已有（Ancient） | — |
| 召唤铃铛 Calling Bell | ✅ 已有（Ancient） | — |
| 空鸟笼 Empty Cage | ✅ 已有（Ancient） | — |
| 潘多拉魔盒 Pandora's Box | ✅ 已有（Ancient） | — |
| 添水 Sozu | ✅ 已有（Ancient） | — |
| 灵体外质 Ectoplasm | ✅ 已有（Ancient） | — |
| 贤者之石 Philosopher's Stone | ✅ 已有（Ancient） | — |
| 天鹅绒颈圈 Velvet Choker | ✅ 已有（Ancient） | — |
| 异蛇之眼 Snecko Eye | ✅ 已有（Ancient） | — |
| 符文金字塔 Runic Pyramid | ✅ 已有（Ancient） | — |
| 尘封魔典 Dusty Tome | ✅ 已有（塔2 原创 Ancient） | — |
| 诅咒钥匙 Cursed Key | ❌ 没有 | 🔴 高（宝箱房语义差异） |
| 符文圆顶 Runic Dome | ❌ 没有 | 🟠 中（意图显示层级，联机陷阱） |
| 神圣树皮 Sacred Bark | ❌ 没有 | 🔴 高（药水无统一数值管道） |
| 融合之锤 Fusion Hammer | ❌ 没有 | 🟠 中（火堆选项；与咖啡滤纸互斥） |
| 咖啡滤纸 Coffee Dripper | ❌ 没有 | 🟠 中（火堆选项；与融合之锤互斥） |
| 破碎金冠 Busted Crown | ❌ 没有 | 🟠 中（卡牌奖励选项数） |
| 奴隶项圈 Slavers' Collar | ❌ 没有 | 🟢 低-中（RoomType.Elite/Boss 存在） |
| 小房子 Tiny House | ❌ 没有 | 🟢 低-中（5 个组件各找 API，拾取时触发） |
| 符文立方体 Runic Cube | ❌（本 mod 已移植） | 已解决（AfterCurrentHpChanged 两版签名一致） |
| 痛楚印记 Mark of Pain | ❌（本 mod 已移植） | 已解决（伤口用 CardPilePosition.Random） |
| 袖剑 Wrist Blade | ❌（本 mod 已移植） | 已解决（ModifyDamageAdditive 5/6 参双 patch） |
| 悬浮风筝 Hovering Kite | ❌（本 mod 已移植） | 已解决 |
| 机械臂 Inserter | ❌（本 mod 已移植） | 已解决 |
| 核能电池 Nuclear Battery | ❌（本 mod 已移植） | 已解决 |
| 黑血 Black Blood（铁甲替换） | ⚠️ 塔2 有但为 **Starter**（初始遗物） | 塔1 的"Boss 升级版"语义在塔2 不存在 |
| 蛇之戒 Ring of the Serpent（静默替换） | ⚠️ 塔2 有 RingOfTheSnake 为 **Starter** | 同上（塔1 Boss 版"回合结束多抽"未移植） |
| 冰冻核心 Frozen Core（机器人替换） | ⚠️ 塔2 有 CrackedCore 为 **Starter** | 同上（塔1 Boss 版"回合结束充冰球"未移植） |
| 圣水 Holy Water / 紫莲花 Violet Lotus（观者） | ❌ 塔2 无观者角色 | 不适用 |

> 塔2 的 BlackBlood/RingOfTheSnake/CrackedCore 反编译确认 `RelicRarity.Starter`——塔2 把塔1 的"职业 Boss 替换"降格成了角色初始遗物，Boss 升级版机制（回合结束多抽/回合结束充球）塔2 不存在，需要新实现（有初始版可参照，难度中）。

---

## 三、六大机制差异详解（附代码证据）

### 1. 宝箱房 ≠ 宝箱（影响：诅咒钥匙）

- **塔1**：`CursedKey.onChestOpen(boolean bossChest)` —— 有明确的"开箱"事件钩子；玩家可以**不进宝箱房/不开箱**来规避代价（开箱才有诅咒+额外遗物）。
- **塔2**：没有宝箱对象、没有开箱钩子。宝箱房（`TreasureRoom.EnterInternal` → `TreasureRoomRelicSynchronizer.BeginRelicPicking()`）的流程是：**进入房间即开始遗物挑选**（单机跳过=放弃遗物；联机=投票/猜拳分配，`PlayerVote.index == null` 表示跳过）。即"不开箱"这个状态在塔2 **不存在**——进房=开箱。
- **后果**：塔1 诅咒钥匙的"可选代价"语义无法复刻，只能做成"进宝箱房就付代价"（作者方案：进房立即给一张 Guilty 愧疚）。另外塔1 的"开箱额外多一件遗物"奖励在塔2 也没有自然挂点（宝箱房固定一个 RelicGrabBag 遗物选择）。
- **实现挂钩点**：模型级房间进入钩子（塔2 `Hook.AfterRoomEntered`；RestSiteRoom/TreasureRoom 的 EnterInternal 都调它），或 patch `TreasureRoomRelicSynchronizer.BeginRelicPicking`。

### 2. 意图显示是"本地 UI"不是"怪物数据"（影响：符文圆顶）

- **塔1**：`RunicDome.java` 只做 `onEquip +1 能量`，意图隐藏完全发生在**显示层**（遗物自身不碰意图数据）。
- **塔2**：意图数据 = 怪物状态的 `Monster.NextMove.Intents`（联机各端确定性推演同一份数据）；显示 = 本地节点 `NCreature.UpdateIntent`（NCreature.cs:617）→ `NIntent.UpdateIntent`（NIntent.cs:224）→ `UpdateVisuals()`。塔2 还有原生的调试隐藏（`NCombatUi.IsDebugHidingIntent`）和 `HiddenIntent/UnknownIntent`（怪物自用）。
- **正确移植**：patch 显示层（`NIntent.UpdateIntent`/`UpdateVisuals` 或 `NCreature.UpdateIntent`），持有者本地隐藏意图节点 → **只影响自己，联机无副作用**。
- **作者踩坑**：联机中"只要被拾取，所有玩家都无法看到意图"= 他 patch 的是**数据层**（意图对象/`NextMove`），而意图数据是同步广播的 → 全玩家生效。这是联机游戏里"显示 vs 数据"层级选错的典型事故。

### 3. 药水效果没有统一数值管道（影响：神圣树皮）

- **塔1**：`AbstractPotion` 有 potency（数值）体系，药水在 `initializeData()` 里算数值；`SacredBark.onEquip` 只是对所有药水重新 `initializeData()`，各药水的 initializeData 里检查 `hasRelic("SacredBark")` 翻倍。
- **塔2**：`PotionModel`（Models/PotionModel.cs）**没有 potency 字段、没有统一数值入口**。每个药水子类 override `protected virtual Task OnUse(PlayerChoiceContext, Creature? target)` 自己实现效果（数值来自各自的 DynamicVars / 力量 / 回合数等）。唯一的公共入口 `OnUseWrapper` 只是流程包装（抛瓶动画、历史记录），数值已经"内化"在子类里。
- **后果**：神圣树皮没有单点可加倍 → 要么逐个 patch 药水（塔2 药水 40+ 种），要么 patch DynamicVars 数值（但不覆盖所有药水——比如攻击药水伤害还吃力量）。作者"没有做到修改所有药水的效果"是塔2 机制使然，不是他偷懒。
- **可参考**：塔2 `ChemicalX` 的 `ModifyXValue`（改 X 值卡牌）是同类"遗物改数值"先例，但只覆盖 X 值体系，不覆盖药水。

### 4. 火堆默认只有两个选项，没有"跳过"选项类（影响：融合之锤、咖啡滤纸）

- **塔2 火堆**：`RestSiteOption.Generate(player)` 固定生成 `HealRestSiteOption + SmithRestSiteOption`（联机再加 `MendRestSiteOption`），然后走 `Hook.ModifyRestSiteOptions`（可增删/禁用）。选项有 `virtual bool IsEnabled`（禁用后仍可悬停查看，塔2 官方禁用语义）。
- **"跳过"的真相**：塔2 没有 Skip 选项类，但火堆房间 `NRestSiteRoom` 实现 `IRoomWithProceedButton`（有"继续"按钮），且 `RestSiteSynchronizer` 有 `RestSiteSkippedMessage`（退出时 `BeforeLocalRestSiteExited` 自动清空剩余选项并完成）。**机制层面存在离开路径**。
- **作者实测**："二代火堆处不选择选项直接跳过会卡死"，因此禁止融合之锤与咖啡滤纸同时获得（两个都禁 = 火堆无可用选项）。
- **结论**：单件禁用（`IsEnabled=false` 或从选项列表移除）可行；**两件同持的风险需要实测**（塔2 的 Proceed/跳过路径在"从未选过任何选项"时是否可用，反编译无法完全确定 UI 行为）——作者选择"禁止同时获得"是保守稳妥做法，建议沿用。
- 融合之锤的另一半效果"Boss 战后额外卡牌奖励"：塔2 可行，参照 `WhiteStar`/`DreamCatcher`/`PrayerWheel`（它们就是通过 `DoExtraRewardsIfNeeded`/`RewardsCmd` 追加 `CardReward`）。

### 5. 卡牌奖励选项数硬编码为 3（影响：破碎金冠）

- **塔2**：战斗/事件卡牌奖励的选项数几乎全部硬编码 `3`（`RewardsSet.cs:230-241`、事件、`WhiteStar` 等），构造时传入 `new CardReward(options, 3, player)`。有官方钩子 `AbstractModel.AfterModifyingCardRewardOptions`（`SilkenTress`/`SilverCrucible` 使用），但该钩子改的是**选项内容**（如给卡上附魔），数量在 `CardFactory.CreateForReward(player, count, options)` 已定。
- **塔2 没有问号牌（Question Card）**：塔2 加卡牌的方式是"追加一份额外卡牌奖励"（PrayerWheel/Orrery），**不存在"选项数 +1"机制**。所以塔1 的"金冠+问号牌抵消"组合在塔2 原版不存在。
- **作者踩坑**："破碎金冠和问号牌同时持有仍只能看一张"= 他把金冠实现成"把界面选项数强行改为 1"（写死了），而不是"3-1=2"；叠加问号牌也没救回来。这是实现细节错误，不是塔2 机制限制。
- **正确移植**：patch 卡牌奖励生成处（`RewardsSet` 或 `CardFactory.CreateForReward`），持有金冠时 count-1（至少 1）；若要复刻问号牌联动，需连问号牌一起移植并用同一管道 +1。

### 6. 状态/诅咒卡系统差异（影响：痛楚印记、诅咒钥匙）

- **塔2 有完整的诅咒卡池**：`Guilty`（愧疚）/`Injury`/`Doubt`/`Regret`/`Shame`/`Burn`/`Slimed`/`Wound`/`CurseOfTheBell` 等（Models/Cards）。作者用 Guilty 作诅咒钥匙的代价，选择合理。
- **伤口塞入位置**：塔2 `CardPileCmd.AddToCombatAndPreview(owner, PileType.Draw, 数量, owner, CardPilePosition.Random)` 支持位置参数——作者没传随机位置（或用了默认底部），导致伤口沉底；**本 mod 的痛楚印记已用 `CardPilePosition.Random` 解决**。
- **107/111 差异**：`Wound` 等卡的 API 两版一致（本 mod 已验证）；但"效果类"遗物 hook 必须逐一核对两版签名（先例：`ModifyDamageAdditive` 107=5 参/111=6 参，本 mod 用"双 optional patch 运行时自适应"解决）。

---

## 四、移植通用建议（跨遗物）

1. **稀有度与获取**：塔2 无 Boss 稀有度 → 塔1 Boss 遗物按 `RelicRarity.Ancient` 注册、只在达弗事件出现（本 mod 已验证；RelicGrabBag 四档不含 Ancient，宝箱/商店不会自然出现）。若想放宽获取途径，需额外 patch RelicGrabBag 或宝箱房生成。
2. **联机层级铁律**：只 patch **显示层**（意图 UI、动画），不碰**同步数据层**（怪物状态、意图对象、奖励列表）——否则影响所有玩家（符文圆顶事故）。
3. **双版本编译**：本 mod 直接引用游戏目录 sts2.dll 编译（切分支=换编译目标）；凡 override 官方虚方法都要先核对 107/111 签名（通用 hook 如 `AfterCurrentHpChanged`/`BeforeCombatStart`/`ModifyMaxEnergy` 等两版一致；`ModifyDamageAdditive` 这类变参方法走 optional patch）。
4. **能量类**：塔2 用 `ModifyMaxEnergy`（贤者之石先例），注意 107/111 签名一致（已验证）。
5. **本地化与图鉴**：三语文本（relics.json 合并进官方表）+ 图鉴 Ancient 分类（标题排序/角色光效，本 mod 已实现联动框架，新增遗物自动纳入）。
6. **文本描述**：塔1 遗物描述里的"每回合""战斗开始时"等语义要与塔2 回合/战斗钩子对齐（`BeforeSideTurnStart`/`AfterSideTurnStart`/`BeforeCombatStart` 等，两版签名一致清单见上）。

---

## 五、证据索引（反编译源码位置，decompiled-111 或塔1）

| 主题 | 位置 |
|---|---|
| 塔1 遗物实现 | `0.STS1-Reference/decompiled/com/megacrit/cardcrawl/relics/*.java`（SacredBark/CursedKey/RunicDome 等） |
| 塔2 遗物全集 | `decompiled-111/MegaCrit.Sts2.Core.Models.Relics/*.cs`（300 件） |
| 塔2 宝箱房 | `Rooms/TreasureRoom.cs`、`Multiplayer.Game/TreasureRoomRelicSynchronizer.cs`（投票/跳过语义） |
| 塔2 意图管线 | `Nodes.Combat/NCreature.cs:617`、`Nodes.Combat/NIntent.cs:224` |
| 塔2 药水 | `Models/PotionModel.cs`（OnUse/OnUseWrapper，无 potency） |
| 塔2 火堆 | `Entities.RestSite/RestSiteOption.cs`（Generate/IsEnabled）、`Multiplayer.Game/RestSiteSynchronizer.cs`（SkippedMessage） |
| 塔2 卡牌奖励 | `Rewards/CardReward.cs`（OptionCount）、`Rewards/RewardsSet.cs:230-241`、`Factories/CardFactory.cs:89`（CreateForReward） |
| 塔2 卡牌奖励钩子 | `Hooks/Hook.cs:691`（AfterModifyingCardRewardOptions）、`Models.Relics/SilkenTress.cs` |
| 塔2 诅咒/状态卡 | `Models.Cards/Guilty.cs`、`Wound.cs` 等 |
| 塔2 房间类型 | `Rooms/RoomType.cs`（Elite/Boss 存在 → 奴隶项圈可行） |
| 塔2 初始遗物 | `Models.Relics/BlackBlood.cs`、`RingOfTheSnake.cs`、`CrackedCore.cs`（均 Starter） |
