using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using Common;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace D3DApp
{ 
    public class D3DDemoApp : D3DApplicationDesktop
    {
        Stopwatch clock;
        public D3DDemoApp(System.Windows.Forms.Form window) : base(window)
        {
            clock = Stopwatch.StartNew();
        }

        public override void Run()
        {
            //var myRenderer = ToDispose(new DemoRenderer());
            //myRenderer.Initialize(this);
            
            // Create and initialize the axis lines renderer 
            var axisLines = ToDispose(new AxisLinesRenderer());
            axisLines.Initialize(this);
            // Create and initialize the triangle renderer 
            //var triangle = ToDispose(new TriangleRenderer());
            //triangle.Initialize(this);
            // Create and initialize the quad tristrip renderer
            //var tristrip = ToDispose(new TriStripRenderer());
            //tristrip.Initialize(this);
            // Create and initialize the quad trilist renderer
            //var trilist = ToDispose(new TriListRenderer());
            //trilist.Initialize(this);
            // Create and initialize the quad mesh renderer
            var mesh = ToDispose(new MeshRenderer());
            mesh.Initialize(this);
            mesh.World = Matrix.Scaling(0.5f);
            //var fps = ToDispose(new FpsRenderer());
            //fps.Initialize(this);

            // Initialize the world matrix
            var worldMatrix = Matrix.Identity;

            // Create the projection matrix // Field of View 60degrees = Pi/3 radians // Aspect ratio (based on window size), Near clip, Far clip 
            var projectionMatrix = Matrix.PerspectiveFovLH((float)Math.PI / 3f, Width / (float)Height, 0.5f, 100f);
            // Maintain the correct aspect ratio on resize
            Window.Resize += (s, e) => {  projectionMatrix = Matrix.PerspectiveFovLH( (float)Math.PI / 3f, Width / (float)Height, 0.5f, 100f); };
            float time = 0f;

            SharpDX.Windows.RenderLoop.Run(Window, () => {
                //... Render frame    
                // Clear depth stencil view
                context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth|DepthStencilClearFlags. Stencil, 1.0f, 0);
                // Clear render target view
                context.ClearRenderTargetView(RenderTargetView, Color.DarkCyan);
                time = (float)clock.ElapsedTicks / (float)Stopwatch.Frequency;
                // Set camera position slightly to the right (x), above (y) // and behind (-z
                var cameraPosition = new Vector3((float)Math.Cos(time) , 1, (float)Math.Sin(time));
                var cameraTarget = Vector3.Zero;    // Looking at origin 0,0,0
                var cameraUp = Vector3.UnitY;       // Y+ is Up
                                                    // Create view matrix from our camera pos, target and up 
                var viewMatrix = Matrix.LookAtLH(cameraPosition, cameraTarget, cameraUp);
                // Create viewProjection matrix
                var viewProjection = Matrix.Multiply(viewMatrix, projectionMatrix);
                // Create WorldViewProjection Matrix
                var worldViewProjection = worldMatrix * viewProjection;
                // HLSL defaults to "column-major" order matrices so
                // transpose first (SharpDX uses row-major matrices).
                worldViewProjection.Transpose();
                // Write the worldViewProjection to the constant buffer
                context.UpdateSubresource(ref worldViewProjection, worldViewProjectionBuffer);

                var perFrame = new ConstantBuffers.PerFrame();
                perFrame.CameraPosition = cameraPosition;
                context.UpdateSubresource(ref perFrame, perFrameBuffer);

                // Render the primitives
                axisLines.Render();
                //triangle.Render(); //tristrip.Render();
                //trilist.Render();
                worldViewProjection *= mesh.World;
                context.UpdateSubresource(ref worldViewProjection, worldViewProjectionBuffer);

                var perObject = new ConstantBuffers.PerObject();
                perObject.World = mesh.World * worldMatrix;
                perObject.WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(perObject.World));
                perObject.WorldViewProjection = perObject.World * viewProjection; perObject.Transpose();
                context.UpdateSubresource(ref perObject, perObjectBuffer);

                mesh.Render();
                // Render FPS
                //fps.Render(); // Render instructions + position changes
                //textRenderer.Render();

                //myRenderer.Render();
                Present();
            });
        }

        // The vertex shader 
        ShaderBytecode vertexShaderBytecode;
        VertexShader vertexShader;
        // The pixel shader 
        ShaderBytecode pixelShaderBytecode;
        PixelShader pixelShader;
        // The vertex layout for the IA 
        InputLayout vertexLayout;
        // A buffer that will be used to update the constant buffer
        // used by the vertex shader. This contains our worldViewProjection matrix 
        Buffer worldViewProjectionBuffer, perObjectBuffer, perFrameBuffer;
        // Our depth stencil state
        DepthStencilState depthStencilState;

        DeviceContext context;

        protected override void CreateDeviceDependentResources(DeviceManager deviceManager)
        {
            base.CreateDeviceDependentResources(deviceManager);
            // Release all resources 
            RemoveAndDispose(ref vertexShader);
            RemoveAndDispose(ref vertexShaderBytecode);
            RemoveAndDispose(ref pixelShader);
            RemoveAndDispose(ref pixelShaderBytecode);
            RemoveAndDispose(ref vertexLayout);
            RemoveAndDispose(ref worldViewProjectionBuffer);
            RemoveAndDispose(ref depthStencilState);

            // Get a reference to the Device1 instance and context 
            var device = deviceManager.Direct3DDevice;
            context = deviceManager.Direct3DContext;

            ShaderFlags shaderFlags = ShaderFlags.None;
            #if DEBUG
            shaderFlags = ShaderFlags.Debug;
            #endif
            // Compile and create the vertex shader 
            vertexShaderBytecode = ToDispose(ShaderBytecode.CompileFromFile("Textured.hlsl", "VSMain", "vs_5_0", shaderFlags)); 
            vertexShader = ToDispose(new VertexShader(device, vertexShaderBytecode));
            // Compile and create the pixel shader
            //pixelShaderBytecode = ToDispose(ShaderBytecode.CompileFromFile("Textured.hlsl", "PSMain", "ps_5_0", shaderFlags));
            //pixelShader = ToDispose(new PixelShader(device, pixelShaderBytecode));
            //HLSLCompiler lets us use #includes
            using (pixelShaderBytecode = HLSLCompiler.CompileFromFile(@"Textured.hlsl", "PSMain", "ps_5_0")) pixelShader = ToDispose(new PixelShader(device, pixelShaderBytecode));

            // Layout from VertexShader input signature
            vertexLayout = ToDispose(new InputLayout(device,
                vertexShaderBytecode.GetPart(ShaderBytecodePart.InputSignatureBlob),
                new[]{
                    new InputElement("SV_Position", 0, Format.R32G32B32_Float, 0, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                    new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 24, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0)
                }));

            // Create the buffer that will store our WVP matrix 
            worldViewProjectionBuffer = ToDispose(new Buffer(device, 
                Utilities.SizeOf<Matrix>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None, 0));

            perObjectBuffer = ToDispose(new Buffer(device, 
                Utilities.SizeOf<ConstantBuffers.PerObject>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None, 0));

            perFrameBuffer = ToDispose(new Buffer(device,
                Utilities.SizeOf<ConstantBuffers.PerFrame>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None, 0));


            // Configure the OM to discard pixels that are
            // further than the current pixel in the depth buffer.
            depthStencilState = ToDispose(new DepthStencilState(device, new DepthStencilStateDescription() {
                IsDepthEnabled = true,
                // enable depth?
                DepthComparison = Comparison.Less,
                DepthWriteMask = SharpDX.Direct3D11.DepthWriteMask.All,
                IsStencilEnabled = false,
                // enable stencil?
                StencilReadMask = 0xff,                // 0xff (no mask)   
                StencilWriteMask = 0xff,                // 0xff (no mask)   
                                                        // Configure FrontFace depth/stencil operations 
                FrontFace = new DepthStencilOperationDescription()    {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment    },
                // Configure BackFace depth/stencil operations 
                BackFace = new DepthStencilOperationDescription()    {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement
                },
            }));

            // Tell the IA what the vertices will look like
            context.InputAssembler.InputLayout = vertexLayout;
            // Bind constant buffer to vertex shader stage
            //context.VertexShader.SetConstantBuffer(0, worldViewProjectionBuffer);
            context.VertexShader.SetConstantBuffer(0, perObjectBuffer);
            context.VertexShader.SetConstantBuffer(1, perFrameBuffer);
            context.PixelShader.SetConstantBuffer(1, perFrameBuffer);
            // Set the vertex shader to run
            context.VertexShader.Set(vertexShader);
            // Set the pixel shader to run
            context.PixelShader.Set(pixelShader);
            // Set our depth stencil state
            context.OutputMerger.DepthStencilState = depthStencilState;
        }

        protected override SwapChainDescription1 CreateSwapChainDescription()
        {
            var description = base.CreateSwapChainDescription();
            description.SampleDescription.Count = 4;
            description.SampleDescription.Quality = 0;
            return description;
        }

    }
}
