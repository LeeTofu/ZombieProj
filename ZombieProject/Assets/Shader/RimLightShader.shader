// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RimLightShader"
{
	Properties
	{
			_MainTex("Albedo", 2D) = "white" {}
			_BumpMap("NormalMap", 2D) = "bump" {}
			_RimColor("RimColor", Color) = (1,1,1,1)
			_RimPower("RimPower", Range(0, 10)) = 1
	}

	SubShader
	{
			Tags { "RenderType" = "Opanque" }
			CGPROGRAM
			#pragma surface surf Lambert

			sampler2D _MainTex;
			sampler2D _BumpMap;
			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 viewDir;
			};

			float4 _RimColor;
			float _RimPower;

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				float rim = saturate(dot(o.Normal, IN.viewDir));
				o.Emission = pow(1 - rim, _RimPower) * _RimColor.rgb;
				o.Alpha = c.a;
			}
			ENDCG
	}
	FallBack "Diffuse"
}
