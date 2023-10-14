// Upgrade NOTE: upgraded instancing buffer 'GPUTextureWBG' to new syntax.

// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GPUTextureWBG"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_BackgroundColor("BackgroundColor", Color) = (0,0,0,0)
		_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;

		UNITY_INSTANCING_BUFFER_START(GPUTextureWBG)
			UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
#define _MainTex_ST_arr GPUTextureWBG
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr GPUTextureWBG
			UNITY_DEFINE_INSTANCED_PROP(float4, _BackgroundColor)
#define _BackgroundColor_arr GPUTextureWBG
		UNITY_INSTANCING_BUFFER_END(GPUTextureWBG)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
			float4 tex2DNode10 = tex2D( _MainTex, uv_MainTex );
			float4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			float4 temp_output_5_0 = ( _Color_Instance * tex2DNode10 );
			float4 _BackgroundColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_BackgroundColor_arr, _BackgroundColor);
			float4 ifLocalVar8 = 0;
			if( tex2DNode10.a >= 0.01 )
				ifLocalVar8 = temp_output_5_0;
			else
				ifLocalVar8 = _BackgroundColor_Instance;
			o.Emission = ifLocalVar8.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;198.1244,-269.1792;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;4;-156.312,-361.7168;Inherit;False;InstancedProperty;_Color;Color;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-147.7521,207.094;Inherit;False;InstancedProperty;_BackgroundColor;BackgroundColor;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;728.1623,-154.8496;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;GPUTextureWBG;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;10;-316.4631,-132.123;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;8;437.3523,-110.5854;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.01;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;5;0;4;0
WireConnection;5;1;10;0
WireConnection;0;2;8;0
WireConnection;8;0;10;4
WireConnection;8;2;5;0
WireConnection;8;3;5;0
WireConnection;8;4;6;0
ASEEND*/
//CHKSM=6E6634E739305D478A8CE73AFCF66CABE88CE2C7