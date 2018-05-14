using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using UnityEngine; // For Rect class.

namespace RimTES
{
    [HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard", new[] { typeof(Rect), typeof(Pawn), typeof(Action), typeof(Rect) })]
    class HarmonyPatch_CharacterCardUtility_DrawCharacterCard
    {
        public static bool Prefix(ref Rect rect, ref Pawn pawn, ref Action randomizeCallback, ref Rect creationRect)
        {
            return true;
        }
    }

}
