using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using DarvRelicPoolMod.Relics;

namespace DarvRelicPoolMod;

/// <summary>
/// 达弗新选项生成器（设计 v2.2，2026-08-25 用户改版）：
/// 选项1 = 星盘/潘多拉魔盒/黑星/召唤铃铛/空鸟笼 五选一（各 20%，金字塔占用时不出）
/// 选项2 = 添水/灵体外质/贤者之石/天鹅绒颈圈/异蛇之眼 五选一（各 20%，金字塔占用时不出；
///         蛇眼开关关闭时剔除异蛇之眼）
/// 符文金字塔：选项1 先独立 20% 判定；未命中则选项2 再 20% 判定（不会同时出现；总概率 36%）
/// 选项3 = 尘封魔典 50% / 角色专属池 50%（池内均等）
///   （铁甲/静默/故障 2 件各 25%；储君/亡灵契约师 = 三件先古遗物，排除已持有稀有版的对应件；
///     空池（三件稀有全持有）→ 100% 魔典；魔典守卫（无先古卡）→ 100% 角色池）
/// </summary>
public static class DarvOptionPool
{
    // 槽位1：五选一
    private static readonly RelicModel[] Slot1Pool =
    {
        ModelDb.Relic<Astrolabe>(),
        ModelDb.Relic<PandorasBox>(),
        ModelDb.Relic<BlackStar>(),
        ModelDb.Relic<CallingBell>(),
        ModelDb.Relic<EmptyCage>(),
    };

    // 槽位2：五选一
    private static readonly RelicModel[] Slot2Pool =
    {
        ModelDb.Relic<Sozu>(),
        ModelDb.Relic<Ectoplasm>(),
        ModelDb.Relic<PhilosophersStone>(),
        ModelDb.Relic<VelvetChoker>(),
        ModelDb.Relic<SneckoEye>(),
    };

    public static IReadOnlyList<EventOption> Generate(Darv darv, Player owner)
    {
        var settings = DarvSettings.Current;
        var entry = owner.Character.Id.Entry.ToUpperInvariant();

        // 未知角色（mod 角色/未来新角色）无专属池与开关 → 回退原版逻辑（用户决策）；
        // 已知角色按职业开关：关闭 → 回退该版本原版逻辑
        if (!IsKnownCharacter(entry) || !settings.IsEnabledForCharacter(entry))
        {
            return VanillaFallbackPool.Generate(darv, owner);
        }

        var rng = darv.Rng;

        // 符文金字塔（用户决策）：选项1 先独立 20% 判定；未命中则选项2 再 20% 判定（不会同时出现）
        bool pyramidInSlot1 = rng.NextDouble() < 0.2;
        bool pyramidInSlot2 = !pyramidInSlot1 && rng.NextDouble() < 0.2;

        var options = new List<EventOption>(3)
        {
            MakeRelicOption(darv, pyramidInSlot1 ? ModelDb.Relic<RunicPyramid>() : rng.NextItem(Slot1Pool)),
            MakeRelicOption(darv, pyramidInSlot2 ? ModelDb.Relic<RunicPyramid>() : rng.NextItem(BuildSlot2Pool(settings, entry))),
            MakeSlot3Option(darv, owner, settings, rng),
        };
        return options;
    }

    /// <summary>
    /// 槽位2 池：按角色"异蛇之眼出现开关"剔除 SneckoEye（默认全开=五选一各 20%）；
    /// 关闭后剩余四件等分（各 25%）。
    /// </summary>
    private static List<RelicModel> BuildSlot2Pool(DarvSettings settings, string characterEntry)
    {
        if (settings.IsSneckoEnabledForCharacter(characterEntry))
        {
            return [.. Slot2Pool];
        }
        var sneckoId = ModelDb.Relic<SneckoEye>().Id;
        return Slot2Pool.Where(r => r.Id != sneckoId).ToList();
    }

    private static bool IsKnownCharacter(string entry)
    {
        return entry is "IRONCLAD" or "SILENT" or "DEFECT" or "REGENT" or "NECROBINDER";
    }

    /// <summary>
    /// 魔典守卫：检测当前角色卡池是否含先古稀有度卡（与 DustyTome.SetupForPlayer 相同筛选）。
    /// 无卡时魔典不可用（否则选择后崩溃，官方原版同样存在此边界），调用方应将魔典概率并入符文金字塔。
    /// </summary>
    public static bool HasAncientCards(Player owner)
    {
        foreach (var card in owner.Character.CardPool.GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint))
        {
            if (card.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(card))
            {
                return true;
            }
        }
        return false;
    }

    private static EventOption MakeSlot3Option(Darv darv, Player owner, DarvSettings settings, Rng rng)
    {
        bool tomeAvailable = HasAncientCards(owner);
        var characterPool = BuildCharacterPool(owner, settings);

        // 空池（储君/亡灵契约师三件稀有遗物全持有）：100% 魔典（用户决策）。
        // 魔典不可用（理论不发生：mod 仅对官方 5 职业生效且均有先古卡）→ 极端兜底符文金字塔 + Warn。
        if (characterPool.Count == 0)
        {
            if (!tomeAvailable)
            {
                MegaCrit.Sts2.Core.Logging.Log.Warn($"{ModEntry.ModId}: empty character pool and no Ancient cards; slot 3 -> Runic Pyramid (extreme fallback).");
                return MakeRelicOption(darv, ModelDb.Relic<RunicPyramid>());
            }
            return MakeDustyTomeOption(darv, owner);
        }

        // 正常掷数：50% 尘封魔典 / 50% 角色专属池（池内均等）
        double roll = rng.NextDouble() * 100;
        if (roll < 50)
        {
            // 魔典守卫：无先古卡时魔典概率并入角色池（用户决策兜底）
            if (!tomeAvailable)
            {
                MegaCrit.Sts2.Core.Logging.Log.Warn($"{ModEntry.ModId}: character has no Ancient cards; Dusty Tome disabled (slot 3 -> character pool).");
                return MakeRelicOption(darv, rng.NextItem(characterPool));
            }
            return MakeDustyTomeOption(darv, owner);
        }
        return MakeRelicOption(darv, rng.NextItem(characterPool));
    }

    /// <summary>
    /// 按当前角色构建选项3的专属遗物池（50% 分支内均等随机）。
    /// 储君/亡灵契约师：三件先古遗物，排除"已持有稀有版"的对应件；三件稀有全持有 → 空池（100% 魔典）。
    /// </summary>
    private static List<RelicModel> BuildCharacterPool(Player owner, DarvSettings settings)
    {
        var entry = owner.Character.Id.Entry.ToUpperInvariant();
        switch (entry)
        {
            case "IRONCLAD":
                return settings.SwapMarkOfPainNuclearBattery
                    ? NewPool(ModelDb.Relic<RunicCube>(), ModelDb.Relic<NuclearBattery>())
                    : NewPool(ModelDb.Relic<RunicCube>(), ModelDb.Relic<MarkOfPain>());
            case "SILENT":
                return NewPool(ModelDb.Relic<HoveringKite>(), ModelDb.Relic<WristBlade>());
            case "DEFECT":
                return settings.SwapMarkOfPainNuclearBattery
                    ? NewPool(ModelDb.Relic<MarkOfPain>(), ModelDb.Relic<Inserter>())
                    : NewPool(ModelDb.Relic<NuclearBattery>(), ModelDb.Relic<Inserter>());
            case "REGENT":
            {
                var pool = new List<RelicModel>(3);
                if (!HasRelic(owner, ModelDb.Relic<LunarPastry>())) pool.Add(ModelDb.Relic<AncientLunarPastry>());
                if (!HasRelic(owner, ModelDb.Relic<MiniRegent>())) pool.Add(ModelDb.Relic<AncientMiniRegent>());
                if (!HasRelic(owner, ModelDb.Relic<OrangeDough>())) pool.Add(ModelDb.Relic<AncientOrangeDough>());
                return pool;
            }
            case "NECROBINDER":
            {
                var pool = new List<RelicModel>(3);
                if (!HasRelic(owner, ModelDb.Relic<Bookmark>())) pool.Add(ModelDb.Relic<AncientBookmark>());
                if (!HasRelic(owner, ModelDb.Relic<IvoryTile>())) pool.Add(ModelDb.Relic<AncientIvoryTile>());
                if (!HasRelic(owner, ModelDb.Relic<BigHat>())) pool.Add(ModelDb.Relic<AncientBigHat>());
                return pool;
            }
            default:
                return [];
        }
    }

    private static List<RelicModel> NewPool(params RelicModel[] relics) => [.. relics];

    private static bool HasRelic(Player owner, RelicModel relic)
    {
        foreach (var owned in owner.Relics)
        {
            if (owned.Id == relic.Id)
            {
                return true;
            }
        }
        return false;
    }

    private static EventOption MakeRelicOption(Darv darv, RelicModel? relic)
    {
        return darv.RelicOption(relic!.ToMutable());
    }

    private static EventOption MakeDustyTomeOption(Darv darv, Player owner)
    {
        var tome = (DustyTome)ModelDb.Relic<DustyTome>().ToMutable();
        if (owner != null)
        {
            tome.SetupForPlayer(owner);
        }
        return darv.RelicOption(tome);
    }
}
