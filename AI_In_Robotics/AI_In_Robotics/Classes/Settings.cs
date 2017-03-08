using Lego.Ev3.Core;

namespace AI_In_Robotics.Classes
{
    public static class Settings
    {
        //Sonar
        public const InputPort sonarPort = InputPort.Four;
        public const double sonarCalibrationAddition = 0;
        public const double sonarCalibrationMultiplyer = 1;
        //Infrared
        public const InputPort infraredPort = InputPort.One;
        public const double infraredCalibrationAddition = 0;
        public const double infraredCalibrationMultiplyer = 1;
    }
}
