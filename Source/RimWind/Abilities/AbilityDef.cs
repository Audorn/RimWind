using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public class AbilityDef : ThingDef
    {
        public List<AbilityCategoryDef> abilityCategoryDefs = new List<AbilityCategoryDef>();
        public List<TagDef> tags = new List<TagDef>();
        public string texPath = "";
        public new Texture2D uiIcon = BaseContent.BadTex; // When remove inheriting from ThingDef remove 'new'
        public int ticksToForget = 0;

        public AbilityDef() { thingClass = typeof(Ability); }
        public AbilityDef(Type thingClass) { this.thingClass = thingClass; }

        public SoundDef soundDef = SoundDefOf.TickTiny;
        public KeyBindingDef keyBindingDef = KeyBindingDefOf.Misc1;
        public string defaultDescriptionKey { get { return defName + "Description"; } }
        public string defaultLabelKey { get { return defName + "Label"; } }

        public string iconPath = "";

        public int minRange = 0;
        public int maxRange = 0;


        // Verb_LaunchProjectile allows player selection of target.
        public float speed = 4f;
        public bool flyOverhead = false;
        public bool alwaysFreeIntercept = false;
        public DamageDef damageDef = DamageDefOf.Bullet;
        public int damageAmountBase = 1;
        public SoundDef soundHitThickRoof = SoundDefOf.BulletImpactMetal;
        public SoundDef soundExplode = SoundDefOf.BulletImpactMetal;
        public SoundDef soundImpactAnticipate = SoundDefOf.AmountDecrement;
        public SoundDef soundAmbient = SoundDefOf.AmbientSpace;
        public float explosionRadius = 0f;
        public int explosionDelay = 0;
        public ThingDef preExplosionSpawnThingDef = ThingDefOf.Silver; // summoning?
        public float preExplosionSpawnChance = 1f;
        public int preExplosionSpawnThingCount = 1;
        public ThingDef postExplosionSpawnThingDef = ThingDefOf.Gold; // summoning?
        public float postExplosionSpawnThingChance = 1f;
        public int postExplosionSpawnThingCount = 1;
        public bool applyDamageToExplosionCellsNeighbors = false;
        public float explosionChanceToStartFire = 0f;
        public EffecterDef explosionEffectDef = EffecterDefOf.ArmorDeflect;
        public bool ai_IsIncendiary = false;






        public bool HasTag(TagDef tag) { return tags.Contains(tag); }
        public bool HasAnyTag(List<TagDef> tags)
        {
            foreach (TagDef tag in tags)
            {
                if (HasTag(tag))
                    return true;
            }

            return false;
        }
        public bool HasAllTags(List<TagDef> tags)
        {
            foreach (TagDef tag in tags)
            {
                if (!HasTag(tag))
                    return false;
            }

            return true;
        }

        public bool InAbilityCategory(AbilityCategoryDef abilityCategoryDef) { return abilityCategoryDefs.Contains(abilityCategoryDef); }
        public bool InAnyOfAbilityCategories(List<AbilityCategoryDef> abilityCategoryDefs)
        {
            foreach (AbilityCategoryDef abilityCategoryDef in abilityCategoryDefs)
            {
                if (InAbilityCategory(abilityCategoryDef))
                    return true;
            }

            return false;
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            foreach (StatDrawEntry stat in base.SpecialDisplayStats())
                yield return stat;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
                yield return error;
        }

        public override void ClearCachedData() { base.ClearCachedData(); }
        public override string ToString() { return base.ToString(); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override void ResolveReferences() { base.ResolveReferences(); }

        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (!texPath.NullOrEmpty())
                    uiIcon = ContentFinder<Texture2D>.Get(texPath, true);
            });
        }
    }
}
