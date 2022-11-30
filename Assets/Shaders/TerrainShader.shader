Shader "Custom/TerrainShader"
{
   Properties {
	   _MainTex ("Texture", 2D) = "white" {}
	   _WallTex ("Wall Texture", 2D) = "white" {}
	   _TexScale("Texture Scale", Float) = 1

	}

	SubShader {

		Tags {"RenderType" = "Opaque"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _WallTex;
		float _TexScale;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutputStandard o){
			
			float3 scaledWorldPos = IN.worldPos / _TexScale;

			float3 pWeight = abs(IN.worldNormal);
			pWeight /= (pWeight.x + pWeight.y + pWeight.z);

			float3 xProj = tex2D (_WallTex, scaledWorldPos.yz) * pWeight.x;
			float3 yProj = tex2D (_MainTex, scaledWorldPos.xz) * pWeight.y;
			float3 zProj = tex2D (_WallTex, scaledWorldPos.xy) * pWeight.z;

			o.Albedo = xProj + yProj + zProj;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
