using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using STS2RitsuLib.Interop.AutoRegistration;
namespace DarvRelicPoolMod.Relics;

/// <summary>
/// 袖剑（塔1 BOSS 遗物移植）：0 费攻击牌造成的伤害 +4。
/// 塔2 实现：ModifyDamageAdditive（仿 打击木人）+ 0 费判定（仿 万物归一：能量费==0 且非 X 费）。
/// 注：塔2 星费体系下，星费&gt;0 的卡按能量费判定（星费攻击牌极少，与万物归一一致）。
/// 双版本：107 与 111 的 ModifyDamageAdditive 签名不同（5 参 vs 6 参），override 无法跨版本编译，
/// 因此本类不写 override，统一由 Patches/WristBladeCompatPatcher 的 optional Postfix 注入
/// （运行时反射解析 5 参/6 参目标，版本自适应）。判定逻辑在 ShouldGrantBonus。
/// </summary>
[RegisterRelic(typeof(DarvModRelicPool))]
public sealed class WristBlade : RelicModel
{

    public override string PackedIconPath => "res://DarvRelicPoolMod/images/relics/wBlade.png";

    public override string PackedIconOutlinePath => "res://DarvRelicPoolMod/images/relics/wBlade_outline.png";

    public override string BigIconPath => "res://DarvRelicPoolMod/images/relics/wBlade.png";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    /// <summary>
    /// 统一判定：PoweredAttack 且 攻击牌 且 归属本人 且 非 X 费 且 能量费==0 → +4。
    /// WristBladeCompatPatcher.Postfix（5 参/6 参目标共用）调用。
    /// </summary>
    public static bool ShouldGrantBonus(WristBlade self, Creature? dealer, ValueProp props, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack())
        {
            return false;
        }
        if (cardSource == null)
        {
            return false;
        }
        if (cardSource.Type != CardType.Attack)
        {
            return false;
        }
        if (dealer != self.Owner.Creature && cardSource.Owner != self.Owner)
        {
            return false;
        }
        if (cardSource.EnergyCost.CostsX)
        {
            return false;
        }
        if (cardSource.EnergyCost.GetWithModifiers(CostModifiers.All) != 0)
        {
            return false;
        }
        return true;
    }
}
