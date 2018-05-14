using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace RimTES
{
    public class WorkGiver_HarvestReagents : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Designation> desList = pawn.Map.designationManager.allDesignations;
            for (int i = 0; i < desList.Count; i++)
            {
                Designation des = desList[i];
                if (des.def == DefDatabase<DesignationDef>.GetNamed("HarvestReagents"))
                    yield return des.target.Thing;
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            //if (t.def.category != ThingCategory.Plant)
              //  return null;

            LocalTargetInfo target = t;
            if (!pawn.CanReserve(target, 1, -1, null, forced))
                return null;

            if (t.IsForbidden(pawn))
                return null;

            if (t.IsBurning())
                return null;

            foreach (Designation current in pawn.Map.designationManager.AllDesignationsOn(t))
            {
                if (current.def == DefDatabase<DesignationDef>.GetNamed("HarvestReagents"))
                {
                    Job result = null;
                    CompHarvestableReagent harvestableReagent = t.TryGetComp<CompHarvestableReagent>();
                    if (!harvestableReagent.IsSecondaryHarvest)
                    {
                        Plant plant = (Plant)t;
                        if (plant != null && !plant.HarvestableNow)
                            return result;
                        else
                            return new Job(DefDatabase<JobDef>.GetNamed("HarvestReagent"), t);

                        // Animals
                        // Inanimates
                    }

                    if (!harvestableReagent.HarvestableNow)
                        return result;

                    return new Job(DefDatabase<JobDef>.GetNamed("HarvestReagent"), t);
                }
            }
            return null;
        }
    }
}
