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
    public class CompAbilityHolder : ThingComp
    {
        public CompProperties_AbilityHolder Props { get { return (CompProperties_AbilityHolder)props; } }
        public List<AbilityHolderTypeChance> TypeChances { get { return Props.types; } }

        public List<Ability> EmptyAbilitiesList = new List<Ability>();
        public List<Ability> abilities = new List<Ability>();
        public List<Ability> Abilities { get { return abilities.NullOrEmpty() ? EmptyAbilitiesList : abilities; } }
        public List<Ability> ClickableAbilities { get { return abilities.FindAll(a => a.clickable == true); } }
        public List<Ability> SortedClickableAbilities { get { return ClickableAbilities.OrderBy(a => a.gizmoOrder).ToList(); } }
        public List<Ability> ForgettingAbilities { get { return abilities.FindAll(a => a.forgetting == true); } }

        public readonly int maxForgettable = 2;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            //Ability ability = AbilityMaker.MakeAbility(RimTESDefOf.FlameTouch);
            //if (TryAddAbility(ability))
            //    Log.Warning("added " + ability.ToString() + " to " + parent.LabelCap);
        }

        public bool TryAddAbility(Ability ability)
        {
            if (abilities.NullOrEmpty())
            {
                abilities.Add(ability);
                return true;
            }

            foreach (Ability heldAbility in abilities)
            {
                if (ability.def != null && heldAbility.def == ability.def)
                    return false;
            }

            abilities.Add(ability);
            return true;
        }
        public bool TryRemoveAbility(Ability ability)
        {
            if (!abilities.NullOrEmpty() && abilities.Contains(ability))
            {
                abilities.Remove(ability);
                return true;
            }

            return false;
        }
        public void ToggleClickable(Ability ability)
        {
            Ability toggledAbility = abilities.Find(a => a == ability);
            toggledAbility.clickable = !toggledAbility.clickable;

            if (toggledAbility.clickable)
            {
                toggledAbility.gizmoOrder = LastGizmoNumber + 1;
                return;
            }

            toggledAbility.gizmoOrder = -1;
        }
        public int LastGizmoNumber
        {
            get
            {
                int lastGizmo = -1;
                foreach (Ability ability in abilities)
                {
                    if (ability.gizmoOrder > lastGizmo)
                        lastGizmo = ability.gizmoOrder;
                }

                return lastGizmo;
            }
        }
        public void ToggleForgettable(Ability ability)
        {
            Ability toggledAbility = abilities.Find(a => a == ability);

            if (!toggledAbility.forgetting)
            {
                if (TotalAbilitiesBeingForgotten > maxForgettable || toggledAbility.lastAttemptedForget > 0)
                    return;

                toggledAbility.remainingTicksToForget = toggledAbility.def.ticksToForget;
            }
            else
            {
                toggledAbility.remainingTicksToForget = -1;
                toggledAbility.lastAttemptedForget = toggledAbility.def.ticksToForget;
            }

            toggledAbility.forgetting = !toggledAbility.forgetting;
        }
        public void ForgetAbility(Ability ability)
        {
            abilities.Remove(ability);
        }
        public int TotalAbilitiesBeingForgotten
        {
            get
            {
                List<Ability> abilitiesBeingForgotten = abilities.FindAll(a => a.forgetting == true);
                return abilitiesBeingForgotten.Count;
            }
        }
        public void ReorderClickable(Ability ability, int offset)
        {
            Ability reorderedAbility = abilities.Find(a => a == ability);
            int num = reorderedAbility.gizmoOrder + offset;
            foreach (Ability abilityToMove in SortedClickableAbilities)
            {
                if (abilityToMove.gizmoOrder == num)
                {
                    if (offset < 0)
                        abilityToMove.gizmoOrder++;
                    else
                        abilityToMove.gizmoOrder--;
                }
            }
            reorderedAbility.gizmoOrder = num;
        }

        public override void CompTick()
        {
            base.CompTick();

            List<Ability> abilitiesToForget = new List<Ability>();
            foreach (Ability ability in abilities)
            {
                if (ability.lastAttemptedForget > 0)
                    ability.lastAttemptedForget--;

                if (ability.forgetting)
                {
                    if (ability.remainingTicksToForget > 0)
                        ability.remainingTicksToForget--;
                    else
                        abilitiesToForget.Add(ability);
                }
            }

            foreach (Ability ability in abilitiesToForget)
                ForgetAbility(ability);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref abilities, "abilities", LookMode.Deep);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Ability ability in SortedClickableAbilities)
            {
                SoundDef commandSoundDef;
                KeyBindingDef commandHotKey;
                string commandDefaultDescKey;
                string commandIconPath;
                string commandDefaultLabelKey;

                if (ability is CustomAbility customAbility)
                {
                    commandSoundDef = customAbility.soundDef;
                    commandHotKey = customAbility.keyBindingDef;
                    commandDefaultDescKey = customAbility.defaultDescriptionKey;
                    commandIconPath = customAbility.iconPath;
                    commandDefaultLabelKey = customAbility.defaultLabelKey;
                }
                else
                {
                    commandSoundDef = ability.def.soundDef;
                    commandHotKey = ability.def.keyBindingDef;
                    commandDefaultDescKey = ability.def.defaultDescriptionKey;
                    commandIconPath = ability.def.iconPath;
                    commandDefaultLabelKey = ability.def.defaultLabelKey;
                }

                yield return new Command_Action
                {
                    action = delegate
                    {
                        commandSoundDef.PlayOneShotOnCamera(null);
                        ActivateAbility(ability);
                    },
                    hotKey = commandHotKey,
                    defaultDesc = commandDefaultDescKey.Translate(),
                    icon = commandIconPath.NullOrEmpty() ? BaseContent.BadTex : ContentFinder<Texture2D>.Get(commandIconPath, true),
                    defaultLabel = commandDefaultLabelKey.Translate()
                };
            }

            foreach (Gizmo c in base.CompGetGizmosExtra())
                yield return c;
        }

        private void ActivateAbility(Ability ability)
        {
            Log.Warning(ability.def.LabelCap + " ACTIVATED!");
        }
    }
}
