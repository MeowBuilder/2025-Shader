#version 330

layout(location=0) out vec4 FragColor;

in vec2 v_UV;

uniform sampler2D u_RGBTexture;

uniform float u_Time;

const float c_PI = 3.141592;

void main()
{
	vec2 newUV = v_UV;
	float dx = sin(v_UV.y * 2 * c_PI * 4 + u_Time) * 0.1;
	float dy = sin(v_UV.x * 2 * c_PI * 4 + u_Time) * 0.2;

	newUV += vec2(dx,dy);

	vec4 sampledClor = texture(u_RGBTexture, newUV);
	FragColor = sampledClor;
}
