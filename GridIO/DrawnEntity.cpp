#include "DrawnEntity.h"

DrawnEntity::DrawnEntity(int textureID) {
	this->textureID = textureID;
	this->position = vec3();
	this->scale = vec2(1, 1);
}

mat4 DrawnEntity::GetModelMat() {
	mat4 identity(1.0f);
	return translate(identity, position);
}