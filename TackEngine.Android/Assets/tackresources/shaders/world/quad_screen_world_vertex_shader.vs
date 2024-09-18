#version 300 es
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 fTexCoord;

uniform mat4 uModelMat;
uniform int uSplitScreenCamIndex;

void main() {
    vec4 pos = uModelMat * vec4(aPos, 1.0);
	
	if (uSplitScreenCamIndex == 0){
		gl_Position = vec4((pos.x / 2.0) - 0.5, (pos.y / 2.0) + 0.5, pos.z, pos.w);
	} else if (uSplitScreenCamIndex == 1) {
		gl_Position = vec4((pos.x / 2.0) + 0.5, (pos.y / 2.0) + 0.5, pos.z, pos.w);
	} else if (uSplitScreenCamIndex == 2) {
		gl_Position = vec4((pos.x / 2.0) - 0.5, (pos.y / 2.0) - 0.5, pos.z, pos.w);
	} else{
		gl_Position = vec4((pos.x / 2.0) + 0.5, (pos.y / 2.0) - 0.5, pos.z, pos.w);
	}
	
	fTexCoord = aTexCoord;
}