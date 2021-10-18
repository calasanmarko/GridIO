#pragma once

#include <glm/glm.hpp>

struct CameraSettings
{
public:
	float xPos;
	float yPos;
	float zPos;
	float fov;

	CameraSettings();
	CameraSettings(float xPos, float yPos, float zPos, float fov);
};