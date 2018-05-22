using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_AbilityHolder : CompProperties
    {
        public CompProperties_AbilityHolder() { compClass = typeof(CompAbilityHolder); }

        public List<AbilityHolderTypeChance> types = new List<AbilityHolderTypeChance>();

        // ==========
        public string defaultLabelKey = "";
        public string defaultDescriptionKey = "";
        public string iconPath = "";
        // ==========
    }

    public class AbilityHolderTypeChance
    {
        public AbilityCategoryDef abilityCategoryDef;
        public float chance = 1f;
    }
}
