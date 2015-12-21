﻿using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using D3D11Device = SharpDX.Direct3D11.Device;

namespace eedx
{
    class GraphicsD3D11
    {
        private enum BufferingType
        {
            DoubleBuffering = 1,
            TripleBuffering = 2
        }

        private SwapChain _swapChain;
        public SwapChain SwapChain { get { return _swapChain; } }
        private D3D11Device _device;
        private DeviceContext _deviceContext;
        private RenderTargetView _renderTargetView;

        private Color _bgColor = Color.Aquamarine;

        public void Initialise(RenderForm renderForm, bool windowed)
        {
            ModeDescription modeDescription = DescribeBuffer();
            SwapChainDescription swapChainDescription = DescribeSwapChain(modeDescription, renderForm, windowed);
            CreateDevice(swapChainDescription);
            AssignDeviceContext();
            CreateRenderTargetView();
        }

        private ModeDescription DescribeBuffer()
        {
            ModeDescription desc = new ModeDescription()
            {
                Width = DemoConsts.kWidth,
                Height = DemoConsts.kHeight,
                RefreshRate = new Rational(DemoConsts.kRefreshRate, 1),
                Format = Format.R8G8B8A8_UNorm
            };
            return desc;
        }

        private SwapChainDescription DescribeSwapChain(ModeDescription modeDescription, RenderForm renderForm, bool windowed)
        {
            SwapChainDescription desc = new SwapChainDescription()
            {
                ModeDescription = modeDescription,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = (int)BufferingType.DoubleBuffering,
                OutputHandle = renderForm.Handle,
                IsWindowed = windowed
            };
            return desc;
        }

        private void CreateDevice(SwapChainDescription swapChainDescription)
        {
            D3D11Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.None,
                swapChainDescription,
                out _device,
                out _swapChain);
        }

        private void AssignDeviceContext()
        {
            _deviceContext = _device.ImmediateContext;
        }

        private void CreateRenderTargetView()
        {
            using (Texture2D backBuffer = _swapChain.GetBackBuffer<Texture2D>(0))
            {
                _renderTargetView = new RenderTargetView(_device, backBuffer);
            }
            _deviceContext.OutputMerger.SetRenderTargets(_renderTargetView);
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
            _swapChain.Present(1, PresentFlags.None);
        }
    }
}
