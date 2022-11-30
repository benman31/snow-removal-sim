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
        _TimeScale ("Time scale", float) = 1
        _Distortion("Distortion", range(-5,5)) = 1
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
            float _Size, _T, _Distortion, _TimeScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float N21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float2 Layer(float2 inUV, float t)
            {  
                float2 aspect = float2(2, 1);
                float2 uv = inUV * _Size * aspect;
                uv.y += t * .25;
                // uv.y *= 2;
                float2 gv = frac(uv) - .5;

                float2 id = floor(uv);
                float n = N21(id); //0 1
                
                //multiply by 2 pi to match the period of the sin wave for more randomness
                t+= n * 6.2831; 

                float w = inUV.y * 10;
                
                float x = (n - .5) * .8; //vary x pos from -.4 to .4
                x += (.4 - abs(x)) * sin(3*w) * pow(sin(w), 6) * .45;

                float y = -sin(t+sin(t+sin(t)*.5))*.45;
                // y *= 2;
                y -= (gv.x - x) * (gv.x - x);

                float2 dropPos = (gv - float2(x, y)) / aspect;
                float drop = smoothstep(.05, .03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * .25)) / aspect;
                trailPos.y = (frac(trailPos.y * 8) - .5) / 8;
                float trail = smoothstep(.03, .01, length(trailPos));
                float fogTrail = smoothstep(-.05, .05, dropPos.y);
                fogTrail *= smoothstep(.5, y, gv.y);   //can improve

                trail *= fogTrail;
                fogTrail *= smoothstep(.05, .04, abs(dropPos.x));

                float2 offset = drop*dropPos + trail*trailPos;

                return float2(offset);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = fmod(_Time.y * _TimeScale + _T, 7200);

                float4 col = 0;

                float2 drops = Layer(i.uv, t);
                drops += Layer(i.uv * 1.2 + 7, t);
                drops += Layer(i.uv * 1.5 - 7, t);
                // drops += Layer(i.uv * 3 + 2.5, t);
                col = tex2D(_MainTex, i.uv + drops * _Distortion);
                
                return col;
            }
            ENDCG
        }
    }
}
