/*
edited by: Abdelrahman Awad to blend between two skyboxes
*/

Shader "Custom/Skybox"
{
    Properties {
        _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
        _Sun ("Sun Size", Float) = 2.5
        _SunPow ("Sun Intensity", Float) = 4.0
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        [NoScaleOffset] _FrontTex ("Front [+Z]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _BackTex ("Back [-Z]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _LeftTex ("Left [+X]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _RightTex ("Right [-X]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _UpTex ("Up [+Y]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _DownTex ("Down [-Y]   (HDR)", 2D) = "grey" {}
        _Blend ("Blend", Range(0,1)) = 0
        [NoScaleOffset] _FrontTex2 ("Front 2 [+Z]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _BackTex2 ("Back 2 [-Z]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _LeftTex2 ("Left 2 [+X]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _RightTex2 ("Right 2 [-X]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _UpTex2 ("Up 2 [+Y]   (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _DownTex2 ("Down 2 [-Y]   (HDR)", 2D) = "grey" {}
    }
 
SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off
   
    CGINCLUDE
    #include "UnityCG.cginc"
    #include "Lighting.cginc"
 
    half4 _Tint;
    half _Sun;
    half _SunPow;
    half _Exposure;
    float _Blend;
   
    struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
    };
    struct v2f {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float3 viewDir : TEXCOORD1;
    };
    v2f vert (appdata_t v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = v.texcoord;
        o.viewDir = WorldSpaceViewDir (v.vertex);
        return o;
    }
    half4 skybox_frag (v2f i, sampler2D smp, sampler2D smp2, half4 smpDecode)
    {
        half4 tex = tex2D (smp, i.texcoord);
        half4 tex2 = tex2D (smp2, i.texcoord);
        half3 c = lerp(DecodeHDR (tex, smpDecode), DecodeHDR (tex2, smpDecode), _Blend);
        c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
        c *= _Exposure;
        half spec = pow (max (0, dot (normalize (i.viewDir), -_WorldSpaceLightPos0)), pow (_Sun, 8)) * _SunPow;
        return half4(c + (_LightColor0) * spec, 1);
    }
    ENDCG
   
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _FrontTex;
        sampler2D _FrontTex2;
        half4 _FrontTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex2, _FrontTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _BackTex;
        sampler2D _BackTex2;
        half4 _BackTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex2, _BackTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _LeftTex;
        sampler2D _LeftTex2;
        half4 _LeftTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex2, _LeftTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _RightTex;
        sampler2D _RightTex2;
        half4 _RightTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex2, _RightTex_HDR); }
        ENDCG
    }  
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _UpTex;
        sampler2D _UpTex2;
        half4 _UpTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex2, _UpTex_HDR); }
        ENDCG
    }  
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _DownTex;
        sampler2D _DownTex2;
        half4 _DownTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex2, _DownTex_HDR); }
        ENDCG
    }
}
}
