// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Fader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Fade("_Fade", Float) = 1
		_IsAddisive("_IsAddisive", Int) = 1
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				uniform float _Timer;
				sampler2D _MainTex;
				float _Fade;
				int _IsAddisive;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed4 black = fixed4(_Fade, _Fade, _Fade, 0);
					if (_IsAddisive > 0) {
						col += black;
					}
					else {
						col *= 1 - black;
					}

					return col;
				}
				ENDCG
			}
		}
}
