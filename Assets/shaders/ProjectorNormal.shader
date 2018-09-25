// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Normal" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_CookieTex ("Cookie", 2D) = "" {}
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend OneMinusSrcAlpha SrcAlpha
			Offset -1, -2
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvCookie : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvCookie = mul (unity_Projector, vertex);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _CookieTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texS = tex2Dproj (_CookieTex, UNITY_PROJ_COORD(i.uvCookie));
				texS.rgb *= _Color.rgb;
				texS.a = 1.0-texS.a;
				return texS;
			}
			ENDCG
		}
	}
}
