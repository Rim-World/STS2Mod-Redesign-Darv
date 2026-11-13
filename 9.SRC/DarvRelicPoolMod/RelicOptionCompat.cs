using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace DarvRelicPoolMod;

/// <summary>
/// 跨 0.107 / 0.111 / 0.113 的 RelicOption 调用垫片。
/// 0.107 / 0.111 存在三参重载（RelicModel, string, string），0.113 删除该重载，只保留两参。
/// 这里按运行时存在的重载反射调用，避免 DLL 绑定到某个版本独有的方法签名。
/// </summary>
public static class RelicOptionCompat
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly MethodInfo? WithDonePage = typeof(AncientEventModel).GetMethod(
        "RelicOption",
        Flags,
        binder: null,
        types: [typeof(RelicModel), typeof(string), typeof(string)],
        modifiers: null);

    private static readonly MethodInfo? WithoutDonePage = typeof(AncientEventModel).GetMethod(
        "RelicOption",
        Flags,
        binder: null,
        types: [typeof(RelicModel), typeof(string)],
        modifiers: null);

    /// <summary>
    /// 创建遗物选项。全部调用点都沿用原版无参调用的默认值：
    /// pageName = "INITIAL"，107/111 的 customDonePage = null；113 的两参重载没有 donePage。
    /// </summary>
    public static EventOption Create(AncientEventModel darv, RelicModel relic)
    {
        MethodInfo target = WithDonePage ?? WithoutDonePage
            ?? throw new MissingMethodException(
                "AncientEventModel.RelicOption(RelicModel, string[, string]) not found.");

        object?[] args = target == WithDonePage
            ? [relic, "INITIAL", null]
            : [relic, "INITIAL"];

        try
        {
            return target.Invoke(darv, args) as EventOption
                ?? throw new InvalidOperationException("AncientEventModel.RelicOption returned null.");
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw; // 上一行必抛；仅为满足编译器。
        }
    }
}
