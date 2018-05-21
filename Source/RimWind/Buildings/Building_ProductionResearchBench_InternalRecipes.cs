using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimTES
{
    public class Building_ProductionResearchBench_InternalRecipes : Building_ResearchBench, IThingHolder, IBillGiver_Enchant, IBillGiverWithTickAction_Enchant
    {
        private CompInnerContainerItemFilter itemFilterComp;
        private CompRefuelable refuelableComp;
        private CompBreakdownable breakdownableComp;
        private CompPowerTrader powerComp;

        protected ThingOwner innerContainer;
        protected bool contentsKnown;
        public bool HasAnyContents { get { return innerContainer.Count > 0; } }
        public ThingOwner GetDirectlyHeldThings() { return innerContainer; }
        public Thing ContainedThing { get { return (innerContainer.Count != 0) ? innerContainer[0] : null; } }
        public void GetChildHolders(List<IThingHolder> outChildren) { ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings()); }

        public BillStack_Enchant billStack;
        public BillStack_Enchant BillStack { get { return billStack; } }
        public IntVec3 BillInteractionCell { get { return InteractionCell; } }

        public Building_ProductionResearchBench_InternalRecipes()
        {
            innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
            billStack = new BillStack_Enchant(this);
        }

        public override void Tick()
        {
            base.Tick();
            innerContainer.ThingOwnerTick(true);
        }
        public override void TickRare()
        {
            base.TickRare();
            innerContainer.ThingOwnerTickRare(true);
        }

        public bool CanWorkWithoutPower
        {
            get
            {
                return powerComp == null || def.building.unpoweredWorkTableWorkSpeedFactor > 0f;
            }
        }
        public virtual bool UsableNow { get { return CanWorkWithoutPower || (powerComp != null && powerComp.PowerOn) || (refuelableComp == null || refuelableComp.HasFuel) && (breakdownableComp == null || !breakdownableComp.BrokenDown); } }
        public bool CurrentlyUsableForBills() { Log.Error("testing"); return UsableNow; }
        public virtual void UsedThisTick()
        {
            if (refuelableComp != null)
                refuelableComp.Notify_UsedThisTick();
        }

        public virtual bool Accepts(Thing thing)
        {
            if (itemFilterComp == null || itemFilterComp.NotSet || itemFilterComp.AcceptsHowMany(thing) > 0)
                return innerContainer.CanAcceptAnyOf(thing, true);

            return false;
        }
        public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (!Accepts(thing))
                return false;

            int quantity = 0;
            if (itemFilterComp == null || itemFilterComp.NotSet)
                quantity = thing.stackCount;
            else
                quantity = Mathf.Min(itemFilterComp.AcceptsHowMany(thing), thing.stackCount);

            int wasAdded;
            if (thing.holdingOwner != null)
                wasAdded = thing.holdingOwner.TryTransferToContainer(thing, innerContainer, quantity, true);
            else
                wasAdded = innerContainer.TryAdd(thing, quantity, true);

            if (wasAdded >= 0)
            {
                if (thing.Faction != null && thing.Faction.IsPlayer)
                    contentsKnown = true;
                return true;
            }

            return false;
        }

        public virtual void RemoveThing(Thing thing)
        {
            innerContainer.TryDrop(thing, ThingPlaceMode.Near, 1, out thing, null);
        }
        public virtual void BeginEnchantingThing(Thing thing)
        {
            int index = innerContainer.IndexOf(thing);
            innerContainer.ElementAt(index).TryGetComp<CompStorableByDesignation>().inUseByBill = true;
        }

        public virtual void EjectContents()
        {
            innerContainer.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
            contentsKnown = true;
        }

        public override bool ClaimableBy(Faction fac)
        {
            if (innerContainer.Any)
            {
                for (int i = 0; i < innerContainer.Count; i++)
                {
                    if (innerContainer[i].Faction == fac)
                        return true;
                }
                return false;
            }
            return base.ClaimableBy(fac);
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string str;
            if (!contentsKnown)
                str = "UnknownLower".Translate();
            else
                str = innerContainer.ContentsString;

            if (!text.NullOrEmpty())
                text += "\n";

            return text + "EnchantingBuildingContains".Translate() + ": " + str;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (itemFilterComp == null)
                    itemFilterComp = GetComp<CompInnerContainerItemFilter>();
            }
            Scribe_Values.Look(ref itemFilterComp, "itemFilterComp", null, false);
            Scribe_Deep.Look(ref billStack, "billStack", new object[] { this });
            Scribe_Deep.Look(ref innerContainer, "innerContainer", new object[] { this });
            Scribe_Values.Look(ref contentsKnown, "contentsKnown", false, false);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            itemFilterComp = GetComp<CompInnerContainerItemFilter>();
            refuelableComp = GetComp<CompRefuelable>();
            breakdownableComp = GetComp<CompBreakdownable>();
            powerComp = GetComp<CompPowerTrader>();
            if (Faction != null && Faction.IsPlayer)
                contentsKnown = true;
        }

        public override void DeSpawn()
        {
            if (HasAnyContents)
                EjectContents();
            base.DeSpawn();
        }
    }
}
