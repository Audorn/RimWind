using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace RimTES
{
    class BiomeWorker_Grazelands : BiomeWorker
    {
        public override float GetScore(Tile tile)
        {
            if (tile.WaterCovered)
                return -100f;
            if (tile.temperature < -10f)
                return 0f;
            if (tile.rainfall < 600f || tile.rainfall >= 2000f)
                return 0f;

            return 22.5f + (tile.temperature - 20f) * 1.8f + (tile.rainfall - 600f) / 85f;
        }
    }
}
