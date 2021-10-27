#include "CameraSettings.h"

CameraSettings::CameraSettings() {
	this->xPos = 0;
	this->yPos = 0;
	this->zPos = -5;
	this->fov = glm::radians(45.0f);
	this->color = Color();
	this->valid = true;
}

CameraSettings::CameraSettings(float xPos, float yPos, float zPos, float fov, Color color) {
	this->xPos = xPos;
	this->yPos = yPos;
	this->zPos = zPos;
	this->fov = glm::radians(fov);
	this->color = color;
	this->valid = true;
}