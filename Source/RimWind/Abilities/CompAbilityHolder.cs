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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            Ability ability = AbilityMaker.MakeAbility(RimTESDefOf.FlameTouch);
            if (TryAddAbility(ability))
                Log.Warning("added " + ability.ToString() + " to " + parent.LabelCap);
        }

        public Ability MakeAbility(AbilityData abilityData)
        {
            return new Ability(abilityData);
        }

        public bool TryAddAbility(Ability ability)
        {
            if (abilities.NullOrEmpty() || !abilities.Contains(ability))
            {
                abilities.Add(ability);
                return true;
            }

            return false;
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

        public override void PostExposeData()
        {
            base.PostExposeData();
            /*
            Scribe_Defs.Look(ref ((CompProperties_StorableByDesignation)props).designationDef, "designationDef");
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultLabelKey, "defaultLabelKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultDescriptionKey, "defaultDescriptionKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).iconPath, "iconPath", "", false);
            */
            Scribe_Collections.Look(ref abilities, "abilities", LookMode.Deep, false);
        }
        // ==========
        public string DefaultLabelKey { get { return Props.defaultLabelKey; } }
        public string DefaultDescriptionKey { get { return Props.defaultDescriptionKey; } }
        public string IconPath { get { return Props.iconPath; } }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
                yield return c;

            yield return new Command_Action
            {
                action = delegate
                {
                    SoundDefOf.TickTiny.PlayOneShotOnCamera(null);
                    Log.Error(string.Concat(new object[]
                    {
                        parent.LabelCap,
                        " has ",
                        abilities.Count,
                        " abilities.  They are: ",
                        abilities
                    }));
                    
                },
                hotKey = KeyBindingDefOf.Misc1,
                defaultDesc = DefaultDescriptionKey.Translate(),
                icon = ContentFinder<Texture2D>.Get(IconPath, true),
                defaultLabel = DefaultLabelKey.Translate()
            };
        }
        // ==========
    }
}
