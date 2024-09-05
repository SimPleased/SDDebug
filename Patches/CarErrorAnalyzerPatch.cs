using HarmonyLib;
using SDDebug.MenuGUI;
using System.Collections.Generic;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(CarErrorAnalyzer))]
    internal class CarErrorAnalyzerPatch
    {
        [HarmonyPatch("FindErrors")]
        [HarmonyPrefix]
        static bool FindErrorsPatch(ref List<CarError> __result)
        {
            if (SDBuildMenu.disableErrors.Value)
                __result = new List<CarError>();
            return !SDBuildMenu.disableErrors.Value;
        }
    }
}
