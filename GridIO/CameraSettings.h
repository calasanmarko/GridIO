#pragma once

#include "Color.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

using namespace glm;

class CameraSettings
{
private:
	vec3 pos;
	float fov;
	mat4 view;
	mat4 proj;
public:
	Color color;
	bool valid;

	CameraSettings();
	CameraSettings(vec3 pos, float fov, Color color);
	void SetCameraPos(vec3 pos, float fov);
	vec3 GetCameraPos();
	mat4* GetViewMat();
	mat4* GetProjMat();
	float GetFOV();
};