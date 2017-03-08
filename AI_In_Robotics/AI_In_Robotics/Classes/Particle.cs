using System;
using System.Windows;
using System.Windows.Media;
using MathNet.Spatial.Euclidean;
using Color = Lego.Ev3.Core.Color;

namespace AI_In_Robotics.Classes
{
    public class Particle : IComparable
    {
        public Point2D pos { get; set; }
        public double weight { get; set; }
        public double theta { get; set; }
        public double X => pos.X;
        public double Y => pos.Y;

        public Particle()
        {
            pos = new Point2D(Rand.RandomDouble()*100, Rand.RandomDouble()*100);
            //TODO 100 should be map sizes of x and y
            theta = Math.PI*Rand.RandomInt(0, 360)/180;
        }

        public void PositionNoise()
        {
            pos = new Point2D(pos.X + Rand.Gauss(), pos.Y + Rand.Gauss()); //TODO variance defaults to 1
        }

        public void ThetaNoise()
        {
            theta = (theta + Rand.Gauss(0, 0.2))%(2*Math.PI); // TODO variance defaults to 1  3.0 / 180.0 * Math.PI
        }

        public int CompareTo(object obj)
        {
            var obj1 = obj as Particle;
            if (weight < obj1.weight)
                return -1;
            if (weight == obj1.weight)
                return 0;
            return 1;
        }

        public Particle Clone()
        {
            return new Particle
            {
                pos = pos,
                weight = weight,
                theta = theta
            };
        }
    }
}
