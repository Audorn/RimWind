using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RimTES
{
    class PawnCapacityWorker_Arcane : PawnCapacityWorker
    {
        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            string tag = "ArcaneSource";
            return PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, 3.40282347E+38f, impactors);
        }

        public override bool CanHaveCapacity(BodyDef body)
        {
            return body.HasPartWithTag("ArcaneSource");
        }
    }
}
