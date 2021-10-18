#include "Texture.h"

Texture::Texture(const char* filename, GLenum colorFormat, stringstream* output) {
	this->output = output;
	int widthImg, heightImg, numColCh;
	unsigned char* bytes = stbi_load(filename, &widthImg, &heightImg, &numColCh, 0);

	glGenTextures(1, &id);
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, id);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_MIRRORED_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_MIRRORED_REPEAT);

	glTexImage2D(GL_TEXTURE_2D, 0, colorFormat, widthImg, heightImg, 0, colorFormat, GL_UNSIGNED_BYTE, bytes);
	glGenerateMipmap(GL_TEXTURE_2D);
	stbi_image_free(bytes);
	glBindTexture(GL_TEXTURE_2D, 0);
}

void Texture::Bind() {
	glBindTexture(GL_TEXTURE_2D, id);
}

void Texture::Unbind() {
	glBindTexture(GL_TEXTURE_2D, 0);
}

void Texture::Destroy() {
	glDeleteTextures(1, &id);
}