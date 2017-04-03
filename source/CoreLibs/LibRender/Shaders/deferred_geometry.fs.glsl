#version 330

in vec2 vTexCoord;
in vec3 vNormal;

layout(location = 0) out vec3 Normal;
layout(location = 1) out vec4 AlbedoSpec;

uniform sampler2D tex;

void main () {
	vec4 texcolor = texture(tex, vTexCoord).rgba;
	if (bool(texcolor.a < 1.0)) {
		discard;
	}
	Normal = vNormal;
	AlbedoSpec = vec4(texcolor.rgb, 1.0);
}
