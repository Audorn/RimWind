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
    public class Dialog_BillConfig_RimWind : Window
    {
        private IntVec3 billGiverPos;

        private Bill_Production_Enchant bill;

        private Vector2 scrollPosition;

        public override Vector2 InitialSize { get { return new Vector2(760f, 600f); } }

        public Dialog_BillConfig_RimWind(Bill_Production_Enchant bill, IntVec3 billGiverPos)
        {
            this.billGiverPos = billGiverPos;
            this.bill = bill;
            forcePause = true;
            doCloseX = true;
            closeOnEscapeKey = true;
            doCloseButton = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public override void WindowUpdate() { base.WindowUpdate(); }
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect rect = new Rect(0f, 0f, 400f, 50f);
            Widgets.Label(rect, bill.LabelCap);

            Rect rect2 = new Rect(0f, 80f, 200f, inRect.height - 80f);
            Rect rect3 = new Rect(rect2.xMax + 17f, 50f, 180f, inRect.height - 50f - CloseButSize.y);
            Rect rect4 = new Rect(rect3.xMax + 17f, 50f, inRect.width - (rect3.xMax + 17f), inRect.height - 50f - CloseButSize.y);
            Text.Font = GameFont.Small;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect3);
            if (bill.suspended)
            {
                if (listing_Standard.ButtonText("Suspended".Translate(), null))
                    bill.suspended = false;
            }
            else if (listing_Standard.ButtonText("NotSuspended".Translate(), null))
                bill.suspended = true;

            string labelCap = bill.storeMode.LabelCap;
            if (listing_Standard.ButtonText(labelCap, null))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (BillStoreModeDef current in from bsm in DefDatabase<BillStoreModeDef>.AllDefs orderby bsm.listOrder select bsm)
                {
                    BillStoreModeDef smLocal = current;
                    list.Add(new FloatMenuOption(smLocal.LabelCap, delegate
                    {
                        bill.storeMode = smLocal;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            listing_Standard.Gap(12f);

            if (bill.workSkill != null)
            {
                listing_Standard.Label("AllowedSkillRange".Translate(new object[]
                {
                    bill.workSkill.label
                }), -1f);
                listing_Standard.IntRange(ref bill.allowedSkillRange, 0, 20);
            }

            listing_Standard.End();
            StringBuilder stringBuilder = new StringBuilder();
            if (bill.enchantmentTask.description != null)
            {
                stringBuilder.AppendLine(bill.enchantmentTask.description);
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine("WorkAmount".Translate() + ": " + bill.enchantmentTask.WorkAmountTotal().ToStringWorkAmount());
            stringBuilder.AppendLine();

            // Display the item and the soul gem.

            if (!bill.enchantmentTask.skillRequirements.NullOrEmpty())
            {
                stringBuilder.AppendLine("MinimumSkills".Translate());
                stringBuilder.AppendLine(bill.enchantmentTask.MinSkillString);
            }

            Text.Font = GameFont.Small;
            string text5 = stringBuilder.ToString();
            if (Text.CalcHeight(text5, rect2.width) > rect2.height)
                Text.Font = GameFont.Tiny;

            Widgets.Label(rect2, text5);
            Text.Font = GameFont.Small;

            if (bill.enchantmentTask.products.Count > 0)
                Widgets_Extensions.InfoCardButton(rect2.x, rect4.y, bill.enchantmentTask.products);

        }
    }
}
