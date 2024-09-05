using BepInEx.Configuration;
using SimLibGUI;

namespace SDDebug.MenuGUI
{
    internal static class SDBuildMenu
    {
        public static ConfigEntry<bool> colourAnything;
        public static ConfigEntry<bool> showHiddenParts;
        public static ConfigEntry<bool> disableErrors;
        public static ConfigEntry<bool> enableBuildClip;
        public static ConfigEntry<bool> ignorePartLimits;
        public static ConfigEntry<bool> ignorePartDirection;
        public static ConfigEntry<bool> ignorePartType;

        public static float engineSpeedMultiplier = 1f;
        public static float engineTorqueMultiplier = 1f;
        public static float partHealthMultiplier = 1f;
        public static float partWeightMultiplier = 1f;
        public static float wheelGripMultiplier = 1f;
        public static float aeroMultiplier = 1f;

        public static Part selectedPart = null;

        private static SimGUI menuGUI;

        public static void Initialize(ConfigFile config, ref SimGUI menuGUI)
        {
            SDBuildMenu.menuGUI = menuGUI;

            colourAnything = config.Bind("Building", "colourAnything", false, "Allows you to color any part");
            showHiddenParts = config.Bind("Building", "showHiddenParts", false, "Makes parts that are excluded from the game visible(DLC parts stay hidden)");
            disableErrors = config.Bind("Building", "disableErrors", false, "Allows you to disable errors");
            enableBuildClip = config.Bind("Building", "enableBuildClip", false, "Enable part clipping");
            ignorePartLimits = config.Bind("Building", "ignorePartLimits", false, "Ignore part limits");
            ignorePartDirection = config.Bind("Building", "ignorePartDirection", false, "Ignore part direction");
            ignorePartType = config.Bind("Building", "ignorePartType", false, "Ignore part type");

            SDBuildMenu.menuGUI.AddTab("Building", DrawTab);
        }

        public static void DrawTab()
        {
            colourAnything.Value = menuGUI.AddSwitch("Colour Anything", "colourAnything", colourAnything.Value);
            showHiddenParts.Value = menuGUI.AddSwitch("Show Hidden Parts", "showHiddenParts", showHiddenParts.Value);

            disableErrors.Value = menuGUI.AddSwitch("Disable Errors", "disableErrors", disableErrors.Value);
            enableBuildClip.Value = menuGUI.AddSwitch("Part Clipping", "buildClipToggle", enableBuildClip.Value);
            ignorePartLimits.Value = menuGUI.AddSwitch("Ignore Part Limits", "ignorePartLimits", ignorePartDirection.Value);
            ignorePartDirection.Value = menuGUI.AddSwitch("Ignore Part Direction", "ignorePartDirection", ignorePartDirection.Value);
            ignorePartType.Value = menuGUI.AddSwitch("Ignore Part Type", "ignorePartType", ignorePartType.Value);

            engineSpeedMultiplier = menuGUI.AddSlider("Engine Speed Multiplier", "engineSpeedMultiplier", engineSpeedMultiplier, 0.5f, 50f);
            engineTorqueMultiplier = menuGUI.AddSlider("Engine Torque Multiplier", "engineTorqueMultiplier", engineTorqueMultiplier, 0.5f, 50f);

            partHealthMultiplier = menuGUI.AddSlider("Part Health Multiplier", "partHealthMultiplier", partHealthMultiplier, 0.5f, 50f);
            partWeightMultiplier = menuGUI.AddSlider("Part Weight Multiplier", "partWeightMultiplier", partWeightMultiplier, 0.05f, 5f);

            wheelGripMultiplier = menuGUI.AddSlider("Wheel Grip Multiplier", "wheelGripMultiplier", wheelGripMultiplier, 0.5f, 50f);

            aeroMultiplier = menuGUI.AddSlider("Aero Multiplier", "aeroMultiplier", aeroMultiplier, 0.5f, 50f);

            if (selectedPart != null)
            {
                menuGUI.NextColumn();

                menuGUI.AddSection("Selected Part Stats", "selectedPartStats", () =>
                {
                    menuGUI.AddLabel($"Part Name: {selectedPart.PartName}");
                    menuGUI.AddLabel($"Part Category: {selectedPart.PartCategory}");
                    menuGUI.AddLabel($"Part Price: {selectedPart.Price}");
                    menuGUI.AddLabel($"Part Weight: {selectedPart.Mass}");
                    menuGUI.AddLabel($"Part Health: {selectedPart.Health}");
                    menuGUI.AddLabel($"Part Armour: {selectedPart.Armor}");
                    menuGUI.AddLabel($"Part Grip: {selectedPart.Grip}");

                    if (selectedPart.IsWheel)
                    {
                        menuGUI.AddLabel($"Wheel Radius: {selectedPart.WheelRadius}");
                        menuGUI.AddLabel($"Wheel Width: {selectedPart.WheelWidth}");
                    }
                    if (selectedPart.IsMotor)
                    {
                        menuGUI.AddLabel($"Motor Power: {selectedPart.MotorPower}");
                        menuGUI.AddLabel($"Motor Torque: {selectedPart.MotorTorque}");
                        menuGUI.AddLabel($"Motor Top Speed: {selectedPart.MotorTopspeed}");
                    }
                    if (selectedPart.IsGear)
                    {
                        menuGUI.AddLabel($"Gear Teeth: {selectedPart.GearTooths}");
                        menuGUI.AddLabel($"Gear Visual Teeth: {selectedPart.GearToothsVisual}");
                    }
                    if (selectedPart.IsGearTransmission)
                    {
                        menuGUI.AddLabel($"Transmission States: {selectedPart.GearTransmissionStatesAmount}");
                    }
                    if (selectedPart.GeneratesDownforce)
                    {
                        menuGUI.AddLabel($"Downforce Strength: {selectedPart.DownforceStrength}");
                        menuGUI.AddLabel($"Downforce : {selectedPart.DownforceSurface}");
                    }
                });
            }
        }
    }
}
