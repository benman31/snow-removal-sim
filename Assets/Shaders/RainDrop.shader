/*
Written by: Abdelrahman Awad
*/

Shader "Unlit/RainDrop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Size", float) = 1
        _T ("Time", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Size, _T;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y * 0 + _T;

                fixed4 col = 0;
                
                float2 uv = i.uv * _Size;
                uv.y += t * .25;    //moves down the grid at a pace similar to the droplets
                float2 gv = frac(uv)-.5;    //adjust origin from 0,0 to .5,.5

                float x = 0;
                float y = -sin(t+sin(t+sin(t)*.5))*.45; //function to simulate fast downwards movement, but slow upwards movement

                float2 dropPos = (gv - float2(x, y));  
                float drop = smoothstep(.05, 0.03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * .25));
                trailPos.y = (frac(trailPos.y * 8) - .5) / 8;   //-.5 to readjust the origin from 0 to .5 again
                float trail = smoothstep(.03, .01, length(trailPos));
                trail *= smoothstep(-.05, .05, dropPos.y);
                trail *= smoothstep(.5, y, gv.y);

                col += trail;
                col += drop;

                if(gv.x > .48 || gv.y > .49)
                {
                    col = float4(1,0,0,1);
                }

                return col;
            }
            ENDCG
        }
    }
}
