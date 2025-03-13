using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public class DiggingMapComponent : MapComponent
    {
        public int digTickProgress = 0;
        public int digTickMax = 1200;

        public DiggingMapComponent(Map map) : base(map) { }

        public override void MapComponentTick()
        {
            if (digTickProgress >= digTickMax)
            {
                foreach (Zone allZone in map.zoneManager.AllZones)
                {
                    if (allZone is Zone_Digging zone_Digging)
                    {
                        zone_Digging.InitializeResourcesInZone();
                    }
                }
                digTickProgress = 0;
            }
            digTickProgress++;
        }

    }
}
