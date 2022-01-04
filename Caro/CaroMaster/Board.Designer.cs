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
			this.Chess = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.Chess.Location = new System.Drawing.Point(50, 50);
			this.Chess.Name = "Chess";
			this.Chess.Size = new System.Drawing.Size(500, 500);
			this.Chess.TabIndex = 0;
			this.Chess.Paint += new System.Windows.Forms.PaintEventHandler(this.Chess_Paint);
			// 
			// Board
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Controls.Add(this.Chess);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Board";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cờ Caro";
			this.ResumeLayout(false);

        }

		#endregion

		private Panel Chess;
	}
}

