using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_Enchantable : CompProperties
    {
        public int enchantingLimit = 0;
        public int cumulativeEnchantmentValue = 0;
        public int currentSoulEnergy = 0;

        public CompProperties_Enchantable()
        {
            compClass = typeof(CompEnchantable);
        }
    }
}
