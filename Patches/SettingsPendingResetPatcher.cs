using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Settings;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 设置暂存残留修正（用户设计：未保存的改动不生效、不保留）：进入/关闭 ritsulib 设置界面时
/// 清空 DeferredSaveBinding 的本会话暂存值。
/// - OnSubmenuOpened Prefix：在 EnsureUiUpToDate 刷新控件**之前**清暂存 → 重开设置页时
///   刷新回调 Read() 返回磁盘值 → 未保存的改动不显示（按钮保持修改前状态），与生效值一致
/// - OnSubmenuClosed Prefix：退出设置界面时兜底清空，防止下次打开时残留
/// - 刻意不 hook OnSubmenuHidden/OnSubmenuShown：隐藏→重新显示路径没有内容刷新点，
///   若此时清暂存会导致 UI 显示与 Read() 不一致（控件还显示旧改动值）
/// 落盘语义不变：只有点击"保存并退出"按钮才写盘。
/// </summary>
public sealed class SettingsPendingResetPatcher : IPatchMethod
{
    public static string PatchId => "darv_settings_pending_reset";
    public static string Description => "Reset deferred-save pending values when the settings submenu opens or closes";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets() =>
    [
        PatchTarget.Method<RitsuModSettingsSubmenu>("OnSubmenuOpened"),
        PatchTarget.Method<RitsuModSettingsSubmenu>("OnSubmenuClosed"),
    ];

    public static void Prefix()
    {
        DarvSettingsUi.ResetPendingBindings();
    }
}
