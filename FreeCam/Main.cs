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
        ///     The speed Z and X keys tilt the camera at.
        /// </summary>
        public static float TiltSpeed { get; set; } = 1f;

        /// <summary>
        ///     The speed Z and X keys tilt the camera at when the 'Shift' key is held down.
        /// </summary>
        public static float FastTiltSpeed { get; set; } = 4f;

        /// <summary>
        ///     Mouse sensitivity for free look.
        /// </summary>
        public static float FreeLookSensitivity { get; set; } = 128f;

        /// <summary>
        ///     Whether if the HUD should be hidden or not during the FreeCam mode.
        /// </summary>
        public static bool HideHUD { get; set; } = true;

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
            TiltSpeed = (float) settings["TiltSpeed"];
            FastTiltSpeed = (float) settings["FastTiltSpeed"];

            FreeLookSensitivity = (float) settings["FreeLookSensitivity"];
            HideHUD = (bool)settings["HideHUD"];

            return new List<Type> { typeof(FreeCamController) };
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModUpdate()
        {
            // Only run the following code when the player exists within the scene and the 'Tab' key is pressed
            var player = Object.FindObjectOfType<Player>();
            if (player == null || !Input.GetKeyDown(KeyCode.Tab)) return;

            // If the FreeCam is off
            if (_freeCam == null)
            {
                // Get the default camera controller
                _cameraController = player.GetCameraController();
                if (_cameraController != null)
                {
                    // Disable it and put our own controller in place
                    _cameraController.enabled = false;

                    _freeCam = new FreeCamController(_cameraController.gameObject.AddComponent(Il2CppType.Of<FreeCamController>()).Pointer);
                }

                // Stop everything
                GameManager.SetPause(true);
            }

            // If the FreeCam is on
            else
            {
                // Enable the original camera controller back
                _cameraController.enabled = true;

                // Destroy ours
                Object.Destroy(_freeCam);
                _freeCam = null;

                // Resume the game
                GameManager.SetPause(false);
            }
        }
    }
}