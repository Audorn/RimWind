using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace RimTES
{
    public static class HaulAIUtility_Helper
    {
        public static Job HaulToBuildingJob(Pawn p, Thing t, Building b, int quantity)
        {
            float statValue = p.GetStatValue(StatDefOf.CarryingCapacity, true);

            Job job = new Job(DefDatabase<JobDef>.GetNamed("HaulToBuilding"), t, b);
            job.count = (int)Mathf.Min(quantity, statValue);
            job.count = job.count < 1 ? 1 : job.count;
            job.haulOpportunisticDuplicates = true;
            job.haulMode = HaulMode.ToContainer;
            return job;
        }

    }
}
