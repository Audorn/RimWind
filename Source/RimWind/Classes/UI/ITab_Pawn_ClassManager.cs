using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimTES
{ 
    public class ITab_Pawn_ClassManager : ITab
    {
        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                if (SelPawn != null)
                    pawn = SelPawn;
                else
                {
                    Corpse corpse = SelThing as Corpse;
                    if (corpse != null)
                        pawn = corpse.InnerPawn;
                }

                if (pawn == null)
                {
                    Log.Error("Class Manager tab found no selected pawn to display.");
                    return null;
                }

                return pawn;
            }
        }
        public override bool IsVisible { get { return PawnToShowInfoAbout != null; } }
        public CompAbilityHolder AbilityHolderComp
        {
            get
            {
                if (PawnToShowInfoAbout == null)
                    return null;

                return PawnToShowInfoAbout.GetComp<CompAbilityHolder>();
            }
        }

        public ITab_Pawn_ClassManager()
        {
            size = CharacterCardUtility.PawnCardSize + new Vector2(17f, 17f) * 2f;
            labelKey = "TabClassManager";
            tutorTag = "ClassManager";
        }

        public bool birthSignVisible = false;
        public bool abilityManagerVisible = false;
        protected override void FillTab()
        {
            Rect classCardRect = new Rect(17f, 17f, CharacterCardUtility.PawnCardSize.x, CharacterCardUtility.PawnCardSize.y);

            Rect outRect = new Rect(0f, 31f, size.x, size.y - 31f);
            CharacterClassCardUtility.DrawClassSummary(outRect, PawnToShowInfoAbout);
        }
    }
}
