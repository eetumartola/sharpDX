using System.Runtime.InteropServices;
using SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Color Color;

    // Constructor taking position, normal and color    
    public Vertex(Vector3 position, Vector3 normal, Color color)
    {
        Position = position;
        Normal = normal;
        Color = color;
    }
    // Snip: additional constructors here 
    public Vertex(Vector3 position, Color color) : this(position, Vector3.Normalize(position), color) { }
    public Vertex(float x, float y, float z, Color color) : this( new Vector3(x, y, z), Vector3.Normalize(new Vector3(x, y, z)), color) { }
    public Vertex(Vector3 position) : this(position, Color.White) { }
}

