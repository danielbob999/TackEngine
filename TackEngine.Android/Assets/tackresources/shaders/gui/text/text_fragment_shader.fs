#version 300 es
out mediump vec4 FragColor;

in mediump vec2 fTexCoord;

struct CameraInfo {
	mediump vec2 position;
	mediump vec2 size;
	mediump float zoomFactor;
};

struct Mask {
	mediump vec2 topRight;
	mediump vec2 bottomLeft;
};

uniform sampler2D uTexture;
uniform mediump vec4 uColour;
uniform CameraInfo uCameraInfo;

uniform Mask uMaskList[10];
uniform int uMaskCount;

bool isPointInMask(mediump vec2 point, mediump vec2 topRight, mediump vec2 bottomLeft){
	if (point.y < bottomLeft.y){
		if (point.y > topRight.y){
			return true;
		}
	}		
	
	return false;
}

void main(){
	mediump vec2 tFragCoord = vec2((gl_FragCoord.x - (uCameraInfo.size.x / 2.0)) / (uCameraInfo.size.x / 2.0), ((gl_FragCoord.y - (uCameraInfo.size.y / 2.0)) * -1.0) / (uCameraInfo.size.y / 2.0));
	
	int maskPassCount = 0;
		
	for (int i = 0; i < uMaskCount; i++){
		if (isPointInMask(tFragCoord, uMaskList[i].topRight, uMaskList[i].bottomLeft)){
			maskPassCount++;
		}
	}
		
	if (maskPassCount == uMaskCount){
		FragColor = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0);
	} else {
		FragColor = vec4(0, 0, 0, 0);
	}
}