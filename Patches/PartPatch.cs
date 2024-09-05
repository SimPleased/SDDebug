using BepInEx;
using BlacklistNS;
using HarmonyLib;
using ImportParts;
using NWH.WheelController3D;
using PartIntroductionNS;
using SDDebug.MenuGUI;
using System;
using UnityEngine;
using UnlockConditions;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(Part))]
    internal class PartPatch
    {
        [HarmonyPatch("MakePart")]
        [HarmonyPostfix]
        static void MakePartPatch(ref Part __result)
        {
            __result = new PatchedPart(__result);
        }
    }

    public class PatchedPart : Part
    {
        public PatchedPart(Part part)
        {
            originalPart = part;
            _importPart = Traverse.Create(part).Field("_importPart").GetValue<ImportPart>();
        }

        Part originalPart { get; set; }

        public override bool HasBuildingLimitations => SDBuildMenu.ignorePartLimits.Value ? false : originalPart.HasBuildingLimitations;

        public override ValueTuple<PartDirection, PartRotation>[] PossibleBuildingLimitations => originalPart.PossibleBuildingLimitations;

        public override PartProperty[] PartProperties => originalPart.PartProperties;

        public override PartType PartType => originalPart.PartType;

        public override bool HasProperties => originalPart.HasProperties;

        public override bool IsWheel => originalPart.IsWheel;

        public override bool IsTankTrackWheel => originalPart.IsTankTrackWheel;

        public override bool IsMotor => originalPart.IsMotor;

        public override bool IsGear => originalPart.IsGear;

        public override bool IsBarSolidPart => originalPart.IsBarSolidPart;

        public override int AxeLength => originalPart.AxeLength;

        public override DLCConfig DLCConfig => originalPart.DLCConfig;

        public override bool IsAxe => originalPart.IsAxe;

        public override float GearTooths => originalPart.GearTooths;

        public override float MotorTorque => originalPart.MotorTorque;

        public override TorqueFunction TorqueFunction => originalPart.TorqueFunction;

        public override float MotorTopspeed => originalPart.MotorTopspeed;

        public override float Mass => originalPart.Mass * SDBuildMenu.partWeightMultiplier;

        public override float Armor => originalPart.Armor * SDBuildMenu.partHealthMultiplier;

        public override float Grip => originalPart.Grip * SDBuildMenu.wheelGripMultiplier;

        public override float WheelRadius => originalPart.WheelRadius;

        public override bool IsSteerSusp => originalPart.IsSteerSusp;

        public override float Health => originalPart.Health * SDBuildMenu.partHealthMultiplier;

        public override bool GeneratesDownforce => originalPart.GeneratesDownforce;

        public override float DownforceStrength => originalPart.DownforceStrength * SDBuildMenu.aeroMultiplier;

        public override Vector2 DownforceSurface => originalPart.DownforceSurface;

        public override bool HasSkill => originalPart.HasSkill;

        public override Connectionpoint[] GetConnectionpoints(PartDirection direction, PartRotation rotation) => originalPart.GetConnectionpoints(direction, rotation);

        public override Connectionpoint[] GetConnectionpointsWithDummyGearOutside(PartDirection direction, PartRotation rotation) => originalPart.GetConnectionpointsWithDummyGearOutside(direction, rotation);

        public override PartCategory PartCategory => originalPart.PartCategory;

        public override float SuspensionDistance => originalPart.SuspensionDistance;

        public override float SuspensionStiffness => originalPart.SuspensionStiffness;

        public override int TierPoints => originalPart.TierPoints;

        public override Material[] AlternativeColorMats => originalPart.AlternativeColorMats;

        public override Material DefaultMaterial => originalPart.DefaultMaterial;

        public override string TooltipText => originalPart.TooltipText;

        public override string PartName => originalPart.PartName.IsNullOrWhiteSpace() ? originalPart.GetType().Name : originalPart.PartName;

        public override bool ProvidesCustomPartProperties => originalPart.ProvidesCustomPartProperties;

        public override bool IsGeneralActivatable => originalPart.IsGeneralActivatable;

        public override bool IsEngineImprovementGadget => originalPart.IsEngineImprovementGadget;

        public override bool IsNitro => originalPart.IsNitro;

        public override float NitroBoostStrength => originalPart.NitroBoostStrength;

        public override float PartPoolReducedFactor => originalPart.PartPoolReducedFactor;

        public override float NitroFillAmount => originalPart.NitroFillAmount;

        public override int TireFrictionPresetIndex => originalPart.TireFrictionPresetIndex;

        public override int Price => originalPart.Price;

        public override bool IsGearTransmission => originalPart.IsGearTransmission;

        public override bool IsPowertrainPart => originalPart.IsPowertrainPart;

        public override int GearTransmissionStatesAmount => originalPart.GearTransmissionStatesAmount;

        public override bool IsPropeller => originalPart.IsPropeller;

        public override float TierPointsRelative => originalPart.TierPointsRelative;

        public override int TierLevelPart => originalPart.TierLevelPart;

        public override float PistonMinExpansion => originalPart.PistonMinExpansion;

        public override float PistonExpansion => originalPart.PistonExpansion;

        public override bool IsPiston => originalPart.IsPiston;

        public override int SoundBuildCustom => originalPart.SoundBuildCustom;

        public override bool IsDestroyedOnDestroy => originalPart.IsDestroyedOnDestroy;

        public override bool IsSharkFin => originalPart.IsSharkFin;

        public override SharkFinFlatSideDirection SharkFinFlatSideDirection => originalPart.SharkFinFlatSideDirection;

        public override bool IsColorable => SDBuildMenu.colourAnything.Value ? true : originalPart.IsColorable;

        public override bool PartPropertiesForbidWASDKeys => originalPart.PartPropertiesForbidWASDKeys;

        public override int ColorMaterialIndex => originalPart.ColorMaterialIndex;

        public override bool OnlyApplyColor => originalPart.OnlyApplyColor;
        public override Vector3 CenterOfMass => originalPart.CenterOfMass;

        public override bool IsServoMotor => originalPart.IsServoMotor;

        public override bool HasChildBoxColliders => originalPart.HasChildBoxColliders;

        public override bool IsRigidbodyHingeDetach => originalPart.IsRigidbodyHingeDetach;

        public override bool IsExhaust => originalPart.IsExhaust;

        public override bool IsRotateAnimVanilla => originalPart.IsRotateAnimVanilla;

        public override float GearToothsVisual => originalPart.GearToothsVisual;
        
        public override float WheelWidth => originalPart.WheelWidth;

        public override bool IsExcludedFromGame => SDBuildMenu.showHiddenParts.Value ? false : originalPart.IsExcludedFromGame;

        public override bool IsSeat => originalPart.IsSeat;

        public override GameObject UnlockMetaInfo => originalPart.UnlockMetaInfo;

        public override UnlockablePart UnlockablePart => originalPart.UnlockablePart;

        public override BlacklistChallengeConfig[] ChallengesWhereUnlocked => originalPart.ChallengesWhereUnlocked;

        public override PartIntroduction PartIntroduction => originalPart.PartIntroduction;

        public override FrictionPreset FrictionPresetAsphalt => originalPart.FrictionPresetAsphalt;

        public override FrictionPreset FrictionPresetSand => originalPart.FrictionPresetSand;

        public override Material SkidMaterial => originalPart.SkidMaterial;

        public override Material SkidMaterialSand => originalPart.SkidMaterialSand;

        private TorqueFunction _cachedTorqueFunction;
    }
}
