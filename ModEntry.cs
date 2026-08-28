using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Patching.Core;

namespace DarvRelicPoolMod;

/// <summary>
/// Mod 入口：注册程序集、注册设置数据、注册设置页、应用达弗选项 patch。
/// 生效语义（用户设计）：无快照；只有点击"保存并退出"才落盘生效；
/// 未保存的改动仅 UI 暂存，不生效、退出设置界面即丢弃（SettingsPendingResetPatcher）。
/// </summary>
[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    public const string ModId = "DarvRelicPoolMod";

    public static void Initialize()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

            // 设置数据存储（设置页与运行期读取共用）
            using (RitsuLibFramework.BeginModDataRegistration(ModId))
            {
                RitsuLibFramework.GetDataStore(ModId).Register(
                    key: "settings",
                    fileName: "settings.json",
                    scope: STS2RitsuLib.Utils.Persistence.SaveScope.Global,
                    defaultFactory: () => new DarvSettings(),
                    autoCreateIfMissing: true);
            }

            // 设置页（含多语言、保存并退出、联机说明）
            DarvSettingsUi.Register();

            // 达弗选项 patch（关键路径，失败即禁用 mod）
            var patcher = RitsuLibFramework.CreatePatcher(ModId, "darv");
            patcher.RegisterPatch<Patches.DarvOptionsPatcher>();
            patcher.RegisterPatch<Patches.DarvAllPossibleOptionsPatcher>();
            patcher.RegisterPatch<Patches.WristBladeCompatPatcher>();

            // 图鉴外观 + 设置暂存清理 + 先古图标渲染层翻转（可选：失败不影响玩法）
            var cosmeticPatcher = RitsuLibFramework.CreatePatcher(ModId, "darv_cosmetic");
            cosmeticPatcher.RegisterPatch<Patches.RelicCollectionOutlinePatcher>();
            cosmeticPatcher.RegisterPatch<Patches.RelicCollectionSortPatcher>();
            cosmeticPatcher.RegisterPatch<Patches.SettingsPendingResetPatcher>();
            cosmeticPatcher.RegisterPatch<Patches.NRelicMirrorPatcher>();
            cosmeticPatcher.PatchAll();
            RitsuLibFramework.ApplyRequiredPatcher(patcher, () =>
            {
                Log.Error($"{ModId}: required Darv patches failed to apply; disabling mod behavior.");
            });

            Log.Info($"{ModId}: initialized.");
        }
        catch (Exception e)
        {
            Log.Error($"{ModId}: failed to initialize: {e}");
        }
    }
}
