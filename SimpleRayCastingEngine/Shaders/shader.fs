#version 330 core

uniform vec3 wallColor;

out vec4 FragColor;

void main()
{
    FragColor = vec4(wallColor, 1.0);  // Use the wallColor uniform
}
