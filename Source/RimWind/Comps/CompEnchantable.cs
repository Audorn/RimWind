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

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ((CompProperties_Enchantable)props).enchantingLimit, "EnchantingLimit", 0, false);
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

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
                yield return c;

            yield return new Command_Action
            {
                action = delegate
                {
                    SoundDefOf.TickTiny.PlayOneShotOnCamera(null);
                },
                hotKey = KeyBindingDefOf.Misc1,
                defaultDesc = "TakeToBeEnchantedDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/TryReconnect", true),
                defaultLabel = "TakeToBeEnchantedLabel".Translate()
            };
        }
    }
}
