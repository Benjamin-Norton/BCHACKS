// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ColorblindnessShader" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;	
		
	uniform float4x4 _colorTransform;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : COLOR 
	{
		half4 color = mul(_colorTransform, tex2D(_MainTex, i.uv));
		return color;
	}

	ENDCG 
	
	Subshader 
	{
		Pass 
 		{
			  ZTest Always Cull Off ZWrite Off
			  Fog { Mode off }      
		
		      CGPROGRAM
		      
		      #pragma fragmentoption ARB_precision_hint_fastest
		      #pragma vertex vert
		      #pragma fragment frag
		
		      ENDCG
		}
	}
	Fallback off
}