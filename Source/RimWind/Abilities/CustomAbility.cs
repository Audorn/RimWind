using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class CustomAbility : Ability
    {
        public Color IconDrawColor;
        public string texPath;
        public Texture uiIcon;
        public int ticksToForget = 0;

        public SoundDef soundDef = SoundDefOf.TickTiny;
        public KeyBindingDef keyBindingDef = KeyBindingDefOf.Misc1;
        public string defaultDescriptionKey = "DefaultDescriptionKeyNotSetForCommand";
        public string defaultLabelKey = "DefaultLabelKeyNotSetForCommand";
        public string iconPath = "";

        public int minRange = 0;
        public int maxRange = 0;



        // don't forget to add values from AbilityDef.
        public float speed = 4f;
        public bool flyOverhead = false;
        public bool alwaysFreeIntecept = false;
        public DamageDef damageDef = null;
        public int damageAmountBase = 1;
        public SoundDef soundHitThickRoof = null;
        public SoundDef soundExplode = null;
        public SoundDef soundImpactAnticipate = null;
        public SoundDef soundAmbient = null;
        public float explosionRadius = 0f;
        public int explosionDelay = 0;
        public ThingDef preExplosionSpawnThingDef = null; // summoning?
        public float preExplosionSpawnChance = 1f;
        public int preExplosionSpawnThingCount = 1;
        public ThingDef postExplosionSpawnThingDef = null; // summoning?
        public float postExplosionSpawnThingChance = 1f;
        public int postExplosionSpawnThingCount = 1;
        public bool applyDamageToExplosionCellsNeighbors = false;
        public float explosionChanceToStartFire = 0f;
        public EffecterDef explosionEffectDef = null;
        public bool ai_IsIncendiary = false;








        public CustomAbility() { }
        public CustomAbility(AbilityDef abilityDef) { def = abilityDef; }

        public override void PostMake() { }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
