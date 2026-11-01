# 素材清单（pics/素材）

> 2026-08-23 提取。用途：达弗修改 mod 的遗物图标素材（本体 + 轮廓线）。
> 命名规则：`<英文ID>.png`（本体）+ `<英文ID>_outline.png`（轮廓线）。塔2 图标来自游戏 pck 图集裁剪（原始尺寸 58~85px）；塔1 图标来自塔1 游戏 jar（128×128 原始尺寸）。

## 塔2-遗物/（18 个遗物 × 2 = 36 张）

来源：`C:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck`
（解包 → `images/atlases/relic_atlas.sprites/*.tres` region → BPTC 纹理解码裁剪，Godot 4.5.1 脚本处理）

| 文件 | 遗物（官方中文名） | 英文 ID | 稀有度 | 用途 |
|---|---|---|---|---|
| astrolabe(.png/_outline) | 星盘 | Astrolabe | 先古 | 三选项槽位 1 池 |
| pandoras_box | 潘多拉魔盒 | PandorasBox | 先古 | 槽位 1 池 |
| black_star | 黑星 | BlackStar | 先古 | 槽位 1 池 |
| calling_bell | 召唤铃铛 | CallingBell | 先古 | 槽位 1 池 |
| empty_cage | 空鸟笼 | EmptyCage | 先古 | 槽位 1 池 |
| sozu | 添水 | Sozu | 先古 | 槽位 2 池 |
| ectoplasm | 灵体外质 | Ectoplasm | 先古 | 槽位 2 池 |
| philosophers_stone | 贤者之石 | PhilosophersStone | 先古 | 槽位 2 池 |
| velvet_choker | 天鹅绒颈圈 | VelvetChoker | 先古 | 槽位 2 池 |
| snecko_eye | 异蛇之眼 | SneckoEye | 先古 | 槽位 2 池 |
| dusty_tome | 尘封魔典 | DustyTome | 先古 | 槽位 3（50%） |
| runic_pyramid | 符文金字塔 | RunicPyramid | 先古 | 槽位 3（30%） |
| lunar_pastry | 月亮糕点 | LunarPastry | 稀有 | 储君角色池 |
| mini_regent | 迷你储君 | MiniRegent | 稀有 | 储君角色池 |
| orange_dough | 橙色团块 | OrangeDough | 稀有 | 储君角色池（追加） |
| big_hat | 大帽子 | BigHat | 稀有 | 亡灵契约师角色池（追加） |
| bookmark | 书签 | Bookmark | 稀有 | 亡灵契约师角色池 |
| ivory_tile | 象牙麻将牌 | IvoryTile | 稀有 | 亡灵契约师角色池 |

## 塔1-遗物/（6 个遗物 × 2 = 12 张）

来源：`C:\Game\Steam\steamapps\common\SlayTheSpire\desktop-1.0.jar` 内 `images/relics/`（128×128）

| 文件 | 塔1 遗物 | 塔1 稀有度 | 塔2 中对应角色 | 用途（槽位 3 角色池 20%） |
|---|---|---|---|---|
| runicCube(.png/_outline) | 符文立方体 Runic Cube | BOSS | 铁甲战士 | 铁甲战士池 |
| mark_of_pain | 痛楚印记 Mark of Pain | BOSS | 铁甲战士 | 铁甲战士池 |
| kite | 悬浮风筝 Hovering Kite | BOSS | 静默猎手 | 静默猎手池 |
| wBlade | 袖剑 WristBlade | BOSS | 静默猎手 | 静默猎手池 |
| battery | 核能电池 Nuclear Battery | BOSS | 故障机器人 | 故障机器人池 |
| inserter | 机械臂 Inserter | BOSS | 故障机器人 | 故障机器人池 |

## 备注

- 塔2 图标取自游戏本体（非截图），含透明通道；mod 中按原路径 `res://...` 打包即可直接使用，或参考 BaseLib `RelicImageOverridePatch` 机制。
- 塔1 图标用于新注册的 6 件遗物；若希望与塔2 风格统一，可后续对塔1 图做描边/尺寸归一化处理（128×128 → 塔2 图集约 78×78 基准）。
- 提取/裁剪工具链（过程文件，已留存于 `%TEMP%\sts2-pck-extract`、`%TEMP%\godot-pck-export`、`%TEMP%\pcktool`，可复用于将来提取其他素材）。
