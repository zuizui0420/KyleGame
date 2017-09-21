﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Damage"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DamageTex("DamageTex", 2D) = "white" {}
		_DamageRatio("DamageRatio", Float) = 1

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
		sampler2D _DamageTex;
		float _DamageRatio;

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, i.uv);

			fixed4 tex = tex2D(_DamageTex, i.uv);
			fixed4 c = lerp(col, tex, tex.a);

			fixed4 result = lerp(col, c, _DamageRatio);

		return result;
		}
			ENDCG
		}
		}
}
