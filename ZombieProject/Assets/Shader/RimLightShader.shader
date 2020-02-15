// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RimLightShader"
{
	Properties
	{
			_MainTex("Albedo", 2D) = "white" {}
			_RimColor("RimColor", Color) = (1,1,1,1)
			_RimPower("RimPower", Range(0, 10)) = 1
	}

		SubShader
			{
					Tags { "RenderType" = "Opanque" }
					CGPROGRAM
					#pragma surface surf Lambert

					sampler2D _MainTex;
					struct Input {
						float2 uv_MainTex;
						float3 viewDir;
					};

					float4 _RimColor;
					float _RimPower;

					void surf(Input IN, inout SurfaceOutput o)
					{
						half rim = 1.0 - saturate(dot(o.Normal, IN.viewDir));

						if (rim < 0.6)
						{
							o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
						}
						else
						{
							o.Emission = pow(rim, _RimPower) * _RimColor.rgb;
							o.Albedo = _RimColor.rgb;
						}
					}
					ENDCG
			}
				FallBack "Diffuse"
}
