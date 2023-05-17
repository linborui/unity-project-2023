


//this shader isnt meant to be rendered, its just a shell that stores the GF shader variant values so that 
//the GrassShaderGUI can compile the right version
//it just serves as a safe backup in case the material gets borked somehow, this shader should always work since its basically empty

Shader "GrassFlow/Grass Material Repair" {
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


		//---------------------------------------------------------------------------------
		//----------------------------OPTIMIZATION--------------------------------------
		//---------------------------------------------------------------------------------
	[HideInInspector]_CollapseStart("Performance & Optimization", Float) = 0
		[Toggle(MESH_UVS)] MESH_UVS("Use Mesh UVs", Float) = 1
		[Toggle(MESH_NORMALS)] MESH_NORMALS("Use Mesh Normals", Float) = 0
		[Toggle(MESH_COLORS)] MESH_COLORS("Use Vertex Height Colors", Float) = 0

		[Toggle(MAP_COLOR)] MAP_COLOR("Dynamic Color Map", Float) = 0
		[Toggle(MAP_PARAM)] MAP_PARAM("Dynamic Param Map", Float) = 0
		[Toggle(MAP_TYPE)]  MAP_TYPE ("Dynamic Type  Map", Float) = 0

		[Toggle(GRASS_RIPPLES)]  GRASS_RIPPLES ("Allow Ripples", Float) = 0
		[Toggle(GRASS_FORCES)]  GRASS_FORCES ("Allow Multiple Forces", Float) = 1
	_CollapseEnd("Performance & Optimization", Float) = 0


		//---------------------------------------------------------------------------------
		//----------------------------HIDDEN SHADER VARIANT VALUES--------------------------------------
		//---------------------------------------------------------------------------------
		[HideInInspector]Pipe_Type("Pipe_Type", Float) = 0
		[HideInInspector]Render_Type("Render_Type", Float) = 0
		[HideInInspector]Render_Path("Render_Path", Float) = 0
		[HideInInspector]Depth_Pass("Depth_Pass", Float) = 1
		[HideInInspector]Forward_Add("Forward_Add", Float) = 0			
		[HideInInspector]No_Transparency("No_Transparency", Float) = 0
		[HideInInspector]Lower_Quality("Lower_Quality", Float) = 0
		[HideInInspector]VERSION("VERSION", Float) = 13
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
