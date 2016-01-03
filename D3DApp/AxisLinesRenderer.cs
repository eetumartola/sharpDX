using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Common;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

public class AxisLinesRenderer : Common.RendererBase
{
    // The vertex buffer for axis lines
    Buffer axisLinesVertices;
    // The binding structure of the axis lines vertex buffer
    VertexBufferBinding axisLinesBinding;

    protected override void CreateDeviceDependentResources()
    {
        base.CreateDeviceDependentResources();
        // Ensure that if already set the device resources 
        // are correctly disposed of before recreating 
        RemoveAndDispose(ref axisLinesVertices);
        // Retrieve our SharpDX.Direct3D11.Device1 instance
        var device = this.DeviceManager.Direct3DDevice;
        // Create xyz-axis arrows // X is Red, Y is Green, Z is Blue
        // The arrows point along the + for each axis
        axisLinesVertices = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new[] {
            /*  Vertex Position       Vertex Color */
            new Vector4(-1f,0f,0f,1f),(Vector4)Color.Red,            // - x-axis
            new Vector4(1f,0f,0f,1f), (Vector4)Color.Red,            // + x-axis
            new Vector4(0.9f,-0.05f,0f,1f),(Vector4)Color.Red,            //head start
            new Vector4(1f,0f,0f,1f), (Vector4)Color.Red, new Vector4(0.9f,0.05f,0f,1f), (Vector4)Color.Red, new Vector4(1f,0f,0f,1f), (Vector4)Color.Red, // head end 
            new Vector4(0f,-1f,0f,1f), (Vector4)Color.Lime, // - y-axis
            new Vector4(0f,1f,0f,1f), (Vector4)Color.Lime,  // + y-axis 
            new Vector4(-0.05f,0.9f,0f,1f),(Vector4)Color.Lime,//head start
            new Vector4(0f,1f,0f,1f), (Vector4)Color.Lime, new Vector4(0.05f,0.9f,0f,1f), (Vector4)Color.Lime, new Vector4(0f,1f,0f,1f), (Vector4)Color.Lime,  // head end 
            new Vector4(0f,0f,-1f,1f), (Vector4)Color.Blue, // - z-axis
            new Vector4(0f,0f,1f,1f), (Vector4)Color.Blue,  // + z-axis
            new Vector4(0f,-0.05f,0.9f,1f),(Vector4)Color.Blue,//head start 
            new Vector4(0f,0f,1f,1f), (Vector4)Color.Blue, new Vector4(0f,0.05f,0.9f,1f), (Vector4)Color.Blue, new Vector4(0f,0f,1f,1f), (Vector4)Color.Blue,// head end
        }));
        axisLinesBinding = new VertexBufferBinding(axisLinesVertices, Utilities.SizeOf<Vector4>() * 2, 0);

    }

    protected override void DoRender()
    {
        // Get the context reference
        var context = this.DeviceManager.Direct3DContext;
        // Render the Axis lines 
        // Tell the IA we are using lines 
        context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;
        // Pass in the line vertices
        context.InputAssembler.SetVertexBuffers(0, axisLinesBinding);
        // Draw the 18 vertices or our xyz-axis arrows
        context.Draw(18, 0);
    }
}