// Jeremy Anderson
// HarmonyPatch_GenerateImpliedDefs_PreResolve.cs
// 2018-04-17

using Verse;

namespace RimTES
{
    abstract class Logger
    {
        static public void Message(string text) { Log.Message(text); }
        static public void DebugMessage(string text)
        {
#if DEBUG
                Log.Message(text);
#endif
        }

        static public void Warning(string text) { Log.Warning(text); }
        static public void DebugWarning(string text)
        {
#if DEBUG
                Log.Warning(text);
#endif
        }

        static public void Error(string text) { Log.Error(text); }
        static public void DebugError(string text)
        {
#if DEBUG
                Log.Error(text);
#endif
        }
    }
}
