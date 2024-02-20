#version 440 core

out vec4 FragColor;

in vec2 fTexCoord;

struct CameraInfo {
	vec2 position;
	vec2 size;
	float zoomFactor;
};

struct Light {
	vec2 position;
	vec4 colour;
	float intensity;
	float radius;
};

struct LightingInfo {
	int lightCount;
	Light lights[30];
	vec4 ambientColour;
	float ambientColourIntensity;
};

uniform sampler2D uTexture;
uniform vec4 uColour;
uniform CameraInfo uCameraInfo;

void main()
{
	FragColor = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0) * uColour.w;
}