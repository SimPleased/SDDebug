using HarmonyLib;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(CalculatedCar))]
    public static class CalculatedCarErrorsPatch
    {
        [HarmonyPatch("CarErrors", MethodType.Setter)]
        [HarmonyPrefix]
        static bool CarErrorsPatch(ref List<CarError> value)
        {
            if (!SDBuildMenu.disableErrors.Value)
                return true;

            var stackTrace = new StackTrace(true);
            var callingMethod = stackTrace.GetFrames()
                .Skip(1)  // Skip the current method
                .FirstOrDefault(f => f.GetMethod().DeclaringType != typeof(CalculatedCar));

            string callingClass = callingMethod != null
                ? callingMethod.GetMethod().DeclaringType.FullName
                : "Unknown";

            Plugin.Logger.LogInfo($"CarErrors setter called by class: {callingClass}");
            Plugin.Logger.LogInfo($"Attempting to set {value.Count} CarErrors on CalculatedCar instance");

            value = new List<CarError>();
            return false;
        }
    }
}
