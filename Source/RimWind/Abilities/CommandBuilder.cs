using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimTES
{
    public class CommandBuilder
    {
        public SoundDef soundDef = SoundDefOf.TickTiny;
        public KeyBindingDef keyBindingDef = KeyBindingDefOf.Misc1;
        public string defaultDescriptionKey = "DefaultDescriptionKeyNotSetForCommand";
        public string defaultLabelKey = "DefaultLabelKeyNotSetForCommand";
        public string iconPath = "";

        public IEnumerable<Command_Action> Click(Ability ability, CompAbilityHolder abilityHolder)
        {
            yield return new Command_Action
            {
                action = delegate
                {
                    soundDef.PlayOneShotOnCamera(null);
                    Log.Error(string.Concat(new object[]
                    {
                        abilityHolder.parent.LabelCap,
                        " is an ",
                        (abilityHolder.parent.GetComp<CompCharacterClass>() != null) ? abilityHolder.parent.GetComp<CompCharacterClass>().classRecord.def.defName : "CompCharacterClass was NULL",
                        " has ",
                        abilityHolder.abilities.Count,
                        " abilities."
                    }));

                    string tags = "";
                    string categories = "";

                    if (ability.def != null)
                    {
                        foreach (TagDef tagDef in ability.def.tags)
                            tags += tagDef.defName + ", ";

                        foreach (AbilityCategoryDef abilityCategoryDef in ability.def.abilityCategoryDefs)
                            categories += abilityCategoryDef.defName + ", ";

                        Log.Warning(string.Concat(new object[]
                        {
                            ability.def.LabelCap,
                            ", Tags: ",
                            tags,
                            " Categories: ",
                            categories
                        }));
                    }

                },
                hotKey = keyBindingDef,
                defaultDesc = defaultDescriptionKey.Translate(),
                icon = ContentFinder<Texture2D>.Get(iconPath, true),
                defaultLabel = defaultLabelKey.Translate()
            };
        }
    }
}
