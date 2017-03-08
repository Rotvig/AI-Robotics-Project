using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AI_In_Robotics.Classes;
using AI_In_Robotics.Utils;
using Lego.Ev3.Core;
using MathNet.Spatial.Euclidean;
using Lego.Ev3.Desktop;

namespace AI_In_Robotics
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Brick brick;   

        public MainWindow()
        {
            InitializeComponent();

            /* For outoutting console in window
            // Instantiate the writer
            var _writer = new TextBoxStreamWriter(textBox);
            // Redirect the out Console stream
            Console.SetOut(_writer);
            */
            var pathFinding = new Astar();
            var myMap = new Map(20, 20);

            myMap.AddSquare(1, 1, 2, 3, 0);
            myMap.AddSquare(12, 12, 5, 5, 45);

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = myMap.GetAStarRoadMap(fromX, fromY, toX, toY);


            var endNode = pathFinding.AStar(roadMap, fromX, fromY, toX, toY);
            pathFinding.PrintPath(endNode, fromX, fromY, toX, toY);

            myMap.PrintRoadMap(roadMap, endNode, fromX, fromY, toX, toY);

            //Init
            
            //SensorFusion Sensoes = new SensorFusion(brick);

            //Sensoes.Read();

            //Sensoes.Read();

            //Sensoes.Read();

            //Sensoes.Read();

            //Sensoes.Read();

            //Sensoes.Read();

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
        }

        private async void Ready(object sender, RoutedEventArgs e)
        {
            brick = new Brick(new BluetoothCommunication("COM5"), true);
       
            await brick.ConnectAsync();

            await brick.DirectCommand.PlayToneAsync(100, 440, 500);
        }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var bitmap = new Bitmap(400, 400);
            Image.Source = bitmap.DrawPixels(new List<Particle>
            {
                new Particle
                {
                    pos = new Point2D(5, 5)
                },
                new Particle
                {
                    pos = new Point2D(0, 0)
                },
                new Particle
                {
                    pos = new Point2D(50, 50)
                }
            });
    }
}


