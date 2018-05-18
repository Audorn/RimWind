using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimTES
{
    public class CompEnchantable : ThingComp
    {
        public CompProperties_Enchantable Props
        {
            get
            {
                return (CompProperties_Enchantable)props;
            }
        }
        public int EnchantingLimit { get { return Props.enchantingLimit; } }
        public int CumulativeEnchantmentValue { get { return Props.cumulativeEnchantmentValue; } }
        public int CurrentSoulEnergy { get { return Props.currentSoulEnergy; } }
        public bool currentlyEnchanting = false;
        public bool enchanted = false;

        public Rect DoInterface(float x, float y, float width, int index)
        {
            Rect rect = new Rect(x, y, width, 53f);
            Widgets.ThingIcon(new Rect(x, y, 32, 32), parent);
            Widgets.Label(new Rect(x + 32, y, rect.width, rect.height), parent.LabelCap);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            return rect;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ((CompProperties_Enchantable)props).enchantingLimit, "enchantingLimit", 0, false);
            Scribe_Values.Look(ref ((CompProperties_Enchantable)props).cumulativeEnchantmentValue, "enchantingValue", 0, false);
            Scribe_Values.Look(ref ((CompProperties_Enchantable)props).currentSoulEnergy, "currentEnergy", 0, false);
            Scribe_Values.Look(ref currentlyEnchanting, "currentlyEnchanting", false, false);
            Scribe_Values.Look(ref enchanted, "enchanted", false, false);
        }

        public override string CompInspectStringExtra()
        {
            if (CumulativeEnchantmentValue < 1)
                return null;

            string enchantmentInfo = string.Concat(new string[] 
            {
                "CurrentSoulEnergy".Translate(),
                ": ",
                CurrentSoulEnergy.ToString(),
                "/",
                EnchantingLimit.ToString()
            });

            return enchantmentInfo;
        }

        public override string GetDescriptionPart()
        {
            string enchantmentInfo = string.Concat(new string[] 
            {
                "EnchantingLimit".Translate(),
                ": ",
                EnchantingLimit.ToString(),
                "\n",
                "CumulativeEnchantmentValue".Translate(),
                ": ",
                CumulativeEnchantmentValue.ToString(),
                "\n",
                "CurrentSoulEnergy".Translate(),
                ": ",
                CurrentSoulEnergy.ToString()
            });

            return enchantmentInfo;
        }
    }
}
