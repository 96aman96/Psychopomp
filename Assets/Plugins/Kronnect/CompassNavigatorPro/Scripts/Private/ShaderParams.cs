using UnityEngine;

namespace CompassNavigatorPro {

    public partial class CompassPro : MonoBehaviour {

        static class ShaderParams {
            // textures
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int MiniMapTex = Shader.PropertyToID("_MiniMapTex");
            public static int MaskTex = Shader.PropertyToID("_MaskTex");
            public static int BorderTex = Shader.PropertyToID("_BorderTex");
            public static int FoWTexture = Shader.PropertyToID("_FogOfWarTex");

            public static int FollowPos = Shader.PropertyToID("_FollowPos");
            public static int Rotation = Shader.PropertyToID("_Rotation");
            public static int ConeRotation = Shader.PropertyToID("_ConeRotation");
            public static int UVOffset = Shader.PropertyToID("_UVOffset");
            public static int UVFogOffset = Shader.PropertyToID("_UVFogOffset");
            public static int FoWTintColor = Shader.PropertyToID("_FogOfWarTintColor");
            public static int Effects = Shader.PropertyToID("_Effects");
            public static int LUTTexture = Shader.PropertyToID("_LUTTex");

            public static int VignetteColor = Shader.PropertyToID("_VignetteColor");
            public static int RingsData = Shader.PropertyToID("_RingsData");
            public static int RingsColor = Shader.PropertyToID("_RingsColor");
            public static int RingsPulseData = Shader.PropertyToID("_RingsPulse");

            public static int ViewConeColor = Shader.PropertyToID("_ViewConeColor");
            public static int ViewConeData = Shader.PropertyToID("_ViewConeData");
            public static int ViewConeOutlineColor = Shader.PropertyToID("_ViewConeOutlineColor");

            public static int Color = Shader.PropertyToID("_Color");
            public static int Angle = Shader.PropertyToID("_Angle");
            public static int TintColor = Shader.PropertyToID("_TintColor");
            public static int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
            public static int BackgroundOpaque = Shader.PropertyToID("_BackgroundOpaque");

            public static int OffscreenIndicatorIconTexture = Shader.PropertyToID("_IconTex");

            public static int CompassData = Shader.PropertyToID("_CompassData");
            public static int CompassIP = Shader.PropertyToID("_CompassIP");
            public static int CompassAngle = Shader.PropertyToID("_CompassAngle");
            public static int TicksSize = Shader.PropertyToID("_TicksSize");
            public static int TicksColor = Shader.PropertyToID("_TicksColor");

            public static int FXData = Shader.PropertyToID("_FXData");

            public static int CircleStartRadius = Shader.PropertyToID("_StartRadius");
            public static int CircleInnerColor = Shader.PropertyToID("_InnerColor");

            public const string SKW_COMPASS_LUT = "COMPASS_LUT";
            public const string SKW_COMPASS_FOG_OF_WAR = "COMPASS_FOG_OF_WAR";
            public const string SKW_COMPASS_ROTATED = "COMPASS_ROTATED";
            public const string SKW_COMPASS_INDICATOR_KEEPSIZE = "COMPASS_INDICATOR_KEEPSIZE";
            public const string SKW_COMPASS_RADAR = "COMPASS_RADAR";
            public const string SKW_COMPASS_VIEW_CONE = "COMPASS_VIEW_CONE";
            public const string SKW_COMPASS_VIEW_CONE_OUTLINE = "COMPASS_VIEW_CONE_OUTLINE";
            public const string SKW_TICKS = "TICKS";
            public const string SKW_TICKS_180 = "TICKS_180";
            public const string SKW_TICKS_360 = "TICKS_360";
        }
    }



}



