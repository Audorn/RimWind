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
    public class WorkGiver_CarryToEnchantBuilding : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode { get { return PathEndMode.Touch; } }
        public override Danger MaxPathDanger(Pawn pawn) { return Danger.Deadly; }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Designation> desList = pawn.Map.designationManager.allDesignations;
            for (int i = 0; i < desList.Count; i++)
            {
                Designation des = desList[i];
                if (des.def == DefDatabase<DesignationDef>.GetNamed("TakeToEnchant"))
                    yield return des.target.Thing;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.Map.designationManager.DesignationOn(t, DefDatabase<DesignationDef>.GetNamed("TakeToEnchant")) == null)
                return false;

            List<Building> buildings = pawn.Map.listerBuildings.allBuildingsColonist;
            foreach (Building building in buildings)
            {
                if (building.GetComp<CompInnerContainerItemFilter>() != null)
                {
                    Building_ProductionResearchBench_InternalRecipes b = (Building_ProductionResearchBench_InternalRecipes)building;
                    CompInnerContainerItemFilter filter = b.GetComp<CompInnerContainerItemFilter>();
                    if (filter != null)
                    {
                        int quantity = filter.AcceptsHowMany(t);
                        if (quantity <= 0)
                            break;

                        LocalTargetInfo target = t;
                        return pawn.CanReserve(target, 1, -1, null, forced);
                    }
                }
            }

            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
                return null;

            List<Building> buildings = pawn.Map.listerBuildings.allBuildingsColonist;
            foreach (Building building in buildings)
            {
                if (building.GetComp<CompInnerContainerItemFilter>() != null)
                {
                    Building_ProductionResearchBench_InternalRecipes b = (Building_ProductionResearchBench_InternalRecipes)building;
                    CompInnerContainerItemFilter filter = b.GetComp<CompInnerContainerItemFilter>();
                    if (filter != null)
                    {
                        int quantity = filter.AcceptsHowMany(t);
                        if (quantity <= 0)
                            break;

                        Job job = new Job(DefDatabase<JobDef>.GetNamed("CarryToEnchantingBuilding"), t, b);
                        job.count = quantity;
                        job.haulOpportunisticDuplicates = true;
                        job.haulMode = HaulMode.ToContainer;
                        return job;
                    }
                }
            }

            JobFailReason.Is("NoEnchantingBuildingsWithSufficientStorageFound".Translate());
            return null;
        }
    }
}
