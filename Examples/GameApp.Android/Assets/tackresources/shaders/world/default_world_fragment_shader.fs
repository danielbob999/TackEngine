#version 300 es

out mediump vec4 FragColor;

in mediump vec2 fTexCoord;

uniform sampler2D uTexture;
uniform mediump vec4 uColour;

void main()
{
	FragColor = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0);
}