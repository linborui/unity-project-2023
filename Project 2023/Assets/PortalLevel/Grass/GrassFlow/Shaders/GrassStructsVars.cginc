

//manual feature definitions
#define PREMULTIPLIED_WPOS
#define MAX_BATCH_COUNT 4



//SRP STUFF
#if !defined(SRP)
#ifndef SHADOW_CASTER
	//#define GRASS_SHADOW_COORDS(num) float4 _ShadowCoord : TEXCOORD5;
#define GRASS_SHADOW_COORDS(num) UNITY_SHADOW_COORDS(num)
#else
#define GRASS_SHADOW_COORDS(num)
#endif
#else

#define GRASS_SHADOW_COORDS(num) float4 _ShadowCoord : TEXCOORD5;

float3 _WorldSpaceLightPos0;
#endif


#if defined(SRP) && defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
//idunno, i feel like its just not really feasible to support lightmap stuff rn bc too much work
//theres no easy way to get it integrated into the bake system etc so getting it to receive lightmaps is tricky
//#define REQUIRES_LIGHTMAP_UV
#endif




//requirement definitions
#if (defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHT_SHADOWS)) && !defined(_ADDITIONAL_LIGHTS_VERTEX) && defined(GF_PPLIGHTS)
#define SRP_FRAGMENT_ADDITIONAL_LIGHTS
#endif


#if !defined(GRASS_DEPTH)
	#if defined(GF_SPECULAR)
	#if !defined(DEFERRED)
	#define USE_SPECULAR
	#endif
	
	
	#if defined(USE_SPECULAR) && defined(GF_PPLIGHTS)
	#define FRAGMENT_REQUIRES_SPECULAR
	#endif
	#endif
	
	
	#if defined(DEFERRED) || defined(GF_PPLIGHTS) || defined(FRAGMENT_REQUIRES_SPECULAR)\
			  || defined(GF_NORMAL_MAP) || defined(FORWARD_ADD)
	#define FRAGMENT_REQUIRES_NORMAL
	#endif

	#if defined(GF_SELF_SHADOW) && !defined(FORWARD_ADD)
	#define USE_SELF_SHADOW
	#endif
#endif


#if (defined(GF_PPLIGHTS) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)\
	|| (defined(SHADOWS_SCREEN) && !defined(UNITY_NO_SCREENSPACE_SHADOWS)))\
	&& !defined(DEFERRED)
	#define FRAGMENT_HANDLES_SHADOWS
#endif


#if defined(FORWARD_ADD) || defined(GF_PPLIGHTS)\
	|| defined(FRAGMENT_HANDLES_SHADOWS)
#define FRAGMENT_REQUIRES_WORLDPOS
#endif


#if !defined(DEFERRED)
uniform half4 _LightColor0;
#endif

uniform float numTexturesPctUV;

float4x4 objToWorldMatrix;
float4x4 worldToObjMatrix;


//---------------------------------------------------------------------------------
//----------------------------PROPS--------------------------------------
//---------------------------------------------------------------------------------

#if defined(SRP) && !defined(SHADER_API_METAL)
CBUFFER_START(UnityPerMaterial)
#endif

uniform half bladeWidth;
uniform half bladeSharp;
uniform float widthLODscale;

//an important note about the cbuffer props:
//it only matters for values represented in the shader properties, not values that are only assigned by script (afaik)

uniform float windMult;
uniform float4 _noiseScale;
uniform float4 _noiseSpeed;
uniform float3 windDir;
uniform float4 _noiseScale2;
uniform float4 _noiseSpeed2;
uniform float3 windDir2;
uniform float4 windTint;
//uniform float windFlutter;

uniform sampler2D _MainTex;
uniform half4 _MainTex_ST;


uniform float numTextures;
uniform int textureAtlasScalingCutoff;

uniform sampler2D dhfParamMap;
uniform sampler2D    colorMap;
uniform sampler2D     typeMap;
uniform half4 dhfParamMap_ST;
uniform half4    colorMap_ST;
uniform half4     typeMap_ST;

uniform half4 _Color;
uniform half4 altCol;
uniform bool alphaLock;
uniform half alphaClip;
uniform half _AO;
uniform half bladeHeight;
uniform half ambientCO;
#define diffuseCO (1.0 - ambientCO)
uniform half4 variance;
uniform half grassFade;
uniform half grassFadeSharpness;
uniform half seekSun;
uniform half topViewPush;
uniform half flatnessMult;
uniform half4 flatTint;
uniform half4 selfShadowScaleOffset;
uniform half selfShadowWind;

#if !defined(LOWER_QUALITY)
uniform sampler3D _NoiseTex;
#else
uniform sampler2D _NoiseTex2D;
#endif

#if !defined(LOWER_QUALITY)
uniform float bladeLateralCurve;
uniform float bladeVerticalCurve;
uniform float bladeStiffness;
#endif

#if !defined(DEFERRED)

uniform half ambientCOShadow;
uniform half edgeLight;
uniform half edgeLightSharp;

#else
uniform float _Metallic;
uniform float _Gloss;

uniform sampler2D _SpecMap;

uniform sampler2D _OccMap;
uniform float occMult;
#endif

#if defined(GF_SPECULAR)
uniform float specularMult;
uniform float specSmooth;
uniform float specHeight;
uniform float4 specTint;
#endif

#if defined(GF_USE_DITHER) || defined(SHADOW_CASTER) || defined(SEMI_TRANSPARENT)
uniform sampler3D _DitherMaskLOD;
#define DITHERMASK_REQUIRED
#endif



#if defined(GF_NORMAL_MAP)
uniform sampler2D bumpMap;
uniform float normalStrength;
#endif

uniform float blendNormal;


#if defined(SRP) && !defined(SHADER_API_METAL)
CBUFFER_END
#endif


#if !defined(GRASS_FORCES)
float3 mainForcePos;
float2 mainForceParam;
#endif

float meshInvVertCount;
uint grassPerTri;

//per chunk props
//float lodMult;
//uint posBufferOffset;
//float _instanceLod;
float4 batchData[MAX_BATCH_COUNT];

//---------------------------------------------------------------------------------
//----------------------------STRUCTS--------------------------------------
//---------------------------------------------------------------------------------



#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2) && !defined(GRASS_DEPTH)
#define FOG_ON
#endif


//#define MESH_UVS
//#define MESH_NORMALS
//#define MESH_COLORS

struct v2g {
	float3 vertex : POSITION;

	#if defined(MESH_UVS)
	float2 uv : TEXCOORD0;
	#endif

	#if defined(MESH_NORMALS)
	float3 norm : NORMAL;
	#endif

	#if defined(MESH_COLORS)
	float4 col : COLOR;
	#endif
};

#define COMPRESSED_POS

struct GrassPosCompressed {
	float3 pos;
	uint3 data1;
	uint2 data2;
};

//struct GrassPos {
//	float3 pos;
//	float3 norm;
//	float2 uv;
//	float4 params; // chunk id, height, flatness, wind
//	float4 col;
//	#define lodData params.x
//};

uniform float maxVertH;


#if defined(FRUSTUM_CULLED)
StructuredBuffer<float2> posIdBuffer;
#endif

#if defined(COMPRESSED_POS)
uniform StructuredBuffer<GrassPosCompressed> grassPosBuffer;
#else
uniform StructuredBuffer<GrassPos> grassPosBuffer;
#endif



struct ShadowDatas{
	float4 pos;
	float3 worldPos;
	float4 _ShadowCoord;
};

#if !defined(SHADOW_CASTER)
struct g2f {
	float4 pos : SV_POSITION;
	float4 col : COLOR;

	float4 uv : TEXCOORD0;

	#if defined(FRAGMENT_REQUIRES_WORLDPOS)
	float3 worldPos : TEXCOORD2;
	#endif

	#if defined(FRAGMENT_REQUIRES_NORMAL)
	float4 normal : NORMAL;
	#endif

	#if defined(FRAGMENT_REQUIRES_SPECULAR)
	float3 specVec : SPEC;
	#endif

	#if defined(USE_SELF_SHADOW)
	float3 selfShadowUV : SELFSHADOW;
	#endif

	#if defined(FRAGMENT_HANDLES_SHADOWS)
	GRASS_SHADOW_COORDS(5)
	#endif

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
	#endif	
};
#else
struct g2f {
	float4 pos : SV_POSITION;

	//#if defined(SEMI_TRANSPARENT)
	float4 uv : TEXCOORD0;
	//#endif

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
		#endif
};
#endif

struct VertexData {
	float3 vertex;
	float3 norm;
	float4 color;
	float4 dhfParams; //xyz = density, height, flatten, wind str
	float typeParams; //controls grass texture atlas index
	float lodData;
};

struct RippleData {
	float4 pos; // w = strength
	float4 drssParams;//xyzw = decay, radius, sharpness, speed
};


uniform StructuredBuffer<RippleData> forcesBuffer;
uniform int forcesCount;
