#version 400

uniform vec3 lightPosition;
uniform float lightIntensity;
uniform vec3 cameraPosition;
in vec3 fragNormal;
in vec3 fragPosition;
in vec4 fragColor;

out vec4 fragColor;

void main() {
    // Calculate lighting
    vec3 lightDir = normalize(lightPosition - fragPosition);
    float diffuse = max(dot(fragNormal, lightDir), 0.0);

    // Compute distance to light and apply attenuation
    float distToLight = length(lightPosition - fragPosition);
    float attenuation = 1.0 / (distToLight * distToLight);
    diffuse *= attenuation * lightIntensity;

    // Shadow Ray
    vec3 shadowRay = normalize(lightPosition - fragPosition);
    float shadowFactor = 1.0;

    // Check for occlusion (simple version, you could ray march here)
    if (distance(fragPosition, lightPosition) < 10.0) {
        shadowFactor = 0.5; // Simple shadow factor (could be more complex)
    }

    // Final color: Apply light and shadow
    fragColor = fragColor * (diffuse * shadowFactor);
}
