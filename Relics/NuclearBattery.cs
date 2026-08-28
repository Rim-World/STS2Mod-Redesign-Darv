using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Orbs;

using STS2RitsuLib.Interop.AutoRegistration;
namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 核能电池（塔1 BOSS 遗物移植）：战斗开始时充能 1 个等离子球。
/// 塔2 实现：BeforeSideTurnStart + TurnNumber&lt;=1 → OrbCmd.Channel&lt;PlasmaOrb&gt;（仿 破损核心）。
/// 仅出现在故障机器人玩家面前，球栏位天然存在。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class NuclearBattery : RelicModel
{

    public override string PackedIconPath => "res://DarvRelicPoolMod/images/relics/battery.png";

    public override string PackedIconOutlinePath => "res://DarvRelicPoolMod/images/relics/battery_outline.png";

    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/battery.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.PlayerCombatState.TurnNumber <= 1)
        {
            Flash();
            await OrbCmd.Channel<PlasmaOrb>(new BlockingPlayerChoiceContext(), Owner);
        }
    }
}
