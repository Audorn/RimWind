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

        public List<CharacterClassFactionModifiers> factions = new List<CharacterClassFactionModifiers>();

        //public CharacterClassDef() { thingClass = typeof(CharacterClass); }
        //public CharacterClassDef(Type thingClass) { this.thingClass = thingClass; }










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

    public class CharacterClassFactionModifiers
    {
        public List<FactionDef> factionDefs = new List<FactionDef>();
        public List<StatModRange> statMods = new List<StatModRange>();
        public List<SkillModRange> skillMods = new List<SkillModRange>();
        public List<AbilityDef> abilities = new List<AbilityDef>();
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
}
