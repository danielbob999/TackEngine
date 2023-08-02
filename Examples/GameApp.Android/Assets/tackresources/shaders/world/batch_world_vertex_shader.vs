#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec4 aColour;
layout (location = 3) in float aTextureIndex;
layout (location = 4) in float aBatchObjectIndex;

out vec2 fragTexCoord;
out vec4 fragColour;
flat out int fragTextureIndex;
flat out int fragBatchObjectIndex;

void main()
{
    gl_Position = vec4(aPos, 1.0);
	
	fragTexCoord = aTexCoord;
	fragColour = aColour;
	fragTextureIndex = int(aTextureIndex);
	fragBatchObjectIndex = int(aBatchObjectIndex);
}