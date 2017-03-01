using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Desktop;
using Lego.Ev3.Core;

namespace AItest
{
    public class Motion
    {
        Brick _brick;

        Queue<Action> Commands = new Queue<Action>();

        public Motion(Brick brick)
        {
            _brick = brick;
            _brick.BrickChanged += _brick_BrickChanged;
        }

        public void Move(MotionEnum motion, int _power, uint _ms)
        {
            if (motion == MotionEnum.Right)
            {
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Backward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false)));
            }
            else if (motion == MotionEnum.Left)
            {
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Backward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Forward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A, _power, _ms, false)));
            }
            else if (motion == MotionEnum.Front)
            {
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Forward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false)));
            }
            else if (motion == MotionEnum.Back)
            {
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Backward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Backward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false)));
            }
        }

        public async Task RotationScan(Brick _brick, Sensor sonar, Sensor infrared)
        {
            throw new ArgumentException("NOT IMPLEMENTED !!!");
            var SonarData = new List<uint>();
            var InfraredData = new List<uint>();
            uint rotationTime = 1900;

            for (uint i = 0; i <= rotationTime; i += rotationTime / 10)
            {
                Move(MotionEnum.Right, 50, rotationTime / 10);
                System.Threading.Thread.Sleep((int)rotationTime / 10);
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
                Commands.Enqueue(new Action(() => System.Threading.Thread.Sleep(200)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Backward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A | OutputPort.D, 50, 180, false)));
            }
            else if (motion == MotionEnum.Left)
            {
                Commands.Enqueue(new Action(() => System.Threading.Thread.Sleep(200)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.A, Polarity.Backward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.D, Polarity.Forward)));
                Commands.Enqueue(new Action(() => _brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A | OutputPort.D, 50, 180, false)));
            }
            else
                throw new ArgumentException("Only right or left");
        }

        public void ExecuteCommands()
        {
            _brick_BrickChanged(this, new BrickChangedEventArgs());
        }

        

        void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
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
