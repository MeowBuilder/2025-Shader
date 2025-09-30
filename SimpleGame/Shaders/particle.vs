#version 330

in vec3 a_Position;
in float a_Value;
in vec4 a_Color;
in vec2 a_lifeTime; // (sTime,lifeTime)
in vec3 a_Vel;
in float a_Mass;
in float a_Period;

out vec4 v_Color;

uniform float u_Time;
uniform vec3 u_Force;

const float c_PI = 3.141592;
const vec2 c_Gravity = vec2(0,-9.8);

void raining()
{
	float lifeTime = a_lifeTime.y;
	float newAlpha = 1.0f;
	vec4 newPosition = vec4(a_Position,1);

	float newTime = u_Time - (a_lifeTime.x);

	if (newTime > 0){
		float t = fract(newTime / lifeTime) * lifeTime;
		float tt = t*t;

		float forceX = u_Force.x + c_Gravity.x * a_Mass;
		float forceY = u_Force.y + c_Gravity.y * a_Mass;

		float aX = forceX / a_Mass;
		float aY = forceY / a_Mass;
		
		float x = ( (a_Vel.x) * t + 0.5 * aX * tt );
		float y = ( (a_Vel.y) * t + 0.5 * aY * tt );
	
		newPosition.xy += vec2(x,y);
		newAlpha = 1.0 - t/lifeTime;
	}
	else{
		newPosition.xy += vec2(-100000,0);
	}

	gl_Position = newPosition;
	v_Color = vec4(a_Color.rgb, newAlpha);
}

void sinParticle()
{
	vec4 centerC = vec4(1,0,0,1);
	vec4 borderC = vec4(1,1,1,0);

	float lifeTime = a_lifeTime.y;
	float newTime = u_Time - a_lifeTime.x;
	vec4 newPosition = vec4(a_Position,1);
	float newAlpha = 1.0f;
	vec4 newColor = a_Color;

	if (newTime > 0)
	{
		float period = a_Period * 10; //반복주기
		float t = fract(newTime / lifeTime) * lifeTime;
		float tt = t*t;

		float x = (2 * t - 1);
		float y = t * sin(2 * t * c_PI * period) * ((a_Value - 0.5) * 2);
		y *=  sin(fract(newTime/lifeTime) * c_PI);
		newColor = mix(centerC,borderC,abs(y*5));

		//파동
		//float x = cos(2 * a_Value * c_PI) * fract(u_Time);
		//float y = sin(2 * a_Value * c_PI) * fract(u_Time);
		//newColor = mix(centerC,borderC,sqrt(x*x + y*y));

		newPosition.xy += vec2(x,y);
		newAlpha = 1 - t/lifeTime;
	}
	else
	{
		newPosition.xy += vec2(-100000,0);
	}

	gl_Position = newPosition;

	v_Color = vec4(newColor.rgb,newAlpha);
}

void circleParticle()
{
	float newTime = u_Time - a_lifeTime.x;
	float lifeTime = a_lifeTime.y;

	float newAlpha = 1.0f;
	vec4 newPosition = vec4(a_Position,1);

	if (newTime > 0)
	{
		float t = fract(newTime / lifeTime) * lifeTime;
		float tt = t*t;

		//float x = sin(2 * a_Value * c_PI);
		//float y = cos(2 * a_Value * c_PI);
		float x = cos(2 * a_Value * c_PI) * fract(u_Time);
		float y = sin(2 * a_Value * c_PI) * fract(u_Time);

		
		float newX = x + 0.5 * c_Gravity.x * tt;
		float newY = y + 0.5 * c_Gravity.y * tt;

		newPosition.xy += vec2(newX,newY);

		newAlpha = 1 - t/lifeTime;
	}
	else
	{
		newPosition.xy += vec2(-100000,0);
	}

	gl_Position = newPosition;

	v_Color = vec4(a_Color.rgb,newAlpha);
}

void main()
{
	//raining();
	//sinParticle();
	circleParticle();
}
