using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using STS2RitsuLib.Patching.Models;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 图鉴外观补丁（0.113 版）：在 NRelicCollectionCategory.LoadRelicNodes 之后，统一处理该分类的
/// 达弗遗物角色色轮廓和排序。
/// 不再 patch NRelicCollectionEntry._Ready / NRelicCollectionCategory.LoadRelics：
/// 这两个方法在 0.113 中已被其他补丁（MintySpire2、RitsuLib Transpiler）占用；
/// 再叠加补丁会让 MonoMod JIT 钩子抛 ArgumentException，补丁体完全无法执行。
/// LoadRelicNodes 是私有的建节点 helper，107/111/113 签名一致，当前没有其他 mod 补丁；
/// 它在条目 _Ready 之后、NRelicCollection.LoadRelics 设置焦点邻居之前执行，排序不会破坏键盘导航。
/// </summary>
public sealed class RelicCollectionOutlinePatcher : IPatchMethod
{
    private const string ModRelicPrefix = "DARV_RELIC_POOL_MOD_RELIC_";

    public static string PatchId => "darv_relic_collection_appearance";
    public static string Description => "Apply character-colored outlines and stable ordering to Darv relics while the relic collection creates its entries";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets() =>
    [
        // 目标缺失时只忽略补丁，不影响 mod 加载；107/111/113 均存在。
        PatchTarget.OptionalMethod<NRelicCollectionCategory>("LoadRelicNodes"),
    ];

    public static void Postfix(NRelicCollectionCategory __instance)
    {
        try
        {
            var container = __instance.GetNodeOrNull<GridContainer>("%RelicsContainer");
            if (container == null)
            {
                return;
            }

            var entries = container.GetChildren()
                .OfType<NRelicCollectionEntry>()
                .Where(GodotObject.IsInstanceValid)
                .ToList();

            foreach (var entry in entries)
            {
                ApplyOutline(entry);
            }

            RelicCollectionSortPatcher.Apply(entries);
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: relic collection appearance patch failed: {e}");
        }
    }

    /// <summary>
    /// 兜底：只吞 MonoMod JIT 钩子的 ArgumentException（目标被其他 mod 叠加时的已知故障），
    /// 原版 LoadRelicNodes 自身抛出的异常继续向上抛，不掩盖游戏问题。
    /// </summary>
    public static Exception? Finalizer(Exception? __exception)
    {
        if (__exception is ArgumentException &&
            (__exception.StackTrace?.Contains("MonoMod", StringComparison.Ordinal) ?? false))
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: relic collection appearance JIT failure (finalizer): {__exception}");
            return null;
        }
        return __exception;
    }

    private static void ApplyOutline(NRelicCollectionEntry entry)
    {
        if (entry.ModelVisibility != ModelVisibility.Visible)
        {
            return;
        }

        var holder = entry.GetNodeOrNull<Control>("RelicHolder");
        var nRelic = holder?.GetChildren().OfType<NRelic>().FirstOrDefault();
        if (nRelic == null || nRelic.Outline == null)
        {
            return;
        }

        RelicModel model;
        try
        {
            model = nRelic.Model;
        }
        catch (InvalidOperationException)
        {
            return;
        }

        var color = GetOutlineColor(model, DarvSettings.Current);
        if (color == null)
        {
            return;
        }

        var c = color.Value;
        c.A = 0.66f; // 与官方角色池光效一致
        nRelic.Outline.SelfModulate = c;
    }

    private static bool IsModRelic(RelicModel relic)
    {
        return relic.Id.Entry.StartsWith(ModRelicPrefix, StringComparison.Ordinal);
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
