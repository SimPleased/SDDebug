using HarmonyLib;
using SDDebug.MenuGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SDDebug.Patches
{
    [HarmonyPatch]
    internal class TorqueFunctionPatch
    {
        /*[HarmonyPatch(typeof(TorqueFunction), "XScale", MethodType.Getter)]
        [HarmonyPostfix]
        static void XScalePatch(ref float __result)
        {
            __result /= SDBuildMenu.engineSpeedMultiplier.Value;
        }

        [HarmonyPatch(typeof(TorqueFunction), "YScale", MethodType.Getter)]
        [HarmonyPostfix]
        static void YScalePatch(ref float __result)
        {
            __result *= SDBuildMenu.engineTorqueMultiplier.Value;
        }*/

        [HarmonyPatch(typeof(TorqueFunctionCombustion), MethodType.Constructor, new Type[] { typeof(float), typeof(float) })]
        [HarmonyPrefix]
        static bool CombustionPatch(ref float maxTorque, ref float maxSpeed)
        {
            Plugin.Logger.LogInfo("TorqueFuncionConstant constructor was called.");
            Plugin.Logger.LogInfo($"Original maxTorque: {maxTorque}");
            Plugin.Logger.LogInfo($"Original maxSpeed: {maxSpeed}");

            maxTorque *= SDBuildMenu.engineTorqueMultiplier;
            maxSpeed *= SDBuildMenu.engineSpeedMultiplier;

            return true;
        }

        [HarmonyPatch(typeof(TorqueFuncionConstant), MethodType.Constructor, new Type[] { typeof(float), typeof(float) })]
        [HarmonyPrefix]
        static bool ConstantPatch(ref float constValue, ref float motorTopspeed)
        {
            Plugin.Logger.LogInfo("TorqueFuncionConstant constructor was called.");
            Plugin.Logger.LogInfo($"Original constValue: {constValue}");
            Plugin.Logger.LogInfo($"Original motorTopspeed: {motorTopspeed}");

            constValue *= SDBuildMenu.engineTorqueMultiplier;
            motorTopspeed *= SDBuildMenu.engineSpeedMultiplier;

            return true;
        }

        [HarmonyPatch(typeof(TorqueFunctionAnimationCurve), MethodType.Constructor, new Type[] { typeof(AnimationCurve) })]
        [HarmonyPrefix]
        static bool AnimationCurvePatch(ref AnimationCurve animationCurve)
        {
            Plugin.Logger.LogInfo("TorqueFunctionAnimationCurve constructor was called.");

            AnimationCurve scaledCurve = new AnimationCurve();

            for (int i = 0; i < animationCurve.keys.Length; i++)
            {
                Plugin.Logger.LogInfo($"Node{i} Original Torque: {animationCurve.keys[i].value}");
                Plugin.Logger.LogInfo($"Node{i} Original Speed: {animationCurve.keys[i].time}");

                Keyframe originalKey = animationCurve.keys[i];

                float scaledTime = originalKey.time * SDBuildMenu.engineSpeedMultiplier;
                float scaledValue = originalKey.value * SDBuildMenu.engineTorqueMultiplier;

                float scaledInTangent = originalKey.inTangent * (SDBuildMenu.engineTorqueMultiplier / SDBuildMenu.engineSpeedMultiplier);
                float scaledOutTangent = originalKey.outTangent * (SDBuildMenu.engineTorqueMultiplier / SDBuildMenu.engineSpeedMultiplier);

                Keyframe scaledKey = new Keyframe(scaledTime, scaledValue, scaledInTangent, scaledOutTangent);

                scaledKey.weightedMode = originalKey.weightedMode;
                scaledKey.inWeight = originalKey.inWeight;
                scaledKey.outWeight = originalKey.outWeight;

                scaledCurve.AddKey(scaledKey);
            }

            animationCurve = scaledCurve;

            return true;
        }

        [HarmonyPatch(typeof(TorqueFunctionConstantServo), MethodType.Constructor, new Type[] { typeof(float) })]
        [HarmonyPrefix]
        static bool ConstantServoPatch(ref float constValue)
        {
            Plugin.Logger.LogInfo("TorqueFunctionConstantServo constructor was called.");
            Plugin.Logger.LogInfo($"Original constValue: {constValue}");


            constValue *= SDBuildMenu.engineTorqueMultiplier;

            return true;
        }
    }
}
