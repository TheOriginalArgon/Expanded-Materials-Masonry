using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace ExpandedMaterialsMasonry
{
    [StaticConstructorOnStartup]
    public class Command_SetDiggingResource : Command
    {
        public Map map;
        public Zone_Digging zone;
        public Command_SetDiggingResource()
        {
            defaultDesc = "EM_ChooseResourceDesc".Translate();
            defaultLabel = "EM_ChooseResource".Translate();
            // Add icon display
            foreach (object selectedObject in Find.Selector.SelectedObjects)
            {
                if (selectedObject is Zone_Digging zone_Digging)
                {
                    if (zone_Digging.thingToDigFor != null)
                    {
                        icon = zone_Digging.thingToDigFor.uiIcon;
                    }
                }
            }
        }

        public void InitializeZoneResources(Zone_Digging zone)
        {
            zone.InitializeResourcesInZone();
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            if (zone.isZoneBigEnough)
            {
                foreach (ThingDef material in zone.resourcesInThisZone.Keys)
                {
                    options.Add(new FloatMenuOption("EM_ChooseMaterialToDig".Translate(material.label), delegate
                    {
                        zone.thingToDigFor = material;
                        InitializeZoneResources(zone);
                    }, MenuOptionPriority.Default, null, null, 29f));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
        }
    }
}
