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
        static Sensor Sonar;
        static Sensor Infrared;

        static void Main(string[] args)
        {
            //Init
            var brick = new Brick(new BluetoothCommunication("COM11"), true);
            Connect(brick).Wait();
            System.Threading.Thread.Sleep(100);
            Sonar = new Sensor(brick, true);
            Infrared = new Sensor(brick);

            //DO stuff
            //Move(Motion.Front, brick, 50, 5000).Wait();
            RotationScan(brick).Wait();


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
                Environment.Exit(0);
            }
        }

        private static async Task RotationScan(Brick _brick)
        {
            var SonarData = new List<uint>();
            var InfraredData = new List<uint>();
            uint rotationTime = 1900;

            for (uint i = 0; i <= rotationTime; i += rotationTime/10)
            {
                await Move(Motion.Right, _brick, 50, rotationTime/10);
                System.Threading.Thread.Sleep((int)rotationTime / 10);
                SonarData.Add(Sonar.Read());
                InfraredData.Add(Infrared.Read());
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


        private static async Task Move(Motion motion, Brick _brick, int _power, uint _ms)
        {
            if (motion == Motion.Right)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Forward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Backward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
            else if (motion == Motion.Left)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Backward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A, _power, _ms, false);
            }
            else if (motion == Motion.Front)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Forward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Forward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.D, _power, _ms, false);
            }
            else if (motion == Motion.Back)
            {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Backward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.D, Polarity.Backward);
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
