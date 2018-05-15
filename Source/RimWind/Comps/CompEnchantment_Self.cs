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
    public class CompEnchantment_Self : CompEnchantment
    {
        public CompProperties_Enchantment_Self Props
        {
            get
            {
                return (CompProperties_Enchantment_Self)props;
            }
        }
        public int SoulCost { get { return Props.soulCost; } }
    }
}
