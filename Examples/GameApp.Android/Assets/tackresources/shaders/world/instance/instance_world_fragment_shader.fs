#version 440 core
out mediump vec4 FragColor;

in mediump vec2 fTexCoord;
flat in int fInstanceId;

uniform sampler2D uTextures[32];
uniform int uTextureLinks[32];
uniform mediump vec4 uColours[32];

void main()
{
	//FragColor = texture(uTextures[fragTextureIndex], fragTexCoord) * vec4(fragColour.xyz, 1.0f) * fragColour.w;
	FragColor = texture(uTextures[uTextureLinks[fInstanceId]], fTexCoord) * uColours[fInstanceId];
	//FragColor = FragColor = texture(uTextures[fInstanceId], fTexCoord) * uColours[fInstanceId];
	//FragColor = vec4(1, 0, 0, 1);
}