using HarmonyLib;
using UnityEngine.SceneManagement;

namespace RustedSteelMod.Patches;

[HarmonyPatch(typeof(sleepManager))]
public class EndlessStopAdvancing
{
    [HarmonyPatch(nameof(sleepManager.LoadNextDay))]
    [HarmonyPrefix]
    public static bool SkipIfEndless(sleepManager __instance)
    {
        if (__instance.day.activeDay >= 0) return true;
        
        EndlessManager.CurrentScore++;
        if (EndlessManager.CurrentScore > EndlessManager.HighScore)
        {
            EndlessManager.HighScore = EndlessManager.CurrentScore;
        }
        
        SceneManager.LoadScene("main");
        return false; // skip original
    }
}