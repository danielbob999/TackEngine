#version 440 core

out vec4 FragColor;

in vec2 fTexCoord;

struct CameraInfo {
	vec2 position;
	vec2 size;
	float zoomFactor;
};

struct Light {
	vec2 position;
	vec4 colour;
	float intensity;
	float radius;
};

struct LightingInfo {
	int lightCount;
	Light lights[30];
	vec4 ambientColour;
	float ambientColourIntensity;
};

uniform LightingInfo uLightingInfo;
uniform sampler2D uTexture;
uniform vec4 uColour;
uniform CameraInfo uCameraInfo;

void main()
{

	if (uLightingInfo.lightCount == 0){
		//vec4 ambientColour = vec4(uLightingInfo.ambientColour.x * uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColour.y * uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColour.z * uLightingInfo.ambientColourIntensity, 1);
		vec4 ambientColour = vec4(uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColourIntensity, 1);
		vec4 col = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0) * uColour.w;
		
		FragColor = ambientColour * col;
	} else {
		//vec4 ambientColour = vec4(uLightingInfo.ambientColour.x * uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColour.y * uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColour.z * uLightingInfo.ambientColourIntensity, 1);
		vec4 ambientColour = vec4(uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColourIntensity, uLightingInfo.ambientColourIntensity, 1);
		vec4 col = texture(uTexture, fTexCoord) * vec4(uColour.xyz, 1.0) * uColour.w;
		vec4 lightAdditive = vec4(0, 0, 0, 0);
		
		for (int i = 0; i < uLightingInfo.lightCount; i++){
			vec2 actualLightPos = (uLightingInfo.lights[i].position.xy - uCameraInfo.position) * uCameraInfo.zoomFactor;
			vec2 fragCoord = gl_FragCoord.xy + vec2((uCameraInfo.size.x / 2.0) * -1.0, (uCameraInfo.size.y / 2.0) * -1.0);
			
			float distLightToFrag = min(distance(actualLightPos, fragCoord), uLightingInfo.lights[i].radius);
		
			
			if (distLightToFrag < uLightingInfo.lights[i].radius){
				float lightAmount = (uLightingInfo.lights[i].radius - distLightToFrag) / uLightingInfo.lights[i].radius;
				
				vec4 finalAdd = vec4(uLightingInfo.lights[i].colour.xyz, lightAmount) * (lightAmount * uLightingInfo.lights[i].intensity);
				
				lightAdditive = vec4(
					min(lightAdditive.x + finalAdd.x, 1),
					min(lightAdditive.y + finalAdd.y, 1),
					min(lightAdditive.z + finalAdd.z, 1),
					min(lightAdditive.w + finalAdd.w, 1));
			}
		}
		
		ambientColour += lightAdditive;
		
		FragColor = ambientColour * col;
	}
}