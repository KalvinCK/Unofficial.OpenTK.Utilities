#version 460 core

layout(location = 0) in vec3 inPos;
layout(location = 1) in vec2 inTex;
layout(location = 2) in vec4 inColor;


layout(location = 0) out vec2 OutTex;
layout(location = 1) out vec4 OutColor;


void main()
{
	OutTex = inTex;
	OutColor = inColor;

	gl_Position = vec4(inPos, 1.0);
}