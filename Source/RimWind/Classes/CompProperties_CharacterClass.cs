using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_CharacterClass : CompProperties
    {
        public List<FactionBasedCharacterClassChance> factions = new List<FactionBasedCharacterClassChance>();

        public CompProperties_CharacterClass() { compClass = typeof(CompCharacterClass); }
    }

    public class FactionBasedCharacterClassChance
    {
        public List<FactionDef> factionDefs = new List<FactionDef>();
        public List<CharacterClassChance> characterClasses = new List<CharacterClassChance>();
    }

    public class CharacterClassChance
    {
        public CharacterClassDef characterClassDef;
        public float chance = 1f;
    }
}
