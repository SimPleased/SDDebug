using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SDDebug.MenuGUI;
using SDDebug.Patches;
using SimLibGUI;
using UnityEngine;

namespace SDDebug
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "com.SimPleased.SDDebug";
        private const string modName = "SDDebug";
        private const string modVersion = "1.0.0.0";

        private SimGUI menuGUI;

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource Logger;
        void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            Logger.LogInfo($"{modName} has started!");

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(PartBuildingContainerPatch));
            harmony.PatchAll(typeof(CarErrorAnalyzerPatch));
            harmony.PatchAll(typeof(CarAnalyzerPatch));
            harmony.PatchAll(typeof(CarAnalyzer2Patch));
            harmony.PatchAll(typeof(DrivingCarPatch));
            harmony.PatchAll(typeof(RepairManagerClientPatch));
            harmony.PatchAll(typeof(ConnectionpointPatch));
            harmony.PatchAll(typeof(TorqueFunctionPatch));
            harmony.PatchAll(typeof(SteamUserStatsPatch));
            harmony.PatchAll(typeof(BlacklistChallengeConfigPatch));
            harmony.PatchAll(typeof(CalculatedCarErrorsPatch));
            harmony.PatchAll(typeof(MechanicalOutputShifterPatch));
            harmony.PatchAll(typeof(BuildingMouseMovementPatch));
            harmony.PatchAll(typeof(PartPatch));
        }

        void Start()
        {
            menuGUI = gameObject.AddComponent<SimGUI>();
            menuGUI.SetTitle("Screw Drivers Debug");
            menuGUI.SetToggleKey(KeyCode.F3);
            menuGUI.SetWindowSize(new Vector2(750, 500));
            menuGUI.SetColumnWidth(300);

            SDDrivingMenu.Initialize(Config, ref menuGUI);
            SDBuildMenu.Initialize(Config, ref menuGUI);
            SDServerMenu.Initialize(ref menuGUI);

            DontDestroyOnLoad(menuGUI);
        }
    }
}
