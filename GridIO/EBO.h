#pragma once

#include "Output.h"
#include <gl/glew.h>
#include <GLFW/glfw3.h>

class EBO
{
public:
	GLuint id;
	stringstream* output;

	EBO(GLuint* indices, GLsizeiptr size, stringstream* output);
	void Bind();
	void Unbind();
	void Destroy();
};