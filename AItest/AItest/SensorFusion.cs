using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AItest
{
    class SensorFusion
    {
        static Sensor Sonar;
        static Sensor Infrared;

        //Values for Kalmann filter sensor fusion
        Matrix<double> _C;
        Matrix<double> _R;
        Matrix<double> Pk;
        Matrix<double> xEstimate;

        public SensorFusion(Brick brick)
        {
            //Inizilize the two sensores
            Sonar = new Sensor(brick, false);
            Infrared = new Sensor(brick, true);

            //Define values for the Kalmann filter
            _C = new DenseMatrix(2, 1, new double[] { 10, 10 }); // Define how mutch each sensor contribute to the fusion result
            _R = DenseMatrix.OfArray(new double[,] { { 1, 0 }, { 0, 0.5 } }); // Define how mutch each sensor contribute to the fusion result

            //Inizilize values for the Kalmann filter
            Pk = new DenseMatrix(1, 1, new double[] { 1 });
            xEstimate = new DenseMatrix(1, 1, new double[] { 0 });
        }

        public double Read()
        {
            //double SonarSensor = Sonar.Read();
            //double InfraredSensor = Infrared.Read();
            double SonarSensor = 1;
            double InfraredSensor = 2;
            Matrix<double> Zk = new DenseMatrix(2, 1, new double[] { SonarSensor, InfraredSensor });

            Matrix<double> Gk = Pk.Multiply(_C.Transpose()).Multiply(_C.Multiply(Pk).Multiply(_C.Transpose()).Add(_R).Inverse());

            xEstimate = xEstimate.Add(Gk.Multiply(Zk.Subtract(_C.Multiply(xEstimate))));

            Pk = Pk.Multiply(DenseMatrix.CreateIdentity(1).Subtract(Gk.Multiply(_C)));

            return xEstimate[0, 0];
        }
    }
}
