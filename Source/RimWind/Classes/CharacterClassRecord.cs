using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;


namespace RimTES
{
    public class CharacterClassRecord : IExposable
    {
        private Pawn pawn;
        public CharacterClassDef def;
        private int levelInt;
        public int Level { get { return levelInt; } set { levelInt = value; } }
        public float xpSinceLastLevel;
        public float xpSinceMidnight;
        public int thingIDNumber = -1;

        public const int IntervalTicks = 200;
        public const int MinLevel = 0;
        public const int MaxLevel = 20;
        public const int MaxFullRateXpPerDay = 4000;
        public const int MasterClassThreshold = 14;
        public const float SaturatedLearningFactor = 0.2f;

        public string ThingID
        {
            get { return (def.HasThingIDNumber) ? def.defName + thingIDNumber.ToString() : def.defName; }
            set { thingIDNumber = Thing.IDNumberFromThingID(value); }
        }

        private static readonly SimpleCurve XpForLevelUpCurve = new SimpleCurve
        {
            { new CurvePoint(0f, 1000f), true },
            { new CurvePoint(9f, 10000f), true },
            { new CurvePoint(19f, 30000f), true }
        };

        public static float XpRequiredToLevelUpFrom(int startingLevel) { return XpForLevelUpCurve.Evaluate(startingLevel); }
        public float XpRequiredForLevelUp { get { return XpRequiredToLevelUpFrom(levelInt); } }

        public CharacterClassRecord() { }
        public CharacterClassRecord(Pawn pawn) { this.pawn = pawn; }
        public CharacterClassRecord(Pawn pawn, CharacterClassDef def)
        {
            this.pawn = pawn;
            this.def = def;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            if (def.HasThingIDNumber)
            {
                string thingID = ThingID;
                Scribe_Values.Look(ref thingID, "id", null, false);
                ThingID = thingID;
            }
            Scribe_Values.Look(ref levelInt, "level", 0, false);
            Scribe_Values.Look(ref xpSinceLastLevel, "xpSinceLastLevel", 0f, false);
            Scribe_Values.Look(ref xpSinceMidnight, "xpSinceMidnight", 0f, false);
        }
    }
}
