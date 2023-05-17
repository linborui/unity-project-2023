#ifdef URP
//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#endif


#define UNITY_FOG_COORDS_PACKED(idx, vectype) vectype fogCoord : TEXCOORD##idx;
#define UNITY_FOG_COORDS(idx) UNITY_FOG_COORDS_PACKED(idx, float1)

#if defined(FOG_LINEAR)
    // factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
    #define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = (coord) * unity_FogParams.z + unity_FogParams.w
#elif defined(FOG_EXP)
    // factor = exp(-density*z)
    #define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = unity_FogParams.y * (coord); unityFogFactor = exp2(-unityFogFactor)
#elif defined(FOG_EXP2)
    // factor = exp(-(density*z)^2)
    #define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = unity_FogParams.x * (coord); unityFogFactor = exp2(-unityFogFactor*unityFogFactor)
#else
    #define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = 0.0
#endif

#define UNITY_CALC_FOG_FACTOR(coord) UNITY_CALC_FOG_FACTOR_RAW(UNITY_Z_0_FAR_FROM_CLIPSPACE(coord))



inline float4 UnityObjectToClipPos(in float3 pos)
{
#if defined(STEREO_CUBEMAP_RENDER_ON)
    return UnityObjectToClipPosODS(pos);
#else
    // More efficient than computing M*VP matrix product
    return mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(pos, 1.0)));
#endif
}

#define UNITY_FOG_LERP_COLOR(col,fogCol,fogFac) col.rgb = lerp((fogCol).rgb, (col).rgb, saturate(fogFac))

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
    #if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
        // mobile or SM2.0: fog factor was already calculated per-vertex, so just lerp the color
        #define UNITY_APPLY_FOG_COLOR(coord,col,fogCol) UNITY_FOG_LERP_COLOR(col,fogCol,(coord).x)
    #else
        // SM3.0 and PC/console: calculate fog factor and lerp fog color
        #define UNITY_APPLY_FOG_COLOR(coord,col,fogCol) UNITY_CALC_FOG_FACTOR((coord).x); UNITY_FOG_LERP_COLOR(col,fogCol,unityFogFactor)
    #endif
    #define UNITY_EXTRACT_FOG(name) float _unity_fogCoord = name.fogCoord
    #define UNITY_EXTRACT_FOG_FROM_TSPACE(name) float _unity_fogCoord = name.tSpace2.y
    #define UNITY_EXTRACT_FOG_FROM_WORLD_POS(name) float _unity_fogCoord = name.worldPos.w
    #define UNITY_EXTRACT_FOG_FROM_EYE_VEC(name) float _unity_fogCoord = name.eyeVec.w
#else
    #define UNITY_APPLY_FOG_COLOR(coord,col,fogCol)
    #define UNITY_EXTRACT_FOG(name)
    #define UNITY_EXTRACT_FOG_FROM_TSPACE(name)
    #define UNITY_EXTRACT_FOG_FROM_WORLD_POS(name)
    #define UNITY_EXTRACT_FOG_FROM_EYE_VEC(name)
#endif

#ifdef UNITY_PASS_FORWARDADD
#define UNITY_APPLY_FOG(coord,col) UNITY_APPLY_FOG_COLOR(coord,col,fixed4(0,0,0,0))
#else
#define UNITY_APPLY_FOG(coord,col) UNITY_APPLY_FOG_COLOR(coord,col,unity_FogColor)
#endif

inline float3 UnityObjectToWorldNormal(in float3 norm)
{
#ifdef UNITY_ASSUME_UNIFORM_SCALING
    return UnityObjectToWorldDir(norm);
#else
    // mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
    return normalize(mul(norm, (float3x3)unity_WorldToObject));
#endif
}

#if defined(SHADER_API_VULKAN) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)

real ReadHeightmap(real4 height)
{
    return (height.r + height.g * 256.0) / 257.0; // (255.0 * height.r + 255.0 * 256.0 * height.g) / 65535.0
}

#else

real ReadHeightmap(real4 height)
{
    return height.r;
}

#endif