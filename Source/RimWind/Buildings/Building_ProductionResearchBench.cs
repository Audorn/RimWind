using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RimTES
{ 
    public class Building_ProductionResearchBench : Building_ResearchBench, IBillGiver, IBillGiverWithTickAction
    {
        public BillStack billStack;

        private CompPowerTrader powerComp;

        private CompRefuelable refuelableComp;

        private CompBreakdownable breakdownableComp;

        public bool CanWorkWithoutPower
        {
            get
            {
                return powerComp == null || def.building.unpoweredWorkTableWorkSpeedFactor > 0f;
            }
        }

        public virtual bool UsableNow
        {
            get
            {
                return (CanWorkWithoutPower || (powerComp != null && powerComp.PowerOn)) && (refuelableComp == null || refuelableComp.HasFuel) && (breakdownableComp == null || !breakdownableComp.BrokenDown);
            }
        }

        public BillStack BillStack
        {
            get
            {
                return billStack;
            }
        }

        public IntVec3 BillInteractionCell
        {
            get
            {
                return InteractionCell;
            }
        }

        public IEnumerable<IntVec3> IngredientStackCells
        {
            get
            {
                return GenAdj.CellsOccupiedBy(this);
            }
        }

        public Building_ProductionResearchBench()
        {
            billStack = new BillStack(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref billStack, "billStack", new object[]
            {
                this
            });
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            refuelableComp = GetComp<CompRefuelable>();
            breakdownableComp = GetComp<CompBreakdownable>();
        }

        public virtual void UsedThisTick()
        {
            if (refuelableComp != null)
            {
                refuelableComp.Notify_UsedThisTick();
            }
        }

        public bool CurrentlyUsableForBills()
        {
            return UsableNow;
        }
    }
}
