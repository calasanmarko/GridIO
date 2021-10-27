#include "Textures.h"

Textures::Textures(stringstream* output) {
	this->ids = new GLuint[32];
	this->output = output;
	this->count = 0;
}

void Textures::AddTextures(int addCount, const char** paths, GLenum colorFormat) {
	if (count + addCount > 32) {
		throw exception("Cannot load more than 32 textures at one time.");
	}
	else {
		glGenTextures(addCount, ids + count);
		for (int i = 0; i < addCount; i++) {
			int currIndex = count + i;

			int widthImg, heightImg, numColCh;
			unsigned char* bytes = stbi_load(paths[i], &widthImg, &heightImg, &numColCh, 0);

			glActiveTexture(GL_TEXTURE0 + currIndex);
			glBindTexture(GL_TEXTURE_2D, ids[currIndex]);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_MIRRORED_REPEAT);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_MIRRORED_REPEAT);

			glTexImage2D(GL_TEXTURE_2D, 0, colorFormat, widthImg, heightImg, 0, colorFormat, GL_UNSIGNED_BYTE, bytes);
			glGenerateMipmap(GL_TEXTURE_2D);
			stbi_image_free(bytes);
		}

		glBindTexture(GL_TEXTURE_2D, 0);
		count += addCount;
	}
}

void Textures::Bind(int textureID) {
	glActiveTexture(GL_TEXTURE0 + textureID);
	glBindTexture(GL_TEXTURE_2D, ids[textureID]);
}

void Textures::Unbind() {
	glBindTexture(GL_TEXTURE_2D, 0);
}

void Textures::Destroy() {
	glDeleteTextures(count, ids);
}