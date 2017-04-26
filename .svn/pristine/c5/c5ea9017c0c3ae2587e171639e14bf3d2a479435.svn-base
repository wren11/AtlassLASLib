using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.2 PDRF3
    /// </summary>
    public class TcLasPoint3 : TcLasPoint2
    {
        [DisplayName("GPS Time")]
        public Double GPSTime { get; protected set; }

        public TcLasPoint3() { }

        protected TcLasPoint3(TsLasHeader12 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, Byte prmClassification, Byte prmUserData,
            UInt16 prmPointSourceID, Byte prmBitMask, SByte prmScanAngleRank,
            UInt16 prmRed, UInt16 prmGreen, UInt16 prmBlue, Double prmGPSTime)
            : base(prmHeader, prmX, prmY, prmZ, prmIntensity, prmClassification,
            prmUserData, prmPointSourceID, prmBitMask, prmScanAngleRank, prmRed, prmGreen, prmBlue)
        {
            GPSTime = prmGPSTime;
        }

        public TcLasPoint3(TsLasPoint3 prmPoint, TsLasHeader12 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z, prmPoint.Intensity, prmPoint.Classification,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.BitMask, prmPoint.ScanAngleRank,
            prmPoint.Red, prmPoint.Green, prmPoint.Blue, prmPoint.GPSTime)
        {
        }
    }
}
