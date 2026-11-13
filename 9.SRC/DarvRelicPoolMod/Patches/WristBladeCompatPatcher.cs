using System;
using System.Reflection;
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
/// - 运行时解析 CardPlay 类型，并检查 6 参方法是否真实存在；
/// - 107 的 CardPlay 类型存在，但 ModifyDamageAdditive 仍是 5 参；
/// - 111/113 才是 6 参，因此按「方法签名」而不是「类型是否存在」选择目标。
/// 因此无论本 dll 从 107 还是 111 编译，运行时都能自适应。Postfix 不声明 CardPlay 参数
/// （Harmony 按参数名注入，多余的目标参数不注入），判定逻辑共用 WristBlade.ShouldGrantBonus。
/// </summary>
public sealed class WristBladeCompatPatcher : IPatchMethod
{
    public static string PatchId => "darv_wristblade_compat";
    public static string Description => "Version-adaptive +4 damage for WristBlade (5-arg and 6-arg ModifyDamageAdditive)";
    public static bool IsCritical => true;

    // 直接从已加载的 sts2 程序集解析，避免依赖程序集限定名和加载上下文。
    // 注意：107 也有 CardPlay 类型，但 ModifyDamageAdditive 是 5 参；
    // 因此不能只看类型，必须检查 6 参方法是否真实存在。
    private static readonly Type? CardPlayType =
        typeof(AbstractModel).Assembly.GetType(
            "MegaCrit.Sts2.Core.Entities.Cards.CardPlay",
            throwOnError: false);

    private static readonly MethodInfo? SixArgMethod = CardPlayType is null
        ? null
        : typeof(AbstractModel).GetMethod(
            "ModifyDamageAdditive",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types:
            [
                typeof(Creature),
                typeof(decimal),
                typeof(ValueProp),
                typeof(Creature),
                typeof(CardModel),
                CardPlayType,
            ],
            modifiers: null);

    public static ModPatchTarget[] GetTargets()
    {
        // 只注册当前版本真实存在的那一个签名，避免 optional target ignored 噪音。
        if (SixArgMethod != null && CardPlayType != null)
        {
            return
            [
                PatchTarget.OptionalMethod<AbstractModel>(
                    "ModifyDamageAdditive",
                    typeof(Creature),
                    typeof(decimal),
                    typeof(ValueProp),
                    typeof(Creature),
                    typeof(CardModel),
                    CardPlayType),
            ];
        }

        return
        [
            // 5 参（default 107 的基类虚方法）
            PatchTarget.OptionalMethod<AbstractModel>(
                "ModifyDamageAdditive",
                typeof(Creature),
                typeof(decimal),
                typeof(ValueProp),
                typeof(Creature),
                typeof(CardModel)),
        ];
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
