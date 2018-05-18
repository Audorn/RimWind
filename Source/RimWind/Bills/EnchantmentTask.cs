using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class EnchantmentTask : IExposable
    {
        [DefaultValue(null)]
        public string label;
        public string LabelCap { get { return label.NullOrEmpty() ? null : label.CapitalizeFirst(); } }
        public override string ToString() { return label.NullOrEmpty() ? "noLabelError" : LabelCap; }

        [DefaultValue(null)]
        public string description;

        public Type workerClass = typeof(EnchantWorker);
        public WorkTypeDef requiredGiverWorkType;
        public float workAmount = -1f;
        public float WorkAmountTotal()
        {
            if (workAmount >= 0f)
                return workAmount;

            return GetStatValueAbstract(StatDefOf.WorkToMake);
        }
        public float GetStatValueAbstract(StatDef stat)
        {
            // Derive the amount from the enchantments and the stat.
            return workAmount;
        }

        public Thing enchantableThing;
        public Thing soulGem;
        public List<Thing> products;

        public List<Enchantment> enchantments;
        public List<SkillRequirement> skillRequirements;
        public string MinSkillString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                bool flag = false;
                if (skillRequirements != null)
                {
                    foreach (SkillRequirement skillRequirement in skillRequirements)
                    {
                        stringBuilder.AppendLine(string.Concat(new object[]
                        {
                            "   ",
                            skillRequirement.skill.skillLabel.CapitalizeFirst(),
                            ": ",
                            skillRequirement.minLevel
                        }));
                        flag = true;
                    }
                }
                if (!flag)
                    stringBuilder.AppendLine("   (" + "NoneLower".Translate() + ")");

                return stringBuilder.ToString();
            }
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref label, "label", null, false);
            Scribe_Values.Look(ref description, "description", null, false);
            Scribe_Deep.Look(ref workerClass, "workerClass", typeof(EnchantWorker), false);
            Scribe_Defs.Look(ref requiredGiverWorkType, "requiredGiverWorkType");
            Scribe_Values.Look(ref workAmount, "workAmount", -1f, false);
            Scribe_Deep.Look(ref enchantableThing, "enchantableThing", null, false);
            Scribe_Deep.Look(ref soulGem, "soulGem", null, false);
            Scribe_Collections.Look(ref enchantments, "enchantments", LookMode.Deep, false);
            Scribe_Collections.Look(ref products, "products", LookMode.Deep, false);
        }
    }
}
