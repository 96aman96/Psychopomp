using UnityEngine;

#if USES_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace CompassNavigatorPro {

    public static class CNP2HDRPCameraSetup {

#if USES_HDRP

        public static bool usesHDRP = true;

        public static void SetupHDRPCamera(Camera cam, bool renderShadows, Color backgroundColor) {
            HDAdditionalCameraData camData = cam.GetComponent<HDAdditionalCameraData>();
            if (camData == null) camData = cam.gameObject.AddComponent<HDAdditionalCameraData>();
            camData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
            camData.backgroundColorHDR = backgroundColor;

            FrameSettingsOverrideMask frameSettingsOverrideMask = camData.renderingPathCustomFrameSettingsOverrideMask;
            camData.customRenderingSettings = true;
            bool bitMask = !renderShadows;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.ShadowMaps] = bitMask;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.Shadowmask] = bitMask;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.ScreenSpaceShadows] = bitMask;
            camData.renderingPathCustomFrameSettingsOverrideMask = frameSettingsOverrideMask;

        }
#else
        public static bool usesHDRP = false;

        public static void SetupHDRPCamera(Camera cam, bool renderShadows, Color backgroundColor) { }

#endif

    }
}
