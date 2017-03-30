#version 330 core

out vec4 FragColor;

in vec2 vTexCoord;

uniform int ms_count = 4;
uniform sampler2DMS texture;

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

	FragColor = texture_value;
}
