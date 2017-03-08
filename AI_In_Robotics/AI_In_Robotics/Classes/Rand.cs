using System;

namespace AI_In_Robotics.Classes
{
    public static class Rand
    {
        //Function to get random number
        private static readonly Random rand = new Random();
        private static readonly object syncLock = new object();

        public static int RandomInt()
        {
            lock (syncLock)
            {
                return rand.Next();
            }
        }

        public static int RandomInt(int min, int max)
        {
            lock (syncLock)
            {
                return rand.Next(min, max);
            }
        }

        public static double RandomDouble()
        {
            lock (syncLock)
            {
                return rand.NextDouble();
            }
        }

        public static double Gauss(double mu = 0, double sigma = 1)
        {
            var u1 = 1.0 - RandomDouble(); //uniform(0,1] random doubles
            var u2 = 1.0 - RandomDouble();
            var randStdNormal = Math.Sqrt(-2.0*Math.Log(u1))*Math.Sin(2.0*Math.PI*u2); //random normal(0,1)
            return mu + sigma*randStdNormal; //random normal(mean,stdDev^2)
        }
    }
}


