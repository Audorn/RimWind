using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RimTES
{
    public class CompInnerContainerItemFilter : ThingComp
    {
        private CompProperties_InnerContainerItemFilter Props { get { return (CompProperties_InnerContainerItemFilter)props; } }
        public List<ItemFilterSetting> ItemFilterSettings { get { return Props.itemFilterSettings; } }

        public bool NotSet { get { return (ItemFilterSettings.Count == 0) ? true : false; } }

        public int AcceptsHowMany(Thing thing)
        {
            foreach (ItemFilterSetting filterSetting in ItemFilterSettings)
            {
                if (filterSetting.thingDef != null && filterSetting.thingDef == thing.def)
                    return filterSetting.capacity - filterSetting.stored;

                if (filterSetting.thingCategoryDef != null)
                {
                    foreach (ThingCategoryDef thingCategoryDef in thing.def.thingCategories)
                    {
                        if (filterSetting.thingCategoryDef == thingCategoryDef)
                            return filterSetting.capacity - filterSetting.stored;
                    }
                }
            }
            return 0;
        }
    }
}
