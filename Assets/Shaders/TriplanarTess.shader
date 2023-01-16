Shader "Custom/TriplanarTess"
{
    Properties
    {
        _Tess ("Tessellation", Range(1,32)) = 4
        _SnowColor ("SnowColor", Color) = (1,1,1,1)
        _SnowTex ("Snow (RGB)", 2D) = "white" {}
        _GroundColor ("GroundColor", Color) = (1,1,1,1)
        _GroundTex ("Ground (RGB)", 2D) = "white" {}
        _Splat ("SplatMap", 2D) = "black" {}
        
        _Displacement ("Displacement", Range(0, 1.0)) = 0.3
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _TexScale("Texture Scale", Float) = 1
	    _TriplanarBlendSharpness ("Blend Sharpness",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:disp tessellate:tessDistance

        // Required for Tessellation
        #pragma target 4.6

        #include "Tessellation.cginc" // required

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0; // For uv of displacement map
        };

        float _Tess;

        float4 tessDistance (appdata v0, appdata v1, appdata v2) {
            float minDist = 10.0;
            float maxDist = 25.0;
            return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
        }

        sampler2D _Splat;
        float _Displacement;
        float _TexScale;
		float _TriplanarBlendSharpness;

        void disp (inout appdata v)
        {
            half2 yUV = v.vertex.xz / _TexScale;
			half2 xUV = v.vertex.zy / _TexScale;
			half2 zUV = v.vertex.xy / _TexScale;

            // Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
			half3 yDiff = tex2D (_Splat, yUV);
			half3 xDiff = tex2D (_Splat, xUV);
			half3 zDiff = tex2D (_Splat, zUV);
            // Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(v.normal), _TriplanarBlendSharpness);
			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
			// Finally, blend together all three samples based on the blend mask.
			//o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;

            float d = tex2Dlod(_Splat, float4(xUV.xy,0,0)).r * blendWeights.x;
            d += tex2Dlod(_Splat, float4(yUV.xy,0,0)).r * blendWeights.y;
            d += tex2Dlod(_Splat, float4(zUV.xy,0,0)).r * blendWeights.z;
            d *= _Displacement;

            //float d = tex2Dlod(_Splat, float4(v.texcoord.xy,0,0)).r * _Displacement;
            // Depress vertices downward 
            v.vertex.xyz -= v.normal * d;
            // Offset the final result by 1x the max displacement so the surface collider sits at the bottom of the depression
            v.vertex.xyz += v.normal * _Displacement;
        }

        sampler2D _GroundTex;
        fixed4 _GroundColor;
        sampler2D _SnowTex;
        fixed4 _SnowColor;

        struct Input
        {
            float2 uv_GroundTex;
            float2 uv_SnowTex;
            float2 uv_Splat;
            float3 worldPos;
			float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Find our UVs for each axis based on world position of the fragment.
			half2 yUV = IN.worldPos.xz / _TexScale;
			half2 xUV = IN.worldPos.zy / _TexScale;
			half2 zUV = IN.worldPos.xy / _TexScale;
			// Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
			half3 yDiffSnow = tex2D (_SnowTex, yUV);
			half3 xDiffSnow = tex2D (_SnowTex, xUV);
			half3 zDiffSnow = tex2D (_SnowTex, zUV);

            half3 yDiffGround = tex2D (_GroundTex, yUV);
			half3 xDiffGround = tex2D (_GroundTex, xUV);
			half3 zDiffGround = tex2D (_GroundTex, zUV);

            half3 yDiffSplat = tex2D (_Splat, yUV);
			half3 xDiffSplat = tex2D (_Splat, xUV);
			half3 zDiffSplat = tex2D (_Splat, zUV);
			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);
			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
			// Finally, blend together all three samples based on the blend mask.
			//o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;

            // Albedo comes from a texture tinted by color
            fixed4 snow = tex2D (_SnowTex, xUV.xy) * blendWeights.x; //* _SnowColor;
            snow += tex2D (_SnowTex, yUV.xy) * blendWeights.y;
            snow += tex2D (_SnowTex, zUV.xy) * blendWeights.z;
            snow *= _SnowColor;

            fixed4 ground = tex2D (_GroundTex, xUV.xy) * blendWeights.x;
            ground += tex2D (_GroundTex, yUV.xy) * blendWeights.y;
            ground += tex2D (_GroundTex, zUV.xy) * blendWeights.z;
            ground *= _GroundColor;

            //fixed4 ground = tex2D (_GroundTex, IN.uv_GroundTex) * _GroundColor;
            half amount = tex2Dlod(_Splat, float4(xUV.xy,0,0)).r * blendWeights.x;
            amount += tex2Dlod(_Splat, float4(yUV.xy,0,0)).r * blendWeights.y;
            amount += tex2Dlod(_Splat, float4(zUV.xy,0,0)).r * blendWeights.z;

            fixed4 c = lerp(snow, ground, amount);
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
