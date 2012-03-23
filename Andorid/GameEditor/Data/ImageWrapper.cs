using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GameData;

namespace GameEditor.Data
{
    public class ImageWrapper
    {
        public static GameData.Image GetImage(string imageValue)
        {
            try
            {
                string[] cloumns = imageValue.Split(':');
                if (cloumns.Length != 2)
                    return null;

                string imagesetName = cloumns[0];
                string imageName = cloumns[1];

                Imageset imageset = ImagesetManager.Instance.Get(imagesetName);
                if (imageset == null)
                    return null;

                GameData.Image image = imageset.Get(imageName);
                if (image.Tag == null)
                {
                    System.Drawing.Image texture = System.Drawing.Image.FromFile(imageset.FileName);
                    Bitmap srcBitmap = new Bitmap(texture);

                    Rectangle rect = new Rectangle(image.X, image.Y, image.Width, image.Height);
                    image.Tag = srcBitmap.Clone(rect, srcBitmap.PixelFormat);
                }

                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
