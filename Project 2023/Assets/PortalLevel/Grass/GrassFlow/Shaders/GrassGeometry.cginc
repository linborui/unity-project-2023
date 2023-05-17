// Upgrade NOTE: replaced 'defined UNITY_BRANCH' with 'defined (UNITY_BRANCH)'


// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles


#define INTERPOLATE_BC(source, fieldName, UV) \
	source[0]fieldName * UV.x + \
	source[1]fieldName * UV.y + \
	source[2]fieldName * UV.z

static float2 minmax = float2(0, 1);
static float2 UVT[3] = {
	float2(minmax.x, minmax.x),
	float2(minmax.y * 0.5, minmax.y),
	float2(minmax.y, minmax.x),
};

static float NaN = sqrt(-1 - 1.0);


g2f CreateBladeFromSource(float2 rngPrimer, float instID, in VertexData lvd, v2g grassVert, uint realInstID) {


	#define Cull\
			lvd.vertex = NaN;\
			o.pos = NaN;\
			return o

	

	#define vd1 IN[0]
	#define vd2 IN[1]
	#define vd3 IN[2]

	g2f o;

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	o.instanceID = realInstID;
	HandleSinglePassStereoInstanced(o); //Insert
	#endif

	rngfloat rndSeed = rngPrimer + 2.1378952;


	#define RNG rndm(rndSeed)

	#if defined(USE_PARAM_MAP)
	UNITY_BRANCH
	if (RNG > lvd.dhfParams.x) {
		Cull;
	}
	#endif

	#if defined(PREMULTIPLIED_WPOS)
	float3 worldPos = lvd.vertex.xyz;
	float3 surfWorldNormal = lvd.norm;
	#else
	float3 worldPos = mul(grassToWorld, float4(lvd.vertex.xyz, 1));
	float3 surfWorldNormal = normalize(GrassToWorldNormal(lvd.norm));
	#endif


	float3 toTri = worldPos - _WorldSpaceCameraPos;
	#if defined(GF_USE_DITHER)
	float ditherRand = RNG; //have to do this here to make sure the rng stays in sync with the shadows
	#if defined(DEFERRED) || defined(GF_USE_DITHER) || defined(SHADOW_CASTER)
	//variate length to avoid artifacting in dithering
	toTri *= ditherRand * 0.35 + 1.0;
	#endif
	#endif

	//calculate fade alpha
	float camDist = rsqrt(dot(toTri, toTri));
	half alphaBlendo = saturate(pow(camDist * grassFade, grassFadeSharpness));


	#if defined(SHADOW_CASTER)
	float fracFade = alphaBlendo;
	#else
	//makes it so that the distance fade is not dithered by not alphaBlendo for fracFade
	//not sure if this tradeoff is worth it since it allows some minor depth popping at the distance fade
	//but if the distance fade is dithered at lower resolutions the dither pattern becomes fairly apparent
	float fracFade = 1;
	#endif


	#if defined(FRUSTUM_CULLED)
		fracFade = alphaBlendo * lvd.lodData;
		#if !defined(GF_USE_DITHER)
		alphaBlendo = fracFade;
		#endif
	#else
	if (instID > 0 && instID > (lvd.lodData)-1) {
		fracFade = frac((lvd.lodData));
		fracFade = (fracFade > 0 ? fracFade : 1);

		fracFade *= alphaBlendo;
		#if !defined(GF_USE_DITHER)
		alphaBlendo = fracFade;
		#endif
	}
	#endif	

	o.uv.w = fracFade;



	#if !defined(BILLBOARD)
	//float3 camRight = float3(1, RNG * 0.5f - 0.25f, RNG * 0.5f - 0.25f);
	float3 camRight = (float3(RNG, 0, RNG) * 2 - 1);
	#else
	//gets the camera-right vector
	float3 camRight = unity_WorldToCamera[0]
		//+ unity_WorldToCamera[1].xyz * (RNG * 0.5f - 0.25f)
		//+ unity_WorldToCamera[2].xyz * (RNG * 0.5f - 0.25f)
		;
	#endif


	#define TYPE_MAP
	#if defined(TYPE_MAP)
	#define _MaxTexAtlasSize 16.0
	float typeSamp = lvd.typeParams * _MaxTexAtlasSize;

	#define typeIdx floor(typeSamp)
	#define typePct frac(typeSamp)

	camDist = 1.0 + (typeIdx < textureAtlasScalingCutoff ? widthLODscale : 0) / camDist;
	#else
	camDist = 1.0 + widthLODscale / camDist;
	#endif

	float widthMult = (1.0 + lvd.dhfParams.z * 0.5) * bladeWidth * Variate(RNG, variance.w) * camDist;
	float finalHeight = GET_FINAL_HEIGHT(lvd);


	//place meshes on surface
	float normHeight = saturate(grassVert.vertex.y * maxVertH);
	#if defined(MESH_COLORS)
	float customHeight = finalHeight * grassVert.col.r;
	#else
	float customHeight = finalHeight * normHeight;
	#endif

	#if defined(LOD_SCALING)
	normHeight *= fracFade;
	#endif

	float sharp = lerp(widthMult * (1 - saturate(normHeight)), widthMult, bladeSharp);
	finalHeight *= normHeight;

	
	float3x3 tMatrix = GetSurfaceRotationMatrix(surfWorldNormal, camRight);
	//float3x4 tMatrix = GetSurfaceRotationMatrixOld(worldPos, surfWorldNormal, , rndSeed);
	
	#if defined(MESH_NORMALS)
	float3 worldNormal = mul(tMatrix, grassVert.norm);
	worldNormal = normalize(lerp(worldNormal, surfWorldNormal, blendNormal));
	#else
	float3 worldNormal = surfWorldNormal;
	#endif

	//zero y pos for multiplying to keep it flat since height is controlled later by grass positioning
	ScaleMatrix(tMatrix, float3(sharp, bladeHeight, sharp));
	float3 localPos = mul(tMatrix, float3(grassVert.vertex.x, 0, grassVert.vertex.z));
	lvd.vertex = localPos + worldPos;

	#ifdef FRAGMENT_REQUIRES_NORMAL
	o.normal = float4(worldNormal.xyz, 1);
	#endif



	float3 posVariance = GET_POS_VARIANCE(finalHeight);

	//posVariance = 0; finalHeight = 0;	
	float3 tV = TP_Vert(lvd, posVariance, finalHeight, 1, surfWorldNormal);

	finalHeight = customHeight;
	#if !defined(LOWER_QUALITY)
	float fh2 = finalHeight * finalHeight;
	finalHeight = lerp(finalHeight, fh2, bladeStiffness);
	#endif

	half noiseSamp;
	float3 windAdd = GetWindAdd(tV, lvd, finalHeight, noiseSamp);
	//tV = mul(grassToWorld, float4(tV, 1));
	float3 rippleForce = GetRippleForce(tV);
	// float3 rippleForce = 0;



	float3 topView = -unity_WorldToCamera[1].xyz * saturate(dot(unity_WorldToCamera[2].xyz, -surfWorldNormal) - 0.5) * (-3 * topViewPush);

	tV += rippleForce * normHeight;
	tV += windAdd * normHeight;
	tV += topView * normHeight;
	worldPos = tV;


	#if defined(TYPE_MAP)
	//since our type pct can only technically store 16 values and can never actually be "1"
	//our 16 values are actually 0-15
	//so we need this value to scale the rnd number into the space of our fractional type pct
	//to be clear, typePct will only ever be 0 to 0.9375 and occur in steps of (1 / 16 = 0.0625)
	const static float rndScale = 1.0 - (1.0 / 16.0); // = 0.9375

	float uvXL = (RNG * rndScale) < typePct ? typeIdx * numTexturesPctUV : 0;
	float uvXR = uvXL + numTexturesPctUV;
	#else
	static float uvXL = 0;
	static float uvXR = 1;
	#endif


	SET_WORLDPOS(o, worldPos);
	o.pos = GetClipPos(worldPos, o.pos);

	ShadowDatas shadowData;
	shadowData.pos = o.pos;
	shadowData.worldPos = worldPos;
	TRANSFER_GRASS_SHADOW(shadowData, worldPos);

	#if defined(FRAGMENT_HANDLES_SHADOWS)
	o._ShadowCoord = shadowData._ShadowCoord;
	#endif

	#if defined(MESH_UVS)
	float2 uv = grassVert.uv;
	#else
	float2 uv = grassVert.vertex.xy + 0.5;
	#endif

	uv.x = uv.x * numTexturesPctUV + uvXL;
	SET_UV(float3(uv, o.pos.z));


	#if defined(USE_SELF_SHADOW)
	//localPos += surfWorldNormal * normHeight;
	float3x3 shadowMatrix = GetSelfShadowMatrix(GET_LIGHT_DIR(o));
	o.selfShadowUV.xy = mul(shadowMatrix, grassVert.vertex).xy;
	o.selfShadowUV.xy *= selfShadowScaleOffset.xy;
	o.selfShadowUV.xy += selfShadowScaleOffset.zw;
	o.selfShadowUV.xy += (noiseSamp - 0.5) * selfShadowWind;
	o.selfShadowUV.x = o.selfShadowUV.x * numTexturesPctUV + uvXL;
	o.selfShadowUV.z = pow(normHeight, 4);
	#endif



	#if !defined(SHADOW_CASTER) && !defined(FORWARD_ADD) && !defined(DEFERRED)

		//noiseSamp = 0.8f + noiseSamp * 0.25f;
		noiseSamp = noiseSamp * 1.5 - 0.5;
		float3 windTintAdd = float3(1, 1, 1) + windTint.rgb * windTint.a * noiseSamp;
		windTintAdd += flatTint.rgb * flatTint.a * lvd.dhfParams.z;

		//TOP Vert - no AO on this one
		//ShadeVert(o, tV, lvd, shade, noiseSamp, alphaBlendo, rndSeed, float2(0.5, 1.0));
		//float4 bladeCol = float4(lvd.color * saturate(pow(Variate(RNG, variance.z), 1)), alphaBlendo);
		float3 variantCol = lerp(_Color, altCol, saturate(RNG * variance.z));
		float4 bladeCol = float4(lerp(lvd.color.rgb, lvd.color.rgb * variantCol, lvd.color.a), alphaBlendo);
		bladeCol.rgb *= windTintAdd;

		float atten = 1;
		#if !defined(GF_PPLIGHTS)
			float3 lighting = GetMainLighting(tV, worldNormal, o.pos);

			#if (defined(SHADOWS_SCREEN) || defined(SRP)) && !defined(FRAGMENT_HANDLES_SHADOWS)
				atten = GRASS_SHADOW_ATTENUATION(shadowData);
				lighting *= ambientCOShadow + (1.0 - ambientCOShadow) * atten;
			#endif

			bladeCol.rgb *= lighting;
		#endif

		o.col = bladeCol;
	#endif


	#if defined(FORWARD_ADD)
	float4 bladeCol = float4(lvd.color.rgb, alphaBlendo);
	o.col = bladeCol;
	#endif

	#if defined(DEFERRED)
	noiseSamp = noiseSamp * 1.5 - 0.5;
	float3 windTintAdd = float3(1, 1, 1) + windTint.rgb * windTint.a * noiseSamp;
	float3 variantCol = lerp(_Color, altCol, pow(RNG * variance.z, 1));
	float4 bladeCol = float4(lerp(lvd.color.rgb, lvd.color.rgb * variantCol, lvd.color.a), alphaBlendo);
	bladeCol.rgb *= windTintAdd;

	o.col = bladeCol;
	o.uv.w *= alphaBlendo;
	#endif

	


	#if !defined(SHADOW_CASTER)
	//Reduce AO a bit on flattened grass and variate AO a smidge
	float3 startCol = bladeCol;
	float aoValue = lerp(
		lvd.dhfParams.z + (RNG * 0.2) + _AO + (noiseSamp * 0.2),
		1.0,
		(1.0 - lvd.dhfParams.x) * 0.6
	);
	bladeCol.rgb *= aoValue; // apply AO
	bladeCol.a = alphaBlendo;

	
	#if defined(USE_SPECULAR) && !defined(FORWARD_ADD) && !defined(DEFERRED)
		#if defined(FORWARD_ADD)
		float3 lightDir = normalize(GET_LIGHT_DIR(o) - worldPos);
		#else
		//directional
		float3 lightDir = GET_LIGHT_DIR(o);
		#endif

		float3 specVec = normalize(normalize(_WorldSpaceCameraPos.xyz - worldPos) + lightDir);
		float specHeightMult = lerp(aoValue, 1 + (noiseSamp * 0.2), specHeight);
		specVec *= lerp(specHeightMult, 1, normHeight);

		#if defined(GF_PPLIGHTS)
		o.specVec = specVec;
		#else
		float spec = (CalcSpecular(specVec, worldNormal, atten));
		o.col.rgb += GET_LIGHT_COL(o) * specTint * spec * saturate(aoValue);
		#endif
	#endif




	//lerp AO based on height
	o.col = lerp(bladeCol, o.col, normHeight * normHeight);
	//o.col = lerp(bladeCol, o.col, normHeight);

	o.col.a = alphaBlendo;
	#endif


	return o;
}

