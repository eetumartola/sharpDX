using System.Runtime.InteropServices;
using SharpDX;

public static class ConstantBuffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DirectionalLight
    {
        public SharpDX.Color4 Color;
        public SharpDX.Vector3 Direction;
        float _padding0;
    }
   
    [StructLayout(LayoutKind.Sequential)]
    public struct PerObject
    {
        public Matrix WorldViewProjection;
        // World matrix to calculate lighting in world space  
        public Matrix World;
        // Inverse transpose of World (for normals)  
        public Matrix WorldInverseTranspose;
        // Transpose the matrices so that they are in column 
        // major order for HLSL  
        internal void Transpose()
        {
            this.World.Transpose();
            this.WorldInverseTranspose.Transpose();
            this.WorldViewProjection.Transpose();
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PerFrame
    {
        public DirectionalLight Light;
        public SharpDX.Vector3 CameraPosition;
        float _padding0;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PerMaterial
    {
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public float SpecularPower;
        public uint HasTexture; // Has texture 0 false, 1 true  
        Vector2 _padding0;
        public Color4 Emissive;
        public Matrix UVTransform; // Support UV transforms 
    }

    }