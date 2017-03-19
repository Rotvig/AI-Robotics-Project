using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using AI_In_Robotics.Classes;
using AI_In_Robotics.Utils;
using Lego.Ev3.Core;
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

        private static readonly int N = 100000;
        private static ParticleFilter filter;
        private static Astar Astar;
        private static Map World;
        private static Bitmap OriginalBitmap;
        private static Bitmap BitmapClone;
        private int TestCounter;


        public MainWindow()
        {
            InitializeComponent();

            //For outoutting console in window
            // Instantiate the writer
            //var _writer = new TextBoxStreamWriter(textBox);
            // Redirect the out Console stream
            //Console.SetOut(_writer);

            // Init project classes
        }

        private async void Ready(object sender, RoutedEventArgs e)
        {
            //brick = new Brick(new BluetoothCommunication("COM9"), true); // Jeppe
            //brick = new Brick(new BluetoothCommunication("COM3"), true); // Kim1
            brick = new Brick(new BluetoothCommunication("COM5"), true); // Kim2

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

            filter = new ParticleFilter(N, World, 220, 35, 180);

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = World.GetAStarRoadMap(fromX, fromY, toX, toY);

            Astar = new Astar(roadMap, 5, 5, 10);

            OriginalBitmap = new Bitmap(259, 130);
            Image.Source = OriginalBitmap.DrawObjects(roadMap);

            BitmapClone = (Bitmap)OriginalBitmap.Clone();
            Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
        }

        private void KeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                Sensors.CalibrateSensors();
            }

            if (e.Key == Key.P)
            {
                TestCounter++;
                motionControle.RotationScan(Sensors, filter, OriginalBitmap);
            }

            if (e.Key == Key.M)
            {
                motionControle.motionTest();
            }

            if (e.Key == Key.S)
            {
                motionControle.RotationScan(Sensors, filter, OriginalBitmap);
            }


            // specific test
            if (e.Key == Key.H) // Højre
            {
                motionControle.PIDTurn(-45);
                filter.TurnParticlesRight(45);

                //filter.Resample(Sensors.Read());

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }
            if (e.Key == Key.V) // Venstre
            {
                motionControle.PIDTurn(45);
                filter.TurnParticlesRight(45);

                //filter.Resample(Sensors.Read());

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }
            if (e.Key == Key.F) // Fremad
            {
                motionControle.PIDMove(10);
                filter.MoveParticles(10);

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }
            if (e.Key == Key.R) // Resample
            {
                filter.Resample(Sensors.Read());

                BitmapClone = (Bitmap)OriginalBitmap.Clone();
                Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);
            }

            if (e.Key == Key.Space)
            {
                new Thread(StartRobot).Start();
            }
        }

        private void StartRobot()
        {
            var position = filter.getPosition();
            var dontDoAnything = false;
            while ((position.X - Astar._goalX) > 5 && (position.Y - Astar._goalY) > 5)
            {
                filter.Resample(Sensors.Read()); // Complete run
                position = filter.getPosition();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    BitmapClone.Drawparticles(filter.ParticleSet);

                    Image.Source = BitmapClone.DrawRobotPos(position);
                }
                    ));

                var movement = Astar.FindPath((int)position.X, (int)position.Y);


                if (dontDoAnything)
                {
                    dontDoAnything = false;
                }
                else
                {
                    var turnThisMuch = motionControle.TurnCommand(movement, filter.RadToDeg(position.theta));
                    if (turnThisMuch < 0)
                    {
                        filter.TurnParticlesRight(Math.Abs(turnThisMuch));
                    }
                    else
                    {
                        filter.TurnParticlesLeft(turnThisMuch);
                    }
                    motionControle.PIDMove(2);
                    filter.MoveParticles(2);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        BitmapClone = (Bitmap)OriginalBitmap.Clone();
                        BitmapClone.Drawparticles(filter.ParticleSet);

                        Image.Source = BitmapClone.DrawRobotPos(position);
                    }
                        ));
                }
            }
        }
    }
}


