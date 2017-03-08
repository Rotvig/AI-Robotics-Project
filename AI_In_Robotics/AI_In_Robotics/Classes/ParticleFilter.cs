﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;

namespace AI_In_Robotics.Classes
{
    public class ParticleFilter
    {
        private readonly int _N; // Number of particles
        public List<Particle> ParticleSet = new List<Particle>();

        private Map World;

        public ParticleFilter(int N, Map myWorld)
        {
            World = myWorld;

            _N = N;
            GenerateParticleSet();
        }


        private void GenerateParticleSet()
        {
            for (var i = 0; i < _N; ++i)
            {
                Particle part;
                do
                {
                    part = new Particle(World.WorldSizeX, World.WorldSizeY);
                    part.weight = 1 / (double)_N;
                } while (World.IsPointInSquare(part.pos));

                ParticleSet.Add(part);
            }
        }


        public void MoveParticles(double dist)
        {
            foreach (var part in ParticleSet)
            {
                part.pos = new Point2D(part.pos.X + Math.Cos(part.theta) * dist,
                                        part.pos.Y + Math.Sin(part.theta) * dist);
                part.PositionNoise();

                // If outside world or inside landmark, destroy particle and make new random particle.
                if (part.pos.X > World.WorldSizeX || part.pos.X < 0 || part.pos.Y > World.WorldSizeY ||
                    part.pos.Y < 0 || World.IsPointInSquare(part.pos))
                {
                    var tmpPart = new Particle(World.WorldSizeX, World.WorldSizeY);
                    part.pos = tmpPart.pos;
                    part.theta = tmpPart.theta;
                }
            }
        }


        public void TurnParticlesLeft(double angleDeg)
        {
            var angleRad = (Math.PI * angleDeg / 180);
            foreach (var part in ParticleSet)
            {
                part.theta = (part.theta + angleRad) % (2 * Math.PI);
                part.ThetaNoise();
                if (part.theta < 0)
                {
                    part.theta += 2 * Math.PI;
                }
            }
        }


        public void TurnParticlesRight(double angleDeg)
        {
            var angleRad = (Math.PI * angleDeg / 180);
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
            var worldDiag = Math.Sqrt(Math.Pow(World.WorldSizeX, 2) + Math.Pow(World.WorldSizeY, 2));
            if (measurement > worldDiag)
            {
                return;
            }

            SetWeights(measurement);
            var tmpSet = new List<Particle>();
            var index = Rand.RandomInt(0, ParticleSet.Count);
            double beta = 0;
            var max = 2 * ParticleSet.Max(part => part.weight);

            for (var i = 0; i < ParticleSet.Count; ++i)
            {
                beta += Rand.RandomDouble() * max;
                while (beta > ParticleSet[index].weight)
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
            var worldDiag = Math.Sqrt(Math.Pow(World.WorldSizeX, 2) + Math.Pow(World.WorldSizeY, 2));

            foreach (var part in ParticleSet)
            {
                var dist = CalculateShortestDistance(part);
                part.weight = worldDiag - Math.Abs(measurement - dist);
            }

            var tmpSet = ParticleSet.OrderBy(part => part.weight).Reverse().ToList();
            var i = tmpSet.Count;
            foreach (var part in tmpSet)
            {
                part.weight = part.weight * i;
                i--;
            }
            ParticleSet = tmpSet;
            NormalizeWeights();
        }


        public double MeasureProb(double measurement, Particle part)
        {
            double prob = 1;
            var senseNoise = 5.0; // TODO senseNoise??
            double maxMeasure = 176; // TODO maxMeasure = approx world diagonal
            double expectedDist = World.GetDistance(part.pos, part.theta, maxMeasure);

            prob *= Math.Exp(-Math.Pow(expectedDist - measurement, 2) / Math.Pow(senseNoise, 2) / 2) /
                    Math.Sqrt(2 * Math.PI * Math.Pow(senseNoise, 2));

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
            foreach (var part in ParticleSet)
            {
                sumWeights += part.weight;
            }
            foreach (var part in ParticleSet)
            {
                part.weight = part.weight / sumWeights;
            }
        }

        private double CalculateShortestDistance(Particle part)
        {
            var tmpX = part.pos.X;
            var tmpY = part.pos.Y;
            while (!(tmpX > World.WorldSizeX)
                   && !(tmpX < 0.001)
                   && !(tmpY > World.WorldSizeY)
                   && !(tmpY < 0.001)
                   && !(World.IsPointInSquare(part.pos)))
            {
                tmpX = tmpX + Math.Cos(part.theta);
                tmpY = tmpY + Math.Sin(part.theta);
            }
            var shortestDistance = Math.Sqrt(Math.Pow(tmpX - part.pos.X, 2) + Math.Pow(tmpY - part.pos.Y, 2));
            return shortestDistance;
        }
    }
}
