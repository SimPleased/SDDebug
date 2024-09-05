using _scripts._multiplayer._controller._game;
using _scripts._multiplayer._controller;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimLibGUI;
using HarmonyLib;

namespace SDDebug.MenuGUI
{
    internal static class SDServerMenu
    {
        public static float updateTime = 10f;

        private static int selectedMapIndex;
        private static int selectedPlayerIndex;

        private static DateTime nextUpdate;
        private static DrivingCar playersCar;
        private static List<PlayerInfo> playerInfos;
        private static SimGUI menuGUI;

        public static void Initialize(ref SimGUI menuGUI)
        {
            SDServerMenu.menuGUI = menuGUI;
            SDServerMenu.menuGUI.AddTab("Server", DrawTab, UpdateTab);
        }

        public static void DrawTab()
        {
            if (playerInfos == null || playerInfos.Count == 0)
            {
                menuGUI.AddLabel("No loaded players");
                return;
            }

            selectedPlayerIndex = Math.Min(menuGUI.AddDropdown("Selected Player:", "selectPlayer", playerInfos.Select(playerInfo => playerInfo.PlayerName).ToArray()), playerInfos.Count - 1);

            menuGUI.AddSection($"{playerInfos[selectedPlayerIndex].PlayerName}'s Player Info", "playerInfo", () =>
            {
                menuGUI.AddLabel($"Player ID: {playerInfos[selectedPlayerIndex].PlayerID}");
                menuGUI.AddLabel($"Player Steam ID: {playerInfos[selectedPlayerIndex].CSteamID}");
                menuGUI.AddLabel($"Player Ping: {Mathf.RoundToInt(playerInfos[selectedPlayerIndex].Ping * 1000)}ms");
            });

            if (!SDDrivingMenu.IsCarNull(playersCar))
            {
                menuGUI.NextColumn();

                SDDrivingMenu.DisplayCarStats(menuGUI, playersCar);
                /*if (menuGUI.AddButton("Save Car"))
                {
                    SaveDrivingCar(playersCar, $"{playerInfos[selectedPlayerIndex].PlayerName}'s Car");
                }*/
            }
        }

        public static void UpdateTab(bool isTabActive)
        {
            if (isTabActive && DateTime.Now >= nextUpdate)
            {
                nextUpdate = DateTime.Now.AddSeconds(updateTime);
                playerInfos = GetPlayerInfos();
                playersCar = SDDrivingMenu.GetCarOfPlayer(playerInfos[selectedPlayerIndex].PlayerID);
            }
        }

        static List<PlayerInfo> GetPlayerInfos()
            => GameController.GetInstance(menuGUI.gameObject).PlayerInfos;

        /*public static void SaveDrivingCar(DrivingCar car, string carName)
        {
            try
            {
                PartBuilding[] partBuilding = car.CalculatedCar.Parts.Select(calcPart => ConvertToPartBuilding(calcPart)).ToArray();

                CarPropertiesSetting carProperties = new CarPropertiesSetting();
                carProperties.CarColors = partBuilding.Select(part => part.Color).ToArray();
                carProperties.CarName = carName;
                carProperties.PartConfigs = car.CalculatedCar.Parts.Select(calcPart => calcPart.PartConfiguration).ToArray();

                string fileName = FileLoader.FetchNewRandomCarName();

                FileLoader.SaveToFile(partBuilding, fileName, carProperties);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex.Message);
                Plugin.Logger.LogError(ex.StackTrace);
                Plugin.Logger.LogError(ex);
            }
        }

        public static PartBuilding ConvertToPartBuilding(CalculatedPart calculatedPart)
        {
            if (calculatedPart == null)
                throw new ArgumentNullException(nameof(calculatedPart));

            PartBuilding partBuilding = new GameObject("PartBuilding").AddComponent<PartBuilding>();

            if (calculatedPart.PartConfiguration == null)
                throw new ArgumentNullException(nameof(calculatedPart.PartConfiguration));

            partBuilding.SetPart(calculatedPart.PartConfiguration.partType);
            partBuilding.SetPosition(calculatedPart.PartConfiguration.partPosition);
            partBuilding.SetDirection(calculatedPart.PartConfiguration.partDirection);
            partBuilding.SetRotation(calculatedPart.PartConfiguration.partRotation);
            partBuilding.Color = calculatedPart.Color;

            return partBuilding;
        }*/
    }
}
