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

        public List<Ability> EmptyClickableAbilitiesList = new List<Ability>();
        public List<Ability> clickableAbilities = new List<Ability>();
        public List<Ability> ClickableAbilities { get { return clickableAbilities.NullOrEmpty() ? EmptyClickableAbilitiesList : clickableAbilities; } }

        public List<ForgettingAbility> EmptyForgettingAbilitiesList = new List<ForgettingAbility>();
        public List<ForgettingAbility> forgettingAbilities = new List<ForgettingAbility>();
        public List<ForgettingAbility> ForgettingAbilities { get { return forgettingAbilities.NullOrEmpty() ? EmptyForgettingAbilitiesList : forgettingAbilities; } }
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
        public bool TryAddClickable(Ability ability)
        {
            if (clickableAbilities.NullOrEmpty())
            {
                clickableAbilities.Add(ability);
                return true;
            }

            foreach (Ability clickableAbility in clickableAbilities)
            {
                if (clickableAbility.def != null && clickableAbility.def == ability.def)
                    return false;
            }

            clickableAbilities.Add(ability);
            return true;
        }
        public bool TryRemoveClickable(Ability ability)
        {
            if (!clickableAbilities.NullOrEmpty() && clickableAbilities.Contains(ability))
            {
                clickableAbilities.Remove(ability);
                return true;
            }

            return false;
        }
        public bool TryAddForgettable(Ability ability)
        {
            if (forgettingAbilities.Count >= maxForgettable)
                return false;

            if (abilities.Find(a => a == ability).lastAttemptedForget > 0)
                return false;

            if (forgettingAbilities.NullOrEmpty())
            {
                forgettingAbilities.Add(new ForgettingAbility(ability));
                return true;
            }

            foreach (ForgettingAbility forgettingAbility in forgettingAbilities)
            {
                if (forgettingAbility.ability.def != null && forgettingAbility.ability.def == ability.def)
                    return false;
            }

            forgettingAbilities.Add(new ForgettingAbility(ability));
            return true;
        }
        public bool TryRemoveForgettable(Ability ability)
        {
            if (!forgettingAbilities.NullOrEmpty())
            {
                ForgettingAbility forgettingAbility = forgettingAbilities.Find(fA => fA.ability == ability);
                if (forgettingAbility != null)
                {
                    forgettingAbilities.Remove(forgettingAbility);
                    Ability abilityToDisableForgettingOn = abilities.Find(a => a == ability);
                    if (abilityToDisableForgettingOn != null)
                        abilityToDisableForgettingOn.lastAttemptedForget = ability.def.ticksToForget;
                    return true;
                }
            }

            return false;
        }

        public void ReorderClickable(Ability ability, int offset)
        {
            int num = clickableAbilities.IndexOf(ability);
            num += offset;
            if (num >= 0)
            {
                clickableAbilities.Remove(ability);
                clickableAbilities.Insert(num, ability);
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            foreach (Ability ability in abilities)
            {
                if (ability.lastAttemptedForget > 0)
                    ability.lastAttemptedForget--;
            }

            List<ForgettingAbility> forgottenAbilities = new List<ForgettingAbility>();
            foreach (ForgettingAbility forgettingAbility in forgettingAbilities)
            {
                if (forgettingAbility.ticks <= 0)
                    forgottenAbilities.Add(forgettingAbility);
                else
                    forgettingAbility.ticks--;
            }

            foreach (ForgettingAbility forgottenAbility in forgottenAbilities)
            {
                TryRemoveForgettable(forgottenAbility.ability);
                TryRemoveAbility(forgottenAbility.ability);
                TryRemoveClickable(forgottenAbility.ability);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref abilities, "abilities", LookMode.Deep);
            Scribe_Collections.Look(ref clickableAbilities, "clickableAbilities", LookMode.Deep);
            Scribe_Collections.Look(ref forgettingAbilities, "forgettingAbilities", LookMode.Deep);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
                yield return c;

            foreach (Ability ability in clickableAbilities)
            {
                //Log.Warning("looking at ability: " + ability.def.LabelCap);
                if (ability.command != null)
                    yield return ability.command.Click(ability, this) as Gizmo;
            }
        }
    }

    public class ForgettingAbility : IExposable
    {
        public Ability ability;
        public int ticks;

        public ForgettingAbility() { }
        public ForgettingAbility(Ability ability)
        {
            this.ability = ability;
            ticks = ability is CustomAbility customAbility ? (ability as CustomAbility).ticksToForget : ability.def.ticksToForget;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref ability, "ability");
            Scribe_Values.Look(ref ticks, "ticks");
        }
    }
}
