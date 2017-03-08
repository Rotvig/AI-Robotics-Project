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
        public static BitmapSource Drawparticles(this Bitmap bitmap, IReadOnlyCollection<Particle> listOfCoordinates)
        {
            foreach (var particle in listOfCoordinates)
            {
                bitmap.SetPixel(Convert.ToInt32(particle.X), Convert.ToInt32(particle.Y), Color.Red);
            }
            return bitmap.BitmapToBitmapSource();
        }

        public static BitmapSource DrawObjects(this Bitmap bitmap, char[,] roadMap)
        {
            for (var xIndex = 0; xIndex < roadMap.GetLength(0); xIndex++)
            {
                for (var yIndex = 0; yIndex < roadMap.GetLength(1); yIndex++)
                {
                    if (roadMap[xIndex, yIndex] == 'X')
                    {
                        bitmap.SetPixel(xIndex, yIndex, Color.Black);
                    }
                }
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


