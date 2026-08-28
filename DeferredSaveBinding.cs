using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace DarvRelicPoolMod;

/// <summary>
/// 延迟保存绑定（用户设计：**只有点击"保存并退出"才落盘；未保存的改动不生效、不保留**）。
/// - Write() 只写入内存暂存（_pending）：UI 显示改动值，但**不写入任何存储实例**（DarvSettings.Load()
///   返回的是 ModDataStore 内部持有的同一实例，绝不可直接改，否则任何触发 store.Save 都会落盘）
/// - Read()  返回"暂存值 ?? 磁盘值"：设置界面内显示暂存改动（不弹回）；退出设置界面后由
///   SettingsPendingResetPatcher 清空暂存 → 重进显示磁盘值（修改前状态），与生效值一致
/// - Save()  空操作（阻止 ritsulib 设置界面的防抖自动保存：0.35s FlushDirtyBindings 不再落盘）
/// - 唯一落盘途径：DarvSettingsUi 的"保存并退出"按钮（以磁盘为基础、用 PendingValue 覆盖改动项后 Save）
/// </summary>
public sealed class DeferredSaveBinding : IStructuredModSettingsValueBinding<bool>, IDefaultModSettingsValueBinding<bool>
{
    private readonly Func<DarvSettings, bool> _getter;
    private readonly Action<DarvSettings, bool> _setter;
    private readonly bool _defaultValue;
    private bool? _pending;

    public DeferredSaveBinding(bool defaultValue, Func<DarvSettings, bool> getter, Action<DarvSettings, bool> setter)
    {
        _defaultValue = defaultValue;
        _getter = getter;
        _setter = setter;
    }

    public string ModId => ModEntry.ModId;

    public string DataKey => "settings";

    public SaveScope Scope => SaveScope.Global;

    public IStructuredModSettingsValueAdapter<bool> Adapter { get; } = ModSettingsStructuredData.Json<bool>();

    public bool Read()
    {
        // 暂存值 ?? 磁盘值：UI 初始化/刷新显示暂存改动（不弹回）；暂存被清后显示磁盘值。
        return _pending ?? _getter(DarvSettings.Load());
    }

    public void Write(bool value)
    {
        // 仅内存暂存，不落盘、不改任何存储实例。
        _pending = value;
    }

    public void Save()
    {
        // 空操作：只有"保存并退出"按钮显式用 PendingValue 覆盖磁盘后才落盘。
    }

    public bool CreateDefaultValue()
    {
        return _defaultValue;
    }

    /// <summary>本次会话内有改动的值；null 表示未改动（保存时以磁盘值为准）。</summary>
    public bool? PendingValue => _pending;

    /// <summary>清空暂存（保存成功后、或退出设置界面丢弃未保存改动时）。</summary>
    public void ClearPending()
    {
        _pending = null;
    }
}
