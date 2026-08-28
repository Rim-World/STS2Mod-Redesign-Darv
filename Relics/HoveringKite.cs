using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

using STS2RitsuLib.Interop.AutoRegistration;
namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 悬浮风筝（塔1 BOSS 遗物移植）：每回合第一次弃牌时 +1 能量。
/// 塔2 实现：AfterCardDiscarded（仅主动弃牌触发，回合结束自动弃牌不触发）
/// + 每回合标记重置（AfterSideTurnStart，仿 战争艺术）。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class HoveringKite : RelicModel
{

    public override string PackedIconPath => "res://DarvRelicPoolMod/images/relics/kite.png";

    public override string PackedIconOutlinePath => "res://DarvRelicPoolMod/images/relics/kite_outline.png";

    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/kite.png";
    private bool _triggeredThisTurn;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new EnergyVar(1) };

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.ForEnergy(this) };

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner)
        {
            return;
        }
        if (Owner.Creature.Side != Owner.Creature.CombatState.CurrentSide)
        {
            return; // 仅自己回合
        }
        if (_triggeredThisTurn)
        {
            return;
        }
        _triggeredThisTurn = true;
        Flash();
        await PlayerCmd.GainEnergy(1m, Owner);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature))
        {
            _triggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _triggeredThisTurn = false;
        return Task.CompletedTask;
    }
}
