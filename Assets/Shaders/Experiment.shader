Shader "Custom/SnowTrailShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _SplatTex ("Splat Map", 2D) = "white" {}
        _DisplacementStrength ("Displacement Strength", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard tessellatephong
        #include "UnityStandardTessellation.cginc"

        struct Input
        {
            float2 uv_TexCoord0;
            float3 worldPos;
            float3 worldNormal;
            float4 color : COLOR;
        };

        void tessellate(
            inout appdata_tess tessellatedData,
            const InputPatch<appdata_base, 3> patch,
            const int i)
        {
            tessellatedData = patch[i];
            tessellatedData.texcoord = patch[i].uv_TexCoord0;
        }

        sampler2D _SplatTex;
        float _DisplacementStrength;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 splat = tex2D(_SplatTex, IN.worldPos.xz);
            float displacement = splat.r * _DisplacementStrength;
            IN.worldPos.y -= displacement;

            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
        }

        v2f vert (appdata_base v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.uv = o.worldPos.xz;
            return o;
        }
        ENDCG
    }

    FallBack "Diffuse"
}