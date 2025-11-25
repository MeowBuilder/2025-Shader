#pragma once

#include <string>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <cassert>

#include "Dependencies\glew.h"
#include "LoadPng.h"

class Renderer
{
public:
	Renderer(int windowSizeX, int windowSizeY);
	~Renderer();

	bool IsInitialized();
	void ReloadAllShaderPrograms();
	void DrawSolidRect(float x, float y, float z, float size, float r, float g, float b, float a);
	void DrawTest();
	void DrawParticle();

	void DrawGridMesh();
	void DrawFullScreenColor(float r, float g, float b, float a);

	void DrawFS();
	void DrawTexture(float x, float y, float sx, float sy, GLuint TexID);
	void DrawDebugTextures();

	void DrawFBOs();

private:
	void Initialize(int windowSizeX, int windowSizeY);
	void CompileAllShaderPrograms();
	void DeleteAllShaderPrograms();
	bool ReadFile(char* filename, std::string *target);
	void AddShader(GLuint ShaderProgram, const char* pShaderText, GLenum ShaderType);
	GLuint CompileShaders(char* filenameVS, char* filenameFS);
	void CreateVertexBufferObjects();
	void GetGLPosition(float x, float y, float *newX, float *newY);
	void GenerateParticles(int numParticle);
	void CreateGridMesh(int x, int y);
	GLuint CreatePngTexture(char* filePath, GLuint samplingMethod);
	void CreateFBOs();


	bool m_Initialized = false;
	
	unsigned int m_WindowSizeX = 0;
	unsigned int m_WindowSizeY = 0;

	GLuint m_VBORect = 0;
	GLuint m_SolidRectShader = 0;

	//lecture2, 3
	GLuint m_VBOTestPos = 0;
	GLuint m_VBOTestColor = 0;
	GLuint m_TestShader = 0;

	//Time
	float m_time = 0;

	//Particle
	GLuint m_ParticleShader = 0;
	GLuint m_VBOParticle = 0;
	GLuint m_VBOParticleVertexCount = 0;

	//Grid Mesh
	GLuint m_GridMeshShader = 0;
	GLuint m_GridMeshVertexCount = 0;
	GLuint m_GridMeshVBO = 0;

	//Full screen
	GLuint m_VBOFullScreen = 0;
	GLuint m_FullScreenShader = 0;

	//For raindrop effect
	float m_Points[100 * 4];

	//For Fragmnt Shader factory
	GLuint m_VBOFS = 0;
	GLuint m_FSShader = 0;

	//Textures
	GLuint m_RGBTexture = 0;
	GLuint m_FlagTexture = 0;

	GLuint m_0Texture = 0;
	GLuint m_1Texture = 0;
	GLuint m_2Texture = 0;
	GLuint m_3Texture = 0;
	GLuint m_4Texture = 0;
	GLuint m_5Texture = 0;
	GLuint m_6Texture = 0;
	GLuint m_7Texture = 0;
	GLuint m_8Texture = 0;
	GLuint m_9Texture = 0;
	GLuint m_NUMTexture = 0;

	//Texture
	GLuint m_TexVBO = 0;
	GLuint m_TexShader = 0;

	//FBO Color Buffer
	GLuint m_RT0 = 0;
	GLuint m_RT1 = 0;
	GLuint m_RT2 = 0;
	GLuint m_RT3 = 0;
	GLuint m_RT4 = 0;

	//FBOs
	GLuint m_FBO0 = 0;
	GLuint m_FBO1 = 0;
	GLuint m_FBO2 = 0;
	GLuint m_FBO3 = 0;
	GLuint m_FBO4 = 0;
};

