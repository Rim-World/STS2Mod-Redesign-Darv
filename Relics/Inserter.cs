using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

using STS2RitsuLib.Interop.AutoRegistration;
namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 机械臂（塔1 BOSS 遗物移植）：每 2 个回合获得 1 个充能球栏位。
/// 塔2 实现：HappyFlower 回合计数模式（[SavedProperty] 持久化）+ OrbCmd.AddSlots。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class Inserter : RelicModel
{

    public override string PackedIconPath => "res://DarvRelicPoolMod/images/relics/inserter.png";

    public override string PackedIconOutlinePath => "res://DarvRelicPoolMod/images/relics/inserter_outline.png";

    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/inserter.png";
    private int _turnsSeen;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => TurnsSeen;

    [SavedProperty]
    public int TurnsSeen
    {
        get => _turnsSeen;
        set
        {
            AssertMutable();
            _turnsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
        {
            return;
        }
        TurnsSeen = (TurnsSeen + 1) % 2;
        if (TurnsSeen == 0)
        {
            Flash();
            await OrbCmd.AddSlots(Owner, 1);
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        return Task.CompletedTask;
    }
}
