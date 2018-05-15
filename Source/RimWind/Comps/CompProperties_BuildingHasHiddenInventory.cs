using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_BuildingHasHiddenInventory : CompProperties
    {
        public List<InventoryItemSetting> itemSettings = new List<InventoryItemSetting>();

        public CompProperties_BuildingHasHiddenInventory()
        {
            compClass = typeof(CompBuildingHasHiddenInventory);
        }
    }

    public class InventoryItemSetting
    {
        public string thingDef;
        public int quantity = 1;
    }
}
