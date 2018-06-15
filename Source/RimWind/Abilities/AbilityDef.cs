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

        public AbilityDef() { thingClass = typeof(Ability); }
        public AbilityDef(Type thingClass) { this.thingClass = thingClass; }

        public CommandBuilder command = null;
        public int ticksToForget = 0;




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
