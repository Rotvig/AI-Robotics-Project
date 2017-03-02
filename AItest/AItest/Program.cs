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
        static Motion Car;

        static void Main(string[] args)
        {
            var pathFinding = new Astar();
            Map myMap = new Map(10, 10);

            myMap.AddSquare(3, 3, 2, 2, 0);

            int fromX = 0, fromY = 1, toX = 0, toY = 2;
            var roadMap = myMap.GetAStarRoadMap(1, fromX, fromY, toX, toY);

            var endNode = pathFinding.AStar(roadMap, fromX, fromY, toX, toY);
            pathFinding.PrintPath(endNode, fromX, fromY, toX, toY);

            //Init
            //var brick = new Brick(new BluetoothCommunication("COM11"), true);
            /*var brick = new Brick(new UsbCommunication(), true);
            Connect(brick).Wait();
            System.Threading.Thread.Sleep(100);
            Sonar = new Sensor(brick, true);
            //Infrared = new Sensor(brick);
            Car = new Motion(brick);

            //DO stuff
            //Move(Motion.Front, brick, 50, 5000).Wait();
            //Car.RotationScan(brick, Sonar, Infrared).Wait();
            Car.Turn90Deg(MotionEnum.Left, brick).Wait();
            System.Threading.Thread.Sleep(2000);
            Car.Move(MotionEnum.Front, brick, 100, 500).Wait();

            
            brick.Disconnect();
            Console.ReadKey();*/

            Console.ReadKey();
        }

        private static async Task Connect(Brick _brick)
        {
            try
            {
                await _brick.ConnectAsync();
                System.Threading.Thread.Sleep(100);
                //await _brick.DirectCommand.PlayToneAsync(100, 440, 500);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect");
                Environment.Exit(0);
            }
        }
    }
}
