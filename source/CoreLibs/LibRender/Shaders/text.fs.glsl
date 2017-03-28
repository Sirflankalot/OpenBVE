#version 330 core

out vec4 FragColor;
in vec2 vTexCoord;

uniform sampler2D textTexture;

uniform vec4 color;

void main() {
	vec4 mix_color = texture(textTexture, vTexCoord).rgba;
	float avg = (mix_color.r + mix_color.g + mix_color.b) / 3.0;

	//vec4 mixed = mix(backgroundColor, color, avg);
	FragColor = vec4(color.rgb, mix_color.a);
}
