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
    public class Bill_Production_Enchant : Bill_Enchant, IExposable
    {
        public BillStoreModeDef storeMode = BillStoreModeDefOf.BestStockpile;
        public bool paused;
        protected override string StatusString { get { return paused ? (" " + "Paused".Translate()) : string.Empty; } }
        protected override float StatusLineMinHeight { get { return (!CanUnpause()) ? 0f : 24f; } }
        public Bill_Production_Enchant() { }
        public Bill_Production_Enchant(EnchantmentTask enchantmentTask) : base(enchantmentTask) { }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref storeMode, "storeMode");
            Scribe_Values.Look(ref paused, "paused", false, false);
            if (storeMode == null)
                storeMode = BillStoreModeDefOf.BestStockpile;
        }

        public override BillStoreModeDef GetStoreMode() { return storeMode; }
        public override bool ShouldDoNow()
        {
            if (suspended)
                return false;
            return true;
        }

        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            Messages.Message("MessageBillComplete".Translate(new object[] { LabelCap }), (Thing)billStack.billGiver, MessageTypeDefOf.TaskCompletion);
        }

        protected override void DoConfigInterface(Rect baseRect, Color baseColor)
        {
            Rect rect = new Rect(28f, 32f, 100f, 30f);
            GUI.color = new Color(1f, 1f, 1f, 0.65f);
            Widgets.Label(rect, "repTextHere");

            GUI.color = baseColor;
            WidgetRow widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp, 99999f, 4f);
            if (widgetRow.ButtonText("Details".Translate() + "...", null, true, false))
                Find.WindowStack.Add(new Dialog_BillConfig_RimWind(this, ((Thing)billStack.billGiver).Position));
        }

        private bool CanUnpause() { return paused; }

        public override void DoStatusLineInterface(Rect rect)
        {
            if (paused)
            {
                WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenUp, 99999f, 4f);
                if (widgetRow.ButtonText("Unpause".Translate(), null, true, false))
                    paused = false;
            }
        }
    }
}
