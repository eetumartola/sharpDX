// A simple directional light (e.g. the sun)
struct DirectionalLight 
{    
	float4 Color; 
	float3 Direction;
};

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
	DirectionalLight Light;
	float3 CameraPosition;
};

cbuffer PerMaterial : register (b2) 
{ 
	float4 MaterialAmbient;  
	float4 MaterialDiffuse;  
	float4 MaterialSpecular;  
	float MaterialSpecularPower; 
	bool HasTexture; 
	float4 MaterialEmissive;  
	float4 UVTransform;
};


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

float3 Lambert(float4 pixelDiffuse, float3 normal, float3 toLight)
{
	// Calculate diffuse color (Lambert's Cosine Law - dot product of light and normal).Saturate to clamp the  value within 0 to 1.  
	
	float3 diffuseAmount = saturate(dot(normal, toLight));
	return pixelDiffuse.rgb * diffuseAmount;
}
