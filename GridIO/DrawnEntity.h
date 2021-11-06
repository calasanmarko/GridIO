#pragma once

#include <gl/glew.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

using namespace glm;

class DrawnEntity {
public:
	int textureID;
	vec3 position;
	vec2 scale;

	DrawnEntity(int textureID);
	mat4 GetModelMat();
};