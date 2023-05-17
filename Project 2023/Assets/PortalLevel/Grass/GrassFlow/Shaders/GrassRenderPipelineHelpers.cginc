

#define grassToWorld objToWorldMatrix
#define worldToGrass worldToObjMatrix

#define CalcSpecular(specVec, normal, atten) pow(saturate(dot(specVec, normal)), specSmooth * 100) * specularMult * specSmooth * atten


#if !defined(SRP)
//------------------------STANDARD-------------------------//
//#define grassToWorld unity_ObjectToWorld
//#define worldToGrass unity_WorldToObject
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0))

#define GET_GI(worldNormal, worldPos) max(0, ShadeSH9(worldNormal) * ambientCO)
//#define GET_GI(worldNormal, worldPos) max(0, ShadeSHPerPixel(worldNormal.xyz, 0, worldPos) * ambientCO)
#define GET_LIGHT_DIR(light) _WorldSpaceLightPos0
#define GET_LIGHT_COL(light) _LightColor0.rgb

//the zero is for bogus lightcoords
#define TRANSFER_GRASS_SHADOW(o, wpos) UNITY_TRANSFER_SHADOW(o, 0)

#define GRASS_SHADOW_ATTENUATION(i) UNITY_SHADOW_ATTENUATION(i, i.worldPos)

#define UNPACK_NORMAL(n, scale) UnpackNormalWithScale(n, scale)

#elif defined(URP)
//------------------------UNIVERSAL-------------------------//
//#define grassToWorld UNITY_MATRIX_M
//#define worldToGrass UNITY_MATRIX_I_M

#ifdef SRP_SHADOWCASTER
#if UNITY_REVERSED_Z
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));\
				pos.z = min(pos.z, pos.w * UNITY_NEAR_CLIP_VALUE)
#else
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));\
				pos.z = max(pos.z, pos.w * UNITY_NEAR_CLIP_VALUE)
#endif
#else
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0))
#endif

#define GET_GI(worldNormal, worldPos) SampleSH(worldNormal) * ambientCO
#define GET_LIGHT_DIR(light) _MainLightPosition.xyz
// unity_LightData.z is 1 when not culled by the culling mask, otherwise 0.
// unity_ProbesOcclusion.x is the mixed light probe occlusion data
#if USE_FORWARD_PLUS
	#define GET_LIGHT_COL(light) _MainLightColor.rgb * unity_ProbesOcclusion.x
#else
	#define GET_LIGHT_COL(light) _MainLightColor.rgb * unity_LightData.z * unity_ProbesOcclusion.x
#endif


#if defined(SHADOW_CASTER) || defined(DEFERRED)
	#define TRANSFER_GRASS_SHADOW(o, wpos)
#else
	#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
	#define TRANSFER_GRASS_SHADOW(o, wpos) o._ShadowCoord = ComputeScreenPos(o.pos)
	#else
	#define TRANSFER_GRASS_SHADOW(o, wpos) o._ShadowCoord = wpos.xyzz
	#endif
#endif

//#define GRASS_SHADOW_ATTENUATION(i) MainLightRealtimeShadow(TransformWorldToShadowCoord(i.worldPos))
#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
#define GRASS_SHADOW_ATTENUATION(i) MainLightShadow(i._ShadowCoord, i.worldPos, unity_ProbesOcclusion, _MainLightOcclusionProbes);
#else
#define GRASS_SHADOW_ATTENUATION(i) MainLightShadow(TransformWorldToShadowCoord(i._ShadowCoord), i.worldPos, unity_ProbesOcclusion, _MainLightOcclusionProbes);
#endif

#define UNPACK_NORMAL(n, scale) UnpackNormalScale(n, scale)

#if USE_FORWARD_PLUS
    #define GFLIGHT_LOOP_BEGIN(lightCount, clipPos) { \
    uint lightIndex; \
    ClusterIterator _urp_internal_clusterIterator = ClusterInit(GetNormalizedScreenSpaceUV(clipPos), wPos, 0); \
    [loop] while (ClusterNext(_urp_internal_clusterIterator, lightIndex)) { \
        lightIndex += URP_FP_DIRECTIONAL_LIGHTS_COUNT;
    #define GFLIGHT_LOOP_END } }
#else
	#define GFLIGHT_LOOP_BEGIN(lightCount, clipPos) \
    for (uint lightIndex = 0u; lightIndex < lightCount; ++lightIndex) {

    #define GFLIGHT_LOOP_END }
#endif



inline void GetURPLight(uint lightIndex, float3 wPos, float3 normal, inout float3 lighting){
	#if defined(_ADDITIONAL_LIGHT_SHADOWS)
		//if _ADDITIONAL_LIGHT_SHADOWS is defined then we must be in a version of URP that supports this beefier function
		half4 shadowMask = unity_ProbesOcclusion;
		Light light = GetAdditionalLight(lightIndex, wPos, shadowMask);

	#else
		Light light = GetAdditionalLight(lightIndex, wPos);
	#endif

	#ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, GetMeshRenderingLayer()))
#endif
        {
			half shade = ambientCO + diffuseCO * saturate(dot(light.direction, normal) + 0.25);
			half atten = light.shadowAttenuation * light.distanceAttenuation;

			#if defined(USE_SPECULAR)
			float3 specVec = normalize(normalize(_WorldSpaceCameraPos.xyz - wPos) + light.direction);
			lighting += light.color * CalcSpecular(specVec, normal, atten) * specTint;
			#endif

			lighting += (light.color * shade * atten);
		}
}

//annoying that i have to put this here, but due to lame #include order dependancies, yknow
//otherwise,some variables arent decalred yet if it was in the URPInclude.cginc
inline float3 GetURPLighting(float3 wPos, float3 normal, float4 clipPos){
    float3 lighting = 0;

	#if defined(_ADDITIONAL_LIGHTS_VERTEX) || defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHT_SHADOWS)
	int additionalLightsCount = GetAdditionalLightsCount();

	#if USE_FORWARD_PLUS
    for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++){
		GetURPLight(lightIndex, wPos, normal, lighting);
    }
    #endif

	GFLIGHT_LOOP_BEGIN(additionalLightsCount, clipPos)
		GetURPLight(lightIndex, wPos, normal, lighting);
	GFLIGHT_LOOP_END
	#endif

    return lighting;
}

#endif