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
    public class CompTherianthropy : ThingComp
    {
        public CompProperties_Therianthropy Props { get { return (CompProperties_Therianthropy)props; } }
    }
}
