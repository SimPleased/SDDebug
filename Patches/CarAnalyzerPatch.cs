using HarmonyLib;
using SDDebug.MenuGUI;
using System.Collections.Generic;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(CarAnalyzer))]
    internal class CarAnalyzerPatch
    {
        [HarmonyPatch("getPreErrors")]
        [HarmonyPrefix]
        static bool getPreErrorsPatch(ref List<CarError> __result)
        {
            if (SDBuildMenu.disableErrors.Value)
                __result = new List<CarError>();
            return !SDBuildMenu.disableErrors.Value;
        }

        [HarmonyPatch("calculateTorqueNode")]
        [HarmonyPrefix]
        static bool calculateTorqueNodePatch(ref bool __result)
        {
            if (SDBuildMenu.disableErrors.Value)
                __result = true;
            return !SDBuildMenu.disableErrors.Value;
        }
    }
}
