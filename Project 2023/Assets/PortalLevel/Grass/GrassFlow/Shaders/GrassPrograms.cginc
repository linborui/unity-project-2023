
//include the other goodies
#if defined(URP)
//this needs to be first since it has the CBUFFER macro that the vars need
#include "GrassURPInclude.cginc"
#else
#include "UnityStandardUtils.cginc"
#endif

#include "GrassStructsVars.cginc"
#include "GrassRenderPipelineHelpers.cginc"
#include "GrassFunc.cginc"


#include "GrassMeshBuffer.cginc"
#include "GrassGeometry.cginc"


#if defined(DEFERRED)
#include "GrassDeferred.cginc"
#endif





g2f mesh_vertex_shader(v2g IN, uint vertID : SV_VertexID, uint instID : SV_InstanceID){

	
	uint realInstID = instID;
	uint posBufferOffset = 0;

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	//this is also really dumb but basically we need to make sure that the instID is halved and floored
	//since in stereo instancing each instance is rendered once per eye and they count as separate instances
	//doing this ensures each pair of instances is set back to what the original non stereo instanced id would be
	instID = floor(instID * 0.5);
	#endif


	#if !defined(FRUSTUM_CULLED)
	//handle batch decoding
	float4 batch = 0;
	#define batchOffset x
	#define batchLodStepMult y
	#define batchInstanceLod z
	#define batchInstances w
	#if MAX_BATCH_COUNT <= 2
		batch = batchData[0];
		#if MAX_BATCH_COUNT == 2
		if(instID >= batch.batchInstances){
			instID -= batch.batchInstances;
			batch = batchData[1];
		}
		#endif
	#else
		//i kinda cant believe unrolling this manually actually helps mobile performance
		[unroll]
		for(int i = 0; i < MAX_BATCH_COUNT; i++){
			instID -= batch.batchInstances;
			batch = batchData[i];
			if(instID < batch.batchInstances){
				break;
			}
		}
	#endif
	posBufferOffset = batch.batchOffset;
	#endif


	//calc actual grass inst id from the duplicated geometry
	uint meshInst = instID * grassPerTri;
	uint submeshInst = vertID * meshInvVertCount;
	instID = meshInst + submeshInst;


	VertexData gvd = GetPosData(instID, posBufferOffset);


	#if !defined(FRUSTUM_CULLED)
	//mult by inverse lod step value
	//this is precalced by frustum culling so not necessary there
	instID = meshInst * batch.batchLodStepMult;
	gvd.lodData = batch.batchInstanceLod;
	#endif

	g2f o = CreateBladeFromSource(gvd.vertex.xz, instID, gvd, IN, realInstID);

	return o;
}



#if !defined(SHADOW_CASTER)


//this fragment shader looks like a lot
//but if the settings are unchecked, it essentially just returns mainTex * col + fog
#if !defined(DEFERRED)
half4 fragment_shader(g2f i) : SV_Target{
#else
FragmentOutput fragment_shader(g2f i) {
#endif

	//spsi shadow stuff
	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	UNITY_SETUP_INSTANCE_ID(i);
	#endif

	//return 0;
	half4 texCol = tex2D(_MainTex, i.uv);

	#if !defined(NO_TRANSPARENCY)
	#if defined(GF_USE_DITHER)

		#define _Dither_Alpha i.uv.w

		//half alphaRef = tex3D(_DitherMaskLOD, float3((i.worldPos.xy + i.worldPos.z)*2, _Dither_Alpha*0.9375 + 0.0001)).a;
		half alphaRef = tex3D(_DitherMaskLOD, float3(i.pos.xy * 0.25, _Dither_Alpha * 0.9375 + 0.0001)).a;
		clip(alphaRef - 0.01);

		#if defined(DEFERRED)
		i.col.a = 1;
		#endif

	#endif

#if defined(SEMI_TRANSPARENT)
	clip(texCol.a - alphaClip);
#endif
	if(alphaLock) texCol.a = 1;
	#endif



	#if defined(GF_NORMAL_MAP)
	//honestly not entirely sure why subtracting one from the Z component of the normal somehow fixes all the issues
	//i guess its in the range of 0-2 because of the way the scale method works, but not sure why because the x,y compontents are -1 to 1
	//almost seems like this would be a bug but maybe i just dont understand something
	i.normal.xyz = normalize(i.normal.xyz + (UNPACK_NORMAL(tex2D(bumpMap, i.uv), normalStrength) - float3(0,0,1)));
	#endif


	float3 lighting = 1;
#ifdef FORWARD_ADD
	UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
	lighting = GetMainLighting(i.worldPos, i.normal, i.pos) * atten;
#elif !defined(DEFERRED)

	#if defined(GF_PPLIGHTS)
		lighting = GetMainLighting(i.worldPos, i.normal, i.pos);		
	#endif

	#if defined(FRAGMENT_HANDLES_SHADOWS)
		float atten = GRASS_SHADOW_ATTENUATION(i);
		lighting *= ambientCOShadow + (1.0 - ambientCOShadow) * atten;
	#else
		float atten = 1;
	#endif


	#if defined(USE_SPECULAR) && defined(GF_PPLIGHTS)
	float spec = CalcSpecular(i.specVec, i.normal, atten);
	i.col.rgb += GET_LIGHT_COL(i) * specTint * spec;

	//i dont think reflections are sensical on grass 🤔
	//float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
	//float3 reflectionDir = reflect(-viewDir, i.normal);
	//float4 envSample = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflectionDir);
	//i.col.rgb += DecodeHDR(envSample, unity_SpecCube0_HDR);
	#endif
#endif

	#if defined(USE_SELF_SHADOW)
	float selfShadow = ambientCOShadow + (1.0 - ambientCOShadow) * (1 - saturate(tex2D(_MainTex, i.selfShadowUV).a * 5));
	selfShadow = lerp(selfShadow, saturate(selfShadow * 3), i.selfShadowUV.z);
	lighting *= selfShadow;
	#endif


	texCol.rgb *= lighting;
	texCol = (texCol * i.col);



#if defined(DEFERRED)
	return GrassToGBuffer(i, texCol);
#else
	GRASS_APPLY_FOG(i.uv.z, texCol);
	//UNITY_APPLY_FOG(i.uv.z, texCol);
	return texCol;
#endif

}
#endif

#if defined(SHADOW_CASTER)
void fragment_shader(g2f i) {
	
	#if !defined(NO_TRANSPARENCY)
	
		half alpha = 1;
	
	#if defined(SEMI_TRANSPARENT)
		alpha = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)).a;
	#endif
	
		//i.uv.w contains alpha from lod stuff
		alpha *= tex3D(_DitherMaskLOD, float3(i.pos.xy * 0.25, i.uv.w * 0.9375 + 0.0001)).a;
	
		clip(alpha - alphaClip);
	
	#endif
}
#endif