#version 440 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 fTexCoord;
flat out int fInstanceId;

uniform mat4 uModelMat[32];

void main()
{
    //gl_Position = uOrthoMat * uPosMat * uRotationMat * uScaleMat * vec4(aPos, 1.0);
    gl_Position = uModelMat[gl_InstanceID] * vec4(aPos, 1.0);
	
	fTexCoord = aTexCoord;
	fInstanceId = gl_InstanceID;
}