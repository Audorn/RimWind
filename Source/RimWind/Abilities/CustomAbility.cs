using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CustomAbility : Ability
    {
        public Color IconDrawColor;
        public string texPath;
        public Texture uiIcon;
        public int ticksToForget = 0;




        public CustomAbility() { }
        public CustomAbility(AbilityDef abilityDef) { def = abilityDef; }

        public override void PostMake() { }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
