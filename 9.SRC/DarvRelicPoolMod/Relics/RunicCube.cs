using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

using STS2RitsuLib.Interop.AutoRegistration;
namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 符文立方体（塔1 BOSS 遗物移植）：每当你失去生命时，抽 1 张牌。
/// 塔2 实现：AfterCurrentHpChanged（delta &lt; 0 = 失血）→ CardPileCmd.Draw。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class RunicCube : RelicModel
{

    public override string PackedIconPath => "res://DarvRelicPoolMod/images/relics/runicCube.png";

    public override string PackedIconOutlinePath => "res://DarvRelicPoolMod/images/relics/runicCube_outline.png";

    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/runicCube.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner.Creature)
        {
            return;
        }
        if (delta >= 0)
        {
            return; // 仅失血（delta 为负）触发
        }
        if (!CombatManager.Instance.IsInProgress)
        {
            return;
        }
        Flash();
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), 1, Owner);
    }
}
