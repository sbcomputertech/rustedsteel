using HarmonyLib;
using Serilog;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace RustedSteelMod.Patches;

[HarmonyPatch(typeof(sleepManager))]
public class QuickWakeUp
{
    public static float BeforeWakeDelay = 2f;
    public static float FadeToBedTime = 1f;
    
    [HarmonyPatch(nameof(sleepManager.GetOutOfBedRoutine), MethodType.Enumerator)]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> ChangeDelay(IEnumerable<CodeInstruction> instructions)
    {
        var ins1 = instructions.First(i => i.Is(OpCodes.Ldc_R4, 2f));
        ins1.opcode = OpCodes.Ldsfld;
        ins1.operand = AccessTools.Field(typeof(QuickWakeUp), nameof(BeforeWakeDelay));

        var ins2 = instructions.First(i => i.Is(OpCodes.Ldc_R4, 1f));
        ins2.opcode = OpCodes.Ldsfld;
        ins2.operand = AccessTools.Field(typeof(QuickWakeUp), nameof(FadeToBedTime));
        
        return instructions;
    }
}