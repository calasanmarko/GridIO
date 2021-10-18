#include "VBO.h"

VBO::VBO(GLfloat* vertices, GLsizeiptr size, stringstream* output) {
	this->output = output;
	glGenBuffers(1, &id);
	glBindBuffer(GL_ARRAY_BUFFER, id);
	glBufferData(GL_ARRAY_BUFFER, size, vertices, GL_STATIC_DRAW);
}

void VBO::Bind() {
	glBindBuffer(GL_ARRAY_BUFFER, id);
}

void VBO::Unbind() {
	glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void VBO::Destroy() {
	glDeleteBuffers(1, &id);
}