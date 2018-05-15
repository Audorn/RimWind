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
    public class CompEnchantment_OnTarget : CompEnchantment
    {
        public CompProperties_Enchantment_OnTarget Props
        {
            get
            {
                return (CompProperties_Enchantment_OnTarget)props;
            }
        }
        public int SoulCost { get { return Props.soulCost; } }
    }
}
