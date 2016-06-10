namespace DL_IDE
{
    partial class DL_IDE
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_code = new System.Windows.Forms.Panel();
            this.code_field = new System.Windows.Forms.RichTextBox();
            this.panel_main = new System.Windows.Forms.Panel();
            this.panel_close = new System.Windows.Forms.Label();
            this.panel_menu = new System.Windows.Forms.Panel();
            this.button_run = new System.Windows.Forms.Button();
            this.panel_code.SuspendLayout();
            this.panel_main.SuspendLayout();
            this.panel_menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_code
            // 
            this.panel_code.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panel_code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_code.Controls.Add(this.code_field);
            this.panel_code.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_code.ForeColor = System.Drawing.SystemColors.Control;
            this.panel_code.Location = new System.Drawing.Point(0, 70);
            this.panel_code.Name = "panel_code";
            this.panel_code.Size = new System.Drawing.Size(1024, 698);
            this.panel_code.TabIndex = 2;
            // 
            // code_field
            // 
            this.code_field.AcceptsTab = true;
            this.code_field.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.code_field.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.code_field.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.code_field.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.code_field.ForeColor = System.Drawing.SystemColors.Control;
            this.code_field.Location = new System.Drawing.Point(10, 10);
            this.code_field.Margin = new System.Windows.Forms.Padding(10);
            this.code_field.Name = "code_field";
            this.code_field.Size = new System.Drawing.Size(1010, 678);
            this.code_field.TabIndex = 0;
            this.code_field.Text = "";
            // 
            // panel_main
            // 
            this.panel_main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panel_main.Controls.Add(this.panel_close);
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_main.Location = new System.Drawing.Point(0, 0);
            this.panel_main.Name = "panel_main";
            this.panel_main.Size = new System.Drawing.Size(1024, 30);
            this.panel_main.TabIndex = 0;
            // 
            // panel_close
            // 
            this.panel_close.ForeColor = System.Drawing.SystemColors.Control;
            this.panel_close.Location = new System.Drawing.Point(994, 0);
            this.panel_close.Name = "panel_close";
            this.panel_close.Size = new System.Drawing.Size(30, 30);
            this.panel_close.TabIndex = 0;
            this.panel_close.Text = "X";
            this.panel_close.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panel_close.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_close_MouseDonw);
            this.panel_close.MouseEnter += new System.EventHandler(this.panel_close_MouseEnter);
            this.panel_close.MouseLeave += new System.EventHandler(this.panel_close_MouseLeave);
            // 
            // panel_menu
            // 
            this.panel_menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panel_menu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_menu.Controls.Add(this.button_run);
            this.panel_menu.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_menu.Location = new System.Drawing.Point(0, 30);
            this.panel_menu.Name = "panel_menu";
            this.panel_menu.Size = new System.Drawing.Size(1024, 40);
            this.panel_menu.TabIndex = 1;
            // 
            // button_run
            // 
            this.button_run.Cursor = System.Windows.Forms.Cursors.Default;
            this.button_run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_run.ForeColor = System.Drawing.SystemColors.Control;
            this.button_run.Location = new System.Drawing.Point(5, 5);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(70, 30);
            this.button_run.TabIndex = 0;
            this.button_run.Text = "RUN";
            this.button_run.UseVisualStyleBackColor = true;
            this.button_run.Click += new System.EventHandler(this.button_run_Click);
            // 
            // DL_IDE
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.panel_code);
            this.Controls.Add(this.panel_menu);
            this.Controls.Add(this.panel_main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DL_IDE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDE";
            this.panel_code.ResumeLayout(false);
            this.panel_main.ResumeLayout(false);
            this.panel_menu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_main;
        private System.Windows.Forms.Panel panel_menu;
        private System.Windows.Forms.Label panel_close;
        private System.Windows.Forms.Button button_run;
        private System.Windows.Forms.RichTextBox code_field;
        private System.Windows.Forms.Panel panel_code;
    }
}

