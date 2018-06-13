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
                    return filterSetting.capacity - HasHowMany(thing.def);

                if (filterSetting.thingCategoryDef != null)
                {
                    foreach (ThingCategoryDef thingCategoryDef in thing.def.thingCategories)
                    {
                        if (filterSetting.thingCategoryDef == thingCategoryDef)
                            return filterSetting.capacity - HasHowMany(thingCategoryDef);
                    }
                }
            }
            return 0;
        }

        public int HasHowMany(ThingDef thingDef)
        {
            ThingOwner innerContainer = parent.TryGetInnerInteractableThingOwner();
            if (innerContainer == null)
                return 0;

            int quantity = 0;
            foreach (Thing t in innerContainer)
                quantity += (t.def == thingDef) ? t.stackCount : 0;

            return quantity;
        }

        public int HasHowMany(ThingCategoryDef thingCategoryDef)
        {
            ThingOwner innerContainer = parent.TryGetInnerInteractableThingOwner();
            if (innerContainer == null)
                return 0;

            int quantity = 0;
            foreach (Thing t in innerContainer)
                foreach (ThingCategoryDef tCategoryDef in t.def.thingCategories)
                        quantity += (thingCategoryDef == tCategoryDef) ? t.stackCount : 0;

            return quantity;
        }

        public void RecordRemoved(Thing thing, int quantity)
        {

        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref ((CompProperties_InnerContainerItemFilter)props).itemFilterSettings, "itemFilterSettings", LookMode.Deep);
        }

    }
}
