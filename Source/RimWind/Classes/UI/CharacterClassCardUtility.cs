﻿using System;
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
        private const float IconSize = 24f; //20f
        private static float AbilityHeight = 32f;
        private static float AbilityVerticalGap = 4f;

        private static List<AbilityCategoryDef> AbilityCategoryDefs = new List<AbilityCategoryDef>();
        private static List<bool> onAbilityCategories = new List<bool>();
        private static Vector2 AbilitiesScrollPosition = default(Vector2);
        private static Vector2 ActiveAbilitiesScrollPosition = default(Vector2);

        private static float iconSize = 32f;

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

        public static float DrawClassSummaryTab(Rect rect, Pawn pawn, float curY)
        {
            CompCharacterClass characterClassComp = pawn.GetComp<CompCharacterClass>();

            curY += 4f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            Rect classNameRect = new Rect(0f, curY, rect.width, 34f);
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

        public static float DrawBirthSignTab(Rect rect, Pawn pawn, float curY)
        {
            CompCharacterClass characterClassComp = pawn.GetComp<CompCharacterClass>();
            // BirthSignComp

            curY += 4f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            Rect birthSignNameRect = new Rect(0f, curY, rect.width, 34f);
            Widgets.Label(birthSignNameRect, "BirthSignNameHere");

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            curY += 34f;

            return curY;
        }

        public static float DrawAbilitiesTab(Rect rect, Pawn pawn, float curY)
        {
            CompAbilityHolder abilityHolderComp = pawn.GetComp<CompAbilityHolder>();
            List<Ability> abilities = abilityHolderComp.abilities;
            List<Ability> activeAbilities = abilityHolderComp.SortedClickableAbilities;
            List<Ability> forgettingAbilities = abilityHolderComp.ForgettingAbilities;

            // ========== All Abilities ==========
            PrepareAbilityCategoryDefs(abilities);
            onAbilityCategories.Clear();
            foreach (AbilityCategoryDef catDef in AbilityCategoryDefs)
                onAbilityCategories.Add(false);
            if (!onAbilityCategories.NullOrEmpty())
                onAbilityCategories[0] = true;

            Rect allAbilitiesByCategoryRect = new Rect(0f, 40f, rect.width, 148f);
            Widgets.DrawMenuSection(allAbilitiesByCategoryRect);
            List<TabRecord> abilityTabs = new List<TabRecord>();

            for (int i = 0; i < AbilityCategoryDefs.Count; i++)
            {
                abilityTabs.Add(new TabRecord((AbilityCategoryDefs[i].defName + "Key").Translate(), delegate
                {
                    for (int t = 0; t < onAbilityCategories.Count; t++)
                        onAbilityCategories[t] = false;

                    onAbilityCategories[i] = true;
                }, onAbilityCategories[i]));
            }

            if (!abilityTabs.NullOrEmpty())
            {
                TabDrawer.DrawTabs(allAbilitiesByCategoryRect, abilityTabs);

                rect = rect.ContractedBy(9f);
                GUI.BeginGroup(allAbilitiesByCategoryRect);
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperCenter;

                DoAbilityCategories(allAbilitiesByCategoryRect, abilities, pawn);

                GUI.EndGroup();
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(allAbilitiesByCategoryRect, "PawnHasNoAbilities".Translate());
            }

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect abilityCapacity = new Rect(rect.xMax - 180f, 190f, 180f, 30f);
            GUI.BeginGroup(abilityCapacity);

            Rect labelRect = new Rect(0f, 0f, 130f, 30f);
            Widgets.Label(labelRect, "AcquiredAbilities".Translate() + ": ");

            Rect currentMaxRect = new Rect(120f, 0f, 50f, 30f);
            Widgets.Label(currentMaxRect, "99/99");

            GUI.EndGroup();

            // ========== Active Abilities ==========

            Rect activeAbilitiesOuterRect = new Rect(4f, 225f, rect.width + 10f, 140f);
            Rect activeAbilitiesViewRect = new Rect(
                activeAbilitiesOuterRect.x,
                0f,
                activeAbilitiesOuterRect.width - 16f,
                activeAbilities.NullOrEmpty() ? activeAbilitiesOuterRect.height : activeAbilities.Count * AbilityHeight + (activeAbilities.Count - 1) * AbilityVerticalGap);

            Widgets.BeginScrollView(activeAbilitiesOuterRect, ref ActiveAbilitiesScrollPosition, activeAbilitiesViewRect, true);
            GUI.BeginGroup(activeAbilitiesViewRect);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (!activeAbilities.NullOrEmpty())
            {
                DoActiveAbilities(abilityHolderComp, activeAbilitiesViewRect, activeAbilities);
            }

            GUI.EndGroup();
            Widgets.EndScrollView();

            if (activeAbilities.NullOrEmpty())
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(activeAbilitiesOuterRect, "PawnHasNoActiveAbilities".Translate());
            }

            // ========== Forgetting Abilities ==========

            Rect forgettingAbilitiesRect = new Rect(4f, 385f, rect.width + 10f, 68f);

            GUI.BeginGroup(forgettingAbilitiesRect);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (!forgettingAbilities.NullOrEmpty())
                DoForgettingAbilities(abilityHolderComp, forgettingAbilitiesRect, forgettingAbilities);
            GUI.EndGroup();

            if (forgettingAbilities.NullOrEmpty())
            {
                Text.Font = GameFont.Small;
                Widgets.Label(forgettingAbilitiesRect, "PawnIsNotForgettingAbilities".Translate());
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            return curY; // obsolete
        }

        private static void DoAbilityCategories(Rect rect, List<Ability> abilities, Pawn pawn)
        {
            Rect abilitiesOuterRect = new Rect(4f, 4f, rect.width - 8f, rect.height - 8f);
            Rect abilitiesViewRect = new Rect(
                    abilitiesOuterRect.x,
                    0f,
                    abilitiesOuterRect.width - 16f,
                    abilities.NullOrEmpty() ? abilitiesOuterRect.height : abilities.Count * AbilityHeight + (abilities.Count - 1) * AbilityVerticalGap);

            for (int i = 0; i < AbilityCategoryDefs.Count; i++)
            {
                if (onAbilityCategories[i])
                {
                    List<Ability> abilitiesInCategory = new List<Ability>();
                    foreach (Ability ability in abilities)
                        if (ability.def.abilityCategoryDefs.Contains(AbilityCategoryDefs[i]))
                            abilitiesInCategory.Add(ability);

                    DoAbilitiesInCategory(abilitiesOuterRect, abilitiesViewRect, abilitiesInCategory, pawn);
                }
            }
        }

        private static void DoAbilitiesInCategory(Rect outerRect, Rect viewRect, List<Ability> abilities, Pawn pawn)
        {
            Widgets.BeginScrollView(outerRect, ref AbilitiesScrollPosition, viewRect, true);
            GUI.BeginGroup(viewRect);

            if (!abilities.NullOrEmpty())
                DoAbilities(pawn.GetComp<CompAbilityHolder>(), viewRect, abilities);

            GUI.EndGroup();
            Widgets.EndScrollView();
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
            CompAbilityHolder abilityHolderComp = pawn.GetComp<CompAbilityHolder>();

            GUI.BeginGroup(abilityRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Color.white;

            Rect iconRect = new Rect(0f, 0f, iconSize, iconSize);
            Widgets_Extensions.AbilityIcon(iconRect, ability);

            GUI.color = abilityHolderComp.ClickableAbilities.Contains(ability) ? Color.green : Color.white;
            Rect labelRect = new Rect(iconRect.width + 4f, 0f, 120f, 32f);
            Widgets.Label(labelRect, ability.def.LabelCap);
            GUI.color = Color.white;

            float deleteRectWidth = ability.lastAttemptedForget <= 0 ? 34f : 154f;

            Rect infoRect = new Rect(abilityRect.xMax - 24f - 24f - deleteRectWidth, 0f, 24f, 24f);
            if (Widgets.ButtonImage(infoRect, Widgets_Extensions.infoTex, Color.white))
                Log.Warning("clicked info");

            Rect addRect = new Rect(abilityRect.xMax - 24f - deleteRectWidth, 0f, 24f, 24f);
            if (Widgets.ButtonImage(addRect, Widgets_Extensions.plusTex, Color.white))
                abilityHolderComp.ToggleClickable(ability);

            Rect deleteRect = new Rect(abilityRect.xMax - deleteRectWidth, 0f, deleteRectWidth - 10f, 24f); // 24f + 10f (scrollbar) = 34f
            if (ability.lastAttemptedForget <= 0)
            {
                if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, ability.forgetting ? new Color(0.2f, 0.2f, 0.2f) : Color.white))
                    abilityHolderComp.ToggleForgettable(ability);
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Tiny;
                Widgets.Label(deleteRect, GenDate.ToStringTicksToPeriod(ability.lastAttemptedForget, true, true));
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            GUI.color = abilityHolderComp.ClickableAbilities.Contains(ability) ? Color.green : Color.white;
            float effectsWidth = abilityRect.width - iconRect.width - labelRect.width - infoRect.width - addRect.width - deleteRect.width - 8f; // 8f sum of gaps.
            Rect effectsRect = new Rect(labelRect.x + labelRect.width + 4f, 0f, effectsWidth, 32f);
            Widgets.Label(effectsRect, ability.def.description);
            GUI.color = Color.white;
            GUI.EndGroup();

            if (ability.forgetting)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect forgettingRect = new Rect(abilityRect.x, abilityRect.y, abilityRect.width, abilityRect.height);
                GUI.DrawTexture(forgettingRect, TexUI.TextBGBlack);
                Widgets.Label(forgettingRect, "ForgettingAbility".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }

            if (Mouse.IsOver(abilityRect))
                return ability;

            return null;
        }

        public static void PrepareAbilityCategoryDefs(List<Ability> abilities)
        {
            AbilityCategoryDefs.Clear();
            List<AbilityCategoryDef> allAbilityCategories = DefDatabase<AbilityCategoryDef>.AllDefsListForReading;
            foreach (AbilityCategoryDef abilityCategoryDef in allAbilityCategories)
            {
                foreach (Ability ability in abilities)
                {
                    if (ability.def.abilityCategoryDefs.Contains(abilityCategoryDef))
                        if (!AbilityCategoryDefs.Contains(abilityCategoryDef))
                            AbilityCategoryDefs.Add(abilityCategoryDef);
                }
            }
            List<AbilityCategoryDef> abilityCategoryDefsToRemove = new List<AbilityCategoryDef>();
            foreach (AbilityCategoryDef abilityCategoryDef in AbilityCategoryDefs)
            {
                foreach (Ability ability in abilities)
                {
                    if (!abilityCategoryDefsToRemove.Contains(abilityCategoryDef) && !ability.def.abilityCategoryDefs.Contains(abilityCategoryDef))
                        abilityCategoryDefsToRemove.Add(abilityCategoryDef);
                }
            }

            foreach (AbilityCategoryDef abilityCategoryDef in abilityCategoryDefsToRemove)
            {
                if (AbilityCategoryDefs.Contains(abilityCategoryDef))
                    AbilityCategoryDefs.Remove(abilityCategoryDef);
            }
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
            CompAbilityHolder abilityHolderComp = pawn.GetComp<CompAbilityHolder>();

            GUI.BeginGroup(abilityRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Color.white;

            Rect iconRect = new Rect(0f, 0f, iconSize, iconSize);
            Widgets_Extensions.AbilityIcon(iconRect, ability);

            Rect labelRect = new Rect(iconRect.width + 2f, 0f, 120f, 32f);
            Widgets.Label(labelRect, ability.def.LabelCap);

            Rect upRect = new Rect(abilityRect.xMax - 24f - 24f - 34f, 0f, 24f, 24f);
            if (ability.gizmoOrder > 0)
            {
                if (Widgets.ButtonImage(upRect, Widgets_Extensions.reorderUpTex, Color.white))
                    abilityHolderComp.ReorderClickable(ability, -1);
            }

            Rect downRect = new Rect(abilityRect.xMax - 24f - 34f, 0f, 24f, 24f);
            if (ability.gizmoOrder != abilityHolderComp.LastGizmoNumber)
            {
                if (Widgets.ButtonImage(downRect, Widgets_Extensions.reorderDownTex, Color.white))
                    abilityHolderComp.ReorderClickable(ability, 1);
            }

            Rect deleteRect = new Rect(abilityRect.xMax - 34f, 0f, 24f, 24f); // 24f + 10f (scrollbar) = 34f
            if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, Color.white))
                abilityHolderComp.ToggleClickable(ability);

            float effectsWidth = abilityRect.width - iconRect.width - labelRect.width - upRect.width - downRect.width - deleteRect.width;
            Rect effectsRect = new Rect(labelRect.x + labelRect.width + 2f, 0f, effectsWidth, 32f);
            Widgets.Label(effectsRect, ability.def.description);

            GUI.EndGroup();

            if (Mouse.IsOver(abilityRect))
                return ability;

            return null;
        }

        private static float DoForgettingAbilities(CompAbilityHolder abilityHolderComp, Rect rect, List<Ability> forgettingAbilities)
        {
            float y = 0f;
            for (int i = 0; i < forgettingAbilities.Count; i++)
            {
                Rect abilityRect = new Rect(rect.x, y, rect.width, AbilityHeight);
                Ability hoveredAbility = DoForgettingAbilityInterface(abilityRect, forgettingAbilities[i], abilityHolderComp.parent as Pawn);

                //                if (hoveredAbility != null)
                //                    mouseHoveredAbility = hoveredAbility;

                y += AbilityHeight + AbilityVerticalGap;
            }

            return y;
        }

        private static Ability DoForgettingAbilityInterface(Rect abilityRect, Ability forgettingAbility, Pawn pawn)
        {
            CompAbilityHolder abilityHolderComp = pawn.GetComp<CompAbilityHolder>();

            GUI.BeginGroup(abilityRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Color.white;

            Rect iconRect = new Rect(0f, 0f, iconSize, iconSize);
            Widgets_Extensions.AbilityIcon(iconRect, forgettingAbility);

            Rect labelRect = new Rect(iconRect.width + 2f, 0f, 120f, 32f);
            Widgets.Label(labelRect, forgettingAbility.def.LabelCap);

            Rect timerRect = new Rect(abilityRect.xMax - 180f - 34f, 0f, 180f, 24f);
            GUI.color = forgettingAbility.remainingTicksToForget > forgettingAbility.remainingTicksToForget * 0.5 ? Color.white : forgettingAbility.remainingTicksToForget > forgettingAbility.remainingTicksToForget * 0.2 ? Color.yellow : Color.red;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Tiny;
            Widgets.Label(timerRect, GenDate.ToStringTicksToPeriod(forgettingAbility.remainingTicksToForget, true, true));
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;

            Rect deleteRect = new Rect(abilityRect.xMax - 34f, 0f, 24f, 24f); // 24f + 10f (scrollbar) = 34f
            if (Widgets.ButtonImage(deleteRect, Widgets_Extensions.deleteXTex, Color.white))
                abilityHolderComp.ToggleForgettable(forgettingAbility);

            float effectsWidth = abilityRect.width - iconRect.width - labelRect.width - timerRect.width - deleteRect.width;
            Rect effectsRect = new Rect(labelRect.x + labelRect.width + 2f, 0f, effectsWidth, 32f);
            //Widgets.Label(effectsRect, forgettingAbility.def.description);

            GUI.EndGroup();

            if (Mouse.IsOver(abilityRect))
                return forgettingAbility;

            return null;
        }

    }
}
