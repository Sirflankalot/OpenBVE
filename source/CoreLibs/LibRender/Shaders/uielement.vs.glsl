#version 330 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 texcoord;

out vec2 vTexCoord;

uniform mat2 rotation;
uniform vec2 scale;
uniform vec2 translate;

uniform float ratio;

void main () {
	vec2 scl = scale * position;
	vec2 rot = (1 / ratio) * (rotation * scl);
	vec2 tran = rot + translate - vec2(1, -1);

	gl_Position = vec4(tran, 0.0, 1.0);
	vTexCoord = vec2(texcoord.x, 1 - texcoord.y);
}
