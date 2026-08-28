using STS2RitsuLib;

namespace DarvRelicPoolMod;

/// <summary>
/// 设置模型（ModDataStore JSON 持久化，公共属性序列化）。
/// 5 职业开关默认全开；交换开关默认关闭；5 角色"槽位2 异蛇之眼"开关默认全开。
/// 生效语义（用户设计）：**无快照 + 只有"保存并退出"才落盘生效**。
/// - Current 直接读磁盘态（ModDataStore 内部实例，只读使用，绝不可修改——修改会污染存储内部对象）
/// - 未保存的改动只存在于 DeferredSaveBinding 的暂存（_pending），不生效、退出设置界面即丢弃
/// - "保存并退出"把暂存覆盖到磁盘后，重启游戏立即生效（无需等新局）
/// </summary>
public sealed class DarvSettings
{
    public bool IroncladEnabled { get; set; } = true;
    public bool SilentEnabled { get; set; } = true;
    public bool DefectEnabled { get; set; } = true;
    public bool RegentEnabled { get; set; } = true;
    public bool NecrobinderEnabled { get; set; } = true;

    /// <summary>交换铁甲战士池的痛楚印记与故障机器人池的核能电池。</summary>
    public bool SwapMarkOfPainNuclearBattery { get; set; }

    /// <summary>槽位2（第二个选项）是否出现异蛇之眼，按角色单独开关；默认全开=现状。</summary>
    public bool SneckoIronclad { get; set; } = true;
    public bool SneckoSilent { get; set; } = true;
    public bool SneckoDefect { get; set; } = true;
    public bool SneckoRegent { get; set; } = true;
    public bool SneckoNecrobinder { get; set; } = true;

    public static DarvSettings Load()
    {
        return RitsuLibFramework.GetDataStore(ModEntry.ModId).Get<DarvSettings>("settings");
    }

    /// <summary>当前生效的设置 = 磁盘值（未保存的改动不影响；保存并退出后重启即按新值）。</summary>
    public static DarvSettings Current => Load();

    public bool IsEnabledForCharacter(string characterEntry)
    {
        return characterEntry switch
        {
            "IRONCLAD" => IroncladEnabled,
            "SILENT" => SilentEnabled,
            "DEFECT" => DefectEnabled,
            "REGENT" => RegentEnabled,
            "NECROBINDER" => NecrobinderEnabled,
            _ => true,
        };
    }

    /// <summary>槽位2 是否出现异蛇之眼（按角色）。职业回退原版时由调用方整体回退，本开关不生效。</summary>
    public bool IsSneckoEnabledForCharacter(string characterEntry)
    {
        return characterEntry switch
        {
            "IRONCLAD" => SneckoIronclad,
            "SILENT" => SneckoSilent,
            "DEFECT" => SneckoDefect,
            "REGENT" => SneckoRegent,
            "NECROBINDER" => SneckoNecrobinder,
            _ => true,
        };
    }
}
