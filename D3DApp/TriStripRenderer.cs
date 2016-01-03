using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Common;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

public class TriStripRenderer : Common.RendererBase
{
    // The vertex buffer for axis lines
    Buffer tristripVertices;
    // The binding structure of the axis lines vertex buffer
    VertexBufferBinding tristripBinding;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref tristripVertices);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;

        tristripVertices = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new Vector4[] {
            /*  Vertex Position       Vertex Color */
            new Vector4(0.25f, 0.0f, -0.5f, 1.0f),        new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Base-left
            new Vector4(0.25f, 0.5f, -0.5f, 1.0f),        new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // Top-left   
            new Vector4(0.75f, 0.0f, -0.5f, 1.0f),        new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Base-right   
            new Vector4(0.75f, 0.5f, -0.5f, 1.0f),        new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Top-right  
        }));
        tristripBinding = new VertexBufferBinding(tristripVertices, Utilities.SizeOf<Vector4>() * 2, 0);
    }

    protected override void DoRender()
    {
        // Get the context reference
        var context = this.DeviceManager.Direct3DContext;
        // Render the tri strip
        // Tell the IA we are using a tri strip
        context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleStrip;
        // Pass in the quad vertices
        context.InputAssembler.SetVertexBuffers(0, tristripBinding);
        // Draw the 4 vertices of our quad
        context.Draw(4, 0);
    }
}