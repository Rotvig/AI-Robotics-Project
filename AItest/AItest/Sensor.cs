using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Desktop;
using Lego.Ev3.Core;

namespace AItest
{
    public class Sensor
    {
        int calibrationAddition;
        int calibrationMultiplyer;
        Brick _brick;
        InputPort _port;

        public Sensor(Brick brick, bool sonar = false)
        {
            _brick = brick;
            if(sonar)
            {
                calibrationAddition = Settings.sonarCalibrationAddition;
                calibrationMultiplyer = Settings.sonarCalibrationMultiplyer;
                _port = Settings.sonarPort;
            }
            else
            {
                calibrationAddition = Settings.infraredCalibrationAddition;
                calibrationMultiplyer = Settings.infraredCalibrationMultiplyer;
                _port = Settings.infraredPort;
            }
        }

        public uint Read()
        {
            System.Threading.Thread.Sleep(20);
            return (uint)_brick.Ports[_port].RawValue;
        }
    }
}
