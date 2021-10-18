#include "ShaderProgram.h"

ShaderProgram::ShaderProgram(Shader* vertShader, Shader* fragShader, stringstream* output) {
	this->output = output;
	id = glCreateProgram();
	glAttachShader(id, vertShader->id);
	glAttachShader(id, fragShader->id);
	glLinkProgram(id);
	CheckError();
}

void ShaderProgram::Activate() {
	glUseProgram(id);
}

void ShaderProgram::Destroy() {
	glDeleteProgram(id);
}

int ShaderProgram::CheckError() {
	GLint hasLinked;
	glGetProgramiv(id, GL_LINK_STATUS, &hasLinked);

	if (hasLinked == GL_TRUE) {
		cout << "Linked shader program" << endl;
		return EXIT_SUCCESS;
	}
	else {
		char errorLog[1024];
		glGetProgramInfoLog(id, 1024, NULL, errorLog);
		cout << "Error linking shader program: " << errorLog << endl;
		return EXIT_FAILURE;
	}
}