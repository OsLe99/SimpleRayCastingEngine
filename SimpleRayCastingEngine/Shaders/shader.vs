#version 330 core
uniform vec3 wallColor;

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexColor;

out vec3 fragColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    fragColor = vertexColor;
    gl_Position = projection * view * model * vec4(vertexPosition, 1.0);
}
