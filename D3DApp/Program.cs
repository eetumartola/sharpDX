using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3DApp
{
    class Program
    {

        [STAThread]
        static void Main()
        {
            var form = new Form1();
            form.Text = "D3DDemoApp";
            form.ClientSize = new System.Drawing.Size(1024, 768);
            form.Show();

            using (var app = new D3DDemoApp(form))
            {
                // Only render frames at the maximum rate the
                // display device can handle.
                app.VSync = true;
                // Initialize the framework (creates D3D device etc)    
                // and any device dependent resources are also created.   
                app.Initialize();
                // Run the application message/rendering loop.    
                app.Run();
            }
        }
    }
}
