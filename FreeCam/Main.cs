using System;
using System.Collections.Generic;
using Flash2;
using UnhollowerRuntimeLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FreeCam
{
    public static class Main
    {
        private static CameraController _cameraController;
        private static FreeCamController _freeCam;

        /// <summary>
        ///     Regular movement speed of the camera.
        /// </summary>
        public static float MovementSpeed { get; set; } = 12f;

        /// <summary>
        ///     Movement speed of the camera when the 'Shift' key is held down.
        /// </summary>
        public static float FastMovementSpeed { get; set; } = 64f;

        /// <summary>
        ///     Movement speed of the camera with the scroll wheel.
        /// </summary>
        public static float ZoomSpeed { get; set; } = 128f;

        /// <summary>
        ///     Movement speed of the camera with the scroll wheel when the 'Shift' key is held down.
        /// </summary>
        public static float FastZoomSpeed { get; set; } = 1024f;

        /// <summary>
        ///     The speed T, F, G, and H keys rotate the camera at.
        /// </summary>
        public static float RotationSpeed { get; set; } = 1f;

        /// <summary>
        ///     The speed T, F, G, and H keys rotate the camera at when the 'Shift' key is held down.
        /// </summary>
        public static float FastRotationSpeed { get; set; } = 2f;

        /// <summary>
        ///     The speed R and Y keys tilt the camera at.
        /// </summary>
        public static float TiltSpeed { get; set; } = 1f;

        /// <summary>
        ///     The speed R and Y keys tilt the camera at when the 'Shift' key is held down.
        /// </summary>
        public static float FastTiltSpeed { get; set; } = 2f;

        /// <summary>
        ///     Mouse sensitivity for free look.
        /// </summary>
        public static float FreeLookSensitivity { get; set; } = 128f;

        /// <summary>
        ///     Hides the HUD while the FreeCam mode is active.
        /// </summary>
        public static bool HideHUD { get; set; } = true;

        /// <summary>
        ///     Stops the game while the FreeCam mode is active.
        /// </summary>
        public static bool FreezeGameplay { get; set; } = true;

        /// <summary>
        ///    Disables the controls while the FreeCam mode is active and the gameplay is not frozen.
        /// </summary>
        public static bool DisableControls { get; set; } = false;

        /// <summary>
        ///     Disables the stage tilt while the FreeCam mode is active.
        /// </summary>
        public static bool DisableVisualStageTilt { get; set; } = true;


        /// <summary>
        ///     When the mod is loaded at the very start of the game.
        /// </summary>
        /// <param name="settings">Settings for the mod.</param>
        /// <returns>A list of types that inherit types from Unity and the base game.</returns>
        public static List<Type> OnModLoad(Dictionary<string, object> settings)
        {
            // Load the settings

            MovementSpeed = (float) settings["MovementSpeed"];
            FastMovementSpeed = (float) settings["FastMovementSpeed"];
            ZoomSpeed = (float) settings["ZoomSpeed"];
            FastZoomSpeed = (float) settings["FastZoomSpeed"];
            TiltSpeed = (float)settings["TiltSpeed"];
            FastTiltSpeed = (float)settings["FastTiltSpeed"];
            RotationSpeed = (float)settings["RotationSpeed"];
            FastRotationSpeed = (float)settings["FastRotationSpeed"];

            FreeLookSensitivity = (float) settings["FreeLookSensitivity"];
            HideHUD = (bool)settings["HideHUD"];
            FreezeGameplay = (bool)settings["FreezeGameplay"];
            DisableControls = (bool)settings["DisableControls"];
            DisableVisualStageTilt = (bool)settings["DisableVisualStageTilt"];

            return new List<Type> { typeof(FreeCamController) };
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModUpdate()
        {
            // If the player doesn't exist
            var player = Object.FindObjectOfType<Player>();
            if (player == null)
            {
                // If the FreeCam is enabled without the player, disable it
                _freeCam = null;
                return;
            }

            // When the key is pressed
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // Get the default camera controller
                _cameraController = player.GetCameraController();

                // Toggle it
                if (_freeCam == null)
                    EnableCam();
                else
                    DisableCam();
            }
        }

        private static void EnableCam()
        {
            // If there's an existing camera controller...
            if (_cameraController != null)
            {
                // Disable it and put our own controller in place
                _cameraController.enabled = false;
                _freeCam = new FreeCamController(_cameraController.gameObject.AddComponent(Il2CppType.Of<FreeCamController>())
                    .Pointer);
            }

            // Stop everything
            if (FreezeGameplay)
                GameManager.SetPause(true);
            else if (DisableControls)
            {
                var gravityController = Object.FindObjectOfType<GravityController>();
                if (gravityController != null)
                {
                    gravityController.m_IsEnableControl = false;
                }
            }
        }

        private static void DisableCam()
        {
            // Enable the original camera controller back
            if (_cameraController != null)
                _cameraController.enabled = true;

            // Destroy ours
            Object.Destroy(_freeCam);
            _freeCam = null;

            // Resume the game
            if (FreezeGameplay)
                GameManager.SetPause(false);
            else if (DisableControls)
            {
                var gravityController = Object.FindObjectOfType<GravityController>();
                if (gravityController != null)
                {
                    gravityController.m_IsEnableControl = true;
                }
            }
        }
    }
}