using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace AItest
{
    public class Particle : IComparable
    {
        public Point2D pos { get; set; }
        public double weight { get; set; }
        public double theta { get; set; }


        public Particle()
        {
            pos = new Point2D(Rand.RandomDouble() * 100, Rand.RandomDouble() * 100); //TODO 100 should be map sizes of x and y
            theta = Math.PI * Rand.RandomInt(0, 360) / 180;
        }

        public void PositionNoise()
        {
            this.pos = new Point2D(pos.X + Rand.Gauss(), pos.Y + Rand.Gauss()); //TODO variance defaults to 1
        }

        public void ThetaNoise()
        {
            this.theta = (theta + Rand.Gauss(0, 0.2)) % (2*Math.PI); // TODO variance defaults to 1  3.0 / 180.0 * Math.PI
        }

        public int CompareTo(object obj)
        {
            var obj1 = obj as Particle;
            if (weight < obj1.weight)
                return -1;
            if (weight == obj1.weight)
                return 0;
            else
                return 1;
        }

        public Particle Clone()
        {
            return new Particle
            {
                pos = this.pos,
                weight = this.weight,
                theta = this.theta
            };
        }
    }
}
