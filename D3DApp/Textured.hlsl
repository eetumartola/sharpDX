
// Constant buffer to be updated by application per object
cbuffer PerObject : register(b0)
{
    // WorldViewProjection matrix
    float4x4 WorldViewProj;
};

// Globals for texture sampling
Texture2D ShaderTexture : register(t0);
SamplerState Sampler : register(s0);

// Vertex Shader input structure with position and color
struct VertexShaderInput
{
    float4 Position : SV_Position;
	float3 Normal : NORMAL;    // Normal - for lighting and mapping operations
	float4 Color : COLOR;     // Color - vertex color, used to generate a diffuse color
	//float2 TextureUV : TEXCOORD0;
};

// Vertex Shader output structure consisting of the
// transformed position and original color
// This is also the pixel shader input
struct VertexShaderOutput
{
    float4 Position : SV_Position;
	float3 Normal : NORMAL;    // Normal - for lighting and mapping operations
	float4 Color : COLOR;     // Color - vertex color, used to generate a diffuse color
	//float2 TextureUV : TEXCOORD0;
};

// Vertex shader main function
VertexShaderOutput VSMain(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    // Transform the position from object space to homogeneous projection space
    output.Position = mul(input.Position, WorldViewProj);
	//output.TextureUV = input.TextureUV;
	//output.TextureUV = vec2(input.Position.x, input.Position.y)
	output.Color = input.Color;
	output.Normal = input.Normal;

    return output;
}

// A simple Pixel Shader that simply passes through the interpolated color
float4 PSMain(VertexShaderOutput input) : SV_Target
{
	//return ShaderTexture.Sample(Sampler, input.TextureUV);
	return input.Color;
}