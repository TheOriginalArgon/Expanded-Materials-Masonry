using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ExpandedMaterialsMasonry
{
    //[StaticConstructorOnStartup]
    //public class Command_SelectDiggingResource : Command
    //{
    //    public Map map;
    //    public CompDiggingSpot compDiggingSpot;

    //    public Command_SelectDiggingResource()
    //    {
    //        defaultDesc = "EM_ChooseResourceDesc".Translate();
    //        defaultLabel = "EM_ChooseResource".Translate();

    //        foreach (object selectedObject in Find.Selector.SelectedObjects)
    //        {
    //            if (selectedObject is Building building)
    //            {
    //                compDiggingSpot = building.TryGetComp<CompDiggingSpot>();
    //                if (compDiggingSpot != null)
    //                {
    //                    icon = compDiggingSpot.selectedResource.uiIcon;
    //                }
    //            }
    //        }
    //    }

    //    public override void ProcessInput(Event ev)
    //    {
    //        base.ProcessInput(ev);
    //        List<FloatMenuOption> options = new List<FloatMenuOption>();
    //        foreach (ThingDefCountClass c in compDiggingSpot.resources)
    //        {
    //            options.Add(new FloatMenuOption("EM_ChooseMaterialToDig".Translate(c.thingDef.label), delegate
    //            {
    //                compDiggingSpot.selectedResource = c.thingDef;
    //            }, MenuOptionPriority.Default, null, null, 29f));
    //        }
    //        Find.WindowStack.Add(new FloatMenu(options));
    //    }
    //}
}
