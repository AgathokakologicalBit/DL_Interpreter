using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DL_IDE
{
    public partial class DL_IDE : Form
    {
        public DL_IDE()
        {
            InitializeComponent();
        }

        private void panel_close_MouseEnter(object sender, EventArgs e) => panel_close.BackColor = Color.FromArgb(50, 50, 50);
        private void panel_close_MouseLeave(object sender, EventArgs e) => panel_close.BackColor = Color.FromArgb(25, 25, 25);
        private void panel_close_MouseDonw(object sender, MouseEventArgs e) => Application.Exit();

        private void button_run_Click(object sender, EventArgs e)
        {
            File.WriteAllText("_.temp.dl", code_field.Text);
            var console = Process.Start("DL_console_interpreter.exe", "_.temp.dl");
            console.PriorityClass = ProcessPriorityClass.High;
        }
    }
}
