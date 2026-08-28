using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarvRelicPoolMod;

/// <summary>
/// 达弗 mod 专用遗物池：仅用于把 6 件塔1 新遗物注册进 ModelDb（保证 ModelDb.Relic&lt;T&gt;() 可用），
/// 不参与任何抽取（RelicGrabBag 只从 SharedRelicPool 与角色池取；本池不会被读取）。
/// </summary>
[RegisterSharedRelicPool]
public sealed class DarvModRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "colorless";
}
