#version 330

layout(location=0) out vec4 FragColor;

in vec2 v_UV;

uniform float u_Time;

const float c_PI = 3.141592;

void main()
{
	vec4 newColor = vec4(0);

	float xValue = pow(abs(sin(v_UV.x*c_PI*2 * 1.5)),256);
	float yValue = pow(abs(cos(v_UV.y*c_PI*2 * 4)),12);

	newColor = vec4(xValue);
	newColor -= vec4(yValue);

	if (v_UV.x > 0.4 && v_UV.x < 0.6){
		newColor.b = 0;
	}

	FragColor = vec4(newColor);
}
