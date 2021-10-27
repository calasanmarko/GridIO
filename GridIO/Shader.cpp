#include "Shader.h"

string ReadFile(string path, stringstream* output) {
	string test = "aaaa";
	ifstream fileStream(path);
	if (fileStream.is_open()) {
		stringstream sstream;
		sstream << fileStream.rdbuf();
		string res = sstream.str();
		return res;
	}
	else {
		*output << "Error reading shader file.";
	}
}

Shader::Shader(GLenum type, string path, stringstream* output) {
	this->output = output;
	id = glCreateShader(type);
	string shaderFile = ReadFile(path, output);
	const char* charFile = shaderFile.c_str();
	glShaderSource(id, 1, &charFile, NULL);
	glCompileShader(id);
	CheckError(path);
}

void Shader::Destroy() {
	glDeleteShader(id);
}

int Shader::CheckError(string path) {
	GLint hasCompiled;
	glGetShaderiv(id, GL_COMPILE_STATUS, &hasCompiled);

	if (hasCompiled == GL_TRUE) {
		*output << "Compiled shader at " << path << endl;
		return EXIT_SUCCESS;
	}
	else {
		char* errorLog = new char[1024];
		glGetShaderInfoLog(id, 1024, NULL, errorLog);
		*output << "Error compiling shader at " << path << ": " << errorLog << endl;
		return EXIT_FAILURE;
	}
}