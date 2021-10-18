#pragma once

#include "Output.h"
#include <gl/glew.h>
#include <GLFW/glfw3.h>

class VBO
{
public:
	GLuint id;
	stringstream* output;

	VBO(GLfloat* vertices, GLsizeiptr size, stringstream* output);
	void Bind();
	void Unbind();
	void Destroy();
};