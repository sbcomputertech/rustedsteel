using HarmonyLib;
using Serilog;
using TMPro;
using UnityEngine;

namespace RustedSteelMod.Features;

[HarmonyPatch(typeof(options))]
public class ActualPause
{
    [HarmonyPatch(nameof(options.Start))]
    [HarmonyPostfix]
    public static void ChangeText(options __instance)
    {
        var text = __instance.parent.transform.Find("not paused").GetComponent<TextMeshProUGUI>();
        text.SetText("The game is PAUSED");
        
        Log.Debug("Setup ActualPause feature");
    }
    
    [HarmonyPatch(nameof(options.OpenMenu))]
    [HarmonyPostfix]
    public static void MakeMenuPause(options __instance)
    {
        Time.timeScale = __instance.viewing ? 0 : 1;
    }
}