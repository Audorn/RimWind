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
            Log.Warning("checking for: " + ability.def.LabelCap);
            foreach (Ability a in clickableAbilities)
            {
                Log.Warning("Checking " + a.def.LabelCap + "... " + (a == ability).ToString());
            }
            if (clickableAbilities.NullOrEmpty())
                Log.Warning("No Clickable Abilities");

            if (!clickableAbilities.NullOrEmpty() && clickableAbilities.Contains(ability))
            {
                int i = clickableAbilities.IndexOf(ability);
                Log.Warning("clickable ability found at: " + i.ToString() + " (" + clickableAbilities[i].def.LabelCap + ")");
                clickableAbilities.RemoveAt(i);
                return true;
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

        public override void PostExposeData()
        {
            base.PostExposeData();
            /*
            Scribe_Defs.Look(ref ((CompProperties_StorableByDesignation)props).designationDef, "designationDef");
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultLabelKey, "defaultLabelKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultDescriptionKey, "defaultDescriptionKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).iconPath, "iconPath", "", false);
            */
            Scribe_Collections.Look(ref abilities, "abilities", LookMode.Deep);
            Scribe_Collections.Look(ref clickableAbilities, "clickableAbilities", LookMode.Deep);
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
}
