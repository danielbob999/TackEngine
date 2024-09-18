#version 300 es

out mediump vec4 FragColor;

in mediump vec2 fTexCoord;

struct CameraInfo {
	mediump vec2 position;
	mediump vec2 size;
	mediump float zoomFactor;
};

uniform sampler2D uTexture;
uniform mediump vec4 uColour;
uniform CameraInfo uCameraInfo;

void main()
{
	FragColor = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0) * uColour.w;
}