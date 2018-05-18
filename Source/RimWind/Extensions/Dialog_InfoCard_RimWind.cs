using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class Dialog_InfoCard_RimWind : Window
    {
        private enum InfoCardTab : byte
        {
            Stats,
            Character,
            Health,
            Records
        }

        private EnchantmentTask enchantmentTask;
        private List<Thing> products;
        private InfoCardTab tab;

        public override Vector2 InitialSize { get { return new Vector2(950f, 760f); } }
        protected override float Margin { get { return 0f; } }
        public Dialog_InfoCard_RimWind(EnchantmentTask enchantmentTask)
        {
            this.enchantmentTask = enchantmentTask;
            tab = InfoCardTab.Stats;
            Setup();
        }
        public Dialog_InfoCard_RimWind(List<Thing> products)
        {
            this.products = products;
            tab = InfoCardTab.Stats;
            Setup();
        }

        public void Setup()
        {
            forcePause = true;
            closeOnEscapeKey = true;
            doCloseButton = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            soundAppear = SoundDef.Named("InfoCard_Open");
            soundClose = SoundDef.Named("InfoCard_Close");
            StatsReportUtility.Reset();
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.InfoCard, KnowledgeAmount.Total);
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect);
            rect = rect.ContractedBy(18f);
            rect.height = 34f;
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, GetTitle());

            Rect rect2 = new Rect(inRect);
            rect2.yMin = rect.yMax;
            rect2.yMax -= 38f;

            Rect rect3 = rect2;
            rect3.yMin += 45f;

            List<TabRecord> list = new List<TabRecord>();
            TabRecord item = new TabRecord("TabStats".Translate(), delegate
            {
                tab = InfoCardTab.Stats;
            }, tab == InfoCardTab.Stats);
            list.Add(item);

            TabDrawer.DrawTabs(rect3, list);
            FillCard(rect3.ContractedBy(18f));
        }

        protected void FillCard(Rect cardRect)
        {
            if (tab == InfoCardTab.Stats)
            {
                if (enchantmentTask != null)
                {
                    Thing enchantableThing = enchantmentTask.enchantableThing;
                    Thing soulGem = enchantmentTask.soulGem;
                    if (enchantableThing != null)
                        StatsReportUtility.DrawStatsReport(cardRect, enchantableThing);
                    if (soulGem != null)
                        StatsReportUtility.DrawStatsReport(cardRect, soulGem); // needs its own.
                }
                
                if (!products.NullOrEmpty())
                {
                    // Draw the products instead!
                    Thing enchantableThing = enchantmentTask.enchantableThing;
                    if (enchantableThing != null)
                        StatsReportUtility.DrawStatsReport(cardRect, enchantableThing);
                }
            }
        }

        private string GetTitle()
        {
            if (enchantmentTask != null)
                return enchantmentTask.LabelCap;

            return "error";
        }
    }
}
