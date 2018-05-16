using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RimTES
{
    public class CompProperties_InnerContainerItemFilter : CompProperties
    {
        // See CompProperties_Flickable for string preparation for UI buttons.

        public List<ItemFilterSetting> itemFilterSettings = new List<ItemFilterSetting>();

        public CompProperties_InnerContainerItemFilter() { compClass = typeof(CompInnerContainerItemFilter); }
    }

    public class ItemFilterSetting : IExposable
    {
        public ThingDef thingDef = null;
        public ThingCategoryDef thingCategoryDef = null;
        public int capacity = 1;
        public int stored = 0;

        public void ExposeData()
        {
            Scribe_Values.Look(ref thingDef, "thingDef", null, false);
            Scribe_Values.Look(ref thingCategoryDef, "thingCategoryDef", null, false);
            Scribe_Values.Look(ref capacity, "capacity", 1, false);
            Scribe_Values.Look(ref stored, "stored", 0, false);
        }
    }
}
