using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace KeePassRPC
{
    public static class DpiFix
    {

        public static Image ScaleImageTo16x16(Image img, bool bForceNewObject)
        {
            if (img == null) { Debug.Assert(false); return null; }

            int w = img.Width;
            int h = img.Height;
            int sw = 16;
            int sh = 16;

            if ((w == sw) && (h == sh) && !bForceNewObject)
                return img;

            Bitmap bmp = new Bitmap(sw, sh, img.PixelFormat);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                RectangleF rSource = new RectangleF(0, 0, w, h);
                RectangleF rDest = new RectangleF(0, 0, sw, sh);

                // Prevent border drawing bug
                rSource.Offset(-0.5f, -0.5f);

                g.DrawImage(img, rDest, rSource, GraphicsUnit.Pixel);
            }

            return bmp;
        }
    }


}
