Shader "Custom/Perspective" {
	Properties{
	_Color("Color", Color) = (1,1,1,1)
	_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
	_Metallic("Metallic", Range(0,1)) = 0.0
	_OcclusionColor("OcclusionColor",Color) = (0.7,0.3,0,2)
	}
		SubShader
	{
	Tags { "RenderType" = "Opaque" }
	LOD 200
		//此Pass务必放在前面
		Pass
		{
		ZTest Greater
		ZWrite Off//重要，关闭ZWrite后，后渲染的会覆盖此Pass
		CGPROGRAM
					#pragma vertex vert 
					#pragma fragment frag
					fixed4 _OcclusionColor;
					float4 vert(float4 v : POSITION) : SV_POSITION {
						return UnityObjectToClipPos(v);
					}
					fixed4 frag() : SV_Target{
						return _OcclusionColor;
					}
					ENDCG
		}
		ZTest LEqual
		CGPROGRAM
						// Physically based Standard lighting model, and enable shadows on all light types
						#pragma surface surf Standard fullforwardshadows
						// Use shader model 3.0 target, to get nicer looking lighting
						#pragma target 3.0
						struct Input {
						float2 uv_MainTex;
						};
						sampler2D _MainTex;
						half _Glossiness;
						half _Metallic;
						fixed4 _Color;
						void surf(Input IN, inout SurfaceOutputStandard o) {
							// Albedo comes from a texture tinted by color
							fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
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
