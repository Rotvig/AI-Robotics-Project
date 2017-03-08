using System.Threading;
using Lego.Ev3.Core;

namespace AI_In_Robotics.Classes
{
    public class Sensor
    {
        private int calibrationAddition;
        private int calibrationMultiplyer;
        private readonly Brick _brick;
        private readonly InputPort _port;

        public Sensor(Brick brick, bool sonar = false)
        {
            _brick = brick;
            if (sonar)
            {
                calibrationAddition = Settings.sonarCalibrationAddition;
                calibrationMultiplyer = Settings.sonarCalibrationMultiplyer;
                _port = Settings.sonarPort;
                //_brick.Ports[_port].SetMode(InfraredMode.Proximity);
            }
            else
            {
                calibrationAddition = Settings.infraredCalibrationAddition;
                calibrationMultiplyer = Settings.infraredCalibrationMultiplyer;
                _port = Settings.infraredPort;
                //_brick.Ports[_port].SetMode(UltrasonicMode.SiCentimeters);
            }
        }

        public uint Read()
        {
            Thread.Sleep(20);
            return (uint) _brick.Ports[_port].SIValue - (uint) calibrationAddition;
        }
    }
}
