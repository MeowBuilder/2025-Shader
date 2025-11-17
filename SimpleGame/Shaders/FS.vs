#version 330

in vec3 a_Position;

out vec2 v_UV;

void main()
{
	vec4 newPosition = vec4(a_Position,1);
	gl_Position = newPosition;

	v_UV.x = (a_Position.x+1)/2;
	v_UV.y = (1-a_Position.y)/2;
}
