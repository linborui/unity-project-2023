Shader "GrassFlow/Forward Standard" {
	Properties {


		//---------------------------------------------------------------------------------
		//----------------------------GRASS PROPS--------------------------------------
		//---------------------------------------------------------------------------------
		[Space(15)]
		[HideInInspector] _CollapseStart("Grass Properties", Float) = 1
		[HDR]_Color("Grass Color", Color) = (1,1,1,1)
		bladeHeight("Blade Height", Float) = 1.0
		bladeWidth("Blade Width", Float) = 0.05
		bladeSharp("Blade Sharpness", Float) = 0.3
		[Toggle(BILLBOARD)]
		_BILLBOARD("Billboard", Float) = 1
		seekSun("Seek Sun", Float) = 0.6
		topViewPush("Top View Adjust", Float) = 0.5
		flatnessMult("Flatness Adjust", Float) = 1.25
		[HDR]flatTint("Flatness Tint", Color) = (1,1,1, 0.15)
		[HDR]altCol("Variation Color", Color) = (0,0,0,1)
		variance("Variances (p,h,c,w)", Vector) = (0.4, 0.4, 0.4, 0.4)	
		_CollapseEnd("Grass Properties", Float) = 0



		//---------------------------------------------------------------------------------
		//----------------------------LIGHTING--------------------------------------
		//---------------------------------------------------------------------------------
		[HideInInspector] _CollapseStart("Lighting Properties", Float) = 0
	
	
		[Toggle(GF_PPLIGHTS)] _ppLights("Per-Pixel Lights", Float) = 0
	

		_AO("AO", Float) = 0.25
		ambientCO("Ambient", Float) = 0.1

		blendNormal("Blend Surface Normal", Float) = 0


	
		ambientCOShadow("Shadow Ambient", Float) = 0.5
	
		edgeLight("Edge On Light", Float) = 0.4
		edgeLightSharp("Edge On Light Sharpness", Float) = 8
	

		[HideInInspector] _CollapseStart("Specular", Float) = 0
		[Toggle(GF_SPECULAR)]_GF_SPECULAR("Enable Specular", Float) = 0
		specSmooth("Smoothness", Float) = 0.16
		specularMult("Specular Mult", Float) = 2
		specHeight("Specular Height Adjust", Float) = 0.5
		specTint("Specular Tint", Color) = (1,1,1,1)
		_CollapseEnd("Specular", Float) = 0
	
	

		[HideInInspector] _CollapseStart("Normal Map", Float) = 0
		[Toggle(GF_NORMAL_MAP)]_GF_NORMAL_MAP("Enable Normal Mapping", Float) = 0
		[NoScaleOffset] bumpMap("Normal Map", 2D) = "bump" {}
		normalStrength("Strength", Float) = 0.1
		_CollapseEnd("Normal Map", Float) = 0

		[HideInInspector] _CollapseStart("Self Shadow", Float) = 0
		[Toggle(GF_SELF_SHADOW)]  GF_SELF_SHADOW ("Fake Self Shadow", Float) = 0
		selfShadowWind ("Self Shadow Wind", float) = 0.15
		selfShadowScaleOffset("Self Shadow Scale/Offset", Vector) = (0.75, 0.75, 0.5, 0)
		_CollapseEnd("Self Shadow", Float) = 0

		_CollapseEnd("Lighting Properties", Float) = 0



		//---------------------------------------------------------------------------------
		//----------------------------LOD--------------------------------------
		//---------------------------------------------------------------------------------
		[Space(15)]
		[HideInInspector] _CollapseStart("LOD Properties", Float) = 0
	
		[Toggle(ALPHA_TO_MASK)]
		_ALPHA_TO_MASK("Alpha To Mask", Float) = 0
		[Toggle(GF_USE_DITHER)]
		_GF_USE_DITHER("Use Dither", Float) = 0
	
		[Toggle(LOD_SCALING)]
		_LOD_SCALING("Use LOD Scaling", Float) = 0
		widthLODscale("Width LOD Scale", Float) = 0.04
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling Mode", Float) = 0
		grassFade("Grass Fade", Float) = 120
		grassFadeSharpness("Fade Sharpness", Float) = 8
		_CollapseEnd("LOD Properties", Float) = 0



		//---------------------------------------------------------------------------------
		//----------------------------WIND--------------------------------------
		//---------------------------------------------------------------------------------
		[Space(15)]
		[HideInInspector]_CollapseStart("Wind Properties", Float) = 0
		windMult("Wind Strength Mult", Float) = 1
		[HDR]windTint("Wind Tint", Color) = (1,1,1, 0.15)
		_noiseScale("Noise Scale", Vector) = (1,1,.7)
		_noiseSpeed("Noise Speed", Vector) = (1.5,1,0.35)
		windDir  ("Wind Direction", Vector) = (-0.7,-0.6,0.1)
	
		_noiseScale2("Secondary Noise Scale", Vector) = (2,2,1)
		_noiseSpeed2("Secondary Noise Speed", Vector) = (2.5,2,1.35)
		windDir2 ("Secondary Wind Direction", Vector) = (0.5,0.5,1.2)
	
		_CollapseEnd("Wind Properties", Float) = 0



	
		//---------------------------------------------------------------------------------
		//----------------------------BENDING--------------------------------------
		//---------------------------------------------------------------------------------
		[Space(15)]
		[HideInInspector]_CollapseStart("Bendable Settings", Float) = 0
		bladeLateralCurve("Curvature", Float) = 0
		bladeVerticalCurve("Droop", Float) = 0
		bladeStiffness("Floppyness", Float) = 0
		_CollapseEnd("Bendable Settings", Float) = 0
	


		//---------------------------------------------------------------------------------
		//----------------------------MAPS--------------------------------------
		//---------------------------------------------------------------------------------
		[Space(15)]
		[HideInInspector]_CollapseStart("Maps and Textures", Float) = 0
	
		[Toggle] alphaLock("Discard Texture Alpha", Float) = 1
		[Toggle(SEMI_TRANSPARENT)]
		_SEMI_TRANSPARENT("Enable Alpha Clip", Float) = 0
		alphaClip("Alpha Clip", Float) = 0.25
	
		numTextures("Number of Textures", Int) = 1
		textureAtlasScalingCutoff("Type Texture Scaling Cutoff", Int) = 16
		_MainTex("Grass Texture", 2D) = "white"{}
	
		colorMap("Grass Color Map", 2D) = "white"{}
		dhfParamMap("Grass Parameter Map", 2D) = "white"{}
		typeMap("Grass Type Map", 2D) = "black"{}
		_CollapseEnd("Maps and Textures", Float) = 0

	
		//---------------------------------------------------------------------------------
		//----------------------------OPTIMIZATION--------------------------------------
		//---------------------------------------------------------------------------------
	[HideInInspector]_CollapseStart("Performance & Optimization", Float) = 0
		[Toggle(MESH_UVS)] MESH_UVS("Use Mesh UVs", Float) = 1
		[Toggle(MESH_NORMALS)] MESH_NORMALS("Use Mesh Normals", Float) = 0
		[Toggle(MESH_COLORS)] MESH_COLORS("Use Vertex Height Colors", Float) = 0

		[Toggle(MAP_COLOR)] MAP_COLOR("Dynamic Color Map", Float) = 0
		[Toggle(MAP_PARAM)] MAP_PARAM("Dynamic Param Map", Float) = 1
		[Toggle(MAP_TYPE)]  MAP_TYPE ("Dynamic Type  Map", Float) = 0

		[Toggle(GRASS_RIPPLES)]  GRASS_RIPPLES ("Allow Ripples", Float) = 0
		[Toggle(GRASS_FORCES)]  GRASS_FORCES ("Allow Multiple Forces", Float) = 1
	_CollapseEnd("Performance & Optimization", Float) = 0



		//---------------------------------------------------------------------------------
		//----------------------------HIDDEN SHADER VARIANT VALUES--------------------------------------
		//---------------------------------------------------------------------------------
		[HideInInspector]Pipe_Type("Pipe_Type", Float) = 0
		[HideInInspector]Render_Path("Render_Path", Float) = 0
		[HideInInspector]Depth_Pass("Depth_Pass", Float) = 1
		[HideInInspector]Forward_Add("Forward_Add", Float) = 0
		[HideInInspector]No_Transparency("No_Transparency", Float) = 0
		[HideInInspector]Lower_Quality("Lower_Quality", Float) = 0
		[HideInInspector]VERSION("VERSION", Float) = 14
	}

	SubShader{


	
	Tags{ "Queue" = "AlphaTest"}
	

	
		pass {
			Name "ForwardBasePass"

		
			Blend SrcAlpha OneMinusSrcAlpha
		
		
			Tags {"LightMode" = "ForwardBase" }
		
		

			
			AlphaToMask[_ALPHA_TO_MASK]
			

			Cull [_Cull]

			CGPROGRAM //-----------------

		
			#pragma multi_compile_fwdbase
		
			#pragma multi_compile_fog
			
			
			#pragma shader_feature_local GF_SPECULAR
			#pragma shader_feature_local GF_PPLIGHTS
			#pragma shader_feature_local GF_NORMAL_MAP
			#pragma shader_feature_local GF_SELF_SHADOW

			#pragma shader_feature_local MESH_NORMALS

			#pragma shader_feature_local MAP_COLOR
			#pragma shader_feature_local MAP_TYPE
			
		


			#pragma target 4.5

			#pragma fragment fragment_shader
			#pragma vertex mesh_vertex_shader

			
			#pragma shader_feature_local GF_USE_DITHER
			#pragma shader_feature_local SEMI_TRANSPARENT
			
			

			#pragma shader_feature_local FRUSTUM_CULLED
			#pragma shader_feature_local BILLBOARD
			#pragma shader_feature_local LOD_SCALING

			#pragma shader_feature_local MESH_UVS
			#pragma shader_feature_local MESH_COLORS

			#pragma shader_feature_local MAP_PARAM
			#pragma shader_feature_local USE_MAPS_OVERRIDE

			#pragma shader_feature_local GRASS_RIPPLES
			#pragma shader_feature_local GRASS_FORCES




		
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
		
		
			#include "../GrassPrograms.cginc"

			ENDCG
		}// base pass
	

	

	

	
		
	}

	CustomEditor "GrassFlow.GrassShaderGUI"
}