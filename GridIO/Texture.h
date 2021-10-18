#pragma once

#include "Output.h"
#include <gl/glew.h>
#include <GLFW/glfw3.h>
#include "stb_image.h"

class Texture
{
public:
	GLuint id;
	stringstream *output;
	Texture(const char* filename, GLenum colorFormat, stringstream* output);

	void Bind();
	void Unbind();
	void Destroy();
};