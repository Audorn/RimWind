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

        public List<AbilityData> EmptyAbilitiesList = new List<AbilityData>();
        public List<AbilityData> abilities = new List<AbilityData>();
        public List<AbilityData> Abilities { get { return abilities.NullOrEmpty() ? EmptyAbilitiesList : abilities; } }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            //Ability ability = AbilityMaker.MakeAbility(RimTESDefOf.FlameTouch);
            //if (TryAddAbility(ability))
            //    Log.Warning("added " + ability.ToString() + " to " + parent.LabelCap);
        }

        public bool TryAddAbility(AbilityData ability)
        {
            if (abilities.NullOrEmpty())
            {
                abilities.Add(ability);
                return true;
            }

            foreach (AbilityData heldAbility in abilities)
            {
                if (ability.def != null && heldAbility.def == ability.def)
                    return false;
            }

            abilities.Add(ability);
            return true;
        }

        public bool TryRemoveAbility(AbilityData ability)
        {
            if (!abilities.NullOrEmpty() && abilities.Contains(ability))
            {
                abilities.Remove(ability);
                return true;
            }

            return false;
        }

        public void ExposeData()
        {
            /*
            Scribe_Defs.Look(ref ((CompProperties_StorableByDesignation)props).designationDef, "designationDef");
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultLabelKey, "defaultLabelKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).defaultDescriptionKey, "defaultDescriptionKey", "", false);
            Scribe_Values.Look(ref ((CompProperties_StorableByDesignation)props).iconPath, "iconPath", "", false);
            */
            Scribe_Collections.Look(ref abilities, "abilities", LookMode.Deep);
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
                        " is an ",
                        (parent.GetComp<CompCharacterClass>() != null) ? parent.GetComp<CompCharacterClass>().classRecord.def.defName : "CompCharacterClass was NULL",
                        " has ",
                        abilities.Count,
                        " abilities."
                    }));
                    foreach (AbilityData ability in abilities)
                    {
                        string tags = "";
                        string categories = "";

                        if (ability.def != null)
                        {
                            foreach (TagDef tagDef in ability.def.tags)
                                tags += tagDef.defName + ", ";

                            foreach (AbilityCategoryDef abilityCategoryDef in ability.def.abilityCategoryDefs)
                                categories += abilityCategoryDef.defName + ", ";

                            Log.Warning(string.Concat(new object[]
                            {
                            ability.def.LabelCap,
                            ", Tags: ",
                            tags,
                            " Categories: ",
                            categories
                            }));
                        }
                    }
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
