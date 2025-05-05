using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public class TerrainDefExtension_Diggable : DefModExtension
    {
        public List<ThingDefCountClass> resources;
        public ThingDef pollutedResource;
    }
}
