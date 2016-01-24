using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Common;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

public class MeshRenderer : Common.RendererBase
{
    // The vertex buffer for tri vertices
    Buffer cubeVertices;
    Buffer cubeIndices;
    // The binding structure of the axis lines vertex buffer
    VertexBufferBinding cubeBinding;
    // Shader texture resource 
    ShaderResourceView textureView;
    // Control sampling behavior with this state
    SamplerState samplerState;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref cubeVertices);
        RemoveAndDispose(ref cubeIndices);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;

        var color = Color.LightGray;
        cubeVertices = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new Vertex[] {
            new Vertex(-0.5f, 0.5f, -0.5f, color), // 0-Top-left 
            new Vertex(0.5f, 0.5f, -0.5f,  color), // 1-Top-right
            new Vertex(0.5f, -0.5f, -0.5f, color), // 2-Base-right
            new Vertex(-0.5f, -0.5f, -0.5f,color), // 3-Base-left
            new Vertex(-0.5f, 0.5f, 0.5f,  color), // 4-Topleft
            new Vertex(0.5f, 0.5f, 0.5f,   color), // 5-Top-right
            new Vertex(0.5f, -0.5f, 0.5f,  color), // 6-Base-right
            new Vertex(-0.5f, -0.5f, 0.5f, color), // 7-Base-left
        }));
        cubeBinding = new VertexBufferBinding(cubeVertices, Utilities.SizeOf<Vertex>(), 0);
        cubeIndices = ToDispose(Buffer.Create(device, BindFlags.IndexBuffer, new ushort[] {
            0, 1, 2, // Front A  
            0, 2, 3, // Front B  
            1, 5, 6, // Right A  
            1, 6, 2, // Right B 
            1, 0, 4, // Top A
            1, 4, 5, // Top B  
            5, 4, 7, // Back A 
            5, 7, 6, // Back B  
            4, 0, 3, // Left A  
            4, 3, 7, // Left B 
            3, 2, 6, // Bottom A 
            3, 6, 7, // Bottom B
        }));

        // Load texture
        textureView = ToDispose(ShaderResourceView.FromFile(device, "data/Texture.png"));
        // Create our sampler state
        samplerState = ToDispose(new SamplerState(device, new SamplerStateDescription()
        {
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
        context.InputAssembler.SetIndexBuffer(cubeIndices, Format.R16_UInt, 0);
        context.InputAssembler.SetVertexBuffers(0, cubeBinding);

        // Set the shader resource
        context.PixelShader.SetShaderResource(0, textureView);
        // Set the sampler state
        context.PixelShader.SetSampler(0, samplerState);

        // Draw the 6 vertices of our quad
        context.DrawIndexed(36, 0, 0);
    }
}