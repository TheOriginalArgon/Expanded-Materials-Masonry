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
    public class Zone_Digging : Zone
    {
        public override bool IsMultiselectable => false;

        public ThingDef thingToDigFor = null;
        public int drop = 0;
        private static List<Color> diggingZoneColors = new List<Color>();

        // Probably change these to lists once the debugging number value is not necessary.
        public Dictionary<ThingDef, int> resourcesInThisZone = new Dictionary<ThingDef, int>();
        public Dictionary<TerrainDef, int> terrainsInThisZone = new Dictionary<TerrainDef, int>();

        private static int nextDiggingZoneColor = 0;
        public bool allowDigging = true;
        public bool isZoneBigEnough = false;
        public bool isZonePolluted = false;
        public bool isZoneEmpty = true;
        public bool someoneDigging = false;


        protected override Color NextZoneColor => NextDiggingZoneColor();

        private static IEnumerable<Color> DiggingZoneColors()
        {
            yield return Color.Lerp(new Color(1f, 0f, 0f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0.5f, 0.5f, 0f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0.25f, 1f, 0f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0.65f, 0f, 0.25f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0.25f, 0f, 1f), Color.gray, 0.5f);
            yield break;
        }

        public static Color NextDiggingZoneColor()
        {
            diggingZoneColors.Clear();
            foreach (Color color in  DiggingZoneColors())
            {
                Color item = new Color(color.r, color.g, color.b, 0.09f);
                diggingZoneColors.Add(item);
            }
            Color result = diggingZoneColors[nextDiggingZoneColor];
            nextDiggingZoneColor++;
            if (nextDiggingZoneColor >= diggingZoneColors.Count)
            {
                nextDiggingZoneColor = 0;
            }
            return result;
        }

        public void InitializeResourcesInZone()
        {
            resourcesInThisZone = new Dictionary<ThingDef, int>();
            terrainsInThisZone = new Dictionary<TerrainDef, int>();
            isZonePolluted = false;
            if (cells.Count < 25) // CREATE SETTINGS: MINIMUM AMOUNT OF CELLS
            {
                isZoneBigEnough = false;
            }
            else
            {
                isZoneBigEnough = true;
                if (ModsConfig.BiotechActive)
                {
                    foreach (IntVec3 c in cells)
                    {
                        if (c.IsPolluted(Map))
                        {
                            isZonePolluted = true;
                            break;
                        }
                    }
                }
                for (int i = 0; i < cells.Count; i++)
                {
                    TerrainDef terrain = cells[i].GetTerrain(Map);
                    if (!terrainsInThisZone.ContainsKey(terrain))
                    {
                        terrainsInThisZone.Add(terrain, 1);
                    }
                    else
                    {
                        terrainsInThisZone[terrain]++;
                    }
                }
                foreach (TerrainDef terrain in terrainsInThisZone.Keys)
                {
                    foreach (DiggableTerrainDef item in DefDatabase<DiggableTerrainDef>.AllDefs.Where((DiggableTerrainDef t) => t.terrain == terrain))
                    {
                        foreach (ThingDefCountClass resource in item.surfaceLayerYields)
                        {
                            if (!resourcesInThisZone.ContainsKey(resource.thingDef))
                            {
                                resourcesInThisZone.Add(resource.thingDef, 1);
                            }
                            else
                            {
                                resourcesInThisZone[resource.thingDef]++;
                            }
                        }
                    }
                }
            }
        }

        public Zone_Digging() { }
        public Zone_Digging(ZoneManager zoneManager) : base("EM_DiggingZone".Translate(), zoneManager)
        {
            InitializeResourcesInZone();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref allowDigging, "allowDigging", true, false);
            Scribe_Values.Look(ref isZoneBigEnough, "isZoneBigEnough", true, false);
        }

        public override string GetInspectString()
        {
            string text = string.Empty;
            if (isZonePolluted)
            {
                text += "EM_DiggingZoneIsPolluted".Translate();
            }
            else
            {
                if (!isZoneBigEnough)
                {
                    text += "EM_DiggingZoneTooSmall".Translate(cells.Count, 25); // ADD THE SETTINGS' CORRECT VALUE.
                }
                else
                {
                    // DEBUG: This shows the resources and terrains located in each zone.
                    foreach (KeyValuePair<TerrainDef, int> kvp in terrainsInThisZone)
                    {
                        text += kvp.Key.label + " " + kvp.Value + " ";
                    }
                    text += "\n";
                    foreach (KeyValuePair<ThingDef, int> kvp in resourcesInThisZone)
                    {
                        text += kvp.Key.label + " " + kvp.Value + " ";
                    }
                }
            }
            return text;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            yield return DiggingSettingUtility.SetDiggingResourceCommand(this, Map);
            yield return new Command_Toggle
            {
                defaultLabel = "EM_CommandAllowDigging".Translate(),
                defaultDesc = "EM_CommandAllowDiggingDesc".Translate(),
                hotKey = KeyBindingDefOf.Command_ItemForbid,
                icon = ContentFinder<Texture2D>.Get("UI/Designators/EM_AllowDigging", true),
                isActive = (() => allowDigging),
                toggleAction = () =>
                {
                    allowDigging = !allowDigging;
                }
            };
            yield break;
        }

        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Digging_Expand>();
            yield break;
        }
    }
}
