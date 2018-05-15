using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimTES
{ 
    public class ITab_RBills_Enchanting : ITab
    {
        private float viewHeight = 1000f;

        private Vector2 scrollPosition = default(Vector2);

        private Bill mouseoverBill;

        private static readonly Vector2 WinSize = new Vector2(420f, 480f);

        protected Building_ProductionResearchBench SelTable
        {
            get
            {
                return (Building_ProductionResearchBench)SelThing;
            }
        }

        public ITab_RBills_Enchanting()
        {
            size = WinSize;
            labelKey = "TabBills";
            tutorTag = "Bills";
        }

        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            Rect rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                for (int i = 0; i < SelTable.def.AllRecipes.Count; i++)
                {
                    if (SelTable.def.AllRecipes[i].AvailableNow)
                    {
                        RecipeDef recipe = SelTable.def.AllRecipes[i];
                        list.Add(new FloatMenuOption(recipe.LabelCap, delegate
                        {
                            if (!SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
                            {
                                Bill.CreateNoPawnsWithSkillDialog(recipe);
                            }
                            Bill bill = recipe.MakeNewBill();
                            SelTable.billStack.AddBill(bill);
                            if (recipe.conceptLearned != null)
                            {
                                PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
                            }
                            if (TutorSystem.TutorialMode)
                            {
                                TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap);
                            }
                        }, MenuOptionPriority.Default, null, null, 29f, (Rect r) => Widgets.InfoCardButton(r.x + 5f, r.y + (r.height - 24f) / 2f, recipe), null));
                    }
                }
                if (!list.Any())
                {
                    list.Add(new FloatMenuOption("NoneBrackets".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                return list;
            };
            mouseoverBill = SelTable.billStack.DoListing(rect, recipeOptionsMaker, ref scrollPosition, ref viewHeight);
        }

        public override void TabUpdate()
        {
            if (mouseoverBill != null)
            {
                mouseoverBill.TryDrawIngredientSearchRadiusOnMap(SelTable.Position);
                mouseoverBill = null;
            }
        }

    }
}
