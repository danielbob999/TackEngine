#version 300 es

out mediump vec4 FragColor;
uniform mediump vec4 uColour;

void main()
{
	FragColor = vec4(uColour.xyz, 1.0) * uColour.w;
}