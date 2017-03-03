#version 330

uniform mat4 world_mat;
uniform mat4 view_mat;
uniform mat4 proj_mat;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 tex_coord;
layout (location = 2) in vec3 normal;

void main() {
	gl_Position = world_mat * view_mat * proj_mat * vec4(position, 1.0);
}
