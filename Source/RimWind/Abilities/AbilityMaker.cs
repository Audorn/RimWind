using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public static class AbilityMaker
    {
        public static Ability MakeAbility(AbilityDef abilityDef)
        {
            Ability ability = (Ability)Activator.CreateInstance(abilityDef.thingClass);
            ability.def = abilityDef;
            ability.PostMake();
            return ability;
        }

        public static Ability MakeAbility(AbilityData abilityData)
        {
            Ability ability = new Ability(abilityData) { def = null };
            ability.PostMake();
            return ability;
        }
    }
}
