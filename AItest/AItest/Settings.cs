using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Desktop;
using Lego.Ev3.Core;

namespace AItest
{
    public static class Settings
    {
        //Sonar
        public const InputPort sonarPort = InputPort.Four;
        public const int sonarCalibrationAddition = 1;
        public const int sonarCalibrationMultiplyer = 1;
        //Infrared
        public const InputPort infraredPort = InputPort.One;
        public const int infraredCalibrationAddition = 1;
        public const int infraredCalibrationMultiplyer = 1;

    }
}
