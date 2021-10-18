#pragma once

#include <iostream>
#include <fstream>
#include <sstream>
#include <gl/glew.h>
#include <GLFW/glfw3.h>

using namespace std;

string ReadFile(string path, stringstream* output);

class Shader
{
public:
	GLuint id;
	stringstream* output;
	Shader(GLenum type, string path, stringstream* output);

	void Destroy();
private:
	int CheckError(string path);
};