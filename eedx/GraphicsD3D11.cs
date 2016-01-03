using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11Device2 = SharpDX.Direct3D11.Device2;

namespace eedx
{
    class GraphicsD3D11
    {
        private enum BufferingType
        {
            DoubleBuffering = 1,
            TripleBuffering = 2
        }

        private SwapChain1 _swapChain;
        private SwapChain2 _swapChain2;
        public SwapChain1 SwapChain { get { return _swapChain; } }
        public SwapChain2 SwapChain2 { get { return _swapChain2; } }
        private D3D11Device2 _device;
        private DeviceContext2 _deviceContext;
        private RenderTargetView _renderTargetView;

        private Color _bgColor = Color.Aquamarine;

        public void Initialise(RenderForm renderForm, bool windowed)
        {
            //ModeDescription modeDescription = DescribeBuffer();
            SwapChainDescription1 swapChainDescription = DescribeSwapChain(renderForm, windowed);
            CreateDevice(swapChainDescription, renderForm);
            AssignDeviceContext();
            CreateRenderTargetView();
        }

        private SwapChainDescription1 DescribeSwapChain(RenderForm renderForm, bool windowed)
        {
            SwapChainDescription desc = new SwapChainDescription()
            {
                //ModeDescription = modeDescription,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = (int)BufferingType.DoubleBuffering,
                //OutputHandle = renderForm.Handle,
               // IsWindowed = windowed
            };
            var desc1 = new SwapChainDescription1()
            {
                Width = renderForm.ClientSize.Width,
                Height = renderForm.ClientSize.Height,
                Format = Format.R8G8B8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.BackBuffer | Usage.RenderTargetOutput,
                BufferCount = (int)BufferingType.DoubleBuffering,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.Discard,
            };

            return desc1;
        }

        private void CreateDevice(SwapChainDescription1 swapChainDescription, RenderForm renderForm)
        {
            // First create a regular D3D11 device
            using (var device11 = new D3D11Device(
             SharpDX.Direct3D.DriverType.Hardware,
             DeviceCreationFlags.Debug, // normlly .None
             new[] {
                 SharpDX.Direct3D.FeatureLevel.Level_11_1,
                 SharpDX.Direct3D.FeatureLevel.Level_11_0,
             }))
            {
                // Query device for the Device2 interface (ID3D11Device1)
                _device = device11.QueryInterfaceOrNull<D3D11Device2>();
                if (_device == null) throw new System.NotSupportedException("SharpDX.Direct3D11.Device1 is not supported");
            }

            // Rather than create a new DXGI Factory we reuse the
            // one that has been used internally to create the device
            using (var dxgi = _device.QueryInterface<SharpDX.DXGI.Device2>())
            using (var adapter = dxgi.Adapter)
            using (var factory = adapter.GetParent<Factory2>())
            {
                SwapChainDescription1 desc1 = swapChainDescription;
                _swapChain = new SwapChain1(factory,  _device, renderForm.Handle,  ref desc1,
                new SwapChainFullScreenDescription()
                {
                    RefreshRate = new Rational(60, 1),
                    Scaling = DisplayModeScaling.Centered,
                    Windowed = true
                },
                adapter.Outputs[0]); //restrict display
            }
            _swapChain2 = _swapChain.QueryInterfaceOrNull<SwapChain2>();

            _device.DebugName = "The Device";
            _swapChain.DebugName = "The 11.0 SwapChain";
            _swapChain2.DebugName = "The 11.2 SwapChain";
            //_backBuffer.DebugName = "The Backbuffer";

        }

        private void AssignDeviceContext()
        {
            _deviceContext = _device.ImmediateContext2;
        }

        private void CreateRenderTargetView()
        {
            using (Texture2D backBuffer = _swapChain.GetBackBuffer<Texture2D>(0))
            {
                _renderTargetView = new RenderTargetView(_device, backBuffer);
            }
            _deviceContext.OutputMerger.SetRenderTargets(_renderTargetView);
            _renderTargetView.DebugName = "The RenderTargetView";

        }

        public void TearDown()
        {
            _swapChain.Dispose();
            _device.Dispose();
            _deviceContext.Dispose();
            _renderTargetView.Dispose();
        }

        public void ClearRenderTargetView()
        {
            _deviceContext.ClearRenderTargetView(_renderTargetView, _bgColor);
        }

        public void PresentSwapChain()
        {
            // Output the current active Direct3D objects
            //System.Diagnostics.Debug.Write(SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());

            _swapChain2.Present(1, PresentFlags.RestrictToOutput);//, new PresentParameters());
        }
    }
}
