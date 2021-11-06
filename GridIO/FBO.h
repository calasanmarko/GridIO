#pragma once

#include "Output.h"
#include <gl/glew.h>
#include <GLFW/glfw3.h>

class FBO
{
public:
	GLuint id;
	GLuint textureID;
	GLuint rbo;
	stringstream* output;

	FBO(int width, int height, stringstream* output);
	void Bind();
	void Unbind();
	void Destroy();
};