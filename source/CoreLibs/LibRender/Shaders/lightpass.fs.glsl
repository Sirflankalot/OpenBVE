#version 330 core

uniform sampler2D Normal;
uniform sampler2D AlbedoSpec;

in vec2 vTexCoord;

out vec4 FragColor;

void main () {
	//FragColor = vec4(texture(AlbedoSpec, vTexCoord).rgb, 1.0);
	FragColor = vec4(texture(Normal, vTexCoord).rgb, 1.0);
}
