using Godot;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using STS2RitsuLib.Patching.Models;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 图鉴角色光效（问题①）：官方角色专属遗物在图鉴中按角色池 LabOutlineColor 发光（铁甲红/静默绿/故障蓝）。
/// 本 mod 的遗物注册在自定义池（非角色池），需在图鉴条目渲染后按角色映射设置 outline 颜色。
/// 状态遵循官方逻辑（2026-08-26 修复）：仅 ModelVisibility.Visible（已见过）上角色色；
/// NotSeen（未见过）保持官方灰色态（白 outline），避免"灰 icon + 角色色 outline"的混合态导致光效暗淡。
/// 开启"交换痛楚印记与核能电池"时，两件遗物的颜色随互换联动。
/// </summary>
public sealed class RelicCollectionOutlinePatcher : IPatchMethod
{
    public static string PatchId => "darv_relic_collection_outline";
    public static string Description => "Apply character-colored outline to Darv mod relics in the relic collection";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets() =>
    [
        PatchTarget.Method<NRelicCollectionEntry>("_Ready"),
    ];

    public static void Postfix(NRelicCollectionEntry __instance)
    {
        try
        {
            // 与官方一致：未见过（NotSeen）与未解锁（Locked）不显示角色光效，仅已见过（Visible）上色
            if (__instance.ModelVisibility != ModelVisibility.Visible)
            {
                return;
            }
            if (__instance._relicNode is not NRelic n || n.Model == null)
            {
                return;
            }
            var settings = DarvSettings.Current;
            Color? color = GetOutlineColor(n.Model, settings);
            if (color == null)
            {
                return;
            }
            var c = color.Value;
            c.A = 0.66f; // 与官方角色池光效一致
            n.Outline.SelfModulate = c;
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: outline patch failed: {e}");
        }
    }

    private static bool IsModRelic(RelicModel relic)
    {
        return relic.Id.Entry.StartsWith("DARV_RELIC_POOL_MOD_RELIC_", StringComparison.Ordinal);
    }

    /// <summary>按角色映射颜色；互换开启时痛楚印记(原铁甲)/核能电池(原故障)互换归属。
    /// 新 6 件先古遗物：储君→橙、亡灵契约师→粉（官方角色池配色）。</summary>
    public static Color? GetOutlineColor(RelicModel relic, DarvSettings settings)
    {
        if (!IsModRelic(relic))
        {
            return null;
        }
        string entry = relic.Id.Entry;
        bool swapped = settings.SwapMarkOfPainNuclearBattery;

        bool ironclad = entry == "DARV_RELIC_POOL_MOD_RELIC_RUNIC_CUBE"
            || (entry == "DARV_RELIC_POOL_MOD_RELIC_MARK_OF_PAIN" && !swapped)
            || (entry == "DARV_RELIC_POOL_MOD_RELIC_NUCLEAR_BATTERY" && swapped);
        bool silent = entry is "DARV_RELIC_POOL_MOD_RELIC_HOVERING_KITE" or "DARV_RELIC_POOL_MOD_RELIC_WRIST_BLADE";
        bool defect = entry == "DARV_RELIC_POOL_MOD_RELIC_INSERTER"
            || (entry == "DARV_RELIC_POOL_MOD_RELIC_NUCLEAR_BATTERY" && !swapped)
            || (entry == "DARV_RELIC_POOL_MOD_RELIC_MARK_OF_PAIN" && swapped);
        bool regent = entry is "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_LUNAR_PASTRY"
            or "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_MINI_REGENT"
            or "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_ORANGE_DOUGH";
        bool necrobinder = entry is "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_BOOKMARK"
            or "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_BIG_HAT"
            or "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_IVORY_TILE";

        if (ironclad)
        {
            return StsColors.red;
        }
        if (silent)
        {
            return StsColors.green;
        }
        if (regent)
        {
            return StsColors.orange;
        }
        if (necrobinder)
        {
            return StsColors.pink;
        }
        if (defect)
        {
            return StsColors.blue;
        }
        return null;
    }
}
