using System.Runtime.InteropServices;
using SharpDX;

public static class ConstantBuffers
{
    // structures defined here
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
        public SharpDX.Vector3 CameraPosition;
        float _padding0;
    }
}