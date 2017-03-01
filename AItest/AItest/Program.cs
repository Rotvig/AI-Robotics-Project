﻿using System;
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
            Map myMap = new Map(10, 10);

            myMap.testMap(myMap);

            //Init
<<<<<<< HEAD
            //var brick = new Brick(new BluetoothCommunication("COM11"), true);
            var brick = new Brick(new UsbCommunication(), true);
=======
            /*var brick = new Brick(new BluetoothCommunication("COM11"), true);
>>>>>>> Code for generating map and get distance from position to objects is done
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

<<<<<<< HEAD
            Console.ReadKey();
            brick.Disconnect();
=======
            Console.ReadKey();*/
>>>>>>> Code for generating map and get distance from position to objects is done
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
