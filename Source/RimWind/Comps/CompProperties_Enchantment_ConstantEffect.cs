using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_Enchantment_ConstantEffect : CompProperties_Enchantment
    {
        public CompProperties_Enchantment_ConstantEffect()
        {
            compClass = typeof(CompEnchantment_ConstantEffect);
        }
    }
}
