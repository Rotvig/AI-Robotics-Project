using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lego.Ev3.Core;

namespace AI_In_Robotics.Classes
{
    public class Motion
    {
        private readonly Brick _brick;

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
            movePControle(40, 0);
            //movePControle(10, 90);
            //movePControle(10, -90);
            //movePControle(10, -90);
        }

        private void movePControle(double moveDistanceCm, double turnAngle)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter("D:MotorPIDtest.txt");
            var moveCiclesCount = 0;
            var kp = 1;
            var gyroOffset = getGyro() + turnAngle;
            var Tp = 20;

            while(moveCiclesCount < moveDistanceCm/3.2)
            {
                var gyroValue = getGyro();
                //file.WriteLine("gyro value");
                //file.WriteLine(getGyro());
                var error = gyroValue - gyroOffset;
                var turn = kp * error;
                //file.WriteLine("Error");
                //file.WriteLine(error);
                var powerRight = Tp - turn;
                var powerLeft = Tp + turn;
                //file.WriteLine("Right power");
                //file.WriteLine(powerRight);
                //file.WriteLine("Left power");
                //file.WriteLine(powerLeft);

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

                _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(Settings.rightMotorPort, (int)powerRight, 500, true);
                Thread.Sleep(2);
                _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(Settings.leftMotorPort, (int)powerLeft, 500, true);
                Thread.Sleep(2);
                Thread.Sleep(500);

                moveCiclesCount++;
            }
        }

        public void Move(MotionEnum motion, int _power, uint _ms)
        {
            if (motion == MotionEnum.Right)
            {
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Backward));
                Commands.Enqueue(
                    () =>
                        _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms,
                            false));
            }
            else if (motion == MotionEnum.Left)
            {
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Backward));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Forward));
                Commands.Enqueue(
                    () => _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A, _power, _ms, false));
            }
            else if (motion == MotionEnum.Front)
            {
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Forward));
                Commands.Enqueue(
                    () =>
                        _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms,
                            false));
            }
            else if (motion == MotionEnum.Back)
            {
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Backward));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Backward));
                Commands.Enqueue(
                    () =>
                        _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms,
                            false));
            }
        }

        public async Task RotationScan(Brick _brick, Sensor sonar, Sensor infrared)
        {
            throw new ArgumentException("NOT IMPLEMENTED !!!");
            var SonarData = new List<double>();
            var InfraredData = new List<double>();
            uint rotationTime = 1900;

            for (uint i = 0; i <= rotationTime; i += rotationTime/10)
            {
                Move(MotionEnum.Right, 50, rotationTime/10);
                Thread.Sleep((int) rotationTime/10);
                SonarData.Add(sonar.Read());
                InfraredData.Add(infrared.Read());
            }

            foreach (var val in SonarData)
            {
                Console.WriteLine(val);
            }

            Console.WriteLine("----------");

            foreach (var val in InfraredData)
            {
                Console.WriteLine(val);
            }
        }

        public void Turn90Deg(MotionEnum motion)
        {
            if (motion == MotionEnum.Right)
            {
                Commands.Enqueue(() => Thread.Sleep(200));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Backward));
                Commands.Enqueue(
                    () => _brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A | OutputPort.D, 50, 180, false));
            }
            else if (motion == MotionEnum.Left)
            {
                Commands.Enqueue(() => Thread.Sleep(200));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Backward));
                Commands.Enqueue(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Forward));
                Commands.Enqueue(
                    () => _brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A | OutputPort.D, 50, 180, false));
            }
            else
                throw new ArgumentException("Only right or left");
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

    public enum MotionEnum
    {
        Right,
        Left,
        Front,
        Back
    };
}
