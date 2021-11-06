#include "CameraSettings.h"

CameraSettings::CameraSettings() {
	SetCameraPos(vec3(0, 0, -5), radians(45.0f));
	this->color = Color();
	this->valid = true;
}

CameraSettings::CameraSettings(vec3 pos, float fov, Color color) {
	SetCameraPos(pos, fov);
	this->color = color;
	this->valid = true;
}

void CameraSettings::SetCameraPos(vec3 pos, float fov) {
	this->pos = pos;
	this->fov = fov;
	view = mat4(1.0f);
	proj = mat4(1.0f);
	view = translate(view, pos);
	proj = perspective(radians(fov), 16.0f / 9.0f, 0.1f, 100.0f);
}

vec3 CameraSettings::GetCameraPos() {
	return pos;
}

mat4* CameraSettings::GetViewMat() {
	return &view;
}

mat4* CameraSettings::GetProjMat() {
	return &proj;
}

float CameraSettings::GetFOV() {
	return fov;
}