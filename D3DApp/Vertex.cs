using System.Runtime.InteropServices;
using SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Color Color;
    public Vector2 UV;

    // Constructor taking position, normal and color    
    public Vertex(Vector3 position, Vector3 normal, Color color, Vector2 uv)
    {
        Position = position;
        Normal = normal;
        Color = color;
        UV = uv;
    }
    // Snip: additional constructors here 
    public Vertex(Vector3 position, Color color) : this(position, Vector3.Normalize(position), color, new Vector2(0.0f, 0.0f)) { }
    public Vertex(float x, float y, float z, Color color) : this( new Vector3(x, y, z), Vector3.Normalize(new Vector3(x, y, z)), color, new Vector2(0.0f, 0.0f)) { }
    public Vertex(Vector3 position) : this(position, Color.White) { }
}

