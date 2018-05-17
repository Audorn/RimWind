using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

namespace RimTES
{
    public class JobDriver_HaulToBuilding : JobDriver
    {
        private const TargetIndex thingInd = TargetIndex.A;
        private const TargetIndex buildingInd = TargetIndex.B;

        protected Thing Thing { get { return job.GetTarget(TargetIndex.A).Thing; } }
        protected Building_ProductionResearchBench_InternalRecipes Building { get { return (Building_ProductionResearchBench_InternalRecipes)job.GetTarget(TargetIndex.B).Thing; } }

        public override string GetReport()
        {
            Thing thing;
            if (pawn.CurJob == job && pawn.carryTracker.CarriedThing != null)
                thing = pawn.carryTracker.CarriedThing;
            else
                thing = TargetThingA;

            if (thing == null || !job.targetB.HasThing)
                return "ReportHaulingUnknown".Translate();

            return "ReportHaulingTo".Translate(new object[]
            {
                thing.LabelCap,
                job.targetB.Thing.LabelShort
            });
        }

        public override bool TryMakePreToilReservations()
        {
            return pawn.Reserve(Thing, job, 1, -1, null) && pawn.Reserve(Building, job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnAggroMentalState(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell)
                .FailOnDestroyedNullOrForbidden(TargetIndex.A)
                .FailOnDespawnedNullOrForbidden(TargetIndex.B)
                .FailOn(() => Building.GetComp<CompInnerContainerItemFilter>().AcceptsHowMany(Thing) <= 0)
                .FailOn(() => !pawn.CanReach(Thing, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                .FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false); // not always 1
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell);
            yield return Toils_General.Wait(100)
                .FailOnCannotTouch(TargetIndex.B, PathEndMode.InteractionCell)
                .WithProgressBarToilDelay(TargetIndex.B, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate
                {
                    Building.TryAcceptThing(Thing, true);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        public override object[] TaleParameters()
        {
            return new object[]
            {
                pawn,
                Thing
            };
        }
    }
}
