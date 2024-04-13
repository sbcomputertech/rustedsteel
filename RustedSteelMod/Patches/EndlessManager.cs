using System.Collections;
using HarmonyLib;
using Serilog;
using UnityEngine;
using Random = System.Random;

namespace RustedSteelMod.Patches;

[HarmonyPatch(typeof(dayManager))]
public class EndlessManager
{
    public static bool InEndless => PlayerPrefs.GetInt("into endless") > 0;

    public static int HighScore
    {
        get => PlayerPrefs.GetInt("endless hs");
        set => PlayerPrefs.SetInt("endless hs", value);
    }
    public static int CurrentScore { get; set; }
    
    private static Random _rng = new();
    
    [HarmonyPatch(nameof(dayManager.Start))]
    [HarmonyPostfix]
    public static void Setup()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [HarmonyPatch(nameof(dayManager.ChangeLevel))]
    [HarmonyPostfix]
    public static void HandleEndlessRandomisation(dayManager __instance, int day)
    {
        if (day < 0)
        {
            Log.Debug("Randoming monster values for endless");
            RandomiseLifeForm(__instance);
        }
    }

    public static void RandomiseLifeForm(dayManager m)
    {
        var isHostile = _rng.NextBool();
        var isKnown = _rng.NextBool();
        var whichSound = _rng.NextBool();
        
        m.data.SelectLifeform(isKnown, isHostile);
        m.data.sound = whichSound ? m.data.sound_day3 : m.data.sound_day2;
        m.data.sound_struggle = whichSound ? m.data.soundstruggle_day3 : m.data.soundstruggle_day2;
        m.detonator.HideMarkedCasing();
        
        m.data.threatLevel = (4 + (isHostile ? 1 : 0) - (isKnown ? 1 : 0)).ToString();
        m.data.depth[m.data.activeIndex] = "-9999m";
        
        m.equipment.SurfaceAnalyzer(true);
        m.equipment.Welder(true);
        
        m.gl.interactionAllowed_computer = true;
        m.gl.interactionAllowed_analyzer = true;
        m.gl.interactionAllowed_carrierLever = true;
        m.gl.interactionAllowed_casingRack = true;
        m.gl.interactionAllowed_ejectDisk = true;
        m.gl.interactionAllowed_injector = true;
        m.gl.interactionAllowed_instrument = true;
        m.gl.interactionAllowed_welder = true;

        m.sleep.opt.settingsAllowed = true;
        m.sleep.QuietWakeUp();
        
        m.blockout.Exterior(false);
        m.blockout.Interior(true);
        
        Log.Debug("Monster info: Hostile={Hostile}, KnownClass={Known}", isHostile, isKnown);
    }
}