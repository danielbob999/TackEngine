#version 440 core
out vec4 FragColor;

in vec2 fTexCoord;

struct CameraInfo {
	vec2 position;
	vec2 size;
	float zoomFactor;
};

struct Mask {
	vec2 topRight;
	vec2 bottomLeft;
};

uniform sampler2D uTexture;
uniform vec4 uColour;
uniform CameraInfo uCameraInfo;

uniform Mask uMaskList[10];
uniform int uMaskCount;

bool isPointInMask(vec2 point, vec2 topRight, vec2 bottomLeft){
	if (point.y < bottomLeft.y){
		if (point.y > topRight.y){
			return true;
		}
	}		
	
	return false;
}

void main(){
	vec2 tFragCoord = vec2((gl_FragCoord.x - (uCameraInfo.size.x / 2.0)) / (uCameraInfo.size.x / 2.0), ((gl_FragCoord.y - (uCameraInfo.size.y / 2.0)) * -1.0) / (uCameraInfo.size.y / 2.0));
	
	int maskPassCount = 0;
		
	for (int i = 0; i < uMaskCount; i++){
		if (isPointInMask(tFragCoord, uMaskList[i].topRight, uMaskList[i].bottomLeft)){
			maskPassCount++;
		}
	}
		
	if (maskPassCount == uMaskCount){
		FragColor = vec4(uColour.xyz, texture(uTexture, fTexCoord));
	} else {
		FragColor = vec4(0, 0, 0, 0);
	}
}