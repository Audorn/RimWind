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
    public class CompHarvestableReagent : ThingComp
    {

        private float growth = Rand.Range(0f, 0.8f);
        public CompProperties_HarvestableReagent Props
        {
            get
            {
                return (CompProperties_HarvestableReagent)props;
            }
        }
        public bool HarvestableNow { get { return Growth > HarvestMinGrowth; } }
        public int HarvestYield { get { return Props.harvestYield; } }
        public bool HarvestDestroys { get { return Props.harvestDestroys; } }
        public int YieldNow()
        {
            if (!HarvestableNow)
                return 0;
            if (HarvestYield <= 0f)
                return 0;

            Plant plant = (Plant)parent;
            if (plant != null && plant.Blighted)
                return 0;

            float num = HarvestYield;
            float num2 = Mathf.InverseLerp(HarvestMinGrowth, 1f, Growth);
            num2 = 0.5f + num2 * 0.5f;
            num *= num2;
            num *= Mathf.Lerp(0.5f, 1f, parent.HitPoints / parent.MaxHitPoints);
            num *= Find.Storyteller.difficulty.cropYieldFactor;
            return GenMath.RoundRandom(num);
        }
        public bool IsSecondaryHarvest { get { return Props.isSecondaryHarvest; } }
        public int ReagentGrowDays { get { return Props.growDays; } }
        public float GrowthRate
        {
            get
            {
                Plant plant = (Plant)parent;
                if (plant != null)
                {
                    if (!RequiresLight)
                        return plant.GrowthRate / plant.GrowthRateFactor_Light;
                    else
                        return plant.GrowthRate;
                }

                // Animal.
                return 0f;
            }
        }
        public int ReagentAmount { get { return Props.harvestYield; } }
        public bool HarvestFailable { get { return Props.harvestFailable; } }
        public float HarvestAfterGrowth { get { return Props.harvestAfterGrowth; } }
        public float HarvestMinGrowth { get { return Props.harvestMinGrowth; } }
        public int HarvestWork { get { return Props.harvestWork; } }
        public ThingDef HarvestedThingDef { get { return Props.harvestedThingDef; } }
        public bool RequiresLight { get { return Props.requiresLight; } }
        public bool HasEnoughLightToGrow
        {
            get
            {
                if (!RequiresLight)
                    return true;

                if (GenLocalDate.DayPercent(parent) < 0.25f || GenLocalDate.DayPercent(parent) > 0.8f)
                    return false;

                return true;
            }

        }
        public bool HibernateWithParent { get { return Props.hibernateWithParent; } }
        public string SaveKey { get { return "reagentGrowth"; } }
        public float Growth
        {
            get
            {
                return growth;
            }
            set
            {
                growth = Mathf.Clamp01(value);
                growth = growth > 1f ? 1f : growth;
            }
        }
        public float GrowthPerTick
        {
            get
            {
                Plant plant = (Plant)parent;
                if (HibernateWithParent)
                    if (GenPlant.GrowthSeasonNow(plant.Position, plant.Map))
                        return 0f;

                if (!HasEnoughLightToGrow)
                    return 0f;

                float num = 1f / (60000f * ReagentGrowDays);
                return num * GrowthRate;
            }
        }

        public bool Active
        {
            get
            {
                if (parent != null)
                {
                    return true;
                }

                return false;
            }
        }

        public bool ActiveAndGrown { get { return Active && Growth >= 1f; } }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref growth, SaveKey, 0f, false);
        }

        public override string CompInspectStringExtra()
        {
            if (!Active)
                return null;

            return HarvestedThingDef.label.Translate() + ": " + growth.ToStringPercent();
        }

        public override void CompTick()
        {
            if (!Active)
                return;

            Plant plant = (Plant)parent;
            if (plant != null)
            {
                float num = Growth;
                Growth += (GrowthPerTick * 2000f);
            }
        }

        public void ReagentCollected(Pawn doer)
        {
            if (HarvestDestroys)
                parent.Destroy(DestroyMode.Vanish);
            else
            {
                Growth = HarvestAfterGrowth;
            }
        }
    }
}
