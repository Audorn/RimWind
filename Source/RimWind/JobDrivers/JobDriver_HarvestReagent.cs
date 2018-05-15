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
    public class JobDriver_HarvestReagent : JobDriver
    {
        private float workDone;
        protected float xpPerTick;
        protected const TargetIndex PlantInd = TargetIndex.A;

        protected Thing Thing { get { return job.targetA.Thing; } }
        public override bool TryMakePreToilReservations()
        {
            LocalTargetInfo target = job.GetTarget(TargetIndex.A);
            if (target.IsValid && !pawn.Reserve(target, job, 1, -1, null))
                return false;
            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.A), job, 1, -1, null);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Init();
            yield return Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);
            Toil initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A);
            yield return initExtractTargetFromQueue;
            yield return Toils_JobTransforms.SucceedOnNoTargetInQueue(TargetIndex.A);
            yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A, true);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, initExtractTargetFromQueue);
            Toil cut = new Toil();
            Plant plant = (Plant)Thing;
            CompHarvestableReagent reagent = Thing.TryGetComp<CompHarvestableReagent>();
            cut.tickAction = delegate
            {
                Pawn actor = cut.actor;
                if (actor.skills != null)
                    actor.skills.Learn(DefDatabase<SkillDef>.GetNamed("Alchemy", true), xpPerTick, false);
                float statValue = actor.GetStatValue(DefDatabase<StatDef>.GetNamed("ReagentHarvestingSpeed"), true);
                float num = statValue;
                workDone += num;
                if (!reagent.IsSecondaryHarvest)
                {
                    if (plant != null)
                    {
                        if (workDone >= plant.def.plant.harvestWork)
                        {
                            if (plant.def.plant.harvestedThingDef != null)
                            {
                                if (actor.RaceProps.Humanlike && plant.def.plant.harvestFailable && Rand.Value > actor.GetStatValue(DefDatabase<StatDef>.GetNamed("ReagentHarvestingYield"), true))
                                {
                                    Vector3 loc = (pawn.DrawPos + plant.DrawPos) / 2f;
                                    MoteMaker.ThrowText(loc, Map, "TextMote_ReagentHarvestFailed".Translate(), 3.65f);
                                }
                                else
                                {
                                    int num2 = plant.YieldNow();
                                    if (num2 > 0)
                                    {
                                        Thing harvestedThing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef, null);
                                        harvestedThing.stackCount = num2;
                                        if (actor.Faction != Faction.OfPlayer)
                                            harvestedThing.SetForbidden(true, true);
                                        GenPlace.TryPlaceThing(harvestedThing, actor.Position, Map, ThingPlaceMode.Near, null);
                                        actor.records.Increment(DefDatabase<RecordDef>.GetNamed("ReagentsHarvested"));
                                    }
                                }
                            }
                            plant.def.plant.soundHarvestFinish.PlayOneShot(actor);
                            plant.PlantCollected();
                            workDone = 0f;
                            ReadyForNextToil();
                            return;
                        }

                    }
                    // Animal
                    // Inanimate
                }
                else
                {
                    if (workDone >= reagent.HarvestWork)
                    {
                        if (reagent.HarvestedThingDef != null)
                        {
                            if (actor.RaceProps.Humanlike && reagent.HarvestFailable && Rand.Value > actor.GetStatValue(DefDatabase<StatDef>.GetNamed("ReagentHarvestingYield"), true))
                            {
                                Vector3 loc = (pawn.DrawPos + reagent.parent.DrawPos) / 2f;
                                MoteMaker.ThrowText(loc, Map, "TextMote_ReagentHarvestFailed".Translate(), 3.65f);
                            }
                            else
                            {
                                int num2 = reagent.YieldNow();
                                if (num2 > 0)
                                {
                                    Thing harvestedThing = ThingMaker.MakeThing(reagent.HarvestedThingDef, null);
                                    harvestedThing.stackCount = num2;
                                    if (actor.Faction != Faction.OfPlayer)
                                        harvestedThing.SetForbidden(true, true);
                                    GenPlace.TryPlaceThing(harvestedThing, actor.Position, Map, ThingPlaceMode.Near, null);
                                    actor.records.Increment(DefDatabase<RecordDef>.GetNamed("ReagentsHarvested"));
                                }
                            }
                        }
                        if (plant != null)
                            plant.def.plant.soundHarvestFinish.PlayOneShot(actor);

                        reagent.ReagentCollected(actor);
                        workDone = 0f;
                        ReadyForNextToil();
                        return;
                    }
                }
            };
            cut.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            cut.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            cut.defaultCompleteMode = ToilCompleteMode.Never;
            cut.WithEffect(EffecterDefOf.Harvest, TargetIndex.A);
            if (!reagent.IsSecondaryHarvest)
            {
                if (plant != null)
                {
                    cut.WithProgressBar(TargetIndex.A, () => workDone / plant.def.plant.harvestWork, true, -0.5f);
                    cut.PlaySustainerOrSound(() => plant.def.plant.soundHarvesting);
                }

                // Animals
            }
            else
            {
                cut.WithProgressBar(TargetIndex.A, () => workDone / reagent.HarvestWork, true, -0.5f);
                if (plant != null)
                    cut.PlaySustainerOrSound(() => plant.def.plant.soundHarvesting);
            }

            yield return cut;
            Toil workDoneToil = WorkDoneToil();
            if (workDoneToil != null)
                yield return workDoneToil;
            yield return Toils_Jump.Jump(initExtractTargetFromQueue);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workDone, "workDone", 0f, false);
        }

        protected void Init()
        {
            xpPerTick = 0.11f;
        }

        protected Toil WorkDoneToil()
        {
            return Toils_General.RemoveDesignationsOnThing(TargetIndex.A, DefDatabase<DesignationDef>.GetNamed("HarvestReagents", true));
        }
    }
}
