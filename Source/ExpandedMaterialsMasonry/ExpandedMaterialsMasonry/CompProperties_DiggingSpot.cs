using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ExpandedMaterialsMasonry
{
    public enum DiggingSpotState
    {
        Top,
        Mid,
        Deep
    }

    public class CompProperties_DiggingSpot : CompProperties
    {
        public CompProperties_DiggingSpot()
        {
            compClass = typeof(CompDiggingSpot);
        }
    }

    public class CompDiggingSpot : ThingComp
    {
        public static readonly Material DiggingSpot_Top = MaterialPool.MatFrom("Building/EM_DiggingSpot_a");
        public static readonly Material DiggingSpot_Mid = MaterialPool.MatFrom("Building/EM_DiggingSpot_b");
        public static readonly Material DiggingSpot_Deep = MaterialPool.MatFrom("Building/EM_DiggingSpot_c");

        public List<ThingDefCountClass> resourcesSurface;
        public List<ThingDefCountClass> resourcesMid;
        public List<ThingDefCountClass> resourcesDeep;
        public List<ThingDef> pollutedResources;

        private int progress;
        private bool exhausted = false;
        private DiggingSpotState State
        {
            get
            {
                if (progress > 500) { return DiggingSpotState.Deep; }
                if (progress > 200) { return DiggingSpotState.Mid; }
                else { return DiggingSpotState.Top; }
            }
        }

        private bool IsPolluted => parent.Map.pollutionGrid.IsPolluted(parent.Position);

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            TerrainDef terrain = parent.Map.terrainGrid.TerrainAt(parent.Position);
            DiggableTerrainDef diggableTerrainDef = DiggableTerrainDef.GetForTerrain(terrain);
            if (diggableTerrainDef != null)
            {
                resourcesSurface = diggableTerrainDef.surfaceLayerYields;
                resourcesMid = diggableTerrainDef.midLayerYields;
                resourcesDeep = diggableTerrainDef.deepLayerYields;
                pollutedResources = diggableTerrainDef.pollutedYields;
            }
            else
            {
                Log.Error("No DiggableTerrainDef found for terrain: " + terrain.defName);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (parent.Spawned)
            {
                var sb = new StringBuilder();
                sb.Append("Progress: ").Append(progress).AppendLine()
                  .Append("Layer: ").Append(State.ToString());
                if (IsPolluted)
                {
                    sb.Append("Terrain is polluted.");
                }
                return sb.ToString();
            }
            return null;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref progress, "progress", 0, true);
        }

        public bool CanDig()
        {
            if (exhausted)
            {
                return false;
            }
            return true;
        }

        //public override IEnumerable<Gizmo> CompGetGizmosExtra()
        //{
        //    foreach (Gizmo item in base.CompGetGizmosExtra()) { yield return item; }
        //    yield return DiggingSpotUtility.SetDiggingResource(this, parent.Map);
        //}

        public override void CompTickRare()
        {
            if (exhausted)
            {
                int num = 0;
                if (parent.Map.weatherManager.RainRate > 0) { num = 5; }
                progress -= 5 + num;
                if (progress < 200)
                {
                    exhausted = false;
                }
            }
            else
            {
                if (progress > 600)
                {
                    exhausted = true;
                }
            }

            parent.DirtyMapMesh(parent.MapHeld);
        }

        //public override void PostDraw()
        //{
        //    //base.PostDraw();
        //    Material mat = parent.Graphic.MatSingle;
        //    Texture2D tex = null;
        //    switch (State)
        //    {
        //        case DiggingSpotState.Deep:
        //            tex = ContentFinder<Texture2D>.Get("Building/EM_DiggingSpot_c");
        //            break;
        //        case DiggingSpotState.Mid:
        //            tex = ContentFinder<Texture2D>.Get("Building/EM_DiggingSpot_b");
        //            break;
        //        case DiggingSpotState.Top:
        //            tex = ContentFinder<Texture2D>.Get("Building/EM_DiggingSpot_a");
        //            break;
        //    }

        //    mat.SetTexture("_MainTex", tex);
        //}

        public override void PostPrintOnto(SectionLayer layer)
        {
            if (parent.def.drawerType != DrawerType.RealtimeOnly)
            {
                Vector3 baseDrawPos = parent.DrawPos;
                baseDrawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor();

                switch (State)
                {
                    case DiggingSpotState.Deep:
                        Printer_Plane.PrintPlane(layer, baseDrawPos, new Vector2(1.5f, 1.5f), DiggingSpot_Deep);
                        break;
                    case DiggingSpotState.Mid:
                        Printer_Plane.PrintPlane(layer, baseDrawPos, new Vector2(1.5f, 1.5f), DiggingSpot_Mid);
                        break;
                    case DiggingSpotState.Top:
                        Printer_Plane.PrintPlane(layer, baseDrawPos, new Vector2(1.5f, 1.5f), DiggingSpot_Top);
                        break;
                }
            }
        }

        private List<ThingDefCountClass> GetCurrentLayerYields()
        {
            switch (State)
            {
                case DiggingSpotState.Deep:
                    return resourcesDeep;
                case DiggingSpotState.Mid:
                    return resourcesMid;
                case DiggingSpotState.Top:
                default:
                    return resourcesSurface;
            }
        }

        private Thing ProcessYield(ThingDef resource)
        {
            if (IsPolluted)
            {
                if (Rand.Chance(0.5f))
                {
                    return ThingMaker.MakeThing(pollutedResources.RandomElement());
                }
            }
            return ThingMaker.MakeThing(resource);
        }

        public void YieldResource(Pawn digger)
        {
            // Produce the resource.
            List<ThingDefCountClass> currentYields = GetCurrentLayerYields();
            foreach (ThingDefCountClass t in currentYields)
            {
                int baseAmount = t.count;
                int extraDrop = (int)((2 + digger.skills.GetSkill(SkillDefOf.Mining).Level) * (digger.GetStatValue(StatDefOf.MiningYield) / 10000f));
                int stackCount = baseAmount + extraDrop;
                Thing res = ProcessYield(t.thingDef);
                res.stackCount = stackCount;
                GenPlace.TryPlaceThing(res, parent.InteractionCell, parent.Map, ThingPlaceMode.Near, null, (IntVec3 p) => p != parent.Position && p != parent.InteractionCell);
                progress += baseAmount + (extraDrop / 2);
            }
        }
    }
}
