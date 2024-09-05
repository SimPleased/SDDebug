using HarmonyLib;
using SDDebug.MenuGUI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(SteamUserStats))]
    internal class SteamUserStatsPatch
    {
        [HarmonyPatch("UploadLeaderboardScore")]
        [HarmonyPrefix]
        static bool UploadLeaderboardScorePatch()
            => !SDDrivingMenu.disableUploadLeaderboard.Value;
    }
}
