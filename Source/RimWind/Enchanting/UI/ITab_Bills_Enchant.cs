using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimTES
{ 
    public class ITab_Bills_Enchant : ITab
    {

        private Vector2 enchantablesScrollPosition = default(Vector2);
        private Vector2 soulGemsScrollPosition = default(Vector2);

        private Bill_Enchant mouseoverBill;
        private Thing mouseHoveredThing;

        private static ThingCategoryDef EnchantableCategoryDef = DefDatabase<ThingCategoryDef>.GetNamed("Enchantables");
        private static ThingCategoryDef SoulGemCategoryDef = DefDatabase<ThingCategoryDef>.GetNamed("SoulGems");

        private static readonly Vector2 WinSize = new Vector2(420f, 480f);
        private static float EnchantableThingHeight = 53f;
        private static float EnchantableVerticalGap = 6f;
        private static float SoulGemHeight = 40f;
        private static float SoulGemVerticalGap = 6f;
        private static float SoulGemWidth = 56f;

        protected Building_ProductionResearchBench_InternalRecipes SelTable
        { get { return (Building_ProductionResearchBench_InternalRecipes)SelThing; } }

        public ITab_Bills_Enchant()
        {
            size = WinSize;
            labelKey = "TabBillsEnchant";
            tutorTag = "Bills"; // Tutor Enchant?
        }

        private void DoEnchantables(ThingOwner thingOwner, Rect baseRect, List<Thing> enchantables)
        {
            float y = 0f;
            for (int i = 0; i < enchantables.Count; i++)
            {
                CompStorableByDesignation storableComp = enchantables[i].TryGetComp<CompStorableByDesignation>();
                Rect enchantableThingRect = new Rect(baseRect.x, y, baseRect.width, EnchantableThingHeight);
                Thing hoveredEnchantable = storableComp.DoEnchantableInterface(enchantableThingRect, SelTable);

                if (hoveredEnchantable != null)
                    mouseHoveredThing = hoveredEnchantable;

                y += EnchantableThingHeight + EnchantableVerticalGap;
            }
        }

        private void DoSoulGems(ThingOwner thingOwner, Rect baseRect, List<Thing> soulGems)
        {
            WidgetRow soulGemsRow = new WidgetRow(
                baseRect.x,
                baseRect.y,
                UIDirection.RightThenDown);

            float x = baseRect.x;
            float y = 0f;
            int maxPerRow = (int)(baseRect.width / SoulGemWidth) - 1;
            for (int i = 0; i < soulGems.Count; i++)
            {
                CompStorableByDesignation storableComp = soulGems[i].TryGetComp<CompStorableByDesignation>();
                Rect soulGemRect = new Rect(x * (SoulGemWidth + SoulGemVerticalGap), y, SoulGemWidth, SoulGemHeight);
                Thing hoveredSoulGem = storableComp.DoSoulGemInterface(soulGemRect, SelTable);

                if (hoveredSoulGem != null)
                    mouseHoveredThing = hoveredSoulGem;

                if (x != 0 && x % maxPerRow == 0)
                {
                    x = 0;
                    y += SoulGemHeight + SoulGemVerticalGap;
                }
                else
                    x++;
            }
        }

        protected override void FillTab()
        {
            ThingOwner thingOwner = SelTable.GetDirectlyHeldThings();
            CompInnerContainerItemFilter itemFilterComp = SelTable.GetComp<CompInnerContainerItemFilter>();

            Rect baseRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            GUI.BeginGroup(baseRect); // Entire Window

            // DoEnchantBills();

            // Enchantables
            List<Thing> enchantables = new List<Thing>();
            foreach (Thing thing in thingOwner)
            {
                foreach (ThingCategoryDef thingCategoryDef in thing.def.thingCategories)
                {
                    CompStorableByDesignation storableComp = thing.TryGetComp<CompStorableByDesignation>();
                    if (thingCategoryDef == EnchantableCategoryDef && !storableComp.inUseByBill)
                        enchantables.Add(thing);
                }
            }

            Rect enchantablesOuterRect = new Rect(0f, 220f, baseRect.width, 116f);
            Rect enchantablesViewRect = new Rect(
                    enchantablesOuterRect.x,
                    0f,
                    enchantablesOuterRect.width - 16f,
                    enchantables.NullOrEmpty() ? enchantablesOuterRect.height : enchantables.Count * EnchantableThingHeight + (enchantables.Count - 1) * EnchantableVerticalGap);

            Rect enchantablesLabelRect = new Rect(enchantablesOuterRect.x, enchantablesOuterRect.y - 30f, enchantablesOuterRect.width, 30f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.yellow;
            Widgets.Label(enchantablesLabelRect, "EnchantableItemsLabel".Translate());
            GUI.color = Color.gray;

            Widgets.BeginScrollView(enchantablesOuterRect, ref enchantablesScrollPosition, enchantablesViewRect, true);
            GUI.BeginGroup(enchantablesViewRect);
            if (enchantables.NullOrEmpty())
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperCenter;
                GUI.color = Color.white;
                Widgets.Label(enchantablesViewRect, "EnchantableInstructionsTrans".Translate());
            }
            else
                DoEnchantables(thingOwner, enchantablesViewRect, enchantables);

            GUI.EndGroup(); // enchantablesViewRect
            Widgets.EndScrollView();

            // Soul Gems
            List<Thing> soulGems = new List<Thing>();
            foreach (Thing thing in thingOwner)
            {
                foreach (ThingCategoryDef thingCategoryDef in thing.def.thingCategories)
                {
                    CompStorableByDesignation storableComp = thing.TryGetComp<CompStorableByDesignation>();
                    if (thingCategoryDef == SoulGemCategoryDef && !storableComp.inUseByBill)
                        soulGems.Add(thing);
                }
            }

            int maxPerRow = (int)(baseRect.width / SoulGemWidth) - 1;
            Rect soulGemsOuterRect = new Rect(0f, 370f, baseRect.width, 94f);
            Rect soulGemsViewRect = new Rect(
                soulGemsOuterRect.x,
                0f,
                soulGemsOuterRect.width - 16f,
                soulGems.NullOrEmpty() ? soulGemsOuterRect.height : (soulGems.Count / maxPerRow) * SoulGemHeight + (soulGems.Count - 1) * SoulGemVerticalGap);

            Rect soulGemsLabelRect = new Rect(soulGemsOuterRect.x, soulGemsOuterRect.y - 30f, soulGemsOuterRect.width, 30f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.yellow;
            Widgets.Label(soulGemsLabelRect, "SoulGemsLabel".Translate());
            GUI.color = Color.gray;

            Widgets.BeginScrollView(soulGemsOuterRect, ref soulGemsScrollPosition, soulGemsViewRect, true);
            GUI.BeginGroup(soulGemsViewRect);
            if (soulGems.NullOrEmpty())
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperCenter;
                GUI.color = Color.white;
                Widgets.Label(soulGemsViewRect, "SoulGemsInstructionsTrans".Translate());
            }
            else
                DoSoulGems(thingOwner, soulGemsViewRect, soulGems);

            GUI.EndGroup(); // soulGemsViewRect
            Widgets.EndScrollView();

            GUI.EndGroup(); // baseRect
            Text.Anchor = TextAnchor.UpperLeft;
        }

    }
}
