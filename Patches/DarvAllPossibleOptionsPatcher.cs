using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using STS2RitsuLib.Patching.Models;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 达弗 AllPossibleOptions 替换：图鉴"先古 → 达弗"子分类、控制台补全等依赖该枚举。
/// 需与选项生成保持一致（槽位1+槽位2+符文金字塔+尘封魔典+6 新遗物+储君/亡灵 6 件 rare）。
/// </summary>
public sealed class DarvAllPossibleOptionsPatcher : IPatchMethod
{
    public static string PatchId => "darv_relic_pool_all_possible";
    public static string Description => "Replace Darv.AllPossibleOptions with the full custom pool";
    public static bool IsCritical => true;

    public static ModPatchTarget[] GetTargets() =>
    [
        PatchTarget.Getter<Darv>(nameof(Darv.AllPossibleOptions)),
    ];

    public static bool Prefix(Darv __instance, ref IEnumerable<EventOption> __result)
    {
        try
        {
            __result = DarvAllPossibleOptions.Build(__instance);
            return false;
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: AllPossibleOptions failed, falling back to vanilla: {e}");
            return true;
        }
    }
}
