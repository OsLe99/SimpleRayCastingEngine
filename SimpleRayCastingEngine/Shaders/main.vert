#version 400
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec4 vertexColor;
uniform mat4 mvp;
out vec3 fragNormal;
out vec3 fragPosition;
out vec4 fragColor;

void main() {
    fragNormal = normalize(vertexNormal);
    fragPosition = vertexPosition;
    fragColor = vertexColor;
    gl_Position = mvp * vec4(vertexPosition, 1.0);
}
