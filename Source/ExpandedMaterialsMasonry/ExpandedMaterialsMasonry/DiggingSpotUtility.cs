using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public static class DiggingSpotUtility
    {
        public static Command_SelectDiggingResource SetDiggingResource(CompDiggingSpot passingComp, Map passingMap)
        {
            return new Command_SelectDiggingResource
            {
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                compDiggingSpot = passingComp,
            };
        }
    }
}
