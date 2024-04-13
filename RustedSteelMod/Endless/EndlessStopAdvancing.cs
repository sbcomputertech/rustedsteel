using HarmonyLib;
using Serilog;
using UnityEngine.SceneManagement;

namespace RustedSteelMod.Endless;

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
        Log.Debug("Endless scores: {CurrentScore}/{HighScore}", EndlessManager.CurrentScore, EndlessManager.HighScore);
        
        SceneManager.LoadScene("main");
        return false; // skip original
    }
}