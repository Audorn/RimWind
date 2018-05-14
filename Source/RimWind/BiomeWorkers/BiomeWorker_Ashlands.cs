using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace RimTES
{
    public class BiomeWorker_Ashlands : BiomeWorker
    {
        public override float GetScore(Tile tile)
        {
            if (tile.WaterCovered)
                return -100f;
            if (tile.hilliness == Hilliness.SmallHills)
                return 0f;
            if (tile.rainfall >= 600f)
                return 0f;

            return tile.temperature + 0.0001f;
        }
    }
}
