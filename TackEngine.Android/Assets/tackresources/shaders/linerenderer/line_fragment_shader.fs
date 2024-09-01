#version 300 es

out mediump vec4 FragColor;

in mediump vec2 fTexCoord;

uniform mediump vec4 uColour;

void main()
{
	FragColor = uColour;
}