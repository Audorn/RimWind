// Jeremy Anderson
// MainTabWindow_SoulGems.cs
// 2018-05-14

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimTES
{
    class MainTabWindow_SoulGems : MainTabWindow_PawnTable
    {
        protected override PawnTableDef PawnTableDef => DefDatabase<PawnTableDef>.GetNamed("SoulGems");
        public override void PostOpen()
        {
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }
    }
}
