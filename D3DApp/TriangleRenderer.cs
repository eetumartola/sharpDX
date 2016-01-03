using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Common;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

public class TriangleRenderer : Common.RendererBase
{
    // The vertex buffer for axis lines
    Buffer triangleVertices;
    // The binding structure of the axis lines vertex buffer
    VertexBufferBinding triangleBinding;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref triangleVertices);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;
        // Create xyz-axis arrows // X is Red, Y is Green, Z is Blue
        // The arrows point along the + for each axis
        triangleVertices = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new[] {
            /*  Vertex Position       Vertex Color */
            new Vector4(0.0f, 0.0f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Base-right  
            new Vector4(-0.5f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Base-left  
            new Vector4(-0.25f, 1f, 0.25f, 1.0f),  new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // Apex 
        }));
        triangleBinding = new VertexBufferBinding(triangleVertices, Utilities.SizeOf<Vector4>() * 2, 0);
    }

    protected override void DoRender()
    {
        // Get the context reference
        var context = this.DeviceManager.Direct3DContext;
        // Render the Axis lines 
        // Tell the IA we are using lines 
        context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
        // Pass in the line vertices
        context.InputAssembler.SetVertexBuffers(0, triangleBinding);
        // Draw the 18 vertices or our xyz-axis arrows
        context.Draw(3, 0);
    }
}