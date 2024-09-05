using HarmonyLib;
using ScriptableObjectsVariables;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(BuildingMouseMovement))]
    internal class BuildingMouseMovementPatch
    {
        private static DateTime nextUpdate = DateTime.Now;
        private static float updateDelay = 5f;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(BuildingMouseMovement __instance)
        {
            if (DateTime.Now < nextUpdate)
                return;

            nextUpdate = DateTime.Now.AddSeconds(updateDelay);

            if (SDBuildMenu.selectedPart == null)
                SDBuildMenu.selectedPart = Part.MakePart(__instance.PartViewInHand.Type);
            else if (__instance.PartViewInHand.Type != SDBuildMenu.selectedPart.PartType)
                SDBuildMenu.selectedPart = Part.MakePart(__instance.PartViewInHand.Type);
        }
    }
}
