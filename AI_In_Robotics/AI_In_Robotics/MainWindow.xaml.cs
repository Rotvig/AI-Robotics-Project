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

        static int N = 1000;
        static ParticleFilter filter;
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
            var endNode = pathFinding.AStar(roadMap, fromX, fromY, toX, toY, 1);

            var bigMap = Astar.EnLargeObjects(roadMap, 1);

            //pathFinding.PrintPath(endNode, fromX, fromY, toX, toY);
            //myMap.PrintRoadMap(roadMap, endNode, fromX, fromY, toX, toY);
            myMap.PrintRoadMap(bigMap, endNode, fromX, fromY, toX, toY);

        }

        private async void Ready(object sender, RoutedEventArgs e)
        {
            //brick = new Brick(new BluetoothCommunication("COM5"), true);

            brick = new Brick(new BluetoothCommunication("COM11"), true);
       
            await brick.ConnectAsync();

            await brick.DirectCommand.PlayToneAsync(100, 440, 500);

            Sensors = new SensorFusion(brick);
            motionControle = new Motion(brick);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            World = new Map(152, 123);
            filter = new ParticleFilter(N, World);

            //World.AddSquare(1, 1, 2, 3, 0);
            //World.AddSquare(12, 12, 5, 5, 45);

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = World.GetAStarRoadMap(fromX, fromY, toX, toY);


            OriginalBitmap = new Bitmap(153, 124);
            Image.Source = OriginalBitmap.DrawObjects(roadMap);

            BitmapClone = (Bitmap)OriginalBitmap.Clone();
            Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
        }

        private void KeyEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                Sensors.CalibrateSensors();
            }

            if (e.Key == System.Windows.Input.Key.P)
            {
                TestCounter++;
                if (TestCounter == 1)
                {
                    filter.Resample(250);
                    filter.Resample(250);
                    filter.TurnParticlesRight(90);
                    filter.Resample(50);
                    filter.Resample(50);
                    filter.TurnParticlesRight(90);
                    filter.Resample(150);
                    filter.Resample(150);
                    filter.TurnParticlesRight(90);
                    filter.Resample(350);
                    filter.Resample(350);
                    filter.TurnParticlesRight(90);
                    filter.Resample(250);
                    filter.Resample(250);

                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
                }
                if (TestCounter == 2)
                {
                    filter.MoveParticles(25);
                    filter.Resample(225);
                    filter.Resample(225);

                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
                }
                if (TestCounter == 3)
                {
                    filter.MoveParticles(50);
                    filter.Resample(175);
                    filter.Resample(175);

                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
                }
                if (TestCounter == 4)
                {
                    filter.TurnParticlesRight(90);
                    filter.Resample(50);
                    filter.Resample(50);
                    filter.TurnParticlesRight(90);
                    filter.Resample(225);
                    filter.Resample(225);
                    filter.TurnParticlesRight(90);
                    filter.Resample(350);
                    filter.Resample(350);
                    filter.TurnParticlesRight(90);
                    filter.Resample(175);
                    filter.Resample(175);

                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
                }
            }

            if (e.Key == System.Windows.Input.Key.M)
            {
                motionControle.motionTest();
            }

            if (e.Key == System.Windows.Input.Key.S)
            {
                motionControle.RotationScan(Sensors, filter, OriginalBitmap);
            }
        }
    }
}


