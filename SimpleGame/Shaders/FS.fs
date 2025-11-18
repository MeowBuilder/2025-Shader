#version 330

layout(location=0) out vec4 FragColor;

in vec2 v_UV;

uniform sampler2D u_RGBTexture;

uniform float u_Time;

const float c_PI = 3.141592;

void Test(){
	vec2 newUV = v_UV;
	float dx = sin(v_UV.y * 2 * c_PI * 4 + u_Time) * 0.1;
	float dy = sin(v_UV.x * 2 * c_PI * 4 + u_Time) * 0.2;

	newUV += vec2(dx,dy);

	vec4 sampledClor = texture(u_RGBTexture, newUV);
	FragColor = sampledClor;
}

void Circles(){
	vec2 newUV = v_UV;
	vec2 center = vec2(0.5,0.5);
	float d = distance(newUV,center);
	vec4 newColor = vec4(0);

	float value = sin(d * 4 * c_PI * 8 + u_Time * 16);
	newColor = vec4(value);

	FragColor = newColor;
}

void Flag()
{
	vec2 newUV = vec2(v_UV.x,1-v_UV.y - 0.5);
	vec4 newColor = vec4(0);

	float width = 0.25 * (1-newUV.x);
	float sinValue = 0.2 * sin(newUV.x * 2 * c_PI - u_Time * 2) * newUV.x * 1.2;
	float value = round(0.5+(sinValue + width - newUV.y));
	float value2 = round(0.5-(sinValue - width - newUV.y));

	newColor = vec4(value) * vec4(value2);


	//if (newUV.y < sinValue + width && newUV.y > sinValue - width) {
	//	newColor = vec4(1);
	//}
	//else{
	//	discard;
	//}

	FragColor = newColor;
}

void Q1()
{
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = newUV.x;
	float y = 1 - abs(2 * (newUV.y - 0.5)); // newUV.y - 0.5 => -0.5 ~ 0.5 \ * 2 => -1 ~ 1 \ abs() => 1~0~1 \ 1- => 0~1~0
	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	FragColor = newColor;
}

void Q2(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = fract(newUV.x * 3);
	float y = (2 - floor(newUV.x * 3))/3 + (newUV.y / 3);

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	FragColor = newColor;
}

void Q3(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = fract(newUV.x * 3);
	float y = (floor(newUV.x * 3))/3 + (newUV.y / 3);

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	FragColor = newColor;
}

void Q4(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = newUV.x * 2 + (0.5 * (1-round(newUV.y)));
	float y = newUV.y*2;

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	FragColor = newColor;
}

void Q5(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = newUV.x * 2;
	float y = newUV.y * 2 + (0.5 * (round(newUV.x)));

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	FragColor = newColor;
}

void main()
{
	//Test();
	//Circles();
	//Flag();
	Q5();
}
