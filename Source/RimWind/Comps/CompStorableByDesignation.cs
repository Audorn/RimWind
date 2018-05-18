using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimTES
{
    class CompStorableByDesignation : ThingComp
    {
        public CompProperties_StorableByDesignation Props { get { return (CompProperties_StorableByDesignation)props; } }
        public DesignationDef DesignationDef { get { return Props.designationDef; } }
        public string DefaultLabelKey { get { return Props.defaultLabelKey; } }
        public string DefaultDescriptionKey { get { return Props.defaultDescriptionKey; } }
        public string IconPath { get { return Props.iconPath; } }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref ((CompProperties_StorableByDesignation)props).designationDef, "designationDef");
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultLabelKey, "defaultLabelKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultDescriptionKey, "defaultDescriptionKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).iconPath, "iconPath", "", false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
                yield return c;

            Designation existingDesignation = Find.VisibleMap.designationManager.DesignationOn(parent, DefDatabase<DesignationDef>.GetNamed("HaulToEnchant"));

            if (existingDesignation == null)
            {
                yield return new Command_Action
                {
                    action = delegate
                    {
                        SoundDefOf.TickTiny.PlayOneShotOnCamera(null);
                        Map map = Find.VisibleMap;

                        map.designationManager.RemoveAllDesignationsOn(parent, false);
                        map.designationManager.AddDesignation(new Designation(parent, DesignationDef));
                    },
                    hotKey = KeyBindingDefOf.Misc1,
                    defaultDesc = DefaultDescriptionKey.Translate(),
                    icon = ContentFinder<Texture2D>.Get(IconPath, true),
                    defaultLabel = DefaultLabelKey.Translate()
                };
            }
        }
    }
}
