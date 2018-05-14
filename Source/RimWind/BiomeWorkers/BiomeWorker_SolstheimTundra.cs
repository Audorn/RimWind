using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace RimTES
{
    class BiomeWorker_SolstheimTundra : BiomeWorker
    {
        public override float GetScore(Tile tile)
        {
            if (tile.WaterCovered)
                return -100f;

            return -tile.temperature;
        }
    }
}
