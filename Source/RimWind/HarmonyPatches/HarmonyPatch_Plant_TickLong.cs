using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimTES.HarmonyPatches
{
    [HarmonyPatch(typeof(Plant), "TickLong")]
    public class HarmonyPatch_Plant_TickLong
    {
        public static void Postfix(Plant __instance)
        {
            List<ThingComp> comps = __instance.AllComps;
            if (comps != null)
            {
                int i = 0;
                int count = comps.Count;
                while (i < count)
                {
                    comps[i].CompTick();
                    i++;
                }
            }
        }
    }
}
