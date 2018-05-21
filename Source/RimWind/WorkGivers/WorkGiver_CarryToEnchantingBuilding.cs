using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

// Error: Only the first building ever built is seen as viable.

namespace RimTES
{
    public class WorkGiver_HaulToEnchantBuilding : WorkGiver_Scanner
    {
        private static string NoEnchantingBuildingsWithSufficientStorageFoundTrans;
        private static string NoEnchantableItemsAreDesignatedForStorageTrans;

        public override PathEndMode PathEndMode { get { return PathEndMode.Touch; } }
        public override Danger MaxPathDanger(Pawn pawn) { return Danger.Deadly; }

        public WorkGiver_HaulToEnchantBuilding()
        {
            if (NoEnchantableItemsAreDesignatedForStorageTrans == null)
                NoEnchantableItemsAreDesignatedForStorageTrans = "NoEnchantableItemsAreDesignatedForStorage".Translate();
            if (NoEnchantingBuildingsWithSufficientStorageFoundTrans == null)
                NoEnchantingBuildingsWithSufficientStorageFoundTrans = "NoEnchantingBuildingsWithSufficientStorageFound".Translate();
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Designation> desList = pawn.Map.designationManager.allDesignations;
            for (int i = 0; i < desList.Count; i++)
            {
                Designation des = desList[i];
                if (des.def == DefDatabase<DesignationDef>.GetNamed("HaulToEnchant"))
                    yield return des.target.Thing;
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, thing, forced)
                || !pawn.CanReserve(thing, 1, -1, null, forced) 
                || thing.IsForbidden(pawn) 
                || thing.IsBurning())
                return null;

            if (pawn.Map.designationManager.DesignationOn(thing, DefDatabase<DesignationDef>.GetNamed("HaulToEnchant")) == null)
                return null;

            foreach (Building building in pawn.Map.listerBuildings.allBuildingsColonist)
            {
                if (!(building is Building_ProductionResearchBench_InternalRecipes))
                    continue;

                CompInnerContainerItemFilter filter = building.GetComp<CompInnerContainerItemFilter>();
                int quantity = -1;
                if (filter == null)
                    continue;

                quantity = filter.AcceptsHowMany(thing);
                if (quantity <= 0)
                    continue;

                if (!pawn.CanReserveAndReach(building, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, false))
                    continue;

                Job job = HaulAIUtility_Helper.HaulToBuildingJob(pawn, thing, building, quantity);
                if (job != null)
                    return job;
            }

            JobFailReason.Is(NoEnchantingBuildingsWithSufficientStorageFoundTrans);
            return null;
        }
    }
}
