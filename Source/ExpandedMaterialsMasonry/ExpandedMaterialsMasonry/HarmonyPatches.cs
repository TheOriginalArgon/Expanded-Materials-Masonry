using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;

namespace ExpandedMaterialsMasonry
{
    public class ExpandedMaterialsMasonry : Mod
    {
        public static Harmony harmony;
        public ExpandedMaterialsMasonry(ModContentPack content) : base(content)
        {
            harmony = new Harmony("argon.expandedmaterials.masonry");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch]
    public static class HarmonyPatch_Deconstruct
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Designator_Deconstruct), nameof(Designator_Deconstruct.CanDesignateThing))]
        private static bool CanDesignateThing_DiggingSpot(Thing t, ref AcceptanceReport __result)
        {
            CompDiggingSpot compDiggingSpot = t.TryGetComp<CompDiggingSpot>();
            if (compDiggingSpot != null)
            {
                if (compDiggingSpot.CanBeDeconstructed)
                {
                    __result = true;
                }
                else
                {
                    __result = "EM_CannotDeconstructDiggingSpot".Translate();
                }
                return false;
            }
            return true;
        }
    }
}
