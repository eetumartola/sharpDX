using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Common;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

public class MeshRenderer : Common.RendererBase
{
    // The vertex buffer for tri vertices
    Buffer meshVertexBuffer;
    Buffer meshIndexBuffer;
    // The binding structure of the axis lines vertex buffer
    VertexBufferBinding meshBinding;
    // Shader texture resource 
    ShaderResourceView textureView;
    // Control sampling behavior with this state
    SamplerState samplerState;

    HClassicGeo geo;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref meshVertexBuffer);
        RemoveAndDispose(ref meshIndexBuffer);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;

        var loader = new HClassicLoader();
        //geo = loader.Parse(@"data\pig_cd_uv.hclassic"); //point attribs only
        geo = loader.Parse(@"data\pig_tri_n_vertexuv.hclassic");   //vertex and primitive attribs

        var meshVertices = new Vertex[geo.NPoints];
        for (int i = 0; i < meshVertices.Length; i++)
        {
            /*
            uint triIndex = (uint) Math.Floor((float)i / 3.0f);
            //Vector3 point1 = geo.Points[geo.Tris[triIndex].verts[0]];
            //Vector3 point2 = geo.Points[geo.Tris[triIndex].verts[1]];
            //Vector3 point3 = geo.Points[geo.Tris[triIndex].verts[2]];
            Vector3 point1 = geo.Points[ (int) MathUtil.Mod(i+1, geo.NPoints)];
            Vector3 point2 = geo.Points[i];
            Vector3 point3 = geo.Points[(int)MathUtil.Mod(i + 2, geo.NPoints)];

            Vector3 tangent = point1 - point2;
            Vector3 bitangent = point3 - point2;
            */
            Vector3 normal = geo.PointAttributes[0].attr[i];
            //Color pointColor = new Color(geo.PointAttributes[1].attr[i]);
            //Vector2 pointUV = new Vector2(geo.PointAttributes[2].attr[i].X, 1.0f-geo.PointAttributes[2].attr[i].Y);
            Vector2 pointUV = new Vector2(geo.PointAttributes[1].attr[i].X, 1.0f - geo.PointAttributes[1].attr[i].Y);
            meshVertices[i] = new Vertex(geo.Points[i], Vector3.Normalize(normal), Color.Silver, pointUV);
            //meshVertices[i] = new Vertex(geo.Points[i], Vector3.Normalize(normal), Color.Silver, new Vector2(0.0f));
            //meshVertices[i] = new Vertex(geo.Points[i], point2, Color.Silver, new Vector2(0.0f));
        }

        meshVertexBuffer = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, meshVertices));
        meshBinding = new VertexBufferBinding(meshVertexBuffer, Utilities.SizeOf<Vertex>(), 0);

        var meshIndices = new ushort[3 * geo.NPrims];
        for (int i = 0; i < geo.NPrims; i++)
        {
            tri currTri = geo.Tris[i];
            meshIndices[3 * i + 0] = (ushort)currTri.verts[2];
            meshIndices[3 * i + 1] = (ushort)currTri.verts[1];
            meshIndices[3 * i + 2] = (ushort)currTri.verts[0]; //handedness
        }


        meshIndexBuffer = ToDispose(Buffer.Create(device, BindFlags.IndexBuffer, meshIndices));

        // Load texture
        textureView = ToDispose(ShaderResourceView.FromFile(device, "data/pig_uv.jpg"));
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
        // Pass in the mesh vertices
        context.InputAssembler.SetIndexBuffer(meshIndexBuffer, Format.R16_UInt, 0);
        context.InputAssembler.SetVertexBuffers(0, meshBinding);

        // Set the shader resource
        context.PixelShader.SetShaderResource(0, textureView);
        // Set the sampler state
        context.PixelShader.SetSampler(0, samplerState);

        // Draw the 6 vertices of our quad
        context.DrawIndexed((int)geo.NPrims *3, 0, 0);
    }
}