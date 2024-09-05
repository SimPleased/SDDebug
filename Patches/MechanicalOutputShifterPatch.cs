using BepInEx;
using HarmonyLib;
using SappUnityUtils.IO.SimpleSaveables;
using SDDebug.MenuGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(MechanicalOutputShifter))]
    internal class MechanicalOutputShifterPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static bool UpdatePatch(MechanicalOutputShifter __instance)
        {
            if (!SDDrivingMenu.customShifting.Value)
                return true;

            Traverse traverse = Traverse.Create(__instance);

            float shiftTorqueDeadTime = traverse.Field("shiftTorqueDeadTime").GetValue<float>();
            shiftTorqueDeadTime = Mathf.Max(shiftTorqueDeadTime - Time.deltaTime, 0f);
            traverse.Field("shiftTorqueDeadTime").SetValue(shiftTorqueDeadTime);

            float shiftCooldown = traverse.Field("shiftCooldown").GetValue<float>();
            shiftCooldown = Mathf.Max(shiftCooldown - Time.deltaTime, 0f);
            traverse.Field("shiftCooldown").SetValue(shiftCooldown);

            int currentGear = traverse.Field("currentGear").GetValue<int>();

            if (Saveables.GetValueBool(SaveID.ManualGearShiftEnabled.Value(), false))
            {
                if (UnityInput.Current.GetKeyDown(SDDrivingMenu.manualShiftUp.Value))
                {
                    if (currentGear < __instance.GearStateMap.GearsAmount - 1)
                    {
                        __instance.ShiftToGear(currentGear + 1);
                    }
                }
                else if (UnityInput.Current.GetKeyDown(SDDrivingMenu.manualShiftDown.Value) && currentGear > 0)
                {
                    __instance.ShiftToGear(currentGear - 1);
                }
            }
            else
            {
                if (shiftCooldown <= 0f)
                {
                    List<MechanicalOutputWheel> wheels = traverse.Field("wheels").GetValue<List<MechanicalOutputWheel>>();

                    float averageWheelVelocity = 0f;
                    int workingWheels = 0;
                    foreach (MechanicalOutputWheel wheel in wheels)
                    {
                        if (wheel.IsBroken)
                            continue;

                        averageWheelVelocity += wheel.CurrentVelocity;
                        workingWheels++;
                    }
                    averageWheelVelocity /= Mathf.Max(workingWheels, 1);

                    int desiredGear = __instance.GearStateMap.BestGear(averageWheelVelocity);
                    traverse.Field("desiredGear").SetValue(desiredGear);

                    if (desiredGear != currentGear)
                    {
                        int aimingGear = traverse.Field("aimingGear").GetValue<int>();
                        float aimingFor = traverse.Field("aimingFor").GetValue<float>();

                        if (aimingGear != desiredGear)
                        {
                            aimingGear = desiredGear;
                            aimingFor = 0f;

                        }
                        else
                        {
                            aimingFor += Time.deltaTime;

                            if (aimingFor >= traverse.Field("overThreshFor").GetValue<float>())
                            {
                                __instance.ShiftToGear(desiredGear);
                            }
                        }

                        traverse.Field("aimingGear").SetValue(aimingGear);
                        traverse.Field("aimingFor").SetValue(aimingFor);
                    }
                }
            }

            return false;
        }
    }
}
