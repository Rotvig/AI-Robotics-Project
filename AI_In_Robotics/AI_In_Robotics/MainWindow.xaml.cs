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
        private Motion motionControle;

        static int N = 100000;
        static ParticleFilter filter;
        static Astar Astar;
        static Map World;
        static Bitmap OriginalBitmap;
        static Bitmap BitmapClone;
        private int TestCounter = 0;


        public MainWindow()
        {
            InitializeComponent();

            //For outoutting console in window
            // Instantiate the writer
            //var _writer = new TextBoxStreamWriter(textBox);
            // Redirect the out Console stream
            //Console.SetOut(_writer);

            // Init project classes
            AStartTest();
        }

        private void AStartTest()
        {
            var pathFinding = new Astar();
            var myMap = new Map(20, 20);
            myMap.AddSquare(1, 1, 2, 3, 0);
            myMap.AddSquare(12, 12, 5, 5, 45);

            int fromX = 0, fromY = 19, toX = 19, toY = 19;
            var roadMap = myMap.GetAStarRoadMap(fromX, fromY, toX, toY);
            var endNode = pathFinding.FindPath(roadMap, fromX, fromY, toX, toY, 1);

            var bigMap = Astar.EnLargeObjects(roadMap, 1);

            //pathFinding.PrintPath(endNode, fromX, fromY, toX, toY);
            //myMap.PrintRoadMap(roadMap, endNode, fromX, fromY, toX, toY);
            myMap.PrintRoadMap(bigMap, endNode, fromX, fromY, toX, toY);

        }

        private async void Ready(object sender, RoutedEventArgs e)
        {
            //brick = new Brick(new BluetoothCommunication("COM11"), true); // Jeppe
            brick = new Brick(new BluetoothCommunication("COM3"), true); // Kim1
            //brick = new Brick(new BluetoothCommunication("COM5"), true); // Kim2

            await brick.ConnectAsync();

            await brick.DirectCommand.PlayToneAsync(100, 440, 500);

            Sensors = new SensorFusion(brick);
            motionControle = new Motion(brick);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            World = new Map(258, 129);
            World.AddSquare(68, 24, 24, 32, 0);
            World.AddSquare(48, 94, 19, 12, 0);
            World.AddSquare(133, 96, 16, 24, 0);
            World.AddSquare(160, 27, 36, 14, 0);

            filter = new ParticleFilter(N, World);
            Astar = new Astar();

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = World.GetAStarRoadMap(fromX, fromY, toX, toY);


            OriginalBitmap = new Bitmap(259, 130);
            Image.Source = OriginalBitmap.DrawObjects(roadMap);

            BitmapClone = (Bitmap)OriginalBitmap.Clone();
            Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
        }

        private void KeyEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.C)
            {
                Sensors.CalibrateSensors();
            }

            if (e.Key == System.Windows.Input.Key.P)
            {
                TestCounter++;
                motionControle.RotationScan(Sensors, filter, OriginalBitmap);
            }

            if (e.Key == System.Windows.Input.Key.M)
            {
                motionControle.motionTest();
            }

            if (e.Key == System.Windows.Input.Key.S)
            {
                motionControle.RotationScan(Sensors, filter, OriginalBitmap);
            }



            // specific test
            if (e.Key == System.Windows.Input.Key.H) // Højre
            {
                motionControle.PIDTurn(-45);
                filter.TurnParticlesRight(45);

                //filter.Resample(Sensors.Read());

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }
            if (e.Key == System.Windows.Input.Key.V) // Venstre
            {
                motionControle.PIDTurn(45);
                filter.TurnParticlesRight(45);

                //filter.Resample(Sensors.Read());

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }
            if (e.Key == System.Windows.Input.Key.F) // Fremad
            {
                motionControle.PIDMove(10);
                filter.MoveParticles(10);

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }
            if (e.Key == System.Windows.Input.Key.R) // Resample
            {
                filter.Resample(Sensors.Read());

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }

            if (e.Key == System.Windows.Input.Key.Space)
            {
                while(true)
                {
                    filter.Resample(Sensors.Read()); // Complete run
                    //var position = filter.GetPosition();
                    //var orientation = filter.GetOrientation();

                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);

                    Astar.FindPath();

                }
            }

        }
    }
}


