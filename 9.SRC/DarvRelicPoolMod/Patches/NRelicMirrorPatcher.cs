using Godot;
using MegaCrit.Sts2.Core.Nodes.Relics;
using STS2RitsuLib.Patching.Models;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 先古遗物图标渲染层翻转（2026-08-26，用户批准方案）：
/// 六件先古遗物通过 override IconBaseName 复用官方图集帧（与稀有版同一 AtlasTexture，
/// region/margin 元数据天然保留 → 光效与稀有版一致），图标方向为"官方原版"；
/// 本 patch 在渲染层做水平镜像，恢复"左右翻转"视觉。
/// 翻转范围：NRelic 下**所有 TextureRect 子节点**（Icon、Outline 光效层、背景层），
/// 覆盖所有 NRelic 场合（图鉴、战斗遗物栏、事件/达弗选项），仅对本 mod 的先古六件生效。
/// 尺寸未就绪（任一子节点 Size 为 0）时用 Tree timer 延迟一帧重跑（布局完成后 Size 才确定）。
/// 注意：不翻转 NRelic 自身 Scale——图鉴条目 hover 动画会覆盖 NRelic.Scale（1.25），
/// 子节点翻转不受其影响。
/// </summary>
public sealed class NRelicMirrorPatcher : IPatchMethod
{
    private const string AncientPrefix = "DARV_RELIC_POOL_MOD_RELIC_ANCIENT_";

    public static string PatchId => "darv_nrelic_mirror";
    public static string Description => "Mirror ancient Darv relic icons horizontally at render level (official atlas frames are unflipped)";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets() =>
    [
        PatchTarget.Method<NRelic>("_Ready"),
    ];

    public static void Postfix(NRelic __instance)
    {
        try
        {
            if (__instance.Model == null ||
                !__instance.Model.Id.Entry.StartsWith(AncientPrefix, System.StringComparison.Ordinal))
            {
                return;
            }

            bool needsDeferred = ApplyMirrorToTextureRects(__instance);

            // 任一子节点尺寸未就绪 → 延迟一帧重跑（布局完成后 Size 才确定）
            if (needsDeferred)
            {
                var tree = __instance.GetTree();
                if (tree != null)
                {
                    var timer = tree.CreateTimer(0f);
                    timer.Timeout += () =>
                    {
                        if (GodotObject.IsInstanceValid(__instance))
                        {
                            ApplyMirrorToTextureRects(__instance);
                        }
                    };
                }
            }
        }
        catch (System.Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: NRelic mirror patch failed: {e}");
        }
    }

    /// <summary>镜像 NRelic 下所有 TextureRect 子节点（Icon/Outline/背景）；返回是否有节点因尺寸未就绪而跳过。</summary>
    private static bool ApplyMirrorToTextureRects(NRelic nRelic)
    {
        bool skipped = false;
        foreach (var child in nRelic.GetChildren())
        {
            if (child is TextureRect texture)
            {
                if (ApplyMirror(texture))
                {
                    skipped = true;
                }
            }
        }
        return skipped;
    }

    /// <summary>对单个 TextureRect 做水平镜像；尺寸未就绪时返回 true（调用方延迟重试）。</summary>
    private static bool ApplyMirror(TextureRect texture)
    {
        if (!GodotObject.IsInstanceValid(texture))
        {
            return false;
        }
        if (texture.Size.X <= 0f || texture.Size.Y <= 0f)
        {
            return true;
        }
        texture.PivotOffset = texture.Size * 0.5f;
        texture.Scale = new Vector2(-1f, 1f);
        return false;
    }
}
