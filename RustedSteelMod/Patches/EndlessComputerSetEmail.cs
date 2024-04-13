using HarmonyLib;
using UnityEngine;

namespace RustedSteelMod.Patches;

[HarmonyPatch(typeof(computerHost))]
public class EndlessComputerSetEmail
{
    [HarmonyPatch(nameof(computerHost.SendEmail))]
    [HarmonyPostfix]
    public static void SetRightEmail(computerHost __instance)
    {
        if(PlayerPrefs.GetInt("active day") >= 0) return;

        __instance.activeGUI = __instance.messages_day0;
        foreach (var go in __instance.activeGUI)
        {
            go.SetActive(true);
        }
    }
}