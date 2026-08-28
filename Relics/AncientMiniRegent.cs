using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 先古迷你储君（迷你储君 的先古版）：你每回合第一次获得星辉、和每回合第一次消耗星辉时，
/// 各获得 1 点力量（一回合最多两次）。
/// 参考官方 MiniRegent（仅消耗星辉 +1 力/每回合一次），增加"获得星辉"通道（AfterStarsGained）。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class AncientMiniRegent : RelicModel
{
    private bool _strengthFromSpentUsedThisTurn;
    private bool _strengthFromGainedUsedThisTurn;
    public override string IconBaseName => "mini_regent";
    
    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/ancient_mini_regent.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<StrengthPower>(1m) };

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromPower<StrengthPower>() };

    public override async Task AfterStarsSpent(int amount, Player spender)
    {
        if (spender == Owner && !_strengthFromSpentUsedThisTurn)
        {
            _strengthFromSpentUsedThisTurn = true;
            Flash();
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, null);
        }
    }

    public override async Task AfterStarsGained(int amount, Player gainer)
    {
        if (gainer == Owner && !_strengthFromGainedUsedThisTurn)
        {
            _strengthFromGainedUsedThisTurn = true;
            Flash();
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, null);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature))
        {
            _strengthFromSpentUsedThisTurn = false;
            _strengthFromGainedUsedThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _strengthFromSpentUsedThisTurn = false;
        _strengthFromGainedUsedThisTurn = false;
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        try
        {
            Owner.RelicGrabBag.Remove(ModelDb.Relic<MiniRegent>());
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: failed to remove MiniRegent from grab bag: {e}");
        }
        return Task.CompletedTask;
    }
}
