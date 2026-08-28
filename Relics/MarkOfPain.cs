using System.Threading.Tasks;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

using STS2RitsuLib.Interop.AutoRegistration;
namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 痛楚印记（塔1 BOSS 遗物移植）：每回合 +1 能量上限；战斗开始时 2 张伤口洗入抽牌堆。
/// 塔2 实现：ModifyMaxEnergy（仿贤者之石）+ BeforeCombatStart 塞伤口（仿失礼茶具）。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class MarkOfPain : RelicModel
{

    public override string PackedIconPath => "res://DarvRelicPoolMod/images/relics/mark_of_pain.png";

    public override string PackedIconOutlinePath => "res://DarvRelicPoolMod/images/relics/mark_of_pain_outline.png";

    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/mark_of_pain.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new EnergyVar(1) };

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.ForEnergy(this) };

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != Owner)
        {
            return amount;
        }
        return amount + 1m;
    }

    public override async Task BeforeCombatStart()
    {
        Flash();
        await CardPileCmd.AddToCombatAndPreview<Wound>(Owner.Creature, PileType.Draw, 2, Owner, CardPilePosition.Random);
    }
}
