using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompBuildingHasHiddenInventory : ThingComp
    {
        private int maxTotalItems;
        private int currentTotalItems = 0;
        private List<ThingDef> items = new List<ThingDef>();
        private List<int> itemsOfThingDef = new List<int>();

        public CompProperties_BuildingHasHiddenInventory Props
        {
            get
            {
                return (CompProperties_BuildingHasHiddenInventory)props;
            }
        }

        public List<InventoryItemSetting> ItemSettings { get { return Props.itemSettings; } }
        public List<ThingDef> Items { get { return items; } }

        public CompBuildingHasHiddenInventory()
        {
            maxTotalItems = 0;
            foreach(InventoryItemSetting itemSetting in ItemSettings)
            {
                maxTotalItems += itemSetting.quantity;
                itemsOfThingDef.Add(0);
            }
        }

        public void AddItem(ThingDef thingDef)
        {
            if (currentTotalItems >= maxTotalItems)
                return;

            bool roomFound = false;
            int index = 0;
            for (int i = 0; i < ItemSettings.Count; i++)
            {
                if (ItemSettings[i].thingDef == thingDef.defName && itemsOfThingDef[i] < ItemSettings[i].quantity)
                {
                    index = i;
                    roomFound = true;
                    break;
                }
                    
            }

            if (roomFound)
            {
                items.Add(thingDef);
                itemsOfThingDef[index]++;
                currentTotalItems++;
            }
        }

        public void RemoveItem(ThingDef thingDef)
        {
            if (!Items.Contains(thingDef))
                return;

            for (int i = 0; i < ItemSettings.Count; i++)
            {
                if (ItemSettings[i].thingDef == thingDef.defName)
                {
                    items.Remove(thingDef);
                    itemsOfThingDef[i]--;
                    currentTotalItems--;
                    break;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            foreach (ThingDef item in items)
            {
                ThingDef i = item;
                Scribe_Values.Look(ref i, "Item", null, false);
            }
        }

    }
}
