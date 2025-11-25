#version 330

layout(location=0) out vec4 FragColor;

in vec2 v_Tex;

uniform sampler2D u_TexID;

void main()
{
	FragColor = texture(u_TexID,vec2(v_Tex.x,1-v_Tex.y));
}
