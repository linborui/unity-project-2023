

#define Variate(rnd, variance) (1.0f - rnd * variance)
#define VariateBalanced(rnd, variance) (rnd * variance - variance * 0.5)
#define RndFloat3 float3(rndm(rndSeed), rndm(rndSeed), rndm(rndSeed))
#define Float4(val) float4(val,val,val,1.0f)
#define Float3(val) float3(val,val,val)

#define lerp3(v1, v2, v3, UV) (lerp(lerp(v1, v2, UV.x), v3, UV.y))

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#ifdef FORWARD_ADD
	#define GRASS_APPLY_FOG(coord, col) UNITY_CALC_FOG_FACTOR(coord); col.rgb = lerp(0, (col).rgb, saturate(unityFogFactor))
	#else
	#define GRASS_APPLY_FOG(coord, col) UNITY_CALC_FOG_FACTOR(coord); col.rgb = lerp((unity_FogColor).rgb, (col).rgb, saturate(unityFogFactor))
	#endif
#else
	#define GRASS_APPLY_FOG(coord, col)
#endif

#if !defined(GRASS_DEPTH) || defined(SEMI_TRANSPARENT) || defined(GRASS_TESSELATION)
#if defined(FOG_ON)
#define SET_UV(inuv) o.uv.xyz = float3(TRANSFORM_TEX(inuv.xy, _MainTex), inuv.z)
#else
#define SET_UV(inuv) o.uv.xy = TRANSFORM_TEX(inuv.xy, _MainTex)
#endif
#else
#define SET_UV(uv) 
#endif

#if defined(FRAGMENT_REQUIRES_WORLDPOS) && !defined(SHADOW_CASTER)
#define SET_WORLDPOS(o, inPos) o.worldPos = inPos
#else
#define SET_WORLDPOS(o, inPos)
#endif

#define HandleSinglePassStereoInstanced(v)\
UNITY_SETUP_INSTANCE_ID(v);\
UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)





//float3 VorHash(float3 x)
//{
//	x = float3(dot(x, float3(127.1, 311.7, 74.7)),
//		dot(x, float3(269.5, 183.3, 246.1)),
//		dot(x, float3(113.5, 271.9, 124.6)));
//
//	return frac(sin(x)*43758.5453123);
//}

#define rngfloat float2


//inline float rndm(inout rngfloat x)
//{
//	x = rngfloat(dot(x, rngfloat(127.1, 311.7, 74.7, 211.3)),
//				 dot(x, rngfloat(269.5, 183.3, 246.1, 69.2)),
//				 dot(x, rngfloat(113.5, 271.9, 124.6, 301.3)),
//				 dot(x, rngfloat(308.2, 143.6, 53.4, 192.1)));
//
//	x = frac(sin(x)*43758.5453123);
//
//	return x;
//}


//this seems to be about the bare minimum i can get away with on rng before noticeable artifacts start showing up
inline float rndm(inout rngfloat x) {
	//ok so, in my testing this is somehow faster than the one below
	//even though it has one more dot(), idk, nothing makes sense anymore
	//i wanna go home

	x = frac(cos(rngfloat(
		dot(x, rngfloat(127.1, 311.7)),
		dot(x, rngfloat(269.5, 183.3))
	)) * 43758.5453123);

	//x = dot(x, rngfloat(127.1, 311.7));
	//x = frac(sin(x) * 43758.5453123);

	return x.x + 0.00000001;
}


//in my testing at least, somehow this is slower than the above method
//despite the other one having a sin() in it
//perhaps this varies on different hardware but man idk what to do
//inline float rndm(inout rngfloat p)
//{
//	float3 p3 = frac(float3(p.xyx) * .1031);
//	p3 += dot(p3, p3.yzx + 33.33);
//	p = frac((p3.x + p3.y) * p3.z);
//	return p + 0.00000001;
//}

//older rnd method, i think its technically better quality but the rng quality isnt super vital for this application
//static float4 _q = float4(1225.0, 1585.0, 2457.0, 2098.0);
//static float4 _r = float4(1112.0, 367.0, 92.0, 265.0);
//static float4 _a = float4(3423.0, 2646.0, 1707.0, 1999.0);
//static float4 _m = float4(4194287.0, 4194277.0, 4194191.0, 4194167.0);

//inline float rndm(inout rngfloat n) {
//	rngfloat beta = floor(n / _q);
//	rngfloat p = _a * (n - beta * _q) - beta * _r;
//	beta = (sign(-p) + rngfloat(1.0, 1.0, 1.0, 1.0)) * rngfloat(0.5, 0.5, 0.5, 0.5) * _m;
//	n = (p + beta);
//
//	return frac(dot(n / _m, rngfloat(1.0, -1.0, 1.0, -1.0)));
//}


//#define PI 3.14159265358979323846264
//#define stddev 0.3
//#define mean 0.5
//float gaussrand(float2 co)
//{
//	// Box-Muller method for sampling from the normal distribution
//	// http://en.wikipedia.org/wiki/Normal_distribution#Generating_values_from_normal_distribution
//	// This method requires 2 uniform random inputs and produces 2 
//	// Gaussian random outputs.  We'll take a 3rd random variable and use it to
//	// switch between the two outputs.
//
//	float U, V, R, Z;
//	// Add in the CPU-supplied random offsets to generate the 3 random values that
//	// we'll use.
//	U = frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453) + 0.000000000001;
//	V = frac(sin(dot(co.yx, float2(52.9898, 18.233))) * 13758.5453) + 0.000000000001;
//	R = frac(sin(dot(co.xy, float2(62.9898, 38.233))) * 93758.5453) + 0.000000000001;
//	// Switch between the two random outputs.
//	if (R < 0.5)
//		Z = sqrt(-2.0 * log(U)) * sin(2.0 * PI * V);
//	else
//		Z = sqrt(-2.0 * log(U)) * cos(2.0 * PI * V);
//
//	// Apply the stddev and mean.
//	return Z * stddev + mean;
//}

#define map(value, fromSource, toSource) (value - fromSource) / (toSource - fromSource)
#define MapFrom01(value, lower, upper) (lower + (upper - lower) * value)

inline float3 GrassToWorldNormal(in float3 norm)
{
	#ifdef UNITY_ASSUME_UNIFORM_SCALING
	return (mul((float3x3)grassToWorld, norm));
	#else
	return (mul(norm, (float3x3)worldToGrass));
	#endif
}

inline float3 WorldToGrassDir(in float3 dir)
{
	return (mul((float3x3)worldToGrass, dir));
}

#if defined(MAP_COLOR) || defined(USE_MAPS_OVERRIDE)
#define USE_COLOR_MAP
#endif
#if defined(MAP_PARAM) || defined(USE_MAPS_OVERRIDE)
#define USE_PARAM_MAP
#endif
#if defined(MAP_TYPE) || defined(USE_MAPS_OVERRIDE)
#define USE_TYPE_MAP
#endif

inline void SampleVertexData(inout VertexData ovd, float2 UV) {

	#if defined(USE_COLOR_MAP)
	half4 colSamp = tex2Dlod(colorMap, float4(TRANSFORM_TEX(UV, colorMap), 0, 0));
	ovd.color = colSamp;
	//ovd.color = lerp(colSamp.rgb, colSamp.rgb * _Color, colSamp.a);
	#endif

	#if defined(USE_PARAM_MAP)
	ovd.dhfParams = tex2Dlod(dhfParamMap, float4(TRANSFORM_TEX(UV, dhfParamMap), 0, 0));
	ovd.dhfParams.z = saturate(1.0 - ovd.dhfParams.z) * 0.75;
	#endif

	#if defined(USE_TYPE_MAP)
	ovd.typeParams = tex2Dlod(typeMap, float4(TRANSFORM_TEX(UV, typeMap), 0, 0)).r;
	#endif
}

#define GetMatrixPos(mat) float3(mat._m03, mat._m13, mat._m23)
#define GetMatrixUV(mat) float2(mat._m30, mat._m31)
#define GetMatrixNormal(mat) mul((float3x3)mat, float3(0, 1, 0))



inline VertexData GetPosData(uint idx, uint offset){

	VertexData g;

	#if defined(FRUSTUM_CULLED)
	float2 cullData = posIdBuffer[idx];
	uint posIdx = cullData.x;
	g.lodData = cullData.y;
	#else
	uint posIdx = offset + idx;
	g.lodData = 0;
	#endif


	GrassPosCompressed c = grassPosBuffer[posIdx];

	g.vertex = c.pos;
	//g.vertex.y = 0;

	//normals
	//g.chunkIdx = (c.data1[0] & 0x0000FFFF); 
	g.norm.x = f16tof32(c.data1[0] >> 16);
	g.norm.y = f16tof32(c.data1[1]); g.norm.z = f16tof32(c.data1[1] >> 16);

	static float unpackMult = 1.0 / 255.0;
	#define unpack(val) (float4((val & 0xFF), (val & 0xFF00) >> 8, (val & 0xFF0000) >> 16, (val & 0xFF000000) >> 24) * unpackMult)
	
	g.dhfParams = unpack(c.data2[0]);
	g.typeParams = g.dhfParams.x;
	#if !defined(USE_PARAM_MAP)
	g.dhfParams.x = 1;
	#endif
	
	#if !defined(USE_COLOR_MAP)
	float4 colSamp = unpack(c.data2[1]);
	//g.color = lerp(colSamp.rgb, colSamp.rgb * _Color, colSamp.a);
	g.color = colSamp;
	#else
	g.color = 0;
	#endif


	#if defined(USE_COLOR_MAP) || defined(USE_PARAM_MAP) || defined(USE_TYPE_MAP)
	float2 uv;
	uv.x = f16tof32(c.data1[2]); uv.y = f16tof32(c.data1[2] >> 16);
	SampleVertexData(g, uv);
	#endif
	

	return g;
}


static float3 upVec = float3(0, 1, 0);


#define GET_FINAL_HEIGHT(vd) bladeHeight * vd.dhfParams.y * Variate(rndm(rndSeed), variance.y)
#define GET_POS_VARIANCE(finalHeight) VariateBalanced(RndFloat3, variance.x) * finalHeight





//GEOMETRY
inline float3 TP_Vert(VertexData vd, float3 posVariance, float finalHeight, float t, float3 wNorm) {

	float3 pos = vd.vertex + lerp(wNorm, upVec, seekSun) * finalHeight +
		posVariance +
		vd.dhfParams.z * finalHeight * flatnessMult * -wNorm;

	#if !defined(LOWER_QUALITY)
	float3 curveDir = (windDir * 3 + cross(wNorm, posVariance));
	pos += (
		((bladeLateralCurve * curveDir * t) +
			(bladeVerticalCurve * -wNorm * t)) * (1 - vd.dhfParams.z) +
		(cross(wNorm, float3(1, 0, 0)) * bladeLateralCurve * 10 * t) * vd.dhfParams.z * vd.dhfParams.y
		) * finalHeight;
	#endif

	//extend length of flattened grass
	pos += (pos - vd.vertex) * vd.dhfParams.z;


	return pos;
}

#if !defined(LOWER_QUALITY)
#define SampleWind(pos) tex3Dlod(_NoiseTex, float4(pos * noiseScale + noiseOffset, 0)).r * vd.dhfParams.w
#else
#define SampleWind(pos) tex2Dlod(_NoiseTex2D, float4(pos * noiseScale + noiseOffset, 0)).r * vd.dhfParams.w
#endif

inline float3 GetWindAdd(in float3 pos, in VertexData vd, in float finalHeight, out half noiseSamp) {

	float4 noiseScale = _noiseScale.xzyw * 0.01;
	float4 noiseOffset = _Time.x * float4(_noiseSpeed.xzy, 0);

	noiseSamp = SampleWind(pos.xzy);
	//noiseSamp = 1.0;

	//noiseSamp = saturate(noiseSamp - 0.1);
	//apply main wind dir and wind strength from the param map
	float3 windAdd = windDir * noiseSamp;



	#if !defined(LOWER_QUALITY)
	noiseScale = _noiseScale2.zxyw * 0.01;
	noiseOffset = _Time.x * float4(_noiseSpeed2.zxy, 0);
	float noiseSamp2 = SampleWind(pos.zxy);
	noiseSamp2 = pow(noiseSamp2, 2) - 0.5;

	windAdd += lerp(Float3(0.0), windDir2 * noiseSamp2, noiseSamp);


	//float flutter = cos(noiseSamp * 100) * windFlutter * saturate(noiseSamp2);
	//windAdd += windDir2 * flutter;
	#endif

		//dampen wind effet on flattened grass
	windAdd *= 1.0 - vd.dhfParams.z * 0.9;

	//increase wind effects on taller grass
	windAdd *= pow(finalHeight, 0.5);

	return windAdd * windMult;
}


static int MAX_RIPPLES = 128 + 1;
inline float3 GetRippleForce(in float3 pos) {

	RippleData rip;
	float totalStrength = 0;
	float3 forceDir = 0.001;

	uint ripCount = 0;

	#if defined(GRASS_RIPPLES)
		//count baked into here as part of squishing the buffers together
		ripCount = forcesBuffer[MAX_RIPPLES - 1].pos.x;

		if(ripCount + forcesCount == 0) return 0;

		for (uint i = 0; i < ripCount; i++) {
			rip = forcesBuffer[i];
			//float3 toPos = pos - mul(worldToGrass, float4(rip.pos.xyz, 1.0));
			float3 toPos = pos - rip.pos.xyz;
			float localStrength = rip.pos.w * (1.0 - saturate(dot(toPos, toPos) * rip.drssParams.z));
			totalStrength += localStrength;

			forceDir += toPos * localStrength;
		}
	#endif

	#if defined(GRASS_FORCES)
		ripCount = MAX_RIPPLES + forcesCount;
		for (int r = MAX_RIPPLES; r < ripCount; r++) {
			rip = forcesBuffer[r];
			//float3 toPos = pos - mul(worldToGrass, float4(rip.pos.xyz, 1.0));
			float3 toPos = pos - rip.pos.xyz;
			float localStrength = rip.drssParams.w * (1.0 - saturate(dot(toPos, toPos) * rip.drssParams.z));
			totalStrength += localStrength;

			forceDir += toPos * localStrength;
		}
	#else
		float3 toPos = pos - mainForcePos;
		float localStrength = mainForceParam.x * (1.0 - saturate(dot(toPos, toPos) * mainForceParam.y));
		#if !defined(GRASS_RIPPLES)
		return toPos * saturate(localStrength);
		#else
		totalStrength += localStrength;
		forceDir += toPos * localStrength;
		#endif
	#endif
		

	return normalize(forceDir) * saturate(totalStrength);
}


#if !defined(DEFERRED)
inline float3 GetMainLighting(float3 worldPos, float3 worldNormal, float4 clipPos) {

	half o;

	#if defined(FORWARD_ADD)
	float4 lightDir = GET_LIGHT_DIR(o);
	float3 lightVector = lightDir.xyz - worldPos * lightDir.w;
	float distanceSqr = max(dot(lightVector, lightVector), 0);
	half3 lightDirection = half3(lightVector * rsqrt(distanceSqr));
	#else
	half3 lightDirection = GET_LIGHT_DIR(o);
	#endif

	half shade = ambientCO + diffuseCO * saturate(dot(lightDirection, worldNormal) + 0.25);

	#if !defined(LOWER_QUALITY)
	shade *= 1 + max(dot(lightDirection, cross(cross(lightDirection, float3(0, -edgeLightSharp, 0)), worldNormal)) - (edgeLightSharp - edgeLight), 0);
	#endif

	float3 lighting = GET_LIGHT_COL(o) * shade;

	#if !defined(FORWARD_ADD)
	lighting += GET_GI(float4(worldNormal, 1), worldPos);
	#endif

	#if defined(SRP)
	//URP LIGHTING
	lighting += GetURPLighting(worldPos, worldNormal, clipPos);
	#endif

	return lighting;
}
#endif