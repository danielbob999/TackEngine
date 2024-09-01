#version 440 core

out vec4 FragColor;

in vec2 fTexCoord;

uniform vec4 uColour;

void main()
{
	FragColor = uColour;
}