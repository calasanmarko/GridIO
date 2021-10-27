#pragma once

#include "Output.h"
#include <gl/glew.h>
#include <GLFW/glfw3.h>
#include "stb_image.h"

class Textures
{
public:
	GLuint* ids;
	stringstream *output;
	int count = 0;
	Textures(stringstream* ouptut);

	void AddTextures(int addCount, const char** paths, GLenum colorFormat);
	void Bind(int textureID);
	void Unbind();
	void Destroy();
};