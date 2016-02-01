#include "Common.hlsl"

// Globals for texture sampling
Texture2D ShaderTexture : register(t0);
SamplerState Sampler : register(s0);

// Vertex shader main function
VertexShaderOutput VSMain(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    // Transform the position from object space to homogeneous projection space
    output.Position = mul(input.Position, WorldViewProjection);
	output.TextureUV = input.TextureUV;
	//output.TextureUV = float2(input.Position.x, input.Position.y);
	//output.TextureUV = mul(float4(input.TextureUV.x, input.TextureUV.y, 0, 1), (float4x2)UVTransform).xy;
	output.Color = input.Color;

	output.WorldNormal = mul(input.Normal, (float3x3)WorldInverseTranspose); 
	// transform input position to world  
	output.WorldPosition = mul(input.Position, World).xyz;

    return output;
}


float4 PSMain(VertexShaderOutput input) : SV_Target
{
	float3 normal = normalize(input.WorldNormal); 
	//float3 toEye = normalize(CameraPosition – input.WorldPosition);
	float3 toEye = normalize(CameraPosition - input.WorldPosition);
	float3 toLight = normalize(-Light.Direction);

	//float4 color = float4(1.0f);

	float3 diffuse = Lambert(input.Color, normal, toLight);
	
	//float4 map = float4(1.0f, 1.0f, 1.0f, 1.0f);
	float4 map =  ShaderTexture.Sample(Sampler, input.TextureUV);
	return input.Color * float4(diffuse, 1.0f) * map;

	//return float4(input.WorldNormal, 1.0);
}