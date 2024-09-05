using HarmonyLib;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.PlayerLoop.PreLateUpdate;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(DrivingCar))]
    internal class DrivingCarPatch
    {
        public static float bufferTime = 1f;
        private static DateTime repairBuffer = DateTime.MinValue;

        [HarmonyPatch("IsImmune", MethodType.Getter)]
        [HarmonyPrefix]
        static bool IsImmunePatch(ref bool __result)
        {
            if (SDDrivingMenu.enableGodMode.Value)
                __result = true;
            return !SDDrivingMenu.enableGodMode.Value;
        }

        [HarmonyPatch("Repair")]
        [HarmonyPrefix]
        static bool RepairPatch(DrivingCar __instance)
        {
            if (SDDrivingMenu.enableInstantRepair.Value && DateTime.Now >= repairBuffer)
            {
                repairBuffer = DateTime.Now.AddSeconds(bufferTime);
                __instance.RepairInstantly();
                return false;
            }
            return true;
        }
    }
}
