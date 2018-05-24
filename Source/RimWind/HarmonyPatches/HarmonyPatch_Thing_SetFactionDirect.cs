using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;
using Verse;
using RimWorld;

namespace RimTES
{
    [HarmonyPatch(typeof(Thing), "SetFactionDirect", new[] { typeof(Faction) })]
    public class HarmonyPatch_Thing_SetFactionDirect
    {
        public static void Postfix(Thing __instance)
        {
            Pawn pawn = (Pawn)__instance;
            if (pawn != null && pawn.def.CanHaveFaction)
            {
                CompCharacterClass characterClassComp = pawn.GetComp<CompCharacterClass>();

                if (characterClassComp != null)
                    characterClassComp.SelectCharacterClass();
            }
        }

    }
}
