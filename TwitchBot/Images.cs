using System;
using System.Drawing;
using System.Windows.Forms;

namespace TwitchBot
{
	public partial class Images : Form
	{
		public Images()
		{
			InitializeComponent();
		}

		public Image Image
		{
			get { return pictureBox1.Image; }
			set
			{
				pictureBox1.Image = value;
				Size = new Size(value.Size.Width + 16, value.Size.Height + 38);
			}
		}

		private void Images_FormClosing(object sender, FormClosingEventArgs e)
		{
			Visible = false;
			e.Cancel = true;
		}
	}
}
