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

uniform vec2 uSliceDimensions;
uniform vec2 uSliceBorder;
uniform bool uIsNineSlicedTexture;

uniform Mask uMaskList[10];
uniform int uMaskCount;

float map(float value, float originalMin, float originalMax, float newMin, float newMax) {
    return (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
}

bool isPointInMask(vec2 point, vec2 topRight, vec2 bottomLeft){
	if (point.y < bottomLeft.y){
		if (point.y > topRight.y){
			return true;
		}
	}		
	
	return false;
}

// Helper function, because WET code is bad code
// Takes in the coordinate on the current axis and the borders 
float processAxis(float coord, float textureBorder, float windowBorder) {
    if (coord < windowBorder)
        return map(coord, 0, windowBorder, 0, textureBorder) ;
    if (coord < 1 - windowBorder) 
        return map(coord,  windowBorder, 1 - windowBorder, textureBorder, 1 - textureBorder);
    return map(coord, 1 - windowBorder, 1, 1 - textureBorder, 1);
} 

void main() {
	vec2 tFragCoord = vec2((gl_FragCoord.x - (uCameraInfo.size.x / 2.0)) / (uCameraInfo.size.x / 2.0), ((gl_FragCoord.y - (uCameraInfo.size.y / 2.0)) * -1.0) / (uCameraInfo.size.y / 2.0));
	
	int maskPassCount = 0;
		
	for (int i = 0; i < uMaskCount; i++){
		if (isPointInMask(tFragCoord, uMaskList[i].topRight, uMaskList[i].bottomLeft)){
			maskPassCount++;
		}
	}
		
	if (maskPassCount == uMaskCount){
		if (uIsNineSlicedTexture){
			vec2 newUV = vec2(
				processAxis(fTexCoord.x, uSliceBorder.x, uSliceDimensions.x),
				processAxis(fTexCoord.y, uSliceBorder.y, uSliceDimensions.y)
			);
				
			FragColor = texture2D(uTexture, newUV) * vec4(uColour.xyz, 1.0) * uColour.w;
		} else {
			FragColor = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0) * uColour.w;
		}
	}
}