using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace RimTES
{
    public interface IBillGiver_Enchant
    {
        Map Map { get; }
        BillStack_Enchant BillStack { get; }
        bool CurrentlyUsableForBills();
    }
}
