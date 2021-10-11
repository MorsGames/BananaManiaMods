using System;
using System.Collections.Generic;
using Flash2;
using UnityEngine;
using static Flash2.GameParam;
using Object = UnityEngine.Object;

namespace AccurateControls
{
    public static class Main
    {
        private static Quaternion _visualStageTilt = Quaternion.identity;

        /// <summary>
        ///     Tries to recreate the controls and the physics of Super Monkey Ball 2 as closely as possible.
        /// </summary>
        public static bool AccurateControls { get; set; } = true;

        /// <summary>
        ///     Changes the way the camera works to make it feel closer to how it felt in the classic games.
        /// </summary>
        public static bool AccurateCamera { get; set; } = true;

        /// <summary>
        ///     Prevents you from rotating the camera while moving.
        /// </summary>
        public static bool DisableCameraControlsWhileMoving { get; set; } = false;

        /// <summary>
        ///     When the mod is loaded at the very start of the game.
        /// </summary>
        /// <param name="settings">Settings for the mod.</param>
        public static void OnModLoad(Dictionary<string, object> settings)
        {
            AccurateControls = (bool) settings["AccurateControls"];
            AccurateCamera = (bool)settings["AccurateCamera"]; 
            DisableCameraControlsWhileMoving = (bool)settings["DisableCameraControlsWhileMoving"];
        }

        /// <summary>
        ///     When the mod starts its shenanigans.
        /// </summary>
        public static void OnModStart()
        {
            // New values for the physics.
            if (AccurateControls)
            {
                physicsParam.gravity = 0.00981f;
                physicsParam.gravityTiltPadMax = 107f;
                physicsParam.gravityTiltWorldMax = 32.526f;
                physicsParam.gravityXZDirectionalMultiplier = 0f;
            }

            // New values for the camera
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
                    mainGameParam.cameraParam.cameraRotYPadLRate[i] = 0f;
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
            // Disable camera rotation based on speed
            if (DisableCameraControlsWhileMoving) {
                var player = Object.FindObjectOfType<Player>();
                if (player != null)
                {
                    mainGameParam.cameraParam.enableControl = player.velocity <= 12f;
                }
            }

            // Only run the following code if the controls are being changed
            if (!AccurateControls) return;

            // Let's use our custom method for visual stage tilt
            var gravityController = Object.FindObjectOfType<GravityController>();
            if (gravityController != null)
            {
                var tempTilt = gravityController.m_RotationGravity;
                tempTilt =
                    Quaternion.Slerp(tempTilt, Quaternion.identity, 0.35f);
                _visualStageTilt =
                    Quaternion.Slerp(_visualStageTilt, tempTilt, 11.0f * Time.deltaTime);
                gravityController.m_Rotation = _visualStageTilt;
                gravityController.m_LerpRatioMax = 1f;
                gravityController.m_LerpRatioMin = 0.0f;
            }
            else
            {
                _visualStageTilt = Quaternion.identity;
            }
        }
    }
}