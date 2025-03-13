using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace ExpandedMaterialsMasonry
{
    internal class WorkGiver_Dig : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            Danger maxDanger = pawn.NormalMaxDanger();
            List<Zone> zonesList = pawn.Map.zoneManager.AllZones;
            for (int i = 0; i < zonesList.Count; i++)
            {
                if (!(zonesList[i] is Zone_Digging diggingZone))
                {
                    continue;
                }
                if (diggingZone.cells.Count == 0)
                {
                    Log.ErrorOnce("Digging zone has 0 cells (this indicates a big error taking place, please report to Argón immediately): " + diggingZone, -563487);
                }
                if (!diggingZone.someoneDigging && diggingZone.allowDigging && !diggingZone.isZonePolluted && diggingZone.isZoneBigEnough && !diggingZone.ContainsStaticFire && diggingZone.thingToDigFor != null && pawn.CanReserveAndReach(diggingZone.cells[0], PathEndMode.OnCell, maxDanger))
                {
                    for (int j = 0; j < diggingZone.cells.Count; j++)
                    {
                        yield return diggingZone.cells[j];
                    }
                }
            }
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 cell, bool forced = false)
        {
            LocalTargetInfo target = cell;
            if (!pawn.CanReserve(target))
            {
                return null;
            }
            return new Job(DefDatabase<JobDef>.GetNamed("EM_DigJob"), cell);
        }
    }
}
