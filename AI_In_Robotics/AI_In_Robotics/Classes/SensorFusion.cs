using Lego.Ev3.Core;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AI_In_Robotics.Classes
{
    internal class SensorFusion
    {
        private static Sensor Sonar;
        private static Sensor Infrared;

        //Values for Kalmann filter sensor fusion
        private readonly Matrix<double> _C;
        private readonly Matrix<double> _R;
        private Matrix<double> Pk;
        private Matrix<double> xEstimate;

        public SensorFusion(Brick brick)
        {
            //Inizilize the two sensores
            Sonar = new Sensor(brick, false);
            Infrared = new Sensor(brick, true);

            //Define values for the Kalmann filter
            _C = new DenseMatrix(2, 1, new double[] {10, 10});
                // Define how mutch each sensor contribute to the fusion result
            _R = DenseMatrix.OfArray(new[,] {{1, 0}, {0, 0.5}});
                // Define how mutch each sensor contribute to the fusion result

            //Inizilize values for the Kalmann filter
            Pk = new DenseMatrix(1, 1, new double[] {1});
            xEstimate = new DenseMatrix(1, 1, new double[] {0});
        }

        public double Read()
        {
            //double SonarSensor = Sonar.Read();
            //double InfraredSensor = Infrared.Read();
            double SonarSensor = 1;
            double InfraredSensor = 2;
            Matrix<double> Zk = new DenseMatrix(2, 1, new[] {SonarSensor, InfraredSensor});

            var Gk = Pk.Multiply(_C.Transpose()).Multiply(_C.Multiply(Pk).Multiply(_C.Transpose()).Add(_R).Inverse());

            xEstimate = xEstimate.Add(Gk.Multiply(Zk.Subtract(_C.Multiply(xEstimate))));

            Pk = Pk.Multiply(DenseMatrix.CreateIdentity(1).Subtract(Gk.Multiply(_C)));

            return xEstimate[0, 0];
        }
    }
}
