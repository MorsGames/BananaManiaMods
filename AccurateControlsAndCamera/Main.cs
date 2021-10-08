using System;
using System.Collections.Generic;
using Flash2;
using UnityEngine;
using static Flash2.GameParam;
using Object = UnityEngine.Object;

namespace AccurateControlsAndCamera
{
    public static class Main
    {
        /// <summary>
        ///     Tries to recreate the controls and the physics of Super Monkey Ball 2 as closely as possible.
        /// </summary>
        public static bool AccurateControls { get; set; } = true;

        /// <summary>
        ///     Changes the way the camera works to make it feel closer to how it felt in the classic games.
        /// </summary>
        public static bool AccurateCamera { get; set; } = true;


        public static float StageTiltTemp { get; set; } = 0.95f;

        /// <summary>
        ///     When the mod is loaded at the very start of the game.
        /// </summary>
        /// <param name="settings">Settings for the mod.</param>
        public static void OnModLoad(Dictionary<string, object> settings)
        {
            AccurateControls = (bool) settings["AccurateControls"];
            AccurateCamera = (bool)settings["AccurateCamera"];

            StageTiltTemp = (float)settings["StageTiltTemp"];
        }

        public static void OnModStart()
        {
            if (AccurateControls)
            {
                physicsParam.gravity = 0.00981f;
                physicsParam.gravityTiltPadMax = 107f;
                physicsParam.gravityTiltWorldMax = 32.526f;
                physicsParam.gravityXZDirectionalMultiplier = 0f;
            }

            if (AccurateCamera)
            {
                mainGameParam.cameraParam.cameraBackChaseScaleMinus = 64f;
                mainGameParam.cameraParam.cameraBackChaseScalePlus = 16f;
                mainGameParam.cameraParam.cameraChaseDiffMinus = 16f;
                mainGameParam.cameraParam.cameraChaseDiffPlus = 8f;
                mainGameParam.cameraParam.cameraMaxBallSpeedForMoveRate = -1f;
                mainGameParam.cameraParam.cameraMinBallSpeedForMoveRate = -1f;
                for (var i = 0; i < mainGameParam.cameraParam.cameraRotYPadLRate.Count; i++)
                {
                    mainGameParam.cameraParam.cameraRotYPadLRate[i] = 0;
                }

                mainGameParam.cameraParam.cameraRotYSpeedBase = 6f;
                mainGameParam.cameraParam.followMonkeyInnerFactor = 0f;
                mainGameParam.cameraParam.followMonkeyScale = 0f;
                mainGameParam.cameraParam.limitXAngL = -70f;
            }
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModLateUpdate()
        {
            if (AccurateControls)
            {
                var gravityController = Object.FindObjectOfType<GravityController>();
                if (gravityController != null)
                {
                    gravityController.m_Rotation =
                        Quaternion.Slerp(Quaternion.identity, gravityController.m_Rotation, StageTiltTemp);
                    gravityController.m_LerpRatioMax = 1f;
                    gravityController.m_LerpRatioMin = 0.14f;
                }
            }
        }
    }
}