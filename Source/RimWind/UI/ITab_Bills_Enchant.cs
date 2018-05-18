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
        private float enchantableViewHeight = 200f;

        private Vector2 scrollPosition = default(Vector2);

        private Bill_Enchant mouseoverBill;
        private Thing mouseHoveredThing;

        private static readonly Vector2 WinSize = new Vector2(420f, 480f);

        protected Building_ProductionResearchBench_InternalRecipes SelTable
        { get { return (Building_ProductionResearchBench_InternalRecipes)SelThing; } }

        public ITab_Bills_Enchant()
        {
            size = WinSize;
            labelKey = "TabBillsEnchant";
            tutorTag = "Bills";
        }

        protected override void FillTab()
        {
            ThingOwner thingOwner = SelTable.GetDirectlyHeldThings();
            CompInnerContainerItemFilter itemFilterComp = SelTable.GetComp<CompInnerContainerItemFilter>();

            Rect baseRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            GUI.BeginGroup(baseRect);
            if (itemFilterComp != null)
            {
                bool noBills = true; // Check this one later.
                bool noEnchantables = true;
                bool noSoulGems = true;
                foreach (ItemFilterSetting filterSetting in itemFilterComp.ItemFilterSettings)
                {
                    List<Thing> currentThings = new List<Thing>();
                    foreach (Thing thing in thingOwner)
                    {
                        if (filterSetting.thingDef != null && filterSetting.thingDef == thing.def)
                            currentThings.Add(thing);

                        else if (filterSetting.thingCategoryDef != null)
                        {
                            foreach (ThingCategoryDef categoryDef in thing.def.thingCategories)
                                if (filterSetting.thingCategoryDef == categoryDef)
                                    currentThings.Add(thing);
                        }
                    }

                    Rect outRect = new Rect(0f, 200f, baseRect.width, baseRect.height - 300f);
                    Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, enchantableViewHeight);
                    Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect, true);
                    // Enchantables
                    if (!currentThings.NullOrEmpty() && filterSetting.thingCategoryDef != null && filterSetting.thingCategoryDef == DefDatabase<ThingCategoryDef>.GetNamed("Enchantables"))
                    {
                        noEnchantables = false;
                        float y = viewRect.y;
                        for (int i = 0; i < currentThings.Count; i++)
                        {
                            CompEnchantable enchantable = currentThings[i].TryGetComp<CompEnchantable>();
                            if (enchantable == null || enchantable.currentlyEnchanting)
                                continue;

                            Rect enchantableItemRect = new Rect(viewRect.x, y, viewRect.width, 64f);
                            GUI.BeginGroup(enchantableItemRect);
                            Text.Font = GameFont.Small;
                            Text.Anchor = TextAnchor.UpperLeft;
                            GUI.color = Color.white;

                            Rect labelRect = new Rect(12f, 0f, baseRect.width - 48f - 20f, enchantableItemRect.height + 5f);
                            Widgets.Label(labelRect, enchantable.parent.LabelCap);

                            Rect iconRect = new Rect(12f, 0f, 64f, 64f);
                            Widgets.ThingIcon(iconRect, enchantable.parent);


                            //Rect eRect = enchantable.DoInterface(0f, y, viewRect.width, i);
                            if (Mouse.IsOver(iconRect))
                                mouseHoveredThing = currentThings[i];

                            GUI.EndGroup();
                            y += enchantableItemRect.height + 6f;
                        }

                    }
                    if (noEnchantables)
                    {
                        Rect enchantableInstructionsRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 100f);
                        GUI.BeginGroup(enchantableInstructionsRect);
                        Text.Font = GameFont.Small;
                        Text.Anchor = TextAnchor.UpperCenter;
                        GUI.color = Color.white;

                        Widgets.Label(enchantableInstructionsRect, "EnchantableInstructionsTrans".Translate());
                        GUI.EndGroup();
                    }
                    Widgets.EndScrollView();
                    Text.Anchor = TextAnchor.UpperLeft;

                    // Soul Gems
                    if (!currentThings.NullOrEmpty() && filterSetting.thingDef != null && filterSetting.thingDef == DefDatabase<ThingDef>.GetNamed("WoodLog"))
                    {
                        noSoulGems = false;
                        for (int i = 0; i < currentThings.Count; i++)
                        {

                        }
                    }

                    
                }
            }
            GUI.EndGroup();
        }

    }
}
