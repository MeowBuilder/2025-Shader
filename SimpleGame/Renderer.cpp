#include "stdafx.h"
#include "Renderer.h"

Renderer::Renderer(int windowSizeX, int windowSizeY)
{
	Initialize(windowSizeX, windowSizeY);
}


Renderer::~Renderer()
{
}

void Renderer::Initialize(int windowSizeX, int windowSizeY)
{
	//Set window size
	m_WindowSizeX = windowSizeX;
	m_WindowSizeY = windowSizeY;

	//Compile All Shader Programs
	CompileAllShaderPrograms();
	
	//Create VBOs
	CreateVertexBufferObjects();

	GenerateParticles(10000);

	if (m_SolidRectShader > 0 && m_VBORect > 0)
	{
		m_Initialized = true;
	}
}

void Renderer::CompileAllShaderPrograms()
{
	//Load shaders
	m_SolidRectShader = CompileShaders("./Shaders/SolidRect.vs", "./Shaders/SolidRect.fs");
	m_TestShader = CompileShaders("./Shaders/Test.vs", "./Shaders/Test.fs");
	m_ParticleShader = CompileShaders("./Shaders/Particle.vs", "./Shaders/Particle.fs");
}

void Renderer::DeleteAllShaderPrograms()
{
	glDeleteShader(m_SolidRectShader);
	glDeleteShader(m_TestShader);
	glDeleteShader(m_ParticleShader);
}

bool Renderer::IsInitialized()
{
	return m_Initialized;
}

void Renderer::ReloadAllShaderPrograms()
{
	DeleteAllShaderPrograms();
	CompileAllShaderPrograms();
}

void Renderer::CreateVertexBufferObjects()
{
	float rect[]
		=
	{
		-1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f, -1.f / m_WindowSizeX, 1.f / m_WindowSizeY, 0.f, 1.f / m_WindowSizeX, 1.f / m_WindowSizeY, 0.f, //Triangle1
		-1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f,  1.f / m_WindowSizeX, 1.f / m_WindowSizeY, 0.f, 1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f, //Triangle2
	};

	glGenBuffers(1, &m_VBORect);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBORect);
	glBufferData(GL_ARRAY_BUFFER, sizeof(rect), rect, GL_STATIC_DRAW);

	float temp = 0.5f;
	float size = 0.1;

	//lecture2, 3
	float testPos[] =
	{
		(0.f - temp) * size, (0.f - temp) * size, 0.f, 0.5,
		(1.f - temp) * size, (0.f - temp) * size, 0.f, 0.5,
		(1.f - temp) * size, (1.f - temp) * size, 0.f, 0.5,

		(0.f - temp) * size, (0.f - temp) * size, 0.f, 0.5,
		(1.f - temp) * size, (1.f - temp) * size, 0.f, 0.5,
		(0.f - temp) * size, (1.f - temp) * size, 0.f, 0.5, //Quad1

		(0.f - temp)* size, (0.f - temp)* size, 0.f, 1.0,
		(1.f - temp)* size, (0.f - temp)* size, 0.f, 1.0,
		(1.f - temp)* size, (1.f - temp)* size, 0.f, 1.0,

		(0.f - temp)* size, (0.f - temp)* size, 0.f, 1.0,
		(1.f - temp)* size, (1.f - temp)* size, 0.f, 1.0,
		(0.f - temp)* size, (1.f - temp)* size, 0.f, 1.0, //Quad2
	};

	glGenBuffers(1, &m_VBOTestPos);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTestPos);
	glBufferData(GL_ARRAY_BUFFER, sizeof(testPos), testPos, GL_STATIC_DRAW); //�̶� VRAM�� �޸𸮰� ������ ���ε尡 �����

	float testColor[] =
	{
		1.f, 1.f, 1.f, 1.f,
		1.f, 1.f, 1.f, 1.f,
		1.f, 1.f, 1.f, 1.f, 

		0.f, 0.f, 0.f, 1.f,
		0.f, 0.f, 0.f, 1.f, 
		0.f, 0.f, 0.f, 1.f,//Quad1

		1.f, 1.f, 1.f, 1.f,
		1.f, 1.f, 1.f, 1.f,
		1.f, 1.f, 1.f, 1.f,

		0.f, 0.f, 0.f, 1.f,
		0.f, 0.f, 0.f, 1.f,
		0.f, 0.f, 0.f, 1.f,//Quad2
	};

	glGenBuffers(1, &m_VBOTestColor);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTestColor);
	glBufferData(GL_ARRAY_BUFFER, sizeof(testColor), testColor, GL_STATIC_DRAW); //�̶� VRAM�� �޸𸮰� ������ ���ε尡 �����
}

void Renderer::AddShader(GLuint ShaderProgram, const char* pShaderText, GLenum ShaderType)
{
	//���̴� ������Ʈ ����
	GLuint ShaderObj = glCreateShader(ShaderType);

	if (ShaderObj == 0) {
		fprintf(stderr, "Error creating shader type %d\n", ShaderType);
	}

	const GLchar* p[1];
	p[0] = pShaderText;
	GLint Lengths[1];

	size_t slen = strlen(pShaderText);
	if (slen > INT_MAX) {
		// Handle error
	}
	GLint len = (GLint)slen;

	Lengths[0] = len;
	//���̴� �ڵ带 ���̴� ������Ʈ�� �Ҵ�
	glShaderSource(ShaderObj, 1, p, Lengths);

	//�Ҵ�� ���̴� �ڵ带 ������
	glCompileShader(ShaderObj);

	GLint success;
	// ShaderObj �� ���������� ������ �Ǿ����� Ȯ��
	glGetShaderiv(ShaderObj, GL_COMPILE_STATUS, &success);
	if (!success) {
		GLchar InfoLog[1024];

		//OpenGL �� shader log �����͸� ������
		glGetShaderInfoLog(ShaderObj, 1024, NULL, InfoLog);
		fprintf(stderr, "Error compiling shader type %d: '%s'\n", ShaderType, InfoLog);
		printf("%s \n", pShaderText);
	}

	// ShaderProgram �� attach!!
	glAttachShader(ShaderProgram, ShaderObj);
}

bool Renderer::ReadFile(char* filename, std::string *target)
{
	std::ifstream file(filename);
	if (file.fail())
	{
		std::cout << filename << " file loading failed.. \n";
		file.close();
		return false;
	}
	std::string line;
	while (getline(file, line)) {
		target->append(line.c_str());
		target->append("\n");
	}
	return true;
}

GLuint Renderer::CompileShaders(char* filenameVS, char* filenameFS)
{
	GLuint ShaderProgram = glCreateProgram(); //�� ���̴� ���α׷� ����

	if (ShaderProgram == 0) { //���̴� ���α׷��� ����������� Ȯ��
		fprintf(stderr, "Error creating shader program\n");
	}

	std::string vs, fs;

	//shader.vs �� vs ������ �ε���
	if (!ReadFile(filenameVS, &vs)) {
		printf("Error compiling vertex shader\n");
		return -1;
	};

	//shader.fs �� fs ������ �ε���
	if (!ReadFile(filenameFS, &fs)) {
		printf("Error compiling fragment shader\n");
		return -1;
	};

	// ShaderProgram �� vs.c_str() ���ؽ� ���̴��� �������� ����� attach��
	AddShader(ShaderProgram, vs.c_str(), GL_VERTEX_SHADER);

	// ShaderProgram �� fs.c_str() �����׸�Ʈ ���̴��� �������� ����� attach��
	AddShader(ShaderProgram, fs.c_str(), GL_FRAGMENT_SHADER);

	GLint Success = 0;
	GLchar ErrorLog[1024] = { 0 };

	//Attach �Ϸ�� shaderProgram �� ��ŷ��
	glLinkProgram(ShaderProgram);

	//��ũ�� �����ߴ��� Ȯ��
	glGetProgramiv(ShaderProgram, GL_LINK_STATUS, &Success);

	if (Success == 0) {
		// shader program �α׸� �޾ƿ�
		glGetProgramInfoLog(ShaderProgram, sizeof(ErrorLog), NULL, ErrorLog);
		std::cout << filenameVS << ", " << filenameFS << " Error linking shader program\n" << ErrorLog;
		return -1;
	}

	glValidateProgram(ShaderProgram);
	glGetProgramiv(ShaderProgram, GL_VALIDATE_STATUS, &Success);
	if (!Success) {
		glGetProgramInfoLog(ShaderProgram, sizeof(ErrorLog), NULL, ErrorLog);
		std::cout << filenameVS << ", " << filenameFS << " Error validating shader program\n" << ErrorLog;
		return -1;
	}

	glUseProgram(ShaderProgram);
	std::cout << filenameVS << ", " << filenameFS << " Shader compiling is done.";

	return ShaderProgram;
}

void Renderer::DrawSolidRect(float x, float y, float z, float size, float r, float g, float b, float a)
{
	float newX, newY;

	GetGLPosition(x, y, &newX, &newY);

	//Program select
	glUseProgram(m_SolidRectShader);

	glUniform4f(glGetUniformLocation(m_SolidRectShader, "u_Trans"), newX, newY, 0, size);
	glUniform4f(glGetUniformLocation(m_SolidRectShader, "u_Color"), r, g, b, a);

	int attribPosition = glGetAttribLocation(m_SolidRectShader, "a_Position");
	glEnableVertexAttribArray(attribPosition);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBORect);
	glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 3, 0);

	glDrawArrays(GL_TRIANGLES, 0, 6);

	glDisableVertexAttribArray(attribPosition);

	glBindFramebuffer(GL_FRAMEBUFFER, 0);
}

void Renderer::DrawTest()
{
	m_time += 0.016;

	//Program select
	glUseProgram(m_TestShader);

	int uTimeLoc = glGetUniformLocation(m_TestShader, "u_Time");
	glUniform1f(uTimeLoc, m_time);

	int a_PositionLoc = glGetAttribLocation(m_TestShader, "a_Position");
	int a_RadiusLoc = glGetAttribLocation(m_TestShader, "a_Radius");
	int a_ColorLoc = glGetAttribLocation(m_TestShader, "a_Color");

	glEnableVertexAttribArray(a_PositionLoc);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTestPos);
	glVertexAttribPointer(a_PositionLoc, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 4, 0);

	glEnableVertexAttribArray(a_RadiusLoc);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTestPos);
	glVertexAttribPointer(a_RadiusLoc, 1, GL_FLOAT, GL_FALSE, sizeof(float) * 4, (GLvoid*)(sizeof(float) * 3));

	glEnableVertexAttribArray(a_ColorLoc);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTestColor);
	glVertexAttribPointer(a_ColorLoc, 4, GL_FLOAT, GL_FALSE, sizeof(float) * 4, 0);

	glDrawArrays(GL_TRIANGLES, 0, 12);
	glDisableVertexAttribArray(a_PositionLoc);
	glDisableVertexAttribArray(a_ColorLoc);
	glBindFramebuffer(GL_FRAMEBUFFER, 0);
}

void Renderer::DrawParticle()
{
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	m_time += 0.016;

	//Program select
	GLuint shader = m_ParticleShader;
	glUseProgram(shader);

	int uTimeLoc = glGetUniformLocation(shader, "u_Time");
	glUniform1f(uTimeLoc, m_time);

	int uForceLoc = glGetUniformLocation(shader, "u_Force");	
	glUniform3f(uForceLoc, 0, 0, 0);

	int a_PositionLoc = glGetAttribLocation(shader, "a_Position");
	int a_ValueLoc = glGetAttribLocation(shader, "a_Value");
	int a_ColorLoc = glGetAttribLocation(shader, "a_Color");
	int a_lifeTimeLoc = glGetAttribLocation(shader, "a_lifeTime");
	int a_VelLoc = glGetAttribLocation(shader, "a_Vel");
	int a_MassLoc = glGetAttribLocation(shader, "a_Mass");
	int a_PeriodLoc = glGetAttribLocation(shader, "a_Period");

	glBindBuffer(GL_ARRAY_BUFFER, m_VBOParticle);

	glEnableVertexAttribArray(a_PositionLoc);
	glEnableVertexAttribArray(a_ValueLoc);
	glEnableVertexAttribArray(a_ColorLoc);
	glEnableVertexAttribArray(a_lifeTimeLoc);
	glEnableVertexAttribArray(a_VelLoc);
	glEnableVertexAttribArray(a_MassLoc);
	glEnableVertexAttribArray(a_PeriodLoc);

	int stridesize = 15;

	glVertexAttribPointer(a_PositionLoc, 3, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, 0);
	glVertexAttribPointer(a_ValueLoc, 1, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, (GLvoid*)(sizeof(float) * 3));
	glVertexAttribPointer(a_ColorLoc, 4, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, (GLvoid*)(sizeof(float) * 4));
	glVertexAttribPointer(a_lifeTimeLoc, 2, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, (GLvoid*)(sizeof(float) * 8));
	glVertexAttribPointer(a_VelLoc, 3, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, (GLvoid*)(sizeof(float) * 10));
	glVertexAttribPointer(a_MassLoc, 1, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, (GLvoid*)(sizeof(float) * 13));
	glVertexAttribPointer(a_PeriodLoc, 1, GL_FLOAT, GL_FALSE, sizeof(float) * stridesize, (GLvoid*)(sizeof(float) * 14));

	glDrawArrays(GL_TRIANGLES, 0, m_VBOParticleVertexCount);
	//glDrawArrays(GL_TRIANGLES, 0,6);

	glDisableVertexAttribArray(a_PositionLoc);
	glDisableVertexAttribArray(a_ValueLoc);
	glDisableVertexAttribArray(a_ColorLoc);
	glDisableVertexAttribArray(a_lifeTimeLoc);
	glDisableVertexAttribArray(a_VelLoc);
	glDisableVertexAttribArray(a_MassLoc);
	glDisableVertexAttribArray(a_PeriodLoc);

	glBindFramebuffer(GL_FRAMEBUFFER, 0);

	glDisable(GL_BLEND);
}

void Renderer::GetGLPosition(float x, float y, float *newX, float *newY)
{
	*newX = x * 2.f / m_WindowSizeX;
	*newY = y * 2.f / m_WindowSizeY;
}

void Renderer::GenerateParticles(int numParticle)
{
	int floatCountPerVertex = 3 + 1 + 4 + 2 + 3 + 1 + 1; 
	//x, y, z, value, r, g, b, a, (sTime, lifeTime), vx, vy, vz, mass, period
	int verticesCountPerParticle = 6;
	int floatCountPerParticle = floatCountPerVertex * verticesCountPerParticle;
	int totalVerticesCount = numParticle * verticesCountPerParticle;
	int totalFloatCount = floatCountPerVertex * totalVerticesCount;

	float* vertices = new float[totalFloatCount];

	for (int i = 0; i < numParticle; i++)
	{
		float x, y, z, value, r, g, b, a;
		x = 0.f;//(float)rand() / (float)RAND_MAX * 4.f - 2.f;
		y = 0.f;//(float)rand() / (float)RAND_MAX + 1.0f;
		z = 0.f;
		value = (float)rand() / (float)RAND_MAX;
		r = (float)rand() / (float)RAND_MAX;
		g = (float)rand() / (float)RAND_MAX;
		b = (float)rand() / (float)RAND_MAX;
		a = (float)rand() / (float)RAND_MAX;
		//r = 1;
		//g = 1;
		//b = 1;
		//a = 1;
		float size;
		size = ((float)rand() / (float)RAND_MAX) * 0.01;
		//size = 0.05f;
		float sTime, lifeTime;
		sTime = ((float)rand() / (float)RAND_MAX) * 5.0;
		lifeTime = ((float)rand() / (float)RAND_MAX);
		float vx, vy, vz;
		vx = ((float)rand() / (float)RAND_MAX * 2.f - 1.0f);
		vy = ((float)rand() / (float)RAND_MAX * 2.f - 1.0f);
		vz = 0.f;
		float mass = ((float)rand() / (float)RAND_MAX) + 1.0f;
		float period = ((float)rand() / (float)RAND_MAX) + 1.f;


		int index = i * floatCountPerParticle;

		vertices[index] = x - size; index++; //v1
		vertices[index] = y - size; index++;
		vertices[index] = z; index++;
		vertices[index] = value; index++;
		vertices[index] = r; index++;
		vertices[index] = g; index++;
		vertices[index] = b; index++;
		vertices[index] = a; index++;
		vertices[index] = sTime; index++;
		vertices[index] = lifeTime; index++;
		vertices[index] = vx; index++;
		vertices[index] = vy; index++;
		vertices[index] = vz; index++;
		vertices[index] = mass; index++;
		vertices[index] = period; index++;

		vertices[index] = x + size; index++; //v2
		vertices[index] = y + size; index++;
		vertices[index] = z; index++;
		vertices[index] = value; index++;
		vertices[index] = r; index++;
		vertices[index] = g; index++;
		vertices[index] = b; index++;
		vertices[index] = a; index++;
		vertices[index] = sTime; index++;
		vertices[index] = lifeTime; index++;
		vertices[index] = vx; index++;
		vertices[index] = vy; index++;
		vertices[index] = vz; index++;
		vertices[index] = mass; index++;
		vertices[index] = period; index++;

		vertices[index] = x - size; index++; //v3
		vertices[index] = y + size; index++;
		vertices[index] = z; index++;
		vertices[index] = value; index++;
		vertices[index] = r; index++;
		vertices[index] = g; index++;
		vertices[index] = b; index++;
		vertices[index] = a; index++;
		vertices[index] = sTime; index++;
		vertices[index] = lifeTime; index++;
		vertices[index] = vx; index++;
		vertices[index] = vy; index++;
		vertices[index] = vz; index++;
		vertices[index] = mass; index++;
		vertices[index] = period; index++;

		vertices[index] = x - size; index++; //v4
		vertices[index] = y - size; index++;
		vertices[index] = z; index++;
		vertices[index] = value; index++;
		vertices[index] = r; index++;
		vertices[index] = g; index++;
		vertices[index] = b; index++;
		vertices[index] = a; index++;
		vertices[index] = sTime; index++;
		vertices[index] = lifeTime; index++;
		vertices[index] = vx; index++;
		vertices[index] = vy; index++;
		vertices[index] = vz; index++;
		vertices[index] = mass; index++;
		vertices[index] = period; index++;

		vertices[index] = x + size; index++; //v5
		vertices[index] = y - size; index++;
		vertices[index] = z; index++;
		vertices[index] = value; index++;
		vertices[index] = r; index++;
		vertices[index] = g; index++;
		vertices[index] = b; index++;
		vertices[index] = a; index++;
		vertices[index] = sTime; index++;
		vertices[index] = lifeTime; index++;
		vertices[index] = vx; index++;
		vertices[index] = vy; index++;
		vertices[index] = vz; index++;
		vertices[index] = mass; index++;
		vertices[index] = period; index++;

		vertices[index] = x + size; index++; //v6
		vertices[index] = y + size; index++;
		vertices[index] = z; index++;
		vertices[index] = value; index++;
		vertices[index] = r; index++;
		vertices[index] = g; index++;
		vertices[index] = b; index++;
		vertices[index] = a; index++;
		vertices[index] = sTime; index++;
		vertices[index] = lifeTime; index++;
		vertices[index] = vx; index++;
		vertices[index] = vy; index++;
		vertices[index] = vz; index++;
		vertices[index] = mass; index++;
		vertices[index] = period; index++;

	}

	glGenBuffers(1, &m_VBOParticle);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOParticle);
	glBufferData(GL_ARRAY_BUFFER, sizeof(float) * totalFloatCount, vertices, GL_STATIC_DRAW);

	delete[] vertices;

	m_VBOParticleVertexCount = totalVerticesCount;
}