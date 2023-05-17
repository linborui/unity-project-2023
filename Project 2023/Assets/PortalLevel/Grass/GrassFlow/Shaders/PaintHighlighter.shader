Shader "Hidden/GrassFlow/PaintHighlighter"
{
    Properties
    {
		paintHighlightBrushTex ("Albedo", 2D) = "white" {}
		paintHightlightColor ("Tint", Color) = (1, 1, 1, 1)
		paintHighlightBrushParams ("param", Vector) = (1, 1, 1, 1)

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination", Float) = 0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent+1000"}
        //LOD 100

        Pass
        {

			//Blend SrcAlpha OneMinusSrcAlpha
			//Blend[_SrcBlend][_DstBlend]

            Blend One One
            //Blend DstColor Zero
            ZTest Always
			ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "AutoLight.cginc"

            #undef RENDERMODE_MESH
            #include "GrassStructsVars.cginc"
            #include "GrassRenderPipelineHelpers.cginc"
            #include "GrassFunc.cginc"


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

            sampler2D paintHighlightBrushTex;
            float4 paintHighlightBrushParams; //xy = brushPos, z = size, w = lerp value (on or off)
            float4 paintHightlightColor;


            sampler2D terrainHeightMap;
            float3 terrainSize;
            float3 invTerrainSize;
            float2 terrainChunkSize;
            float terrainMapOffset;
            float2 _chunkPos;
            bool isTerrain;

            #define map(value, fromSource, toSource) (value - fromSource) / (toSource - fromSource)

            inline float4 GetPaintHighlight(float4 inCol, float2 uv) {
            	float2 topLeft = paintHighlightBrushParams.xy - paintHighlightBrushParams.z;
            	float2 topRight = paintHighlightBrushParams.xy + paintHighlightBrushParams.z;
            	float2 brushTexCoord = map(uv, topLeft, topRight);
            
                #define Samp(uv) tex2Dlod(paintHighlightBrushTex, float4(uv, 0, 0)).a
                #define EdgeMinMax(uv) brushPct = Samp(uv);\
                minPct = min(minPct, brushPct);\
                maxPct = max(maxPct, brushPct)

                float brushPct;
                float minPct = 1;
                float maxPct = 0;

                #define pixOff 0.02
                #define pixNil 0
                //sample min max along grid for edge detection
                EdgeMinMax(brushTexCoord + float2(-pixOff, -pixOff));
                EdgeMinMax(brushTexCoord + float2(+pixNil, -pixOff));
                EdgeMinMax(brushTexCoord + float2(+pixOff, -pixOff));

                EdgeMinMax(brushTexCoord + float2(-pixOff, +pixNil));
                EdgeMinMax(brushTexCoord + float2(+pixOff, +pixNil));

                EdgeMinMax(brushTexCoord + float2(-pixOff, +pixOff));
                EdgeMinMax(brushTexCoord + float2(+pixNil, +pixOff));
                EdgeMinMax(brushTexCoord + float2(+pixOff, +pixOff));

            	EdgeMinMax(brushTexCoord);
                //float delta = (maxPct - minPct) * 0.5 + 0.5;
                float delta = (maxPct - minPct) > 0.05 ? 1 : 0.5;

            	brushPct *= paintHighlightBrushParams.w * (brushTexCoord.x >= 0 && brushTexCoord.y >= 0 && brushTexCoord.x <= 1 && brushTexCoord.y <= 1);
                

                float4 brushCol = (paintHightlightColor);
            	return float4(lerp(inCol, brushCol, brushPct * delta).rgb, inCol.a);
            }


            #if !defined(SRP)
            float ReadHeightmap(float4 height) {
            	#if (API_HAS_GUARANTEED_R16_SUPPORT)
            	return height.r;
            	#else
            	return (height.r + height.g * 256.0f) / 257.0f; // (255.0f * height.r + 255.0f * 256.0f * height.g) / 65535.0f
            	#endif
            }
            #endif
            
            inline float3 GetHeightmapSimple(inout float2 rndUV) {
            
            	float3 bladePos;
            
            	//calc worldspace pos from the random position within this chunk
            	bladePos.xz = _chunkPos + terrainChunkSize * rndUV;
            
            	//transform the terrain space coordinates into 0-1 texture space
            	rndUV = bladePos.xz * invTerrainSize.xz;
            	rndUV = rndUV * (1 - terrainMapOffset * 2) + terrainMapOffset;
            
            	float heightSamp = ReadHeightmap(tex2Dlod(terrainHeightMap, float4(rndUV, 0, 0))) * 2;
            
            	bladePos.y = heightSamp * terrainSize.y;
            
            	return bladePos;
            }


            v2f vert (appdata v)
            {
                v2f o;


               if(!isTerrain){
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
               }else{
                   //need to sample heightmap stuff to deal with terrains 
                   o.uv = v.vertex.xz;
			       o.vertex = mul(unity_ObjectToWorld, float4(GetHeightmapSimple(o.uv), 1));
                   o.vertex = mul(UNITY_MATRIX_VP, float4(o.vertex.xyz, 1.0));
               }

                
                return o;
            }

            fixed4 frag (v2f i, uint pID : SV_PrimitiveID) : SV_Target
            {
                //float4 col = GetPaintHighlight(float4(1, 1, 1, 0), i.uv);
                float4 col = GetPaintHighlight(0, i.uv);

                //float2 rng = pID;
                //col = col * float4(rndm(rng), rndm(rng), rndm(rng), 1);

                return col;
            }
            ENDCG
        }
    }
}
