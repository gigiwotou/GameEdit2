using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameEditor.Controls
{
    public partial class DoubleBuffControl : UserControl
    {
        private Bitmap mBackgroundBitmap;
        private Graphics mBackgroundGraphics;

        public DoubleBuffControl()
        {
            InitializeComponent();
        }

        public Graphics Graphics { get { return mBackgroundGraphics; } }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (mBackgroundBitmap == null || (mBackgroundBitmap.Size != Size && Size.Width != 0 && Size.Height != 0))
            {
                if (mBackgroundBitmap != null)
                    mBackgroundBitmap.Dispose();
                mBackgroundBitmap = new Bitmap(Size.Width, Size.Height);
                mBackgroundGraphics = Graphics.FromImage(mBackgroundBitmap);

                ForceUpdate();

                Invalidate();
            }

            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // do nothing...
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // swap the background image to the panel
            if (mBackgroundBitmap != null)
                e.Graphics.DrawImage(mBackgroundBitmap, 0, 0);
        }

        public virtual void ForceUpdate()
        {
            Graphics.Clear(BackColor);
        }
    }
}
