using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class Designator_HarvestReagent : Designator
    {
        protected DesignationDef designationDef;

        public override int DraggableDimensions { get { return 2; } }

        public Designator_HarvestReagent()
        {
            defaultLabel = "DesignatorHarvestReagent".Translate();
            defaultDesc = "DesignatorHarvestReagentDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/Harvest", true);
            soundDragSustain = SoundDefOf.DesignateDragStandard;
            soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.DesignateHarvest;
            hotKey = KeyBindingDefOf.Misc2;
            designationDef = DefDatabase<DesignationDef>.GetNamed("HarvestReagents");
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {

            if (Map.designationManager.DesignationOn(t, designationDef) != null)
                return false;

            CompHarvestableReagent harvestableReagent = t.TryGetComp<CompHarvestableReagent>();
            //if (harvestableReagent == null)
            //    return "NoHarvestableReagentPresent".Translate();

            if (!harvestableReagent.IsSecondaryHarvest)
            {
                Plant plant = (Plant)t;
                if (plant != null && !plant.HarvestableNow)
                        return "NoHarvestableReagentPresent".Translate();
                // Animals
                // Inanimates

            }

            if (harvestableReagent.IsSecondaryHarvest && !harvestableReagent.HarvestableNow)
                return "NoHarvestableReagentPresent".Translate();

            return true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(Map) || c.Fogged(Map))
                return false;

            List<Thing> things = c.GetThingList(Map);
            if (things.Count < 1)
                return false;

            foreach (Thing thing in things)
            {
                if (thing.TryGetComp<CompHarvestableReagent>() == null)
                    return "NoHarvestableReagentPresent".Translate();
                else
                {
                    AcceptanceReport result = CanDesignateThing(thing);
                    if (!result.Accepted)
                        return result;
                }
            }
            return true;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Plant plant = c.GetPlant(Map);
            AcceptanceReport result = CanDesignateThing(plant);
            if (plant != null && result.Accepted)
                DesignateThing(plant);
        }

        public override void DesignateThing(Thing t)
        {
            Map.designationManager.RemoveAllDesignationsOn(t, false);
            Map.designationManager.AddDesignation(new Designation(t, designationDef));
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }
    }
}
