using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public class Designator_ZoneAdd_Digging : Designator_ZoneAdd
    {
        protected override string NewZoneLabel => "EM_DiggingZone".Translate();

        public Designator_ZoneAdd_Digging()
        {
            zoneTypeToPlace = typeof(Zone_Digging);
            defaultLabel = "EM_DiggingZone".Translate();
            defaultDesc = "EM_DiggingZoneDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/EM_ZoneCreate_Digging");
            hotKey = KeyBindingDefOf.Misc2;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!base.CanDesignateCell(c).Accepted)
            {
                return false;
            }
            TerrainDef terrainDef = Map.terrainGrid.TerrainAt(c);
            foreach (AllowedDiggingTerrainDef allTerrains in DefDatabase<AllowedDiggingTerrainDef>.AllDefs)
            {
                foreach (string allowedTerrain in allTerrains.allowedTerrains)
                {
                    if (allowedTerrain == terrainDef.defName && c.Walkable(Map))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override Zone MakeNewZone()
        {
            return new Zone_Digging(Find.CurrentMap.zoneManager);
        }
    }
}
