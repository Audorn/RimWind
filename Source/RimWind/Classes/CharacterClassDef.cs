using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CharacterClassDef : ThingDef
    {
        public string iconPath = "";

        public CharacterClassModifier defaultSettings = null;
        public List<CharacterClassModifier> factions = new List<CharacterClassModifier>();

        //public CharacterClassDef() { thingClass = typeof(CharacterClass); }
        //public CharacterClassDef(Type thingClass) { this.thingClass = thingClass; }

        public int MaximumNumberOfAbilities (FactionDef factionDef)
        {
            foreach (CharacterClassModifier factionSetting in factions)
            {
                foreach (FactionDef faction in factionSetting.factionDefs)
                {
                    if (faction == factionDef && factionSetting.maximumNumberOfAbilities >= 0)
                    {
                        return factionSetting.maximumNumberOfAbilities;
                    }
                }
            }

            if (defaultSettings != null)
                return defaultSettings.maximumNumberOfAbilities;

            return -1;
        }








        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            foreach (StatDrawEntry stat in base.SpecialDisplayStats())
                yield return stat;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
                yield return error;
        }

        public override void ClearCachedData() { base.ClearCachedData(); }
        public override string ToString() { return base.ToString(); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override void ResolveReferences() { base.ResolveReferences(); }
        public override void PostLoad() { base.PostLoad(); }
    }

    public class CharacterClassModifier
    {
        public List<FactionDef> factionDefs = new List<FactionDef>();
        public List<StatModRange> statMods = new List<StatModRange>();
        public List<SkillModRange> skillMods = new List<SkillModRange>();
        public int maximumNumberOfAbilities = -1;
        public List<AbilityDef> excludedAbilities = null;
        public List<AbilityDef> abilities = new List<AbilityDef>();
        public List<AbilitySelector> numberOfAbilitiesWithTags = new List<AbilitySelector>();
        public List<AbilitySelector> numberOfAbilitiesInCategories = new List<AbilitySelector>();
    }

    public class StatModRange
    {
        public StatDef statDef;
        public float min = 0f;
        public float max = 0f;
    }

    public class SkillModRange
    {
        public SkillDef skillDef;
        public float min = 0f;
        public float max = 0f;
    }

    public class AbilitySelector
    {
        public int targetNumber = 0;
        public List<TagDef> tags = new List<TagDef>();
        public bool requireAllTags = false; // Only applies to tags.
        public List<AbilityCategoryDef> abilityCategoryDefs = new List<AbilityCategoryDef>();
    }
}
