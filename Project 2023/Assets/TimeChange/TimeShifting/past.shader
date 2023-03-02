Shader "Unlit/past"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_NoiseTex("Noise", 2D) = "black" {}
		_DistortFactor("distortfactor",Float) = 0
		_DistortStrength("distortstrength",Float) = 0
		_DistortCenter("distortcenter",Vector) = (0.5,0.5,0,0)
	    _AddColor("baseColor",Color) = (1,1,1,1)
	}
		SubShader{
	Tags {"RenderType" = "Opaque"}


	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _NoiseTex_ST;
			float _DistortFactor; //扭曲強度
			float4 _DistortCenter; //扭曲中心點xy值(0-1)屏幕空間
			float _DistortStrength;
			float4 _AddColor;
			//_MainTex_ST，当定义一张纹理的时候，该纹理的uv坐标的缩放与偏移会
			//整合成一个float4类型数据填充到_MainTex_ST当中，所以你需要手动声明一下
		CBUFFER_END

	TEXTURE2D(_MainTex);
	SAMPLER(sampler_MainTex);
	TEXTURE2D(_NoiseTex);
	SAMPLER(sampler_NoiseTex);


	struct VertexInput
	{
		float4 position : POSITION;
		float2 uv : TEXCOORD0;
	};
	struct VertexOutput
	{
		float4 position : SV_POSITION;
		float2 uv : TEXCOORD0;
	};
	ENDHLSL

		Pass{
		HLSLPROGRAM
		#pragma vertex vert
		#pragma fragment frag



		VertexOutput vert(VertexInput i) {
			VertexOutput o;
			o.position = TransformObjectToHClip(i.position.xyz);
			o.uv = i.uv;
			return o;
		}
		float4 frag(VertexOutput i) : SV_Target
		{
			//計算偏移方向
			float2 dir = i.uv - _DistortCenter.xy;
			//最終偏移的值: 方向* (1-長度) , 越靠外偏移越小
			float2 scaleOffset = _DistortFactor * normalize(dir) * (1 - length(dir));
			//採樣noise 貼圖
			float4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv);
			//noise 的權重 = 參數 * 距離, 越靠外邊, 擾動越嚴重
			float2 noiseOffset = noise.xy * _DistortStrength * dir;
			//計算最終offset = 兩種扭曲offset 的差
			float2 offset = scaleOffset - noiseOffset;
			//計算採樣uv值: 正常uv值+從中間向邊緣逐漸增加的採樣距離
			float2 uv = i.uv + offset;

			float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,uv);
			return baseTex * _AddColor;
		}


		ENDHLSL
		}
		}
}
