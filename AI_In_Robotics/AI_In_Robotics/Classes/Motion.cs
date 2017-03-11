using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using System.Drawing;
using AI_In_Robotics.Utils;

namespace AI_In_Robotics.Classes
{
    public class Motion
    {
        private readonly Brick _brick;
        private const uint MotorMoveTimeMs = 250;
        private const double MoveDistancePerCicle = 2.05;
        private const uint RotaionDegStep = 360 / 4;
        private readonly Queue<Action> Commands = new Queue<Action>();

        public Motion(Brick brick)
        {
            _brick = brick;
            _brick.BrickChanged += _brick_BrickChanged;

            _brick.Ports[Settings.gyroPort].SetMode(GyroscopeMode.Calibrate);
            Thread.Sleep(2000);
            _brick.Ports[Settings.gyroPort].SetMode(GyroscopeMode.Angle);
        }

        private double getGyro()
        {
            return _brick.Ports[Settings.gyroPort].SIValue;
        }

        public void motionTest()
        {
            //Console.WriteLine(getGyro());
            PIDMove(20);
            //PIDTurn(90);
        }

        private void PIDMove(double moveDistanceCm)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter("D:MotorPIDtest.txt");
            double moveCiclesCount = 0;
            double kp = 0.8;
            double Ki = 0.12;
            double Kd = 0.06;
            double gyroOffset = getGyro();
            double Tp = 25;
            double integral = 0;
            double lastError = 0;
            double derivative = 0;

            while (moveCiclesCount < moveDistanceCm/MoveDistancePerCicle)
            {
                double gyroValue = getGyro();
                double error = gyroValue - gyroOffset;
                integral = integral + error;
                derivative = error - lastError;
                double turn = kp * error + Ki * integral + Kd * derivative;
                double powerRight = Tp - turn;
                double powerLeft = Tp + turn;

                moveMoters(powerRight, powerLeft);

                lastError = error;
                moveCiclesCount++;
            }
        }

        private void PIDTurn(double rotateDeg)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter("D:MotorPIDtest.txt");
            double moveCiclesCount = 0;
            double kp = 0.4;
            double Ki = 0.125;
            double Kd = 0.083;
            double gyroOffset = getGyro() + rotateDeg;
            double Tp = 0;
            double integral = 0;
            double error = rotateDeg;
            double lastError = 0;
            double derivative = 0;

            while ((moveCiclesCount < 15) || (Math.Abs(error) > 1))
            {
                double gyroValue = getGyro();
                error = gyroValue - gyroOffset;
                integral = integral + error;
                derivative = error - lastError;
                double turn = kp * error + Ki * integral + Kd * derivative;
                double powerRight = Tp - turn;
                double powerLeft = Tp + turn;

                moveMoters(powerRight, powerLeft);

                lastError = error;
                moveCiclesCount++;
            }
        }

        private void moveMoters(double powerRight, double powerLeft)
        {
            if (powerRight >= 0)
            {
                _brick.DirectCommand.SetMotorPolarityAsync(Settings.rightMotorPort, Polarity.Forward);
                Thread.Sleep(2);
            }
            else
            {
                _brick.DirectCommand.SetMotorPolarityAsync(Settings.rightMotorPort, Polarity.Backward);
                Thread.Sleep(2);
                powerRight = powerRight * -1;
            }

            if (powerRight > 100)
            {
                powerRight = 100;
            }

            if (powerLeft >= 0)
            {
                _brick.DirectCommand.SetMotorPolarityAsync(Settings.leftMotorPort, Polarity.Forward);
                Thread.Sleep(2);
            }
            else
            {
                _brick.DirectCommand.SetMotorPolarityAsync(Settings.leftMotorPort, Polarity.Backward);
                Thread.Sleep(2);
                powerLeft = powerLeft * -1;
            }

            if (powerLeft > 100)
            {
                powerLeft = 100;
            }

            _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(Settings.rightMotorPort, (int)powerRight, MotorMoveTimeMs, true);
            Thread.Sleep(2);
            _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(Settings.leftMotorPort, (int)powerLeft, MotorMoveTimeMs, true);
            Thread.Sleep(2);
            Thread.Sleep((int)MotorMoveTimeMs);
        }

        public void RotationScan(SensorFusion sensors, ParticleFilter Pfilter, Bitmap map)
        {
            Bitmap bitmapClone = (Bitmap)map.Clone();

            for (uint rotationDegCount = 0; rotationDegCount < 360; rotationDegCount += RotaionDegStep)
            {
                var value = sensors.Read();
                Console.WriteLine(value);
                Pfilter.Resample(value);

                PIDTurn(RotaionDegStep);
                Pfilter.TurnParticlesLeft(RotaionDegStep);

                ((MainWindow)System.Windows.Application.Current.MainWindow).Image.Source = bitmapClone.Drawparticles(Pfilter.ParticleSet);
            }

            PIDMove(10);
            Pfilter.MoveParticles(10);
            ((MainWindow)System.Windows.Application.Current.MainWindow).Image.Source = bitmapClone.Drawparticles(Pfilter.ParticleSet);
        }

        public void ExecuteCommands()
        {
            _brick_BrickChanged(this, new BrickChangedEventArgs());
        }


        private void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            if (Commands.Count <= 0)
                return;
            var com = Commands.Dequeue();
            com();
        }
    }
}
