using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class Enchantment : IExposable
    {
        public EnchantmentType type = EnchantmentType.OnHit;
        public SpellSchool school = SpellSchool.Destruction;
        public Effect effect = Effect.Fire;
        public bool isViolation = false;

        public override string ToString()
        {
            return school.ToString() + ": " + effect.ToString() + " " + type.ToString();
        }
        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref type, "type", EnchantmentType.OnHit, false);
            Scribe_Values.Look(ref school, "school", SpellSchool.Destruction, false);
            Scribe_Values.Look(ref effect, "effect", Effect.Fire, false);
        }
    }

    public enum EnchantmentType
    {
        OnHit,
        OnTarget,
        OnSelf,
        ConstantEffect
    }

    public enum SpellSchool
    {
        Destruction,
        Restoration
    }

    public enum Effect
    {
        Fire,
        Ice,
        Heal,
        Cure
    }
}
