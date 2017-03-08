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
        private SensorFusion Sensors;

        public MainWindow()
        {
            InitializeComponent();

            //For outoutting console in window
            // Instantiate the writer
            var _writer = new TextBoxStreamWriter(textBox);
            // Redirect the out Console stream
            Console.SetOut(_writer);

            // Init project classes
            var pathFinding = new Astar();
            var myMap = new Map(20, 20);

            myMap.AddSquare(1, 1, 2, 3, 0);
            myMap.AddSquare(12, 12, 5, 5, 45);

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = myMap.GetAStarRoadMap(fromX, fromY, toX, toY);

            var endNode = pathFinding.AStar(roadMap, fromX, fromY, toX, toY);
            pathFinding.PrintPath(endNode, fromX, fromY, toX, toY);

            myMap.PrintRoadMap(roadMap, endNode, fromX, fromY, toX, toY);
        }

        private async void Ready(object sender, RoutedEventArgs e)
        {
            //brick = new Brick(new BluetoothCommunication("COM5"), true);

            brick = new Brick(new BluetoothCommunication("COM11"), true);
       
            await brick.ConnectAsync();

            await brick.DirectCommand.PlayToneAsync(100, 440, 500);

            Sensors = new SensorFusion(brick);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var bitmap = new Bitmap(400, 400);
            Image.Source = bitmap.Drawparticles(new List<Particle>
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

        private void KeyEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                Sensors.CalibrateSensors();
            }
        }
    }
}


