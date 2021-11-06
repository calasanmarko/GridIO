#include <iostream>
#include <cstdlib>
#include <ctime>

#define STB_IMAGE_IMPLEMENTATION
#define WIN32_LEAN_AND_MEAN
#define INTEROP_TRUE 1
#define INTEROP_FALSE 0

#include <windows.h>
#include <gl/glew.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <comdef.h>
#include "Shader.h"
#include "ShaderProgram.h"
#include "Textures.h"
#include "VAO.h"
#include "VBO.h"
#include "EBO.h"
#include "GridSettings.h"
#include "CameraSettings.h"
#include "DrawnEntity.h"
#include "LinkedList.h"
#include "FBO.h"

#pragma comment(lib, "opengl32.lib")
#pragma comment(lib, "glu32.lib")

using namespace std;
using namespace glm;


GLFWwindow* window;
GridSettings gridSettings;
CameraSettings camSettings;
stringstream output;
int* fpsPointer;
bool stopFrameLoop = false;

ShaderProgram* shaderProgram;
VAO* vao;
VBO* squareVBO;
EBO* squareEBO;
FBO* framebuffer;

GLint viewport[4];
GLuint texUniform;
GLuint scaleUniform;
GLuint viewUniform;
GLuint projUniform;
GLuint modelUniform;

int width;
int height;
const int maxWidth = 2000;
const int maxHeight = 2000;

const int maxDrawnEntities = 1000;
int drawnEntityCount = 0;
List<DrawnEntity*> drawnEntities;
Textures* textures;

float lastMouseToWorldPos[2];

int* engineDoneFrame;
int* glDoneFrame;
void* pixels;
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

void DestroyContext() {
	vao->Destroy();
	squareVBO->Destroy();
	squareEBO->Destroy();
	shaderProgram->Destroy();
	glfwDestroyWindow(window);
	glfwTerminate();
}

void DrawScene() {
	if (camSettings.valid) {
		glClearColor(camSettings.color.r, camSettings.color.g, camSettings.color.b, 1);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glUniformMatrix4fv(viewUniform, 1, GL_FALSE, value_ptr(*camSettings.GetViewMat()));
		glUniformMatrix4fv(projUniform, 1, GL_FALSE, value_ptr(*camSettings.GetProjMat()));

		vao->Bind();

		Node<DrawnEntity*>* currEntityNode = drawnEntities.head;
		while (currEntityNode != nullptr) {
			DrawnEntity* currEntity = currEntityNode->value;

			textures->Bind(currEntity->textureID);
			glUniform1i(texUniform, currEntity->textureID);
			glUniformMatrix4fv(modelUniform, 1, GL_FALSE, value_ptr(currEntity->GetModelMat()));
			glUniform3fv(scaleUniform, 1, value_ptr(currEntity->scale));
			glDrawElements(GL_TRIANGLES, sizeof(squareInd) / sizeof(GLuint), GL_UNSIGNED_INT, 0);

			currEntityNode = currEntityNode->next;
		}
	}
	else {
		glClearColor(0, 0, 0, 1);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	}
}

extern "C" {
	__declspec(dllexport) void AttachContext() {
		glfwMakeContextCurrent(window);
	}

	__declspec(dllexport) void ReleaseContext() {
		glfwMakeContextCurrent(NULL);
	}

	__declspec(dllexport) void SetSize(int newWidth, int newHeight) {
		width = newWidth;
		height = newHeight;
		glViewport(0, 0, width, height);
		viewport[2] = width;
		viewport[3] = height;
	}

	__declspec(dllexport) void DrawFrame() {
		while (*engineDoneFrame == INTEROP_FALSE && !stopFrameLoop) {}
		if (stopFrameLoop) {
			return;
		}
		*engineDoneFrame = INTEROP_FALSE;
		AttachContext();
		glEnable(GL_DEPTH_TEST);
		DrawScene();
		glReadPixels(0, 0, width, height, GL_BGR, GL_UNSIGNED_BYTE, (void*)pixels);


		glfwSwapInterval(0);
		glfwSwapBuffers(window);
		glfwPollEvents();

		*glDoneFrame = INTEROP_TRUE;
		ReleaseContext();
	}

	__declspec(dllexport) void StartFrameLoop() {
		stopFrameLoop = false;
		*glDoneFrame = false;
		double startTime = 0;
		while (!glfwWindowShouldClose(window) && !stopFrameLoop) {
			startTime = glfwGetTime();
			DrawFrame();
			*fpsPointer = (int)(1.0 / (glfwGetTime() - startTime));
		}
		stopFrameLoop = false;
	}

	__declspec(dllexport) void StopFrameLoop() {
		stopFrameLoop = true;
	}

	__declspec(dllexport) void UpdatePixels(byte* pixels) {
		/*for (int i = 0; i < width * height * 4; i++) {
			*pixels = 255;
			pixels++;
		}*/
	}

	__declspec(dllexport) int InitContext(int* newEngineDoneFrame, int* newGlDoneFrame, void* newPixels, int* newFpsPointer) {
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_OPENGL_CORE_PROFILE, GL_TRUE);
		glfwWindowHint(GLFW_RESIZABLE, GL_TRUE);

		window = glfwCreateWindow(width, height, "test", NULL, NULL);
		glfwSetWindowPos(window, 800, 250);
		glfwHideWindow(window);

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

		engineDoneFrame = newEngineDoneFrame;
		glDoneFrame = newGlDoneFrame;
		pixels = newPixels;
		fpsPointer = newFpsPointer;

		Shader vertexShader(GL_VERTEX_SHADER, "default.vert", &output);
		Shader fragShader(GL_FRAGMENT_SHADER, "default.frag", &output);
		shaderProgram = new ShaderProgram(&vertexShader, &fragShader, &output);
		vertexShader.Destroy();
		fragShader.Destroy();

		vao = new VAO(&output);
		vao->Bind();

		squareVBO = new VBO(squareVert, sizeof(squareVert), &output);
		squareEBO = new EBO(squareInd, sizeof(squareInd), &output);
		vao->LinkAttrib(*squareVBO, 0, 3, GL_FLOAT, 8 * sizeof(float), (void*)0);
		vao->LinkAttrib(*squareVBO, 1, 3, GL_FLOAT, 8 * sizeof(float), (void*)(3 * sizeof(float)));
		vao->LinkAttrib(*squareVBO, 2, 2, GL_FLOAT, 8 * sizeof(float), (void*)(6 * sizeof(float)));
		vao->Unbind();
		squareVBO->Unbind();
		squareEBO->Unbind();

		framebuffer = new FBO(width, height, &output);

		stbi_set_flip_vertically_on_load(true);
		textures = new Textures(&output);
		drawnEntities = List<DrawnEntity*>();

		texUniform = glGetUniformLocation(shaderProgram->id, "tex0");
		scaleUniform = glGetUniformLocation(shaderProgram->id, "scale");
		viewUniform = glGetUniformLocation(shaderProgram->id, "view");
		projUniform = glGetUniformLocation(shaderProgram->id, "proj");
		modelUniform = glGetUniformLocation(shaderProgram->id, "model");
		shaderProgram->Activate();

		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		return EXIT_SUCCESS;
	}

	__declspec(dllexport) void GetOutput(char* str, int len) {
		strcpy_s(str, len, output.str().c_str());
		output.str("");
	}

	__declspec(dllexport) void SetGridSettings(float startX, float startY, float startZ, int width, int height) {
		gridSettings = GridSettings(startX, startY, startZ, width, height);
	}

	__declspec(dllexport) void SetCameraPos(float camX, float camY, float camZ, float camFOV) {
		camSettings = CameraSettings(vec3(camX, camY, camZ), camFOV, camSettings.color);
	}

	__declspec(dllexport) void SetValidCamera(bool valid) {
		camSettings.valid = false;
	}

	__declspec(dllexport) void SetBackgroundColor(float r, float g, float b) {
		camSettings.color = Color(r, g, b);
	}

	__declspec(dllexport) int LoadTexture(wchar_t* path, bool alphaChannel) {
		_bstr_t b(path);
		const char* pathArr = b;
		textures->AddTextures(1, &pathArr, alphaChannel ? GL_RGBA : GL_RGB);
		return textures->count - 1;
	}

	__declspec(dllexport) int CreateDrawnEntity(int textureID) {
		return drawnEntities.Add(new DrawnEntity(textureID));
	}

	__declspec(dllexport) void RemoveDrawnEntity(int drawnEntityID) {
		drawnEntities.Remove(drawnEntityID);
	}

	__declspec(dllexport) void SetDrawnEntityPosition(int entityID, float x, float y, float z) {
		drawnEntities.Get(entityID)->position = vec3(x, y, z);
	}

	__declspec(dllexport) void SetDrawnEntityScale(int entityID, float x, float y, float z) {
		drawnEntities.Get(entityID)->scale = vec3(x, y, z);
	}

	void MatToArray(mat4 mat, GLdouble* array) {
		const float* matSource = (const float*)value_ptr(mat);
			for (int i = 0; i < 16; i++) {
				array[i] = matSource[i];
			}
	}

	__declspec(dllexport) float* ScreenToWorldPos(float mouseX, float mouseY) {
		mat4 view = *camSettings.GetViewMat();
		mat4 proj = mat4(1.0f);
		proj = perspective(radians(camSettings.GetFOV()), (float)width / height, 0.1f, camSettings.GetCameraPos().z);

		GLdouble viewArr[16];
		GLdouble projArr[16];
		MatToArray(view, viewArr);
		MatToArray(proj, projArr);

		GLdouble x, y, z;
		gluUnProject(width - mouseX, height - mouseY, 1, viewArr, projArr, viewport, &x, &y, &z);
		lastMouseToWorldPos[0] = x;
		lastMouseToWorldPos[1] = y;
		return lastMouseToWorldPos;
	}


	__declspec(dllexport) void CloseGLWindow() {
		glfwSetWindowShouldClose(window, GL_TRUE);
	}
}