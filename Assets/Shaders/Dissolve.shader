Shader "Custom/Dissolve"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		[HDR]_EdgeColor("EdgeColor", Color) = (0,0,0,0)
		_DissolveAmount("DissolveAmount", Range( 0 , 1)) = 1
		_EdgeThickness("EdgeThickness", Range( 0 , 0.15)) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform sampler2D _NoiseTex;
		uniform float4 _EdgeColor;
		uniform float _DissolveAmount;
		uniform float _EdgeThickness;

		void surf(Input i , inout SurfaceOutputStandard o)
		{
			o.Albedo = (_Tint * tex2D( _Albedo, i.uv_texcoord)).rgb;
			float noise = tex2D(_NoiseTex, i.uv_texcoord).r; //noise is recorded in all channels r, g, and b
			o.Emission = (_EdgeColor * step(noise , (_DissolveAmount + _EdgeThickness))).rgb;
			o.Alpha = 1;
			clip(noise - _DissolveAmount); //clip texels that have a noise value that is <= dissolve amount
		}
		ENDCG
	}
	Fallback "Diffuse"
}
