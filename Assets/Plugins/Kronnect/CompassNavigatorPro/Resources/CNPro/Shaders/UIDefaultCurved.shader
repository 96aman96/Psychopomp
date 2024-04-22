Shader "CompassNavigatorPro/UI Default Curved"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		[HideInInspector] _CompassData("Compass Width", Vector) = (1,0.5,0,0)
		[HideInInspector] _FXData ("FX Data", Vector) = (0,0,0,0)

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
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

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
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _FXData;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			float3 _CompassData;
			#define COMPASS_CENTER _CompassData.y

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

				OUT.vertex = pos;
				OUT.texcoord = IN.texcoord;

				float distToEdge = _FXData.y - abs(scrPosX - COMPASS_CENTER) * 2 + 0.001;
				OUT.color = IN.color * _Color;

				float fadeOut = saturate((distToEdge - _FXData.w) / (_FXData.z + 0.0001));
				if (_FXData.z > 0) {
					OUT.color.a *= fadeOut;
				}

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				#ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}
