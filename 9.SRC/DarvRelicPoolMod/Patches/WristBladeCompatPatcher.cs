using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;
using DarvRelicPoolMod.Relics;

namespace DarvRelicPoolMod.Patches;

/// <summary>
/// 袖剑伤害双版本兼容（关键：不依赖编译期版本）。
/// 背景：AbstractModel.ModifyDamageAdditive 两版签名不同——default 107 为 5 参（无 CardPlay），
/// beta 111 为 6 参。若用 override 写死某一版签名，另一版本运行时虚分派不到（静默失效），
/// 且本 mod 直接引用游戏目录 sts2.dll 编译（切分支后本地 dll 即换版），override 无法跨版本编译。
/// 方案：全部走 Harmony optional patch，目标按"方法名 + 参数类型数组"在**运行时**反射解析：
/// - 5 参 target：107 存在 → 命中；111 不存在 → ignoreIfMissing 忽略
/// - 6 参 target：运行时探测 CardPlay 类型（Type.GetType，避免编译期依赖）——107 无此类型 → 不注册；
///   111 有 → 注册并命中
/// 因此无论本 dll 从 107 还是 111 编译，运行时都能自适应。Postfix 不声明 CardPlay 参数
/// （Harmony 按参数名注入，多余的目标参数不注入），判定逻辑共用 WristBlade.ShouldGrantBonus。
/// </summary>
public sealed class WristBladeCompatPatcher : IPatchMethod
{
    public static string PatchId => "darv_wristblade_compat";
    public static string Description => "Version-adaptive +4 damage for WristBlade (5-arg and 6-arg ModifyDamageAdditive)";
    public static bool IsCritical => true;

    private static readonly Type? CardPlayType =
        Type.GetType("MegaCrit.Sts2.Core.Entities.Cards.CardPlay, sts2");

    public static ModPatchTarget[] GetTargets()
    {
        var targets = new List<ModPatchTarget>
        {
            // 5 参（default 107 的基类虚方法）
            PatchTarget.OptionalMethod<AbstractModel>(
                "ModifyDamageAdditive",
                typeof(Creature),
                typeof(decimal),
                typeof(ValueProp),
                typeof(Creature),
                typeof(CardModel)),
        };

        // 6 参（beta 111 的基类虚方法）；107 无 CardPlay 类型 → 不注册
        if (CardPlayType != null)
        {
            targets.Add(PatchTarget.OptionalMethod<AbstractModel>(
                "ModifyDamageAdditive",
                typeof(Creature),
                typeof(decimal),
                typeof(ValueProp),
                typeof(Creature),
                typeof(CardModel),
                CardPlayType));
        }

        return [.. targets];
    }

    public static void Postfix(AbstractModel __instance, ref decimal __result, Creature? dealer, ValueProp props, CardModel? cardSource)
    {
        try
        {
            if (__instance is WristBlade wb && WristBlade.ShouldGrantBonus(wb, dealer, props, cardSource))
            {
                __result += 4m;
            }
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"{ModEntry.ModId}: wrist blade compat patch failed: {e}");
        }
    }
}
