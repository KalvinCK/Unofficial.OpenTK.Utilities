#version 460
#extension GL_ARB_bindless_texture : enable

layout(location = 0) out vec4 FragColor;

layout(location = 0) in vec2 OutTex;
layout(location = 1) in vec4 OutColor;

layout(binding = 0) uniform Type_BufferConstant
{
	vec3 InterpColor;
};

layout(bindless_sampler, location = 0) uniform sampler2D BaseTexture;

void main()
{
	FragColor = texture(BaseTexture, OutTex);
	FragColor.rgb *= InterpColor;
	//FragColor = vec4(InterpColor, 1.0);
}