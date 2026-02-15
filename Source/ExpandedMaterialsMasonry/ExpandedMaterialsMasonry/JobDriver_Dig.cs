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
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
            this.FailOn(() => !job.targetA.Thing.TryGetComp<CompDiggingSpot>().CanDig());
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil work = ToilMaker.MakeToil("MakeNewToils");
            work.tickAction = delegate
            {
                pawn?.skills?.Learn(SkillDefOf.Mining, 0.015f);
            };
            work.defaultCompleteMode = ToilCompleteMode.Delay;
            work.WithEffect(EffecterDefOf.ConstructDirt, TargetIndex.A);
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            work.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            work.activeSkill = () => SkillDefOf.Mining;
            work.defaultDuration = (int)(2200 - (45 * pawn?.skills?.GetSkill(SkillDefOf.Mining).Level));
            yield return work.WithProgressBarToilDelay(TargetIndex.A, true);
            yield return new Toil
            {
                initAction = delegate
                {
                    ((Building)pawn?.CurJob?.targetA.Thing).GetComp<CompDiggingSpot>().YieldResource(pawn);
                }
            };
        }
    }
}
