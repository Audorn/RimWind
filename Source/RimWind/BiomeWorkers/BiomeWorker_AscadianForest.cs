using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace RimTES
{
    public class BiomeWorker_AscadianForest : BiomeWorker
    {
        public override float GetScore(Tile tile)
        {
            if (tile.WaterCovered)
                return -100f;
            if (tile.temperature < 15f)
                return 0f;
            if (tile.rainfall < 2000f)
                return 0f;

            return 28f + (tile.temperature - 20f) * 1.5f + (tile.rainfall - 600f) / 165f;
        }
    }
}
