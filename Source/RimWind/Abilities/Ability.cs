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
        public int gizmoOrder = -1;
        public bool clickable = false;
        public bool forgetting = false;
        public int remainingTicksToForget = -1;
        public int lastAttemptedForget = 0;
        public bool isCustom = false;

        public Ability() { }
        public Ability(AbilityDef abilityDef) { def = abilityDef; }

        public virtual void PostMake() { }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref lastAttemptedForget, "lastAttemptedForget", 0, false);
            Scribe_Values.Look(ref remainingTicksToForget, "remainingTicksToForget", -1, false);
            Scribe_Values.Look(ref forgetting, "forgetting", false, false);
            Scribe_Values.Look(ref gizmoOrder, "gizmoOrder", -1, false);
            Scribe_Values.Look(ref clickable, "clickable", false, false);
            Scribe_Values.Look(ref isCustom, "isCustom", false, false);
        }
    }
}
