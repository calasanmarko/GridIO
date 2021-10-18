#pragma once

struct GridSettings
{
public:
	float startX;
	float startY;
	float startZ;
	int width;
	int height;
	GridSettings();
	GridSettings(float startX, float startY, float startZ, int width, int height);
};