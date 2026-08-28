using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 先古书签（书签 的先古版）：每回合结束时，随机一张被保留的牌在本场战斗中的耗能永久减少 1。
/// 参考官方 Bookmark（随机保留牌耗能 -1 直到打出），改为 AddThisCombat（本场战斗永久）。
/// 保留判定（2026-08-26 最终确认）：与官方完全一致，直接使用 AfterFlush.retainedCards
/// （官方实测：书签对符文金字塔/均衡/计划妥当保留的手牌同样生效，无额外过滤），
/// 仅保留官方过滤：非 X 费且当前耗能 &gt; 0。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class AncientBookmark : RelicModel
{
    public override string IconBaseName => "bookmark";
    
    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/ancient_bookmark.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Retain) };

    public override Task AfterFlush(PlayerChoiceContext choiceContext, Player player, IReadOnlyCollection<CardModel> flushedCards, IReadOnlyCollection<CardModel> retainedCards)
    {
        if (player != Owner)
        {
            return Task.CompletedTask;
        }
        var candidates = retainedCards
            .Where(c => !c.EnergyCost.CostsX && c.EnergyCost.GetWithModifiers(CostModifiers.Local) > 0)
            .ToList();
        if (candidates.Count == 0)
        {
            return Task.CompletedTask;
        }
        Flash();
        Owner.RunState.Rng.CombatCardSelection.NextItem(candidates)?.EnergyCost.AddThisCombat(-1, reduceOnly: true);
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        try
        {
            Owner.RelicGrabBag.Remove(ModelDb.Relic<Bookmark>());
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: failed to remove Bookmark from grab bag: {e}");
        }
        return Task.CompletedTask;
    }
}
