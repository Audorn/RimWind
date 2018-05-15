using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public abstract class CompProperties_Enchantment : CompProperties
    {
        public int enchantmentValue = 0;
        
        public CompProperties_Enchantment()
        {
            compClass = typeof(CompEnchantment);
        }
    }
}
