#include <iostream>
#include <cstdlib>
#include <ctime>

#define STB_IMAGE_IMPLEMENTATION
#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <gl/glew.h>
#include <GLFW/glfw3.h>
#include<glm/glm.hpp>
#include<glm/gtc/matrix_transform.hpp>
#include<glm/gtc/type_ptr.hpp>
#include "Shader.h"
#include "ShaderProgram.h"
#include "Texture.h"
#include "VAO.h"
#include "VBO.h"
#include "EBO.h"
#include "GridSettings.h"
#include "CameraSettings.h"

#pragma comment(lib, "opengl32.lib")
#pragma comment(lib, "glu32.lib")

using namespace std;
using namespace glm;

GLFWwindow* window;
GridSettings gridSettings;
CameraSettings camSettings;
stringstream output;
float vertScale = 0.5f;
double currFPS = 0;

const int width = 800;
const int height = 600;
GLfloat squareVert[] =
{ //     COORDINATES     /        COLORS      /   TexCoord  //
	-1.0f, -1.0f, 0.0f,		1.00f, 1.00f, 0.00f,	0.0f, 0.0f,	// Bottom left
	1.0f, -1.0f, 0.0f,		1.00f, 1.00f, 0.00f,	1.0f, 0.0f, // Bottom right
	1.0f, 1.0f, 0.0f,		1.00f, 1.00f, 0.00f,	1.0f, 1.0f, // Top right
	-1.0f, 1.0f, 0.0f,		1.00f, 1.00f, 0.00f,	0.0f, 1.0f, // Top left
};

GLuint squareInd[] =
{
	0, 1, 3,
	2, 1, 3
};

int InitContext();
int DestroyContext();
void DrawGrid(ShaderProgram* shaderProgram);

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}


extern "C" __declspec(dllexport) int MainRoutine() {
	int initRes = InitContext();
	if (initRes != EXIT_SUCCESS) {
		return initRes;
	}

	Shader vertexShader(GL_VERTEX_SHADER, "default.vert", &output);
	Shader fragShader(GL_FRAGMENT_SHADER, "default.frag", &output);
	ShaderProgram shaderProgram(&vertexShader, &fragShader, &output);
	vertexShader.Destroy();
	fragShader.Destroy();

	VAO vao(&output);
	vao.Bind();

	VBO squareVBO(squareVert, sizeof(squareVert), &output);
	EBO squareEBO(squareInd, sizeof(squareInd), &output);
	vao.LinkAttrib(squareVBO, 0, 3, GL_FLOAT, 8 * sizeof(float), (void*)0);
	vao.LinkAttrib(squareVBO, 1, 3, GL_FLOAT, 8 * sizeof(float), (void*)(3 * sizeof(float)));
	vao.LinkAttrib(squareVBO, 2, 2, GL_FLOAT, 8 * sizeof(float), (void*)(6 * sizeof(float)));
	vao.Unbind();
	squareVBO.Unbind();
	squareEBO.Unbind();

	stbi_set_flip_vertically_on_load(true);
	Texture texture("kockle.jpg", GL_RGB, &output);

	GLuint texUniform = glGetUniformLocation(shaderProgram.id, "tex0");
	GLuint scaleUniform = glGetUniformLocation(shaderProgram.id, "scale");
	GLuint viewUniform = glGetUniformLocation(shaderProgram.id, "view");
	GLuint projUniform = glGetUniformLocation(shaderProgram.id, "proj");
	shaderProgram.Activate();
	glUniform1i(texUniform, 0);

	float rotation = 0;
	glEnable(GL_DEPTH_TEST);
	double frameStartTime;
	while (!glfwWindowShouldClose(window)) {
		frameStartTime = glfwGetTime();
		glClearColor(0.5f, 0, 0, 1);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		texture.Bind();

		mat4 view = mat4(1.0f);
		mat4 proj = mat4(1.0f);
		view = translate(view, vec3(camSettings.xPos, camSettings.yPos, camSettings.zPos));
		proj = perspective(camSettings.fov, (float)width / height, 0.1f, 100.0f);
		glUniformMatrix4fv(viewUniform, 1, GL_FALSE, value_ptr(view));
		glUniformMatrix4fv(projUniform, 1, GL_FALSE, value_ptr(proj));
		glUniform1f(scaleUniform, vertScale);

		vao.Bind();
		DrawGrid(&shaderProgram);
		glfwSwapInterval(0);
		glfwSwapBuffers(window);
		glfwPollEvents();

		currFPS = 1 / (glfwGetTime() - frameStartTime);
	}


	vao.Destroy();
	squareVBO.Destroy();
	squareEBO.Destroy();
	shaderProgram.Destroy();

	return DestroyContext();
}

void DrawGrid(ShaderProgram* shaderProgram) {
	GLuint modelUniform = glGetUniformLocation(shaderProgram->id, "model");
	mat4 identity = mat4(1.0f);
	mat4 model;
	for (int x = 0; x < gridSettings.width; x++) {
		for (int y = 0; y < gridSettings.height; y++) {
			model = translate(identity, vec3(gridSettings.startX + 2 * x * vertScale, gridSettings.startY - 2 * y * vertScale, gridSettings.startZ));
			glUniformMatrix4fv(modelUniform, 1, GL_FALSE, value_ptr(model));
			glDrawElements(GL_TRIANGLES, sizeof(squareInd) / sizeof(GLuint), GL_UNSIGNED_INT, 0);
		}
	}
}

int InitContext() {
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_OPENGL_CORE_PROFILE, GL_TRUE);
	glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);
	
	window = glfwCreateWindow(width, height, "test", NULL, NULL);
	glfwSetWindowPos(window, 800, 250);

	if (window == nullptr) {
		output << "WINDOW FAIL\n";
		glfwTerminate();
		return EXIT_FAILURE;
	}

	glfwMakeContextCurrent(window);
	if (glewInit() != GLEW_OK) {
		output << "GLEW FAIL\n";
		return EXIT_FAILURE;
	}

	glViewport(0, 0, width, height);
	return EXIT_SUCCESS;
}

int DestroyContext() {
	glfwDestroyWindow(window);
	glfwTerminate();
	return EXIT_SUCCESS;
}

extern "C" __declspec(dllexport) void GetOutput(char* str, int len) {
	strcpy_s(str, len, output.str().c_str());
	output.str("");
}	

extern "C" __declspec(dllexport) void SetGridSettings(float startX, float startY, float startZ, int width, int height) {
	gridSettings = GridSettings(startX, startY, startZ, width, height);
}

extern "C" __declspec(dllexport) void SetTextureScale(float texScale) {
	vertScale = texScale;
}

extern "C" __declspec(dllexport) void SetCameraPos(float camX, float camY, float camZ, float camFOV) {
	camSettings = CameraSettings(camX, camY, camZ, camFOV);
}

extern "C" __declspec(dllexport) double GetFPS() {
	return currFPS;
}

extern "C" __declspec(dllexport) void CloseGLWindow() {
	glfwSetWindowShouldClose(window, GL_TRUE);
}