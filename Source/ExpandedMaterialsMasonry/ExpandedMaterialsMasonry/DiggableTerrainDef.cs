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
    }
}
