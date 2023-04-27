// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rob/DirectionalDissolve"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_NoiseDirection("NoiseDirection", Vector) = (0,1,0,0)
		_NoiseScrollspeed("NoiseScrollspeed", Float) = 1
		_NoiseScale("NoiseScale", Vector) = (2,2,0,0)
		_EdgeThickness("EdgeThickness", Range( 0 , 0.3)) = 0
		[HDR]_EdgeColor("EdgeColor", Color) = (0,0,0,0)
		[Toggle]_VertexPosition("VertexPosition", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_DissolveDirection("DissolveDirection", Vector) = (0,0.66,0,0)
		_DissolveAmount("DissolveAmount", Range( 0 , 1)) = 0.5048291
		_UpperLimit("UpperLimit", Float) = -5
		_LowerLimit("LowerLimit", Float) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
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
		uniform float4 _EdgeColor;
		uniform float _VertexPosition;
		uniform float3 _DissolveDirection;
		uniform float _DissolveAmount;
		uniform float _UpperLimit;
		uniform float _LowerLimit;
		uniform float3 _NoiseDirection;
		uniform float _NoiseScrollspeed;
		uniform float2 _NoiseScale;
		uniform float _EdgeThickness;
		uniform float _Cutoff = 0.5;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = ( _Tint * tex2D( _Albedo, i.uv_texcoord ) ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 normalizeResult154 = normalize( (( _VertexPosition )?( ase_vertex3Pos ):( ase_worldPos )) );
			float3 normalizeResult131 = normalize( _DissolveDirection );
			float dotResult130 = dot( normalizeResult154 , normalizeResult131 );
			float Gradient29 = ( dotResult130 + (-_UpperLimit + (_DissolveAmount - 0.0) * (-_LowerLimit - -_UpperLimit) / (1.0 - 0.0)) );
			float3 normalizeResult153 = normalize( _NoiseDirection );
			float simplePerlin3D62 = snoise( ( ase_vertex3Pos + ( ( normalizeResult153 * _NoiseScrollspeed ) * _Time.y ) )*_NoiseScale.x );
			simplePerlin3D62 = simplePerlin3D62*0.5 + 0.5;
			float Noise69 = simplePerlin3D62;
			float temp_output_60_0 = ( ( ( 1.0 - Gradient29 ) * Noise69 ) - Gradient29 );
			o.Emission = ( _EdgeColor * step( temp_output_60_0 , ( _EdgeThickness + (0.0 + (0.0 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) ) ) ).rgb;
			o.Alpha = 1;
			clip( temp_output_60_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
0;73;1575;574;3729.621;660.189;3.2869;True;False
Node;AmplifyShaderEditor.CommentaryNode;30;-3170.64,746.0917;Inherit;False;1738.878;722.5874;Comment;14;29;18;53;130;145;131;146;19;51;52;122;101;149;147;Fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;100;-3318.806,-563.7974;Inherit;False;1987.844;732.4697;Comment;10;94;62;69;96;89;93;95;92;151;152;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;149;-3095.867,986.0827;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;101;-3100.9,838.7674;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;151;-3273.588,-291.9053;Inherit;False;Property;_NoiseDirection;NoiseDirection;2;0;Create;True;0;0;False;0;0,1,0;-0.08,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;153;-3082.485,-224.3054;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;147;-2818.286,823.9488;Inherit;False;Property;_VertexPosition;VertexPosition;7;0;Create;True;0;0;False;0;0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-3207.117,-84.65452;Inherit;False;Property;_NoiseScrollspeed;NoiseScrollspeed;3;0;Create;True;0;0;False;0;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;122;-2785.634,1066.959;Inherit;False;Property;_DissolveDirection;DissolveDirection;9;0;Create;True;0;0;False;0;0,0.66,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;51;-3143.25,1165.702;Inherit;False;Property;_UpperLimit;UpperLimit;11;0;Create;True;0;0;False;0;-5;0.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3150.737,1273.969;Inherit;False;Property;_LowerLimit;LowerLimit;12;0;Create;True;0;0;False;0;5;-0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;145;-2967.582,1180.276;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-2887.328,-214.7763;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;146;-2972.562,1277.333;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;89;-3202.262,6.519135;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-3128.898,1405.556;Inherit;False;Property;_DissolveAmount;DissolveAmount;10;0;Create;True;0;0;False;0;0.5048291;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;154;-2535.379,864.9078;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;131;-2562.286,1087.187;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;93;-3227.133,-484.1977;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;130;-2311.753,1079.076;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-2824.427,-93.81293;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;53;-2283.485,1302.379;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-2002.911,1116.171;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-2673.391,-323.9803;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;94;-2424.968,-107.1853;Inherit;False;Property;_NoiseScale;NoiseScale;4;0;Create;True;0;0;False;0;2,2;5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-1721.473,1114.16;Inherit;True;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;62;-2216.122,-331.3743;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-1166.349,279.467;Inherit;True;29;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-1554.965,-262.937;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;54;-842.567,1105.075;Inherit;False;820.2998;637.5504;GLow;6;45;47;46;44;49;50;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1146.349,477.8064;Inherit;True;69;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-858.6298,266.9074;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-1125.097,720.1281;Inherit;True;29;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-827.8664,1284.723;Inherit;False;Property;_EdgeThickness;EdgeThickness;5;0;Create;True;0;0;False;0;0;0.05;0;0.3;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;45;-792.4499,1412.926;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-582.9459,278.6074;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;60;-377.0607,513.296;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-596.1231,1393.529;Inherit;True;2;2;0;FLOAT;0.01;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-784.625,-47.60046;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;49;-360.3977,1530.624;Inherit;False;Property;_EdgeColor;EdgeColor;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1.507853,3.706806,4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;44;-353.9499,1236.875;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;83;-496.4124,-344.306;Inherit;False;Property;_Tint;Tint;1;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;-544.7758,-95.64823;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;936bffa65ef866a43969ea77c103adb3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-149.1249,1346.588;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-72.88984,-238.7243;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;698.5323,14.72532;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Rob/DirectionalDissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;8;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;153;0;151;0
WireConnection;147;0;101;0
WireConnection;147;1;149;0
WireConnection;145;0;51;0
WireConnection;152;0;153;0
WireConnection;152;1;96;0
WireConnection;146;0;52;0
WireConnection;154;0;147;0
WireConnection;131;0;122;0
WireConnection;130;0;154;0
WireConnection;130;1;131;0
WireConnection;95;0;152;0
WireConnection;95;1;89;2
WireConnection;53;0;19;0
WireConnection;53;3;145;0
WireConnection;53;4;146;0
WireConnection;18;0;130;0
WireConnection;18;1;53;0
WireConnection;92;0;93;0
WireConnection;92;1;95;0
WireConnection;29;0;18;0
WireConnection;62;0;92;0
WireConnection;62;1;94;0
WireConnection;69;0;62;0
WireConnection;32;0;31;0
WireConnection;57;0;32;0
WireConnection;57;1;70;0
WireConnection;60;0;57;0
WireConnection;60;1;74;0
WireConnection;46;0;47;0
WireConnection;46;1;45;0
WireConnection;44;0;60;0
WireConnection;44;1;46;0
WireConnection;43;1;85;0
WireConnection;50;0;49;0
WireConnection;50;1;44;0
WireConnection;84;0;83;0
WireConnection;84;1;43;0
WireConnection;0;0;84;0
WireConnection;0;2;50;0
WireConnection;0;10;60;0
ASEEND*/
//CHKSM=A53C762C046F1938383035E696EFAFFEDC8D7D31