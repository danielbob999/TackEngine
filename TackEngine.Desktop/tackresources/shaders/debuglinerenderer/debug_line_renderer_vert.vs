#version 440 core
layout (location = 0) in vec3 aPos;

uniform mat4 uModelMat;

void main()
{
    gl_Position = uModelMat * vec4(aPos, 1.0);
}