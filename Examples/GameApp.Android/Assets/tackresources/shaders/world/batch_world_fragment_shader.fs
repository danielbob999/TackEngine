#version 330 core
out vec4 FragColor;

in vec2 fragTexCoord;
in vec4 fragColour;
flat in int fragTextureIndex;
flat in int fragBatchObjectIndex;

uniform sampler2D[16] uTextures;

void main()
{
	FragColor = texture(uTextures[fragTextureIndex], fragTexCoord) * vec4(fragColour.xyz, 1.0f) * fragColour.w;
}