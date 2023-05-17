Shader "Hidden/Grass Material Shader" {
	Properties {

		[Space(15)]
		[HideInInspector] _CollapseStart("Grass Properties", Float) = 1
		[HDR]_Color("Grass Color", Color) = (1,1,1,1)
		bladeHeight("Blade Height", Float) = 1.0
		bladeWidth("Blade Width", Float) = 0.05
		bladeSharp("Blade Sharpness", Float) = 0.3
		seekSun("Seek Sun", Float) = 0.6
		topViewPush("Top View Adjust", Float) = 0.5
		flatnessMult("Flatness Adjust", Float) = 1.25
		[Toggle(BILLBOARD)]
		_BILLBOARD("Billboard", Float) = 1
		variance("Variances (p,h,c,w)", Vector) = (0.4, 0.4, 0.4, 0.4)
		_CollapseEnd("Grass Properties", Float) = 0

		[HideInInspector] _CollapseStart("Lighting Properties", Float) = 0
		_AO("AO", Float) = 0.25
		ambientCO("Ambient", Float) = 0.5
		ambientCOShadow("Shadow Ambient", Float) = 0.5
		edgeLight("Edge On Light", Float) = 0.4
		edgeLightSharp("Edge On Light Sharpness", Float) = 8
		_CollapseEnd("Lighting Properties", Float) = 0

		[Space(15)]
		[HideInInspector] _CollapseStart("LOD Properties", Float) = 0
		[Toggle(GF_USE_DITHER)]
		_GF_USE_DITHER("Use Dither", Float) = 0
		[Toggle(ALPHA_TO_MASK)]
		_ALPHA_TO_MASK("Alpha To Mask", Float) = 0
		widthLODscale("Width LOD Scale", Float) = 0.04
		grassFade("Grass Fade", Float) = 120
		grassFadeSharpness("Fade Sharpness", Float) = 8
		[HideInInspector]_LOD("LOD Params", Vector) = (20, 1.1, 0.2, 0.0)
		_CollapseEnd("LOD Properties", Float) = 0

		[Space(15)]
		[HideInInspector]_CollapseStart("Wind Properties", Float) = 0
		[HDR]windTint("windTint", Color) = (1,1,1, 0.15)
		_noiseScale("Noise Scale", Vector) = (1,1,.7)
		_noiseSpeed("Noise Speed", Vector) = (1.5,1,0.35)
		windDir  ("Wind Direction", Vector) = (-0.7,-0.6,0.1)
		windDir2 ("Secondary Wind Direction", Vector) = (0.5,0.5,1.2)
		_CollapseEnd("Wind Properties", Float) = 0

		[Space(15)]
		[HideInInspector]_CollapseStart("Bendable Settings", Float) = 0
		[Toggle(MULTI_SEGMENT)]
		_MULTI_SEGMENT("Enable Bending", Float) = 0
		bladeLateralCurve("Curvature", Float) = 0
		bladeVerticalCurve("Droop", Float) = 0
		bladeStiffness("Stiffness", Float) = 0
		_CollapseEnd("Bendable Settings", Float) = 0


		[Space(15)]
		[HideInInspector]_CollapseStart("Maps and Textures", Float) = 0
		[Toggle] alphaLock("Discard Texture Alpha", Float) = 1
		[Toggle(SEMI_TRANSPARENT)]
		_SEMI_TRANSPARENT("Enable Alpha Clip", Float) = 0
		alphaClip("Alpha Clip", Float) = 0.25
		numTextures("Number of Textures", Int) = 1
		textureAtlasScalingCutoff("Type Texture Scaling Cutoff", Int) = 16
		_MainTex("Grass Texture", 2D) = "white"{}
		[NoScaleOffset] colorMap("Grass Color Map", 2D) = "white"{}
		[NoScaleOffset] dhfParamMap("Grass Parameter Map", 2D) = "white"{}
		[NoScaleOffset] typeMap("Grass Type Map", 2D) = "black"{}
		_CollapseEnd("Maps and Textures", Float) = 0

		//[PerRendererData] _chunkPos("ChunkPos", Vector)= (0,0,0,0)
		[NoScaleOffset][HideInInspector] terrainNormalMap("Terrain Normal Map", 2D) = "black"{}
	}


	SubShader{

		pass {
			CGPROGRAM

			#pragma vertex vertex_shader
			#pragma fragment fragment_shader

			void vertex_shader(){}
			void fragment_shader(){}

			ENDCG
		}
	}
	CustomEditor "GrassFlow.GrassShaderGUI"
}
