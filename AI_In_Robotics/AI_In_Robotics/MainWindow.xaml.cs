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
        }

        private async void Ready(object sender, RoutedEventArgs e)
        {
            brick = new Brick(new BluetoothCommunication("COM9"), true); // Jeppe
            //brick = new Brick(new BluetoothCommunication("COM3"), true); // Kim1
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

            filter = new ParticleFilter(N, World, 220, 35, 180);

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = World.GetAStarRoadMap(fromX, fromY, toX, toY);

            Astar = new Astar(roadMap, 5, 5, 5);

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
                var position = filter.getPosition();

                while ((position.X - Astar._goalX) > 5 && (position.Y - Astar._goalY) > 5)
                {
                    filter.Resample(Sensors.Read()); // Complete run
                    position = filter.getPosition();

                    BitmapClone = (Bitmap)OriginalBitmap.Clone();
                    Image.Source = BitmapClone.Drawparticles(filter.ParticleSet);

                    Movement movement = Astar.FindPath((int)position.X, (int)position.Y);

                    double orientationTaget = 0;
                    double turnAngle = 0;

                    switch(movement)
                    {
                        case Movement.Right:
                            orientationTaget = 0 * Math.PI;
                            if (position.theta < Math.PI)
                            {
                                turnAngle = (filter.RadToDeg(-position.theta));
                            }
                            else
                            {
                                motionControle.PIDTurn(filter.RadToDeg(2*Math.PI - position.theta));
                            }
                            break;
                        case Movement.Up:
                            orientationTaget = 0.5 * Math.PI;
                            if (position.theta > orientationTaget &&  position.theta < (1.5 * Math.PI))
                            {
                                turnAngle = filter.RadToDeg((0.5 * Math.PI) - position.theta);
                            }
                            else
                            {
                                if(position.theta < Math.PI)
                                {
                                    turnAngle = filter.RadToDeg(0.5 * Math.PI - position.theta);
                                }
                                else
                                {
                                    turnAngle = filter.RadToDeg(0.5 * Math.PI + (position.theta - 1.5 * Math.PI));
                                }
                            }
                            break;
                        case Movement.Left:
                            if (position.theta > orientationTaget && position.theta < (2 * Math.PI))
                            {
                                turnAngle = filter.RadToDeg(Math.PI - position.theta);
                            }
                            else
                            {
                                turnAngle = filter.RadToDeg(Math.PI - position.theta);
                            }
                            break;
                        case Movement.Down:
                            if (position.theta < orientationTaget && position.theta > (0.5 * Math.PI))
                            {
                                turnAngle = -(filter.RadToDeg(1.5 * Math.PI - position.theta));
                            }
                            else
                            {
                                if (position.theta < Math.PI)
                                {
                                    turnAngle = filter.RadToDeg(-0.5 * Math.PI - position.theta);
                                }
                                else
                                {
                                    turnAngle = filter.RadToDeg(1.5 * Math.PI - position.theta);
                                }
                            }
                            break;
                    }

                    motionControle.PIDTurn(turnAngle);
                    filter.TurnParticlesRight(turnAngle);

                    motionControle.PIDMove(2);
                    filter.MoveParticles(2);

                }
            }

        }
    }
}


