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
    internal class JobDriver_Dig : JobDriver
    {
        public float pawnMiningSkill = 10f;
        public ThingDef digThing = null;
        public int digAmount = 1;
        public float diggingSkill = 1f;
        public float skillGainperTick = 0.012f;
        private Zone_Digging diggingZone;
        private Random rand = new Random();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            diggingZone = Map.zoneManager.ZoneAt(base.job.targetA.Cell) as Zone_Digging;
            // < Select a list of items to dig?
            digThing = diggingZone.thingToDigFor;
            // < With more mining skill, speed is increased, not yield.
            Pawn p = pawn;
            LocalTargetInfo targetA = base.job.targetA;
            Job job = base.job;
            if (p.Reserve(targetA, job, 1, -1, null, errorOnFailed))
            {
                p = pawn;
                targetA = base.job.targetA.Cell;
                job = base.job;
                for (int i = 0; i < diggingZone.cells.Count; i++)
                {
                    LocalTargetInfo target = diggingZone.cells[i];
                    p.Reserve(target, job, 1, -1, null, errorOnFailed);
                }
                return p.Reserve(targetA, job, 1, -1, null, errorOnFailed);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (digThing == null)
            {
                EndJobWith(JobCondition.Incompletable);
            }
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
            pawn?.rotationTracker?.FaceTarget(TargetA);
            Toil digToil = new Toil();
            Pawn obj = pawn;
            digToil.tickAction = delegate
            {
                pawn?.skills?.Learn(SkillDefOf.Mining, skillGainperTick);
                if (diggingZone != null && !diggingZone.isZoneBigEnough)
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            Rot4 pawnRotation = pawn.Rotation;
            IntVec3 facingCell = pawnRotation.FacingCell;
            //EffecterDef effecter... (Add effecter mote) TODO
            digToil.defaultCompleteMode = ToilCompleteMode.Delay;
            int duration = 4000; // TODO: Formula based on skill.
            digToil.defaultDuration = duration; // TODO: Formula based on skill. (Same as above)
            digToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return digToil.WithProgressBarToilDelay(TargetIndex.A, false);
            yield return new Toil
            {
                initAction = delegate
                {
                    Thing thing = ThingMaker.MakeThing(digThing);
                    thing.stackCount = 7; // Here is where I should calculate based on the custom Defs.
                    GenSpawn.Spawn(thing, pawn.Position + pawnRotation.Opposite.FacingCell, Map);

                    // This will probably need to be removed since it doesn't seem to be the way I want my digging job to work.
                    StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
                    if (StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, Map, currentPriority, pawn.Faction, out IntVec3 foundCell))
                    {
                        job.SetTarget(TargetIndex.C, foundCell);
                        job.SetTarget(TargetIndex.B, thing);
                        job.count = thing.stackCount;
                    }
                    else
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            // Here come settings to toggle on/off dropping.
        }
    }
}
