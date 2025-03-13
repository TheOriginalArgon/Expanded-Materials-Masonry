using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public static class DiggingSettingUtility
    {
        public static Command_SetDiggingResource SetDiggingResourceCommand(Zone_Digging passingZone, Map passingMap)
        {
            return new Command_SetDiggingResource
            {
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                zone = passingZone,
            };
        }
    }
}
