#version 330 core
out vec4 FragColor;
  
in vec3 ourColor;
in vec2 TexCoord;

uniform sampler2D bTexture;
uniform float ourOpacity;

void main()
{
    FragColor = texture(bTexture, TexCoord) * vec4(ourColor, ourOpacity);
}