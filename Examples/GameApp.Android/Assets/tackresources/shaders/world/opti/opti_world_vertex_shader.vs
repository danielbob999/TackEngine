#version 300 es
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 fTexCoord;

uniform mat4 uModelMat;

void main()
{
    gl_Position = uModelMat * vec4(aPos, 1.0);
	
	fTexCoord = aTexCoord;
}