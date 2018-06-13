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
    public class CompCharacterClass : ThingComp
    {
        public CompProperties_CharacterClass Props { get { return (CompProperties_CharacterClass)props; } }
        public List<CharacterClassChance> Defaults { get { return Props.defaults; } }
        public List<FactionBasedCharacterClassChance> Factions { get { return Props.factions; } }

        public Faction Faction { get { return parent.Faction; } }

        public CharacterClassRecord classRecord;

        public void SelectCharacterClass()
        {
            if (classRecord != null)
                return;

            classRecord = SelectClass();
        }

        private CharacterClassRecord SelectClass()
        {
            List<CharacterClassChance> classChances = new List<CharacterClassChance>();
            classChances = Defaults.ToList();

            foreach (FactionBasedCharacterClassChance factionChance in Factions)
            {
                foreach (FactionDef factionDef in factionChance.factionDefs)
                {
                    if (Faction.def != factionDef)
                        continue;

                    // Modify the defaults and note any additional classes
                    List<CharacterClassChance> additionalClasseChances = new List<CharacterClassChance>();
                    foreach (CharacterClassChance otherClassChance in factionChance.characterClasses)
                    {
                        bool classFound = false;
                        foreach (CharacterClassChance classChance in classChances)
                        {
                            if (classChance.characterClassDef == otherClassChance.characterClassDef)
                            {
                                classChance.chance = otherClassChance.chance;
                                classFound = true;
                                break;
                            }

                        }
                        
                        // Add the character class to a list for later.
                        if (!classFound)
                            additionalClasseChances.Add(otherClassChance);
                    }

                    // Add any character classes that weren't included in 'defaults'
                    foreach (CharacterClassChance classChance in additionalClasseChances)
                        classChances.Add(classChance);
                }
            }

            float cumulativeChances = SumChances(classChances);
            float selectionNum = UnityEngine.Random.Range(0f, cumulativeChances);

            CharacterClassDef selectedClassDef = SelectCharacterClassDef(classChances, selectionNum);
            ConfigureCharacterClass(selectedClassDef);

            return new CharacterClassRecord((Pawn)parent, selectedClassDef);
        }

        private void ConfigureCharacterClass(CharacterClassDef selectedClassDef)
        {
            // Configure from defaults.

            // Override defaults where necessary.
            bool factionFound = false;
            foreach (CharacterClassModifier factionModifiers in selectedClassDef.factions)
            {
                if (factionFound)
                    break;

                foreach (FactionDef factionDef2 in factionModifiers.factionDefs)
                {
                    if (Faction.def != factionDef2)
                        continue;

                    factionFound = true;

                    List<AbilityDef> abilities = SelectAbilities(selectedClassDef);
                    AddAbilities(abilities);
                    // Modify Stats.
                    // Modify Skills.
                }
            }
        }
        private List<AbilityDef> SelectAbilities(CharacterClassDef characterClassDef)
        {
            List<AbilityDef> abilities = new List<AbilityDef>();
            List<AbilityDef> excludedAbilities = new List<AbilityDef>();

            // Get defaults.
            if (characterClassDef.defaultSettings != null)
                abilities = SelectAbilities(characterClassDef.defaultSettings, ref excludedAbilities);

            // Get additional faction specific abilities.
            foreach (CharacterClassModifier factionSettings in characterClassDef.factions)
                abilities = SelectAbilities(factionSettings, ref excludedAbilities, abilities);

            // Only allow the maximum number of abilities.
            if (characterClassDef.MaximumNumberOfAbilities(Faction.def) < 0) // infinite.
                return abilities;

            int n = abilities.Count - characterClassDef.MaximumNumberOfAbilities(Faction.def);
            while (!abilities.NullOrEmpty() && n > 0) // limited.
            {
                abilities.RemoveLast();
                n--;
            }

            return abilities;
        }

        private List<AbilityDef> SelectAbilities(CharacterClassModifier classSettings, ref List<AbilityDef> excludedAbilities, List<AbilityDef> existingAbilities = null)
        {
            List<AbilityDef> abilities = existingAbilities;
            if (abilities.NullOrEmpty())
                abilities = new List<AbilityDef>();

            // Get excluded abilities.
            if (classSettings.excludedAbilities != null)
                excludedAbilities = classSettings.excludedAbilities;

            // Insert the guaranteed abilities.
            foreach (AbilityDef ability in classSettings.abilities)
            {
                if (excludedAbilities.NullOrEmpty() || !excludedAbilities.Contains(ability))
                    abilities.Insert(0, ability);
            }

            // Get the abilities with tags.
            foreach (AbilitySelector abilitySelector in classSettings.numberOfAbilitiesWithTags)
            {
                List<AbilityDef> abilitiesWithTags = new List<AbilityDef>();
                foreach (AbilityDef ability in DefDatabase<AbilityDef>.AllDefs)
                {
                    if (!excludedAbilities.NullOrEmpty() && excludedAbilities.Contains(ability))
                        continue;

                    if ((abilitySelector.requireAllTags && ability.HasAllTags(abilitySelector.tags))
                     || (!abilitySelector.requireAllTags && ability.HasAnyTag(abilitySelector.tags)))
                        abilitiesWithTags.Add(ability);
                }

                for (int i = 0; !abilitiesWithTags.NullOrEmpty() && i < abilitySelector.targetNumber; i++)
                {
                    AbilityDef ability = abilitiesWithTags.RandomElement();
                    abilities.Add(ability);
                    abilitiesWithTags.Remove(ability);
                }
            }

            // Get the abilities in categories.
            foreach (AbilitySelector abilitySelector in classSettings.numberOfAbilitiesInCategories)
            {
                List<AbilityDef> abilitiesInCategories = new List<AbilityDef>();
                foreach (AbilityDef ability in DefDatabase<AbilityDef>.AllDefs)
                {
                    if (!excludedAbilities.NullOrEmpty() && excludedAbilities.Contains(ability))
                        continue;

                    if (ability.InAnyOfAbilityCategories(abilitySelector.abilityCategoryDefs))
                        abilitiesInCategories.Add(ability);
                }

                for (int i = 0; !abilitiesInCategories.NullOrEmpty() && i < abilitySelector.targetNumber; i++)
                {
                    AbilityDef ability = abilitiesInCategories.RandomElement();
                    abilities.Add(ability);
                    abilitiesInCategories.Remove(ability);
                }
            }

            // Eliminate duplicates.
            abilities.RemoveDuplicates();

            return abilities;
        }

        private void AddAbilities(List<AbilityDef> abilities)
        {
            CompAbilityHolder abilityHolder = parent.GetComp<CompAbilityHolder>();
            if (abilityHolder == null)
            {
                Log.Error("AbilityHolder component not found on " + (Pawn)parent + ".");
                return;
            }

            foreach (AbilityDef abilityDef in abilities)
            {
                Ability ability = AbilityMaker.MakeAbility(abilityDef);
                if (!abilityHolder.TryAddAbility(ability))
                {
                    if (ability.def != null)
                        Log.Warning("Failed to add ability (" + ability.def.defName + ") to " + (parent as Pawn) + ".");
                }
            }
        }

        private float SumChances(List<CharacterClassChance> classChances)
        {
            float sum = 0f;
            foreach (CharacterClassChance characterClass in classChances)
                sum += characterClass.chance;

            return sum;
        }

        private CharacterClassDef SelectCharacterClassDef(List<CharacterClassChance> classChances, float num)
        {
            float currentMax = 0f;
            foreach (CharacterClassChance characterClass in classChances)
            {
                currentMax += characterClass.chance;
                if (num < currentMax)
                    return characterClass.characterClassDef;
            }

            Log.Error("Class selection number exceeded class max.  Check your math.");
            return null;
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
            
            Scribe_Deep.Look(ref classRecord, "classRecord");
        }

    }
}
