using UnityEngine;

#if USES_URP
using UnityEngine.Rendering.Universal;
#endif

namespace CompassNavigatorPro {

    public static class CNP2URPCameraSetup {

#if USES_URP

        public static bool usesURP = true;

        public static void SetupURPCamera(Camera cam, bool renderShadows) {
            UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (camData == null) camData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
            if (camData != null) {
                camData.renderShadows = renderShadows;
            }
        }
#else
        public static bool usesURP = false;

        public static void SetupURPCamera(Camera cam, bool renderShadows) { }
#endif


        }
}

