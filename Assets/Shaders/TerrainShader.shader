Shader "Custom/TerrainShader"
{
   Properties {
	   _MainTex ("Texture", 2D) = "white" {}
	   _WallTex ("Wall Texture", 2D) = "white" {}
	   _TexScale("Texture Scale", Float) = 1
	   _TriplanarBlendSharpness ("Blend Sharpness",float) = 1

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
		float _TriplanarBlendSharpness;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutputStandard o){

			// Find our UVs for each axis based on world position of the fragment.
			half2 yUV = IN.worldPos.xz / _TexScale;
			half2 xUV = IN.worldPos.zy / _TexScale;
			half2 zUV = IN.worldPos.xy / _TexScale;
			// Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
			half3 yDiff = tex2D (_MainTex, yUV);
			half3 xDiff = tex2D (_MainTex, xUV);
			half3 zDiff = tex2D (_MainTex, zUV);
			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);
			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
			// Finally, blend together all three samples based on the blend mask.
			o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;


			
			/*float3 scaledWorldPos = IN.worldPos / _TexScale;

			// Weight of each axis of the world vertex normal. Used for blending the textures.
			// We use the absolute value so we can blend more of a given axis if it points strongly in the positive or negative direction
			float3 pWeight = abs(IN.worldNormal);
			pWeight /= (pWeight.x + pWeight.y + pWeight.z);

			float3 xProj = tex2D (_WallTex, scaledWorldPos.yz) * pWeight.x;
			float3 yProj = tex2D (_MainTex, scaledWorldPos.xz) * pWeight.y;
			float3 zProj = tex2D (_WallTex, scaledWorldPos.xy) * pWeight.z;

			o.Albedo = xProj + yProj + zProj;*/

		}

		ENDCG
	}
	Fallback "Diffuse"
}
