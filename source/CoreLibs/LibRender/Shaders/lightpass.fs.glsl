#version 330 core

uniform sampler2D Normal;
uniform sampler2D AlbedoSpec;
uniform sampler2D Depth;

uniform vec3 sunDir;
uniform vec3 suncolor;
uniform float sunbrightness;

uniform mat4 projInverseMatrix;

in vec2 vTexCoord;

out vec4 FragColor;

vec3 getlocation() {
	float depth = texture(Depth, vTexCoord).r;
	
	vec4 clip_space_location;
	clip_space_location.xy = vTexCoord * 2.0 - 1.0;
	clip_space_location.z = texture(Depth, vTexCoord).r * 2.0 - 1.0;
	clip_space_location.w = 1.0;

	vec4 homogenous_location = projInverseMatrix * clip_space_location;
	return homogenous_location.xyz / homogenous_location.w;	
}

void main () {
	vec3 albedo = texture(AlbedoSpec, vTexCoord).rgb;
	float spec = texture(AlbedoSpec, vTexCoord).a;
	vec3 normal = texture(Normal, vTexCoord).xyz;
	vec3 location = getlocation();

	// Sun Diffuse
	float sun_factor = clamp(dot(sunDir, normal), 0, 1);
	vec3 sun_diff = suncolor * sunbrightness * sun_factor;
	
	// Sun Reflection
	vec3 viewDir = normalize(-location); // camera at vec3(0,0,0)
	vec3 halfwayDir = normalize(sunDir + viewDir);
	float sun_spec = pow(max(dot(normal, halfwayDir), 0.0), 16.0);

	// Ambient
	vec3 ambient_val = vec3(0.04);

	vec3 brightness = sun_diff + ambient_val;
	vec3 color = albedo * brightness;
	color += vec3(sun_spec * suncolor) * 10;

	FragColor = vec4(color, 1.0);
}
