using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial.Euclidean;

namespace AItest
{
    public class ParticleFilter
    {
        int _N; // Number of particles
        public List<Particle> ParticleSet = new List<Particle>();

        static List<Point2D> Landmarks = new List<Point2D>(); //TODO Get landmarks from map?

        public ParticleFilter(int N)
        {
            Landmarks.Add(new Point2D(35, 90));
            Landmarks.Add(new Point2D(35, 10));
            Landmarks.Add(new Point2D(50, 10));
            Landmarks.Add(new Point2D(50, 10));
            //Landmarks.Add(new Point2D(20, 20));
            //Landmarks.Add(new Point2D(80, 80));
            //Landmarks.Add(new Point2D(20, 80));
            //Landmarks.Add(new Point2D(80, 20));

            _N = N;
            GenerateParticleSet();
        }


        void GenerateParticleSet()
        {
            for (int i = 0; i < _N; ++i)
            {
                var particle = new Particle();
                particle.weight = 1 / (double)_N;

                ParticleSet.Add(particle);
            }
        }


        public void MoveParticles(double dist)
        {
            foreach(var part in ParticleSet)
            {
                part.pos = new Point2D(part.pos.X + Math.Cos(part.theta) * dist,
                                        part.pos.Y + Math.Sin(part.theta) * dist);
                part.PositionNoise();
            }
        }


        public void TurnParticlesLeft(double angleDeg)
        {
            double angleRad = (Math.PI * angleDeg / 180);
            foreach(var part in ParticleSet)
            {
                part.theta = (part.theta + angleRad) % (2*Math.PI);
                part.ThetaNoise();
                if (part.theta < 0)
                {
                    part.theta += 2 * Math.PI;
                }
            }
        }


        public void TurnParticlesRight(double angleDeg)
        {
            double angleRad = (Math.PI * angleDeg / 180);
            foreach (var part in ParticleSet)
            {
                part.theta = (part.theta - angleRad) % (2 * Math.PI);
                part.ThetaNoise();
                if (part.theta < 0)
                {
                    part.theta += 2 * Math.PI;
                }
            }
        }


        public void Resample(double measurement)
        {
            double worldDiag = Math.Sqrt(Math.Pow(100, 2) + Math.Pow(100, 2)); //TODO world size instead of 100
            if (measurement > worldDiag)
            {
                return;
            }

            SetWeights(measurement);
            List<Particle> tmpSet = new List<Particle>();
            int index = Rand.RandomInt(0, ParticleSet.Count);
            double beta = 0;
            double max = 2 * ParticleSet.Max(part => part.weight);
            
            for( int i = 0; i<ParticleSet.Count; ++i)
            {
                beta += Rand.RandomDouble() * max;
                while(beta > ParticleSet[index].weight)
                {
                    beta -= ParticleSet[index].weight;
                    index = (index + 1) % ParticleSet.Count;
                }
                tmpSet.Add(ParticleSet[index].Clone());
            }

            ParticleSet = tmpSet;
            NormalizeWeights();

        }


        public void SetWeights(double measurement)
        {
            double worldDiag = Math.Sqrt(Math.Pow(100, 2) + Math.Pow(100, 2)); //TODO world size instead of 100
            
            foreach(var part in ParticleSet)
            {
                double dist = CalculateShortestDistance(part);
                part.weight = worldDiag - Math.Abs(measurement - dist);
            }

            var tmpSet = ParticleSet.OrderBy(part => part.weight).Reverse().ToList();
            int i = tmpSet.Count;
            foreach(var part in tmpSet)
            {
                part.weight = part.weight * i;
                i--;
            }
            ParticleSet = tmpSet;
            NormalizeWeights();
        }


        public double MeasureProb(double measurement)
        {
            double prob = 1;
            double senseNoise = 5.0; // TODO senseNoise??
            double expectedDist = 0; // TODO get expected dist?

            prob *= Math.Exp(-Math.Pow(expectedDist - measurement, 2) / Math.Pow(senseNoise, 2) / 2) / Math.Sqrt(2 * Math.PI * Math.Pow(senseNoise, 2));

            return prob;
        }


        public void PrintParticles()
        {
            foreach (var part in ParticleSet)
            {
                Console.WriteLine("Particle: X: " + part.pos.X + "   Y: " + part.pos.Y + "   Theta: " + (part.theta));
            }
        }



        private void NormalizeWeights()
        {
            double sumWeights = 0;
            foreach(var part in ParticleSet)
            {
                sumWeights += part.weight;
            }
            foreach(var part in ParticleSet)
            {
                part.weight = part.weight / sumWeights;
            }
        }

        private double CalculateShortestDistance(Particle part)
        {
            var tmpX = part.pos.X;
            var tmpY = part.pos.Y;
            //TODO world size instead of 100 in tmpX > 100 and tmpY > 100
            //TODO Landmarks from map?
            while (!(tmpX > 100)
                   && !(tmpX < 0.01)
                   && !(tmpY > 100)
                   && !(tmpY < 0.01)
                   && !(tmpX == Landmarks[0].X)
                   && !(tmpX == Landmarks[1].X)
                   && !(tmpX == Landmarks[2].X)
                   && !(tmpX == Landmarks[3].X)
                   && !(tmpY == Landmarks[0].Y)
                   && !(tmpY == Landmarks[1].Y)
                   && !(tmpY == Landmarks[2].Y)
                   && !(tmpY == Landmarks[3].Y))
            {
                tmpX = tmpX + Math.Cos(part.theta);
                tmpY = tmpY + Math.Sin(part.theta);
            }
            var shortestDistance = Math.Sqrt(Math.Pow(tmpX - part.pos.X, 2) + Math.Pow(tmpY - part.pos.Y, 2));
            return shortestDistance;
        }


    }
}
