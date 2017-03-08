using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AI_In_Robotics.Classes
{
    class SensorFusion
    {
        public Sensor Sonar;
        public Sensor Infrared;

        //Values for Kalmann filter sensor fusion
        Matrix<double> _C;
        Matrix<double> _R;
        Matrix<double> Pk;
        Matrix<double> xEstimate;

        //Values for calibration
        const int CalibrationRutinesToBeDone = 4;
        uint CalibrationRutineCount = 0;
        const int NumberOfCalibrationReading = 20;
        List<double> CalDsitances = new List<double>() {10, 15, 20, 25};

        List<List<double>> SonarCalibateData = new List<List<double>>();
        List<List<double>> InfraredCalibateData = new List<List<double>>();

        public SensorFusion(Brick brick)
        {
            //Inizilize the two sensores
            Sonar = new Sensor(brick, true);
            Infrared = new Sensor(brick, false);

            //Define values for the Kalmann filter
            _C = new DenseMatrix(2, 1, new double[] { 10, 10 }); // Define how mutch each sensor contribute to the fusion result
            _R = DenseMatrix.OfArray(new double[,] { { 1, 0 }, { 0, 0.5 } }); // Define how mutch each sensor contribute to the fusion result

            //Inizilize values for the Kalmann filter
            Pk = new DenseMatrix(1, 1, new double[] { 1 });
            xEstimate = new DenseMatrix(1, 1, new double[] { 0 });
        }

        public double Read()
        {
            double SonarSensor = Sonar.Read();
            double InfraredSensor = Infrared.Read();
            //double SonarSensor = 1;
            //double InfraredSensor = 2;
            Matrix<double> Zk = new DenseMatrix(2, 1, new double[] { SonarSensor, InfraredSensor });

            Matrix<double> Gk = Pk.Multiply(_C.Transpose()).Multiply(_C.Multiply(Pk).Multiply(_C.Transpose()).Add(_R).Inverse());

            xEstimate = xEstimate.Add(Gk.Multiply(Zk.Subtract(_C.Multiply(xEstimate))));

            Pk = Pk.Multiply(DenseMatrix.CreateIdentity(1).Subtract(Gk.Multiply(_C)));

            return xEstimate[0, 0];
        }

        public void CalibrateSensors()
        {
            List<double> SonarCalibateDataSeries = new List<double>();
            List<double> InfraredCalibateSeries = new List<double>();

            for (int CalibrationReading = 0; CalibrationReading < NumberOfCalibrationReading; CalibrationReading++)
            {
                SonarCalibateDataSeries.Add(Sonar.Read());
                InfraredCalibateSeries.Add(Infrared.Read());
                System.Threading.Thread.Sleep(100);
            }

            SonarCalibateData.Add(SonarCalibateDataSeries);
            InfraredCalibateData.Add(InfraredCalibateSeries);

            CalibrationRutineCount++;
            Console.WriteLine("Calibration cycle done");

            if (CalibrationRutineCount >= CalibrationRutinesToBeDone)
            {
                CalibrateCalc();
            }
        }

        private void CalibrateCalc()
        {
            List<double> SonarCalibateMeans = new List<double>();
            List<double> SonarCalibateVarians = new List<double>();
            List<double> SonarCalibateError = new List<double>();
            List<double> InfraredCalibateMeans = new List<double>();
            List<double> InfraredCalibateVarians = new List<double>();
            List<double> InfraredCalibateError = new List<double>();

            for (int CalibrationReading = 0; CalibrationReading < CalibrationRutinesToBeDone; CalibrationReading++)
            {
                SonarCalibateMeans.Add(Mean(SonarCalibateData[CalibrationReading], 0, NumberOfCalibrationReading));
                SonarCalibateError.Add(CalDsitances[CalibrationReading] - SonarCalibateMeans[CalibrationReading]);
                SonarCalibateVarians.Add(Variance(SonarCalibateData[CalibrationReading], SonarCalibateMeans[CalibrationReading], 0, NumberOfCalibrationReading));
                InfraredCalibateMeans.Add(Mean(InfraredCalibateData[CalibrationReading], 0, NumberOfCalibrationReading));
                InfraredCalibateError.Add(CalDsitances[CalibrationReading] - InfraredCalibateMeans[CalibrationReading]);
                InfraredCalibateVarians.Add(Variance(InfraredCalibateData[CalibrationReading], InfraredCalibateMeans[CalibrationReading], 0, NumberOfCalibrationReading));
            }
        }


        public double Mean(List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public double Variance(List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }
    }
}
