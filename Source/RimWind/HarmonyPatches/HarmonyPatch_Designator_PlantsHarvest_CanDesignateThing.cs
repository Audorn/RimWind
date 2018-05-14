using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;
using Verse;
using RimWorld;

namespace RimTES
{
    [HarmonyPatch(typeof(Designator_PlantsHarvest), "CanDesignateThing", new[] { typeof(Thing) })]
    public class HarmonyPatch_Designator_PlantsHarvest_CanDesignateThing
    {
        public static void Postfix(Designator_PlantsHarvest __instance, ref Thing t, ref AcceptanceReport __result)
        {
            CompHarvestableReagent harvestableReagent = t.TryGetComp<CompHarvestableReagent>();

            if (harvestableReagent != null && !harvestableReagent.IsSecondaryHarvest)
                __result = "PlantOnlyBearsReagents".Translate();
        }

    }
}
