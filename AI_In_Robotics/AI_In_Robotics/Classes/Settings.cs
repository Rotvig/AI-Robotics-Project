using Lego.Ev3.Core;

namespace AI_In_Robotics.Classes
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
