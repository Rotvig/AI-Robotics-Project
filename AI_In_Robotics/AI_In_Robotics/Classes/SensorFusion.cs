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
    public class SensorFusion
    {
        public Sensor Sonar;
        public Sensor Infrared;

        //Values for Kalmann filter sensor fusion
        Matrix<double> _C;
        Matrix<double> _R;
        Matrix<double> Pk;

        //Values for calibration
        const int CalibrationRutinesToBeDone = 16;
        uint CalibrationRutineCount = 0;
        const int NumberOfCalibrationReading = 30;
        List<double> CalDsitances = new List<double>() {10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85};
        List<double> sonarTestReadings = new List<double>() {17.4, 23.5, 28, 33.3, 38.4, 43.3, 48.1, 53.6, 58.6, 63.5, 68.5, 73.7, 77.8, 83.1, 87.9, 93};
        List<double> infraredTestReadings = new List<double>() {0, 3, 9, 15, 20.5, 25, 29.1, 34.2, 39, 42.9, 46, 48.9, 51.9, 53.8, 55.5, 57};

        List<List<double>> SonarCalibateData = new List<List<double>>();
        List<List<double>> InfraredCalibateData = new List<List<double>>();
        List<List<double>> SensorFusionCalibateData = new List<List<double>>();

        public SensorFusion(Brick brick)
        {
            //Inizilize the two sensores
            Sonar = new Sensor(brick, true);
            Infrared = new Sensor(brick, false);

            //Define values for the Kalmann filter
            _C = new DenseMatrix(2, 1, new double[] { 1, 1 }); // Define how mutch each sensor contribute to the fusion result
            _R = DenseMatrix.OfArray(new double[,] { { Settings.sonarVariance, 0 }, { 0, Settings.infraredVariance } }); // Define variance of the sensor data

            //Inizilize values for the Kalmann filter
            Pk = new DenseMatrix(1, 1, new double[] { 1 });
        }

        public double Read()
        {
            double SonarSensor = Sonar.Read();
            double InfraredSensor = Infrared.Read();
            //double SonarSensor = sonarReadTest();
            //double InfraredSensor = infraredReadTest();

            if(SonarSensor < 50)
            {
                Matrix<double> xEstimate = new DenseMatrix(1, 1, new double[] { (SonarSensor + InfraredSensor) / 2 });

                Matrix<double> Zk = new DenseMatrix(2, 1, new double[] { SonarSensor, InfraredSensor });

                Matrix<double> Gk = Pk.Multiply(_C.Transpose()).Multiply(_C.Multiply(Pk).Multiply(_C.Transpose()).Add(_R).Inverse());

                xEstimate = xEstimate.Add(Gk.Multiply(Zk.Subtract(_C.Multiply(xEstimate))));

                Pk = Pk.Multiply(DenseMatrix.CreateIdentity(1).Subtract(Gk.Multiply(_C)));

                return xEstimate[0, 0];
            }
            else
            {
                return SonarSensor;
            }
        }

        public void CalibrateSensors()
        {
            List<double> SonarCalibateDataSeries = new List<double>();
            List<double> InfraredCalibateSeries = new List<double>();
            List<double> SensorFusionCalibateSeries = new List<double>();

            for (int CalibrationReading = 0; CalibrationReading < NumberOfCalibrationReading; CalibrationReading++)
            {
                SonarCalibateDataSeries.Add(Sonar.Read());
                InfraredCalibateSeries.Add(Infrared.Read());
                //SonarCalibateDataSeries.Add(sonarReadTest());
                //InfraredCalibateSeries.Add(infraredReadTest());
                SensorFusionCalibateSeries.Add(Read());
                System.Threading.Thread.Sleep(1);
            }

            SonarCalibateData.Add(SonarCalibateDataSeries);
            InfraredCalibateData.Add(InfraredCalibateSeries);
            SensorFusionCalibateData.Add(SensorFusionCalibateSeries);

            CalibrationRutineCount++;
            Console.WriteLine("Calibration cycle done");

            if (CalibrationRutineCount >= CalibrationRutinesToBeDone)
            {
                CalibrateCalc();
            }
        }

        private double sonarReadTest()
        {
            return (sonarTestReadings[(int)CalibrationRutineCount] * Settings.sonarCalibrationMultiplyer) + Settings.sonarCalibrationAddition;
        }

        private double infraredReadTest()
        {
            return (infraredTestReadings[(int)CalibrationRutineCount] * Settings.infraredCalibrationMultiplyer) + Settings.infraredCalibrationAddition;
        }

        private void CalibrateCalc()
        {
            List<double> SonarCalibateMeans = new List<double>();
            List<double> SonarCalibateVarians = new List<double>();
            List<double> SonarCalibateError = new List<double>();
            List<double> InfraredCalibateMeans = new List<double>();
            List<double> InfraredCalibateVarians = new List<double>();
            List<double> InfraredCalibateError = new List<double>();
            List<double> SensorFusionCalibateMeans = new List<double>();
            List<double> SensorFusionCalibateVarians = new List<double>();
            List<double> SensorFusionCalibateError = new List<double>();

            for (int CalibrationReading = 0; CalibrationReading < CalibrationRutinesToBeDone; CalibrationReading++)
            {
                SonarCalibateMeans.Add(Mean(SonarCalibateData[CalibrationReading], 0, NumberOfCalibrationReading));
                SonarCalibateError.Add(CalDsitances[CalibrationReading] - SonarCalibateMeans[CalibrationReading]);
                SonarCalibateVarians.Add(Variance(SonarCalibateData[CalibrationReading], SonarCalibateMeans[CalibrationReading], 0, NumberOfCalibrationReading));
                InfraredCalibateMeans.Add(Mean(InfraredCalibateData[CalibrationReading], 0, NumberOfCalibrationReading));
                InfraredCalibateError.Add(CalDsitances[CalibrationReading] - InfraredCalibateMeans[CalibrationReading]);
                InfraredCalibateVarians.Add(Variance(InfraredCalibateData[CalibrationReading], InfraredCalibateMeans[CalibrationReading], 0, NumberOfCalibrationReading));
                SensorFusionCalibateMeans.Add(Mean(SensorFusionCalibateData[CalibrationReading], 0, NumberOfCalibrationReading));
                SensorFusionCalibateError.Add(CalDsitances[CalibrationReading] - SensorFusionCalibateMeans[CalibrationReading]);
                SensorFusionCalibateVarians.Add(Variance(SensorFusionCalibateData[CalibrationReading], SensorFusionCalibateMeans[CalibrationReading], 0, NumberOfCalibrationReading));
            }

            var SonarErrorMean = Mean(SonarCalibateError, 0, CalibrationRutinesToBeDone);
            var SonarErrorVarians = Variance(SonarCalibateError, SonarErrorMean, 0, CalibrationRutinesToBeDone);
            var InfraredErrorMean = Mean(InfraredCalibateError, 0, CalibrationRutinesToBeDone);
            var InfraredErrorVarians = Variance(InfraredCalibateError, InfraredErrorMean, 0, CalibrationRutinesToBeDone);
            var SensorFusionErrorMean = Mean(SensorFusionCalibateError, 0, CalibrationRutinesToBeDone);
            var SensorFusionErrorVarians = Variance(SensorFusionCalibateError, SensorFusionErrorMean, 0, CalibrationRutinesToBeDone);
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
