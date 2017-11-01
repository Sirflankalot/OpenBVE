#version 330 core

uniform mat4 world_mat;
uniform mat4 viewproj_mat;
uniform mat3 normal_mat;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 tex_coord;
layout (location = 2) in vec3 normal;

out vec2 vTexCoord;
out vec3 vNormal;

void main() {
	gl_Position = viewproj_mat * world_mat * vec4(position, 1.0);
	vTexCoord = vec2(1 - tex_coord.x, tex_coord.y);
	vNormal = normalize(normal_mat * normal);
}
