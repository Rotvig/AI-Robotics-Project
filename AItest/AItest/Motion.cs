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

        public Motion(Brick brick)
        {
            _brick = brick;
        }

        public async Task Move(MotionEnum motion, Brick _brick, int _power, uint _ms)
        {
            if (motion == MotionEnum.Right)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Forward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Backward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
            else if (motion == MotionEnum.Left)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Backward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A, _power, _ms, false);
            }
            else if (motion == MotionEnum.Front)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Forward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
            else if (motion == MotionEnum.Back)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Backward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Backward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
        }

        public async Task RotationScan(Brick _brick, Sensor sonar, Sensor infrared)
        {
            var SonarData = new List<uint>();
            var InfraredData = new List<uint>();
            uint rotationTime = 1900;

            for (uint i = 0; i <= rotationTime; i += rotationTime / 10)
            {
                await Move(MotionEnum.Right, _brick, 50, rotationTime / 10);
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
    }

    public enum MotionEnum
    {
        Right,
        Left,
        Front,
        Back
    };
}
