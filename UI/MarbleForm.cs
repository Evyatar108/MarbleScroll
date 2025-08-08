using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleScroll.UI
{
    public partial class MarbleForm : Form
    {
        private NotifyIcon trayIcon;

        public MarbleForm()
        {
            InitializeComponent();
            InitializeSystray();

            Hide();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Hide();
            base.OnLoad(e);
        }

        private void InitializeSystray()
        {
            // Load icon directly from file to avoid namespace issues
            Icon appIcon = null;
            try
            {
                appIcon = new Icon("icon.ico");
            }
            catch
            {
                // Fallback to default system icon if file not found
                appIcon = SystemIcons.Application;
            }
            
            trayIcon = new NotifyIcon(this.components)
            {
                ContextMenuStrip = contextMenuStrip1,
                Icon = appIcon,
                Text = "Scrolly",
                Visible = true
            };
        }

        private void Exit_Click(object sender, EventArgs e)
        {

        }

        private void MarbleForm_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }


    }
}
