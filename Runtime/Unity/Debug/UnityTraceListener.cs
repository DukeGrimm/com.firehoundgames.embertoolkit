using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmberToolkit.Debugging
{
    /// <summary>
    /// Forwards System.Diagnostics.Debug/Trace output into Unity's Console.
    /// Registered automatically at runtime before the first scene loads.
    /// </summary>
    public class UnityTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            UnityEngine.Debug.Log(message);
        }

        public override void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            UnityEngine.Debug.Log(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            if (!string.IsNullOrEmpty(detailMessage))
                UnityEngine.Debug.LogError($"{message}: {detailMessage}");
            else
                UnityEngine.Debug.LogError(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                case TraceEventType.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                default:
                    UnityEngine.Debug.Log(message);
                    break;
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
        }
    }

    static class UnityTraceListenerInstaller
    {
        // Runs automatically at runtime before any scene loads.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            var listeners = System.Diagnostics.Trace.Listeners;
            if (!listeners.OfType<UnityTraceListener>().Any())
            {
                listeners.Add(new UnityTraceListener());
            }
        }
    }
}
