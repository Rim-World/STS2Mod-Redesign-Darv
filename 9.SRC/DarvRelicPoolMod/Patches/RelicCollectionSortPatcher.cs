using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Patching.Models;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 图鉴排序：图鉴"先古"分类的达弗子分类默认按遗物标题排序；
/// 本 patch 把本 mod 遗物按 铁甲(2)→静默(2)→储君(3)→亡灵(3)→故障(2) 的五职业顺序连续排列
/// （用户要求：铁甲战士-静默猎手-储君-亡灵契约师-故障机器人）。
/// 开启"交换痛楚印记与核能电池"时，铁甲/故障组内顺序随互换联动。
/// </summary>
public sealed class RelicCollectionSortPatcher : IPatchMethod
{
    private const string Prefix = "DARV_RELIC_POOL_MOD_RELIC_";

    // 新 6 件先古遗物（储君 3 件在前、亡灵契约师 3 件在后；位于静默组与故障组之间）
    private static readonly string[] AncientMiddle =
    [
        Prefix + "ANCIENT_LUNAR_PASTRY",
        Prefix + "ANCIENT_MINI_REGENT",
        Prefix + "ANCIENT_ORANGE_DOUGH",
        Prefix + "ANCIENT_BOOKMARK",
        Prefix + "ANCIENT_BIG_HAT",
        Prefix + "ANCIENT_IVORY_TILE",
    ];

    private static readonly string[] DefaultOrder =
    [
        Prefix + "RUNIC_CUBE",
        Prefix + "MARK_OF_PAIN",
        Prefix + "HOVERING_KITE",
        Prefix + "WRIST_BLADE",
        .. AncientMiddle,
        Prefix + "NUCLEAR_BATTERY",
        Prefix + "INSERTER",
    ];

    private static readonly string[] SwappedOrder =
    [
        Prefix + "RUNIC_CUBE",
        Prefix + "NUCLEAR_BATTERY",
        Prefix + "HOVERING_KITE",
        Prefix + "WRIST_BLADE",
        .. AncientMiddle,
        Prefix + "MARK_OF_PAIN",
        Prefix + "INSERTER",
    ];

    public static string PatchId => "darv_relic_collection_sort";
    public static string Description => "Group Darv mod relics by character at the end of the Darv subcategory";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets() =>
    [
        PatchTarget.Method<NRelicCollectionCategory>(nameof(NRelicCollectionCategory.LoadRelics)),
    ];

    public static void Postfix(NRelicCollectionCategory __instance, RelicRarity relicRarity)
    {
        try
        {
            if (relicRarity != RelicRarity.Ancient)
            {
                return;
            }
            var settings = DarvSettings.Current;
            var order = settings.SwapMarkOfPainNuclearBattery ? SwappedOrder : DefaultOrder;

            foreach (var sub in __instance._subCategories)
            {
                var container = sub._relicsContainer;
                if (container == null)
                {
                    continue;
                }
                var entries = container.GetChildren().OfType<NRelicCollectionEntry>().ToList();
                if (entries.Count == 0)
                {
                    continue;
                }
                var modEntries = entries.Where(e => e.relic != null && e.relic.Id.Entry.StartsWith(Prefix, System.StringComparison.Ordinal)).ToList();
                if (modEntries.Count == 0)
                {
                    continue;
                }
                var nonMod = entries.Where(e => !modEntries.Contains(e)).ToList();
                var orderedMod = new List<NRelicCollectionEntry>(modEntries.Count);
                foreach (var entryName in order)
                {
                    var match = modEntries.FirstOrDefault(e => e.relic.Id.Entry == entryName);
                    if (match != null)
                    {
                        orderedMod.Add(match);
                    }
                }
                // 其余未匹配的 mod 条目（理论不发生）追加在末尾保持稳定
                orderedMod.AddRange(modEntries.Where(e => !orderedMod.Contains(e)));

                foreach (var e in entries)
                {
                    container.RemoveChild(e);
                }
                foreach (var e in nonMod)
                {
                    container.AddChild(e);
                }
                foreach (var e in orderedMod)
                {
                    container.AddChild(e);
                }
            }
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: sort patch failed: {e}");
        }
    }
}
