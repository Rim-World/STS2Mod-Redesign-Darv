using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 先古月亮糕点（储君稀有遗物 月亮糕点 的先古版）：在你的回合结束时，获得 2 点星辉。
/// 参考官方 LunarPastry（回合结束 +1 星），数量改为 +2 星（两个星辉图标）。
/// 互斥：获得本遗物后，本局内官方 月亮糕点 从 RelicGrabBag 中移除（官方 Remove API）。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class AncientLunarPastry : RelicModel
{
    public override string IconBaseName => "lunar_pastry";
    
    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/ancient_lunar_pastry.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new StarsVar(2) };

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner.Creature))
        {
            Flash();
            await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
        }
    }

    public override Task AfterObtained()
    {
        try
        {
            Owner.RelicGrabBag.Remove(ModelDb.Relic<LunarPastry>());
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: failed to remove LunarPastry from grab bag: {e}");
        }
        return Task.CompletedTask;
    }
}
