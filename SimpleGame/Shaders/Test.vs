#version 330

in vec3 a_Position;
in vec4 a_Color;

out vec4 v_Color;

uniform float u_Time;

const float c_PI = 3.141592;

uniform float u_id;

void main()
{
	vec4 newPosition = vec4(a_Position,1);
	float value = fract(u_Time)*2 - 1;
	float rad = (value + 1)*c_PI;
	float y = sin(rad)*0.5f;
	float x = cos(rad)*0.5f;


	if (u_id == 0){
		newPosition.xy = newPosition.xy + fract(u_Time)*2.0f*vec2(x,y);
	}
	/*
	if (u_id == 1){
		newPosition.xy = newPosition.xy + (value+1)*0.2*vec2(x,y);
	}
	
	if (u_id == 2){
		newPosition.xy = newPosition.xy + vec2(x,y);
	}
	
	if (u_id == 3){
		newPosition.xy = newPosition.xy + vec2(x,y);
	}
	*/
	gl_Position = newPosition;

	v_Color = a_Color;
}
