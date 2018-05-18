using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class BillStack_Enchant : IExposable
    {
        [Unsaved]
        public IBillGiver_Enchant billGiver;

        private List<Bill_Enchant> bills = new List<Bill_Enchant>();
        public const int MaxCount = 2;

        private const float TopAreaHeight = 35f;
        private const float BillInterfaceSpacing = 6f;
        private const float ExtraViewHeight = 60f;

        public List<Bill_Enchant> Bills { get { return bills; } }
        public Bill_Enchant this[int index] { get { return bills[index]; } }
        public int Count { get { return bills.Count; } }
        public Bill_Enchant FirstShouldDoNow
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if (bills[i].ShouldDoNow())
                        return bills[i];
                }

                return null;
            }
        }

        public bool AnyShouldDoNow
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if (bills[i].ShouldDoNow())
                        return true;
                }

                return false;
            }
        }

        public BillStack_Enchant(IBillGiver_Enchant giver) { billGiver = giver; }
        public IEnumerator<Bill_Enchant> GetEnumerator() { return bills.GetEnumerator(); }

        public void AddBill(Bill_Enchant bill)
        {
            bill.billStack = this;
            bills.Add(bill);
        }

        public void Delete(Bill_Enchant bill)
        {
            bill.deleted = true;
            bills.Remove(bill);
        }

        public void Clear() { bills.Clear(); }

        public void Reorder(Bill_Enchant bill, int offset)
        {
            int num = bills.IndexOf(bill);
            num += offset;
            if (num >= 0)
            {
                bills.Remove(bill);
                bills.Insert(num, bill);
            }
        }

        public void RemoveIncompleteBills()
        {
            for (int i = bills.Count - 1; i >= 0; i--)
            {
                if (!bills[i].CompletableEver)
                    bills.Remove(bills[i]);
            }
        }

        public int IndexOf(Bill_Enchant bill) { return bills.IndexOf(bill); }

        public Bill_Enchant DoListing(Rect rect, Func<List<FloatMenuOption>> recipeOptionsMaker, ref Vector2 scrollPosition, ref float viewHeight)
        {
            Bill_Enchant result = null;
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Small;
            if (Count < 15)
            {
                Rect rect2 = new Rect(0f, 0f, 150f, 29f);
                if (Widgets.ButtonText(rect2, "AddBill".Translate(), true, false, true))
                    Find.WindowStack.Add(new FloatMenu(recipeOptionsMaker()));
                UIHighlighter.HighlightOpportunity(rect2, "AddBill");
            }

            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            Rect outRect = new Rect(0f, 35f, rect.width, rect.height - 35f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, viewHeight);
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect, true);

            float num = 0f;
            for (int i = 0; i < Count; i++)
            {
                Bill_Enchant bill = bills[i];
                Rect rect3 = bill.DoInterface(0f, num, viewRect.width, i);
                if (!bill.DeletedOrDereferenced && Mouse.IsOver(rect3))
                    result = bill;
                num += rect3.height + 6f;
            }
            if (Event.current.type == EventType.Layout)
                viewHeight = num + 60f;

            Widgets.EndScrollView();
            GUI.EndGroup();
            return result;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref bills, "bills", LookMode.Deep, new object[] { this });
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                for (int i = 0; i < bills.Count; i++)
                    bills[i].billStack = this;
            }
        }
    }
}
