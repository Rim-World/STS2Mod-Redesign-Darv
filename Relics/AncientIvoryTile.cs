using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 先古象牙麻将牌（象牙麻将牌 的先古版）：每当你打出一张实际耗能大于等于 2 的牌时，获得 1 费。
/// 参考官方 IvoryTile（阈值 3），阈值改为 2；判定用 cardPlay.Resources.EnergyValue（实际支付能量）。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class AncientIvoryTile : RelicModel
{
    private const string EnergyThresholdKey = "EnergyThreshold";
    public override string IconBaseName => "ivory_tile";
    
    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/ancient_ivory_tile.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1),
        new EnergyVar(EnergyThresholdKey, 2),
    };

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.ForEnergy(this) };

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Resources.EnergyValue >= DynamicVars[EnergyThresholdKey].IntValue)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }

    public override Task AfterObtained()
    {
        try
        {
            Owner.RelicGrabBag.Remove(ModelDb.Relic<IvoryTile>());
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: failed to remove IvoryTile from grab bag: {e}");
        }
        return Task.CompletedTask;
    }
}
