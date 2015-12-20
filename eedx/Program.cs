using SharpDX.Windows;
using System.Drawing;
using System.Windows.Forms;

namespace eedx
{
    class Program
    {
        private static RenderForm _renderForm;

        static void Main(string[] args)
        {
            InitialiseRenderForm();
            RenderLoop.Run(_renderForm, RenderCallback);
        }

        private static void InitialiseRenderForm()
        {
            _renderForm = new RenderForm(DemoConsts.kDemoTitle);
            _renderForm.ClientSize = new Size(DemoConsts.kWidth, DemoConsts.kHeight);
            _renderForm.StartPosition = FormStartPosition.CenterScreen;
        }

        private static void RenderCallback()
        {

        }
    }
}
