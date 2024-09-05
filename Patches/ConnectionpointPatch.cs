using HarmonyLib;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(Connectionpoint))]
    internal class ConnectionpointPatch
    {
        [HarmonyPatch("CanBeConnected")]
        [HarmonyPrefix]
        static bool CanBeConnectedPatch(Connectionpoint __instance, ref bool __result, ConnectorType otherType, PartDirection otherDirection, int boxColliderIndex)
        {
            Traverse traverse = Traverse.Create(__instance);
            ConnectorType[] acceptedConnectortypes = traverse.Field("acceptedConnectortypes").GetValue<ConnectorType[]>();
            PartDirection[] boxColliderDirections = traverse.Field("boxColliderDirections").GetValue<PartDirection[]>();

            if (acceptedConnectortypes == null || boxColliderDirections == null)
                return true;

            __result = true;

            if (!SDBuildMenu.ignorePartType.Value && !acceptedConnectortypes.Contains(otherType))
                __result = false;

            if (!SDBuildMenu.ignorePartDirection.Value && boxColliderDirections[boxColliderIndex] != otherDirection)
                __result = false;

            return false;
        }
    }
}
