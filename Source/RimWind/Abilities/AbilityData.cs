using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class AbilityData : IExposable
    {
        public AbilityDef def;
        public string label = "custom";





        public AbilityData() { }
        public AbilityData(AbilityDef abilityDef) { def = abilityDef; }

        public virtual void PostMake() { }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref label, "label", "custom", false);
        }
    }
}
