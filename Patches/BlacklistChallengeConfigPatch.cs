using BlacklistNS;
using CarClassNS;
using HarmonyLib;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(BlacklistChallengeConfig))]
    internal class BlacklistChallengeConfigPatch
    {
        [HarmonyPatch("SetPersonalBestTimeForCheckpoint")]
        [HarmonyPrefix]
        static bool SetPersonalBestTimeForCheckpointPatch()
            => !SDDrivingMenu.disableUploadLeaderboard.Value;

        [HarmonyPatch("SetPersonalBestTime")]
        [HarmonyPrefix]
        static bool SetPersonalBestTimePatch(BlacklistChallengeConfig __instance, float time, CarClass carClass)
            => !SDDrivingMenu.disableUploadLeaderboard.Value;
    }
}
