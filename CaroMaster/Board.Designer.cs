using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace CaroMaster
{
    partial class Board
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        private int width = 800;
        private int height = 600;

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
            this.components = new Container();
            this.ClientSize = new Size(width, height);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScaleMode = AutoScaleMode.None;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Cờ Caro";
        }

        private void InitField()
        {

        }

        #endregion
    }
}

