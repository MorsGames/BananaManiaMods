using System.Collections.Generic;
using Flash2;
using Framework.UI;
using Il2CppSystem;
using Il2CppSystem.Xml;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;


namespace GraphicalTweaks
{
    public static class Main
    {
        private static bool _canModifyPostProcess = true;

        /// <summary>
        ///     Field of View of the camera. -1 sets it to default.
        /// </summary>
        public static float BaseFOV { get; set; } = -1f;

        /// <summary>
        ///     Makes it so that the light direction doesn't rotate with the stage, which can look especially weird on certain
        ///     backgrounds.
        ///     This setting, while making the lighting more accurate in general, could potentially make the movement of shadows
        ///     a bit distracting for some people.
        /// </summary>
        public static bool LightingFix { get; set; } = true;

        /// <summary>
        ///     How strong the post-processing effects should be. 0 means none, 1 means full.
        /// </summary>
        public static float PostProcessAmount { get; set; } = 1f;

        /// <summary>
        ///     Changes the method used for anti aliasing.
        ///     -1 is default (None or FXAA depending on your settings), 0 is none, 1 is FXAA, 2 is SMAA, 3 is TAA.
        /// </summary>
        public static int AntiAliasingMode { get; set; } = -1;

        /// <summary>
        ///     Makes the game a bit less saturated.
        /// </summary>
        public static bool TweakColors { get; set; } = true;

        /// <summary>
        ///     Disables the depth of field effect.
        /// </summary>
        public static bool DisableDOF { get; set; } = false;

        /// <summary>
        ///     Disables the bloom effect.
        /// </summary>
        public static bool DisableBloom { get; set; } = false;

        /// <summary>
        ///     Disables color grading, making the final image much less vibrant.
        /// </summary>
        public static bool DisableColorGrading { get; set; } = false;

        /// <summary>
        ///     Disables the ambient occlusion effect.
        /// </summary>
        public static bool DisableAmbientOcclusion { get; set; } = false;

        /// <summary>
        ///     Disables the chromatic aberration effect.
        /// </summary>
        public static bool DisableChromaticAberration { get; set; } = false;

        /// <summary>
        ///     Disables the vignette effect.
        /// </summary>
        public static bool DisableVignette { get; set; } = false;

        /// <summary>
        ///     Scale of the HUD, as a multiplier.
        /// </summary>
        public static float HUDScale { get; set; } = 1f;

        /// <summary>
        ///     Whether if the HUD should be hidden during gameplay or not.
        /// </summary>
        public static bool HideHUD { get; set; } = false;

        /// <summary>
        ///     When the mod is loaded at the very start of the game.
        /// </summary>
        /// <param name="settings">Settings for the mod.</param>
        public static void OnModLoad(Dictionary<string, object> settings)
        {
            // Load the settings

            BaseFOV = (float)settings["BaseFOV"];
            LightingFix = (bool)settings["LightingFix"];

            PostProcessAmount = (float)settings["PostProcessAmount"];
            AntiAliasingMode = (int)(float)settings["AntiAliasingMode"];
            TweakColors = (bool)settings["TweakColors"];
            DisableDOF = (bool)settings["DisableDOF"];
            DisableBloom = (bool)settings["DisableBloom"];
            DisableColorGrading = (bool)settings["DisableColorGrading"];
            DisableAmbientOcclusion = (bool)settings["DisableAmbientOcclusion"];
            DisableChromaticAberration = (bool)settings["DisableChromaticAberration"];
            DisableVignette = (bool)settings["DisableVignette"];

            HUDScale = (float)settings["HUDScale"];
            HideHUD = (bool)settings["HideHUD"];

            _lightRots = new List<Quaternion>();
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModLateUpdate()
        {
            if (LightingFix)
            {
                var lights = Object.FindObjectsOfType<Light>();
                for (var i = 0; i < lights.Count; i++)
                {
                    var light = lights[i];
                    if (light.type == LightType.Directional)
                        light.transform.rotation = _lightRots[i];
                }
            }
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModUpdate()
        {
            var cameraController = Object.FindObjectOfType<CameraController>();
            if (cameraController != null)
            {
                // Set the field of view
                if (BaseFOV > 0)
                    cameraController.SetFieldOfView(BaseFOV);

                if (_canModifyPostProcess)
                {
                    // Fix the light source places in
                    if (LightingFix)
                    {
                        var lights = Object.FindObjectsOfType<Light>();
                        for (var i = 0; i < lights.Count; i++)
                        {
                            var light = lights[i];
                            _lightRots.Add(light.transform.rotation);
                        }
                    }

                    // Find the Post Process Volume
                    var postProcessVolume = cameraController.m_PostProcessVolume;

                    // Apply the settings
                    if (PostProcessAmount != 1f)
                        postProcessVolume.weight = PostProcessAmount;

                    if (AntiAliasingMode > -1)
                    {
                        var postProcessLayer = Object.FindObjectOfType<PostProcessLayer>();
                        if (postProcessLayer != null)
                            postProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing)AntiAliasingMode;
                    }

                    var profile = postProcessVolume.profile;

                    if (DisableDOF)
                    {
                        var dof = profile.GetSetting<DepthOfField>();
                        dof.enabled.value = false;
                    }

                    if (DisableBloom)
                    {
                        var bloom = profile.GetSetting<Bloom>();
                        bloom.enabled.value = false;
                    }
                    if (DisableColorGrading)
                    {

                        var colorGrading = profile.GetSetting<ColorGrading>();
                        colorGrading.enabled.value = false;
                    }
                    else if (TweakColors)
                    {
                        var colorGrading = profile.GetSetting<ColorGrading>();
                        colorGrading.saturation.value = 1.5f;
                    }

                    if (DisableAmbientOcclusion)
                    {
                        var ambientOcclusion = profile.GetSetting<AmbientOcclusion>();
                        ambientOcclusion.enabled.value = false;
                    }

                    if (DisableChromaticAberration)
                    {
                        var chromaticAberration = profile.GetSetting<ChromaticAberration>();
                        chromaticAberration.enabled.value = false;
                    }

                    if (DisableVignette)
                    {
                        var vignette = profile.GetSetting<Vignette>();
                        vignette.enabled.value = false;
                    }

                    // Let's not run all this every single frame
                    _canModifyPostProcess = false;
                }
            }
            else
            {
                _canModifyPostProcess = true;
                if (_lightRots.Count > 0)
                    _lightRots.Clear();
            }

            // Find the UI Manager
            var uiManager = Object.FindObjectOfType<UIManager>();

            // Ignore the following the UI Manager doesn't exist
            if (uiManager != null)
            {
                // Apply the settings
                if (HUDScale != 1f && uiManager.m_DisplayScale != HUDScale)
                    uiManager.m_DisplayScale = HUDScale;

                uiManager.m_DisplayHUD = false;

                if (HideHUD && MainGameUI.Hud.isActive)
                    MainGameUI.Hud.Deactivate();
            }
        }

        private static List<Quaternion> _lightRots { get; set; }
    }
}