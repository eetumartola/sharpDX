using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Common;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

public class TriListRenderer : Common.RendererBase
{
    // The vertex buffer for tri vertices
    Buffer trilistVertices;
    Buffer trilistIndices;
    // The binding structure of the axis lines vertex buffer
    VertexBufferBinding trilistBinding;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref trilistVertices);
        RemoveAndDispose(ref trilistIndices);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;

        trilistVertices = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new Vector4[] {
            /*  Vertex Position       Vertex Color */
            new Vector4(0.25f, 0.0f, -0.5f, 1.0f),        new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Base-left
            new Vector4(0.25f, 0.5f, -0.5f, 1.0f),        new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // Top-left   
            new Vector4(0.75f, 0.0f, -0.5f, 1.0f),        new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Base-right   
            new Vector4(0.75f, 0.5f, -0.5f, 1.0f),        new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Top-right  
        }));
        trilistBinding = new VertexBufferBinding(trilistVertices, Utilities.SizeOf<Vector4>() * 2, 0);
        trilistIndices = ToDispose(Buffer.Create(device, BindFlags.IndexBuffer, new ushort[] {
            0, 1, 2, // A  
            2, 1, 3  // B 
        }));
    }

    protected override void DoRender()
    {
        // Get the context reference
        var context = this.DeviceManager.Direct3DContext;
        // Render the tri strip
        // Tell the IA we are using a tri list
        context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
        // Pass in the quad vertices
        context.InputAssembler.SetIndexBuffer(trilistIndices, Format.R16_UInt, 0);
        context.InputAssembler.SetVertexBuffers(0, trilistBinding);
        // Draw the 6 vertices of our quad
        context.DrawIndexed(6, 0, 0);
    }
}