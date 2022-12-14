/*
Written by: Abdelrahman Awad
*/

Shader "Hidden/snow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SnowTex ("SnowTex", 2D) = "white" {}
    }
    SubShader
    {
        LOD 100
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        //Blend Srcalpha OneMinusSrcAlpha

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            Texture2D _SnowTex;
            SamplerState sampler_SnowTex;

            float _Radius;
            float _Feather;
            float _Intensity;;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;

                float2 newUV = (i.uv * 2) - 1; //center uv at 0,0
                float circle = length(newUV);
                float mask = smoothstep(_Radius, _Radius + _Feather, circle);

                if(mask > 0 && mask < 1) //add blur to feathered area
                {
                    col = _MainTex.SampleLevel(sampler_MainTex, i.uv, 100);
                }
                else
                {
                    col = _MainTex.Sample(sampler_MainTex, i.uv);
                }
                
                fixed4 snow = _SnowTex.Sample(sampler_SnowTex, i.uv);

                float4 vingetteColor =  mask * snow;

                vingetteColor.w = 1 - _Intensity;

                col = col * col.w + vingetteColor * (1-vingetteColor.w); //blend

                return col;
            }
            ENDCG
        }
    }
}
