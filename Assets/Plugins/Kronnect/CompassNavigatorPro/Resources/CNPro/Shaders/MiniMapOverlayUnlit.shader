Shader "CompassNavigatorPro/MiniMapOverlayUnlit"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_MiniMapTex ("MiniMap Render Texture", 2D) = "black" {}
		_BorderTex ("Border Texture", 2D) = "black" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}

		_FogOfWarTex ("Fog Of War Tex", 2D) = "black" {}
		_FogOfWarTintColor ("Fog Of War Color", Color) = (1,1,1,1)

		_NoiseTex ("Noise (RGB)", 2D) = "white" {}
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		_UVOffset ("UV Offset", Vector) = (0,0,0,1)
		_UVFogOffset("Fog Offset", Vector) = (0,0,0)
		_Rotation("Rotation", Float) = 0
		_ConeRotation("Cone Rotation", Float) = 0
		_Effects("Effects", Vector) = (1,1,0,0)

		_LUTTexture ("LUT Texture", 2D) = "black" {}
		_VignetteColor("Vignette Color", Color) = (0,0,0,1)
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_BackgroundColor("Background Color", Color) = (0,0,0,1)
		_BackgroundOpaque("Background Opaque", Int) = 0

		_RingsData("Rings Data", Vector) = (1, 50, 0, 0)
		_RingsColor("Rings Color", Color) = (1,1,1,1)
		_RingsPulse("Rings Pulse", Vector) = (1,1,1,1)

		_ViewConeColor("View Cone Color", Color) = (1,1,1, 0.25)
		_ViewConeData("View Conde Data", Vector) = (1,1,1)
		_ViewConeOutlineColor("View Cone Outline Color", Color) = (1,1,1,1)

		_FollowPos("Follow Pos", Vector) = (0.5, 0.5, 0)

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always // [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile _ UNITY_UI_ALPHACLIP
			#pragma multi_compile_local _ COMPASS_FOG_OF_WAR COMPASS_RADAR
			#pragma multi_compile_local _ COMPASS_ROTATED
			#pragma multi_compile_local _ COMPASS_LUT
			#pragma multi_compile_local _ COMPASS_VIEW_CONE COMPASS_VIEW_CONE_OUTLINE

			#define PI 3.1415927
			#define PI2 PI * 2.0

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float2 mapUV    : TEXCOORD2;
				#if COMPASS_FOG_OF_WAR
					float2 fogUV    : TEXCOORD3;
				#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex, _MiniMapTex, _BorderTex, _MaskTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float4 _UVOffset;
			float2 _FollowPos;

			sampler2D _FogOfWarTex, _NoiseTex;
			fixed4 _FogOfWarTintColor;
			float4 _UVFogOffset;
			float _Rotation;
			float _ConeRotation;
			half4 _Effects;
			sampler2D _LUTTex;
			float4 _LUTTex_TexelSize;
			fixed4 _VignetteColor;
			fixed4 _ViewConeOutlineColor;
			fixed4 _TintColor;
			fixed4 _BackgroundColor;
			fixed _BackgroundOpaque;
			fixed4 _RingsColor;
			float2 _RingsData;
			#define RINGS_SCALE _RingsData.x
			#define RINGS_WIDTH _RingsData.y

			float4 _RingsPulse;
			#define PULSE_SPEED _RingsPulse.x
			#define PULSE_AMPLITUDE _RingsPulse.y
			#define PULSE_FREQUENCY _RingsPulse.z
			#define PULSE_OPACITY _RingsPulse.w

			fixed4 _ViewConeColor;
			float3 _ViewConeData;
			#define VIEW_CONE_FOV _ViewConeData.x
			#define VIEW_CONE_DIST _ViewConeData.y
			#define VIEW_CONE_FALLOFF _ViewConeData.z

			#define dot2(x) dot(x,x)

			#if COMPASS_ROTATED
			float2 Rotate(float2 uv) {
				uv -= 0.5;
				float s, c;
				sincos(_Rotation, s, c);
				float2x2 rotationMatrix = float2x2(c, -s, s, c);
				uv = mul(uv, rotationMatrix);
				uv += 0.5; 
				return uv;
			}
			#else
				#define Rotate(v) v
			#endif

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_OUTPUT(v2f, OUT);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				float2 rotatedUV = Rotate(IN.texcoord);

				float2 uv = rotatedUV - _UVOffset.xy / _UVOffset.z;
				uv.y = (uv.y-0.5) * _UVOffset.w + 0.5;
				uv = (uv-0.5) * _UVOffset.z + 0.5;
				OUT.mapUV = uv;

				#if COMPASS_FOG_OF_WAR
					uv = rotatedUV - _UVFogOffset.xy;
					uv.y = (uv.y-0.5) * _UVOffset.w + 0.5;
					uv = (uv-0.5) * _UVFogOffset.zw + 0.5;
					OUT.fogUV = uv; 
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			#if COMPASS_FOG_OF_WAR
			fixed4 GetFogOfWar(float2 uv) {
				fixed fogAlpha = tex2D (_FogOfWarTex, uv).a;
                half vxy = (uv.x + uv.y);
				half wt = _Time[1] * 0.5;
				half2 waveDisp1 = half2(wt + cos(wt+uv.y * 32.0) * 0.125, 0) * 0.05;
				fixed4 fog1 = tex2D(_NoiseTex, (uv + waveDisp1) * 8);
                wt *= 1.1;
				half2 waveDisp2 = half2(wt + cos(wt+uv.y * 8.0) * 0.5, 0) * 0.05;
				fixed4 fog2 = tex2D(_NoiseTex, (uv + waveDisp2) * 2);
                fixed4 fog = (fog1 + fog2) * 0.5;
                fog.rgb *= _FogOfWarTintColor.rgb;
                fog.a = fogAlpha * _FogOfWarTintColor.a;
                return fog;
            }
            #endif

			fixed ComputeOutline(float2 uv) {
				float2 dd = _FollowPos - uv;
				float ang = atan2(dd.y, dd.x) + PI * 2.5 - _ConeRotation;
				ang %= PI2;
				float dang = abs(ang);
				float ddist = dot2(dd);

				if (ddist < VIEW_CONE_DIST) {
					if (dang < VIEW_CONE_FOV || dang > PI2 - VIEW_CONE_FOV) {
						return 1;
					}
				}
				return 0;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 border = (tex2D(_BorderTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				fixed4 mask = tex2D(_MaskTex, IN.texcoord);

				border.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				#ifdef UNITY_UI_ALPHACLIP
					clip (border.a - 0.001);
				#endif

				#if COMPASS_RADAR
					fixed4 minimapTex = 0;
				#else
					fixed4 minimapTex = tex2D(_MiniMapTex, IN.mapUV);
					minimapTex = lerp(minimapTex, _BackgroundColor, 1.0 - minimapTex.a);
					if (_BackgroundOpaque) minimapTex.a = 1;

					// Clip out of range area
					minimapTex *= max( abs(IN.mapUV.x - 0.5), abs(IN.mapUV.y - 0.5))< 0.499;

					#if COMPASS_LUT
						float3 lutST = float3(_LUTTex_TexelSize.x, _LUTTex_TexelSize.y, _LUTTex_TexelSize.w - 1);
						float3 lookUp = saturate(minimapTex.rgb) * lutST.zzz;
    					lookUp.xy = lutST.xy * (lookUp.xy + 0.5);
    					float slice = floor(lookUp.z);
    					lookUp.x += slice * lutST.y;
	    				float2 lookUpNextSlice = float2(lookUp.x + lutST.y, lookUp.y);
						fixed3 lut = lerp(tex2D(_LUTTex, lookUp.xy).rgb, tex2D(_LUTTex, lookUpNextSlice).rgb, lookUp.z - slice);
			    		minimapTex.rgb = lerp(minimapTex.rgb, lut, _Effects.z);
					#endif
				#endif

				// Apply fog of war?
				#if COMPASS_FOG_OF_WAR
					fixed4 fogOfWar = GetFogOfWar(IN.fogUV);
					minimapTex.rgb = lerp(minimapTex.rgb, fogOfWar.rgb, fogOfWar.a);
				#endif

				// Brightness & Contrast for map
				minimapTex.rgb = (minimapTex.rgb - 0.5.xxx) * _Effects.y + 0.5.xxx;
				minimapTex.rgb *= _Effects.x;

				#if COMPASS_RADAR
					// pulse
					float dist = length(0.5 - IN.texcoord) * 2.0;
					float x = 1.0 - saturate( (0.5 - abs(0.5 - frac( dist/RINGS_SCALE))) * RINGS_WIDTH);
					x = max(x, saturate(1.0 - frac(_Time.x * PULSE_SPEED - dist * PULSE_FREQUENCY) * PULSE_AMPLITUDE) * PULSE_OPACITY);

					// cross
					float2 crd = abs(0.5 - IN.mapUV);
					float cr = 1.0 - min(crd.x, crd.y);
					x = max( saturate( (cr - 0.993) / 0.005), x);

					// radial gradient
					x *= 1.0 - saturate( dot2(dist));

					minimapTex.rgb = _RingsColor.rgb;
					minimapTex.a = x * _RingsColor.a;
				#endif

				// Add LOS cone
				#if COMPASS_VIEW_CONE || COMPASS_VIEW_CONE_OUTLINE
					float2 dd = _FollowPos - IN.texcoord;
					float ang = atan2(dd.y, dd.x) + PI * 2.5 - _ConeRotation;
					ang %= PI2;
					float dang = abs(ang);
					float ddist = dot2(dd);

					float outline = 0;
					if (ddist < VIEW_CONE_DIST) {
						if (dang < VIEW_CONE_FOV || dang > PI2 - VIEW_CONE_FOV) {
							outline = 0.35;
							fixed viewConeAlpha = _ViewConeColor.a * saturate(1.0 - ddist / VIEW_CONE_FALLOFF);
							_ViewConeColor.a = max(_ViewConeColor.a, minimapTex.a); // avoids seethrough fow
							minimapTex = lerp(minimapTex, _ViewConeColor, viewConeAlpha);
						}
					}

					#if COMPASS_VIEW_CONE_OUTLINE
						const float s = 1.5;
						fixed outline1 = ComputeOutline(IN.texcoord - float2(s, s) / _ScreenParams.xy);
						fixed outline2 = ComputeOutline(IN.texcoord + float2(s, s) / _ScreenParams.xy);
						fixed outline3 = ComputeOutline(IN.texcoord - float2(-s, s) / _ScreenParams.xy);
						fixed outline4 = ComputeOutline(IN.texcoord + float2(-s, s) / _ScreenParams.xy);

						outline = (outline + outline1 + outline2 + outline3 + outline4) / 5.0;
						minimapTex = lerp(minimapTex, half4(_ViewConeOutlineColor.rgb, 1.0), fwidth(outline) * _ViewConeOutlineColor.a);
					#endif

				#endif

				// Vignette
				#if !COMPASS_RADAR
					minimapTex.rgb = lerp(minimapTex.rgb, _VignetteColor.rgb, saturate( _Effects.w * dot2(dot2( 0.5 - IN.texcoord))));
					minimapTex.rgb = lerp(minimapTex.rgb, _TintColor.rgb, _TintColor.a);
				#endif

				// Mask & border
				fixed4 color = mask * minimapTex * IN.color;

				color = lerp(color, border, border.a);

				return color;
			}
		ENDCG
		}
	}
}