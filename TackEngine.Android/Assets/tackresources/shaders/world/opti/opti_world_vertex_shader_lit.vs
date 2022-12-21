#version 300 es
in vec3 aPos;
in vec2 aTexCoord;

out vec2 fTexCoord;

uniform mat4 uModelMat;

void main()
{
    gl_Position = uModelMat * vec4(aPos, 1.0);
	
	fTexCoord = aTexCoord;
}