#version 330 core

uniform mat4 world_mat;
uniform mat4 view_mat;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 tex_coord;
layout (location = 2) in vec3 normal;

out vec2 vTexCoord;
out vec3 vNormal;

void main() {
	gl_Position = view_mat * world_mat * vec4(position, 1.0);
	vTexCoord = tex_coord;
	vNormal = normal;
}
