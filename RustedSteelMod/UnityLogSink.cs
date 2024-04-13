using Serilog.Core;
using Serilog.Events;
using UnityEngine;

namespace RustedSteelMod;

public class UnityLogSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        switch (logEvent.Level)
        {
            case LogEventLevel.Verbose:
            case LogEventLevel.Debug:
            case LogEventLevel.Information:
                Debug.Log(logEvent.RenderMessage());
                break;
            case LogEventLevel.Warning:
                Debug.LogWarning(logEvent.RenderMessage());
                break;
            case LogEventLevel.Error:
            case LogEventLevel.Fatal:
                Debug.LogError(logEvent.RenderMessage());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}