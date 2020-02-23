// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DissolveShader"
{
	Properties
	{
	   _MainTex("Texture (RGB)", 2D) = "white" {}
	   _SliceGuide("SliceGuide (RGB)", 2D) = "white" {}
	   _SliceAmount("SliceAmount", Range(0.0, 1.0)) = 0.5
	   _DissolveColor("DissolveColor", Color) = (1,0,0,1)
	   _DissolveEmission("DissolveEmission", Range(0,1)) = 0.5
	   _DissolveWidth("DissolveWidth", Range(0,0.1)) = 0.05
	}
		SubShader
	   {
		  Tags { "RenderType" = "Opanque" }
		  Cull Off
		  CGPROGRAM
		  #pragma surface surf Lambert
		  struct Input
		  {
			 float2 uv_MainTex;
			 float2 uv_SliceGuide;
		  };
		  sampler2D _MainTex;
		  sampler2D _SliceGuide;
		  fixed _Mask;
		  float2 _test;
		  fixed _SliceAmount;
		  fixed4 _DissolveColor;
		  fixed _DissolveEmission;
		  fixed _DissolveWidth;
		  void surf(Input IN, inout SurfaceOutput o)
		  {
			  _test = IN.uv_SliceGuide;
			 _Mask = tex2D(_SliceGuide, _test).r;
			 if (_Mask < _SliceAmount)
				discard;
			 else
			 {
				if ((_Mask < _SliceAmount + _DissolveWidth) && _SliceAmount != 0)
				{
				   o.Albedo = _DissolveColor;
				   o.Emission = _DissolveColor + _DissolveEmission;
				}
				else
				   o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			 }
		  }

		  ENDCG
	   }
		   //Properties
		   //{
		   //	_MainTex("Texture", 2D) = "white" {}
		   //	_SliceGuide("SliceGuide", 2D) = "white" {}
		   //	_SliceAmount("SliceAmount", Range(0.0, 1.0)) = 0.5
		   //	_DissolveColor("DissolveColor", Color) = (1,1,0,1)
		   //    _DissolveEmission("DissolveEmission", Range(0,1)) = 1
		   //    _DissolveWidth("DissolveWidth", Range(0,0.1)) = 0.05
		   //}
		   //	SubShader
		   //	{
		   //		Pass
		   //		{
		   //			Tags{ "LightMode" = "ForwardBase" }

		   //			CGPROGRAM
		   //	#pragma vertex vert
		   //	#pragma fragment frag

		   //	#include "UnityCG.cginc"
		   //	#include "Lighting.cginc"

		   //			sampler2D _MainTex;
		   //			sampler2D _SliceGuide;
		   //			fixed _SliceAmount;
		   //			fixed4 _DissolveColor;
		   //			fixed _DissolveEmission;
		   //			fixed _DissolveWidth;
		   //			fixed4 col;
		   //			struct appdata
		   //			{
		   //				float4 vertex : POSITION;
		   //				float3 normal : NORMAL;
		   //				float2 uv : TEXCOORD0;
		   //			};

		   //			struct v2f
		   //			{
		   //				float4 vertex : SV_POSITION;
		   //				float2 uv : TEXCOORD0;
		   //				float3 worldNormal : TEXCOORD1;
		   //				float3 viewDir : TEXCOORD2;
		   //				float3 lightDir : TEXCOORD3;
		   //				float2 uv2 : TEXCOORD4;
		   //			};

		   //			v2f vert(appdata v)
		   //			{
		   //				v2f o;
		   //				o.vertex = UnityObjectToClipPos(v.vertex);
		   //				o.uv2 = v.uv * 500;
		   //				o.uv = v.uv;

		   //				o.worldNormal = UnityObjectToWorldNormal(v.normal);

		   //				float3 lightDir = normalize(_WorldSpaceLightPos0);
		   //				o.lightDir = lightDir;
		   //				o.viewDir = normalize(_WorldSpaceCameraPos - o.worldNormal);

		   //				return o;
		   //			}

		   //			fixed4 frag(v2f i) : SV_Target
		   //			{
		   //				// 주변광
		   //				float4 ambientReflection = 1.0 * UNITY_LIGHTMODEL_AMBIENT;

		   //				// 확산광
		   //				float3 worldNormal = normalize(i.worldNormal);
		   //				float3 lightDir = i.lightDir;
		   //				float3 diffuseReflection = 1.0 * _LightColor0.rgb * saturate(dot(worldNormal, lightDir));

		   //				fixed _Mask = tex2D(_SliceGuide, i.uv2).r;
		   //				if (_Mask < _SliceAmount)
		   //					discard;
		   //				else
		   //				{
		   //					if ((_Mask < _SliceAmount + _DissolveWidth) && _SliceAmount != 0)
		   //					{
		   //						col = _DissolveColor + _DissolveEmission;
		   //					}
		   //					else
		   //						col = tex2D(_MainTex, i.uv);
		   //				}
		   //				col = col * float4(diffuseReflection, 1.0);
		   //				//col = col * float4(ambientReflection + diffuseReflection, 1.0) + float4(specularReflection, 1.0);
		   //				return col;
		   //			}
		   //		ENDCG
		   //		
		   //	}
		   //}

			  Fallback "Diffuse"
}