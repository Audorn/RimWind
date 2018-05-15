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
    public abstract class CompEnchantment : ThingComp
    {
        public int EnchantmentValue { get { return ((CompProperties_Enchantment)props).enchantmentValue; } }
    }
}
