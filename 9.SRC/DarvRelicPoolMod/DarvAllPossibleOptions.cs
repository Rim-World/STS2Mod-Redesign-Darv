using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using DarvRelicPoolMod.Relics;

namespace DarvRelicPoolMod;

/// <summary>
/// 达弗"全部可能选项"清单（v2.2）：槽位1 五件 + 槽位2 五件 + 符文金字塔 + 尘封魔典
/// + 6 件塔1 遗物 + 6 件先古遗物（储君3 + 亡灵3，共 24 件）。
/// 供图鉴（先古 → 达弗 子分类）、DevConsole ancient 命令补全等使用。
/// 注：v2.2 起储君/亡灵角色池为"先古版"而非稀有版本体，故清单移除 6 件 rare、加入 6 件先古。
/// </summary>
public static class DarvAllPossibleOptions
{
    private static readonly RelicModel[] Pool =
    {
        ModelDb.Relic<Astrolabe>(),
        ModelDb.Relic<PandorasBox>(),
        ModelDb.Relic<BlackStar>(),
        ModelDb.Relic<CallingBell>(),
        ModelDb.Relic<EmptyCage>(),
        ModelDb.Relic<Sozu>(),
        ModelDb.Relic<Ectoplasm>(),
        ModelDb.Relic<PhilosophersStone>(),
        ModelDb.Relic<VelvetChoker>(),
        ModelDb.Relic<SneckoEye>(),
        ModelDb.Relic<RunicPyramid>(),
        ModelDb.Relic<DustyTome>(),
        ModelDb.Relic<RunicCube>(),
        ModelDb.Relic<MarkOfPain>(),
        ModelDb.Relic<HoveringKite>(),
        ModelDb.Relic<WristBlade>(),
        ModelDb.Relic<NuclearBattery>(),
        ModelDb.Relic<Inserter>(),
        ModelDb.Relic<AncientLunarPastry>(),
        ModelDb.Relic<AncientMiniRegent>(),
        ModelDb.Relic<AncientOrangeDough>(),
        ModelDb.Relic<AncientBookmark>(),
        ModelDb.Relic<AncientBigHat>(),
        ModelDb.Relic<AncientIvoryTile>(),
    };

    public static IEnumerable<EventOption> Build(Darv darv)
    {
        foreach (var relic in Pool)
        {
            yield return darv.RelicOption(relic.ToMutable());
        }
    }
}
