// Jeremy Anderson
// Bootstrap.cs
// 2018-05-10

using System.Reflection;
using Harmony;
using Verse;


namespace RimTES.Core
{
    public class Bootstrap : Mod
    {
        public Bootstrap(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("RimTES.RimWind_Complete");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
