#include "VAO.h"

VAO::VAO(stringstream* output) {
	this->output = output;
	glGenVertexArrays(1, &id);
}

void VAO::LinkAttrib(VBO vbo, GLuint layout, GLuint numComponents, GLenum type, GLsizeiptr stride, void* offset) {
	vbo.Bind();
	glVertexAttribPointer(layout, numComponents, type, GL_FALSE, stride, offset);
	glEnableVertexAttribArray(layout);
	vbo.Unbind();
}

void VAO::Bind() {
	glBindVertexArray(id);
}

void VAO::Unbind() {
	glBindVertexArray(0);
}

void VAO::Destroy() {
	glDeleteVertexArrays(1, &id);
}