using System.Collections;
using HarmonyLib;
using Serilog;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RustedSteelMod.Patches;

[HarmonyPatch(typeof(menuManager))]
public class UpdatedMainMenu
{
    private static Camera? _mainCam;
    
    private static RectTransform? _rtStart;
    private static RectTransform? _rtContinue;
    private static RectTransform? _rtExit;
    private static RectTransform? _rtEndless;

    private static GameObject? _endlessNav;

    [HarmonyPatch(nameof(menuManager.Start))]
    [HarmonyPostfix]
    public static void InitMenu(menuManager __instance)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
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

            var endlessText = UnityEngine.Object.Instantiate(exit.gameObject, canvas.transform).GetComponent<TextMeshProUGUI>();
            endlessText.name = "endless";
            endlessText.gameObject.SetActive(false);
            var endlessNav = UnityEngine.Object.Instantiate(__instance.navigators.Last(), canvas.transform);
            endlessNav.name = "navigator4";

            var offset = new Vector3(0, -200, 0);
            endlessText.transform.position = exit.position + offset + new Vector3(60, 0, 0);
            endlessNav.transform.position = __instance.navigators.Last().transform.position + offset;
            
            endlessText.SetText("Endless");
            __instance.navigators = __instance.navigators.Append(endlessNav).ToArray();
            _rtEndless = (RectTransform?)endlessText.transform;
            _endlessNav = endlessNav;

            __instance.debug_skipLineUp = true;
        }
        
        Log.Debug("Setup UpdatedMainMenu feature");
    }
    
    [HarmonyPatch(nameof(menuManager.Navigation))]
    [HarmonyPrefix]
    public static bool FixNavigation(menuManager __instance)
    {
        if(_rtEndless != null) _rtEndless.gameObject.SetActive(true);

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
        } else if (RectTransformUtility.RectangleContainsScreenPoint(_rtEndless, mousePos))
        {
            __instance.navIndex = 3;
            hovering = true;
        }
        
        if (hovering)
        {
            __instance.Move();
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl))
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

    [HarmonyPatch(nameof(menuManager.Select))]
    [HarmonyPrefix]
    public static void ResetForLoad(menuManager __instance)
    {
        if(__instance.navIndex == 1 && !__instance.hasSaveFile) return;
        
        Log.Debug("UpdatedMainMenu:ResetForLoad()");
        
        if (_rtEndless != null) _rtEndless.position = new Vector3(1000000, 1000000, 0);
        if(_endlessNav != null) _endlessNav.SetActive(false);
        PlayerPrefs.SetInt("into endless", 0);
    }

    [HarmonyPatch(nameof(menuManager.Select))]
    [HarmonyPostfix]
    public static void CheckForCustomSelects(menuManager __instance)
    {
        if (__instance.navIndex == 3)
        {
            __instance.StartCoroutine(LoadEndlessCO(__instance.tranScript));
        }
    }

    private static IEnumerator LoadEndlessCO(transition t)
    {
        Log.Debug("Entering endless mode!");
            
        PlayerPrefs.SetInt("first time", 1);
        PlayerPrefs.SetInt("active day", -1);
        PlayerPrefs.SetInt("into endless", 1);
        PlayerPrefs.SetInt("died", 0);
        PlayerPrefs.SetFloat("sensitivity", 200f);
        PlayerPrefs.DeleteKey("save file");

        const float fadeTime = 2f;
        t.transitionSpeed = fadeTime;
        t.FadeToBlack();
        yield return new WaitForSeconds(fadeTime + 0.1f);
        
        SceneManager.LoadScene("main");
    }
}