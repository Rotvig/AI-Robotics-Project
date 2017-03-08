using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using AI_In_Robotics.Classes;

namespace AI_In_Robotics.Utils
{
    public static class BitmapDrawing
    {
        public static BitmapSource DrawPixels(this Bitmap bitmap, IReadOnlyCollection<Particle> listOfCoordinates)
        {
            foreach (var particle in listOfCoordinates)
            {
                bitmap.SetPixel(Convert.ToInt32(particle.X), Convert.ToInt32(particle.Y), Color.Red);
            }
            return bitmap.BitmapToBitmapSource();
        }

        public static BitmapSource BitmapToBitmapSource(this Bitmap source)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}


