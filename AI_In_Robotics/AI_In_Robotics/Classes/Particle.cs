﻿using System;
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

        public Particle(double worldSizeX, double worldSizeY)
        {
            pos = new Point2D(Rand.RandomDouble()* worldSizeX, Rand.RandomDouble()* worldSizeY);
            theta = Math.PI*Rand.RandomInt(0, 360)/180;
        }

        public Particle(double worldSizeX, double worldSizeY, bool rand)
        {
            pos = new Point2D(worldSizeX, worldSizeY);
            theta = Math.PI * Rand.RandomInt(0, 360) / 180;
        }

        public Particle(double newX, double newY, double newTheta)
        {
            pos = new Point2D(newX, newY);
            theta = newTheta;
        }

        public Particle() 
        {

        }

        public void PositionNoise()
        {
            pos = new Point2D(pos.X + Rand.Gauss(0, 0.5), pos.Y + Rand.Gauss(0, 0.5)); //TODO variance defaults to 1
        }

        public void ThetaNoise()
        {
            theta = (theta + Rand.Gauss(0, 0.05))%(2*Math.PI); // TODO variance set to 0.2 defaults is 1
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
