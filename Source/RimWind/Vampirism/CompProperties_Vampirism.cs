using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CompProperties_Vampirism : CompProperties
    {
        public CompProperties_Vampirism()
        {
            compClass = typeof(CompVampirism);
        }
    }
}
