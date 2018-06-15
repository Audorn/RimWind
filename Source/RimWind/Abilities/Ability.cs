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
    public class Ability : IExposable
    {
        public AbilityDef def;
        public int lastAttemptedForget = 0;

        public CommandBuilder command = null;

        public Ability() { }
        public Ability(AbilityDef abilityDef) { def = abilityDef; }

        public virtual void PostMake() { }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref command, "command", null, false);
            Scribe_Values.Look(ref lastAttemptedForget, "lastAttemptedForget", 0, false);
        }
    }
}
