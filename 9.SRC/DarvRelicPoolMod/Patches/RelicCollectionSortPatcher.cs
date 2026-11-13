using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 图鉴排序（0.113）：图鉴“先古”分类的达弗子分类默认按遗物标题排序；
/// 本 helper 把本 mod 遗物按 铁甲(2)→静默(2)→储君(3)→亡灵(3)→故障(2) 的五职业顺序连续排列。
/// 由 RelicCollectionOutlinePatcher 在 LoadRelicNodes 之后调用，不再单独挂 Harmony 补丁。
/// 不读取 NRelicCollectionCategory 的 private 字段：通过条目父节点 GridContainer 分组处理。
/// </summary>
public static class RelicCollectionSortPatcher
{
    private const string Prefix = "DARV_RELIC_POOL_MOD_RELIC_";

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

    public static void Apply(IReadOnlyList<NRelicCollectionEntry> entries)
    {
        foreach (var group in entries
                     .Where(GodotObject.IsInstanceValid)
                     .GroupBy(entry => entry.GetParent())
                     .Where(group => group.Key is GridContainer))
        {
            var modEntries = group.Where(IsModEntry).ToList();
            if (modEntries.Count == 0)
            {
                continue;
            }

            var container = (GridContainer)group.Key!;
            var order = DarvSettings.Current.SwapMarkOfPainNuclearBattery ? SwappedOrder : DefaultOrder;
            var nonMod = group.Where(entry => !modEntries.Contains(entry)).ToList();
            var orderedMod = new List<NRelicCollectionEntry>(modEntries.Count);

            foreach (var entryName in order)
            {
                var match = modEntries.FirstOrDefault(entry => entry.relic.Id.Entry == entryName);
                if (match != null)
                {
                    orderedMod.Add(match);
                }
            }

            // 其余未匹配的 mod 条目（理论不发生）追加在末尾保持稳定
            orderedMod.AddRange(modEntries.Where(entry => !orderedMod.Contains(entry)));

            foreach (var entry in group)
            {
                container.RemoveChild(entry);
            }
            foreach (var entry in nonMod)
            {
                container.AddChild(entry);
            }
            foreach (var entry in orderedMod)
            {
                container.AddChild(entry);
            }
        }
    }

    private static bool IsModEntry(NRelicCollectionEntry entry)
    {
        return entry.relic != null
            && entry.relic.Id.Entry.StartsWith(Prefix, StringComparison.Ordinal);
    }
}
