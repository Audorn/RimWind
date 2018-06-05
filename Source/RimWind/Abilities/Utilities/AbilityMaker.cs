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
        public static AbilityData MakeAbility(AbilityDef abilityDef)
        {
            AbilityData ability = (AbilityData)Activator.CreateInstance(abilityDef.thingClass);
            ability.def = abilityDef;
            ability.PostMake();
            return ability;
        }
    }
}
