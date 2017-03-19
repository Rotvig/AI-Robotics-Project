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
        private const uint RotaionDegStep = 360 / 12;
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
            PIDMove(5);
            //PIDTurn(90);
            //PIDTurn(90);
            //PIDTurn(90);
            //PIDTurn(90);
        }

        public void PIDMove(double moveDistanceCm)
        {
            System.IO.StreamWriter fileError = new System.IO.StreamWriter("D:MotorPIDerror.txt");
            System.IO.StreamWriter fileRightPower = new System.IO.StreamWriter("D:MotorRightPower.txt");
            System.IO.StreamWriter fileLeftPower = new System.IO.StreamWriter("D:MotorLeftPower.txt");
            System.IO.StreamWriter fileTime = new System.IO.StreamWriter("D:MotorPIDtime.txt");
            double moveCiclesCount = 0;
            double kp = 1.2;
            double Ki = 0.40;
            double Kd = 1.2;
            double gyroTargetValue = getGyro();
            double movePower = 0;
            if (moveDistanceCm > 0)
            {
                movePower = 25;
            }
            else
            {
                movePower = -25;
            }
            double integral = 0;
            double lastError = 0;
            double derivative = 0;

            while (moveCiclesCount < moveDistanceCm/MoveDistancePerCicle)
            {
                double gyroValue = getGyro();
                double error = gyroValue - gyroTargetValue;
                integral = integral + error;
                derivative = error - lastError;
                double turnPower = kp * error + Ki * integral + Kd * derivative;
                double powerRight = movePower - turnPower;
                double powerLeft = movePower + turnPower;

                moveMoters(powerRight, powerLeft);

                fileError.WriteLine(error);
                fileRightPower.WriteLine(powerRight);
                fileLeftPower.WriteLine(powerLeft);
                fileTime.WriteLine(moveCiclesCount);
                lastError = error;
                moveCiclesCount++;
            }

            fileError.Close();
            fileRightPower.Close();
            fileLeftPower.Close();
            fileTime.Close();
        }

        public void TurnCommand(Movement movement, double angleDeg)
        {
            double orientationTaget = 0;
            double turnAngle = 0;

            switch (movement)
            {
                case Movement.Right:
                    orientationTaget = 0;
                    if (angleDeg < 180)
                    {
                        turnAngle = -angleDeg;
                    }
                    else
                    {
                        PIDTurn(180 - angleDeg);
                    }
                    break;
                case Movement.Up:
                    orientationTaget = 90;
                    if (angleDeg > orientationTaget && angleDeg < 270)
                    {
                        turnAngle = 90 - angleDeg;
                    }
                    else
                    {
                        if (angleDeg < 180)
                        {
                            turnAngle = 90 - angleDeg;
                        }
                        else
                        {
                            turnAngle = 90 + (360 - angleDeg);
                        }
                    }
                    break;
                case Movement.Left:
                    orientationTaget = 180;
                    if (angleDeg > orientationTaget)
                    {
                        turnAngle = 180 - angleDeg;
                    }
                    else
                    {
                        turnAngle = 180 - angleDeg;
                    }
                    break;
                case Movement.Down:
                    orientationTaget = 270;
                    if (angleDeg < orientationTaget && angleDeg > 90)
                    {
                        turnAngle = (270 - angleDeg);
                    }
                    else
                    {
                        if (angleDeg < 180)
                        {
                            turnAngle = -90 - angleDeg;
                        }
                        else
                        {
                            turnAngle = 270 - angleDeg;
                        }
                    }
                    break;
            }

            if (Math.Abs(turnAngle) > 5)
            {
                if (turnAngle > 90)
                {
                    PIDTurn(90);
                    PIDTurn(turnAngle - 90);
                }
                else if (turnAngle < -90)
                {
                    PIDTurn(-90);
                    PIDTurn(turnAngle + 90);
                }
                else
                {
                    PIDTurn(turnAngle);
                }
            }
        }

        public void PIDTurn(double rotateDeg)
        {
            System.IO.StreamWriter fileError = new System.IO.StreamWriter("D:MotorPIDerror.txt");
            System.IO.StreamWriter fileRightPower = new System.IO.StreamWriter("D:MotorRightPower.txt");
            System.IO.StreamWriter fileLeftPower = new System.IO.StreamWriter("D:MotorLeftPower.txt");
            System.IO.StreamWriter fileTime = new System.IO.StreamWriter("D:MotorPIDtime.txt");
            double moveCiclesCount = 0;
            double kp = 1.2;
            double Ki = 0.40;
            double Kd = 1.2;
            double gyroTargetValue = getGyro() + rotateDeg;
            double movePower = 0;
            double integral = 0;
            double error = rotateDeg;
            double lastError = 0;
            double derivative = 0;

            while ((moveCiclesCount < 20) || (Math.Abs(error) > 2))
            {
                double gyroValue = getGyro();
                error = gyroValue - gyroTargetValue;
                integral = integral + error;
                derivative = error - lastError;
                double turnPower = kp * error + Ki * integral + Kd * derivative;
                double powerRight = movePower - turnPower;
                double powerLeft = movePower + turnPower;

                moveMoters(powerRight, powerLeft);

                fileError.WriteLine(error);
                fileRightPower.WriteLine(powerRight);
                fileLeftPower.WriteLine(powerLeft);
                fileTime.WriteLine(moveCiclesCount);
                lastError = error;
                moveCiclesCount++;
            }

            fileError.Close();
            fileRightPower.Close();
            fileLeftPower.Close();
            fileTime.Close();
            Console.WriteLine("Turn done");
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
