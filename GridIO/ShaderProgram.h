#pragma once

#include "Shader.h"

class ShaderProgram
{
public:
	GLuint id;
	stringstream* output;
	ShaderProgram(Shader* vertShader, Shader* fragShader, stringstream* output);

	void Activate();
	void Destroy();
private:
	int CheckError();
};