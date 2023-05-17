Shader "Hidden/Grassflow/Painter" {

	//This shader isnt actually used but im keeping it around just in case
	//its an alternative painting shader that uses traditional vert/frag instead of a computer shader
	//potentially more compatible but no known issues with compmute shader version


	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}

	//Default simple blit vertex stage setup
	CGINCLUDE
		#define map(value, min1, max1, min2, max2) (value - min1) / (max1 - min1) * (max2 - min2) + min2
		
		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		sampler2D _MainTex;
		
		v2f vert (appdata v) {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;

		#if !UNITY_UV_STARTS_AT_TOP
			o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif

			return o;
		}
	ENDCG



	SubShader {

		Cull Off ZWrite Off ZTest Always

		// 0 - Paint
		Pass {
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			sampler2D brushTexture;

			float4 blendParams; // either color or dhf info
			float4 srsBrushParams; // strength, radius, unused, alpha controls type/0 for color 1 for dhf
			float2 brushPos; //brush pos is in uv space
			float2 clampRange;
			
			half4 frag (v2f i) : SV_Target {

				half4 col = tex2Dlod(_MainTex, float4(i.uv, 0, 0));


				float2 topLeft = brushPos - srsBrushParams.y;
				float2 topRight = brushPos + srsBrushParams.y;
				float2 brushTexCoord = map(i.uv, topLeft, topRight, float2(0, 0), float2(1, 1));
				[branch] if (brushTexCoord.x < 0 || brushTexCoord.y < 0 || brushTexCoord.x > 1 || brushTexCoord.y > 1){
					return col;
				}

				float brushPct = tex2Dlod(brushTexture, float4(brushTexCoord, 0, 0)).a * srsBrushParams.x;

				//float dist = distance(brushPos, uv);
				//float brushPct = saturate((1.0 - dist / srsBrushParams.y)) * srsBrushParams.x;

				half4 colorBlend = lerp(col, blendParams, brushPct);
				half4 dhfBlend = col + blendParams * brushPct;

				dhfBlend.x = blendParams.x != 0 && brushPct > 0.001 ? clamp(dhfBlend.x, clampRange.x, clampRange.y) : dhfBlend.x;
				dhfBlend.y = blendParams.y != 0 && brushPct > 0.001 ? clamp(dhfBlend.y, clampRange.x, clampRange.y) : dhfBlend.y;
				dhfBlend.z = blendParams.z != 0 && brushPct > 0.001 ? clamp(dhfBlend.z, clampRange.x, clampRange.y) : dhfBlend.z;
				dhfBlend.w = blendParams.w != 0 && brushPct > 0.001 ? clamp(dhfBlend.w, clampRange.x, clampRange.y) : dhfBlend.w;

				return lerp(colorBlend, dhfBlend, srsBrushParams.w);
			}
			ENDCG
		}

		// 1 - SplatApply
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#pragma target 4.0

			Texture2D splatTex;
			SamplerState samplersplatTex {
				Filter = MIN_MAG_MIP_LINEAR;
				AddressU = Clamp;
				AddressV = Clamp;
			};

			int splatMode;
			int splatChannel;
			float splatTolerance;

			float4 frag(v2f i) : SV_Target {

				int sW, sH;
				splatTex.GetDimensions(sW, sH);
				float2 splatTexOff = float2(0.5 / sW, 0.5 / sH);

				float2 uv = i.uv + splatTexOff;

				half4 result = tex2Dlod(_MainTex, float4(i.uv, 0, 0));
				float splat = splatTex.SampleLevel(samplersplatTex, uv, 0)[splatChannel];
				splat = lerp(splat == 1, splat, splatTolerance);
				splat = lerp(splat, splat > 0, splatTolerance);



				switch (splatMode) {

					case 0:
						//add
						result.r = max(result.r, splat);
						break;

					case 1:
						//subtract
						splat = saturate(1 - splat);
						result.r = min(result.r, splat);
						break;

					case 2:
						//replace
						result.r = splat;
						break;
				}

				return result;
			}
			ENDCG
		}

	}
}
