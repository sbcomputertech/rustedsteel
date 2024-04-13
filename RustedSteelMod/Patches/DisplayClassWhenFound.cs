using HarmonyLib;
using Serilog;

namespace RustedSteelMod.Patches;

[HarmonyPatch]
public class DisplayClassWhenFound
{
    private static bool updateClass;
    private static string mainClass = "N/A";
    private static string subClass = "N/A";
    
    [HarmonyPatch(typeof(layerHost))]
    [HarmonyPatch(nameof(layerHost.NarrowResults))]
    [HarmonyPostfix]
    public static void GetClassIfFound(layerHost __instance)
    {
        if(__instance.numberOfLayersDone < 4) return;
        Log.Debug("All layers have been done on surface annalyser");
        var laser = __instance.manager.laser;
        mainClass = laser.mainclass;
        subClass = laser.subclass;
        Log.Debug("Class is {Class}/{SubClass}", laser.mainclass, laser.subclass);
        updateClass = true;
    }

    [HarmonyPatch(typeof(carrierLeverBranch))]
    [HarmonyPatch(nameof(carrierLeverBranch.Update))]
    [HarmonyPostfix]
    public static void UpdateConsole(carrierLeverBranch __instance)
    {
        if (updateClass)
        {
            updateClass = false;
            __instance.console_lines[2].text = "CLASS........ " + mainClass;
            __instance.console_lines[3].text = "SUB-CLASS.... " + subClass;
        }
    }
}