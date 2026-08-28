using Godot;
using MegaCrit.Sts2.Core.Nodes;
using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;

namespace DarvRelicPoolMod;

/// <summary>
/// 设置页（ritsulib ModSettings）：5 职业开关 + 交换开关 + 蛇眼按钮行 + 联机说明 + 保存并退出。
/// 文案经 I18N（eng/zhs/zht 三份）按游戏语言自动切换。
/// 保存语义（用户设计）：**只有点击"保存并退出"才落盘；未保存的改动不生效、不保留**——
/// 开关改动仅暂存（DeferredSaveBinding._pending），达弗选项始终按磁盘值生成；
/// 退出设置界面（SettingsPendingResetPatcher）丢弃未保存改动，重进显示修改前状态。
/// 蛇眼开关：五个职业按钮并排一行（AddCustom + HBoxContainer + Godot Button，自绘状态）。
/// </summary>
public static class DarvSettingsUi
{
    private static I18N _i18n = null!;
    private static DeferredSaveBinding? _ironcladBinding;
    private static DeferredSaveBinding? _silentBinding;
    private static DeferredSaveBinding? _defectBinding;
    private static DeferredSaveBinding? _regentBinding;
    private static DeferredSaveBinding? _necrobinderBinding;
    private static DeferredSaveBinding? _swapBinding;
    private static DeferredSaveBinding? _sneckoIroncladBinding;
    private static DeferredSaveBinding? _sneckoSilentBinding;
    private static DeferredSaveBinding? _sneckoDefectBinding;
    private static DeferredSaveBinding? _sneckoRegentBinding;
    private static DeferredSaveBinding? _sneckoNecrobinderBinding;

    /// <summary>
    /// 蛇眼按钮行的当前控件实例（页面缓存复用时保持同一实例）。
    /// ritsulib 自定义条目（AddCustom）不注册刷新回调（RegisterRefreshWhenAlive 为 internal），
    /// 因此重进设置界面时按钮状态由 SettingsPendingResetPatcher → ResetPendingBindings 手动同步
    /// （清暂存后按 Read()=磁盘值 刷新视觉，与 ritsulib 默认开关的刷新周期行为一致）。
    /// </summary>
    private static readonly List<(ModSettingsToggleControl Button, DeferredSaveBinding Binding)> _sneckoButtons = [];

    public static void Register()
    {
        _i18n = RitsuLibFramework.CreateModLocalization(
            ModEntry.ModId,
            "settings",
            pckFolders: ["res://DarvRelicPoolMod/localization/settings"]);

        _ironcladBinding = new DeferredSaveBinding(true, s => s.IroncladEnabled, (s, v) => s.IroncladEnabled = v);
        _silentBinding = new DeferredSaveBinding(true, s => s.SilentEnabled, (s, v) => s.SilentEnabled = v);
        _defectBinding = new DeferredSaveBinding(true, s => s.DefectEnabled, (s, v) => s.DefectEnabled = v);
        _regentBinding = new DeferredSaveBinding(true, s => s.RegentEnabled, (s, v) => s.RegentEnabled = v);
        _necrobinderBinding = new DeferredSaveBinding(true, s => s.NecrobinderEnabled, (s, v) => s.NecrobinderEnabled = v);
        _swapBinding = new DeferredSaveBinding(false, s => s.SwapMarkOfPainNuclearBattery, (s, v) => s.SwapMarkOfPainNuclearBattery = v);
        _sneckoIroncladBinding = new DeferredSaveBinding(true, s => s.SneckoIronclad, (s, v) => s.SneckoIronclad = v);
        _sneckoSilentBinding = new DeferredSaveBinding(true, s => s.SneckoSilent, (s, v) => s.SneckoSilent = v);
        _sneckoDefectBinding = new DeferredSaveBinding(true, s => s.SneckoDefect, (s, v) => s.SneckoDefect = v);
        _sneckoRegentBinding = new DeferredSaveBinding(true, s => s.SneckoRegent, (s, v) => s.SneckoRegent = v);
        _sneckoNecrobinderBinding = new DeferredSaveBinding(true, s => s.SneckoNecrobinder, (s, v) => s.SneckoNecrobinder = v);

        RitsuLibFramework.RegisterModSettings(ModEntry.ModId, page => page
            .WithTitle(T("page.title"))
            .WithModDisplayName(T("page.title"))
            .AddSection("top", section => section
                .WithTitle(T("section.top.title"))
                .AddParagraph("topNotice", T("section.top.notice")))
            .AddSection("per_character", section => section
                .WithTitle(T("section.perCharacter.title"))
                .WithDescription(T("section.perCharacter.description"))
                .AddToggle("ironclad", T("toggle.ironclad.label"), _ironcladBinding!, T("toggle.ironclad.description"))
                .AddToggle("silent", T("toggle.silent.label"), _silentBinding!, T("toggle.silent.description"))
                .AddToggle("defect", T("toggle.defect.label"), _defectBinding!, T("toggle.defect.description"))
                .AddToggle("regent", T("toggle.regent.label"), _regentBinding!, T("toggle.regent.description"))
                .AddToggle("necrobinder", T("toggle.necrobinder.label"), _necrobinderBinding!, T("toggle.necrobinder.description")))
            .AddSection("snecko", section => section
                .WithTitle(T("section.snecko.title"))
                .WithDescription(T("section.snecko.description"))
                .AddCustom("sneckoRow", T("section.snecko.title"), BuildSneckoRow))
            .AddSection("swap", section => section
                .WithTitle(T("section.swap.title"))
                .AddToggle("swapPainInserter", T("toggle.swap.label"), _swapBinding!, T("toggle.swap.description")))
            .AddSection("multiplayer", section => section
                .WithTitle(T("section.multiplayer.title"))
                .AddParagraph("multiplayerNotice", T("section.multiplayer.notice")))
            .AddSection("save", section => section
                .WithTitle(T("section.save.title"))
                .AddParagraph("saveNotice", T("section.save.notice"))
                .AddButton("saveAndQuit", T("button.saveAndQuit.label"), T("button.saveAndQuit.text"),
                    SaveAndQuit,
                    description: T("button.saveAndQuit.description"))));
    }

    /// <summary>
    /// 蛇眼按钮行（两行结构）：第一行五个职业名文本（与下方按钮列对齐），
    /// 第二行五个 ritsulib **官方开关控件**（ModSettingsToggleControl——设置页其余开关同款：
    /// 关闭深墨绿 / 开启荧光绿，双色与 On/Off 文字均为官方样式，零自定义样式代码）。
    /// 开关状态 = binding.Read()（暂存 ?? 磁盘）；点击仅暂存（Write），不落盘；
    /// 退出设置界面后暂存被清（SettingsPendingResetPatcher → ResetPendingBindings → SyncSneckoRowVisuals），
    /// 重进显示修改前状态。
    /// </summary>
    private static Control BuildSneckoRow(IModSettingsUiActionHost host)
    {
        string[] names = ["ironclad", "silent", "defect", "regent", "necrobinder"];
        DeferredSaveBinding[] bindings =
        [
            _sneckoIroncladBinding!,
            _sneckoSilentBinding!,
            _sneckoDefectBinding!,
            _sneckoRegentBinding!,
            _sneckoNecrobinderBinding!,
        ];

        var column = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
            MouseFilter = Control.MouseFilterEnum.Pass,
        };
        column.AddThemeConstantOverride("separation", 4);

        // 第一行：职业名（每个与下方开关等宽对齐，水平居中）
        var labelRow = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        labelRow.AddThemeConstantOverride("separation", 8);
        foreach (var name in names)
        {
            labelRow.AddChild(new Label
            {
                Text = _i18n.Get("character." + name, name),
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                HorizontalAlignment = HorizontalAlignment.Center,
            });
        }
        column.AddChild(labelRow);

        // 第二行：五个官方开关（双色由 ritsulib 主题管理）
        var buttonRow = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = Control.MouseFilterEnum.Pass,
        };
        buttonRow.AddThemeConstantOverride("separation", 8);

        _sneckoButtons.Clear();
        for (var i = 0; i < bindings.Length; i++)
        {
            var binding = bindings[i];
            var toggle = new ModSettingsToggleControl(binding.Read(), on => OnSneckoToggleChanged(host, binding, on))
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
                CustomMinimumSize = new Vector2(0f, 44f),
            };
            buttonRow.AddChild(toggle);
            _sneckoButtons.Add((toggle, binding));
        }

        column.AddChild(buttonRow);
        return column;
    }

    /// <summary>官方开关点击回调（控件内部已翻转并切换双色视觉）：仅写入暂存（不落盘、不影响当前局），并标记脏。</summary>
    private static void OnSneckoToggleChanged(IModSettingsUiActionHost host, DeferredSaveBinding binding, bool on)
    {
        binding.Write(on);
        host.MarkDirty(binding);
    }

    /// <summary>
    /// 把蛇眼开关行的视觉同步为当前 Read() 值（清暂存后 = 磁盘值）。
    /// 弥补 AddCustom 条目不在 ritsulib 刷新周期内的缺口（RegisterRefreshWhenAlive 为 internal）。
    /// </summary>
    private static void SyncSneckoRowVisuals()
    {
        foreach (var (toggle, binding) in _sneckoButtons)
        {
            toggle.SetValue(binding.Read());
        }
    }

    /// <summary>
    /// 清空全部开关的本会话暂存值（SettingsPendingResetPatcher 在设置界面打开/关闭时调用），
    /// 并同步蛇眼按钮行视觉。之后 UI 刷新回调 Read() 返回磁盘值 → 未保存的改动不再显示
    /// （重进显示修改前状态，与 ritsulib 默认开关行为一致）。
    /// </summary>
    public static void ResetPendingBindings()
    {
        _ironcladBinding?.ClearPending();
        _silentBinding?.ClearPending();
        _defectBinding?.ClearPending();
        _regentBinding?.ClearPending();
        _necrobinderBinding?.ClearPending();
        _swapBinding?.ClearPending();
        _sneckoIroncladBinding?.ClearPending();
        _sneckoSilentBinding?.ClearPending();
        _sneckoDefectBinding?.ClearPending();
        _sneckoRegentBinding?.ClearPending();
        _sneckoNecrobinderBinding?.ClearPending();
        SyncSneckoRowVisuals();
    }

    /// <summary>
    /// 唯一落盘途径：以磁盘值为基础、用各绑定本会话暂存改动（PendingValue）覆盖后保存，然后退出游戏。
    /// </summary>
    private static void SaveAndQuit()
    {
        try
        {
            var store = RitsuLibFramework.GetDataStore(ModEntry.ModId);
            store.Modify<DarvSettings>("settings", s =>
            {
                if (_ironcladBinding!.PendingValue is bool v) s.IroncladEnabled = v;
                if (_silentBinding!.PendingValue is bool v2) s.SilentEnabled = v2;
                if (_defectBinding!.PendingValue is bool v3) s.DefectEnabled = v3;
                if (_regentBinding!.PendingValue is bool v4) s.RegentEnabled = v4;
                if (_necrobinderBinding!.PendingValue is bool v5) s.NecrobinderEnabled = v5;
                if (_swapBinding!.PendingValue is bool v6) s.SwapMarkOfPainNuclearBattery = v6;
                if (_sneckoIroncladBinding!.PendingValue is bool v7) s.SneckoIronclad = v7;
                if (_sneckoSilentBinding!.PendingValue is bool v8) s.SneckoSilent = v8;
                if (_sneckoDefectBinding!.PendingValue is bool v9) s.SneckoDefect = v9;
                if (_sneckoRegentBinding!.PendingValue is bool v10) s.SneckoRegent = v10;
                if (_sneckoNecrobinderBinding!.PendingValue is bool v11) s.SneckoNecrobinder = v11;
            });
            store.Save("settings");

            ResetPendingBindings();

            MegaCrit.Sts2.Core.Logging.Log.Info($"{ModEntry.ModId}: settings saved and game quitting.");
            NGame.Instance.Quit();
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: failed to save settings: {e}");
        }
    }

    private static ModSettingsText T(string key)
    {
        return ModSettingsText.I18N(_i18n, key, key);
    }
}
