using SharpDX.Windows;
using System.Drawing;
using System.Windows.Forms;

namespace eedx
{
    class Program
    {
        private static RenderForm _renderForm;
        private static Game _game = new Game();

        static void Main(string[] args)
        {
            InitialiseRenderForm();
            _game.Initialise(_renderForm, true);
            RenderLoop.Run(_renderForm, RenderCallback);
            _game.TearDown();
        }

        private static void InitialiseRenderForm()
        {
            _renderForm = new RenderForm(DemoConsts.kDemoTitle);
            _renderForm.ClientSize = new Size(DemoConsts.kWidth, DemoConsts.kHeight);
            _renderForm.StartPosition = FormStartPosition.CenterScreen;
        }

        private static void RenderCallback()
        {
            _game.Update();
            _game.Draw();
        }
    }
}
