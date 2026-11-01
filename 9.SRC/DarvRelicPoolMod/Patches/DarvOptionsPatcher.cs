using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using STS2RitsuLib.Patching.Models;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 达弗选项生成替换：Prefix 拦截 Darv.GenerateInitialOptions（两版本签名一致）。
/// </summary>
public sealed class DarvOptionsPatcher : IPatchMethod
{
    public static string PatchId => "darv_relic_pool_generate";
    public static string Description => "Replace Darv.GenerateInitialOptions with the custom three-slot pool";
    public static bool IsCritical => true;

    public static ModPatchTarget[] GetTargets() =>
    [
        PatchTarget.Method<Darv>("GenerateInitialOptions"),
    ];

    public static bool Prefix(Darv __instance, ref IReadOnlyList<EventOption> __result)
    {
        try
        {
            if (__instance.Owner == null)
            {
                return true; // 无 Owner 时不干预（理论不发生）
            }
            __result = DarvOptionPool.Generate(__instance, __instance.Owner);
            return false;
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: DarvOptionPool failed, falling back to vanilla: {e}");
            return true;
        }
    }
}
