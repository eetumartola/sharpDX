
// Constant buffer to be updated by application per object
cbuffer PerObject : register(b0)
{
	// WorldViewProjection matrix 
	float4x4 WorldViewProjection;   
	// We need the world matrix so that we can 
	// calculate the lighting in world space  
	float4x4 World;    
	// Inverse transpose of world, used for 
	// bringing normals into world space, especially  
	// necessary where non-uniform scaling has been applied  
	float4x4 WorldInverseTranspose;
};

cbuffer PerFrame: register (b1) 
{ 
	float3 CameraPosition;
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
	float2 TextureUV : TEXCOORD0;
};

// Vertex Shader output structure consisting of the
// transformed position and original color
// This is also the pixel shader input
struct VertexShaderOutput // == PixelShaderInput
{
    float4 Position : SV_Position;
	float4 Color : COLOR;     // Color - vertex color, used to generate a diffuse color
	float2 TextureUV : TEXCOORD0;

	float3 WorldNormal : NORMAL;
	float3 WorldPosition : WORLDPOS;

};

// Vertex shader main function
VertexShaderOutput VSMain(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    // Transform the position from object space to homogeneous projection space
    output.Position = mul(input.Position, WorldViewProjection);
	output.TextureUV = input.TextureUV;
	//output.TextureUV = vec2(input.Position.x, input.Position.y)
	output.Color = input.Color;

	output.WorldNormal = mul(input.Normal, (float3x3)WorldInverseTranspose); 
	// transform input position to world  
	output.WorldPosition = mul(input.Position, World).xyz;

    return output;
}

// A simple Pixel Shader that simply passes through the interpolated color
float4 PSMain(VertexShaderOutput input) : SV_Target
{
	//return ShaderTexture.Sample(Sampler, input.TextureUV);
	return input.Color;
}