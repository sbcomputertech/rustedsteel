using HarmonyLib;
using Serilog;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityExplorer;

namespace RustedSteelMod;

public class RustedSteel
{
    #region Singleton
    private static RustedSteel? _instance;
    public static RustedSteel Instance => _instance ?? throw new InvalidOperationException("No instance has been created");

    public static void SetupMod()
    {
        try
        {
            _instance = new RustedSteel();
        }
        catch (Exception e)
        {
            File.WriteAllText("rsFAIL.txt", "Mod initialization failed!\n" + e);    
        }
        
    }
    #endregion

    private readonly Harmony _harmony;
    private bool _initialPatched;
    
    public RustedSteel()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("modlog.txt")
            //.WriteTo.Sink(new UnityLogSink())
            .Enrich.WithProperty("LoggerName", "RSCore")
            .CreateLogger();
        Log.Logger.Information("Setting up mod...");
        
        _harmony = new Harmony(ModInfo.ID);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Log.Logger.Debug("Added scene load hook");
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(_initialPatched) return;
        _initialPatched = true;
        
        _harmony.PatchAll(typeof(RustedSteel).Assembly);
        Log.Logger.Debug("Finished method patching");

        try
        {
            ExplorerStandalone.CreateInstance();
            Log.Debug("Initialized ExplorerStandalone");
        }
        catch (Exception)
        {
            Log.Information("UnityExplorer standalone is not enabled");
        }
        
        InitManager();
    }

    public void InitManager()
    {
        
    }
}