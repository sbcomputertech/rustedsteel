using HarmonyLib;
using Serilog;
using TMPro;
using UnityEngine;

namespace RustedSteelMod.Patches;

[HarmonyPatch(typeof(guiBranch))]
public class EndlessInfoEmailManager
{
    [HarmonyPatch(nameof(guiBranch.Start))]
    [HarmonyPostfix]
    public static void Init(guiBranch __instance)
    {
        if (__instance.isMail && __instance.mailIndex == 1 && PlayerPrefs.GetInt("into endless") > 0)
        {
            Log.Debug("Found first mail bit");

            var name = __instance.transform.Find("name").GetComponent<TextMeshProUGUI>();
            var subject = __instance.transform.Find("subject").GetComponent<TextMeshProUGUI>();
            
            name.SetText("game");
            subject.SetText("Endless mode ON");

            var email = __instance.c.activeMessages[1];
            var header = email.transform.Find("message data/header").GetComponent<TextMeshProUGUI>();
            var content = email.transform.Find("message data/content").GetComponent<TextMeshProUGUI>();
            
            header.SetText("From <game> to <me>.");
            content.SetText(string.Format("Endless mode is active!\nYou have completed {0} pulls so far.\nYour high score is {1} pulls", EndlessManager.CurrentScore, EndlessManager.HighScore));
        }
    }
}