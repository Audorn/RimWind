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
    public class CompEnchantment_ConstantEffect : CompEnchantment
    {
        public CompProperties_Enchantment_ConstantEffect Props
        {
            get
            {
                return (CompProperties_Enchantment_ConstantEffect)props;
            }
        }
    }
}
