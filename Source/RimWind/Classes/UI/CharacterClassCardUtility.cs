using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimTES
{
    [StaticConstructorOnStartup]
    public static class CharacterClassCardUtility
    {
        public const float TopPadding = 20f;
        private const float IconSize = 20f;
        private static float AbilityHeight = 20f;
        private static float AbilityVerticalGap = 4f;

        private static Vector2 AbilitiesScrollPosition = default(Vector2);
        private static Vector2 ActiveAbilitiesScrollPosition = default(Vector2);

        private static float iconSize = 32f;
        private static float abilityLabelHeightMod = 5f;

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private static readonly Color StaticHighlightColor = new Color(0.75f, 0.75f, 0.85f, 1f);
        private static readonly Color ImprovedColor = new Color(0.5f, 0.5f, 0.9f);
        private static readonly Color ReducedColor = new Color(0.7f, 0.7f, 0.7f);

        private static bool allowClassEffects = true;
        private static void ResetTabs()
        {
            onClassSummaryTab = false;
            onBirthSignTab = false;
            onAbilitiesTab = false;
        }

        private static bool onClassSummaryTab = false;
        private static bool OnClassSummaryTab
        {
            get { return onClassSummaryTab; }
            set
            {
                ResetTabs();
                onClassSummaryTab = value;
            }
        }

        private static bool onBirthSignTab = false;
        private static bool OnBirthSignTab
        {
            get { return onBirthSignTab; }
            set
            {
                ResetTabs();
                onBirthSignTab = value;
            }
        }

        private static bool onAbilitiesTab = true;
        private static bool OnAbilitiesTab
        {
            get { return onAbilitiesTab; }
            set
            {
                ResetTabs();
                onAbilitiesTab = value;
            }
        }

        public static void DrawPawnClassCard(Rect outRect, Pawn pawn)
        {
            outRect = outRect.Rounded();
            Rect rect = new Rect(outRect.x, outRect.y, outRect.width * 0.375f, outRect.height).Rounded();
            Rect rect2 = new Rect(rect.xMax, outRect.y, outRect.width - rect.width, outRect.height);
            rect.yMin += 11f;
            DrawClassSummary(rect, pawn);
        }

        public static void DrawClassSummary(Rect rect, Pawn pawn)
        {
            Widgets.DrawMenuSection(rect);
            List<TabRecord> list = new List<TabRecord>();

            if (allowClassEffects)
            {
                list.Add(new TabRecord("ClassSummaryTab".Translate(), delegate
                {
                    OnClassSummaryTab = true;
                }, OnClassSummaryTab));
            }

            list.Add(new TabRecord("BirthSignTab".Translate(), delegate
            {
                OnBirthSignTab = true;
            }, OnBirthSignTab));

            list.Add(new TabRecord("AbilitiesTab".Translate(), delegate
            {
                OnAbilitiesTab = true;
            }, OnAbilitiesTab));

            TabDrawer.DrawTabs(rect, list);
            rect = rect.ContractedBy(9f);
            GUI.BeginGroup(rect);
            float curY = 0f;
            Text.Font = GameFont.Medium;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperCenter;

            if (OnClassSummaryTab)
            {
                curY = DrawClassSummaryTab(rect, pawn, curY);
            }
            else if (OnBirthSignTab)
            {
                curY = DrawBirthSignTab(rect, pawn, curY);
            }
            else if (OnAbilitiesTab)
            {
                curY = DrawAbilitiesTab(rect, pawn, curY);
            }

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
        }

        public static float DrawClassSummaryTab(Rect leftRect, Pawn pawn, float curY)
        {
            CompCharacterClass characterClassComp = pawn.GetComp<CompCharacterClass>();

            curY += 4f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            Rect classNameRect = new Rect(0f, curY, leftRect.width, 34f);
            Widgets.Label(classNameRect, characterClassComp.classRecord.def.LabelCap);
            /*
            TooltipHandler.TipRegion(classNameRect, () => characterClassComp.classRecord.def.description, 73412);
            if (Mouse.IsOver(classNameRect))
                Widgets.DrawHighlight(classNameRect);
                */
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            curY += 34f;

            return curY;
        }

        public static float DrawBirthSignTab(Rect leftRect, Pawn pawn, float curY)
        {
            CompCharacterClass characterClassComp = pawn.GetComp<CompCharacterClass>();
            // BirthSignComp

            curY += 4f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            Rect birthSignNameRect = new Rect(0f, curY, leftRect.width, 34f);
            Widgets.Label(birthSignNameRect, "BirthSignNameHere");

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            curY += 34f;

            return curY;
        }

        public static float DrawAbilitiesTab(Rect leftRect, Pawn pawn, float curY)
        {
            CompAbilityHolder abilityHolderComp = pawn.GetComp<CompAbilityHolder>();
            List<Ability> abilities = abilityHolderComp.abilities;
            List<Ability> activeAbilities = abilityHolderComp.clickableAbilities;

            curY += 4f;
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            Rect abilityNameRect = new Rect(0f, curY, leftRect.width, 34f);
            Widgets.Label(abilityNameRect, "AbilitiesTab".Translate());

            curY += 34f;

            Rect abilitiesOuterRect = new Rect(0f, curY, leftRect.width, 116f);
            Rect abilitiesViewRect = new Rect(
                    abilitiesOuterRect.x,
                    0f,
                    abilitiesOuterRect.width - 16f,
                    abilities.NullOrEmpty() ? abilitiesOuterRect.height : abilities.Count * AbilityHeight + (abilities.Count - 1) * AbilityVerticalGap);

//            GUI.color = Color.gray;

            Widgets.BeginScrollView(abilitiesOuterRect, ref AbilitiesScrollPosition, abilitiesViewRect, true);
            GUI.BeginGroup(abilitiesViewRect);
            if (abilities.NullOrEmpty())
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperCenter;
                GUI.color = Color.white;
                Widgets.Label(abilitiesViewRect, "ThisPawnHasNoAbilities".Translate());
                curY += 100f;
            }
            else
            {
                curY += DoAbilities(abilityHolderComp, abilitiesViewRect, abilities);
                curY += 20f;
            }

            GUI.EndGroup(); // enchantablesViewRect
            Widgets.EndScrollView();

            Rect activeAbilitiesOuterRect = new Rect(0f, curY, leftRect.width, 116f);
            Rect activeAbilitiesViewRect = new Rect(
                activeAbilitiesOuterRect.x,
                0f,
                activeAbilitiesOuterRect.width - 16f,
                activeAbilities.NullOrEmpty() ? activeAbilitiesOuterRect.height : activeAbilities.Count * AbilityHeight + (activeAbilities.Count - 1) * AbilityVerticalGap);

            Widgets.BeginScrollView(activeAbilitiesOuterRect, ref ActiveAbilitiesScrollPosition, activeAbilitiesViewRect, true);
            GUI.BeginGroup(activeAbilitiesViewRect);
            if (activeAbilities.NullOrEmpty())
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperCenter;
                GUI.color = Color.white;
                Widgets.Label(activeAbilitiesViewRect, "ThisPawnHasNoActiveAbilities".Translate());
                curY += 100f;
            }
            else
            {
                curY += DoActiveAbilities(abilityHolderComp, activeAbilitiesViewRect, abilities);
                curY += 20f;
            }

            GUI.EndGroup();
            Widgets.EndScrollView();

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);
            return curY;
        }

        private static float DoAbilities(CompAbilityHolder abilityHolderComp, Rect rect, List<Ability> abilities)
        {
            float y = 0f;
            for (int i = 0; i < abilities.Count; i++)
            {
                Rect abilityRect = new Rect(rect.x, y, rect.width, AbilityHeight);
                Ability hoveredAbility = DoAbilityInterface(abilityRect, abilities[i], abilityHolderComp.parent as Pawn);

//                if (hoveredAbility != null)
//                    mouseHoveredAbility = hoveredAbility;

                y += AbilityHeight + AbilityVerticalGap;
            }

            return y;
        }

        private static Ability DoAbilityInterface(Rect abilityRect, Ability ability, Pawn pawn)
        {
            GUI.BeginGroup(abilityRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            // Disable for passive abilities.
            Rect addRect = new Rect(0f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(addRect, Widgets_Extensions.deleteXTex, Color.white))
                Log.Warning("clicked add");

            Rect iconRect = new Rect(26f, 0f, iconSize, iconSize);
            //Widgets.ThingIcon(iconRect, pawn); // Extend the widgets class.

            Rect labelRect = new Rect(iconRect.x + iconRect.width + 2f, 0f, 100f, 30f);
            Widgets.Label(labelRect, ability.def.LabelCap);

            Rect effectsRect = new Rect(labelRect.x + labelRect.width + 2f, 0f, 200f, 30f);
            Widgets.Label(effectsRect, ability.def.description);

            Rect deleteRect = new Rect(effectsRect.x + effectsRect.width, 0f, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, Color.white))
                Log.Warning("clicked delete");

            GUI.EndGroup();

            if (Mouse.IsOver(abilityRect))
                return ability;

            return null;
        }

        private static float DoActiveAbilities(CompAbilityHolder abilityHolderComp, Rect rect, List<Ability> abilities)
        {
            float y = 0f;
            for (int i = 0; i < abilities.Count; i++)
            {
                Rect abilityRect = new Rect(rect.x, y, rect.width, AbilityHeight);
                Ability hoveredAbility = DoActiveAbilityInterface(abilityRect, abilities[i], abilityHolderComp.parent as Pawn);

                //                if (hoveredAbility != null)
                //                    mouseHoveredAbility = hoveredAbility;

                y += AbilityHeight + AbilityVerticalGap;
            }

            return y;
        }

        private static Ability DoActiveAbilityInterface(Rect abilityRect, Ability ability, Pawn pawn)
        {
            GUI.BeginGroup(abilityRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            // Disable for passive abilities.
            Rect upRect = new Rect(0f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(upRect, Widgets_Extensions.deleteXTex, Color.white))
                Log.Warning("clicked up");

            Rect downRect = new Rect(26f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(downRect, Widgets_Extensions.deleteXTex, Color.white))
                Log.Warning("clicked down");

            Rect iconRect = new Rect(downRect.x + downRect.width + 2f, 0f, iconSize, iconSize);
            //Widgets.ThingIcon(iconRect, pawn); // Extend the widgets class.

            Rect labelRect = new Rect(iconRect.x + iconRect.width + 2f, 0f, 100f, 30f);
            Widgets.Label(labelRect, ability.def.LabelCap);

            Rect effectsRect = new Rect(labelRect.x + labelRect.width + 2f, 0f, 200f, 30f);
            Widgets.Label(effectsRect, ability.def.description);

            Rect deleteRect = new Rect(effectsRect.x + effectsRect.width, 0f, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, Color.white))
                Log.Warning("clicked delete");

            GUI.EndGroup();

            if (Mouse.IsOver(abilityRect))
                return ability;

            return null;
        }

    }
}
