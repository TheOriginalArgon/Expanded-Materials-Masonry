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
        public ThingDef selectedResource;
        public List<ThingDefCountClass> resources;

        private TerrainDefExtension_Diggable extension;
        private float totalResourcePct = 0;
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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            extension = parent.Map.terrainGrid.TerrainAt(parent.Position).GetModExtension<TerrainDefExtension_Diggable>();
            if (extension != null)
            {
                resources = extension.resources;
            }
            else
            {
                Log.Error("CompDiggingSpot tried to fetch terrain resources from a terrain without the proper DefExtension");
            }

            for (int i = 0; i < resources.Count; i++)
            {
                ThingDefCountClass resource = resources[i];
                totalResourcePct += resource.count;
            }
            if (!respawningAfterLoad)
            {
                selectedResource = resources[0].thingDef;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (parent.Spawned)
            {
                string s = "Resources:\n";
                for (int i = 0; i < resources.Count; i++)
                {
                    ThingDefCountClass countClass = resources[i];
                    s += countClass.thingDef.label + ": " + (countClass.count / totalResourcePct).ToStringPercent("F0");
                    if (i < resources.Count - 1)
                    {
                        s += "\n";
                    }
                }
                s+= "Progress: " + progress + "\n";
                s+= "State: " + State.ToString();
                return s;
            }
            return null;
        }

        public override void PostExposeData()
        {
            Scribe_Defs.Look(ref selectedResource, "selectedResource");
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

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra()) { yield return item; }
            yield return DiggingSpotUtility.SetDiggingResource(this, parent.Map);
        }

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
        }

        public override void PostDraw()
        {
            //base.PostDraw();
            Material mat = parent.Graphic.MatSingle;
            Texture2D tex = null;
            switch (State)
            {
                case DiggingSpotState.Deep:
                    tex = ContentFinder<Texture2D>.Get("Building/EM_DiggingSpot_c");
                    break;
                case DiggingSpotState.Mid:
                    tex = ContentFinder<Texture2D>.Get("Building/EM_DiggingSpot_b");
                    break;
                case DiggingSpotState.Top:
                    tex = ContentFinder<Texture2D>.Get("Building/EM_DiggingSpot_a");
                    break;
            }

            mat.SetTexture("_MainTex", tex);
        }

        public void YieldResource(Pawn digger)
        {
            // Produce the resource.
            int baseAmount = resources.Where(x => x.thingDef == selectedResource).FirstOrDefault().count;
            int extraDrop = (int)((2 + digger.skills.GetSkill(SkillDefOf.Mining).Level) * (digger.GetStatValue(StatDefOf.MiningYield) / 10000f));
            int stackCount = baseAmount + extraDrop;
            Thing res = ThingMaker.MakeThing(selectedResource);
            res.stackCount = stackCount;
            GenPlace.TryPlaceThing(res, parent.InteractionCell, parent.Map, ThingPlaceMode.Near, null, (IntVec3 p) => p != parent.Position && p != parent.InteractionCell);
            progress += baseAmount + (extraDrop / 2);
        }
    }
}
