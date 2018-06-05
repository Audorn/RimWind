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

        public bool inUseByBill = false;

        private float iconSize = 32f;
        private float enchantableLabelHeightMod = 5f;
        private float soulGemLabelHeightMod = 5f;
        private float soulGemWidth = 50f;

        public Thing DoEnchantableInterface(Rect enchantableRect, Building_ProductionResearchBench_InternalRecipes building)
        {
            GUI.BeginGroup(enchantableRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            Widgets.Label(new Rect(0f, 0f, enchantableRect.width - 48f - 20f, enchantableRect.height + enchantableLabelHeightMod), parent.LabelCap);

            Rect deleteRect = new Rect(enchantableRect.width - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, Color.white))
                building.RemoveThing(parent);

            Rect iconRect = new Rect(0f, 20f, iconSize, iconSize);
            Widgets.ThingIcon(iconRect, parent);

            CompEnchantable enchantableComp = parent.GetComp<CompEnchantable>();
            Rect enchantMaxRect = new Rect(enchantableRect.x + 38f, 29f, 200f, 22f);
            Widgets.Label(enchantMaxRect, "MagicalCapacity".Translate() + ": " + enchantableComp.EnchantingLimit.ToString());

            Rect studyRect = new Rect(enchantableRect.width - 155f, 29f, 50f, 22f);
            if (Widgets.ButtonText(studyRect, "MarkToStudyEnchantment".Translate(), true, true))
                inUseByBill = true;

            Rect enchantRect = new Rect(enchantableRect.width - 100f, 29f, 100f, 22f);
            if (Widgets.ButtonText(enchantRect, "MarkToEnchant".Translate(), true, true, true))
                inUseByBill = true;

            GUI.EndGroup();

            if (Mouse.IsOver(iconRect))
                return parent;

            return null;
        }

        public Thing DoSoulGemInterface(Rect soulGemRect, Building_ProductionResearchBench_InternalRecipes building)
        {
            GUI.BeginGroup(soulGemRect);
            Rect iconRect = new Rect(0f, 4f, iconSize, iconSize);
            Widgets.ThingIcon(iconRect, parent);

            Rect deleteRect = new Rect(soulGemRect.width - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, Color.white))
                building.RemoveThing(parent);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.LowerCenter;
            GUI.color = Color.white;
            Widgets.Label(new Rect(0f, 0f, soulGemWidth, soulGemRect.height + soulGemLabelHeightMod), String.Format("{0:n0}", 80000));

            GUI.EndGroup();

            if (Mouse.IsOver(iconRect))
                return parent;

            return null;
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

        public override void PostExposeData()
        {
            base.PostExposeData();
            /*
            Scribe_Defs.Look(ref ((CompProperties_StorableByDesignation)props).designationDef, "designationDef");
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultLabelKey, "defaultLabelKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultDescriptionKey, "defaultDescriptionKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).iconPath, "iconPath", "", false);
            */
            Scribe_Values.Look(ref inUseByBill, "inUseByBill", false, false);
        }
    }
}
