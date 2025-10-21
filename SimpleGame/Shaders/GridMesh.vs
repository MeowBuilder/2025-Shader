#version 330
#define MAX_POINTS 100

in vec3 a_Position;

out vec4 v_Color;

uniform float u_Time;

uniform vec4 u_Points[MAX_POINTS];

const float c_PI = 3.141592;

void Flag(){
	vec4 newPosition = vec4(a_Position,1);

	float value = a_Position.x + 0.5;

	newPosition.y = newPosition.y * (1-value);

	float dX = 0;
	float dY = sin(2 * value * c_PI - u_Time * 4) * 0.7 * value;
	float newColor = (sin(2 * value * c_PI - u_Time * 4)+1)/2;

	newPosition += vec4(dX,dY,0,0);

	gl_Position = newPosition;

	v_Color = vec4(newColor);
}

void Wave(){
	vec4 newPosition = vec4(a_Position,1);
	float dX = 0;
	float dY = 0;

	vec2 pos = vec2(a_Position.xy);
	vec2 cen = vec2(0,0);
	float d = distance(pos,cen);
	float v = 2 * clamp(0.5 - d, 0, 1);

	float newColor = sin(d * 4 * c_PI * 10 - u_Time * 30) * v;
	
	newPosition += vec4(dX,dY,0,0);
	gl_Position = newPosition;

	v_Color = vec4(newColor);
}


void RainDrop(){
	vec4 newPosition = vec4(a_Position,1);
	float dX = 0;
	float dY = 0;

	vec2 pos = vec2(a_Position.xy);
	float newColor = 0;
	for (int i = 0;i < MAX_POINTS;i++)
	{
		float sTime = u_Points[i].z;
		float lTime = u_Points[i].w;

		float newTime = u_Time - sTime;
		if (newTime > 0)
		{
			float baseTime = fract(newTime / lTime);
			float oneMinus = 1 - baseTime;
			float t = baseTime * lTime;
			float range = baseTime * lTime / 5;

			vec2 cen = u_Points[i].xy;
			float d = distance(pos,cen);
			float v = 16 * clamp(range - d, 0, 1);

			newColor += sin(d * 4 * c_PI * 10 - t * 30) * v * oneMinus;
		}

		
	}

	newPosition += vec4(dX,dY,0,0);
	gl_Position = newPosition;

	v_Color = vec4(newColor,newColor,newColor*5,newColor);
}

void main()
{
	//Flag();
	//Wave();
	RainDrop();
}




/*if(d < 0.5){
	newColor = 1;
}
else
{
	newColor = 0;
}*/

/*float value = 0.5 - d;
newColor = ceil(value);*/