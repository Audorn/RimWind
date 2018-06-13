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
    [HarmonyPatch(typeof(Plant), "GetInspectString")]
    public class HarmonyPatch_Plant_GetInspectString
    {
        public static void Postfix(Plant __instance, ref string __result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            CompHarvestableReagent harvestableReagent = __instance.GetComp<CompHarvestableReagent>();

            if (__instance.LifeStage == PlantLifeStage.Growing)
            {
                if (harvestableReagent != null && !harvestableReagent.IsSecondaryHarvest)
                    stringBuilder.AppendLine("Reagent".Translate());

                stringBuilder.AppendLine("PercentGrowth".Translate(new object[]
                {
                    (__instance.Growth + 0.0001f).ToStringPercent()
                }));
                stringBuilder.AppendLine("GrowthRate".Translate() + ": " + __instance.GrowthRate.ToStringPercent());
                if (!__instance.Blighted)
                {
                    if (GenLocalDate.DayPercent(__instance) < 0.25f || GenLocalDate.DayPercent(__instance) > 0.8f)
                    {
                        stringBuilder.AppendLine("PlantResting".Translate());
                    }
                    if (!(__instance.GrowthRateFactor_Light > 0.001f))
                    {
                        stringBuilder.AppendLine("PlantNeedsLightLevel".Translate() + ": " + __instance.def.plant.growMinGlow.ToStringPercent());
                    }
                    float growthRateFactor_Temperature = __instance.GrowthRateFactor_Temperature;
                    if (growthRateFactor_Temperature < 0.99f)
                    {
                        if (growthRateFactor_Temperature < 0.01f)
                        {
                            stringBuilder.AppendLine("OutOfIdealTemperatureRangeNotGrowing".Translate());
                        }
                        else
                        {
                            stringBuilder.AppendLine("OutOfIdealTemperatureRange".Translate(new object[]
                            {
                                Mathf.RoundToInt(growthRateFactor_Temperature * 100f).ToString()
                            }));
                        }
                    }
                }
            }
            else if (__instance.LifeStage == PlantLifeStage.Mature)
            {
                if (__instance.HarvestableNow)
                {
                    stringBuilder.AppendLine("ReadyToHarvest".Translate());
                }
                else
                {
                    stringBuilder.AppendLine("Mature".Translate());
                }
            }
            if (__instance.DyingBecauseExposedToLight)
            {
                stringBuilder.AppendLine("DyingBecauseExposedToLight".Translate());
            }
            if (__instance.Blighted)
            {
                stringBuilder.AppendLine("Blighted".Translate() + " (" + __instance.Blight.Severity.ToStringPercent() + ")");
            }

            if (harvestableReagent != null && harvestableReagent.IsSecondaryHarvest)
                ComposeReagentString(ref stringBuilder, harvestableReagent);

            __result = stringBuilder.ToString().TrimEndNewlines();
        }

        private static void ComposeReagentString(ref StringBuilder stringBuilder, CompHarvestableReagent reagent)
        {
            stringBuilder.AppendLine("Reagent".Translate() + ": " + reagent.HarvestedThingDef.label);
            stringBuilder.AppendLine("PercentGrowth".Translate(new object[]
                {
                    (reagent.Growth + 0.0001f).ToStringPercent()
                }));
            stringBuilder.AppendLine("GrowthRate".Translate() + ": " + reagent.GrowthRate.ToStringPercent());
        }
    }
}
