#pragma once

#include "VBO.h"

class VAO
{
public:
	GLuint id;
	stringstream* output;

	VAO(stringstream* output);
	void LinkAttrib(VBO vbo, GLuint layout, GLuint numComponents, GLenum type, GLsizeiptr stride, void* offset);
	void Bind();
	void Unbind();
	void Destroy();
};