#version 330

uniform vec3 wallColor;

out vec4 fragColor;

void main()
{
    fragColor = vec4(wallColor, 1.0);
}
