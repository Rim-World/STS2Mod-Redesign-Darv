using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 先古大帽子（大帽子 的先古版）：在每场战斗开始时，将 3 张随机升级过的虚无牌加入你的手牌。
/// 参考官方 BigHat（2 张随机虚无牌），数量 3 且生成后逐张升级。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class AncientBigHat : RelicModel
{
    public override string IconBaseName => "big_hat";
    
    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/ancient_big_hat.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new CardsVar(3) };

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Ethereal) };

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.PlayerCombatState.TurnNumber <= 1)
        {
            var ethereal = Owner.Character.CardPool
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Keywords.Contains(CardKeyword.Ethereal))
                .ToList();
            if (ethereal.Count > 0)
            {
                Flash();
                var cards = CardFactory.GetDistinctForCombat(Owner, ethereal, DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration)
                    .ToList();
                foreach (var card in cards)
                {
                    CardCmd.Upgrade(card);
                }
                Flash();
                await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
            }
        }
    }

    public override Task AfterObtained()
    {
        try
        {
            Owner.RelicGrabBag.Remove(ModelDb.Relic<BigHat>());
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: failed to remove BigHat from grab bag: {e}");
        }
        return Task.CompletedTask;
    }
}
