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
    // Shader texture resource 
    ShaderResourceView textureView;
    // Control sampling behavior with this state
    SamplerState samplerState;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref trilistVertices);
        RemoveAndDispose(ref trilistIndices);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;

        trilistVertices = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new float[] {
            /*  Vertex Position       Vertex Color 
            new Vector4(0.25f, 0.0f, -0.5f, 1.0f),        new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Base-left
            new Vector4(0.25f, 0.5f, -0.5f, 1.0f),        new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // Top-left   
            new Vector4(0.75f, 0.0f, -0.5f, 1.0f),        new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Base-right   
            new Vector4(0.75f, 0.5f, -0.5f, 1.0f),        new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Top-right  
            */
            0.25f, 0.0f, -0.5f, 1.0f,        0.0f, 0.0f, // Base-left
            0.25f, 0.5f, -0.5f, 1.0f,        0.0f, 1.0f, // Top-left   
            0.75f, 0.0f, -0.5f, 1.0f,        1.0f, 0.0f, // Base-right   
            0.75f, 0.5f, -0.5f, 1.0f,        1.0f, 1.0f, // Top-right  

        }));
        trilistBinding = new VertexBufferBinding(trilistVertices, Utilities.SizeOf<float>() * 6, 0);
        trilistIndices = ToDispose(Buffer.Create(device, BindFlags.IndexBuffer, new ushort[] {
            0, 1, 2, // A  
            2, 1, 3  // B 
        }));

        // Load texture
        textureView = ToDispose( ShaderResourceView.FromFile(device, "data/Texture.png"));
        // Create our sampler state
        samplerState = ToDispose( new SamplerState(device, new SamplerStateDescription() {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
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

        // Set the shader resource
        context.PixelShader.SetShaderResource(0, textureView);
        // Set the sampler state
        context.PixelShader.SetSampler(0, samplerState);

        // Draw the 6 vertices of our quad
        context.DrawIndexed(6, 0, 0);
    }
}