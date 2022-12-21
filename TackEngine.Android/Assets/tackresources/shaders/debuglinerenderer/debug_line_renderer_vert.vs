#version 300 es
in vec3 aPos;

uniform mat4 uModelMat;

void main()
{
    gl_Position = uModelMat * vec4(aPos, 1.0);
}