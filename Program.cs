namespace MarbleScroll
{
    using System;
    using System.Windows.Forms;
    using MarbleScroll.Core;
    using MarbleScroll.UI;

    static class Program
    {
        private static MarbleScrollService scrollService;

        [STAThread]
        static void Main()
        {
            scrollService = new MarbleScrollService();
            scrollService.Start();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Exit;
            
            // Create form but don't pass to Application.Run to avoid exit on form close
            var form = new MarbleForm();
            Application.Run();
        }

        static void Exit(object sender, EventArgs e)
        {
            scrollService?.Stop();
        }
    }
}
