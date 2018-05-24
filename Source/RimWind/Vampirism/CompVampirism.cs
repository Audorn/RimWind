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
    public class CompVampirism : ThingComp
    {
        public CompProperties_Vampirism Props { get { return (CompProperties_Vampirism)props; } }
    }
}
