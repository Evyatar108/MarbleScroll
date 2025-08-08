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
            // Use the icon from Designer resources - it's set in InitializeComponent()
            trayIcon = new NotifyIcon(this.components)
            {
                ContextMenuStrip = contextMenuStrip1,
                Icon = this.Icon, // Use the form's icon which is loaded by the Designer
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
