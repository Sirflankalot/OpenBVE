#version 330 core

out vec4 FragColor;

in vec2 vTexCoord;

uniform int ms_count = 4;
uniform sampler2DMS texture;
uniform float exposure;

in vec4 gl_FragCoord;

void main() {
	vec4 texture_value = vec4(0.0);
	int count = 0;
	for (int i = 0; i < ms_count; ++i) {
		vec4 tex_read = texelFetch(texture, ivec2(gl_FragCoord.xy), i).rgba;
		texture_value += tex_read;
		count += int(tex_read.a > 0);
	}
	texture_value.rgb /= count;
	texture_value.a /= ms_count;

	vec3 mapped = vec3(1.0) - exp(-texture_value.rgb * exposure);
	mapped = pow(mapped, vec3(1.0 / 2.2));

	FragColor = vec4(mapped, 1.0);
}
