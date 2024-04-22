Shader "CompassNavigatorPro/Sprite Curved"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_TintColor ("Back Tint Color", Color) = (1,1,1,1)
		_TicksSize("Ticks Size", Vector) = (1,1,22.5,0)
		_TicksColor("Ticks Color", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _CompassData("Compass Width", Vector) = (1,0.5,0,0)
		[HideInInspector] _FXData ("FX Data", Vector) = (0,0,0,0)
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

		ZTest Always // [unity_GUIZTestMode]
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#pragma multi_compile_local _ TICKS TICKS_180 TICKS_360
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 scrPos   : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TintColor;
			float4 _FXData;
			float3 _CompassData;
			#define COMPASS_WIDTH _CompassData.x
			#define COMPASS_CENTER _CompassData.y
			#define COMPASS_VISIBLE_WIDTH _CompassData.z

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _CompassAngle;
			float4x4 _CompassIP;
			fixed4 _TicksColor;
			float3 _TicksSize;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_OUTPUT(v2f, OUT);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				// Get screen position
				float4 pos = UnityObjectToClipPos(IN.vertex);
				float4 screenPos = ComputeScreenPos(pos);
				float scrPosX = screenPos.x / screenPos.w;

				pos.y -= sin(scrPosX * 3.1415927) * _FXData.x;
				OUT.texcoord = IN.texcoord;

				float distToEdge = _FXData.y - abs(scrPosX - COMPASS_CENTER) * 2 + 0.001;
				float fadeOut = saturate((distToEdge - _FXData.w) / (_FXData.z + 0.0001));
				OUT.color = IN.color * _Color;
				OUT.color.a *= fadeOut;
				screenPos.z = fadeOut;
				OUT.scrPos = screenPos;

				#ifdef PIXELSNAP_ON
					pos = UnityPixelSnap (pos);
				#endif
					OUT.pos = pos;
				return OUT;
			}


			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
					// get the color from an external texture (usecase: Alpha support for ETC1 on android)
					color.a = tex2D (_AlphaTex, uv).r;
				#endif

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target {

				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color * _TintColor;
				
				#if TICKS || TICKS_180 || TICKS_360
					float2 uv = IN.scrPos.xy / IN.scrPos.w;
					float2 scrPos = 2.0 * (uv - COMPASS_CENTER) / COMPASS_WIDTH;
					#if TICKS_180
						float x = (scrPos.xy + COMPASS_CENTER) * 90.0;
					#elif TICKS_360
						float x = (scrPos.xy + COMPASS_CENTER) * 180.0;
					#else
						float3 vpos = mul(_CompassIP, float4(scrPos.xy, 0.5, 1));
						float x = atan2(vpos.z, vpos.x) + 3.1415927;
						x *= 180.0 / 3.1415927; // optimized by compiler
					#endif
					x += _CompassAngle;
					fixed markInterval = _TicksSize.z;
					fixed mark = step(abs(fmod(x + _TicksSize.x * 0.5, markInterval)), _TicksSize.x);

					mark *= abs(IN.texcoord.y - 0.5) < _TicksSize.y;

					// hide if exceeds caps
					mark *= abs(uv.x - 0.5) < COMPASS_VISIBLE_WIDTH;

					// fade out
					mark *= IN.scrPos.z;

					c = lerp(c, _TicksColor, mark);

				#endif

				c.rgb *= c.a;

				return c;
			}
		ENDCG
		}
	}
}
