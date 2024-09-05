using _scripts._multiplayer._controller._game;
using _scripts._multiplayer._controller;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BepInEx.Configuration;
using SimLibGUI;

namespace SDDebug.MenuGUI
{
    internal static class SDDrivingMenu
    {
        public static float updateTime = 5f;

        public static ConfigEntry<bool> enableGodMode;
        public static ConfigEntry<bool> enableSpeedRepair;
        public static ConfigEntry<bool> enableInstantRepair;
        public static ConfigEntry<bool> disableUploadLeaderboard;
        public static ConfigEntry<bool> customShifting;

        public static ConfigEntry<KeyCode> manualShiftUp;
        public static ConfigEntry<KeyCode> manualShiftDown;

        private static DateTime nextUpdate;
        private static DrivingCar currentCar;
        private static SimGUI menuGUI;

        public static void Initialize(ConfigFile config, ref SimGUI menuGUI)
        {
            SDDrivingMenu.menuGUI = menuGUI;

            enableGodMode = config.Bind("Driving", "enableGodMode", false, "Disables parts breaking");
            enableSpeedRepair = config.Bind("Driving", "enableSpeedRepair", false, "Repairing at any speed");
            enableInstantRepair = config.Bind("Driving", "enableInstantRepair", false, "Allows repairing instantly - double reset to reset normally");
            disableUploadLeaderboard = config.Bind("Driving", "disableUploadLeaderboard", true, "Disables uploading to the leaderboard");
            customShifting = config.Bind("Driving", "customShifting", true, "Creates a better manual shifting experience");
            manualShiftUp = config.Bind("Driving", "manualShiftUp", KeyCode.Period, "Keybind for shifting up");
            manualShiftDown = config.Bind("Driving", "manualShiftDown", KeyCode.Comma, "Keybind for shifting down");

            menuGUI.AddTab("Driving", DrawTab, UpdateTab);
        }

        public static void DrawTab()
        {
            enableGodMode.Value = menuGUI.AddSwitch("God Mode", "godMode", enableGodMode.Value);
            enableSpeedRepair.Value = menuGUI.AddSwitch("Any Speed Repair", "speedRepair", enableSpeedRepair.Value);
            enableInstantRepair.Value = menuGUI.AddSwitch("Instant Repair", "instantRepair", enableInstantRepair.Value);
            disableUploadLeaderboard.Value = menuGUI.AddSwitch("Disable Leaderboard Upload", "disableLeaderboard", disableUploadLeaderboard.Value);
            customShifting.Value = menuGUI.AddSwitch("Custom Shifting", "customShifting", customShifting.Value);

            if (customShifting.Value)
            {
                manualShiftUp.Value = menuGUI.AddKeybindButton("Shift Up Key", "manualShiftUp", manualShiftUp.Value);
                manualShiftDown.Value = menuGUI.AddKeybindButton("Shift Down Key", "manualShiftDown", manualShiftDown.Value);
            }

            if (IsCarNull(currentCar))
            {
                menuGUI.AddLabel("Could not find current car.");
                return;
            }

            menuGUI.NextColumn();

            DisplayCarStats(menuGUI, currentCar);
        }

        public static void DisplayCarStats(SimGUI gui, DrivingCar car)
        {
            gui.AddSection($"Car", $"car{car.PlayerID}", () => {
                gui.AddSection("Car Stats", $"{car.Playername}CarStats", () => {
                    gui.AddLabel($"Speed: {car.CurrentVelocity.magnitude * 3.6f}");
                    gui.AddLabel($"Position: {car.Position}");
                    gui.AddLabel($"Rotation: {car.Rotation.eulerAngles}");
                    gui.AddLabel($"Angular Velocity: {car.CurrentAngularVelocity.magnitude * 3.6f}");
                    gui.AddLabel($"Max Health: {car.MaxHealth}");
                    gui.AddLabel($"Current Health: {car.CurrentHealth}");
                    gui.AddLabel($"Engine Load: {car.EngineLoad}");
                    gui.AddLabel($"Turning Angle: {car.SimpleCar2.AllWheels.Max(wheel => Math.Abs(wheel.steerAngle))}");
                    gui.AddLabel($"Current Gear: {car.SimpleCar2.EngineSoundRealizer.MechanicalOutputShifter.CurrentGear + 1}");
                }, true, false);
                gui.AddSection("Car States", $"carStates{car.PlayerID}", () =>
                {
                    gui.AddLabel($"Throttle: {car.IsPressingThrottle}");
                    gui.AddLabel($"Braking: {car.IsBraking}");
                    gui.AddLabel($"Hand Braking: {car.IsHandbraking}");
                    gui.AddLabel($"Repairing: {car.IsRepairing}");
                    gui.AddLabel($"In Reverse: {car.IsInReverse}");
                    gui.AddLabel($"Immune: {car.IsImmune}");
                    gui.AddLabel($"Repair Allowed: {car.IsAllowedToBeRepaired}");
                    gui.AddLabel($"Shifting: {car.SimpleCar2.EngineSoundRealizer.MechanicalOutputShifter.IsShiftTorqueDeadTimeActive}");
                    gui.AddLabel($"Creative Car: {car.SimpleCar2.IsCreativeModeCar}");
                    gui.AddLabel($"Steam Workshop Car: {car.SimpleCar2.IsSteamWorkshopCar}");
                }, true, false);
                gui.AddSection("Car Info", $"carInfo{car.PlayerID}", () =>
                {
                    gui.AddLabel($"Wind Resistance: {car.CarStats.windResistance}");
                    gui.AddLabel($"Grip: {car.CarStats.averageTireGrip}");
                    gui.AddLabel($"Aerodynamics: {car.CarStats.aerodynamics}");
                    gui.AddLabel($"Weight: {car.CarStats.weight}");
                    gui.AddLabel($"Car Class: {car.CalculatedCar.CarClass.className}");
                    gui.AddLabel($"Wheels: {car.CalculatedCar.TotalAmountOfWheels}");
                    gui.AddLabel($"Parts: {car.CalculatedCar.Parts.Count()}");
                    gui.AddLabel($"Gears: {car.CalculatedCar.GearStateMap.GearsAmount}");
                    gui.AddLabel($"Max Speed: {car.CarStats.maxSpeed}");
                    gui.AddLabel($"Acceleration: {car.CarStats.midAcceleration}");
                }, true, false);
            });
        }

        public static void UpdateTab(bool isTabActive)
        {
            if (isTabActive && IsCarNull(currentCar) && DateTime.Now >= nextUpdate)
            {
                nextUpdate = DateTime.Now.AddSeconds(updateTime);
                currentCar = GetCurrentCar();
                if (IsCarNull(currentCar))
                {
                    Debug.LogWarning("Could not find current car");
                }
            }
        }

        static List<PlayerInfo> GetPlayerInfos()
            => GameController.GetInstance(menuGUI.gameObject).PlayerInfos;

        static DrivingCar[] GetCars()
            => UnityEngine.Object.FindObjectsOfType<DrivingCar>();

        static DrivingCar GetCurrentCar()
        {
            byte playerId = 0;

            if (GameController.IsSingleplayer)
            {
                playerId = GetPlayerInfos().FirstOrDefault().PlayerID;
            }
            else
            {
                ulong playerSteamID = SteamUser.GetSteamID().m_SteamID;
                playerId = GetPlayerInfos().Find(playerInfo => playerInfo.CSteamID == playerSteamID).PlayerID;
            }

            return GetCarOfPlayer(playerId);
        }

        public static DrivingCar GetCarOfPlayer(byte playerId)
            => GetCars().FirstOrDefault(car => car.PlayerID == playerId);

        public static bool IsCarNull(DrivingCar car)
        {
            if (car is null)
                return true;

            if (car.SimpleCar2.MainParent.rigidbody == null)
                return true;

            return false;
        }
    }
}
