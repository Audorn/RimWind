using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    [StaticConstructorOnStartup]
    public static class Widgets_Extensions
    {
        public static readonly Texture2D infoTex = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
        public static readonly Texture2D reorderUpTex = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp", true);
        public static readonly Texture2D reorderDownTex = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown", true);
        public static readonly Texture2D deleteXTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        public static readonly Texture2D suspendTex = ContentFinder<Texture2D>.Get("UI/Buttons/Suspend", true);
        public static readonly Texture2D plusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        public static readonly Texture2D minusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Minus", true);
        public static readonly Texture2D addTex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Add", true);
        public static readonly Texture2D revealTex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

        private static bool InfoCardButtonWorker(float x, float y)
        {
            Rect rect = new Rect(x, y, 24f, 24f);
            TooltipHandler.TipRegion(rect, "ToolTipHere");
            bool result = Widgets.ButtonImage(rect, infoTex, GUI.color);
            UIHighlighter.HighlightOpportunity(rect, "InfoCard");
            return result;
        }

        public static bool InfoCardButton(float x, float y, EnchantmentTask enchantmentTask)
        {
            if (InfoCardButtonWorker(x, y))
            {
                Find.WindowStack.Add(new Dialog_InfoCard_RimWind(enchantmentTask));
                return true;
            }

            return false;
        }

        public static bool InfoCardButton(float x, float y, List<Thing> products)
        {
            if (InfoCardButtonWorker(x, y))
            {
                Find.WindowStack.Add(new Dialog_InfoCard_RimWind(products));
                return true;
            }

            return false;
        }

        public static void AbilityIcon(Rect rect, Ability ability, float alpha = 1f)
        {
            Texture resolvedIcon = null;
            if (alpha != 1f)
            {
                Color color = GUI.color;
                color.a *= alpha;
                GUI.color = color;
            }

            if (ability is CustomAbility customAbility)
            {
                if (!customAbility.texPath.NullOrEmpty())
                    resolvedIcon = BaseContent.BadTex;
            }
            else if (ability.def != null)
               resolvedIcon = ability.def.uiIcon;

            if (resolvedIcon != null)
                Widgets.DrawTextureRotated(rect, resolvedIcon, 0f);
        }

    }
}
