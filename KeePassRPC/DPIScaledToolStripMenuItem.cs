using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KeePassRPC
{
    public class DPIScaledToolStripMenuItem : ToolStripMenuItem
    {
        public IList<Image> Images { get; private set; }

        public DPIScaledToolStripMenuItem(string text) : base(text)
        {
            var images = new Image[] { Properties.Resources.KPRPC16, Properties.Resources.KPRPC64 };
            Images = images.OrderBy(i => i.Height).ToList();
            ImageScaling = ToolStripItemImageScaling.None;
            RefreshImage();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RefreshImage();
        }

        public void RefreshImage()
        {
            int h = this.Height;
            Image bestImage = null;
            for (int i = 0; i < Images.Count; i++)
            {
                var img = Images[i];
                if (img.Height > h || i == Images.Count - 1)
                {
                    bestImage = img;
                    break;
                }
            }

            // scale down the image
            Image oldImage = this.Image;
            Bitmap newImage = new Bitmap(h, h);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(bestImage, 0, 0, h, h);
            }

            this.Image = newImage;

            if (oldImage != null)
                oldImage.Dispose();
        }
    }
}
