#pragma once

#include <glm/glm.hpp>
#include "Color.h"

struct CameraSettings
{
public:
	float xPos;
	float yPos;
	float zPos;
	float fov;
	Color color;
	bool valid;

	CameraSettings();
	CameraSettings(float xPos, float yPos, float zPos, float fov, Color color);
};