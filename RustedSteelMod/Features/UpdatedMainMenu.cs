using HarmonyLib;
using Serilog;
using TMPro;
using UnityEngine;

namespace RustedSteelMod.Features;

[HarmonyPatch(typeof(menuManager))]
public class UpdatedMainMenu
{
    private static Camera? _mainCam;
    
    private static RectTransform? _rtStart;
    private static RectTransform? _rtContinue;
    private static RectTransform? _rtExit;
    //private static RectTransform? _rtRusted;

    [HarmonyPatch(nameof(menuManager.Start))]
    [HarmonyPostfix]
    public static void InitMenu(menuManager __instance)
    {
        var canvas = GameObject.Find("player/canvas_player").GetComponent<Canvas>();
        if (canvas == null)
        {
            Log.Error("Failed to grab player canvas");
        }
        else
        {
            var title = canvas.transform.Find("title").gameObject.GetComponent<TextMeshProUGUI>();
            title.SetText(ModInfo.Name);

            var author = canvas.transform.Find("author").gameObject.GetComponent<TextMeshProUGUI>();
            author.SetText("a Carbon Steel expansion\nmod by " + ModInfo.Author);

            var start = canvas.transform.Find("start");
            _rtStart = (RectTransform)start;

            var cont = canvas.transform.Find("continue");
            _rtContinue = (RectTransform)cont;

            var exit = canvas.transform.Find("exit");
            _rtExit = (RectTransform)exit;

            __instance.debug_skipLineUp = true;
        }
        
        Log.Debug("Setup UpdatedMainMenu feature");
    }
    
    [HarmonyPatch(nameof(menuManager.Navigation))]
    [HarmonyPrefix]
    public static bool FixNavigation(menuManager __instance)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_mainCam == null)
        {
            _mainCam = Camera.main;
            if (_mainCam == null) return false;
        }

        var mousePos = Input.mousePosition;
        var hovering = false;
        if (RectTransformUtility.RectangleContainsScreenPoint(_rtStart, mousePos))
        {
            __instance.navIndex = 0;
            hovering = true;
        } else if (RectTransformUtility.RectangleContainsScreenPoint(_rtContinue, mousePos))
        {
            __instance.navIndex = 1;
            hovering = true;
        } else if (RectTransformUtility.RectangleContainsScreenPoint(_rtExit, mousePos))
        {
            __instance.navIndex = 2;
            hovering = true;
        }

        if (hovering)
        {
            __instance.Move();
            if (Input.GetMouseButtonDown(0))
            {
                __instance.Select();
            }

            return false;
        }
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (__instance.navIndex > 0)
            {
                __instance.navIndex--;
                __instance.speaker_move.Play();
                __instance.Move();
            }
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (__instance.navIndex < __instance.navigators.Length - 1)
            {
                __instance.navIndex++;
                __instance.speaker_move.Play();
                __instance.Move();
            }
        } else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            __instance.Select();
        }
        
        return false; // skip original
    }
}