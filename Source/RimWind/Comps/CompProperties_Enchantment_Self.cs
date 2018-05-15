using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_Enchantment_Self : CompProperties_Enchantment
    {
        public int soulCost = 0;
        
        public CompProperties_Enchantment_Self()
        {
            compClass = typeof(CompEnchantment_Self);
        }
    }
}
