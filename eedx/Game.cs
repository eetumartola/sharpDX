using SharpDX.Windows;

namespace eedx
{
    public class Game
    {
        private GraphicsD3D11 _graphics = new GraphicsD3D11();

        public void Initialise(RenderForm form, bool windowed)
        {
            _graphics.Initialise(form, windowed);
        }

        public void Update()
        {

        }

        public void Draw()
        {
            _graphics.ClearRenderTargetView();
            _graphics.PresentSwapChain();
        }

        public void TearDown()
        {
            _graphics.TearDown();
        }
    }
}