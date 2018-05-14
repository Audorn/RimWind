using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

// Something about the temperature allowing it to be too high.  lower the max temp...

namespace RimTES
{
    class BiomeWorker_SolstheimForest : BiomeWorker
    {
        public override float GetScore(Tile tile)
        {
            if (tile.WaterCovered)
                return -100f;
            if (tile.temperature < -10f)
                return 0f;
            if (tile.rainfall < 600f)
                return 0f;

            return 15f;
        }
    }
}
