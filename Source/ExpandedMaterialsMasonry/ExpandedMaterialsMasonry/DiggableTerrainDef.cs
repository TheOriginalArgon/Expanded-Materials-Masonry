using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public class DiggableTerrainDef : Def
    {
        public TerrainDef terrain;
        public List<ThingDefCountClass> surfaceLayerYields;
        public List<ThingDefCountClass> midLayerYields;
        public List<ThingDefCountClass> deepLayerYields;
        public List<ThingDef> pollutedYields;

        public static DiggableTerrainDef GetForTerrain(TerrainDef terrain)
        {
            return DefDatabase<DiggableTerrainDef>.AllDefs.FirstOrDefault(x => x.terrain == terrain);
        }
    }
}
