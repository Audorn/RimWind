using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using RimWorld;
using Verse;

namespace RimTES
{
    class EnchantWorker
    {
        Enchantment enchantment;

        public virtual bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return pawn.Faction != billDoerFaction && enchantment.isViolation;
        }

        public virtual string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
        {
            return enchantment.ToString();
        }

        public virtual void ConsumeIngredient(Thing ingredient)
        {
            ingredient.Destroy(DestroyMode.Vanish);
        }
    }
}
