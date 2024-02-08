#version 440 core

out vec4 FragColor;
uniform vec4 uColour;

void main()
{
	FragColor = vec4(uColour.xyz, 1.0) * uColour.w;
}