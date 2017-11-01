#version 330 core

uniform sampler2D model_tex;

uniform vec3 sunDir;
uniform vec3 suncolor;
uniform float sunbrightness;

in vec4 vViewPos;
in vec3 vNormal;
in vec2 vTexCoord;

out vec4 FragColor;

void main () {
	vec3 albedo = texture(model_tex, vTexCoord).rgb;
	float alpha = texture(model_tex, vTexCoord).a;
	if (alpha == 0) {
		discard;
	}
	float spec = 1.0;
	vec3 normal = vNormal;
	vec3 location = vViewPos.xyz;

	// Sun Diffuse
	float sun_factor = clamp(dot(sunDir, normal), 0, 1);
	vec3 sun_diff = suncolor * sunbrightness * sun_factor;
	
	// Sun Reflection
	vec3 viewDir = normalize(-location); // camera at vec3(0,0,0)
	vec3 halfwayDir = normalize(sunDir + viewDir);
	float sun_spec = pow(max(dot(normal, halfwayDir), 0.0), 16.0);

	// Ambient
	vec3 ambient_val = vec3(1);

	vec3 brightness = sun_diff + ambient_val;
	vec3 color = albedo * brightness;
	color += vec3(sun_spec * suncolor * sunbrightness) * 10;

	FragColor = vec4(color, alpha);
}
