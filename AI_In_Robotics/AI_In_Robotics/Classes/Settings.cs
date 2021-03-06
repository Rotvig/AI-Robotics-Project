﻿using Lego.Ev3.Core;

namespace AI_In_Robotics.Classes
{
    public static class Settings
    {
        //Sonar
        public const InputPort sonarPort = InputPort.One;
        public const double sonarCalibrationAddition = -8.191;
        public const double sonarCalibrationMultiplyer = 1;
        public const double sonarVariance = 0.12;
        //Infrared
        public const InputPort infraredPort = InputPort.Four;
        public const double infraredCalibrationAddition = 10.607;
        public const double infraredCalibrationMultiplyer = 1;
        public const double infraredVariance = 0.52;
        //Gyro
        public const InputPort gyroPort = InputPort.Three;
        //MotorRight
        public const OutputPort rightMotorPort = OutputPort.D;
        //MotorLeft
        public const OutputPort leftMotorPort = OutputPort.A;
    }
}
