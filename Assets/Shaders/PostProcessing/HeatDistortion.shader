/*
Written by: Abdelrahman Awad
*/

Shader "Unlit/HeatDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionTexture ("Distortion Texture", 2D) = "white" {}
        _DistortionStr ("Distortion Strength", float) = .1
        _Cutoff ("Alpha cutoff", Range(0,1)) = .9
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100

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

            sampler2D _MainTex;
            sampler2D _DistortionTexture;
            float4 _MainTex_ST;
            float _DistortionStr;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 distortion = tex2D(_DistortionTexture, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv + distortion.rg * _DistortionStr); //renders scene normally but distorts pixels that are behind distortion particles
                clip(col.a - _Cutoff); //cutoff texture edges 
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
