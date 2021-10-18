#include "GridSettings.h"

GridSettings::GridSettings() {
	this->startX = 0;
	this->startY = 0;
	this->startZ = 0;
	this->width = 0;
	this->height = 0;
}

GridSettings::GridSettings(float startX, float startY, float startZ, int width, int height) {
	this->startX = startX;
	this->startY = startY;
	this->startZ = startZ;
	this->width = width;
	this->height = height;
}