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
        public List<FactionBasedCharacterClassChance> Factions { get { return Props.factions; } }

        public Faction Faction { get { return parent.Faction; } }

        public CharacterClassRecord classRecord;

        public void SelectCharacterClass()
        {
            if (classRecord != null)
                return;

            classRecord = GenerateClass();
        }

        private CharacterClassRecord GenerateClass()
        {
            foreach (FactionBasedCharacterClassChance factionChance in Factions)
            {
                foreach (FactionDef factionDef in factionChance.factionDefs)
                {
                    if (Faction.def != factionDef)
                        continue;

                    float cumulativeChances = SumChancesWithinFaction(factionChance);
                    float selectionNum = UnityEngine.Random.Range(0f, cumulativeChances);

                    CharacterClassDef selectedClassDef = SelectCharacterClassDefWithinFaction(factionChance, selectionNum);
                    ConfigureCharacterClass(selectedClassDef);
                    return new CharacterClassRecord((Pawn)parent, selectedClassDef);
                }
            }

            Log.Error("No character class selected.");
            return null;
        }

        private void ConfigureCharacterClass(CharacterClassDef selectedClassDef)
        {
            bool factionFound = false;
            foreach (CharacterClassFactionModifiers factionModifiers in selectedClassDef.factions)
            {
                if (factionFound)
                    break;

                foreach (FactionDef factionDef2 in factionModifiers.factionDefs)
                {
                    if (Faction.def != factionDef2)
                        continue;

                    factionFound = true;
                    AddAbilities(factionModifiers.abilities);
                    // Modify Stats.
                    // Modify Skills.
                }
            }
        }

        private void AddAbilities(List<AbilityDef> abilities)
        {
            CompAbilityHolder abilityHolder = parent.GetComp<CompAbilityHolder>();
            if (abilityHolder == null)
                return; 

            foreach (AbilityDef abilityDef in abilities)
            {
                Ability ability = AbilityMaker.MakeAbility(abilityDef);
                if (!abilityHolder.TryAddAbility(ability))
                    Log.Warning("Failed to add ability (" + ability.def.defName + ") to " + (Pawn)parent + ".");
            }
        }

        private float SumChancesWithinFaction(FactionBasedCharacterClassChance factionChance)
        {
            float sum = 0f;
            foreach (CharacterClassChance characterClass in factionChance.characterClasses)
                sum += characterClass.chance;

            return sum;
        }

        private CharacterClassDef SelectCharacterClassDefWithinFaction(FactionBasedCharacterClassChance factionChance, float num)
        {
            float currentMax = 0f;
            foreach (CharacterClassChance characterClass in factionChance.characterClasses)
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
            
            Scribe_Deep.Look(ref classRecord, "classRecord", false);
        }

    }
}
