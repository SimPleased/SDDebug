using HarmonyLib;
using Repairing;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(RepairManagerClient))]
    internal class RepairManagerClientPatch
    {
        [HarmonyPatch("isBelowVelThreshes")]
        [HarmonyPrefix]
        static bool isBelowVelThreshesPatch(ref bool __result)
        {
            if (SDDrivingMenu.enableSpeedRepair.Value)
            {
                Plugin.Logger.LogInfo("Forcing the car to reset");
                __result = true;
                return false;
            }
            return true;
        }
    }
}
