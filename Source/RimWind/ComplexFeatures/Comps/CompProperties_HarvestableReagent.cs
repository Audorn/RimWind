using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_HarvestableReagent : CompProperties
    {
        public bool isSecondaryHarvest = false;
        public int growDays = 2;
        public int harvestYield = 1;
        public bool harvestFailable = false;
        public float harvestAfterGrowth = 0f;
        public float harvestMinGrowth = 1f;
        public bool harvestDestroys = false;
        public int harvestWork = 150;
        public ThingDef harvestedThingDef;
        public bool requiresLight = true;
        public bool hibernateWithParent = true;

        public CompProperties_HarvestableReagent()
        {
            compClass = typeof(CompHarvestableReagent);
        }
    }
}
