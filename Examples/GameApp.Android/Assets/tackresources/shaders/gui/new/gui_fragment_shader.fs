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

uniform mediump vec2 uSliceDimensions;
uniform mediump vec2 uSliceBorder;
uniform bool uIsNineSlicedTexture;

uniform Mask uMaskList[10];
uniform int uMaskCount;

mediump float map(mediump float value, mediump float originalMin, mediump float originalMax, mediump float newMin, mediump float newMax) {
    return (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
}

bool isPointInMask(mediump vec2 point, mediump vec2 topRight, mediump vec2 bottomLeft){
	if (point.y < bottomLeft.y){
		if (point.y > topRight.y){
			return true;
		}
	}		
	
	return false;
}

// Helper function, because WET code is bad code
// Takes in the coordinate on the current axis and the borders 
mediump float processAxis(mediump float coord, mediump float textureBorder, mediump float windowBorder) {
    if (coord < windowBorder)
        return map(coord, 0.0, windowBorder, 0.0, textureBorder) ;
    if (coord < 1.0 - windowBorder) 
        return map(coord,  windowBorder, 1.0 - windowBorder, textureBorder, 1.0 - textureBorder);
    return map(coord, 1.0 - windowBorder, 1.0, 1.0 - textureBorder, 1.0);
} 

void main() {
	mediump vec2 tFragCoord = vec2((gl_FragCoord.x - (uCameraInfo.size.x / 2.0)) / (uCameraInfo.size.x / 2.0), ((gl_FragCoord.y - (uCameraInfo.size.y / 2.0)) * -1.0) / (uCameraInfo.size.y / 2.0));
	
	int maskPassCount = 0;
		
	for (int i = 0; i < uMaskCount; i++){
		if (isPointInMask(tFragCoord, uMaskList[i].topRight, uMaskList[i].bottomLeft)){
			maskPassCount++;
		}
	}
		
	if (maskPassCount == uMaskCount){
		if (uIsNineSlicedTexture){
			mediump vec2 newUV = vec2(
				processAxis(fTexCoord.x, uSliceBorder.x, uSliceDimensions.x),
				processAxis(fTexCoord.y, uSliceBorder.y, uSliceDimensions.y)
			);
				
			FragColor = texture(uTexture, newUV) * vec4(uColour.xyz, 1.0) * uColour.w;
		} else {
			FragColor = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0) * uColour.w;
		}
	}
}