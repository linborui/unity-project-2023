


#ifdef UNITY_COLORSPACE_GAMMA
#define grass_ColorSpaceDielectricSpec half4(0.220916301, 0.220916301, 0.220916301, 1.0 - 0.220916301)
#else // Linear values
#define grass_ColorSpaceDielectricSpec half4(0.04, 0.04, 0.04, 1.0 - 0.04) // standard dielectric reflectivity coef at incident angle (= 4%)
#endif


//these functions are just copied from the unity built in cgincs so that we always have access
//even in srp garbage, since basicaly implementing a manual GBuffer function rather than using the srp one
//since srp garbage has a whole setup of its own weird structs with 'surfacedata' and whatnot
inline half GrassOneMinusReflectivityFromMetallic(half metallic)
{
    // We'll need oneMinusReflectivity, so
    //   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)
    // store (1-dielectricSpec) in unity_ColorSpaceDielectricSpec.a, then
    //   1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) =
    //                  = alpha - metallic * alpha
    half oneMinusDielectricSpec = grass_ColorSpaceDielectricSpec.a;
    return oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
}

inline half3 GrassDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
{
    specColor = lerp (grass_ColorSpaceDielectricSpec.rgb, albedo, metallic);
    oneMinusReflectivity = GrassOneMinusReflectivityFromMetallic(metallic);
    return albedo * oneMinusReflectivity;
}




#if defined(URP)

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"


FragmentOutput GrassToGBuffer(g2f i, half4 col){

    half3 specular;
	half specularMonochrome;
	half3 diffuseColor = GrassDiffuseAndSpecularFromMetallic(col.rgb, _Metallic, specular, specularMonochrome);

	float occSamp = lerp(1, tex2D(_OccMap, i.uv).r, occMult);
	float specSamp = tex2D(_SpecMap, i.uv).r * occSamp;

    half3 packedNormalWS = PackNormal(i.normal);

    uint materialFlags = 0;

    //tried doing all this dumb stuff to maybe do it more properly but it doesnt work and idunno why
    //could just be down to like not using the right shader feature keywords and stuff but eh
    //BRDFData brdfData;
    //float alpha = 1;
    //InitializeBRDFData(diffuseColor, _Metallic, specular, _Gloss, alpha, brdfData);

    //Light mainLight = GetMainLight(shadowCoord, i.worldPos, shadowMask);
    //MixRealtimeAndBakedGI(mainLight, i.normal, bakedGI, shadowMask);
    ////half3 GI = bakedGI * diffuseColor * ambientCO;
    //half3 GI = GlobalIllumination(brdfData, bakedGI, 1, i.worldPos, i.normal, 1) * ambientCO;

    //this works fine
    half3 GI = GET_GI(i.normal, i.worldPos) * diffuseColor * occSamp;

    // SimpleLit does not use _SPECULARHIGHLIGHTS_OFF to disable specular highlights.

    #ifdef _RECEIVE_SHADOWS_OFF
    materialFlags |= kMaterialFlagReceiveShadowsOff;
    #endif

    FragmentOutput output;
    output.GBuffer0 = half4(diffuseColor, PackMaterialFlags(materialFlags));   // albedo          albedo          albedo          materialFlags   (sRGB rendertarget)
    output.GBuffer1 = half4(specular, 1 - occSamp);                            // specular        specular        specular        occlusion
    output.GBuffer2 = half4(packedNormalWS, _Gloss);                           // encoded-normal  encoded-normal  encoded-normal  smoothness
    output.GBuffer3 = half4(GI, 1);                                      // GI              GI              GI              [optional: see OutputAlpha()] (lighting buffer)
    #if _RENDER_PASS_ENABLED
    output.GBuffer4 = i.pos.z;
    #endif
    #if OUTPUT_SHADOWMASK
    output.GBUFFER_SHADOWMASK = 0;
    #endif
    #ifdef _LIGHT_LAYERS
    uint renderingLayers = GetMeshRenderingLightLayer();
    // Note: we need to mask out only 8bits of the layer mask before encoding it as otherwise any value > 255 will map to all layers active
    output.GBUFFER_LIGHT_LAYERS = float4((renderingLayers & 0x000000FF) / 255.0, 0.0, 0.0, 0.0);
    #endif

    return output;
}

#else

//annoying to put this here but URP will also define this with the same name so it cant be in the main structs
struct FragmentOutput {
	float4 albedo : SV_Target0;
	float4 specular : SV_Target1;
	float4 normal : SV_Target2;
	float4 light : SV_Target3;
};

FragmentOutput GrassToGBuffer(g2f i, half4 col){

	FragmentOutput deferredData;

	half3 specular;
	half specularMonochrome;
	half3 diffuseColor = GrassDiffuseAndSpecularFromMetallic(col.rgb, _Metallic, specular, specularMonochrome);

	float occSamp = lerp(1, tex2D(_OccMap, i.uv).r, occMult);
	float specSamp = tex2D(_SpecMap, i.uv).r * occSamp;

	deferredData.albedo.rgb = diffuseColor; //albedo	
	deferredData.albedo.a = 1 - occSamp; //occulusion


	deferredData.specular.rgb = specular * i.normal.w * specSamp; //specular tint
	deferredData.specular.a = _Gloss * i.normal.w * specSamp; //shinyness


	deferredData.normal = float4(i.normal.xyz * 0.5 + 0.5, 0);
	//deferredData.normal = float4(0, 1, 0, 1);

	//indirect lighting
	float3 sh9 = GET_GI(i.normal, i.worldPos);

	deferredData.light.rgb = diffuseColor * sh9 * occSamp;

	#if !defined(UNITY_HDR_ON)
	deferredData.light.rgb = exp2(-deferredData.light.rgb);
	#endif

	return deferredData;
}

#endif