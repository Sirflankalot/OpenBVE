#version 330 core

uniform sampler2D model_tex;

in vec2 vTexCoord;

out vec4 FragColor;

void main () {
	FragColor = texture(model_tex, vTexCoord).rgba;
}
