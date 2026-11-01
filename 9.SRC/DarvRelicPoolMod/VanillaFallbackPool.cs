using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DarvRelicPoolMod;

/// <summary>
/// 原版逻辑回退：逐字复刻《杀戮尖塔2》原版 Darv.GenerateInitialOptions。
/// 数据源直接读取游戏静态池 Darv._validRelicSets（内容随游戏版本自适应：107 → 9 集合、111 → 11 集合），
/// 保证"职业关闭时回退到该版本原版默认逻辑"。
/// </summary>
public static class VanillaFallbackPool
{
    public static IReadOnlyList<EventOption> Generate(Darv darv, Player owner)
    {
        var rng = darv.Rng;
        var source = new List<EventOption>();
        foreach (var relicSet in Darv._validRelicSets)
        {
            if (relicSet.filter(owner))
            {
                source.Add(darv.RelicOption(rng.NextItem(relicSet.relics).ToMutable()));
            }
        }

        source.UnstableShuffle(rng);

        // 魔典守卫：角色无先古卡时魔典分支降级为 3 候选（防崩溃，官方原版同样存在此边界）
        if (rng.NextBool() && DarvOptionPool.HasAncientCards(owner))
        {
            var list = source.Take(2).ToList();
            var tome = (DustyTome)ModelDb.Relic<DustyTome>().ToMutable();
            if (owner != null)
            {
                tome.SetupForPlayer(owner);
            }
            list.Add(darv.RelicOption(tome));
            return list;
        }

        return source.Take(3).ToList();
    }
}
