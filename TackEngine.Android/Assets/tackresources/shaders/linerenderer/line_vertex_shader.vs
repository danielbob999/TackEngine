#version 300 es

in vec3 aPos;
in vec2 aTexCoord;

out vec2 fTexCoord;

uniform mat4 uModelMat;
uniform int uLineContext;

void main()
{
	if (uLineContext == 0){
		gl_Position = uModelMat * vec4(aPos, 1.0);
	} else if (uLineContext == 1){
		vec4 pos = uModelMat * vec4(aPos, 1.0);
		gl_Position = vec4(pos.x - 1.0, pos.y + 1.0, pos.z, pos.w);
	}
	
	fTexCoord = aTexCoord;
}