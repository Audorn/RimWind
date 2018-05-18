using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public interface IBillGiverWithTickAction_Enchant : IBillGiver_Enchant
    {
        void UsedThisTick();
    }
}
