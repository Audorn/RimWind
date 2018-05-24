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
    public abstract class Bill_Enchant : IExposable, ILoadReferenceable
    {
        [Unsaved]
        public BillStack_Enchant billStack;

        private int loadID = -1;

        public EnchantmentTask enchantmentTask;
        public bool suspended;
        public IntRange allowedSkillRange = new IntRange(0, 20);
        public bool deleted;

        public const float ButSize = 24f;
        private const float InterfaceBaseHeight = 53f;
        private const float InterfaceStatusLineHeight = 17f;

        public Map Map { get { return billStack.billGiver.Map; } }
        public virtual string Label { get { return enchantmentTask.label; } }
        public virtual string LabelCap { get { return enchantmentTask.LabelCap; } }
        public virtual bool CompletableEver { get { return true; } }

        protected virtual string StatusString { get { return null; } }
        protected virtual float StatusLineMinHeight { get { return 0f; } }

        public StatDef workSpeedStat;
        public StatDef efficiencyStat;
        public SkillDef workSkill;

        public Bill_Enchant() { }

        public Bill_Enchant(EnchantmentTask enchantmentTask)
        {
            this.enchantmentTask = enchantmentTask;
            loadID = Find.UniqueIDsManager.GetNextBillID();
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref loadID, "loadID", 0, false);
            Scribe_Deep.Look(ref enchantmentTask, "enchantment");
            Scribe_Values.Look(ref suspended, "suspended", false, false);
            Scribe_Values.Look(ref allowedSkillRange, "allowedSkillRange", default(IntRange), false);
            Scribe_Defs.Look(ref workSpeedStat, "workSpeedStat");
            Scribe_Defs.Look(ref efficiencyStat, "efficiencyStat");
            Scribe_Defs.Look(ref workSkill, "workSkill");
        }

        public virtual bool PawnAllowedToStartAnew(Pawn p)
        {
            if (workSkill != null)
            {
                int level = p.skills.GetSkill(workSkill).Level;
                if (level < allowedSkillRange.min || level > allowedSkillRange.max)
                    return false;
            }

            return true;
        }

        public virtual void Notify_PawnDidWork(Pawn p) { }
        public virtual void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients) { }
        public abstract bool ShouldDoNow();
        public virtual void Notify_DoBillsStarted(Pawn billDoer) { }
        public virtual void DoStatusLineInterface(Rect rect) { }
        public virtual BillStoreModeDef GetStoreMode() { return BillStoreModeDefOf.BestStockpile; }
        protected virtual void DoConfigInterface(Rect rect, Color baseColor)
        {
            rect.yMin += 29f;
            float y = rect.center.y;
            float num = rect.xMax - (rect.yMax - y);
            Widgets_Extensions.InfoCardButton(num - 12f, y - 12f, enchantmentTask);
        }

        public Rect DoInterface(float x, float y, float width, int index)
        {
            Rect rect = new Rect(x, y, width, 53f);
            float num = 0f;
            if (!StatusString.NullOrEmpty())
                num = Mathf.Max(17f, StatusLineMinHeight);

            rect.height += num;
            Color white = Color.white;
            if (!ShouldDoNow())
                white = new Color(1f, 0.7f, 0.7f, 0.7f);

            GUI.color = white;
            Text.Font = GameFont.Small;
            if (index % 2 == 0)
                Widgets.DrawAltRect(rect);

            GUI.BeginGroup(rect);
            Rect butRect = new Rect(0f, 0f, 24f, 24f);
            if (billStack.IndexOf(this) > 0 && Widgets.ButtonImage(butRect, Widgets_Extensions.reorderUpTex, white))
            {
                billStack.Reorder(this, -1);
                SoundDefOf.TickHigh.PlayOneShotOnCamera(null);
            }
            if (billStack.IndexOf(this) < billStack.Count - 1)
            {
                Rect butRect2 = new Rect(0f, 24f, 24f, 24f);
                if (Widgets.ButtonImage(butRect2, Widgets_Extensions.reorderDownTex, white))
                {
                    billStack.Reorder(this, 1);
                    SoundDefOf.TickLow.PlayOneShotOnCamera(null);
                }
            }

            Rect rect2 = new Rect(28f, 0f, rect.width - 48f - 20f, rect.height + 5f);
            Widgets.Label(rect2, LabelCap);
            DoConfigInterface(rect.AtZero(), white);

            Rect rect3 = new Rect(rect.width - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(rect3, Widgets_Extensions.deleteXTex, white))
                billStack.Delete(this);

            Rect butRect3 = new Rect(rect3);
            butRect3.x -= butRect3.width + 4f;
            if (Widgets.ButtonImage(butRect3, Widgets_Extensions.suspendTex, white))
                suspended = !suspended;

            if (!StatusString.NullOrEmpty())
            {
                Text.Font = GameFont.Tiny;
                Rect rect4 = new Rect(24f, rect.height - num, rect.width - 24f, num);
                Widgets.Label(rect4, StatusString);
                DoStatusLineInterface(rect4);
            }
            GUI.EndGroup();

            if (suspended)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect rect5 = new Rect(rect.x + rect.width / 2f - 70f, rect.y + rect.height / 2f - 20f, 140f, 40f);
                GUI.DrawTexture(rect5, TexUI.GrayTextBG);
                Widgets.Label(rect5, "SuspendedCaps".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            return rect;
        }

        public static void CreateNoPawnsWithSkillDialog(EnchantmentTask enchantmentTask)
        {
            string text = "EnchantmentTaskRequiresSkills".Translate(new object[] { enchantmentTask.LabelCap });
            text += "\n\n";
            text += enchantmentTask.MinSkillString;
            Find.WindowStack.Add(new Dialog_MessageBox(text, null, null, null, null, null, false));
        }

        public override string ToString() { return GetUniqueLoadID(); }
        public string GetUniqueLoadID()
        {
            return string.Concat(new object[]
            {
                "Bill_",
                enchantmentTask.LabelCap,
                "_",
                loadID
            });
        }

        public bool DeletedOrDereferenced
        {
            get
            {
                if (deleted)
                    return true;

                Thing thing = billStack.billGiver as Thing;
                return thing != null && thing.Destroyed;
            }
        }
    }
}
