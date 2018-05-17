using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTES
{
    class CompProperties_StorableByDesignation : CompProperties
    {
        public DesignationDef designationDef = null;
        public string defaultLabelKey = "";
        public string defaultDescriptionKey = "";
        public string iconPath = "";

        public CompProperties_StorableByDesignation()
        {
            compClass = typeof(CompStorableByDesignation);
        }
    }
}
