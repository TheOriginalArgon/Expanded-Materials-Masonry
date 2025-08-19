using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public class PlaceWorker_DiggingSpot : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            bool flag = false;
            foreach (DiggableTerrainDef diggableTerrainDef in DefDatabase<DiggableTerrainDef>.AllDefs)
            {
                if (map.terrainGrid.TerrainAt(loc).defName == diggableTerrainDef.terrain.defName)
                flag = true;
            }
            return flag;
        }
    }
}
