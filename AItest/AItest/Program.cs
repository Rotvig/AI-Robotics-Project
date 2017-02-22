using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Desktop;
using Lego.Ev3.Core;

namespace AItest
{
    class Program
    {
        static void Main(string[] args)
        {
            var brick = new Brick(new BluetoothCommunication("COM3"), true);
            var con = Connect(brick);
            con.Wait();

            Console.ReadKey();
        }

        private static async Task Connect(Brick _brick)
        {
            try
            {
                await _brick.ConnectAsync();
                await _brick.DirectCommand.PlayToneAsync(100, 440, 500);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect");
            }
        }

        private static async Task Move(Motion motion, Brick _brick, int _power, uint _ms)
        {
            if(motion == Motion.Right)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.D, _power, _ms, false);
            }
            else if(motion == Motion.Left)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A, _power, _ms, false);
            }
            else if(motion == Motion.Front)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
            else if (motion == Motion.Back)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.D, Polarity.Backward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
        }

        enum Motion
        {
            Right,
            Left,
            Front,
            Back
        };
    }
}
