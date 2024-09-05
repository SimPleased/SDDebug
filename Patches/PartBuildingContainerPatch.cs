using HarmonyLib;
using SDDebug.MenuGUI;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Linq;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(PartBuildingContainer))]
    internal class PartBuildingContainerPatch
    {
        [HarmonyPatch("CanBePlaced")]
        [HarmonyPrefix]
        static bool CanBePlacedPatch(ref bool __result)
        {
            if (SDBuildMenu.enableBuildClip.Value)
                __result = true;
            return !SDBuildMenu.enableBuildClip.Value;
        }
    }
}
