using Lego.Ev3.Core;

namespace AI_In_Robotics.Classes
{
    public static class Settings
    {
        //Sonar
        public const InputPort sonarPort = InputPort.Four;
        public const double sonarCalibrationAddition = -8.2;
        public const double sonarCalibrationMultiplyer = 1;
        //Infrared
        public const InputPort infraredPort = InputPort.One;
        public const double infraredCalibrationAddition = 10;
        public const double infraredCalibrationMultiplyer = 1;
        //Gyro
        public const InputPort gyroPort = InputPort.Two;
        //MotorRight
        public const OutputPort rightMotorPort = OutputPort.D;
        //MotorLeft
        public const OutputPort leftMotorPort = OutputPort.A;
    }
}
